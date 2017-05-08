using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerProvisioners
{


	using Container = org.cloudbus.cloudsim.container.core.Container;

	/// <summary>
	/// @author sareh
	/// </summary>
	public class ContainerRamProvisionerSimple : ContainerRamProvisioner
	{
		/// <summary>
		/// The RAM table.
		/// </summary>
		private IDictionary<string, float?> containerRamTable;

		/// <param name="availableRam"> the available ram </param>
		public ContainerRamProvisionerSimple(float availableRam) : base(availableRam)
		{
			ContainerRamTable = new Dictionary<string, float?>();
		}

		/// <seealso cref= ContainerRamProvisioner#allocateRamForContainer(Container, float) </seealso>
		/// <param name="container"> the container </param>
		/// <param name="ram">       the ram
		/// @return </param>
		public override bool allocateRamForContainer(Container container, float ram)
		{
			float maxRam = container.Ram;

			if (ram >= maxRam)
			{
				ram = maxRam;
			}

			deallocateRamForContainer(container);

			if (AvailableVmRam >= ram)
			{
				AvailableVmRam = AvailableVmRam - ram;
				ContainerRamTable[container.Uid] = ram;
				container.CurrentAllocatedRam = getAllocatedRamForContainer(container);
				return true;
			}

			container.CurrentAllocatedRam = getAllocatedRamForContainer(container);

			return false;
		}

		/// <summary>
		/// @link ContainerRamProvisioner#getAllocatedRamForContainer(Container) </summary>
		/// <param name="container"> the container
		/// @return </param>
		public override float getAllocatedRamForContainer(Container container)
		{
			if (ContainerRamTable.ContainsKey(container.Uid))
			{
				return ContainerRamTable[container.Uid].Value;
			}
			return 0;
		}

		/// <seealso cref= ContainerRamProvisioner#deallocateRamForContainer(Container) </seealso>
		/// <param name="container"> the container </param>
		public override void deallocateRamForContainer(Container container)
		{
			if (ContainerRamTable.ContainsKey(container.Uid))
			{
				float amountFreed = ContainerRamTable[container.Uid].Value;
                ContainerRamTable.Remove(container.Uid);
                AvailableVmRam = AvailableVmRam + amountFreed;
				container.CurrentAllocatedRam = 0;
			}
		}

		/// <seealso cref= ContainerRamProvisioner#deallocateRamForAllContainers() </seealso>
		public override void deallocateRamForAllContainers()
		{
			base.deallocateRamForAllContainers();
			ContainerRamTable.Clear();
		}


		/// <seealso cref= ContainerRamProvisioner#isSuitableForContainer(Container, float) </seealso>
		/// <param name="container"> the container </param>
		/// <param name="ram">       the vm's ram
		/// @return </param>
		public override bool isSuitableForContainer(Container container, float ram)
		{
			float allocatedRam = getAllocatedRamForContainer(container);
			bool result = allocateRamForContainer(container, ram);
			deallocateRamForContainer(container);
			if (allocatedRam > 0)
			{
				allocateRamForContainer(container, allocatedRam);
			}
			return result;
		}


		/// 
		/// <returns> the containerRamTable </returns>
		protected internal virtual IDictionary<string, float?> ContainerRamTable
		{
			get
			{
				return containerRamTable;
			}
			set
			{
				this.containerRamTable = value;
			}
		}


	}

}