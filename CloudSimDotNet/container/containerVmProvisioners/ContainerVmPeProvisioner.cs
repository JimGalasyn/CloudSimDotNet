using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerVmProvisioners
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;

	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public abstract class ContainerVmPeProvisioner
	{


		/// <summary>
		/// The mips. </summary>
		private double mips;

		/// <summary>
		/// The available mips. </summary>
		private double availableMips;

		/// <summary>
		/// Creates the new PeProvisioner.
		/// </summary>
		/// <param name="mips"> overall amount of MIPS available in the Pe
		/// 
		/// @pre mips>=0
		/// @post $none </param>
		public ContainerVmPeProvisioner(double mips)
		{
            // TEST: (fixed) Auto-generated constructor stub
            Mips = mips;
			AvailableMips = mips;
		}
		/// <summary>
		/// Allocates MIPS for a given containerVm.
		/// </summary>
		/// <param name="containerVm"> virtual machine for which the MIPS are being allocated </param>
		/// <param name="mips"> the mips
		/// </param>
		/// <returns> $true if the MIPS could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateMipsForContainerVm(ContainerVm containerVm, double mips);

		/// <summary>
		/// Allocates MIPS for a given VM.
		/// </summary>
		/// <param name="containerVmUid"> the containerVmUid </param>
		/// <param name="mips"> the mips
		/// </param>
		/// <returns> $true if the MIPS could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateMipsForContainerVm(string containerVmUid, double mips);

		/// <summary>
		/// Allocates MIPS for a given VM.
		/// </summary>
		/// <param name="containerVm"> virtual machine for which the MIPS are being allocated </param>
		/// <param name="mips"> the mips for each virtual Pe
		/// </param>
		/// <returns> $true if the MIPS could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateMipsForContainerVm(ContainerVm containerVm, IList<double?> mips);

		/// <summary>
		/// Gets allocated MIPS for a given VM.
		/// </summary>
		/// <param name="containerVm"> virtual machine for which the MIPS are being allocated
		/// </param>
		/// <returns> array of allocated MIPS
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract IList<double?> getAllocatedMipsForContainerVm(ContainerVm containerVm);

		/// <summary>
		/// Gets total allocated MIPS for a given VM for all PEs.
		/// </summary>
		/// <param name="containerVm"> virtual machine for which the MIPS are being allocated
		/// </param>
		/// <returns> total allocated MIPS
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract double getTotalAllocatedMipsForContainerVm(ContainerVm containerVm);

		/// <summary>
		/// Gets allocated MIPS for a given VM for a given virtual Pe.
		/// </summary>
		/// <param name="containerVm"> virtual machine for which the MIPS are being allocated </param>
		/// <param name="peId"> the pe id
		/// </param>
		/// <returns> allocated MIPS
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract double getAllocatedMipsForContainerVmByVirtualPeId(ContainerVm containerVm, int peId);

		/// <summary>
		/// Releases MIPS used by a VM.
		/// </summary>
		/// <param name="containerVm"> the containerVm
		/// 
		/// @pre $none
		/// @post none </param>
		public abstract void deallocateMipsForContainerVm(ContainerVm containerVm);

		/// <summary>
		/// Releases MIPS used by all VMs.
		/// 
		/// @pre $none
		/// @post none
		/// </summary>
		public virtual void deallocateMipsForAllContainerVms()
		{
			AvailableMips = Mips;
		}

		/// <summary>
		/// Gets the MIPS.
		/// </summary>
		/// <returns> the MIPS </returns>
		public virtual double Mips
		{
			get
			{
				return mips;
			}
			set
			{
				this.mips = value;
			}
		}


		/// <summary>
		/// Gets the available MIPS in the PE.
		/// </summary>
		/// <returns> available MIPS
		/// 
		/// @pre $none
		/// @post $none </returns>
		public virtual double AvailableMips
		{
			get
			{
				return availableMips;
			}
			set
			{
				this.availableMips = value;
			}
		}


		/// <summary>
		/// Gets the total allocated MIPS.
		/// </summary>
		/// <returns> the total allocated MIPS </returns>
		public virtual double TotalAllocatedMips
		{
			get
			{
				double totalAllocatedMips = Mips - AvailableMips;
				if (totalAllocatedMips > 0)
				{
					return totalAllocatedMips;
				}
				return 0;
			}
		}

		/// <summary>
		/// Gets the utilization of the Pe in percents.
		/// </summary>
		/// <returns> the utilization </returns>
		public virtual double Utilization
		{
			get
			{
				return TotalAllocatedMips / Mips;
			}
		}


	}

}