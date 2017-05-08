using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.schedulers
{

	using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
	using ContainerPeList = org.cloudbus.cloudsim.container.lists.ContainerPeList;
	using Container = org.cloudbus.cloudsim.container.core.Container;


	/// <summary>
	/// Created by sareh on 9/07/15.
	/// </summary>
	public abstract class ContainerScheduler
	{
		/// <summary>
		/// The peList. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe> peList;
		private IList<ContainerPe> peList;

		/// <summary>
		/// The map of VMs to PEs. </summary>
		private IDictionary<string, IList<ContainerPe>> peMap;

		/// <summary>
		/// The MIPS that are currently allocated to the VMs. </summary>
		private IDictionary<string, IList<double?>> mipsMap;

		/// <summary>
		/// The total available mips. </summary>
		private double availableMips;

		/// <summary>
		/// The VMs migrating in. </summary>
		private IList<string> containersMigratingIn;

		/// <summary>
		/// The VMs migrating out. </summary>
		private IList<string> containersMigratingOut;


        /// <summary>
        /// Creates a new HostAllocationPolicy.
        /// </summary>
        /// <param name="pelist"> the pelist
        /// @pre peList != $null
        /// @post $none </param>
        //public ContainerScheduler<T1>(IList<T1> pelist) where T1 : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public ContainerScheduler(IList<ContainerPe> pelist)
        {
			PeListProperty = pelist;
			PeMap = new Dictionary<string, IList<ContainerPe>>();
			MipsMap = new Dictionary<string, IList<double?>>();
			AvailableMips = ContainerPeList.getTotalMips(PeListProperty);
			ContainersMigratingIn = new List<string>();
			ContainersMigratingOut = new List<string>();

		}

		/// <summary>
		/// Allocates PEs for a VM.
		/// </summary>
		/// <param name="container"> the container </param>
		/// <param name="mipsShare"> the mips share </param>
		/// <returns> $true if this policy allows a new VM in the host, $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocatePesForContainer(Container container, IList<double?> mipsShare);

		/// <summary>
		/// Releases PEs allocated to a VM.
		/// </summary>
		/// <param name="container"> the container
		/// @pre $none
		/// @post $none </param>
		public abstract void deallocatePesForContainer(Container container);

		/// <summary>
		/// Releases PEs allocated to all the VMs.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public virtual void deallocatePesForAllContainers()
		{
			MipsMap.Clear();
			AvailableMips = ContainerPeList.getTotalMips(PeListProperty);
			foreach (ContainerPe pe in PeListProperty)
			{
				 pe.ContainerPeProvisionerProperty.deallocateMipsForAllContainers();
			}
		}

		/// <summary>
		/// Gets the pes allocated for container.
		/// </summary>
		/// <param name="container"> the container </param>
		/// <returns> the pes allocated for container </returns>
		public virtual IList<ContainerPe> getPesAllocatedForVM(Container container)
		{
			return PeMap[container.Uid];
		}

		/// <summary>
		/// Returns the MIPS share of each Pe that is allocated to a given VM.
		/// </summary>
		/// <param name="container"> the container </param>
		/// <returns> an array containing the amount of MIPS of each pe that is available to the VM
		/// @pre $none
		/// @post $none </returns>
		public virtual IList<double?> getAllocatedMipsForContainer(Container container)
		{
			return MipsMap[container.Uid];
		}

		/// <summary>
		/// Gets the total allocated MIPS for a VM over all the PEs.
		/// </summary>
		/// <param name="container"> the container </param>
		/// <returns> the allocated mips for container </returns>
		public virtual double getTotalAllocatedMipsForContainer(Container container)
		{
			double allocated = 0;
			IList<double?> mipsMap = getAllocatedMipsForContainer(container);
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
				foreach (ContainerPe pe in PeListProperty)
				{
					double tmp = (pe.ContainerPeProvisionerProperty.AvailableMips);
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
        /// Gets the container list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the container list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe> java.util.List<T> getPeList()
        //public virtual IList<T> getPeList<T>() where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public virtual IList<ContainerPe> PeListProperty
        {
			get
			{
				return (IList<ContainerPe>) peList;
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
		//        Log.printConcatLine("The available Mips is ", availableMips);
				return availableMips;
			}
			set
			{
				this.availableMips = value;
			}
		}


		/// <summary>
		/// Gets the containers in migration.
		/// </summary>
		/// <returns> the containers in migration </returns>
		public virtual IList<string> ContainersMigratingOut
		{
			get
			{
				return containersMigratingOut;
			}
			set
			{
				containersMigratingOut = value;
			}
		}


		/// <summary>
		/// Gets the containers migrating in.
		/// </summary>
		/// <returns> the containers migrating in </returns>
		public virtual IList<string> ContainersMigratingIn
		{
			get
			{
				return containersMigratingIn;
			}
			set
			{
				this.containersMigratingIn = value;
			}
		}


		/// <summary>
		/// Gets the pe map.
		/// </summary>
		/// <returns> the pe map </returns>
		public virtual IDictionary<string, IList<ContainerPe>> PeMap
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





	//
	//
	//
	//    public abstract double getTotalUtilizationOfCpu(double time);
	//    public abstract double getCurrentRequestedUtilizationOfRam();
	//    public abstract double getCurrentRequestedUtilizationOfBw();
	//    public abstract List<Double> getCurrentRequestedMips();
	//    public abstract double updateContainerProcessing(double currentTime, List<Double> mipsShare);

	}

}