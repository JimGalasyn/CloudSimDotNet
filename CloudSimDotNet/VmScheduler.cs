using System.Collections.Generic;

/*
 * Title: CloudSim Toolkit Description: CloudSim (Cloud Simulation) Toolkit for Modeling and
 * Simulation of Clouds Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * 
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	using PeList = org.cloudbus.cloudsim.lists.PeList;

	/// <summary>
	/// VmScheduler is an abstract class that represents the policy used by a Virtual Machine Monitor (VMM) 
	/// to share processing power of a PM among VMs running in a host. 
	/// 
	/// Each host has to use is own instance of a VmScheduler
	/// that will so schedule the allocation of host's PEs for VMs running on it.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public abstract class VmScheduler
	{

		/// <summary>
		/// The PEs of the host where the scheduler is associated. </summary>
		private IList<Pe> peList;

		/// <summary>
		/// The map of VMs to PEs, where each key is a VM id and each value is 
		/// a list of PEs allocated to that VM. 
		/// </summary>
		private IDictionary<string, IList<Pe>> peMap;

		/// <summary>
		/// The map of VMs to MIPS, were each key is a VM id and each value is 
		/// the currently allocated MIPS from the respective PE to that VM. 
		/// The PEs where the MIPS capacity is get are defined
		/// in the <seealso cref="#peMap"/>.
		/// 
		/// @todo subclasses such as <seealso cref="VmSchedulerTimeShared"/> have an 
		/// <seealso cref="VmSchedulerTimeShared#mipsMapRequested"/> attribute that
		/// may be confused with this one. So, the name of this one
		/// may be changed to something such as allocatedMipsMap
		/// </summary>
		private IDictionary<string, IList<double?>> mipsMap;

		/// <summary>
		/// The total available MIPS that can be allocated on demand for VMs. </summary>
		private double availableMips;

		/// <summary>
		/// The VMs migrating in the host (arriving). It is the list of VM ids </summary>
		private IList<string> vmsMigratingIn;

		/// <summary>
		/// The VMs migrating out the host (departing). It is the list of VM ids </summary>
		private IList<string> vmsMigratingOut;

        /// <summary>
        /// Creates a new VmScheduler.
        /// </summary>
        /// <param name="pelist"> the list of PEs of the host where the VmScheduler is associated to.
        /// @pre peList != $null
        /// @post $none </param>
        //public VmScheduler<T1>(IList<T1> pelist) where T1 : Pe
        public VmScheduler(IList<Pe> pelist)
		{
            PeListProperty = pelist;
			PeMap = new Dictionary<string, IList<Pe>>();
			MipsMap = new Dictionary<string, IList<double?>>();
			AvailableMips = PeList.getTotalMips(PeListProperty);
			VmsMigratingIn = new List<string>();
			VmsMigratingOut = new List<string>();
		}

		/// <summary>
		/// Requests the allocation of PEs for a VM.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <param name="mipsShare"> the list of MIPS share to be allocated to a VM </param>
		/// <returns> $true if this policy allows a new VM in the host, $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocatePesForVm(Vm vm, IList<double?> mipsShare);

		/// <summary>
		/// Releases PEs allocated to a VM. After that, the PEs may be used
		/// on demand by other VMs.
		/// </summary>
		/// <param name="vm"> the vm
		/// @pre $none
		/// @post $none </param>
		public abstract void deallocatePesForVm(Vm vm);

		/// <summary>
		/// Releases PEs allocated to all the VMs of the host the VmScheduler is associated to.
		/// After that, all PEs will be available to be used on demand for requesting VMs.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public virtual void deallocatePesForAllVms()
		{
			MipsMap.Clear();
			AvailableMips = PeList.getTotalMips(PeListProperty);
			foreach (Pe pe in PeListProperty)
			{
				pe.PeProvisioner.deallocateMipsForAllVms();
			}
		}

		/// <summary>
		/// Gets the pes allocated for a vm.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> the pes allocated for the given vm </returns>
		public virtual IList<Pe> getPesAllocatedForVM(Vm vm)
		{
			return PeMap[vm.Uid];
		}

		/// <summary>
		/// Returns the MIPS share of each host's Pe that is allocated to a given VM.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> an array containing the amount of MIPS of each pe that is available to the VM
		/// @pre $none
		/// @post $none </returns>
		public virtual IList<double?> getAllocatedMipsForVm(Vm vm)
		{
			return MipsMap[vm.Uid];
		}

        /// <summary>
        /// Gets the total allocated MIPS for a VM along all its allocated PEs.
        /// </summary>
        /// <param name="vm"> the vm </param>
        /// <returns> the total allocated mips for the vm </returns>
        /// <remarks>The test suite seems to require that this method
        /// returns 0 when the <see cref="MipsMap"/> is empty.
        /// </remarks>
        public virtual double getTotalAllocatedMipsForVm(Vm vm)
		{
			double allocated = 0;

            if (MipsMap.Count > 0)
            {
                IList<double?> mipsMap = getAllocatedMipsForVm(vm);
                if (mipsMap != null)
                {
                    foreach (double mips in mipsMap)
                    {
                        allocated += mips;
                    }
                }
            }

			return allocated;
		}

		/// <summary>
		/// Returns maximum available MIPS among all the host's PEs.
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
				foreach (Pe pe in PeListProperty)
				{
					double tmp = pe.PeProvisioner.AvailableMips;
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
		/// <returns> mips
		/// @todo It considers that all PEs have the same capacity,
		/// what has been shown doesn't be assured. The peList
		/// received by the VmScheduler can be heterogeneous PEs. </returns>
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
        /// Gets the pe list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the pe list
        /// @todo The warning have to be checked 
        ///  </returns>
        public virtual IList<Pe> PeListProperty
        {
			get
			{
				return (IList<Pe>) peList;
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
		/// Gets the vms migrating out.
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
		public virtual IDictionary<string, IList<Pe>> PeMap
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