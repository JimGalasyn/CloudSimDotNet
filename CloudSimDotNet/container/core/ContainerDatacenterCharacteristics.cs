using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

	using ContainerVmPeList = org.cloudbus.cloudsim.container.lists.ContainerVmPeList;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerDatacenterCharacteristics
	{

		/// <summary>
		/// The resource id -- setup when Resource is created.
		/// </summary>
		private int id;

		/// <summary>
		/// The architecture.
		/// </summary>
		private string architecture;

		/// <summary>
		/// The os.
		/// </summary>
		private string os;

		/// <summary>
		/// The host list.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends ContainerHost> hostList;
		private IList<ContainerHost> hostList;

		/// <summary>
		/// The time zone -- difference from GMT.
		/// </summary>
		private double timeZone;

		/// <summary>
		/// Price/CPU-unit if unit = sec., then G$/CPU-sec.
		/// </summary>
		private double costPerSecond;

		/// <summary>
		/// Resource Types -- allocation policy.
		/// </summary>
		private int allocationPolicy;

		/// <summary>
		/// Time-shared system using Round-Robin algorithm.
		/// </summary>
		public const int TIME_SHARED = 0;

		/// <summary>
		/// Spaced-shared system using First Come First Serve (FCFS) algorithm.
		/// </summary>
		public const int SPACE_SHARED = 1;

		/// <summary>
		/// Assuming all PEs in all Machines have the same rating.
		/// </summary>
		public const int OTHER_POLICY_SAME_RATING = 2;

		/// <summary>
		/// Assuming all PEs in a Machine have the same rating. However, each Machine has different
		/// rating to each other.
		/// </summary>
		public const int OTHER_POLICY_DIFFERENT_RATING = 3;

		/// <summary>
		/// A resource that supports Advanced Reservation mechanisms.
		/// </summary>
		public const int ADVANCE_RESERVATION = 4;

		/// <summary>
		/// The vmm.
		/// </summary>
		private string vmm;

		/// <summary>
		/// The cost per mem.
		/// </summary>
		private double costPerMem;

		/// <summary>
		/// The cost per storage.
		/// </summary>
		private double costPerStorage;

		/// <summary>
		/// The cost per bw.
		/// </summary>
		private double costPerBw;

        /// <summary>
        /// Allocates a new DatacenterCharacteristics object. If the time zone is invalid, then by
        /// default, it will be GMT+0.
        /// </summary>
        /// <param name="architecture">   the architecture of a resource </param>
        /// <param name="os">             the operating system used </param>
        /// <param name="vmm">            the virtual machine monitor used </param>
        /// <param name="hostList">       list of machines in a resource </param>
        /// <param name="timeZone">       local time zone of a user that owns this reservation. Time zone should be of
        ///                       range [GMT-12 ... GMT+13] </param>
        /// <param name="costPerSec">     the cost per sec to use this resource </param>
        /// <param name="costPerMem">     the cost to use memory in this resource </param>
        /// <param name="costPerStorage"> the cost to use storage in this resource </param>
        /// <param name="costPerBw">      the cost per bw
        /// @pre architecture != null
        /// @pre OS != null
        /// @pre VMM != null
        /// @pre machineList != null
        /// @pre timeZone >= -12 && timeZone <= 13
        /// @pre costPerSec >= 0.0
        /// @pre costPerMem >= 0
        /// @pre costPerStorage >= 0
        /// @post $none </param>
        //public ContainerDatacenterCharacteristics<T1>(string architecture, string os, string vmm, IList<T1> hostList, double timeZone, double costPerSec, double costPerMem, double costPerStorage, double costPerBw) where T1 : ContainerHost
        public ContainerDatacenterCharacteristics(string architecture, string os, string vmm, IList<ContainerHost> hostList, double timeZone, double costPerSec, double costPerMem, double costPerStorage, double costPerBw)
		{
			Id = -1;
			Architecture = architecture;
			Os = os;
			HostListProperty = hostList;
			AllocationPolicy = allocationPolicy;
			CostPerSecond = costPerSec;

			TimeZone = 0.0;

			Vmm = vmm;
			CostPerMem = costPerMem;
			CostPerStorage = costPerStorage;
			CostPerBw = costPerBw;
		}

		/// <summary>
		/// Gets the name of a resource.
		/// </summary>
		/// <returns> the resource name
		/// @pre $none
		/// @post $result != null </returns>
		public virtual string ResourceName
		{
			get
			{
				return CloudSim.getEntityName(Id);
			}
		}

		/// <summary>
		/// Gets a Machine with at least one empty Pe.
		/// </summary>
		/// <returns> a Machine object or if not found
		/// @pre $none
		/// @post $none </returns>
		public virtual ContainerHost HostWithFreePe
		{
			get
			{
				return ContainerHostList.getHostWithFreePe(HostListProperty);
			}
		}

		/// <summary>
		/// Gets a Machine with at least a given number of free Pe.
		/// </summary>
		/// <param name="peNumber"> the pe number </param>
		/// <returns> a Machine object or if not found
		/// @pre $none
		/// @post $none </returns>
		public virtual ContainerHost getHostWithFreePe(int peNumber)
		{
			return ContainerHostList.getHostWithFreePe(HostListProperty, peNumber);
		}

		/// <summary>
		/// Gets Millions Instructions Per Second (MIPS) Rating of a Processing Element (Pe). It is
		/// assumed all PEs' rating is same in a given machine.
		/// </summary>
		/// <returns> the MIPS Rating or if no PEs are exists.
		/// @pre $none
		/// @post $result >= -1 </returns>
		public virtual int MipsOfOnePe
		{
			get
			{
				if (HostListProperty.Count == 0)
				{
					return -1;
				}
    
				return ContainerVmPeList.getMips(HostListProperty[0].PeListProperty, 0);
			}
		}

		/// <summary>
		/// Gets Millions Instructions Per Second (MIPS) Rating of a Processing Element (Pe). It is
		/// essential to use this method when a resource is made up of heterogenous PEs/machines.
		/// </summary>
		/// <param name="id">   the machine ID </param>
		/// <param name="peId"> the Pe ID </param>
		/// <returns> the MIPS Rating or if no PEs are exists.
		/// @pre id >= 0
		/// @pre peID >= 0
		/// @post $result >= -1 </returns>
		public virtual int getMipsOfOnePe(int id, int peId)
		{
			if (HostListProperty.Count == 0)
			{
				return -1;
			}

			return ContainerVmPeList.getMips(ContainerHostList.getById(HostListProperty, id).PeListProperty, peId);
		}

		/// <summary>
		/// Gets the total MIPS rating, which is the sum of MIPS rating of all machines in a resource.
		/// <p/>
		/// Total MIPS rating for:
		/// <ul>
		/// <li>TimeShared = 1 Rating of a Pe * Total number of PEs
		/// <li>Other policy same rating = same as TimeShared
		/// <li>SpaceShared = Sum of all PEs in all Machines
		/// <li>Other policy different rating = same as SpaceShared
		/// <li>Advance Reservation = 0 or unknown. You need to calculate this manually.
		/// </ul>
		/// </summary>
		/// <returns> the sum of MIPS ratings
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual int Mips
		{
			get
			{
				int mips = 0;
				switch (AllocationPolicy)
				{
					// Assuming all PEs in all Machine have same rating.
					case ContainerDatacenterCharacteristics.TIME_SHARED:
					case ContainerDatacenterCharacteristics.OTHER_POLICY_SAME_RATING:
						mips = MipsOfOnePe * ContainerHostList.getNumberOfPes(HostListProperty);
						break;
    
					// Assuming all PEs in a given Machine have the same rating.
					// But different machines in a Cluster can have different rating
					case ContainerDatacenterCharacteristics.SPACE_SHARED:
					case ContainerDatacenterCharacteristics.OTHER_POLICY_DIFFERENT_RATING:
						foreach (ContainerHost host in HostListProperty)
						{
							mips += host.TotalMips;
						}
						break;
    
					default:
						break;
				}
    
				return mips;
			}
		}

		/// <summary>
		/// Gets the CPU time given the specified parameters (only for TIME_SHARED). <tt>NOTE:</tt> The
		/// CPU time for SPACE_SHARED and ADVANCE_RESERVATION are not yet implemented.
		/// </summary>
		/// <param name="cloudletLength"> the length of a Cloudlet </param>
		/// <param name="load">           the load of a Cloudlet </param>
		/// <returns> the CPU time
		/// @pre cloudletLength >= 0.0
		/// @pre load >= 0.0
		/// @post $result >= 0.0 </returns>
		public virtual double getCpuTime(double cloudletLength, double load)
		{
			double cpuTime = 0.0;

			switch (AllocationPolicy)
			{
				case ContainerDatacenterCharacteristics.TIME_SHARED:
					cpuTime = cloudletLength / (MipsOfOnePe * (1.0 - load));
					break;

				default:
					break;
			}

			return cpuTime;
		}

		/// <summary>
		/// Gets the total number of PEs for all Machines.
		/// </summary>
		/// <returns> number of PEs
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual int NumberOfPes
		{
			get
			{
				return ContainerHostList.getNumberOfPes(HostListProperty);
			}
		}

		/// <summary>
		/// Gets the total number of <tt>FREE</tt> or non-busy PEs for all Machines.
		/// </summary>
		/// <returns> number of PEs
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual int NumberOfFreePes
		{
			get
			{
				return ContainerHostList.getNumberOfFreePes(HostListProperty);
			}
		}

		/// <summary>
		/// Gets the total number of <tt>BUSY</tt> PEs for all Machines.
		/// </summary>
		/// <returns> number of PEs
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual int NumberOfBusyPes
		{
			get
			{
				return ContainerHostList.getNumberOfBusyPes(HostListProperty);
			}
		}

		/// <summary>
		/// Sets the particular Pe status on a Machine.
		/// </summary>
		/// <param name="status"> Pe status, either <tt>Pe.FREE</tt> or <tt>Pe.BUSY</tt> </param>
		/// <param name="hostId"> Machine ID </param>
		/// <param name="peId">   Pe id </param>
		/// <returns> otherwise (Machine id or Pe id might not be exist)
		/// @pre machineID >= 0
		/// @pre peID >= 0
		/// @post $none </returns>
		public virtual bool setPeStatus(int status, int hostId, int peId)
		{
			return ContainerHostList.setPeStatus(HostListProperty, status, hostId, peId);
		}

		/// <summary>
		/// Gets the cost per Millions Instruction (MI) associated with a resource.
		/// </summary>
		/// <returns> the cost using a resource
		/// @pre $none
		/// @post $result >= 0.0 </returns>
		public virtual double CostPerMi
		{
			get
			{
				return CostPerSecond / MipsOfOnePe;
			}
		}

		/// <summary>
		/// Gets the total number of machines.
		/// </summary>
		/// <returns> total number of machines this resource has. </returns>
		public virtual int NumberOfHosts
		{
			get
			{
				return HostListProperty.Count;
			}
		}

		/// <summary>
		/// Gets the current number of failed machines.
		/// </summary>
		/// <returns> current number of failed machines this resource has. </returns>
		public virtual int NumberOfFailedHosts
		{
			get
			{
				int numberOfFailedHosts = 0;
				foreach (ContainerHost host in HostListProperty)
				{
					if (host.Failed)
					{
						numberOfFailedHosts++;
					}
				}
				return numberOfFailedHosts;
			}
		}

		/// <summary>
		/// Checks whether all machines of this resource are working properly or not.
		/// </summary>
		/// <returns> if all machines are working, otherwise </returns>
		public virtual bool Working
		{
			get
			{
				bool result = false;
				if (NumberOfFailedHosts == 0)
				{
					result = true;
				}
    
				return result;
			}
		}

		/// <summary>
		/// Get the cost to use memory in this resource.
		/// </summary>
		/// <returns> the cost to use memory </returns>
		public virtual double CostPerMem
		{
			get
			{
				return costPerMem;
			}
			set
			{
				this.costPerMem = value;
			}
		}


		/// <summary>
		/// Get the cost to use storage in this resource.
		/// </summary>
		/// <returns> the cost to use storage </returns>
		public virtual double CostPerStorage
		{
			get
			{
				return costPerStorage;
			}
			set
			{
				this.costPerStorage = value;
			}
		}


		/// <summary>
		/// Get the cost to use bandwidth in this resource.
		/// </summary>
		/// <returns> the cost to use bw </returns>
		public virtual double CostPerBw
		{
			get
			{
				return costPerBw;
			}
			set
			{
				this.costPerBw = value;
			}
		}


		/// <summary>
		/// Gets the VMM in use in the datacenter.
		/// </summary>
		/// <returns> the VMM name </returns>
		public virtual string Vmm
		{
			get
			{
				return vmm;
			}
			set
			{
				this.vmm = value;
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
		/// Gets the architecture.
		/// </summary>
		/// <returns> the architecture </returns>
		protected internal virtual string Architecture
		{
			get
			{
				return architecture;
			}
			set
			{
				this.architecture = value;
			}
		}


		/// <summary>
		/// Gets the os.
		/// </summary>
		/// <returns> the os </returns>
		protected internal virtual string Os
		{
			get
			{
				return os;
			}
			set
			{
				this.os = value;
			}
		}


        /// <summary>
        /// Gets the host list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the host list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends ContainerHost> java.util.List<T> HostListProperty()
        //public virtual IList<T> HostListProperty<T>() where T : ContainerHost
        public virtual IList<ContainerHost> HostListProperty
        {
			get
			{
				return (IList<ContainerHost>) hostList;
			}
			set
			{
				this.hostList = value;
			}
		}


		/// <summary>
		/// Gets the time zone.
		/// </summary>
		/// <returns> the time zone </returns>
		protected internal virtual double TimeZone
		{
			get
			{
				return timeZone;
			}
			set
			{
				this.timeZone = value;
			}
		}


		/// <summary>
		/// Gets the cost per second.
		/// </summary>
		/// <returns> the cost per second </returns>
		public virtual double CostPerSecond
		{
			get
			{
				return costPerSecond;
			}
			set
			{
				this.costPerSecond = value;
			}
		}


		/// <summary>
		/// Gets the allocation policy.
		/// </summary>
		/// <returns> the allocation policy </returns>
		protected internal virtual int AllocationPolicy
		{
			get
			{
				return allocationPolicy;
			}
			set
			{
				this.allocationPolicy = value;
			}
		}


	}



}