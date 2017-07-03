using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled
{

    using PowerContainerVmAllocationAbstract = org.cloudbus.cloudsim.container.resourceAllocators.PowerContainerVmAllocationAbstract;
    using PowerContainerVmSelectionPolicy = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicy;
    using org.cloudbus.cloudsim.container.core;
    using PowerContainerVmList = org.cloudbus.cloudsim.container.lists.PowerContainerVmList;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using ExecutionTimeMeasurer = org.cloudbus.cloudsim.util.ExecutionTimeMeasurer;
    using System.Diagnostics;

    /// <summary>
    /// Created by sareh on 28/07/15.
    /// </summary>
    public abstract class PowerContainerVmAllocationPolicyMigrationAbstract : PowerContainerVmAllocationAbstract
	{

		/// <summary>
		/// The vm selection policy.
		/// </summary>
		private PowerContainerVmSelectionPolicy vmSelectionPolicy;

		/// <summary>
		/// The saved allocation.
		/// </summary>
		private readonly IList<IDictionary<string, object>> savedAllocation = new List<IDictionary<string, object>>();

		/// <summary>
		/// The utilization history.
		/// </summary>
		private readonly IDictionary<int?, IList<double?>> utilizationHistory = new Dictionary<int?, IList<double?>>();

		/// <summary>
		/// The metric history.
		/// </summary>
		private readonly IDictionary<int?, IList<double?>> metricHistory = new Dictionary<int?, IList<double?>>();

		/// <summary>
		/// The time history.
		/// </summary>
		private readonly IDictionary<int?, IList<double?>> timeHistory = new Dictionary<int?, IList<double?>>();

		/// <summary>
		/// The execution time history vm selection.
		/// </summary>
		private readonly IList<double?> executionTimeHistoryVmSelection = new List<double?>();

		/// <summary>
		/// The execution time history host selection.
		/// </summary>
		private readonly IList<double?> executionTimeHistoryHostSelection = new List<double?>();

		/// <summary>
		/// The execution time history vm reallocation.
		/// </summary>
		private readonly IList<double?> executionTimeHistoryVmReallocation = new List<double?>();

		/// <summary>
		/// The execution time history total.
		/// </summary>
		private readonly IList<double?> executionTimeHistoryTotal = new List<double?>();

		/// <summary>
		/// Instantiates a new power vm allocation policy migration abstract.
		/// </summary>
		/// <param name="hostList">          the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		public PowerContainerVmAllocationPolicyMigrationAbstract(IList<ContainerHost> hostList, PowerContainerVmSelectionPolicy vmSelectionPolicy) : base(hostList)
		{
			VmSelectionPolicy = vmSelectionPolicy;

		}

		/// <summary>
		/// Optimize allocation of the VMs according to current utilization.
		/// </summary>
		/// <param name="vmList"> the vm list </param>
		/// <returns> the array list< hash map< string, object>> </returns>
		public override IList<IDictionary<string, object>> optimizeAllocation(IList<ContainerVm> vmList)
		{
			ExecutionTimeMeasurer.start("optimizeAllocationTotal");

			ExecutionTimeMeasurer.start("optimizeAllocationHostSelection");
			IList<PowerContainerHostUtilizationHistory> overUtilizedHosts = OverUtilizedHosts;
			ExecutionTimeHistoryHostSelection.Add(ExecutionTimeMeasurer.end("optimizeAllocationHostSelection"));

			printOverUtilizedHosts(overUtilizedHosts);

			saveAllocation();

			ExecutionTimeMeasurer.start("optimizeAllocationVmSelection");
			IList<ContainerVm> vmsToMigrate = getVmsToMigrateFromHosts(overUtilizedHosts);
			ExecutionTimeHistoryVmSelection.Add(ExecutionTimeMeasurer.end("optimizeAllocationVmSelection"));

			Log.printLine("Reallocation of VMs from the over-utilized hosts:");
			ExecutionTimeMeasurer.start("optimizeAllocationVmReallocation");
			IList<IDictionary<string, object>> migrationMap = getNewVmPlacement(vmsToMigrate, new HashSet<ContainerHost>(overUtilizedHosts));
			ExecutionTimeHistoryVmReallocation.Add(ExecutionTimeMeasurer.end("optimizeAllocationVmReallocation"));
			Log.printLine();

			((List<IDictionary<string, object>>)migrationMap).AddRange(getMigrationMapFromUnderUtilizedHosts(overUtilizedHosts, migrationMap));

			restoreAllocation();

			ExecutionTimeHistoryTotal.Add(ExecutionTimeMeasurer.end("optimizeAllocationTotal"));

			return migrationMap;
		}

		/// <summary>
		/// Gets the migration map from under utilized hosts.
		/// </summary>
		/// <param name="overUtilizedHosts"> the over utilized hosts </param>
		/// <returns> the migration map from under utilized hosts </returns>
		protected internal virtual IList<IDictionary<string, object>> getMigrationMapFromUnderUtilizedHosts(IList<PowerContainerHostUtilizationHistory> overUtilizedHosts, IList<IDictionary<string, object>> previousMap)
		{
			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();
			IList<PowerContainerHost> switchedOffHosts = SwitchedOffHosts;

			// over-utilized hosts + hosts that are selected to migrate VMs to from over-utilized hosts
			ISet<PowerContainerHost> excludedHostsForFindingUnderUtilizedHost = new HashSet<PowerContainerHost>();
            // TEST: (fixed) UnionWith vs addAll
            excludedHostsForFindingUnderUtilizedHost.UnionWith(overUtilizedHosts);
			excludedHostsForFindingUnderUtilizedHost.UnionWith(switchedOffHosts);
			excludedHostsForFindingUnderUtilizedHost.UnionWith(extractHostListFromMigrationMap(previousMap));

			// over-utilized + under-utilized hosts
			ISet<PowerContainerHost> excludedHostsForFindingNewVmPlacement = new HashSet<PowerContainerHost>();
			excludedHostsForFindingNewVmPlacement.UnionWith(overUtilizedHosts);
			excludedHostsForFindingNewVmPlacement.UnionWith(switchedOffHosts);

			int numberOfHosts = ContainerHostListProperty.Count;

			while (true)
			{
				if (numberOfHosts == excludedHostsForFindingUnderUtilizedHost.Count)
				{
					break;
				}

                // TEST: (fixed) Does this HashSet conversion work?
                var set = new HashSet<ContainerHost>(excludedHostsForFindingUnderUtilizedHost);
                PowerContainerHost underUtilizedHost = getUnderUtilizedHost(set);
                //PowerContainerHost underUtilizedHost = getUnderUtilizedHost(excludedHostsForFindingUnderUtilizedHost);
                if (underUtilizedHost == null)
				{
					break;
				}

				Log.printConcatLine("Under-utilized host: host #", underUtilizedHost.Id, "\n");

				excludedHostsForFindingUnderUtilizedHost.Add(underUtilizedHost);
				excludedHostsForFindingNewVmPlacement.Add(underUtilizedHost);

				IList<ContainerVm> vmsToMigrateFromUnderUtilizedHost = getVmsToMigrateFromUnderUtilizedHost(underUtilizedHost);
				if (vmsToMigrateFromUnderUtilizedHost.Count == 0)
				{
					continue;
				}

				Log.print("Reallocation of VMs from the under-utilized host: ");
				if (!Log.Disabled)
				{
					foreach (ContainerVm vm in vmsToMigrateFromUnderUtilizedHost)
					{
						Log.print(vm.Id + " ");
					}
				}
				Log.printLine();

                // TODO: Figure out type cast issue.
                //IList<IDictionary<string, object>> newVmPlacement = 
                //    getNewVmPlacementFromUnderUtilizedHost(vmsToMigrateFromUnderUtilizedHost, excludedHostsForFindingNewVmPlacement);
                IList<IDictionary<string, object>> newVmPlacement = null;

                excludedHostsForFindingUnderUtilizedHost.UnionWith(extractHostListFromMigrationMap(newVmPlacement));

				((List<IDictionary<string, object>>)migrationMap).AddRange(newVmPlacement);
				Log.printLine();
			}

			excludedHostsForFindingUnderUtilizedHost.Clear();
			excludedHostsForFindingNewVmPlacement.Clear();
			return migrationMap;
		}

		/// <summary>
		/// Prints the over utilized hosts.
		/// </summary>
		/// <param name="overUtilizedHosts"> the over utilized hosts </param>
		protected internal virtual void printOverUtilizedHosts(IList<PowerContainerHostUtilizationHistory> overUtilizedHosts)
		{
			if (!Log.Disabled)
			{
				Log.printLine("Over-utilized hosts:");
				foreach (PowerContainerHostUtilizationHistory host in overUtilizedHosts)
				{
					Log.printConcatLine("Host #", host.Id);
				}
				Log.printLine();
			}
		}

		/// <summary>
		/// Find host for vm.
		/// </summary>
		/// <param name="vm">            the vm </param>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the power host </returns>
		public virtual PowerContainerHost findHostForVm(ContainerVm vm, ISet<ContainerHost> excludedHosts)
		{
			double minPower = double.MaxValue;
			PowerContainerHost allocatedHost = null;

			foreach (PowerContainerHost host in ContainerHostListProperty)
			{
				if (excludedHosts.Contains(host))
				{
					continue;
				}
				if (host.isSuitableForContainerVm(vm))
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
		/// Checks if is host over utilized after allocation.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <param name="vm">   the vm </param>
		/// <returns> true, if is host over utilized after allocation </returns>
		protected internal virtual bool isHostOverUtilizedAfterAllocation(PowerContainerHost host, ContainerVm vm)
		{
			bool isHostOverUtilizedAfterAllocation = true;
			if (host.containerVmCreate(vm))
			{
				isHostOverUtilizedAfterAllocation = isHostOverUtilized(host);
				host.containerVmDestroy(vm);
			}
			return isHostOverUtilizedAfterAllocation;
		}

        /// <summary>
        /// Find host for vm.
        /// </summary>
        /// <param name="vm"> the vm </param>
        /// <returns> the power host </returns>
        // TEST: (fixed) Figure out return type issue.
        //public override PowerContainerHost findHostForVm(ContainerVm vm)
        public override ContainerHost findHostForVm(ContainerVm vm)
        {
            ISet<ContainerHost> excludedHosts = new HashSet<ContainerHost>();
			if (vm.Host != null)
			{
				excludedHosts.Add(vm.Host);
			}
			PowerContainerHost hostForVm = findHostForVm(vm, excludedHosts);
			excludedHosts.Clear();

			return hostForVm;
		}

		/// <summary>
		/// Extract host list from migration map.
		/// </summary>
		/// <param name="migrationMap"> the migration map </param>
		/// <returns> the list </returns>
		protected internal virtual IList<PowerContainerHost> extractHostListFromMigrationMap(IList<IDictionary<string, object>> migrationMap)
		{
			IList<PowerContainerHost> hosts = new List<PowerContainerHost>();
			foreach (IDictionary<string, object> map in migrationMap)
			{
				hosts.Add((PowerContainerHost) map["host"]);
			}

			return hosts;
		}

		/// <summary>
		/// Gets the new vm placement.
		/// </summary>
		/// <param name="vmsToMigrate">  the vms to migrate </param>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the new vm placement </returns>
		protected internal virtual IList<IDictionary<string, object>> getNewVmPlacement(IList<ContainerVm> vmsToMigrate, ISet<ContainerHost> excludedHosts)
		{
			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();
			PowerContainerVmList.sortByCpuUtilization(vmsToMigrate);
			foreach (ContainerVm vm in vmsToMigrate)
			{
				PowerContainerHost allocatedHost = findHostForVm(vm, excludedHosts);
				if (allocatedHost != null)
				{
					allocatedHost.containerVmCreate(vm);
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
		/// <param name="vmsToMigrate">  the vms to migrate </param>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the new vm placement from under utilized host </returns>
		protected internal virtual IList<IDictionary<string, object>> getNewVmPlacementFromUnderUtilizedHost(IList<ContainerVm> vmsToMigrate, ISet<ContainerHost> excludedHosts)
		{
			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();
			PowerContainerVmList.sortByCpuUtilization(vmsToMigrate);
			foreach (ContainerVm vm in vmsToMigrate)
			{
				PowerContainerHost allocatedHost = findHostForVm(vm, excludedHosts);
				if (allocatedHost != null)
				{
					allocatedHost.containerVmCreate(vm);
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
						((ContainerHost) map["host"]).containerVmDestroy((ContainerVm) map["vm"]);
					}
					migrationMap.Clear();
					break;
				}
			}
			return migrationMap;
		}

		/// <summary>
		/// Gets the vms to migrate from hosts.
		/// </summary>
		/// <param name="overUtilizedHosts"> the over utilized hosts </param>
		/// <returns> the vms to migrate from hosts </returns>
		protected internal virtual IList<ContainerVm> getVmsToMigrateFromHosts(IList<PowerContainerHostUtilizationHistory> overUtilizedHosts)
		{
			IList<ContainerVm> vmsToMigrate = new List<ContainerVm>();
			foreach (PowerContainerHostUtilizationHistory host in overUtilizedHosts)
			{
				while (true)
				{
					ContainerVm vm = VmSelectionPolicy.getVmToMigrate(host);
					if (vm == null)
					{
						break;
					}
					vmsToMigrate.Add(vm);
					host.containerVmDestroy(vm);
					if (!isHostOverUtilized(host))
					{
						break;
					}
				}
			}
			return vmsToMigrate;
		}


		/// <summary>
		/// Gets the vms to migrate from under utilized host.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the vms to migrate from under utilized host </returns>
		protected internal virtual IList<ContainerVm> getVmsToMigrateFromUnderUtilizedHost(PowerContainerHost host)
		{
			IList<ContainerVm> vmsToMigrate = new List<ContainerVm>();
			foreach (ContainerVm vm in host.VmListProperty)
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
		protected internal virtual IList<PowerContainerHostUtilizationHistory> OverUtilizedHosts
		{
			get
			{
				IList<PowerContainerHostUtilizationHistory> overUtilizedHosts = new List<PowerContainerHostUtilizationHistory>();
				foreach (PowerContainerHostUtilizationHistory host in ContainerHostListProperty)
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
		/// Gets the switched off host.
		/// </summary>
		/// <returns> the switched off host </returns>
		protected internal virtual IList<PowerContainerHost> SwitchedOffHosts
		{
			get
			{
				IList<PowerContainerHost> switchedOffHosts = new List<PowerContainerHost>();
				foreach (PowerContainerHost host in ContainerHostListProperty)
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
        /// Gets the under utilized host.
        /// </summary>
        /// <param name="excludedHosts"> the excluded hosts </param>
        /// <returns> the under utilized host </returns>
        // TEST: (fixed) Generic type
        protected internal virtual PowerContainerHost getUnderUtilizedHost(ISet<ContainerHost> excludedHosts)
        {
			double minUtilization = 1;
			PowerContainerHost underUtilizedHost = null;
			foreach (PowerContainerHost host in ContainerHostListProperty)
			{
				if (excludedHosts.Contains(host))
				{
					continue;
				}

				double utilization = host.UtilizationOfCpu;
				if (utilization > 0 && utilization < minUtilization && !areAllVmsMigratingOutOrAnyVmMigratingIn(host) && !areAllContainersMigratingOutOrAnyContainersMigratingIn(host))
				{
					minUtilization = utilization;
					underUtilizedHost = host;
				}
			}
			return underUtilizedHost;
		}

		/// <summary>
		/// Checks whether all vms are in migration.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if successful </returns>
		protected internal virtual bool areAllVmsMigratingOutOrAnyVmMigratingIn(PowerContainerHost host)
		{
			foreach (PowerContainerVm vm in host.VmListProperty)
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
		/// Checks whether all vms are in migration.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if successful </returns>
		protected internal virtual bool areAllContainersMigratingOutOrAnyContainersMigratingIn(PowerContainerHost host)
		{
			foreach (PowerContainerVm vm in host.VmListProperty)
			{
			   if (vm.ContainersMigratingIn.Count != 0)
			   {
				   return true;
			   }
				foreach (Container container in vm.ContainerListProperty)
				{
					if (!container.InMigration)
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Checks if is host over utilized.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if is host over utilized </returns>
		protected internal abstract bool isHostOverUtilized(PowerContainerHost host);


		/// <summary>
		/// Checks if is host over utilized.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if is host over utilized </returns>
		protected internal abstract bool isHostUnderUtilized(PowerContainerHost host);


		/// <summary>
		/// Adds the history value.
		/// </summary>
		/// <param name="host">   the host </param>
		/// <param name="metric"> the metric </param>
		protected internal virtual void addHistoryEntry(ContainerHostDynamicWorkload host, double metric)
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
		/// Save allocation.
		/// </summary>
		protected internal virtual void saveAllocation()
		{
			SavedAllocation.Clear();
			foreach (ContainerHost host in ContainerHostListProperty)
			{
				foreach (ContainerVm vm in host.VmListProperty)
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
		/// Restore allocation.
		/// </summary>
		protected internal virtual void restoreAllocation()
		{
			foreach (ContainerHost host in ContainerHostListProperty)
			{
				host.containerVmDestroyAll();
				host.reallocateMigratingInContainerVms();
			}
			foreach (IDictionary<string, object> map in SavedAllocation)
			{
				ContainerVm vm = (ContainerVm) map["vm"];
				PowerContainerHost host = (PowerContainerHost) map["host"];
				if (!host.containerVmCreate(vm))
				{
					Log.printConcatLine("Couldn't restore VM #", vm.Id, " on host #", host.Id);
                    //Environment.Exit(0);
                    throw new InvalidOperationException("Couldn't restore VM");
				}
				VmTable[vm.Uid] = host;
			}
		}

		/// <summary>
		/// Gets the power after allocation.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <param name="vm">   the vm </param>
		/// <returns> the power after allocation </returns>
		protected internal virtual double getPowerAfterAllocation(PowerContainerHost host, ContainerVm vm)
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
                throw e;
			}
			return power;
		}

		/// <summary>
		/// Gets the power after allocation. We assume that load is balanced between PEs. The only
		/// restriction is: VM's max MIPS < PE's MIPS
		/// </summary>
		/// <param name="host"> the host </param>
		/// <param name="vm">   the vm </param>
		/// <returns> the power after allocation </returns>
		protected internal virtual double getMaxUtilizationAfterAllocation(PowerContainerHost host, ContainerVm vm)
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
		/// <param name="host"> the host </param>
		/// <returns> the utilization of the CPU in MIPS </returns>
		protected internal virtual double getUtilizationOfCpuMips(PowerContainerHost host)
		{
			double hostUtilizationMips = 0;
			foreach (ContainerVm vm2 in host.VmListProperty)
			{
				if (host.VmsMigratingIn.Contains(vm2))
				{
					// calculate additional potential CPU usage of a migrating in VM
					hostUtilizationMips += host.getTotalAllocatedMipsForContainerVm(vm2) * 0.9 / 0.1;
				}
				hostUtilizationMips += host.getTotalAllocatedMipsForContainerVm(vm2);
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
		protected internal virtual PowerContainerVmSelectionPolicy VmSelectionPolicy
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


	//    public abstract List<? extends Container> getContainersToMigrateFromHosts(List<PowerContainerHostUtilizationHistory> overUtilizedHosts);
	}
















}