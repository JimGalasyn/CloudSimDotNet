using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.provisioners
{


	/// <summary>
	/// /**
	/// PeProvisioner is an abstract class that represents the provisioning policy used by a host
	/// to allocate its PEs to virtual machines inside it. It gets a physical PE
	/// and manage it in order to provide this PE as virtual PEs for VMs.
	/// In that way, a given PE might be shared among different VMs.
	/// Each host's PE has to have its own instance of a PeProvisioner.
	/// When extending this class, care must be taken to guarantee that the field
	/// availableMips will always contain the amount of free mips available for future allocations.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public abstract class PeProvisioner
	{

		/// <summary>
		/// The total mips capacity of the PE that the provisioner can allocate to VMs. </summary>
		private double mips;

		/// <summary>
		/// The available mips. </summary>
		private double availableMips;

		/// <summary>
		/// Creates a new PeProvisioner.
		/// </summary>
		/// <param name="mips"> The total mips capacity of the PE that the provisioner can allocate to VMs
		/// 
		/// @pre mips>=0
		/// @post $none </param>
		public PeProvisioner(double mips)
		{
			Mips = mips;
			AvailableMips = mips;
		}

		/// <summary>
		/// Allocates a new virtual PE with a specific capacity for a given VM.
		/// The virtual PE to be added will use the total or partial mips capacity
		/// of the physical PE.
		/// </summary>
		/// <param name="vm"> the virtual machine for which the new virtual PE is being allocated </param>
		/// <param name="mips"> the mips to be allocated to the virtual PE of the given VM
		/// </param>
		/// <returns> $true if the virtual PE could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateMipsForVm(Vm vm, double mips);

		/// <summary>
		/// Allocates a new virtual PE with a specific capacity for a given VM.
		/// </summary>
		/// <param name="vmUid"> the virtual machine for which the new virtual PE is being allocated </param>
		/// <param name="mips"> the mips to be allocated to the virtual PE of the given VM
		/// </param>
		/// <returns> $true if the virtual PE could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		/// <seealso cref= #allocateMipsForVm(org.cloudbus.cloudsim.Vm, double)  </seealso>
		public abstract bool allocateMipsForVm(string vmUid, double mips);

		/// <summary>
		/// Allocates a new set of virtual PEs with a specific capacity for a given VM.
		/// The virtual PE to be added will use the total or partial mips capacity
		/// of the physical PE.
		/// </summary>
		/// <param name="vm"> the virtual machine for which the new virtual PE is being allocated </param>
		/// <param name="mips"> the list of mips capacity of each virtual PE to be allocated to the VM
		/// </param>
		/// <returns> $true if the set of virtual PEs could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none
		/// @todo In this case, each PE can have a different capacity, what 
		/// in many places this situation is not considered, such as 
		/// in the <seealso cref="Vm"/>, <seealso cref="Pe"/> and <seealso cref="DatacenterCharacteristics"/>
		/// classes. </returns>
		public abstract bool allocateMipsForVm(Vm vm, IList<double?> mips);

		/// <summary>
		/// Gets the list of allocated virtual PEs' MIPS for a given VM.
		/// </summary>
		/// <param name="vm"> the virtual machine the get the list of allocated virtual PEs' MIPS
		/// </param>
		/// <returns> list of allocated virtual PEs' MIPS
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract IList<double?> getAllocatedMipsForVm(Vm vm);

		/// <summary>
		/// Gets total allocated MIPS for a given VM for all PEs.
		/// </summary>
		/// <param name="vm"> the virtual machine the get the total allocated MIPS capacity
		/// </param>
		/// <returns> total allocated MIPS
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract double getTotalAllocatedMipsForVm(Vm vm);

		/// <summary>
		/// Gets the MIPS capacity of a virtual Pe allocated to a given VM.
		/// </summary>
		/// <param name="vm"> virtual machine to get a given virtual PE capacity </param>
		/// <param name="peId"> the virtual pe id
		/// </param>
		/// <returns> allocated MIPS for the virtual PE
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract double getAllocatedMipsForVmByVirtualPeId(Vm vm, int peId);

		/// <summary>
		/// Releases all virtual PEs allocated to a given VM.
		/// </summary>
		/// <param name="vm"> the vm
		/// 
		/// @pre $none
		/// @post none </param>
		public abstract void deallocateMipsForVm(Vm vm);

		/// <summary>
		/// Releases all virtual PEs allocated to all VMs.
		/// 
		/// @pre $none
		/// @post none
		/// </summary>
		public virtual void deallocateMipsForAllVms()
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