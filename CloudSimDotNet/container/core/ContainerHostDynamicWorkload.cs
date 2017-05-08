using System.Collections.Generic;
using System.Text;

namespace org.cloudbus.cloudsim.container.core
{

	using ContainerVmBwProvisioner = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmBwProvisioner;
	using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
	using ContainerVmPeList = org.cloudbus.cloudsim.container.lists.ContainerVmPeList;
	using ContainerVmRamProvisioner = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmRamProvisioner;
	using ContainerVmScheduler = org.cloudbus.cloudsim.container.schedulers.ContainerVmScheduler;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

	/// <summary>
	/// Created by sareh on 14/07/15.
	/// </summary>
	public class ContainerHostDynamicWorkload : ContainerHost
	{


			/// <summary>
			/// The utilization mips. </summary>
			private double utilizationMips;

			/// <summary>
			/// The previous utilization mips. </summary>
			private double previousUtilizationMips;

			/// <summary>
			/// The state history. </summary>
			private readonly IList<HostStateHistoryEntry> stateHistory = new List<HostStateHistoryEntry>();

			/// <summary>
			/// Instantiates a new host.
			/// </summary>
			/// <param name="id"> the id </param>
			/// <param name="ramProvisioner"> the ram provisioner </param>
			/// <param name="bwProvisioner"> the bw provisioner </param>
			/// <param name="storage"> the storage </param>
			/// <param name="peList"> the pe list </param>
			/// <param name="vmScheduler"> the VM scheduler </param>
			public ContainerHostDynamicWorkload(int id, ContainerVmRamProvisioner ramProvisioner, ContainerVmBwProvisioner bwProvisioner, long storage, IList<ContainerVmPe> peList, ContainerVmScheduler vmScheduler) : base(id, ramProvisioner, bwProvisioner, storage, peList, vmScheduler)
			{
				UtilizationMips = 0;
				PreviousUtilizationMips = 0;
			}

			/*
			 * (non-Javadoc)
			 * @see cloudsim.Host#updateVmsProcessing(double)
			 */
			public override double updateContainerVmsProcessing(double currentTime)
			{
				double smallerTime = base.updateContainerVmsProcessing(currentTime);
				PreviousUtilizationMips = UtilizationMips;
				UtilizationMips = 0;
				double hostTotalRequestedMips = 0;

				foreach (ContainerVm containerVm in VmListProperty)
				{
					ContainerVmScheduler.deallocatePesForVm(containerVm);
				}

				foreach (ContainerVm containerVm in VmListProperty)
				{
					ContainerVmScheduler.allocatePesForVm(containerVm, containerVm.CurrentRequestedMips);
				}

				foreach (ContainerVm containerVm in VmListProperty)
				{
					double totalRequestedMips = containerVm.CurrentRequestedTotalMips;
					double totalAllocatedMips = ContainerVmScheduler.getTotalAllocatedMipsForContainerVm(containerVm);

					if (!Log.Disabled)
					{
						Log.formatLine("%.2f: [Host #" + Id + "] Total allocated MIPS for VM #" + containerVm.Id + " (Host #" + containerVm.Host.Id + ") is %.2f, was requested %.2f out of total %.2f (%.2f%%)", CloudSim.clock(), totalAllocatedMips, totalRequestedMips, containerVm.Mips, totalRequestedMips / containerVm.Mips * 100);

						IList<ContainerVmPe> pes = ContainerVmScheduler.getPesAllocatedForContainerVM(containerVm);
						StringBuilder pesString = new StringBuilder();
						foreach (ContainerVmPe pe in pes)
						{
							pesString.Append(string.Format(" PE #" + pe.Id + ": {0:F2}.", pe.ContainerVmPeProvisioner.getTotalAllocatedMipsForContainerVm(containerVm)));
						}
						Log.formatLine("%.2f: [Host #" + Id + "] MIPS for VM #" + containerVm.Id + " by PEs (" + NumberOfPes + " * " + ContainerVmScheduler.PeCapacity + ")." + pesString, CloudSim.clock());
					}

					if (VmsMigratingIn.Contains(containerVm))
					{
						Log.formatLine("%.2f: [Host #" + Id + "] VM #" + containerVm.Id + " is being migrated to Host #" + Id, CloudSim.clock());
					}
					else
					{
						if (totalAllocatedMips + 0.1 < totalRequestedMips)
						{
							Log.formatLine("%.2f: [Host #" + Id + "] Under allocated MIPS for VM #" + containerVm.Id + ": %.2f", CloudSim.clock(), totalRequestedMips - totalAllocatedMips);
						}

						containerVm.addStateHistoryEntry(currentTime, totalAllocatedMips, totalRequestedMips, (containerVm.InMigration && !VmsMigratingIn.Contains(containerVm)));

						if (containerVm.InMigration)
						{
							Log.formatLine("%.2f: [Host #" + Id + "] VM #" + containerVm.Id + " is in migration", CloudSim.clock());
							totalAllocatedMips /= 0.9; // performance degradation due to migration - 10%
						}
					}

					UtilizationMips = UtilizationMips + totalAllocatedMips;
					hostTotalRequestedMips += totalRequestedMips;
				}

				addStateHistoryEntry(currentTime, UtilizationMips, hostTotalRequestedMips, (UtilizationMips > 0));

				return smallerTime;
			}

			/// <summary>
			/// Gets the completed vms.
			/// </summary>
			/// <returns> the completed vms </returns>
			public virtual IList<ContainerVm> CompletedVms
			{
				get
				{
					IList<ContainerVm> vmsToRemove = new List<ContainerVm>();
					foreach (ContainerVm containerVm in VmListProperty)
					{
						if (containerVm.InMigration)
						{
							continue;
						}
		//                if the  vm is in waiting state then dont kill it just waite !!!!!!!!!
						 if (containerVm.InWaiting)
						 {
							 continue;
						 }
		//              if (containerVm.getCurrentRequestedTotalMips() == 0) {
		//                    vmsToRemove.add(containerVm);
		//                }
						if (containerVm.NumberOfContainers == 0)
						{
							vmsToRemove.Add(containerVm);
						}
					}
					return vmsToRemove;
				}
			}



		/// <summary>
		/// Gets the completed vms.
		/// </summary>
		/// <returns> the completed vms </returns>
		public virtual int NumberofContainers
		{
			get
			{
				int numberofContainers = 0;
				foreach (ContainerVm containerVm in VmListProperty)
				{
					numberofContainers += containerVm.NumberOfContainers;
					Log.print("The number of containers in VM# " + containerVm.Id + "is: " + containerVm.NumberOfContainers);
					Log.printLine();
				}
				return numberofContainers;
			}
		}




			/// <summary>
			/// Gets the max utilization among by all PEs.
			/// </summary>
			/// <returns> the utilization </returns>
			public virtual double MaxUtilization
			{
				get
				{
					return ContainerVmPeList.getMaxUtilization(PeListProperty);
				}
			}

			/// <summary>
			/// Gets the max utilization among by all PEs allocated to the VM.
			/// </summary>
			/// <param name="vm"> the vm </param>
			/// <returns> the utilization </returns>
			public virtual double getMaxUtilizationAmongVmsPes(ContainerVm vm)
			{
				return ContainerVmPeList.getMaxUtilizationAmongVmsPes(PeListProperty, vm);
			}

			/// <summary>
			/// Gets the utilization of memory.
			/// </summary>
			/// <returns> the utilization of memory </returns>
			public virtual double UtilizationOfRam
			{
				get
				{
					return ContainerVmRamProvisioner.UsedVmRam;
				}
			}

			/// <summary>
			/// Gets the utilization of bw.
			/// </summary>
			/// <returns> the utilization of bw </returns>
			public virtual double UtilizationOfBw
			{
				get
				{
					return ContainerVmBwProvisioner.UsedBw;
				}
			}

			/// <summary>
			/// Get current utilization of CPU in percentage.
			/// </summary>
			/// <returns> current utilization of CPU in percents </returns>
			public virtual double UtilizationOfCpu
			{
				get
				{
					double utilization = UtilizationMips / TotalMips;
					if (utilization > 1 && utilization < 1.01)
					{
						utilization = 1;
					}
					return utilization;
				}
			}

			/// <summary>
			/// Gets the previous utilization of CPU in percentage.
			/// </summary>
			/// <returns> the previous utilization of cpu </returns>
			public virtual double PreviousUtilizationOfCpu
			{
				get
				{
					double utilization = PreviousUtilizationMips / TotalMips;
					if (utilization > 1 && utilization < 1.01)
					{
						utilization = 1;
					}
					return utilization;
				}
			}

			/// <summary>
			/// Get current utilization of CPU in MIPS.
			/// </summary>
			/// <returns> current utilization of CPU in MIPS </returns>
			public virtual double UtilizationOfCpuMips
			{
				get
				{
					return UtilizationMips;
				}
			}

			/// <summary>
			/// Gets the utilization mips.
			/// </summary>
			/// <returns> the utilization mips </returns>
			public virtual double UtilizationMips
			{
				get
				{
					return utilizationMips;
				}
				set
				{
					this.utilizationMips = value;
				}
			}


			/// <summary>
			/// Gets the previous utilization mips.
			/// </summary>
			/// <returns> the previous utilization mips </returns>
			public virtual double PreviousUtilizationMips
			{
				get
				{
					return previousUtilizationMips;
				}
				set
				{
					this.previousUtilizationMips = value;
				}
			}


			/// <summary>
			/// Gets the state history.
			/// </summary>
			/// <returns> the state history </returns>
			public virtual IList<HostStateHistoryEntry> StateHistory
			{
				get
				{
					return stateHistory;
				}
			}

			/// <summary>
			/// Adds the state history entry.
			/// </summary>
			/// <param name="time"> the time </param>
			/// <param name="allocatedMips"> the allocated mips </param>
			/// <param name="requestedMips"> the requested mips </param>
			/// <param name="isActive"> the is active </param>
			public virtual void addStateHistoryEntry(double time, double allocatedMips, double requestedMips, bool isActive)
			{

				HostStateHistoryEntry newState = new HostStateHistoryEntry(time, allocatedMips, requestedMips, isActive);
				if (StateHistory.Count > 0)
				{
					HostStateHistoryEntry previousState = StateHistory[StateHistory.Count - 1];
					if (previousState.Time == time)
					{
						StateHistory[StateHistory.Count - 1] = newState;
						return;
					}
				}
				StateHistory.Add(newState);
			}

	}



}