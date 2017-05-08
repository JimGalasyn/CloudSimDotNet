using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.provisioners
{


	/// <summary>
	/// BwProvisionerSimple is an extension of <seealso cref="BwProvisioner"/> which uses a best-effort policy to
	/// allocate bandwidth (bw) to VMs: 
	/// if there is available bw on the host, it allocates; otherwise, it fails. 
	/// Each host has to have its own instance of a RamProvisioner.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class BwProvisionerSimple : BwProvisioner
	{

		/// <summary>
		/// The BW map, where each key is a VM id and each value
		/// is the amount of BW allocated to that VM. 
		/// </summary>
		private IDictionary<string, long?> bwTable;

		/// <summary>
		/// Instantiates a new bw provisioner simple.
		/// </summary>
		/// <param name="bw"> The total bw capacity from the host that the provisioner can allocate to VMs.  </param>
		public BwProvisionerSimple(long bw) : base(bw)
		{
			BwTable = new Dictionary<string, long?>();
		}

		public override bool allocateBwForVm(Vm vm, long bw)
		{
			deallocateBwForVm(vm);

			if (AvailableBw >= bw)
			{
				AvailableBw = AvailableBw - bw;
				BwTable[vm.Uid] = bw;
				vm.CurrentAllocatedBw = getAllocatedBwForVm(vm);
				return true;
			}

			vm.CurrentAllocatedBw = getAllocatedBwForVm(vm);
			return false;
		}

		public override long getAllocatedBwForVm(Vm vm)
		{
			if (BwTable.ContainsKey(vm.Uid))
			{
				return BwTable[vm.Uid].Value;
			}
			return 0;
		}

		public override void deallocateBwForVm(Vm vm)
		{
			if (BwTable.ContainsKey(vm.Uid))
			{
				long? amountFreed = BwTable[vm.Uid];
                BwTable.Remove(vm.Uid);
                AvailableBw = AvailableBw + amountFreed.Value;
				vm.CurrentAllocatedBw = 0;
			}
		}

		public override void deallocateBwForAllVms()
		{
			base.deallocateBwForAllVms();
			BwTable.Clear();
		}

		public override bool isSuitableForVm(Vm vm, long bw)
		{
			long allocatedBw = getAllocatedBwForVm(vm);
			bool result = allocateBwForVm(vm, bw);
			deallocateBwForVm(vm);
			if (allocatedBw > 0)
			{
				allocateBwForVm(vm, allocatedBw);
			}
			return result;
		}

		/// <summary>
		/// Gets the map between VMs and allocated bw.
		/// </summary>
		/// <returns> the bw map </returns>
		protected internal virtual IDictionary<string, long?> BwTable
		{
			get
			{
				return bwTable;
			}
			set
			{
				this.bwTable = value;
			}
		}


	}

}