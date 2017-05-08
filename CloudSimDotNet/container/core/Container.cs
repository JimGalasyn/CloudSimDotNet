using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

	using ContainerCloudletScheduler = org.cloudbus.cloudsim.container.schedulers.ContainerCloudletScheduler;
	using org.cloudbus.cloudsim;
	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;


	/// <summary>
	/// Created by sareh on 9/07/15.
	/// </summary>
	public class Container
	{

		/// <summary>
		/// The id.
		/// </summary>
		private int id;

		/// <summary>
		/// The user id.
		/// </summary>
		private int userId;

		/// <summary>
		/// The uid.
		/// </summary>
		private string uid;

		/// <summary>
		/// The size.
		/// </summary>
		private long size;

		/// <summary>
		/// The MIPS.
		/// </summary>
		private double mips;

		/// <summary>
		/// The workloadMips.
		/// </summary>
		private double workloadMips;

		/// <summary>
		/// The number of PEs.
		/// </summary>
		private int numberOfPes;

		/// <summary>
		/// The ram.
		/// </summary>
		private float ram;

		/// <summary>
		/// The bw.
		/// </summary>
		private long bw;

		/// <summary>
		/// The containerManager.
		/// </summary>
		private string containerManager;

		/// <summary>
		/// The ContainerCloudlet scheduler.
		/// </summary>
		private ContainerCloudletScheduler containerCloudletScheduler;

		/// <summary>
		/// The ContainerVm.
		/// </summary>
		private ContainerVm vm;

		/// <summary>
		/// In migration flag.
		/// </summary>
		private bool inMigration;

		/// <summary>
		/// The current allocated size.
		/// </summary>
		private long currentAllocatedSize;

		/// <summary>
		/// The current allocated ram.
		/// </summary>
		private float currentAllocatedRam;

		/// <summary>
		/// The current allocated bw.
		/// </summary>
		private long currentAllocatedBw;

		/// <summary>
		/// The current allocated mips.
		/// </summary>
		private IList<double?> currentAllocatedMips;

		/// <summary>
		/// The VM is being instantiated.
		/// </summary>
		private bool beingInstantiated;

		/// <summary>
		/// The mips allocation history.
		/// </summary>
		private readonly IList<VmStateHistoryEntry> stateHistory = new List<VmStateHistoryEntry>();

	//    added from the power Vm
		/// <summary>
		/// The Constant HISTORY_LENGTH.
		/// </summary>
		public const int HISTORY_LENGTH = 30;

		/// <summary>
		/// The utilization history.
		/// </summary>
		private readonly IList<double?> utilizationHistory = new List<double?>();

		/// <summary>
		/// The previous time.
		/// </summary>
		private double previousTime;

		/// <summary>
		/// The scheduling interval.
		/// </summary>
		private double schedulingInterval;


		/// <summary>
		/// Creates a new Container object. </summary>
		/// <param name="id"> </param>
		/// <param name="userId"> </param>
		/// <param name="mips"> </param>
		/// <param name="numberOfPes"> </param>
		/// <param name="ram"> </param>
		/// <param name="bw"> </param>
		/// <param name="size"> </param>
		/// <param name="containerManager"> </param>
		/// <param name="containerCloudletScheduler"> </param>
		/// <param name="schedulingInterval"> </param>
		public Container(int id, int userId, double mips, int numberOfPes, int ram, long bw, long size, string containerManager, ContainerCloudletScheduler containerCloudletScheduler, double schedulingInterval)
		{
			WorkloadMips = mips;
			Id = id;
			UserId = userId;
			Uid = getUid(userId, id);
			Mips = mips;
			NumberOfPes = numberOfPes;
			setRam(ram);
			Bw = bw;
			Size = size;
			ContainerManager = containerManager;
			ContainerCloudletScheduler = containerCloudletScheduler;
			InMigration = false;
			BeingInstantiated = true;
			CurrentAllocatedBw = 0;
			CurrentAllocatedMips = null;
			CurrentAllocatedRam = 0;
			CurrentAllocatedSize = 0;
			SchedulingInterval = schedulingInterval;
		}

		/// <summary>
		/// Updates the processing of cloudlets running on this Container.
		/// </summary>
		/// <param name="currentTime"> current simulation time </param>
		/// <param name="mipsShare">   array with MIPS share of each Pe available to the scheduler </param>
		/// <returns> time predicted completion time of the earliest finishing cloudlet, or 0 if there is no
		/// next events
		/// @pre currentTime >= 0
		/// @post $none </returns>
		public virtual double updateContainerProcessing(double currentTime, IList<double?> mipsShare)
		{
			if (mipsShare != null)
			{
				return ContainerCloudletScheduler.updateContainerProcessing(currentTime, mipsShare);
			}
			return 0.0;
		}


		/// <summary>
		/// Gets the current requested total mips.
		/// </summary>
		/// <returns> the current requested total mips </returns>
		public virtual double CurrentRequestedTotalMips
		{
			get
			{
				double totalRequestedMips = 0;
				foreach (double mips in CurrentRequestedMips)
				{
					totalRequestedMips += mips;
				}
				return totalRequestedMips;
			}
		}

		/// <summary>
		/// Gets the current requested max mips among all virtual PEs.
		/// </summary>
		/// <returns> the current requested max mips </returns>
		public virtual double CurrentRequestedMaxMips
		{
			get
			{
				double maxMips = 0;
				foreach (double mips in CurrentRequestedMips)
				{
					if (mips > maxMips)
					{
						maxMips = mips;
					}
				}
				return maxMips;
			}
		}

		/// <summary>
		/// Gets the current requested bw.
		/// </summary>
		/// <returns> the current requested bw </returns>
		public virtual long CurrentRequestedBw
		{
			get
			{
				if (BeingInstantiated)
				{
					return Bw;
				}
				return (long)(ContainerCloudletScheduler.CurrentRequestedUtilizationOfBw * Bw);
			}
		}

		/// <summary>
		/// Gets the current requested ram.
		/// </summary>
		/// <returns> the current requested ram </returns>
		public virtual float CurrentRequestedRam
		{
			get
			{
				if (BeingInstantiated)
				{
					return getRam();
				}
				return (float)(ContainerCloudletScheduler.CurrentRequestedUtilizationOfRam * getRam());
			}
		}

		/// <summary>
		/// Get utilization created by all cloudlets running on this container.
		/// </summary>
		/// <param name="time"> the time </param>
		/// <returns> total utilization </returns>
		public virtual double getTotalUtilizationOfCpu(double time)
		{
			//Log.printLine("Container: get Current getTotalUtilizationOfCpu"+ getContainerCloudletScheduler().getTotalUtilizationOfCpu(time));
			return ContainerCloudletScheduler.getTotalUtilizationOfCpu(time);
		}

		/// <summary>
		/// Get utilization created by all cloudlets running on this container in MIPS.
		/// </summary>
		/// <param name="time"> the time </param>
		/// <returns> total utilization </returns>
		public virtual double getTotalUtilizationOfCpuMips(double time)
		{
			//Log.printLine("Container: get Current getTotalUtilizationOfCpuMips"+getTotalUtilizationOfCpu(time) * getMips());
			return getTotalUtilizationOfCpu(time) * Mips;
		}

		/// <summary>
		/// Sets the uid.
		/// </summary>
		/// <param name="uid"> the new uid </param>
		public virtual string Uid
		{
			set
			{
				this.uid = value;
			}
			get
			{
				return uid;
			}
		}


		/// <summary>
		/// Generate unique string identificator of the container.
		/// </summary>
		/// <param name="userId"> the user id </param>
		/// <param name="containerId">   the container id </param>
		/// <returns> string uid </returns>
		public static string getUid(int userId, int containerId)
		{
			return userId + "-" + containerId;
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
		/// Sets the user id.
		/// </summary>
		/// <param name="userId"> the new user id </param>
		protected internal virtual int UserId
		{
			set
			{
				this.userId = value;
			}
			get
			{
				return userId;
			}
		}


		/// <summary>
		/// Gets the mips.
		/// </summary>
		/// <returns> the mips </returns>
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
		/// Sets the mips.
		/// </summary>
		/// <param name="mips"> the new mips </param>
		public virtual void changeMips(double mips)
		{
			this.mips = mips;
		}

		/// <summary>
		/// Gets the number of pes.
		/// </summary>
		/// <returns> the number of pes </returns>
		public virtual int NumberOfPes
		{
			get
			{
				return numberOfPes;
			}
			set
			{
				this.numberOfPes = value;
			}
		}


		/// <summary>
		/// Gets the amount of ram.
		/// </summary>
		/// <returns> amount of ram
		/// @pre $none
		/// @post $none </returns>
		public virtual float getRam()
		{
			return ram;
		}
        
        /// <summary>
        /// Gets the amount of ram in the current <see cref="Container"/>.
        /// </summary>
        public virtual float Ram
        {
            get
            {
                return ram;
            }
        }

		/// <summary>
		/// Sets the amount of ram.
		/// </summary>
		/// <param name="ram"> new amount of ram
		/// @pre ram > 0
		/// @post $none </param>
		public virtual void setRam(int ram)
		{
			this.ram = ram;
		}

		/// <summary>
		/// Gets the amount of bandwidth.
		/// </summary>
		/// <returns> amount of bandwidth
		/// @pre $none
		/// @post $none </returns>
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
		/// Gets the amount of storage.
		/// </summary>
		/// <returns> amount of storage
		/// @pre $none
		/// @post $none </returns>
		public virtual long Size
		{
			get
			{
				return size;
			}
			set
			{
				this.size = value;
			}
		}


		/// <summary>
		/// Gets the VMM.
		/// </summary>
		/// <returns> VMM
		/// @pre $none
		/// @post $none </returns>
		public virtual string ContainerManager
		{
			get
			{
				return containerManager;
			}
			set
			{
				this.containerManager = value;
			}
		}


		/// <summary>
		/// Gets the vm.
		/// </summary>
		/// <returns> the vm </returns>
		public virtual ContainerVm Vm
		{
			get
			{
				return vm;
			}
			set
			{
				this.vm = value;
			}
		}

		/// <summary>
		/// Gets the containerCloudletScheduler.
		/// </summary>
		/// <returns> the containerCloudletScheduler </returns>
		public virtual ContainerCloudletScheduler ContainerCloudletScheduler
		{
			get
			{
				return containerCloudletScheduler;
			}
			set
			{
				this.containerCloudletScheduler = value;
			}
		}



		/// <summary>
		/// Checks if is in migration.
		/// </summary>
		/// <returns> true, if is in migration </returns>
		public virtual bool InMigration
		{
			get
			{
				return inMigration;
			}
			set
			{
				this.inMigration = value;
			}
		}


		/// <summary>
		/// Gets the current allocated size.
		/// </summary>
		/// <returns> the current allocated size </returns>
		public virtual long CurrentAllocatedSize
		{
			get
			{
				return currentAllocatedSize;
			}
			set
			{
				this.currentAllocatedSize = value;
			}
		}


		/// <summary>
		/// Gets the current allocated ram.
		/// </summary>
		/// <returns> the current allocated ram </returns>
		public virtual float CurrentAllocatedRam
		{
			get
			{
				return currentAllocatedRam;
			}
			set
			{
				this.currentAllocatedRam = value;
			}
		}


		/// <summary>
		/// Gets the current allocated bw.
		/// </summary>
		/// <returns> the current allocated bw </returns>
		public virtual long CurrentAllocatedBw
		{
			get
			{
				return currentAllocatedBw;
			}
			set
			{
				this.currentAllocatedBw = value;
			}
		}


		/// <summary>
		/// Gets the current allocated mips.
		/// </summary>
		/// <returns> the current allocated mips </returns>
		public virtual IList<double?> CurrentAllocatedMips
		{
			get
			{
				return currentAllocatedMips;
			}
			set
			{
				this.currentAllocatedMips = value;
			}
		}


		/// <summary>
		/// Checks if is being instantiated.
		/// </summary>
		/// <returns> true, if is being instantiated </returns>
		public virtual bool BeingInstantiated
		{
			get
			{
				return beingInstantiated;
			}
			set
			{
				this.beingInstantiated = value;
			}
		}


		/// <summary>
		/// Gets the state history.
		/// </summary>
		/// <returns> the state history </returns>
		public virtual IList<VmStateHistoryEntry> StateHistory
		{
			get
			{
				return stateHistory;
			}
		}

		/// <summary>
		/// Adds the state history entry.
		/// </summary>
		/// <param name="time">          the time </param>
		/// <param name="allocatedMips"> the allocated mips </param>
		/// <param name="requestedMips"> the requested mips </param>
		/// <param name="isInMigration"> the is in migration </param>
		public virtual void addStateHistoryEntry(double time, double allocatedMips, double requestedMips, bool isInMigration)
		{
			VmStateHistoryEntry newState = new VmStateHistoryEntry(time, allocatedMips, requestedMips, isInMigration);
			if (StateHistory.Count > 0)
			{
				VmStateHistoryEntry previousState = StateHistory[StateHistory.Count - 1];
				if (previousState.Time == time)
				{
					StateHistory[StateHistory.Count - 1] = newState;
					return;
				}
			}
			StateHistory.Add(newState);
		}


		/// <summary>
		/// Gets the utilization MAD in MIPS.
		/// </summary>
		/// <returns> the utilization mean in MIPS </returns>
		public virtual double UtilizationMad
		{
			get
			{
				double mad = 0;
				if (UtilizationHistory.Count > 0)
				{
					int n = HISTORY_LENGTH;
					if (HISTORY_LENGTH > UtilizationHistory.Count)
					{
						n = UtilizationHistory.Count;
					}
					double median = MathUtil.median(UtilizationHistory);
					double[] deviationSum = new double[n];
					for (int i = 0; i < n; i++)
					{
						deviationSum[i] = Math.Abs(median - UtilizationHistory[i].Value);
					}
					mad = MathUtil.median(deviationSum);
				}
				return mad;
			}
		}

		/// <summary>
		/// Gets the utilization mean in percents.
		/// </summary>
		/// <returns> the utilization mean in MIPS </returns>
		public virtual double UtilizationMean
		{
			get
			{
				double mean = 0;
				if (UtilizationHistory.Count > 0)
				{
					int n = HISTORY_LENGTH;
					if (HISTORY_LENGTH > UtilizationHistory.Count)
					{
						n = UtilizationHistory.Count;
					}
					for (int i = 0; i < n; i++)
					{
						mean += UtilizationHistory[i].Value;
					}
					mean /= n;
				}
				return mean * Mips;
			}
		}

		/// <summary>
		/// Gets the utilization variance in MIPS.
		/// </summary>
		/// <returns> the utilization variance in MIPS </returns>
		public virtual double UtilizationVariance
		{
			get
			{
				double mean = UtilizationMean;
				double variance = 0;
				if (UtilizationHistory.Count > 0)
				{
					int n = HISTORY_LENGTH;
					if (HISTORY_LENGTH > UtilizationHistory.Count)
					{
						n = UtilizationHistory.Count;
					}
					for (int i = 0; i < n; i++)
					{
						double tmp = UtilizationHistory[i].Value * Mips - mean;
						variance += tmp * tmp;
					}
					variance /= n;
				}
				return variance;
			}
		}

		/// <summary>
		/// Adds the utilization history value.
		/// </summary>
		/// <param name="utilization"> the utilization </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void addUtilizationHistoryValue(final double utilization)
		public virtual void addUtilizationHistoryValue(double utilization)
		{
			UtilizationHistory.Insert(0, utilization);
			if (UtilizationHistory.Count > HISTORY_LENGTH)
			{
				UtilizationHistory.RemoveAt(HISTORY_LENGTH);
			}
		}


		/// <summary>
		/// Gets the utilization history.
		/// </summary>
		/// <returns> the utilization history </returns>
		protected internal virtual IList<double?> UtilizationHistory
		{
			get
			{
				return utilizationHistory;
			}
		}

		/// <summary>
		/// Gets the previous time.
		/// </summary>
		/// <returns> the previous time </returns>
		public virtual double PreviousTime
		{
			get
			{
				return previousTime;
			}
			set
			{
				this.previousTime = value;
			}
		}


		/// <summary>
		/// Gets the scheduling interval.
		/// </summary>
		/// <returns> the schedulingInterval </returns>
		public virtual double SchedulingInterval
		{
			get
			{
				return schedulingInterval;
			}
			set
			{
				this.schedulingInterval = value;
			}
		}




	//

		public virtual IList<double?> CurrentRequestedMips
		{
			get
			{
				if (BeingInstantiated)
				{
					IList<double?> currentRequestedMips = new List<double?>();
    
					for (int i = 0; i < NumberOfPes; i++)
					{
						currentRequestedMips.Add(Mips);
    
					}
    
					return currentRequestedMips;
				}
    
    
				return ContainerCloudletScheduler.CurrentRequestedMips;
			}
		}

		public virtual double WorkloadMips
		{
			get
			{
				return workloadMips;
			}
			set
			{
				this.workloadMips = value;
			}
		}


		/// <summary>
		/// Gets the current requested total mips.
		/// </summary>
		/// <returns> the current requested total mips </returns>
		public virtual double WorkloadTotalMips
		{
			get
			{
    
				//Log.printLine("Container: get Current totalRequestedMips"+ totalRequestedMips);
				return WorkloadMips * NumberOfPes;
			}
		}


	}

}