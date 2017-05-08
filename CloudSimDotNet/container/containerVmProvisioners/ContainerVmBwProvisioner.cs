namespace org.cloudbus.cloudsim.container.containerVmProvisioners
{


	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;

	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public abstract class ContainerVmBwProvisioner
	{

		/// <summary>
		/// The bw.
		/// </summary>
		private long bw;

		/// <summary>
		/// The available bw.
		/// </summary>
		private long availableBw;

		/// <summary>
		/// Creates the new BwProvisioner.
		/// </summary>
		/// <param name="bw"> overall amount of bandwidth available in the host.
		/// @pre bw >= 0
		/// @post $none </param>
		public ContainerVmBwProvisioner(long bw)
		{
			Bw = bw;
			AvailableBw = bw;
		}

		/// <summary>
		/// Allocates BW for a given VM.
		/// </summary>
		/// <param name="containerVm"> virtual machine for which the bw are being allocated </param>
		/// <param name="bw">          the bw </param>
		/// <returns> $true if the bw could be allocated; $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateBwForContainerVm(ContainerVm containerVm, long bw);

		/// <summary>
		/// Gets the allocated BW for VM.
		/// </summary>
		/// <param name="containerVm"> the VM </param>
		/// <returns> the allocated BW for vm </returns>
		public abstract long getAllocatedBwForContainerVm(ContainerVm containerVm);

		/// <summary>
		/// Releases BW used by a VM.
		/// </summary>
		/// <param name="containerVm"> the vm
		/// @pre $none
		/// @post none </param>
		public abstract void deallocateBwForContainerVm(ContainerVm containerVm);

		/// <summary>
		/// Releases BW used by a all VMs.
		/// 
		/// @pre $none
		/// @post none
		/// </summary>
		public virtual void deallocateBwForAllContainerVms()
		{
			AvailableBw = Bw;
		}

		/// <summary>
		/// Checks if BW is suitable for vm.
		/// </summary>
		/// <param name="containerVm"> the vm </param>
		/// <param name="bw">          the bw </param>
		/// <returns> true, if BW is suitable for vm </returns>
		public abstract bool isSuitableForContainerVm(ContainerVm containerVm, long bw);

		/// <summary>
		/// Gets the bw.
		/// </summary>
		/// <returns> the bw </returns>
		public virtual long Bw
		{
			get
			{
				return bw;
			}
			set
			{
				this.bw = value;
			}
		}


		/// <summary>
		/// Gets the available BW in the host.
		/// </summary>
		/// <returns> available bw
		/// @pre $none
		/// @post $none </returns>
		public virtual long AvailableBw
		{
			get
			{
				return availableBw;
			}
			set
			{
				this.availableBw = value;
			}
		}

		/// <summary>
		/// Gets the amount of used BW in the host.
		/// </summary>
		/// <returns> used bw
		/// @pre $none
		/// @post $none </returns>
		public virtual long UsedBw
		{
			get
			{
				return bw - availableBw;
			}
		}

	}

}