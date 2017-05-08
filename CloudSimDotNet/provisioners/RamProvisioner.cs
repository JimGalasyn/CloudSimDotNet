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
	/// RamProvisioner is an abstract class that represents the provisioning policy used by a host
	/// to allocate memory to virtual machines inside it. 
	/// Each host has to have its own instance of a RamProvisioner.
	/// When extending this class, care must be taken to guarantee that the field
	/// availableMemory will always contain the amount of free memory available for future allocations.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public abstract class RamProvisioner
	{

		/// <summary>
		/// The total ram capacity from the host that the provisioner can allocate to VMs. </summary>
		private int ram;

		/// <summary>
		/// The available ram. </summary>
		private int availableRam;

		/// <summary>
		/// Creates the new RamProvisioner.
		/// </summary>
		/// <param name="ram"> The total ram capacity from the host that the provisioner can allocate to VMs.
		/// 
		/// @pre ram>=0
		/// @post $none </param>
		public RamProvisioner(int ram)
		{
			Ram = ram;
			AvailableRam = ram;
		}

		/// <summary>
		/// Allocates RAM for a given VM.
		/// </summary>
		/// <param name="vm"> the virtual machine for which the RAM is being allocated </param>
		/// <param name="ram"> the RAM to be allocated to the VM
		/// </param>
		/// <returns> $true if the RAM could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateRamForVm(Vm vm, int ram);

		/// <summary>
		/// Gets the allocated RAM for a given VM.
		/// </summary>
		/// <param name="vm"> the VM
		/// </param>
		/// <returns> the allocated RAM for the vm </returns>
		public abstract int getAllocatedRamForVm(Vm vm);

		/// <summary>
		/// Releases RAM used by a VM.
		/// </summary>
		/// <param name="vm"> the vm
		/// 
		/// @pre $none
		/// @post none </param>
		public abstract void deallocateRamForVm(Vm vm);

		/// <summary>
		/// Releases RAM used by all VMs.
		/// 
		/// @pre $none
		/// @post none
		/// </summary>
		public virtual void deallocateRamForAllVms()
		{
			AvailableRam = Ram;
		}

		/// <summary>
		/// Checks if it is possible to change the current allocated RAM for the VM
		/// to a new amount, depending on the available RAM.
		/// </summary>
		/// <param name="vm"> the vm to check if there is enough available RAM on the host to 
		/// change the VM allocated RAM </param>
		/// <param name="ram"> the new total amount of RAM for the VM.
		/// </param>
		/// <returns> true, if is suitable for vm </returns>
		public abstract bool isSuitableForVm(Vm vm, int ram);

		/// <summary>
		/// Gets the ram capacity.
		/// </summary>
		/// <returns> the ram </returns>
		public virtual int Ram
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


		/// <summary>
		/// Gets the amount of used RAM in the host.
		/// </summary>
		/// <returns> used ram
		/// 
		/// @pre $none
		/// @post $none </returns>
		public virtual int UsedRam
		{
			get
			{
				return ram - availableRam;
			}
		}

		/// <summary>
		/// Gets the available RAM in the host.
		/// </summary>
		/// <returns> th available ram
		/// 
		/// @pre $none
		/// @post $none </returns>
		public virtual int AvailableRam
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


	}

}