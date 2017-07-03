using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

	using ContainerVmBwProvisioner = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmBwProvisioner;
	using ContainerVmRamProvisioner = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmRamProvisioner;
	using ContainerVmScheduler = org.cloudbus.cloudsim.container.schedulers.ContainerVmScheduler;
	using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
	using ContainerVmPeList = org.cloudbus.cloudsim.container.lists.ContainerVmPeList;
	using org.cloudbus.cloudsim;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerHost
	{


		/// <summary>
		/// The id.
		/// </summary>
		private int id;

		/// <summary>
		/// The storage.
		/// </summary>
		private long storage;

		/// <summary>
		/// The ram provisioner.
		/// </summary>
		private ContainerVmRamProvisioner containerVmRamProvisioner;

		/// <summary>
		/// The bw provisioner.
		/// </summary>
		private ContainerVmBwProvisioner containerVmBwProvisioner;

		/// <summary>
		/// The allocation policy.
		/// </summary>
		private ContainerVmScheduler containerVmScheduler;

		/// <summary>
		/// The vm list.
		/// </summary>
		private readonly IList<ContainerVm> vmList = new List<ContainerVm>();

		/// <summary>
		/// The pe list.
		/// </summary>
		private IList<ContainerVmPe> peList;

		/// <summary>
		/// Tells whether this machine is working properly or has failed.
		/// </summary>
		private bool failed;

		/// <summary>
		/// The vms migrating in.
		/// </summary>
		private readonly IList<ContainerVm> vmsMigratingIn = new List<ContainerVm>();

		/// <summary>
		/// The datacenter where the host is placed.
		/// </summary>
		private ContainerDatacenter datacenter;

        /// <summary>
        /// Instantiates a new host.
        /// </summary>
        /// <param name="id">             the id </param>
        /// <param name="containerVmRamProvisioner"> the ram provisioner </param>
        /// <param name="containerVmBwProvisioner">  the bw provisioner </param>
        /// <param name="storage">        the storage </param>
        /// <param name="peList">         the pe list </param>
        /// <param name="containerVmScheduler">    the vm scheduler </param>
        //public ContainerHost<T1>(int id, ContainerVmRamProvisioner containerVmRamProvisioner, ContainerVmBwProvisioner containerVmBwProvisioner, long storage, IList<T1> peList, ContainerVmScheduler containerVmScheduler) where T1 : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public ContainerHost(int id, ContainerVmRamProvisioner containerVmRamProvisioner, ContainerVmBwProvisioner containerVmBwProvisioner, long storage, IList<ContainerVmPe> peList, ContainerVmScheduler containerVmScheduler)
		{
			Id = id;
			ContainerVmRamProvisioner = containerVmRamProvisioner;
			ContainerVmBwProvisioner = containerVmBwProvisioner;
			Storage = storage;
			ContainerVmScheduler = containerVmScheduler;
			PeListProperty = peList;
			Failed = false;

		}

		/// <summary>
		/// Requests updating of processing of cloudlets in the VMs running in this host.
		/// </summary>
		/// <param name="currentTime"> the current time </param>
		/// <returns> expected time of completion of the next cloudlet in all VMs in this host.
		/// Double.MAX_VALUE if there is no future events expected in this host
		/// @pre currentTime >= 0.0
		/// @post $none </returns>
		public virtual double updateContainerVmsProcessing(double currentTime)
		{
			double smallerTime = double.MaxValue;

			foreach (ContainerVm containerVm in VmListProperty)
			{
				double time = containerVm.updateVmProcessing(currentTime, ContainerVmScheduler.getAllocatedMipsForContainerVm(containerVm));
				if (time > 0.0 && time < smallerTime)
				{
					smallerTime = time;
				}
			}

			return smallerTime;
		}

		/// <summary>
		/// Adds the migrating in vm.
		/// </summary>
		/// <param name="containerVm"> the vm </param>
		public virtual void addMigratingInContainerVm(ContainerVm containerVm)
		{
			//Log.printLine("Host: addMigratingInContainerVm:......");
			containerVm.InMigration = true;

			if (!VmsMigratingIn.Contains(containerVm))
			{
				if (Storage < containerVm.Size)
				{
					Log.printConcatLine("[VmScheduler.addMigratingInContainerVm] Allocation of VM #", containerVm.Id, " to Host #", Id, " failed by storage");
                    //Environment.Exit(0);
                    throw new ArgumentException("Allocation of VM failed", "containerVm");
				}

				if (!ContainerVmRamProvisioner.allocateRamForContainerVm(containerVm, containerVm.CurrentRequestedRam))
				{
					Log.printConcatLine("[VmScheduler.addMigratingInContainerVm] Allocation of VM #", containerVm.Id, " to Host #", Id, " failed by RAM");
                    //Environment.Exit(0);
                    throw new ArgumentException("Allocation of VM failed", "containerVm");
                }

				if (!ContainerVmBwProvisioner.allocateBwForContainerVm(containerVm, containerVm.CurrentRequestedBw))
				{
					Log.printConcatLine("[VmScheduler.addMigratingInContainerVm] Allocation of VM #", containerVm.Id, " to Host #", Id, " failed by BW");
                    //Environment.Exit(0);
                    throw new ArgumentException("Allocation of VM failed", "containerVm");
                }

				ContainerVmScheduler.VmsMigratingIn.Add(containerVm.Uid);
				if (!ContainerVmScheduler.allocatePesForVm(containerVm, containerVm.CurrentRequestedMips))
				{
					Log.printConcatLine("[VmScheduler.addMigratingInContainerVm] Allocation of VM #", containerVm.Id, " to Host #", Id, " failed by MIPS");
                    //Environment.Exit(0);
                    throw new ArgumentException("Allocation of VM failed", "containerVm");
                }

				Storage = Storage - containerVm.Size;

				VmsMigratingIn.Add(containerVm);
                VmListProperty.Add(containerVm);
				updateContainerVmsProcessing(CloudSim.clock());
				containerVm.Host.updateContainerVmsProcessing(CloudSim.clock());
			}
		}

			/// <summary>
			/// Removes the migrating in vm.
			/// </summary>
			/// <param name="vm"> the vm </param>
			public virtual void removeMigratingInContainerVm(ContainerVm vm)
			{
				containerVmDeallocate(vm);
				VmsMigratingIn.Remove(vm);
				VmListProperty.Remove(vm);
				ContainerVmScheduler.VmsMigratingIn.Remove(vm.Uid);
				vm.InMigration = false;
			}

		/// <summary>
		/// Reallocate migrating in vms.
		/// </summary>
		public virtual void reallocateMigratingInContainerVms()
		{
			foreach (ContainerVm containerVm in VmsMigratingIn)
			{
				if (!VmListProperty.Contains(containerVm))
				{
                    VmListProperty.Add(containerVm);
				}
				if (!ContainerVmScheduler.VmsMigratingIn.Contains(containerVm.Uid))
				{
					ContainerVmScheduler.VmsMigratingIn.Add(containerVm.Uid);
				}
				ContainerVmRamProvisioner.allocateRamForContainerVm(containerVm, containerVm.CurrentRequestedRam);
				ContainerVmBwProvisioner.allocateBwForContainerVm(containerVm, containerVm.CurrentRequestedBw);
				ContainerVmScheduler.allocatePesForVm(containerVm, containerVm.CurrentRequestedMips);
				Storage = Storage - containerVm.Size;
			}
		}

		/// <summary>
		/// Checks if is suitable for vm.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> true, if is suitable for vm </returns>
		public virtual bool isSuitableForContainerVm(ContainerVm vm)
		{
			//Log.printLine("Host: Is suitable for VM???......");
			return (ContainerVmScheduler.PeCapacity >= vm.CurrentRequestedMaxMips && ContainerVmScheduler.AvailableMips >= vm.CurrentRequestedTotalMips && ContainerVmRamProvisioner.isSuitableForContainerVm(vm, vm.CurrentRequestedRam) && ContainerVmBwProvisioner.isSuitableForContainerVm(vm, vm.CurrentRequestedBw));
		}

		/// <summary>
		/// Allocates PEs and memory to a new VM in the Host.
		/// </summary>
		/// <param name="vm"> Vm being started </param>
		/// <returns> $true if the VM could be started in the host; $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public virtual bool containerVmCreate(ContainerVm vm)
		{
			//Log.printLine("Host: Create VM???......" + vm.getId());
			if (Storage < vm.Size)
			{
				Log.printConcatLine("[VmScheduler.containerVmCreate] Allocation of VM #", vm.Id, " to Host #", Id, " failed by storage");
				return false;
			}

			if (!ContainerVmRamProvisioner.allocateRamForContainerVm(vm, vm.CurrentRequestedRam))
			{
				Log.printConcatLine("[VmScheduler.containerVmCreate] Allocation of VM #", vm.Id, " to Host #", Id, " failed by RAM");
				return false;
			}

			if (!ContainerVmBwProvisioner.allocateBwForContainerVm(vm, vm.CurrentRequestedBw))
			{
				Log.printConcatLine("[VmScheduler.containerVmCreate] Allocation of VM #", vm.Id, " to Host #", Id, " failed by BW");
				ContainerVmRamProvisioner.deallocateRamForContainerVm(vm);
				return false;
			}

			if (!ContainerVmScheduler.allocatePesForVm(vm, vm.CurrentRequestedMips))
			{
				Log.printConcatLine("[VmScheduler.containerVmCreate] Allocation of VM #", vm.Id, " to Host #", Id, " failed by MIPS");
				ContainerVmRamProvisioner.deallocateRamForContainerVm(vm);
				ContainerVmBwProvisioner.deallocateBwForContainerVm(vm);
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
		/// <param name="containerVm"> the VM
		/// @pre $none
		/// @post $none </param>
		public virtual void containerVmDestroy(ContainerVm containerVm)
		{
			//Log.printLine("Host:  Destroy Vm:.... " + containerVm.getId());
			if (containerVm != null)
			{
				containerVmDeallocate(containerVm);
                VmListProperty.Remove(containerVm);
				containerVm.Host = null;
			}
		}

		/// <summary>
		/// Destroys all VMs running in the host.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public virtual void containerVmDestroyAll()
		{
			//Log.printLine("Host: Destroy all Vms");
			containerVmDeallocateAll();
			foreach (ContainerVm containerVm in VmListProperty)
			{
				containerVm.Host = null;
				Storage = Storage + containerVm.Size;
			}
            VmListProperty.Clear();
		}

		/// <summary>
		/// Deallocate all hostList for the VM.
		/// </summary>
		/// <param name="containerVm"> the VM </param>
		protected internal virtual void containerVmDeallocate(ContainerVm containerVm)
		{
			//Log.printLine("Host: Deallocated the VM:......" + containerVm.getId());
			ContainerVmRamProvisioner.deallocateRamForContainerVm(containerVm);
			ContainerVmBwProvisioner.deallocateBwForContainerVm(containerVm);
			ContainerVmScheduler.deallocatePesForVm(containerVm);
			Storage = Storage + containerVm.Size;
		}

		/// <summary>
		/// Deallocate all hostList for the VM.
		/// </summary>
		protected internal virtual void containerVmDeallocateAll()
		{
			//Log.printLine("Host: Deallocate all the Vms......");
			ContainerVmRamProvisioner.deallocateRamForAllContainerVms();
			ContainerVmBwProvisioner.deallocateBwForAllContainerVms();
			ContainerVmScheduler.deallocatePesForAllContainerVms();
		}

		/// <summary>
		/// Returns a VM object.
		/// </summary>
		/// <param name="vmId">   the vm id </param>
		/// <param name="userId"> ID of VM's owner </param>
		/// <returns> the virtual machine object, $null if not found
		/// @pre $none
		/// @post $none </returns>
		public virtual ContainerVm getContainerVm(int vmId, int userId)
		{
			//Log.printLine("Host: get the vm......" + vmId);
			//Log.printLine("Host: the vm list size:......" + getVmList().size());
			foreach (ContainerVm containerVm in VmListProperty)
			{
				if (containerVm.Id == vmId && containerVm.UserId == userId)
				{
					return containerVm;
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
				//Log.printLine("Host: get the peList Size......" + getPeList().size());
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
				//Log.printLine("Host: get the free Pes......" + ContainerVmPeList.getNumberOfFreePes(getPeList()));
				return ContainerVmPeList.getNumberOfFreePes(PeListProperty);
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
				//Log.printLine("Host: get the total mips......" + ContainerVmPeList.getTotalMips(getPeList()));
				return ContainerVmPeList.getTotalMips(PeListProperty);
			}
		}

		/// <summary>
		/// Allocates PEs for a VM.
		/// </summary>
		/// <param name="containerVm">        the vm </param>
		/// <param name="mipsShare"> the mips share </param>
		/// <returns> $true if this policy allows a new VM in the host, $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public virtual bool allocatePesForContainerVm(ContainerVm containerVm, IList<double?> mipsShare)
		{
			//Log.printLine("Host: allocate Pes for Vm:......" + containerVm.getId());
			return ContainerVmScheduler.allocatePesForVm(containerVm, mipsShare);
		}

		/// <summary>
		/// Releases PEs allocated to a VM.
		/// </summary>
		/// <param name="containerVm"> the vm
		/// @pre $none
		/// @post $none </param>
		public virtual void deallocatePesForContainerVm(ContainerVm containerVm)
		{
			//Log.printLine("Host: deallocate Pes for Vm:......" + containerVm.getId());
			ContainerVmScheduler.deallocatePesForVm(containerVm);
		}

		/// <summary>
		/// Returns the MIPS share of each Pe that is allocated to a given VM.
		/// </summary>
		/// <param name="containerVm"> the vm </param>
		/// <returns> an array containing the amount of MIPS of each pe that is available to the VM
		/// @pre $none
		/// @post $none </returns>
		public virtual IList<double?> getAllocatedMipsForContainerVm(ContainerVm containerVm)
		{
			//Log.printLine("Host: get allocated Pes for Vm:......" + containerVm.getId());
			return ContainerVmScheduler.getAllocatedMipsForContainerVm(containerVm);
		}

		/// <summary>
		/// Gets the total allocated MIPS for a VM over all the PEs.
		/// </summary>
		/// <param name="containerVm"> the vm </param>
		/// <returns> the allocated mips for vm </returns>
		public virtual double getTotalAllocatedMipsForContainerVm(ContainerVm containerVm)
		{
			//Log.printLine("Host: total allocated Pes for Vm:......" + containerVm.getId());
			return ContainerVmScheduler.getTotalAllocatedMipsForContainerVm(containerVm);
		}

		/// <summary>
		/// Returns maximum available MIPS among all the PEs.
		/// </summary>
		/// <returns> max mips </returns>
		public virtual double MaxAvailableMips
		{
			get
			{
				//Log.printLine("Host: Maximum Available Pes:......");
				return ContainerVmScheduler.MaxAvailableMips;
			}
		}

		/// <summary>
		/// Gets the free mips.
		/// </summary>
		/// <returns> the free mips </returns>
		public virtual double AvailableMips
		{
			get
			{
				//Log.printLine("Host: Get available Mips");
				return ContainerVmScheduler.AvailableMips;
			}
		}

		/// <summary>
		/// Gets the machine bw.
		/// </summary>
		/// <returns> the machine bw
		/// @pre $none
		/// @post $result > 0 </returns>
		public virtual long Bw
		{
			get
			{
				//Log.printLine("Host: Get BW:......" + getContainerVmBwProvisioner().getBw());
				return ContainerVmBwProvisioner.Bw;
			}
		}

		/// <summary>
		/// Gets the machine memory.
		/// </summary>
		/// <returns> the machine memory
		/// @pre $none
		/// @post $result > 0 </returns>
		public virtual float Ram
		{
			get
			{
				//Log.printLine("Host: Get Ram:......" + getContainerVmRamProvisioner().getRam());
    
				return ContainerVmRamProvisioner.Ram;
			}
		}

		/// <summary>
		/// Gets the machine storage.
		/// </summary>
		/// <returns> the machine storage
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
		/// Gets the id.
		/// </summary>
		/// <returns> the id </returns>
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
		public virtual ContainerVmRamProvisioner ContainerVmRamProvisioner
		{
			get
			{
				return containerVmRamProvisioner;
			}
			set
			{
				this.containerVmRamProvisioner = value;
			}
		}


		/// <summary>
		/// Gets the bw provisioner.
		/// </summary>
		/// <returns> the bw provisioner </returns>
		public virtual ContainerVmBwProvisioner ContainerVmBwProvisioner
		{
			get
			{
				return containerVmBwProvisioner;
			}
			set
			{
				this.containerVmBwProvisioner = value;
			}
		}


		/// <summary>
		/// Gets the VM scheduler.
		/// </summary>
		/// <returns> the VM scheduler </returns>
		public virtual ContainerVmScheduler ContainerVmScheduler
		{
			get
			{
				return containerVmScheduler;
			}
			set
			{
				this.containerVmScheduler = value;
			}
		}


        /// <summary>
        /// Gets the pe list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the pe list </returns>
        public virtual IList<ContainerVmPe> PeListProperty
        {
			get
			{
				return (IList<ContainerVmPe>) peList;
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
        public virtual IList<ContainerVm> VmListProperty
        {
			get
			{
				return (IList<ContainerVm>) vmList;
			}
		}


		/// <summary>
		/// Checks if is failed.
		/// </summary>
		/// <returns> true, if is failed </returns>
		public virtual bool Failed
		{
			get
			{
				return failed;
			}

            private set
            {
                failed = value;
            }
		}

		/// <summary>
		/// Sets the PEs of this machine to a FAILED status. NOTE: <tt>resName</tt> is used for debugging
		/// purposes, which is <b>ON</b> by default. Use <seealso cref="#setFailed(boolean)"/> if you do not want
		/// this information.
		/// </summary>
		/// <param name="resName"> the name of the resource </param>
		/// <param name="failed">  the failed </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setFailed(string resName, bool failed)
		{
			// all the PEs are failed (or recovered, depending on fail)
			this.failed = failed;
			ContainerVmPeList.setStatusFailed(PeListProperty, resName, Id, failed);
			return true;
		}

		/// <summary>
		/// Sets the PEs of this machine to a FAILED status.
		/// </summary>
		/// <param name="failed"> the failed </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setFailed(bool failed)
		{
			// all the PEs are failed (or recovered, depending on fail)
			this.failed = failed;
			ContainerVmPeList.setStatusFailed(PeListProperty, failed);
			return true;
		}

		/// <summary>
		/// Sets the particular Pe status on this Machine.
		/// </summary>
		/// <param name="peId">   the pe id </param>
		/// <param name="status"> Pe status, either <tt>Pe.FREE</tt> or <tt>Pe.BUSY</tt> </param>
		/// <returns> <tt>true</tt> if the Pe status has changed, <tt>false</tt> otherwise (Pe id might not
		/// be exist)
		/// @pre peID >= 0
		/// @post $none </returns>
		public virtual bool setPeStatus(int peId, int status)
		{
			return ContainerVmPeList.setPeStatus(PeListProperty, peId, status);
		}

		/// <summary>
		/// Gets the vms migrating in.
		/// </summary>
		/// <returns> the vms migrating in </returns>
		public virtual IList<ContainerVm> VmsMigratingIn
		{
			get
			{
				return vmsMigratingIn;
			}
		}

		/// <summary>
		/// Gets the data center.
		/// </summary>
		/// <returns> the data center where the host runs </returns>
		public virtual ContainerDatacenter Datacenter
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