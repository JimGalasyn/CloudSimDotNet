using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerVmProvisioners
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;


	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerVmBwProvisionerSimple : ContainerVmBwProvisioner
	{

		/// <summary>
		/// The bw table.
		/// </summary>
		private IDictionary<string, long?> bwTable;

		/// <summary>
		/// Instantiates a new bw provisioner simple.
		/// </summary>
		/// <param name="bw"> the bw </param>
		public ContainerVmBwProvisionerSimple(long bw) : base(bw)
		{
			BwTable = new Dictionary<string, long?>();
		}


		public override bool allocateBwForContainerVm(ContainerVm containerVm, long bw)
		{
			deallocateBwForContainerVm(containerVm);

			if (AvailableBw >= bw)
			{
				AvailableBw = AvailableBw - bw;
				BwTable[containerVm.Uid] = bw;
				containerVm.CurrentAllocatedBw = getAllocatedBwForContainerVm(containerVm);
				return true;
			}

			containerVm.CurrentAllocatedBw = getAllocatedBwForContainerVm(containerVm);
			return false;
		}

		public override long getAllocatedBwForContainerVm(ContainerVm containerVm)
		{
			if (BwTable.ContainsKey(containerVm.Uid))
			{
				return BwTable[containerVm.Uid].Value;
			}
			return 0;
		}

		public override void deallocateBwForContainerVm(ContainerVm containerVm)
		{
			if (BwTable.ContainsKey(containerVm.Uid))
			{
				long amountFreed = BwTable[containerVm.Uid].Value;
                BwTable.Remove(containerVm.Uid);
                AvailableBw = AvailableBw + amountFreed;
				containerVm.CurrentAllocatedBw = 0;
			}

		}

		/*
		     * (non-Javadoc)
		     * ContainerVmBwProvisioner#deallocateBwForAllContainerVms
		     */
		public override void deallocateBwForAllContainerVms()
		{
			base.deallocateBwForAllContainerVms();
			BwTable.Clear();
		}

		public override bool isSuitableForContainerVm(ContainerVm containerVm, long bw)
		{
			long allocatedBw = getAllocatedBwForContainerVm(containerVm);
			bool result = allocateBwForContainerVm(containerVm, bw);
			deallocateBwForContainerVm(containerVm);
			if (allocatedBw > 0)
			{
				allocateBwForContainerVm(containerVm, allocatedBw);
			}
			return result;
		}


		/// <summary>
		/// Gets the bw table.
		/// </summary>
		/// <returns> the bw table </returns>
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