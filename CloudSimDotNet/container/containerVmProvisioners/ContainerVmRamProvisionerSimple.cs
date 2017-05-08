using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerVmProvisioners
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;


	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerVmRamProvisionerSimple : ContainerVmRamProvisioner
	{

		/// <summary>
		/// The RAM table.
		/// </summary>
		private IDictionary<string, float?> containerVmRamTable;

		/// <param name="availableRam"> the available ram </param>
		public ContainerVmRamProvisionerSimple(int availableRam) : base(availableRam)
		{
			ContainerVmRamTable = new Dictionary<string, float?>();
		}

		public override bool allocateRamForContainerVm(ContainerVm containerVm, float ram)
		{
			float maxRam = containerVm.Ram;

			if (ram >= maxRam)
			{
				ram = maxRam;
			}

			deallocateRamForContainerVm(containerVm);

			if (AvailableRam >= ram)
			{
				AvailableRam = AvailableRam - ram;
				ContainerVmRamTable[containerVm.Uid] = ram;
				containerVm.CurrentAllocatedRam = getAllocatedRamForContainerVm(containerVm);
				return true;
			}

			containerVm.CurrentAllocatedRam = getAllocatedRamForContainerVm(containerVm);

			return false;
		}

		public override float getAllocatedRamForContainerVm(ContainerVm containerVm)
		{
			if (ContainerVmRamTable.ContainsKey(containerVm.Uid))
			{
				return ContainerVmRamTable[containerVm.Uid].Value;
			}
			return 0;
		}

		public override void deallocateRamForContainerVm(ContainerVm containerVm)
		{
			if (ContainerVmRamTable.ContainsKey(containerVm.Uid))
			{
				float amountFreed = ContainerVmRamTable[containerVm.Uid].Value;
                ContainerVmRamTable.Remove(containerVm.Uid);
                AvailableRam = AvailableRam + amountFreed;
				containerVm.CurrentAllocatedRam = 0;
			}

		}


		public override void deallocateRamForAllContainerVms()
		{
			base.deallocateRamForAllContainerVms();
			ContainerVmRamTable.Clear();
		}

		public override bool isSuitableForContainerVm(ContainerVm containerVm, float ram)
		{
			float allocatedRam = getAllocatedRamForContainerVm(containerVm);
			bool result = allocateRamForContainerVm(containerVm, ram);
			deallocateRamForContainerVm(containerVm);
			if (allocatedRam > 0)
			{
				allocateRamForContainerVm(containerVm, allocatedRam);
			}
			return result;
		}


		/// <returns> the containerVmRamTable </returns>
		protected internal virtual IDictionary<string, float?> ContainerVmRamTable
		{
			get
			{
				return containerVmRamTable;
			}
			set
			{
				this.containerVmRamTable = value;
			}
		}


	}

}