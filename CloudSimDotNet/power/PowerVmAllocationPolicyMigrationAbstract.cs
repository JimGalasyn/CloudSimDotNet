using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power
{


    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using PowerVmList = org.cloudbus.cloudsim.power.lists.PowerVmList;
    using ExecutionTimeMeasurer = org.cloudbus.cloudsim.util.ExecutionTimeMeasurer;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// An abstract power-aware VM allocation policy that dynamically optimizes the VM
    /// allocation (placement) using migration.
    /// 
    /// <br/>If you are using any algorithms, policies or workload included in the power package please cite
    /// the following paper:<br/>
    /// 
    /// <ul>
    /// <li><a href="http://dx.doi.org/10.1002/cpe.1867">Anton Beloglazov, and Rajkumar Buyya, "Optimal Online Deterministic Algorithms and Adaptive
    /// Heuristics for Energy and Performance Efficient Dynamic Consolidation of Virtual Machines in
    /// Cloud Data Centers", Concurrency and Computation: Practice and Experience (CCPE), Volume 24,
    /// Issue 13, Pages: 1397-1420, John Wiley & Sons, Ltd, New York, USA, 2012</a>
    /// </ul>
    /// 
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 3.0
    /// </summary>
    public abstract class PowerVmAllocationPolicyMigrationAbstract : PowerVmAllocationPolicyAbstract
	{

		/// <summary>
		/// The vm selection policy. </summary>
		private PowerVmSelectionPolicy vmSelectionPolicy;

		/// <summary>
		/// A list of maps between a VM and the host where it is place.
		/// @todo This list of map is implemented in the worst way.
		/// It should be used just a Map<Vm, Host> to find out 
		/// what PM is hosting a given VM.
		/// </summary>
		private readonly IList<IDictionary<string, object>> savedAllocation = new List<IDictionary<string, object>>();

		/// <summary>
		/// A map of CPU utilization history (in percentage) for each host,
		///        where each key is a host id and each value is the CPU utilization percentage history.
		/// </summary>
		private readonly IDictionary<int?, IList<double?>> utilizationHistory = new Dictionary<int?, IList<double?>>();

		/// <summary>
		/// The metric history. 
		/// @todo the map stores different data. Sometimes it stores the upper threshold,
		/// other it stores utilization threshold or predicted utilization, that
		/// is very confusing.
		/// </summary>
		private readonly IDictionary<int?, IList<double?>> metricHistory = new Dictionary<int?, IList<double?>>();

		/// <summary>
		/// The time when entries in each history list was added. 
		/// All history lists are updated at the same time.
		/// </summary>
		private readonly IDictionary<int?, IList<double?>> timeHistory = new Dictionary<int?, IList<double?>>();

		/// <summary>
		/// The history of time spent in VM selection 
		/// every time the optimization of VM allocation method is called. </summary>
		/// <seealso cref= #optimizeAllocation(java.util.List)  </seealso>
		private readonly IList<double?> executionTimeHistoryVmSelection = new List<double?>();

		/// <summary>
		/// The history of time spent in host selection 
		/// every time the optimization of VM allocation method is called. </summary>
		/// <seealso cref= #optimizeAllocation(java.util.List)  </seealso>
		private readonly IList<double?> executionTimeHistoryHostSelection = new List<double?>();

		/// <summary>
		/// The history of time spent in VM reallocation 
		/// every time the optimization of VM allocation method is called. </summary>
		/// <seealso cref= #optimizeAllocation(java.util.List)  </seealso>
		private readonly IList<double?> executionTimeHistoryVmReallocation = new List<double?>();

		/// <summary>
		/// The history of total time spent in every call of the 
		/// optimization of VM allocation method. </summary>
		/// <seealso cref= #optimizeAllocation(java.util.List)  </seealso>
		private readonly IList<double?> executionTimeHistoryTotal = new List<double?>();

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationAbstract.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		public PowerVmAllocationPolicyMigrationAbstract(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy) : base(hostList)
		{
			VmSelectionPolicy = vmSelectionPolicy;
		}

		/// <summary>
		/// Optimize allocation of the VMs according to current utilization.
		/// </summary>
		/// <param name="vmList"> the vm list
		/// </param>
		/// <returns> the array list< hash map< string, object>> </returns>
		public override IList<IDictionary<string, object>> optimizeAllocation(IList<Vm> vmList)
		{
			ExecutionTimeMeasurer.start("optimizeAllocationTotal");

			ExecutionTimeMeasurer.start("optimizeAllocationHostSelection");
			IList<PowerHostUtilizationHistory> overUtilizedHosts = OverUtilizedHosts;
			ExecutionTimeHistoryHostSelection.Add(ExecutionTimeMeasurer.end("optimizeAllocationHostSelection"));

			printOverUtilizedHosts(overUtilizedHosts);

			saveAllocation();

			ExecutionTimeMeasurer.start("optimizeAllocationVmSelection");
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.cloudbus.cloudsim.Vm> vmsToMigrate = getVmsToMigrateFromHosts(overUtilizedHosts);
			IList<Vm> vmsToMigrate = getVmsToMigrateFromHosts(overUtilizedHosts);
			ExecutionTimeHistoryVmSelection.Add(ExecutionTimeMeasurer.end("optimizeAllocationVmSelection"));

			Log.printLine("Reallocation of VMs from the over-utilized hosts:");
			ExecutionTimeMeasurer.start("optimizeAllocationVmReallocation");
			IList<IDictionary<string, object>> migrationMap = getNewVmPlacement(vmsToMigrate, new HashSet<Host>(overUtilizedHosts));
			ExecutionTimeHistoryVmReallocation.Add(ExecutionTimeMeasurer.end("optimizeAllocationVmReallocation"));
			Log.printLine();

			((List<IDictionary<string, object>>)migrationMap).AddRange(getMigrationMapFromUnderUtilizedHosts(overUtilizedHosts));

			restoreAllocation();

			ExecutionTimeHistoryTotal.Add(ExecutionTimeMeasurer.end("optimizeAllocationTotal"));

			return migrationMap;
		}

		/// <summary>
		/// Gets the migration map from under utilized hosts.
		/// </summary>
		/// <param name="overUtilizedHosts"> the over utilized hosts </param>
		/// <returns> the migration map from under utilized hosts </returns>
		protected internal virtual IList<IDictionary<string, object>> getMigrationMapFromUnderUtilizedHosts(IList<PowerHostUtilizationHistory> overUtilizedHosts)
		{
			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();
			IList<PowerHost> switchedOffHosts = SwitchedOffHosts;

			// over-utilized hosts + hosts that are selected to migrate VMs to from over-utilized hosts
			ISet<PowerHost> excludedHostsForFindingUnderUtilizedHost = new HashSet<PowerHost>();
            // TEST: (fixed) UnionWith == addAll?
            excludedHostsForFindingUnderUtilizedHost.UnionWith(overUtilizedHosts);
			excludedHostsForFindingUnderUtilizedHost.UnionWith(switchedOffHosts);
			excludedHostsForFindingUnderUtilizedHost.UnionWith(extractHostListFromMigrationMap(migrationMap));

			// over-utilized + under-utilized hosts
			ISet<PowerHost> excludedHostsForFindingNewVmPlacement = new HashSet<PowerHost>();
			excludedHostsForFindingNewVmPlacement.UnionWith(overUtilizedHosts);
			excludedHostsForFindingNewVmPlacement.UnionWith(switchedOffHosts);

			int numberOfHosts = HostListProperty.Count;

			while (true)
			{
				if (numberOfHosts == excludedHostsForFindingUnderUtilizedHost.Count)
				{
					break;
				}

                // TEST: (fixed) Does this HashSet conversion work?
                var fubar = excludedHostsForFindingUnderUtilizedHost.ToList();
                var set = new HashSet<Host>(fubar);
                PowerHost underUtilizedHost = getUnderUtilizedHost(set);
                //PowerHost underUtilizedHost = getUnderUtilizedHost(excludedHostsForFindingUnderUtilizedHost);
                if (underUtilizedHost == null)
				{
					break;
				}

				Log.printConcatLine("Under-utilized host: host #", underUtilizedHost.Id, "\n");

				excludedHostsForFindingUnderUtilizedHost.Add(underUtilizedHost);
				excludedHostsForFindingNewVmPlacement.Add(underUtilizedHost);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<? extends org.cloudbus.cloudsim.Vm> vmsToMigrateFromUnderUtilizedHost = getVmsToMigrateFromUnderUtilizedHost(underUtilizedHost);
				IList<Vm> vmsToMigrateFromUnderUtilizedHost = getVmsToMigrateFromUnderUtilizedHost(underUtilizedHost);
				if (vmsToMigrateFromUnderUtilizedHost.Count == 0)
				{
					continue;
				}

				Log.print("Reallocation of VMs from the under-utilized host: ");
				if (!Log.Disabled)
				{
					foreach (Vm vm in vmsToMigrateFromUnderUtilizedHost)
					{
						Log.print(vm.Id + " ");
					}
				}
				Log.printLine();

                // TEST: (fixed) Does this HashSet conversion work?
                var set1 = new HashSet<Host>(excludedHostsForFindingNewVmPlacement);
                
                IList<IDictionary<string, object>> newVmPlacement = getNewVmPlacementFromUnderUtilizedHost(
                    vmsToMigrateFromUnderUtilizedHost, 
                    set1);

				excludedHostsForFindingUnderUtilizedHost.UnionWith(extractHostListFromMigrationMap(newVmPlacement));

				((List<IDictionary<string, object>>)migrationMap).AddRange(newVmPlacement);
				Log.printLine();
			}

			return migrationMap;
		}

		/// <summary>
		/// Prints the over utilized hosts.
		/// </summary>
		/// <param name="overUtilizedHosts"> the over utilized hosts </param>
		protected internal virtual void printOverUtilizedHosts(IList<PowerHostUtilizationHistory> overUtilizedHosts)
		{
			if (!Log.Disabled)
			{
				Log.printLine("Over-utilized hosts:");
				foreach (PowerHostUtilizationHistory host in overUtilizedHosts)
				{
					Log.printConcatLine("Host #", host.Id);
				}
				Log.printLine();
			}
		}

		/// <summary>
		/// Finds a PM that has enough resources to host a given VM
		/// and that will not be overloaded after placing the VM on it.
		/// The selected host will be that one with most efficient
		/// power usage for the given VM.
		/// </summary>
		/// <param name="vm"> the VM </param>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the host found to host the VM </returns>
		public virtual PowerHost findHostForVm(Vm vm, ISet<Host> excludedHosts)
		{
			double minPower = double.MaxValue;
			PowerHost allocatedHost = null;

			foreach (PowerHost host in this.HostListProperty)
			{
				if (excludedHosts.Contains(host))
				{
					continue;
				}
				if (host.isSuitableForVm(vm))
				{
					if (getUtilizationOfCpuMips(host) != 0 && isHostOverUtilizedAfterAllocation(host, vm))
					{
						continue;
					}

					try
					{
						double powerAfterAllocation = getPowerAfterAllocation(host, vm);
						if (powerAfterAllocation != -1)
						{
							double powerDiff = powerAfterAllocation - host.Power;
							if (powerDiff < minPower)
							{
								minPower = powerDiff;
								allocatedHost = host;
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
			return allocatedHost;
		}

		/// <summary>
		/// Checks if a host will be over utilized after placing of a candidate VM.
		/// </summary>
		/// <param name="host"> the host to verify </param>
		/// <param name="vm"> the candidate vm </param>
		/// <returns> true, if the host will be over utilized after VM placement; false otherwise </returns>
		protected internal virtual bool isHostOverUtilizedAfterAllocation(PowerHost host, Vm vm)
		{
			bool isHostOverUtilizedAfterAllocation = true;
			if (host.vmCreate(vm))
			{
				isHostOverUtilizedAfterAllocation = isHostOverUtilized(host);
				host.vmDestroy(vm);
			}
			return isHostOverUtilizedAfterAllocation;
		}

		public override PowerHost findHostForVm(Vm vm)
		{
			ISet<Host> excludedHosts = new HashSet<Host>();
			if (vm.Host != null)
			{
				excludedHosts.Add(vm.Host);
			}
			return findHostForVm(vm, excludedHosts);
		}

		/// <summary>
		/// Extracts the host list from a migration map.
		/// </summary>
		/// <param name="migrationMap"> the migration map </param>
		/// <returns> the list </returns>
		protected internal virtual IList<PowerHost> extractHostListFromMigrationMap(IList<IDictionary<string, object>> migrationMap)
		{
			IList<PowerHost> hosts = new List<PowerHost>();
			foreach (IDictionary<string, object> map in migrationMap)
			{
				hosts.Add((PowerHost) map["host"]);
			}
			return hosts;
		}

		/// <summary>
		/// Gets a new vm placement considering the list of VM to migrate.
		/// </summary>
		/// <param name="vmsToMigrate"> the list of VMs to migrate </param>
		/// <param name="excludedHosts"> the list of hosts that aren't selected as destination hosts </param>
		/// <returns> the new vm placement map </returns>
		protected internal virtual IList<IDictionary<string, object>> getNewVmPlacement(IList<Vm> vmsToMigrate, ISet<Host> excludedHosts)
		{
			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();
			PowerVmList.sortByCpuUtilization(vmsToMigrate);
			foreach (Vm vm in vmsToMigrate)
			{
				PowerHost allocatedHost = findHostForVm(vm, excludedHosts);
				if (allocatedHost != null)
				{
					allocatedHost.vmCreate(vm);
					Log.printConcatLine("VM #", vm.Id, " allocated to host #", allocatedHost.Id);

					IDictionary<string, object> migrate = new Dictionary<string, object>();
					migrate["vm"] = vm;
					migrate["host"] = allocatedHost;
					migrationMap.Add(migrate);
				}
			}
			return migrationMap;
		}

		/// <summary>
		/// Gets the new vm placement from under utilized host.
		/// </summary>
		/// <param name="vmsToMigrate"> the list of VMs to migrate </param>
		/// <param name="excludedHosts"> the list of hosts that aren't selected as destination hosts </param>
		/// <returns> the new vm placement from under utilized host </returns>
		protected internal virtual IList<IDictionary<string, object>> getNewVmPlacementFromUnderUtilizedHost(IList<Vm> vmsToMigrate, ISet<Host> excludedHosts)
		{
			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();
			PowerVmList.sortByCpuUtilization(vmsToMigrate);
			foreach (Vm vm in vmsToMigrate)
			{
				PowerHost allocatedHost = findHostForVm(vm, excludedHosts);
				if (allocatedHost != null)
				{
					allocatedHost.vmCreate(vm);
					Log.printConcatLine("VM #", vm.Id, " allocated to host #", allocatedHost.Id);

					IDictionary<string, object> migrate = new Dictionary<string, object>();
					migrate["vm"] = vm;
					migrate["host"] = allocatedHost;
					migrationMap.Add(migrate);
				}
				else
				{
					Log.printLine("Not all VMs can be reallocated from the host, reallocation cancelled");
					foreach (IDictionary<string, object> map in migrationMap)
					{
						((Host) map["host"]).vmDestroy((Vm) map["vm"]);
					}
					migrationMap.Clear();
					break;
				}
			}
			return migrationMap;
		}

		/// <summary>
		/// Gets the VMs to migrate from hosts.
		/// </summary>
		/// <param name="overUtilizedHosts"> the over utilized hosts </param>
		/// <returns> the VMs to migrate from hosts </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends org.cloudbus.cloudsim.Vm> getVmsToMigrateFromHosts(java.util.List<PowerHostUtilizationHistory> overUtilizedHosts)
		protected internal virtual IList<Vm> getVmsToMigrateFromHosts(IList<PowerHostUtilizationHistory> overUtilizedHosts)
		{
			IList<Vm> vmsToMigrate = new List<Vm>();
			foreach (PowerHostUtilizationHistory host in overUtilizedHosts)
			{
				while (true)
				{
					Vm vm = VmSelectionPolicy.getVmToMigrate(host);
					if (vm == null)
					{
						break;
					}
					vmsToMigrate.Add(vm);
					host.vmDestroy(vm);
					if (!isHostOverUtilized(host))
					{
						break;
					}
				}
			}
			return vmsToMigrate;
		}

		/// <summary>
		/// Gets the VMs to migrate from under utilized host.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the vms to migrate from under utilized host </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends org.cloudbus.cloudsim.Vm> getVmsToMigrateFromUnderUtilizedHost(PowerHost host)
		protected internal virtual IList<Vm> getVmsToMigrateFromUnderUtilizedHost(PowerHost host)
		{
			IList<Vm> vmsToMigrate = new List<Vm>();
			foreach (Vm vm in host.VmListProperty)
			{
				if (!vm.InMigration)
				{
					vmsToMigrate.Add(vm);
				}
			}
			return vmsToMigrate;
		}

		/// <summary>
		/// Gets the over utilized hosts.
		/// </summary>
		/// <returns> the over utilized hosts </returns>
		protected internal virtual IList<PowerHostUtilizationHistory> OverUtilizedHosts
		{
			get
			{
				IList<PowerHostUtilizationHistory> overUtilizedHosts = new List<PowerHostUtilizationHistory>();
				foreach (PowerHostUtilizationHistory host in this.HostListProperty)
				{
					if (isHostOverUtilized(host))
					{
						overUtilizedHosts.Add(host);
					}
				}
				return overUtilizedHosts;
			}
		}

		/// <summary>
		/// Gets the switched off hosts.
		/// </summary>
		/// <returns> the switched off hosts </returns>
		protected internal virtual IList<PowerHost> SwitchedOffHosts
		{
			get
			{
				IList<PowerHost> switchedOffHosts = new List<PowerHost>();
				foreach (PowerHost host in this.HostListProperty)
				{
					if (host.UtilizationOfCpu == 0)
					{
						switchedOffHosts.Add(host);
					}
				}
				return switchedOffHosts;
			}
		}

        /// <summary>
        /// Gets the most under utilized host.
        /// </summary>
        /// <param name="excludedHosts"> the excluded hosts </param>
        /// <returns> the most under utilized host </returns>
        //protected internal virtual PowerHost getUnderUtilizedHost(ISet<Host> excludedHosts)
        protected internal virtual PowerHost getUnderUtilizedHost(ISet<Host> excludedHosts)
        {
			double minUtilization = 1;
			PowerHost underUtilizedHost = null;
			foreach (PowerHost host in this.HostListProperty)
			{
				if (excludedHosts.Contains(host))
				{
					continue;
				}
				double utilization = host.UtilizationOfCpu;
				if (utilization > 0 && utilization < minUtilization && !areAllVmsMigratingOutOrAnyVmMigratingIn(host))
				{
					minUtilization = utilization;
					underUtilizedHost = host;
				}
			}
			return underUtilizedHost;
		}

		/// <summary>
		/// Checks whether all VMs of a given host are in migration.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if successful </returns>
		protected internal virtual bool areAllVmsMigratingOutOrAnyVmMigratingIn(PowerHost host)
		{
			foreach (PowerVm vm in host.VmListProperty)
			{
				if (!vm.InMigration)
				{
					return false;
				}
				if (host.VmsMigratingIn.Contains(vm))
				{
					return true;
				}
			}
			return true;
		}

		/// <summary>
		/// Checks if host is over utilized.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if the host is over utilized; false otherwise </returns>
		protected internal abstract bool isHostOverUtilized(PowerHost host);

		/// <summary>
		/// Adds an entry for each history map of a host.
		/// </summary>
		/// <param name="host"> the host to add metric history entries </param>
		/// <param name="metric"> the metric to be added to the metric history map </param>
		protected internal virtual void addHistoryEntry(HostDynamicWorkload host, double metric)
		{
			int hostId = host.Id;
			if (!TimeHistory.ContainsKey(hostId))
			{
				TimeHistory[hostId] = new List<double?>();
			}
			if (!UtilizationHistory.ContainsKey(hostId))
			{
				UtilizationHistory[hostId] = new List<double?>();
			}
			if (!MetricHistory.ContainsKey(hostId))
			{
				MetricHistory[hostId] = new List<double?>();
			}
			if (!TimeHistory[hostId].Contains(CloudSim.clock()))
			{
				TimeHistory[hostId].Add(CloudSim.clock());
				UtilizationHistory[hostId].Add(host.UtilizationOfCpu);
				MetricHistory[hostId].Add(metric);
			}
		}

		/// <summary>
		/// Updates the list of maps between a VM and the host where it is place. </summary>
		/// <seealso cref= #savedAllocation </seealso>
		protected internal virtual void saveAllocation()
		{
			SavedAllocation.Clear();
			foreach (Host host in HostListProperty)
			{
				foreach (Vm vm in host.VmListProperty)
				{
					if (host.VmsMigratingIn.Contains(vm))
					{
						continue;
					}
					IDictionary<string, object> map = new Dictionary<string, object>();
					map["host"] = host;
					map["vm"] = vm;
					SavedAllocation.Add(map);
				}
			}
		}

		/// <summary>
		/// Restore VM allocation from the allocation history. </summary>
		/// <seealso cref= #savedAllocation </seealso>
		protected internal virtual void restoreAllocation()
		{
			foreach (Host host in HostListProperty)
			{
				host.vmDestroyAll();
				host.reallocateMigratingInVms();
			}
			foreach (IDictionary<string, object> map in SavedAllocation)
			{
				Vm vm = (Vm) map["vm"];
				PowerHost host = (PowerHost) map["host"];
				if (!host.vmCreate(vm))
				{
					Log.printConcatLine("Couldn't restore VM #", vm.Id, " on host #", host.Id);
                    //Environment.Exit(0);
                    throw new InvalidOperationException("Couldn't restore VM");
				}
				VmTable[vm.Uid] = host;
			}
		}

		/// <summary>
		/// Gets the power consumption of a host after placement of a candidate VM.
		/// The VM is not in fact placed at the host.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <param name="vm"> the candidate vm
		/// </param>
		/// <returns> the power after allocation </returns>
		protected internal virtual double getPowerAfterAllocation(PowerHost host, Vm vm)
		{
			double power = 0;
			try
			{
				power = host.PowerModel.getPower(getMaxUtilizationAfterAllocation(host, vm));
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                //Environment.Exit(0);
                throw new InvalidOperationException("PowerModel.getPower failed");
			}
			return power;
		}

		/// <summary>
		/// Gets the max power consumption of a host after placement of a candidate VM.
		/// The VM is not in fact placed at the host.
		/// We assume that load is balanced between PEs. The only
		/// restriction is: VM's max MIPS < PE's MIPS
		/// </summary>
		/// <param name="host"> the host </param>
		/// <param name="vm"> the vm
		/// </param>
		/// <returns> the power after allocation </returns>
		protected internal virtual double getMaxUtilizationAfterAllocation(PowerHost host, Vm vm)
		{
			double requestedTotalMips = vm.CurrentRequestedTotalMips;
			double hostUtilizationMips = getUtilizationOfCpuMips(host);
			double hostPotentialUtilizationMips = hostUtilizationMips + requestedTotalMips;
			double pePotentialUtilization = hostPotentialUtilizationMips / host.TotalMips;
			return pePotentialUtilization;
		}

		/// <summary>
		/// Gets the utilization of the CPU in MIPS for the current potentially allocated VMs.
		/// </summary>
		/// <param name="host"> the host
		/// </param>
		/// <returns> the utilization of the CPU in MIPS </returns>
		protected internal virtual double getUtilizationOfCpuMips(PowerHost host)
		{
			double hostUtilizationMips = 0;
			foreach (Vm vm2 in host.VmListProperty)
			{
				if (host.VmsMigratingIn.Contains(vm2))
				{
					// calculate additional potential CPU usage of a migrating in VM
					hostUtilizationMips += host.getTotalAllocatedMipsForVm(vm2) * 0.9 / 0.1;
				}
				hostUtilizationMips += host.getTotalAllocatedMipsForVm(vm2);
			}
			return hostUtilizationMips;
		}

		/// <summary>
		/// Gets the saved allocation.
		/// </summary>
		/// <returns> the saved allocation </returns>
		protected internal virtual IList<IDictionary<string, object>> SavedAllocation
		{
			get
			{
				return savedAllocation;
			}
		}

		/// <summary>
		/// Sets the vm selection policy.
		/// </summary>
		/// <param name="vmSelectionPolicy"> the new vm selection policy </param>
		protected internal virtual PowerVmSelectionPolicy VmSelectionPolicy
		{
			set
			{
				this.vmSelectionPolicy = value;
			}
			get
			{
				return vmSelectionPolicy;
			}
		}


		/// <summary>
		/// Gets the utilization history.
		/// </summary>
		/// <returns> the utilization history </returns>
		public virtual IDictionary<int?, IList<double?>> UtilizationHistory
		{
			get
			{
				return utilizationHistory;
			}
		}

		/// <summary>
		/// Gets the metric history.
		/// </summary>
		/// <returns> the metric history </returns>
		public virtual IDictionary<int?, IList<double?>> MetricHistory
		{
			get
			{
				return metricHistory;
			}
		}

		/// <summary>
		/// Gets the time history.
		/// </summary>
		/// <returns> the time history </returns>
		public virtual IDictionary<int?, IList<double?>> TimeHistory
		{
			get
			{
				return timeHistory;
			}
		}

		/// <summary>
		/// Gets the execution time history vm selection.
		/// </summary>
		/// <returns> the execution time history vm selection </returns>
		public virtual IList<double?> ExecutionTimeHistoryVmSelection
		{
			get
			{
				return executionTimeHistoryVmSelection;
			}
		}

		/// <summary>
		/// Gets the execution time history host selection.
		/// </summary>
		/// <returns> the execution time history host selection </returns>
		public virtual IList<double?> ExecutionTimeHistoryHostSelection
		{
			get
			{
				return executionTimeHistoryHostSelection;
			}
		}

		/// <summary>
		/// Gets the execution time history vm reallocation.
		/// </summary>
		/// <returns> the execution time history vm reallocation </returns>
		public virtual IList<double?> ExecutionTimeHistoryVmReallocation
		{
			get
			{
				return executionTimeHistoryVmReallocation;
			}
		}

		/// <summary>
		/// Gets the execution time history total.
		/// </summary>
		/// <returns> the execution time history total </returns>
		public virtual IList<double?> ExecutionTimeHistoryTotal
		{
			get
			{
				return executionTimeHistoryTotal;
			}
		}

	}

}