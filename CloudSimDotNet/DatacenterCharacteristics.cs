using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{

	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
	using HostList = org.cloudbus.cloudsim.lists.HostList;
	using PeList = org.cloudbus.cloudsim.lists.PeList;

	/// <summary>
	/// Represents static properties of a resource such as 
	/// architecture, Operating System (OS), management policy (time- or space-shared), 
	/// cost and time zone at which the resource is located along resource configuration.
	/// 
	/// @author Manzur Murshed
	/// @author Rajkumar Buyya
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// @todo the characteristics are used only for datacenter (as the class name indicates),
	/// however, the class documentation uses the generic term "resource" instead of "datacenter",
	/// giving the idea that the class can be used to describe characteristics of other resources.
	/// However, the class was found being used only for datacenters.
	/// </summary>
	public class DatacenterCharacteristics
	{

		/// <summary>
		/// The datacenter id -- setup when datacenter is created. </summary>
		private int id;

		/// <summary>
		/// The architecture of the resource. </summary>
		private string architecture;

		/// <summary>
		/// The Operating System (OS) of the resource. </summary>
		private string os;

        /// <summary>
        /// The hosts owned by the datacenter. </summary>
        //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        //ORIGINAL LINE: private java.util.List<? extends Host> hostList;
        private IList<Host> hostList;
        //private HostList hostList;

        /// <summary>
        /// The time zone, defined as the difference from GMT. </summary>
        private double timeZone;

		/// <summary>
		/// Price/CPU-unit. If unit = sec., then the price is defined as G$/CPU-sec. </summary>
		private double costPerSecond;

		/// <summary>
		/// The CPU allocation policy for all PMs of the datacenter, according to
		/// constants such as <seealso cref="#TIME_SHARED"/>
		/// and <seealso cref="#SPACE_SHARED"/>.
		/// 
		/// @todo The use of int constants difficult to know the valid values
		/// for the property. It may be used a enum instead.
		/// </summary>
		private int allocationPolicy;

		/// <summary>
		/// Time-shared CPU allocation policy using Round-Robin algorithm. </summary>
		public const int TIME_SHARED = 0;

		/// <summary>
		/// Spaced-shared CPU allocation policy using First Come First Serve (FCFS) algorithm. </summary>
		public const int SPACE_SHARED = 1;

		/// <summary>
		/// Assuming all PEs in all PMs have the same rating. </summary>
		public const int OTHER_POLICY_SAME_RATING = 2;

		/// <summary>
		/// Assuming all PEs in a PM have the same rating. However, each PM has different
		/// rating to each other.
		/// </summary>
		public const int OTHER_POLICY_DIFFERENT_RATING = 3;

		/// <summary>
		/// A resource that supports Advanced Reservation mechanisms. </summary>
		public const int ADVANCE_RESERVATION = 4;

		/// <summary>
		/// The Virtual Machine Monitor (VMM), also called hypervisor, used
		/// in the datacenter.. 
		/// </summary>
		private string vmm;

		/// <summary>
		/// The cost per each unity of RAM memory. </summary>
		private double costPerMem;

		/// <summary>
		/// The cost per each unit of storage. </summary>
		private double costPerStorage;

		/// <summary>
		/// The cost of each byte of bandwidth (bw) consumed. </summary>
		private double costPerBw;

        /// <summary>
        /// Creates a new DatacenterCharacteristics object. If the time zone is invalid, then by
        /// default, it will be GMT+0.
        /// </summary>
        /// <param name="architecture"> the architecture of the datacenter </param>
        /// <param name="os"> the operating system used on the datacenter's PMs </param>
        /// <param name="vmm"> the virtual machine monitor used </param>
        /// <param name="hostList"> list of machines in the datacenter </param>
        /// <param name="timeZone"> local time zone of a user that owns this reservation. Time zone should be of
        ///            range [GMT-12 ... GMT+13] </param>
        /// <param name="costPerSec"> the cost per sec of CPU use in the datacenter </param>
        /// <param name="costPerMem"> the cost to use memory in the datacenter </param>
        /// <param name="costPerStorage"> the cost to use storage in the datacenter </param>
        /// <param name="costPerBw"> the cost of each byte of bandwidth (bw) consumed
        /// 
        /// @pre architecture != null
        /// @pre OS != null
        /// @pre VMM != null
        /// @pre machineList != null
        /// @pre timeZone >= -12 && timeZone <= 13
        /// @pre costPerSec >= 0.0
        /// @pre costPerMem >= 0
        /// @pre costPerStorage >= 0
        /// @post $none </param>
        //public DatacenterCharacteristics<T1>(string architecture, string os, string vmm, IList<T1> hostList, double timeZone, double costPerSec, double costPerMem, double costPerStorage, double costPerBw) where T1 : Host
        public DatacenterCharacteristics(string architecture, string os, string vmm, IList<Host> hostList, double timeZone, double costPerSec, double costPerMem, double costPerStorage, double costPerBw)
		{
			Id = -1;
			Architecture = architecture;
			Os = os;
			HostListProperty = hostList;
					/*@todo allocationPolicy is not a parameter. It is setting
					the attribute to itself, what has not effect. */
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
		/// Gets the first PM with at least one empty Pe.
		/// </summary>
		/// <returns> a Machine object or if not found
		/// @pre $none
		/// @post $none </returns>
		public virtual Host HostWithFreePe
		{
			get
			{
				return HostList.getHostWithFreePe(HostListProperty);
			}
		}

		/// <summary>
		/// Gets a Machine with at least a given number of free Pe.
		/// </summary>
		/// <param name="peNumber"> the pe number </param>
		/// <returns> a Machine object or if not found
		/// @pre $none
		/// @post $none </returns>
		public virtual Host getHostWithFreePe(int peNumber)
		{
			return HostList.getHostWithFreePe(HostListProperty, peNumber);
		}

		/// <summary>
		/// Gets the Million Instructions Per Second (MIPS) Rating of the first Processing Element (Pe)
		/// of the first PM. 
		/// <tt>NOTE:</tt>It is assumed all PEs' rating is same in a given machine.
		/// 
		/// </summary>
		/// <returns> the MIPS Rating or -1 if no PEs exists
		/// 
		/// @pre $none
		/// @post $result >= -1
		/// @todo It considers that all PEs of all PM have the same MIPS capacity,
		/// what is not ensured because it is possible to add PMs of different configurations
		/// to a datacenter. Even for the <seealso cref="Host"/> it is possible
		/// to add Pe's of different capacities through the <seealso cref="Host#peList"/> attribute. </returns>
		public virtual int MipsOfOnePe
		{
			get
			{
				if (HostListProperty.Count == 0)
				{
					return -1;
				}
    
						/*@todo Why is it always get the MIPS of the first host in the datacenter?
						The note in the method states that it is considered that all PEs into
						a PM have the same MIPS capacity, but different PM can have different
						PEs' MIPS.*/
				return PeList.getMips(HostListProperty[0].PeListProperty, 0);
			}
		}

		/// <summary>
		/// Gets Millions Instructions Per Second (MIPS) Rating of a Processing Element (Pe). It is
		/// essential to use this method when a datacenter is made up of heterogenous PEs per PMs.
		/// </summary>
		/// <param name="id"> the machine ID </param>
		/// <param name="peId"> the Pe ID </param>
		/// <returns> the MIPS Rating or -1 if no PEs are exists.
		/// 
		/// @pre id >= 0
		/// @pre peID >= 0
		/// @post $result >= -1
		/// 
		/// @todo The id parameter would be renamed to pmId to be clear. </returns>
		public virtual int getMipsOfOnePe(int id, int peId)
		{
			if (HostListProperty.Count == 0)
			{
				return -1;
			}

			return PeList.getMips(HostList.getById(HostListProperty, id).PeListProperty, peId);
		}

		/// <summary>
		/// Gets the total MIPS rating, which is the sum of MIPS rating of all PMs in a datacenter.
		/// <para>
		/// Total MIPS rating for:
		/// <ul>
		/// <li>TimeShared = 1 Rating of a Pe * Total number of PEs
		/// <li>Other policy same rating = same as TimeShared
		/// <li>SpaceShared = Sum of all PEs in all Machines
		/// <li>Other policy different rating = same as SpaceShared
		/// <li>Advance Reservation = 0 or unknown. You need to calculate this manually.
		/// </ul>
		/// 
		/// </para>
		/// </summary>
		/// <returns> the sum of MIPS ratings
		/// 
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual int Mips
		{
			get
			{
				int mips = 0;
						/*@todo It assumes that the heterogeinety of PE's capacity of PMs
						is dependent of the CPU allocation policy of the Datacenter.
						However, I don't see any relation between PMs heterogeinety and
						allocation policy.
						I can have a time shared policy in a datacenter of
						PMs with the same or different processing capacity.
						The same is true for a space shared or even any other policy. 
						*/
    
						/*@todo the method doesn't use polymorphism to ensure that it will
						automatically behave according to the instance of the allocationPolicy used.
						The use of a switch here breaks the Open/Close Principle (OCP).
						Thus, it doesn't allow the class to be closed for changes
						and opened for extension.
						If a new scheduler is created, the class has to be changed
						to include the new scheduler in switches like that below.
						*/
				switch (AllocationPolicy)
				{
								// Assuming all PEs in all PMs have same rating.
								/*@todo But it is possible to add PMs of different configurations
								    in a hostlist attached to a DatacenterCharacteristic attribute
								    of a Datacenter*/
					case DatacenterCharacteristics.TIME_SHARED:
					case DatacenterCharacteristics.OTHER_POLICY_SAME_RATING:
						mips = MipsOfOnePe * HostList.getNumberOfPes(HostListProperty);
					break;
    
					// Assuming all PEs in a given PM have the same rating.
					// But different PMs in a Cluster can have different rating
					case DatacenterCharacteristics.SPACE_SHARED:
					case DatacenterCharacteristics.OTHER_POLICY_DIFFERENT_RATING:
						foreach (Host host in HostListProperty)
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
		/// Gets the amount of CPU time (in seconds) that the cloudlet will spend
		/// to finish processing, considering the current CPU allocation policy 
		/// (currently only for TIME_SHARED) and cloudlet load. 
		/// @todo <tt>NOTE:</tt> The
		/// CPU time for SPACE_SHARED and ADVANCE_RESERVATION are not yet implemented.
		/// </summary>
		/// <param name="cloudletLength"> the length of a Cloudlet </param>
		/// <param name="load"> the current load of a Cloudlet (percentage of load from 0 to 1) </param>
		/// <returns> the CPU time (in seconds)
		/// 
		/// @pre cloudletLength >= 0.0
		/// @pre load >= 0.0
		/// @post $result >= 0.0 </returns>
		public virtual double getCpuTime(double cloudletLength, double load)
		{
			double cpuTime = 0.0;

			switch (AllocationPolicy)
			{
				case DatacenterCharacteristics.TIME_SHARED:
									/*@todo It is not exacly clear what this method does.
									I guess it computes how many time the cloudlet will
									spend using the CPU to finish its job, considering 
									the CPU allocation policy. By this way,
									the load parameter may be cloudlet's the percentage of load (from 0 to 1).
									Then, (getMipsOfOnePe() * (1.0 - load)) computes the amount
									MIPS that is currently being used by the cloudlet.
									Dividing the total cloudlet length in MI by that result
									returns the number of seconds that the cloudlet will spend
									to execute its total MI.
									
									This method has to be reviewed and documentation
									checked.
	
									If load is equals to 1, this calculation will 
									raise and division by zero exception, what makes invalid
									the pre condition defined in the method documention*/
					cpuTime = cloudletLength / (MipsOfOnePe * (1.0 - load));
					break;

				default:
					break;
			}

			return cpuTime;
		}

		/// <summary>
		/// Gets the total number of PEs for all PMs.
		/// </summary>
		/// <returns> number of PEs
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual int NumberOfPes
		{
			get
			{
				return HostList.getNumberOfPes(HostListProperty);
			}
		}

		/// <summary>
		/// Gets the total number of <tt>FREE</tt> or non-busy PEs for all PMs.
		/// </summary>
		/// <returns> number of PEs
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual int NumberOfFreePes
		{
			get
			{
				return HostList.getNumberOfFreePes(HostListProperty);
			}
		}

		/// <summary>
		/// Gets the total number of <tt>BUSY</tt> PEs for all PMs.
		/// </summary>
		/// <returns> number of PEs
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual int NumberOfBusyPes
		{
			get
			{
				return HostList.getNumberOfBusyPes(HostListProperty);
			}
		}

		/// <summary>
		/// Sets the particular Pe status on a PM.
		/// </summary>
		/// <param name="status"> Pe status, either <tt>Pe.FREE</tt> or <tt>Pe.BUSY</tt> </param>
		/// <param name="hostId"> Machine ID </param>
		/// <param name="peId"> Pe id </param>
		/// <returns> otherwise (Machine id or Pe id might not be exist)
		/// @pre machineID >= 0
		/// @pre peID >= 0
		/// @post $none </returns>
		public virtual bool setPeStatus(int status, int hostId, int peId)
		{
			return HostList.setPeStatus(HostListProperty, status, hostId, peId);
		}

		/// <summary>
		/// Gets the cost per Million Instruction (MI) associated with a Datacenter.
		/// </summary>
		/// <returns> the cost using CPU of PM in the Datacenter
		/// @pre $none
		/// @post $result >= 0.0
		/// @todo Again, it considers that all PEs of all PM have the same MIPS capacity,
		/// what is not ensured because it is possible to add PMs of different configurations
		/// to a datacenter </returns>
		public virtual double CostPerMi
		{
			get
			{
				return CostPerSecond / MipsOfOnePe;
			}
		}

		/// <summary>
		/// Gets the total number of PMs.
		/// </summary>
		/// <returns> total number of machines the Datacenter has. </returns>
		public virtual int NumberOfHosts
		{
			get
			{
				return HostListProperty.Count;
			}
		}

		/// <summary>
		/// Gets the current number of failed PMs.
		/// </summary>
		/// <returns> current number of failed PMs the Datacenter has. </returns>
		public virtual int NumberOfFailedHosts
		{
			get
			{
				int numberOfFailedHosts = 0;
				foreach (Host host in HostListProperty)
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
		/// Checks whether all PMs of the datacenter are working properly or not.
		/// </summary>
		/// <returns> if all PMs are working, otherwise </returns>
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
		/// Get the cost to use memory in the datacenter.
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
		/// Get the cost to use storage in the datacenter.
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
		/// Get the cost to use bandwidth in the datacenter.
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
		/// Gets the datacenter id.
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
		/// Gets the Operating System (OS).
		/// </summary>
		/// <returns> the Operating System (OS) </returns>
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
    /// <returns> the host list
    /// @todo check this warning below </returns>
    //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
    //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends Host> java.util.List<T> HostListProperty()
    //public virtual IList<T> HostListProperty<T>() where T : Host
    public virtual IList<Host> HostListProperty
    {
			get
			{
				return hostList;
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
		/// Gets the cost per second of CPU.
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