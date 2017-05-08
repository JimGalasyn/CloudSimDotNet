using System;
using System.Collections.Generic;

/*
 * Title: CloudSim Toolkit Description: CloudSim (Cloud Simulation) Toolkit for Modeling and
 * Simulation of Clouds Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * 
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
	using PeList = org.cloudbus.cloudsim.lists.PeList;
	using BwProvisioner = org.cloudbus.cloudsim.provisioners.BwProvisioner;
	using RamProvisioner = org.cloudbus.cloudsim.provisioners.RamProvisioner;

	/// <summary>
	/// A Host is a Physical Machine (PM) inside a Datacenter. It is also called as a Server.
	/// It executes actions related to management of virtual machines (e.g., creation and destruction).
	/// A host has a defined policy for provisioning memory and bw, as well as an allocation policy for
	/// Pe's to virtual machines. A host is associated to a datacenter. It can host virtual machines.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class Host
	{

		/// <summary>
		/// The id of the host. </summary>
		private int id;

		/// <summary>
		/// The storage capacity. </summary>
		private long storage;

		/// <summary>
		/// The ram provisioner. </summary>
		private RamProvisioner ramProvisioner;

		/// <summary>
		/// The bw provisioner. </summary>
		private BwProvisioner bwProvisioner;

		/// <summary>
		/// The allocation policy for scheduling VM execution. </summary>
		private VmScheduler vmScheduler;

		/// <summary>
		/// The list of VMs assigned to the host. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private final java.util.List<? extends Vm> vmList = new java.util.ArrayList<Vm>();
		private readonly IList<Vm> vmList = new List<Vm>();

		/// <summary>
		/// The Processing Elements (PEs) of the host, that
		/// represent the CPU cores of it, and thus, its processing capacity. 
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends Pe> peList;
		private IList<Pe> peList;

		/// <summary>
		/// Tells whether this host is working properly or has failed. </summary>
		private bool failed;

		/// <summary>
		/// The VMs migrating in. </summary>
		private readonly IList<Vm> vmsMigratingIn = new List<Vm>();

		/// <summary>
		/// The datacenter where the host is placed. </summary>
		private Datacenter datacenter;

        /// <summary>
        /// Instantiates a new host.
        /// </summary>
        /// <param name="id"> the host id </param>
        /// <param name="ramProvisioner"> the ram provisioner </param>
        /// <param name="bwProvisioner"> the bw provisioner </param>
        /// <param name="storage"> the storage capacity </param>
        /// <param name="peList"> the host's PEs list </param>
        /// <param name="vmScheduler"> the vm scheduler </param>
        //public Host<T1>(int id, RamProvisioner ramProvisioner, BwProvisioner bwProvisioner, long storage, IList<T1> peList, VmScheduler vmScheduler) where T1 : Pe
        public Host(int id, RamProvisioner ramProvisioner, BwProvisioner bwProvisioner, long storage, IList<Pe> peList, VmScheduler vmScheduler)
        {

            Id = id;
			RamProvisioner = ramProvisioner;
			BwProvisioner = bwProvisioner;
			Storage = storage;
			VmScheduler = vmScheduler;

            PeListProperty = peList;
			Failed = false;
		}

		/// <summary>
		/// Requests updating of cloudlets' processing in VMs running in this host.
		/// </summary>
		/// <param name="currentTime"> the current time </param>
		/// <returns> expected time of completion of the next cloudlet in all VMs in this host or
		///         <seealso cref="Double#MAX_VALUE"/> if there is no future events expected in this host
		/// @pre currentTime >= 0.0
		/// @post $none
		/// @todo there is an inconsistency between the return value of this method
		/// and the individual call of {@link Vm#updateVmProcessing(double, java.util.List),
		/// and consequently the <seealso cref="CloudletScheduler#updateVmProcessing(double, java.util.List)"/>.
		/// The current method returns <seealso cref="Double#MAX_VALUE"/>  while the other ones
		/// return 0. It has to be checked if there is a reason for this
		/// difference.} </returns>
		public virtual double updateVmsProcessing(double currentTime)
		{
			double smallerTime = double.MaxValue;

			foreach (Vm vm in VmListProperty)
			{
				double time = vm.updateVmProcessing(currentTime, VmScheduler.getAllocatedMipsForVm(vm));
				if (time > 0.0 && time < smallerTime)
				{
					smallerTime = time;
				}
			}

			return smallerTime;
		}

		/// <summary>
		/// Adds a VM migrating into the current host.
		/// </summary>
		/// <param name="vm"> the vm </param>
		public virtual void addMigratingInVm(Vm vm)
		{
			vm.InMigration = true;

			if (!VmsMigratingIn.Contains(vm))
			{
				if (Storage < vm.Size)
				{
					Log.printConcatLine("[VmScheduler.addMigratingInVm] Allocation of VM #", vm.Id, " to Host #", Id, " failed by storage");
                    //Environment.Exit(0);
                    throw new ArgumentException("Storage < vm.Size", "vm");
				}

				if (!RamProvisioner.allocateRamForVm(vm, vm.CurrentRequestedRam))
				{
					Log.printConcatLine("[VmScheduler.addMigratingInVm] Allocation of VM #", vm.Id, " to Host #", Id, " failed by RAM");
                    //Environment.Exit(0);
                    throw new ArgumentException("RamProvisioner.allocateRamForVm failed", "vm");
                }

                if (!BwProvisioner.allocateBwForVm(vm, vm.CurrentRequestedBw))
				{
					Log.printLine("[VmScheduler.addMigratingInVm] Allocation of VM #" + vm.Id + " to Host #" + Id + " failed by BW");
                    //Environment.Exit(0);
                    throw new ArgumentException("BwProvisioner.allocateBwForVm failed", "vm");
                }
                
				VmScheduler.VmsMigratingIn.Add(vm.Uid);
				if (!VmScheduler.allocatePesForVm(vm, vm.CurrentRequestedMips))
				{
					Log.printLine("[VmScheduler.addMigratingInVm] Allocation of VM #" + vm.Id + " to Host #" + Id + " failed by MIPS");
                    //Environment.Exit(0);
                    throw new ArgumentException("BwProvisioner.allocateBwForVm failed", "vm");
                }

				Storage = Storage - vm.Size;

				VmsMigratingIn.Add(vm);
				VmListProperty.Add(vm);
				updateVmsProcessing(CloudSim.clock());
				vm.Host.updateVmsProcessing(CloudSim.clock());
			}
		}

		/// <summary>
		/// Removes a migrating in vm.
		/// </summary>
		/// <param name="vm"> the vm </param>
		public virtual void removeMigratingInVm(Vm vm)
		{
			vmDeallocate(vm);
			VmsMigratingIn.Remove(vm);
            VmListProperty.Remove(vm);
			VmScheduler.VmsMigratingIn.Remove(vm.Uid);
			vm.InMigration = false;
		}

		/// <summary>
		/// Reallocate migrating in vms. Gets the VM in the migrating in queue
		/// and allocate them on the host.
		/// </summary>
		public virtual void reallocateMigratingInVms()
		{
			foreach (Vm vm in VmsMigratingIn)
			{
				if (!VmListProperty.Contains(vm))
				{
                    VmListProperty.Add(vm);
				}
				if (!VmScheduler.VmsMigratingIn.Contains(vm.Uid))
				{
					VmScheduler.VmsMigratingIn.Add(vm.Uid);
				}
				RamProvisioner.allocateRamForVm(vm, vm.CurrentRequestedRam);
				BwProvisioner.allocateBwForVm(vm, vm.CurrentRequestedBw);
				VmScheduler.allocatePesForVm(vm, vm.CurrentRequestedMips);
				Storage = Storage - vm.Size;
			}
		}

		/// <summary>
		/// Checks if the host is suitable for vm. If it has enough resources
		/// to attend the VM.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> true, if is suitable for vm </returns>
		public virtual bool isSuitableForVm(Vm vm)
		{
			return (VmScheduler.PeCapacity >= vm.CurrentRequestedMaxMips && VmScheduler.AvailableMips >= vm.CurrentRequestedTotalMips && RamProvisioner.isSuitableForVm(vm, vm.CurrentRequestedRam) && BwProvisioner.isSuitableForVm(vm, vm.CurrentRequestedBw));
		}

		/// <summary>
		/// Try to allocate resources to a new VM in the Host.
		/// </summary>
		/// <param name="vm"> Vm being started </param>
		/// <returns> $true if the VM could be started in the host; $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public virtual bool vmCreate(Vm vm)
		{
			if (Storage < vm.Size)
			{
				Log.printConcatLine("[VmScheduler.vmCreate] Allocation of VM #", vm.Id, " to Host #", Id, " failed by storage");
				return false;
			}

			if (!RamProvisioner.allocateRamForVm(vm, vm.CurrentRequestedRam))
			{
				Log.printConcatLine("[VmScheduler.vmCreate] Allocation of VM #", vm.Id, " to Host #", Id, " failed by RAM");
				return false;
			}

			if (!BwProvisioner.allocateBwForVm(vm, vm.CurrentRequestedBw))
			{
				Log.printConcatLine("[VmScheduler.vmCreate] Allocation of VM #", vm.Id, " to Host #", Id, " failed by BW");
				RamProvisioner.deallocateRamForVm(vm);
				return false;
			}

			if (!VmScheduler.allocatePesForVm(vm, vm.CurrentRequestedMips))
			{
				Log.printConcatLine("[VmScheduler.vmCreate] Allocation of VM #", vm.Id, " to Host #", Id, " failed by MIPS");
				RamProvisioner.deallocateRamForVm(vm);
				BwProvisioner.deallocateBwForVm(vm);
				return false;
			}

			Storage = Storage - vm.Size;
            VmListProperty.Add(vm);
			vm.Host = this;
			return true;
		}

		/// <summary>
		/// Destroys a VM running in the host.
		/// </summary>
		/// <param name="vm"> the VM
		/// @pre $none
		/// @post $none </param>
		public virtual void vmDestroy(Vm vm)
		{
			if (vm != null)
			{
				vmDeallocate(vm);
                VmListProperty.Remove(vm);
				vm.Host = null;
			}
		}

		/// <summary>
		/// Destroys all VMs running in the host.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public virtual void vmDestroyAll()
		{
			vmDeallocateAll();
			foreach (Vm vm in VmListProperty)
			{
				vm.Host = null;
				Storage = Storage + vm.Size;
			}
            VmListProperty.Clear();
		}

		/// <summary>
		/// Deallocate all resources of a VM.
		/// </summary>
		/// <param name="vm"> the VM </param>
		protected internal virtual void vmDeallocate(Vm vm)
		{
			RamProvisioner.deallocateRamForVm(vm);
			BwProvisioner.deallocateBwForVm(vm);
			VmScheduler.deallocatePesForVm(vm);
			Storage = Storage + vm.Size;
		}

		/// <summary>
		/// Deallocate all resources of all VMs.
		/// </summary>
		protected internal virtual void vmDeallocateAll()
		{
			RamProvisioner.deallocateRamForAllVms();
			BwProvisioner.deallocateBwForAllVms();
			VmScheduler.deallocatePesForAllVms();
		}

		/// <summary>
		/// Gets a VM by its id and user.
		/// </summary>
		/// <param name="vmId"> the vm id </param>
		/// <param name="userId"> ID of VM's owner </param>
		/// <returns> the virtual machine object, $null if not found
		/// @pre $none
		/// @post $none </returns>
		public virtual Vm getVm(int vmId, int userId)
		{
			foreach (Vm vm in VmListProperty)
			{
				if (vm.Id == vmId && vm.UserId == userId)
				{
					return vm;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the pes number.
		/// </summary>
		/// <returns> the pes number </returns>
		public virtual int NumberOfPes
		{
			get
			{
				return PeListProperty.Count;
			}
		}

		/// <summary>
		/// Gets the free pes number.
		/// </summary>
		/// <returns> the free pes number </returns>
		public virtual int NumberOfFreePes
		{
			get
			{
				return PeList.getNumberOfFreePes(PeListProperty);
			}
		}

		/// <summary>
		/// Gets the total mips.
		/// </summary>
		/// <returns> the total mips </returns>
		public virtual int TotalMips
		{
			get
			{
				return PeList.getTotalMips(PeListProperty);
			}
		}

		/// <summary>
		/// Allocates PEs for a VM.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <param name="mipsShare"> the list of MIPS share to be allocated to the VM </param>
		/// <returns> $true if this policy allows a new VM in the host, $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public virtual bool allocatePesForVm(Vm vm, IList<double?> mipsShare)
		{
			return VmScheduler.allocatePesForVm(vm, mipsShare);
		}

		/// <summary>
		/// Releases PEs allocated to a VM.
		/// </summary>
		/// <param name="vm"> the vm
		/// @pre $none
		/// @post $none </param>
		public virtual void deallocatePesForVm(Vm vm)
		{
			VmScheduler.deallocatePesForVm(vm);
		}

		/// <summary>
		/// Gets the MIPS share of each Pe that is allocated to a given VM.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> an array containing the amount of MIPS of each pe that is available to the VM
		/// @pre $none
		/// @post $none </returns>
		public virtual IList<double?> getAllocatedMipsForVm(Vm vm)
		{
			return VmScheduler.getAllocatedMipsForVm(vm);
		}

		/// <summary>
		/// Gets the total allocated MIPS for a VM along all its PEs.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> the allocated mips for vm </returns>
		public virtual double getTotalAllocatedMipsForVm(Vm vm)
		{
			return VmScheduler.getTotalAllocatedMipsForVm(vm);
		}

		/// <summary>
		/// Returns the maximum available MIPS among all the PEs of the host.
		/// </summary>
		/// <returns> max mips </returns>
		public virtual double MaxAvailableMips
		{
			get
			{
				return VmScheduler.MaxAvailableMips;
			}
		}

		/// <summary>
		/// Gets the total free MIPS available at the host.
		/// </summary>
		/// <returns> the free mips </returns>
		public virtual double AvailableMips
		{
			get
			{
				return VmScheduler.AvailableMips;
			}
		}

		/// <summary>
		/// Gets the host bw.
		/// </summary>
		/// <returns> the host bw
		/// @pre $none
		/// @post $result > 0 </returns>
		public virtual long Bw
		{
			get
			{
				return BwProvisioner.Bw;
			}
		}

		/// <summary>
		/// Gets the host memory.
		/// </summary>
		/// <returns> the host memory
		/// @pre $none
		/// @post $result > 0 </returns>
		public virtual int Ram
		{
			get
			{
				return RamProvisioner.Ram;
			}
		}

		/// <summary>
		/// Gets the host storage.
		/// </summary>
		/// <returns> the host storage
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual long Storage
		{
			get
			{
				return storage;
			}
			set
			{
				this.storage = value;
			}
		}

		/// <summary>
		/// Gets the host id.
		/// </summary>
		/// <returns> the host id </returns>
		public virtual int Id
		{
			get
			{
				return id;
			}
			set
			{
				this.id = value;
			}
		}


		/// <summary>
		/// Gets the ram provisioner.
		/// </summary>
		/// <returns> the ram provisioner </returns>
		public virtual RamProvisioner RamProvisioner
		{
			get
			{
				return ramProvisioner;
			}
			set
			{
				this.ramProvisioner = value;
			}
		}


		/// <summary>
		/// Gets the bw provisioner.
		/// </summary>
		/// <returns> the bw provisioner </returns>
		public virtual BwProvisioner BwProvisioner
		{
			get
			{
				return bwProvisioner;
			}
			set
			{
				this.bwProvisioner = value;
			}
		}


		/// <summary>
		/// Gets the VM scheduler.
		/// </summary>
		/// <returns> the VM scheduler </returns>
		public virtual VmScheduler VmScheduler
		{
			get
			{
				return vmScheduler;
			}
			set
			{
				this.vmScheduler = value;
			}
		}


        /// <summary>
        /// Gets the pe list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the pe list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends Pe> java.util.List<T> getPeList()
        //public virtual IList<T> getPeList<T>() where T : Pe
        public virtual IList<Pe> PeListProperty
        {
			get
			{
				return (IList< Pe>) peList;
			}
			set
			{
				this.peList = value;
			}
		}


        /// <summary>
        /// Gets the vm list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the vm list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends Vm> java.util.List<T> getVmList()
        //public virtual IList<Vm> getVmList
        public virtual IList<Vm> VmListProperty
        {
			get
			{
				return (IList<Vm>) vmList;
			}
		}


		/// <summary>
		/// Checks if the host PEs have failed.
		/// </summary>
		/// <returns> true, if the host PEs have failed; false otherwise </returns>
		public virtual bool Failed
		{
			get
			{
				return failed;
			}
            set
            {
                failed = value;
            }
		}

		/// <summary>
		/// Sets the PEs of the host to a FAILED status. NOTE: <tt>resName</tt> is used for debugging
		/// purposes, which is <b>ON</b> by default. Use <seealso cref="#setFailed(boolean)"/> if you do not want
		/// this information.
		/// </summary>
		/// <param name="resName"> the name of the resource </param>
		/// <param name="failed"> the failed </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setFailed(string resName, bool failed)
		{
			// all the PEs are failed (or recovered, depending on fail)
			this.failed = failed;
			PeList.setStatusFailed(PeListProperty, resName, Id, failed);
			return true;
		}

		/// <summary>
		/// Sets the PEs of the host to a FAILED status.
		/// </summary>
		/// <param name="failed"> the failed </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setFailed(bool failed)
		{
			// all the PEs are failed (or recovered, depending on fail)
			this.failed = failed;
			PeList.setStatusFailed(PeListProperty, failed);
			return true;
		}

		/// <summary>
		/// Sets the particular Pe status on the host.
		/// </summary>
		/// <param name="peId"> the pe id </param>
		/// <param name="status"> Pe status, either <tt>Pe.FREE</tt> or <tt>Pe.BUSY</tt> </param>
		/// <returns> <tt>true</tt> if the Pe status has changed, <tt>false</tt> otherwise (Pe id might not
		///         be exist)
		/// @pre peID >= 0
		/// @post $none </returns>
		public virtual bool setPeStatus(int peId, int status)
		{
			return PeList.setPeStatus(PeListProperty, peId, status);
		}

		/// <summary>
		/// Gets the vms migrating in.
		/// </summary>
		/// <returns> the vms migrating in </returns>
		public virtual IList<Vm> VmsMigratingIn
		{
			get
			{
				return vmsMigratingIn;
			}
		}

		/// <summary>
		/// Gets the data center of the host.
		/// </summary>
		/// <returns> the data center where the host runs </returns>
		public virtual Datacenter Datacenter
		{
			get
			{
				return datacenter;
			}
			set
			{
				this.datacenter = value;
			}
		}


	}

}