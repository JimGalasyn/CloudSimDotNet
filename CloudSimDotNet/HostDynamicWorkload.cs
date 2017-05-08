using System.Collections.Generic;
using System.Text;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
	using PeList = org.cloudbus.cloudsim.lists.PeList;
	using BwProvisioner = org.cloudbus.cloudsim.provisioners.BwProvisioner;
	using RamProvisioner = org.cloudbus.cloudsim.provisioners.RamProvisioner;

	/// <summary>
	/// A host supporting dynamic workloads and performance degradation.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public class HostDynamicWorkload : Host
	{

		/// <summary>
		/// The utilization mips. </summary>
		private double utilizationMips;

		/// <summary>
		/// The previous utilization mips. </summary>
		private double previousUtilizationMips;

		/// <summary>
		/// The host utilization state history. </summary>
		private readonly IList<HostStateHistoryEntry> stateHistory = new List<HostStateHistoryEntry>();

		/// <summary>
		/// Instantiates a new host.
		/// </summary>
		/// <param name="id"> the id </param>
		/// <param name="ramProvisioner"> the ram provisioner </param>
		/// <param name="bwProvisioner"> the bw provisioner </param>
		/// <param name="storage"> the storage capacity </param>
		/// <param name="peList"> the host's PEs list </param>
		/// <param name="vmScheduler"> the VM scheduler </param>
		public HostDynamicWorkload(int id, RamProvisioner ramProvisioner, BwProvisioner bwProvisioner, long storage, IList<Pe> peList, VmScheduler vmScheduler) : base(id, ramProvisioner, bwProvisioner, storage, peList, vmScheduler)
		{
			UtilizationMips = 0;
			PreviousUtilizationMips = 0;
		}

		public override double updateVmsProcessing(double currentTime)
		{
			double smallerTime = base.updateVmsProcessing(currentTime);
			PreviousUtilizationMips = UtilizationMips;
			UtilizationMips = 0;
			double hostTotalRequestedMips = 0;

			foreach (Vm vm in VmListProperty)
			{
				VmScheduler.deallocatePesForVm(vm);
			}

			foreach (Vm vm in VmListProperty)
			{
				VmScheduler.allocatePesForVm(vm, vm.CurrentRequestedMips);
			}

			foreach (Vm vm in VmListProperty)
			{
				double totalRequestedMips = vm.CurrentRequestedTotalMips;
				double totalAllocatedMips = VmScheduler.getTotalAllocatedMipsForVm(vm);

				if (!Log.Disabled)
				{
					Log.formatLine("%.2f: [Host #" + Id + "] Total allocated MIPS for VM #" + vm.Id + " (Host #" + vm.Host.Id + ") is %.2f, was requested %.2f out of total %.2f (%.2f%%)", CloudSim.clock(), totalAllocatedMips, totalRequestedMips, vm.Mips, totalRequestedMips / vm.Mips * 100);

					IList<Pe> pes = VmScheduler.getPesAllocatedForVM(vm);
					StringBuilder pesString = new StringBuilder();
					foreach (Pe pe in pes)
					{
						pesString.Append(string.Format(" PE #" + pe.Id + ": {0:F2}.", pe.PeProvisioner.getTotalAllocatedMipsForVm(vm)));
					}
					Log.formatLine("%.2f: [Host #" + Id + "] MIPS for VM #" + vm.Id + " by PEs (" + NumberOfPes + " * " + VmScheduler.PeCapacity + ")." + pesString, CloudSim.clock());
				}

				if (VmsMigratingIn.Contains(vm))
				{
					Log.formatLine("%.2f: [Host #" + Id + "] VM #" + vm.Id + " is being migrated to Host #" + Id, CloudSim.clock());
				}
				else
				{
					if (totalAllocatedMips + 0.1 < totalRequestedMips)
					{
						Log.formatLine("%.2f: [Host #" + Id + "] Under allocated MIPS for VM #" + vm.Id + ": %.2f", CloudSim.clock(), totalRequestedMips - totalAllocatedMips);
					}

					vm.addStateHistoryEntry(currentTime, totalAllocatedMips, totalRequestedMips, (vm.InMigration && !VmsMigratingIn.Contains(vm)));

					if (vm.InMigration)
					{
						Log.formatLine("%.2f: [Host #" + Id + "] VM #" + vm.Id + " is in migration", CloudSim.clock());
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
		/// Gets the list of completed vms.
		/// </summary>
		/// <returns> the completed vms </returns>
		public virtual IList<Vm> CompletedVms
		{
			get
			{
				IList<Vm> vmsToRemove = new List<Vm>();
				foreach (Vm vm in VmListProperty)
				{
					if (vm.InMigration)
					{
						continue;
					}
					if (vm.CurrentRequestedTotalMips == 0)
					{
						vmsToRemove.Add(vm);
					}
				}
				return vmsToRemove;
			}
		}

		/// <summary>
		/// Gets the max utilization percentage among by all PEs.
		/// </summary>
		/// <returns> the maximum utilization percentage </returns>
		public virtual double MaxUtilization
		{
			get
			{
				return PeList.getMaxUtilization(PeListProperty);
			}
		}

		/// <summary>
		/// Gets the max utilization percentage among by all PEs allocated to a VM.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> the max utilization percentage of the VM </returns>
		public virtual double getMaxUtilizationAmongVmsPes(Vm vm)
		{
			return PeList.getMaxUtilizationAmongVmsPes(PeListProperty, vm);
		}

		/// <summary>
		/// Gets the utilization of memory (in absolute values).
		/// </summary>
		/// <returns> the utilization of memory </returns>
		public virtual double UtilizationOfRam
		{
			get
			{
				return RamProvisioner.UsedRam;
			}
		}

		/// <summary>
		/// Gets the utilization of bw (in absolute values).
		/// </summary>
		/// <returns> the utilization of bw </returns>
		public virtual double UtilizationOfBw
		{
			get
			{
				return BwProvisioner.UsedBw;
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
		/// <returns> the previous utilization of cpu in percents </returns>
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
		/// <returns> current utilization of CPU in MIPS
		/// @todo This method only calls the  <seealso cref="#getUtilizationMips()"/>.
		/// getUtilizationMips may be deprecated and its code copied here. </returns>
		public virtual double UtilizationOfCpuMips
		{
			get
			{
				return UtilizationMips;
			}
		}

		/// <summary>
		/// Gets the utilization of CPU in MIPS.
		/// </summary>
		/// <returns> current utilization of CPU in MIPS </returns>
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
		/// Gets the previous utilization of CPU in mips.
		/// </summary>
		/// <returns> the previous utilization of CPU in mips </returns>
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
		/// Gets the host state history.
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
		/// Adds a host state history entry.
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