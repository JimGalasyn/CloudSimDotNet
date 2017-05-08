using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerProvisioners
{

	using Container = org.cloudbus.cloudsim.container.core.Container;


	public abstract class ContainerPeProvisioner
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
		public ContainerPeProvisioner(double mips)
		{
            // TEST: (fixed) Auto-generated constructor stub
            Mips = mips;
			AvailableMips = mips;
		}
		/// <summary>
		/// Allocates MIPS for a given Container.
		/// </summary>
		/// <param name="container">  container for which the MIPS are being allocated </param>
		/// <param name="mips"> the mips
		/// </param>
		/// <returns> $true if the MIPS could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateMipsForContainer(Container container, double mips);

		/// <summary>
		/// Allocates MIPS for a given Container.
		/// </summary>
		/// <param name="containerUid"> the container uid </param>
		/// <param name="mips"> the mips
		/// </param>
		/// <returns> $true if the MIPS could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateMipsForContainer(string containerUid, double mips);

		/// <summary>
		/// Allocates MIPS for a given container.
		/// </summary>
		/// <param name="container"> container for which the MIPS are being allocated </param>
		/// <param name="mips"> the mips for each virtual Pe
		/// </param>
		/// <returns> $true if the MIPS could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateMipsForContainer(Container container, List<double?> mips);

		/// <summary>
		/// Gets allocated MIPS for a given VM.
		/// </summary>
		/// <param name="container"> container for which the MIPS are being allocated
		/// </param>
		/// <returns> array of allocated MIPS
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract IList<double?> getAllocatedMipsForContainer(Container container);

		/// <summary>
		/// Gets total allocated MIPS for a given VM for all PEs.
		/// </summary>
		/// <param name="container"> container for which the MIPS are being allocated
		/// </param>
		/// <returns> total allocated MIPS
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract double getTotalAllocatedMipsForContainer(Container container);

		/// <summary>
		/// Gets allocated MIPS for a given container for a given virtual Pe.
		/// </summary>
		/// <param name="container"> container for which the MIPS are being allocated </param>
		/// <param name="peId"> the pe id
		/// </param>
		/// <returns> allocated MIPS
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract double getAllocatedMipsForContainerByVirtualPeId(Container container, int peId);

		/// <summary>
		/// Releases MIPS used by a container.
		/// </summary>
		/// <param name="container"> the container
		/// 
		/// @pre $none
		/// @post none </param>
		public abstract void deallocateMipsForContainer(Container container);

		/// <summary>
		/// Releases MIPS used by all containers.
		/// 
		/// @pre $none
		/// @post none
		/// </summary>
		public virtual void deallocateMipsForAllContainers()
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