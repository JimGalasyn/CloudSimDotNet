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
    using System.Diagnostics;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using PredicateType = org.cloudbus.cloudsim.core.predicates.PredicateType;

    /// <summary>
    /// PowerDatacenterNonPowerAware is a class that represents a <b>non-power</b> aware data center in the
    /// context of power-aware simulations.
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
    /// @since CloudSim Toolkit 2.0
    /// </summary>
    public class PowerDatacenterNonPowerAware : PowerDatacenter
	{

		/// <summary>
		/// Instantiates a new datacenter.
		/// </summary>
		/// <param name="name"> the datacenter name </param>
		/// <param name="characteristics"> the datacenter characteristics </param>
		/// <param name="schedulingInterval"> the scheduling interval </param>
		/// <param name="vmAllocationPolicy"> the vm provisioner </param>
		/// <param name="storageList"> the storage list
		/// </param>
		/// <exception cref="Exception"> the exception </exception>
		public PowerDatacenterNonPowerAware(string name, DatacenterCharacteristics characteristics, VmAllocationPolicy vmAllocationPolicy, IList<Storage> storageList, double schedulingInterval) : base(name, characteristics, vmAllocationPolicy, storageList, schedulingInterval)
		{
		}

		protected internal override void updateCloudletProcessing()
		{
			if (CloudletSubmitted == -1 || CloudletSubmitted == CloudSim.clock())
			{
				CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.VM_DATACENTER_EVENT));
				schedule(Id, SchedulingInterval, CloudSimTags.VM_DATACENTER_EVENT);
				return;
			}
			double currentTime = CloudSim.clock();
			double timeframePower = 0.0;

			if (currentTime > LastProcessTime)
			{
				double timeDiff = currentTime - LastProcessTime;
				double minTime = double.MaxValue;

				Log.printLine("\n");

				foreach (PowerHost host in this.HostListProperty)
				{
					Log.formatLine("%.2f: Host #%d", CloudSim.clock(), host.Id);

					double hostPower = 0.0;

					try
					{
						hostPower = host.MaxPower * timeDiff;
						timeframePower += hostPower;
					}
					catch (Exception e)
					{
						Debug.WriteLine(e.ToString());
						Debug.WriteLine(e.StackTrace);
					}

					Log.formatLine("%.2f: Host #%d utilization is %.2f%%", CloudSim.clock(), host.Id, host.UtilizationOfCpu * 100);
					Log.formatLine("%.2f: Host #%d energy is %.2f W*sec", CloudSim.clock(), host.Id, hostPower);
				}

				Log.formatLine("\n%.2f: Consumed energy is %.2f W*sec\n", CloudSim.clock(), timeframePower);

				Log.printLine("\n\n--------------------------------------------------------------\n\n");

				foreach (PowerHost host in this.HostListProperty)
				{
					Log.formatLine("\n%.2f: Host #%d", CloudSim.clock(), host.Id);

					double time = host.updateVmsProcessing(currentTime); // inform VMs to update
																			// processing
					if (time < minTime)
					{
						minTime = time;
					}
				}

				Power = Power + timeframePower;

				checkCloudletCompletion();

				/// <summary>
				/// Remove completed VMs * </summary>
				foreach (PowerHost host in this.HostListProperty)
				{
					foreach (Vm vm in host.CompletedVms)
					{
						VmAllocationPolicy.deallocateHostForVm(vm);
						VmListProperty.Remove(vm);
						Log.printLine("VM #" + vm.Id + " has been deallocated from host #" + host.Id);
					}
				}

				Log.printLine();

				if (!DisableMigrations)
				{
					IList<IDictionary<string, object>> migrationMap = VmAllocationPolicy.optimizeAllocation(VmListProperty);

					if (migrationMap != null)
					{
						foreach (IDictionary<string, object> migrate in migrationMap)
						{
							Vm vm = (Vm) migrate["vm"];
							PowerHost targetHost = (PowerHost) migrate["host"];
							PowerHost oldHost = (PowerHost) vm.Host;

							if (oldHost == null)
							{
								Log.formatLine("%.2f: Migration of VM #%d to Host #%d is started", CloudSim.clock(), vm.Id, targetHost.Id);
							}
							else
							{
								Log.formatLine("%.2f: Migration of VM #%d from Host #%d to Host #%d is started", CloudSim.clock(), vm.Id, oldHost.Id, targetHost.Id);
							}

							targetHost.addMigratingInVm(vm);
							incrementMigrationCount();

							/// <summary>
							/// VM migration delay = RAM / bandwidth + C (C = 10 sec) * </summary>
							send(Id, vm.Ram / ((double) vm.Bw / 8000) + 10, CloudSimTags.VM_MIGRATE, migrate);
						}
					}
				}

				// schedules an event to the next time
				if (minTime != double.MaxValue)
				{
					CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.VM_DATACENTER_EVENT));
					// CloudSim.cancelAll(getId(), CloudSim.SIM_ANY);
					send(Id, SchedulingInterval, CloudSimTags.VM_DATACENTER_EVENT);
				}

				LastProcessTime = currentTime;
			}
		}
	}
}