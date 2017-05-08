namespace org.cloudbus.cloudsim.container.containerProvisioners
{

	using Container = org.cloudbus.cloudsim.container.core.Container;

	/// <summary>
	/// ContainerBwProvisioner is an abstract class that represents the provisioning policy of bandwidth to
	/// containers inside a vm. When extending this class, care must be taken to guarantee that
	/// the field availableBw will always contain the amount of free bandwidth available for future
	/// allocations.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Sareh Fotuhi Piraghaj
	/// </summary>
	public abstract class ContainerBwProvisioner
	{


		/// <summary>
		/// The vmBw.
		/// </summary>
		private long vmBw;

		/// <summary>
		/// The available availableVmBw.
		/// </summary>
		private long availableVmBw;


		/// <summary>
		/// Creates the new ContainerBwProvisioner.
		/// </summary>
		/// <param name="vmBw"> the Vm BW
		/// @pre bw >= 0
		/// @post $none </param>
		public ContainerBwProvisioner(long vmBw)
		{
            // TEST: (fixed) Auto-generated constructor stub
            VmBw = vmBw;
			AvailableVmBw = vmBw;
		}

		/// <summary>
		/// Allocates BW for a given container.
		/// </summary>
		/// <param name="container"> containercontainer for which the bw are being allocated </param>
		/// <param name="bw">        the bw </param>
		/// <returns> $true if the bw could be allocated; $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateBwForContainer(Container container, long bw);

		/// <summary>
		/// Gets the allocated BW for container.
		/// </summary>
		/// <param name="container"> the container </param>
		/// <returns> the allocated BW for container </returns>
		public abstract long getAllocatedBwForContainer(Container container);

		/// <summary>
		/// Releases BW used by a container.
		/// </summary>
		/// <param name="container"> the container
		/// @pre $none
		/// @post none </param>
		public abstract void deallocateBwForContainer(Container container);

		/// <summary>
		/// Releases BW used by a all containers.
		/// 
		/// @pre $none
		/// @post none
		/// </summary>
		public virtual void deallocateBwForAllContainers()
		{
			AvailableVmBw = Bw;
		}


		/// <summary>
		/// Checks if BW is suitable for container.
		/// </summary>
		/// <param name="container"> the container </param>
		/// <param name="bw">        the bw </param>
		/// <returns> true, if BW is suitable for container </returns>
		public abstract bool isSuitableForContainer(Container container, long bw);


		/// <summary>
		/// Gets the amount of used BW in the vm.
		/// </summary>
		/// <returns> used bw
		/// @pre $none
		/// @post $none </returns>
		public virtual long UsedVmBw
		{
			get
			{
				return vmBw - availableVmBw;
			}
		}


		/// <returns> the vmBw </returns>
		public virtual long Bw
		{
			get
			{
				return vmBw;
			}
		}

		/// <param name="vmBw"> the vmBw to set </param>
		public virtual long VmBw
		{
			set
			{
				this.vmBw = value;
			}
		}

		/// <returns> the availableVmBw </returns>
		public virtual long AvailableVmBw
		{
			get
			{
				return availableVmBw;
			}
			set
			{
				this.availableVmBw = value;
			}
		}


	}

}