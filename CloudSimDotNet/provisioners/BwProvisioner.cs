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
	/// BwProvisioner is an abstract class that represents the provisioning policy used by a host
	/// to allocate bandwidth (bw) to virtual machines inside it. 
	/// Each host has to have its own instance of a BwProvisioner.
	/// When extending this class, care must be taken to guarantee that
	/// the field availableBw will always contain the amount of free bandwidth available for future
	/// allocations.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public abstract class BwProvisioner
	{

		/// <summary>
		/// The total bandwidth capacity from the host that the provisioner can allocate to VMs. </summary>
		private long bw;

		/// <summary>
		/// The available bandwidth. </summary>
		private long availableBw;

		/// <summary>
		/// Creates the new BwProvisioner.
		/// </summary>
		/// <param name="bw"> The total bandwidth capacity from the host that the provisioner can allocate to VMs.
		/// 
		/// @pre bw >= 0
		/// @post $none </param>
		public BwProvisioner(long bw)
		{
			Bw = bw;
			AvailableBw = bw;
		}

		/// <summary>
		/// Allocates BW for a given VM.
		/// </summary>
		/// <param name="vm"> the virtual machine for which the bw are being allocated </param>
		/// <param name="bw"> the bw to be allocated to the VM
		/// </param>
		/// <returns> $true if the bw could be allocated; $false otherwise
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateBwForVm(Vm vm, long bw);

		/// <summary>
		/// Gets the allocated BW for VM.
		/// </summary>
		/// <param name="vm"> the VM
		/// </param>
		/// <returns> the allocated BW for vm </returns>
		public abstract long getAllocatedBwForVm(Vm vm);

		/// <summary>
		/// Releases BW used by a VM.
		/// </summary>
		/// <param name="vm"> the vm
		/// 
		/// @pre $none
		/// @post none </param>
		public abstract void deallocateBwForVm(Vm vm);

		/// <summary>
		/// Releases BW used by all VMs.
		/// 
		/// @pre $none
		/// @post none
		/// </summary>
		public virtual void deallocateBwForAllVms()
		{
			AvailableBw = Bw;
		}

		/// <summary>
		/// Checks if it is possible to change the current allocated BW for the VM
		/// to a new amount, depending on the available BW.
		/// </summary>
		/// <param name="vm"> the vm to check if there is enough available BW on the host to 
		/// change the VM allocated BW </param>
		/// <param name="bw"> the new total amount of BW for the VM.
		/// </param>
		/// <returns> true, if is suitable for vm </returns>
		public abstract bool isSuitableForVm(Vm vm, long bw);

		/// <summary>
		/// Gets the bw capacity.
		/// </summary>
		/// <returns> the bw capacity </returns>
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
		/// 
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
		/// 
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