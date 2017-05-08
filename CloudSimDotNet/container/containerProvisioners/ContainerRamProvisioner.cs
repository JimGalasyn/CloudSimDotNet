namespace org.cloudbus.cloudsim.container.containerProvisioners
{

	using Container = org.cloudbus.cloudsim.container.core.Container;

	/// <summary>
	/// This class takes care of the provisioning of Container's ram .
	/// @author sareh
	/// </summary>
	public abstract class ContainerRamProvisioner
	{

		/// <summary>
		/// The ram.
		/// </summary>
		private float ram;

		/// <summary>
		/// The available availableContainerRam.
		/// </summary>
		private float availableVmRam;


		/// <summary>
		/// Creates new Container Ram Provisioner
		/// </summary>
		/// <param name="availableContainerRam"> the vm ram </param>
		public ContainerRamProvisioner(float availableContainerRam)
		{
			Ram = availableContainerRam;
			AvailableVmRam = availableContainerRam;
		}

		/// <summary>
		/// allocate vms ram to the container
		/// </summary>
		/// <param name="container"> the container </param>
		/// <param name="ram">       the ram </param>
		/// <returns> $true if successful </returns>
		public abstract bool allocateRamForContainer(Container container, float ram);

		/// <summary>
		/// Get the allocated ram of the container
		/// </summary>
		/// <param name="container"> the container </param>
		/// <returns> the allocated ram of the container </returns>
		public abstract float getAllocatedRamForContainer(Container container);

		/// <summary>
		/// Release the allocated ram amount of the container
		/// </summary>
		/// <param name="container"> the container </param>
		public abstract void deallocateRamForContainer(Container container);

		/// <summary>
		/// Release the allocated ram of the vm.
		/// </summary>
		public virtual void deallocateRamForAllContainers()
		{
			AvailableVmRam = Ram;
		}


		/// <summary>
		/// It checks whether or not the vm have enough ram for the container
		/// </summary>
		/// <param name="container"> the container </param>
		/// <param name="ram">       the vm's ram </param>
		/// <returns> $ture if it is suitable </returns>
		public abstract bool isSuitableForContainer(Container container, float ram);

		/// <summary>
		/// get the allocated ram of the Vm
		/// </summary>
		/// <returns> the used ram of the Vm </returns>
		public virtual float UsedVmRam
		{
			get
			{
				return Ram - availableVmRam;
			}
		}


		/// <summary>
		/// get the available ram
		/// </summary>
		/// <returns> the available ram of the Vm </returns>
		public virtual float AvailableVmRam
		{
			get
			{
				return availableVmRam;
			}
			set
			{
				this.availableVmRam = value;
			}
		}



		/// <returns> the ram </returns>
		public virtual float Ram
		{
			get
			{
				return ram;
			}
			set
			{
				this.ram = value;
			}
		}


	}

}