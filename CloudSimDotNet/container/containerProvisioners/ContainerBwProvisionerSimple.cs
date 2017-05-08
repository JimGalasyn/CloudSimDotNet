using System.Collections.Generic;

/// 
namespace org.cloudbus.cloudsim.container.containerProvisioners
{


	using Container = org.cloudbus.cloudsim.container.core.Container;

	/// <summary>
	/// ContainerBwProvisionerSimple is a class that implements a simple best effort allocation policy: if there
	/// is bw available to request, it allocates; otherwise, it fails.
	/// 
	/// @author Sareh Fotuhi Piraghaj
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// </summary>
	public class ContainerBwProvisionerSimple : ContainerBwProvisioner
	{
		/// <summary>
		/// The container Bw table.
		/// </summary>
		private IDictionary<string, long?> containerBwTable;

		/// <summary>
		/// Instantiates a new container bw provisioner simple.
		/// </summary>
		/// <param name="containerBw"> the containerBw </param>
		public ContainerBwProvisionerSimple(long containerBw) : base(containerBw)
		{
			ContainerBwTable = new Dictionary<string, long?>();
		}

		/// <summary>
		/// Allocate BW for the container
		/// </summary>
		/// <param name="container"> </param>
		/// <param name="bw">       the bw
		/// @return </param>

		public override bool allocateBwForContainer(Container container, long bw)
		{
			deallocateBwForContainer(container);
			if (AvailableVmBw >= bw)
			{
				AvailableVmBw = AvailableVmBw - bw;
				ContainerBwTable[container.Uid] = bw;
				container.CurrentAllocatedBw = getAllocatedBwForContainer(container);
				return true;
			}

			container.CurrentAllocatedBw = getAllocatedBwForContainer(container);

			return false;
		}

		/// <summary>
		/// Get allocated bandwidth for container </summary>
		/// <param name="container">
		/// @return </param>
		public override long getAllocatedBwForContainer(Container container)
		{
			if (ContainerBwTable.ContainsKey(container.Uid))
			{
				return ContainerBwTable[container.Uid].Value;
			}
			return 0;
		}

		/// <summary>
		/// Release the allocated BW for the container </summary>
		/// <param name="container"> </param>
		public override void deallocateBwForContainer(Container container)
		{
			if (ContainerBwTable.ContainsKey(container.Uid))
			{
				long amountFreed = ContainerBwTable[container.Uid].Value;
                ContainerBwTable.Remove(container.Uid);
                AvailableVmBw = AvailableVmBw + amountFreed;
				container.CurrentAllocatedBw = 0;
			}

		}

		/// <summary>
		/// Release the VM bandwidth that is allocated to the container.
		/// </summary>
		public override void deallocateBwForAllContainers()
		{
			base.deallocateBwForAllContainers();
			ContainerBwTable.Clear();
		}

		/// <summary>
		/// Check if the VM has enough BW to allocate to the container </summary>
		/// <param name="container"> </param>
		/// <param name="bw">        the bw
		/// @return </param>
		public override bool isSuitableForContainer(Container container, long bw)
		{
			long allocatedBw = getAllocatedBwForContainer(container);
			bool result = allocateBwForContainer(container, bw);
			deallocateBwForContainer(container);
			if (allocatedBw > 0)
			{
				allocateBwForContainer(container, allocatedBw);
			}
			return result;
		}

		/// <returns> the containerBwTable </returns>
		protected internal virtual IDictionary<string, long?> ContainerBwTable
		{
			get
			{
				return containerBwTable;
			}
			set
			{
				this.containerBwTable = value;
			}
		}


	}

}