using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.schedulers
{

	using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;

	using ContainerVmPeList = org.cloudbus.cloudsim.container.lists.ContainerVmPeList;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;



		/*
	 * Title: CloudSim Toolkit Description: CloudSim (Cloud Simulation) Toolkit for Modeling and
	 * Simulation of Clouds Licence: GPL - http://www.gnu.org/copyleft/gpl.html
	 *
	 * Copyright (c) 2009-2012, The University of Melbourne, Australia
	 */


	/// <summary>
	/// VmScheduler is an abstract class that represents the policy used by a VMM to share processing
	/// power among VMs running in a host.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public abstract class ContainerVmScheduler
	{


		/// <summary>
		/// The peList.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe> peList;
		private IList<ContainerVmPe> peList;

		/// <summary>
		/// The map of VMs to PEs.
		/// </summary>
		private IDictionary<string, IList<ContainerVmPe>> peMap;

		/// <summary>
		/// The MIPS that are currently allocated to the VMs.
		/// </summary>
		private IDictionary<string, IList<double?>> mipsMap;

		/// <summary>
		/// The total available mips.
		/// </summary>
		private double availableMips;

		/// <summary>
		/// The VMs migrating in.
		/// </summary>
		private IList<string> vmsMigratingIn;

		/// <summary>
		/// The VMs migrating out.
		/// </summary>
		private IList<string> vmsMigratingOut;

        /// <summary>
        /// Creates a new HostAllocationPolicy.
        /// </summary>
        /// <param name="pelist"> the pelist
        /// @pre peList != $null
        /// @post $none </param>
        //public ContainerVmScheduler<T1>(IList<T1> pelist) where T1 : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public ContainerVmScheduler(IList<ContainerVmPe> pelist)
		{
            PeListProperty = pelist;
			PeMap = new Dictionary<string, IList<ContainerVmPe>>();
			MipsMap = new Dictionary<string, IList<double?>>();
			AvailableMips = ContainerVmPeList.getTotalMips(PeListProperty);
			VmsMigratingIn = new List<string>();
			VmsMigratingOut = new List<string>();
		}

		/// <summary>
		/// Allocates PEs for a VM.
		/// </summary>
		/// <param name="vm">        the vm </param>
		/// <param name="mipsShare"> the mips share </param>
		/// <returns> $true if this policy allows a new VM in the host, $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocatePesForVm(ContainerVm vm, IList<double?> mipsShare);

		/// <summary>
		/// Releases PEs allocated to a VM.
		/// </summary>
		/// <param name="vm"> the vm
		/// @pre $none
		/// @post $none </param>
		public abstract void deallocatePesForVm(ContainerVm vm);

		/// <summary>
		/// Releases PEs allocated to all the VMs.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public virtual void deallocatePesForAllContainerVms()
		{
			MipsMap.Clear();
			AvailableMips = ContainerVmPeList.getTotalMips(PeListProperty);
			foreach (ContainerVmPe pe in PeListProperty)
			{
				pe.ContainerVmPeProvisioner.deallocateMipsForAllContainerVms();
			}
		}

		/// <summary>
		/// Gets the pes allocated for vm.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> the pes allocated for vm </returns>
		public virtual IList<ContainerVmPe> getPesAllocatedForContainerVM(ContainerVm vm)
		{
			return PeMap[vm.Uid];
		}

		/// <summary>
		/// Returns the MIPS share of each Pe that is allocated to a given VM.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> an array containing the amount of MIPS of each pe that is available to the VM
		/// @pre $none
		/// @post $none </returns>
		public virtual IList<double?> getAllocatedMipsForContainerVm(ContainerVm vm)
		{
			return MipsMap[vm.Uid];
		}

		/// <summary>
		/// Gets the total allocated MIPS for a VM over all the PEs.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> the allocated mips for vm </returns>
		public virtual double getTotalAllocatedMipsForContainerVm(ContainerVm vm)
		{
			double allocated = 0;
			IList<double?> mipsMap = getAllocatedMipsForContainerVm(vm);
			if (mipsMap != null)
			{
				foreach (double mips in mipsMap)
				{
					allocated += mips;
				}
			}
			return allocated;
		}

		/// <summary>
		/// Returns maximum available MIPS among all the PEs.
		/// </summary>
		/// <returns> max mips </returns>
		public virtual double MaxAvailableMips
		{
			get
			{
				if (PeListProperty == null)
				{
					Log.printLine("Pe list is empty");
					return 0;
				}
    
				double max = 0.0;
				foreach (ContainerVmPe pe in PeListProperty)
				{
					double tmp = pe.ContainerVmPeProvisioner.AvailableMips;
					if (tmp > max)
					{
						max = tmp;
					}
				}
    
				return max;
			}
		}

		/// <summary>
		/// Returns PE capacity in MIPS.
		/// </summary>
		/// <returns> mips </returns>
		public virtual double PeCapacity
		{
			get
			{
				if (PeListProperty == null)
				{
					Log.printLine("Pe list is empty");
					return 0;
				}
				return PeListProperty[0].Mips;
			}
		}

        /// <summary>
        /// Gets the vm list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the vm list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe> java.util.List<T> getPeList()
        //public virtual IList<T> getPeList<T>() where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
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
		/// Gets the mips map.
		/// </summary>
		/// <returns> the mips map </returns>
		protected internal virtual IDictionary<string, IList<double?>> MipsMap
		{
			get
			{
				return mipsMap;
			}
			set
			{
				this.mipsMap = value;
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
				return availableMips;
			}
			set
			{
				this.availableMips = value;
			}
		}


		/// <summary>
		/// Gets the vms in migration.
		/// </summary>
		/// <returns> the vms in migration </returns>
		public virtual IList<string> VmsMigratingOut
		{
			get
			{
				return vmsMigratingOut;
			}
			set
			{
				vmsMigratingOut = value;
			}
		}


		/// <summary>
		/// Gets the vms migrating in.
		/// </summary>
		/// <returns> the vms migrating in </returns>
		public virtual IList<string> VmsMigratingIn
		{
			get
			{
				return vmsMigratingIn;
			}
			set
			{
				this.vmsMigratingIn = value;
			}
		}


		/// <summary>
		/// Gets the pe map.
		/// </summary>
		/// <returns> the pe map </returns>
		public virtual IDictionary<string, IList<ContainerVmPe>> PeMap
		{
			get
			{
				return peMap;
			}
			set
			{
				this.peMap = value;
			}
		}


	}



}