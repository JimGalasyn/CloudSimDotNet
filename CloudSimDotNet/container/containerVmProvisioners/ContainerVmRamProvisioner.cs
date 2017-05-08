namespace org.cloudbus.cloudsim.container.containerVmProvisioners
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;

	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public abstract class ContainerVmRamProvisioner
	{

		/// <summary>
		/// The ram.
		/// </summary>
		private float ram;

		/// <summary>
		/// The available availableRam.
		/// </summary>
		private float availableRam;


		/// <summary>
		/// Creates new Containervm Ram Provisioner
		/// </summary>
		/// <param name="availableContainerVmRam"> the vm ram </param>
		public ContainerVmRamProvisioner(float availableContainerVmRam)
		{
			Ram = availableContainerVmRam;
			AvailableRam = availableContainerVmRam;
		}

		/// <summary>
		/// allocate hosts ram to the VM
		/// </summary>
		/// <param name="containerVm"> the containerVm </param>
		/// <param name="ram">       the ram </param>
		/// <returns> $true if successful </returns>
		public abstract bool allocateRamForContainerVm(ContainerVm containerVm, float ram);

		/// <summary>
		/// Get the allocated ram of the containerVm
		/// </summary>
		/// <param name="containerVm"> the containerVm </param>
		/// <returns> the allocated ram of the containerVM </returns>
		public abstract float getAllocatedRamForContainerVm(ContainerVm containerVm);

		/// <summary>
		/// Release the allocated ram amount of the containerVm
		/// </summary>
		/// <param name="containerVm"> the containerVm </param>
		public abstract void deallocateRamForContainerVm(ContainerVm containerVm);

		/// <summary>
		/// Release the allocated ram of the vm.
		/// </summary>
		public virtual void deallocateRamForAllContainerVms()
		{
			AvailableRam = Ram;
		}


		/// <summary>
		/// It checks whether or not the vm have enough ram for the containerVm
		/// </summary>
		/// <param name="containerVm"> the containerVm </param>
		/// <param name="ram">       the vm's ram </param>
		/// <returns> $ture if it is suitable </returns>
		public abstract bool isSuitableForContainerVm(ContainerVm containerVm, float ram);

		/// <summary>
		/// get the allocated ram of the Vm
		/// </summary>
		/// <returns> the used ram of the Vm </returns>
		public virtual float UsedVmRam
		{
			get
			{
				return Ram - availableRam;
			}
		}


		/// <summary>
		/// get the available ram
		/// </summary>
		/// <returns> the available ram of the Vm </returns>
		public virtual float AvailableRam
		{
			get
			{
				return availableRam;
			}
			set
			{
				this.availableRam = value;
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