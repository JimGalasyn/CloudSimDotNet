using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.schedulers
{

	/// <summary>
	/// Created by sareh on 14/07/15.
	/// </summary>
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;


	public class ContainerCloudletSchedulerDynamicWorkload : ContainerCloudletSchedulerTimeShared
	{

			/// <summary>
			/// The mips. </summary>
			private double mips;

			/// <summary>
			/// The number of PEs. </summary>
			private int numberOfPes;

			/// <summary>
			/// The total mips. </summary>
			private double totalMips;

			/// <summary>
			/// The under allocated mips. </summary>
			private IDictionary<string, double?> underAllocatedMips;

			/// <summary>
			/// The cache previous time. </summary>
			private double cachePreviousTime;

			/// <summary>
			/// The cache current requested mips. </summary>
			private IList<double?> cacheCurrentRequestedMips;

			/// <summary>
			/// Instantiates a new vM scheduler time shared.
			/// </summary>
			/// <param name="mips"> the mips </param>
			/// <param name="numberOfPes"> the pes number </param>
			public ContainerCloudletSchedulerDynamicWorkload(double mips, int numberOfPes) : base()
			{
				Mips = mips;
				NumberOfPes = numberOfPes;
				TotalMips = NumberOfPes * Mips;
				UnderAllocatedMips = new Dictionary<string, double?>();
				CachePreviousTime = -1;
			}

			/// <summary>
			/// Updates the processing of cloudlets running under management of this scheduler.
			/// </summary>
			/// <param name="currentTime"> current simulation time </param>
			/// <param name="mipsShare"> array with MIPS share of each Pe available to the scheduler </param>
			/// <returns> time predicted completion time of the earliest finishing cloudlet, or 0 if there is
			///         no next events
			/// @pre currentTime >= 0
			/// @post $none </returns>
			public override double updateContainerProcessing(double currentTime, IList<double?> mipsShare)
			{
				CurrentMipsShare = mipsShare;

				double timeSpan = currentTime - PreviousTime;
				double nextEvent = double.MaxValue;
				IList<ResCloudlet> cloudletsToFinish = new List<ResCloudlet>();

				foreach (ResCloudlet rcl in CloudletExecList)
				{
					rcl.updateCloudletFinishedSoFar((long)(timeSpan * getTotalCurrentAllocatedMipsForCloudlet(rcl, PreviousTime) * Consts.MILLION));

					if (rcl.RemainingCloudletLength == 0)
					{ // finished: remove from the list
						cloudletsToFinish.Add(rcl);
					}
					else
					{ // not finish: estimate the finish time
						double estimatedFinishTime = getEstimatedFinishTime(rcl, currentTime);
						if (estimatedFinishTime - currentTime < CloudSim.MinTimeBetweenEvents)
						{
							estimatedFinishTime = currentTime + CloudSim.MinTimeBetweenEvents;
						}
						if (estimatedFinishTime < nextEvent)
						{
							nextEvent = estimatedFinishTime;
						}
					}
				}

				foreach (ResCloudlet rgl in cloudletsToFinish)
				{
					CloudletExecList.Remove(rgl);
					cloudletFinish(rgl);
				}

				PreviousTime = currentTime;

				if (CloudletExecList.Count == 0)
				{
					return 0;
				}

				cloudletsToFinish.Clear();

				return nextEvent;
			}

			/// <summary>
			/// Receives an cloudlet to be executed in the VM managed by this scheduler.
			/// </summary>
			/// <param name="cl"> the cl </param>
			/// <returns> predicted completion time
			/// @pre _gl != null
			/// @post $none </returns>
			public override double cloudletSubmit(Cloudlet cl)
			{
				return cloudletSubmit(cl, 0);
			}

			/// <summary>
			/// Receives an cloudlet to be executed in the VM managed by this scheduler.
			/// </summary>
			/// <param name="cl"> the cl </param>
			/// <param name="fileTransferTime"> the file transfer time </param>
			/// <returns> predicted completion time
			/// @pre _gl != null
			/// @post $none </returns>
			public override double cloudletSubmit(Cloudlet cl, double fileTransferTime)
			{
				ResCloudlet rcl = new ResCloudlet(cl);
				rcl.CloudletStatus = Cloudlet.INEXEC;

				for (int i = 0; i < cl.NumberOfPes; i++)
				{
					rcl.setMachineAndPeId(0, i);
				}

				CloudletExecList.Add(rcl);
				return getEstimatedFinishTime(rcl, PreviousTime);
			}

			/// <summary>
			/// Processes a finished cloudlet.
			/// </summary>
			/// <param name="rcl"> finished cloudlet
			/// @pre rgl != $null
			/// @post $none </param>
			public override void cloudletFinish(ResCloudlet rcl)
			{
				rcl.CloudletStatus = Cloudlet.SUCCESS;
				rcl.finalizeCloudlet();
				CloudletFinishedList.Add(rcl);
			}

			/// <summary>
			/// Get utilization created by all cloudlets.
			/// </summary>
			/// <param name="time"> the time </param>
			/// <returns> total utilization </returns>
			public override double getTotalUtilizationOfCpu(double time)
			{
				double totalUtilization = 0;
				foreach (ResCloudlet rcl in CloudletExecList)
				{
					totalUtilization += rcl.Cloudlet.getUtilizationOfCpu(time);
				}
				return totalUtilization;
			}

			/// <summary>
			/// Gets the current mips.
			/// </summary>
			/// <returns> the current mips </returns>
			public override IList<double?> CurrentRequestedMips
			{
				get
				{
					if (CachePreviousTime == PreviousTime)
					{
						return CacheCurrentRequestedMips;
					}
					IList<double?> currentMips = new List<double?>();
					double totalMips = getTotalUtilizationOfCpu(PreviousTime) * TotalMips;
					double mipsForPe = totalMips / NumberOfPes;
    
					for (int i = 0; i < NumberOfPes; i++)
					{
						currentMips.Add(mipsForPe);
					}
    
					CachePreviousTime = PreviousTime;
					CacheCurrentRequestedMips = currentMips;
    
					return currentMips;
				}
			}

			/// <summary>
			/// Gets the current mips.
			/// </summary>
			/// <param name="rcl"> the rcl </param>
			/// <param name="time"> the time </param>
			/// <returns> the current mips </returns>
			public override double getTotalCurrentRequestedMipsForCloudlet(ResCloudlet rcl, double time)
			{
				return rcl.Cloudlet.getUtilizationOfCpu(time) * TotalMips;
			}

			/// <summary>
			/// Gets the total current mips for the clouddlet.
			/// </summary>
			/// <param name="rcl"> the rcl </param>
			/// <param name="mipsShare"> the mips share </param>
			/// <returns> the total current mips </returns>
			public override double getTotalCurrentAvailableMipsForCloudlet(ResCloudlet rcl, IList<double?> mipsShare)
			{
				double totalCurrentMips = 0.0;
				if (mipsShare != null)
				{
					int neededPEs = rcl.NumberOfPes;
					foreach (double mips in mipsShare)
					{
						totalCurrentMips += mips;
						neededPEs--;
						if (neededPEs <= 0)
						{
							break;
						}
					}
				}
				return totalCurrentMips;
			}

			/// <summary>
			/// Gets the current mips.
			/// </summary>
			/// <param name="rcl"> the rcl </param>
			/// <param name="time"> the time </param>
			/// <returns> the current mips </returns>
			public override double getTotalCurrentAllocatedMipsForCloudlet(ResCloudlet rcl, double time)
			{
				double totalCurrentRequestedMips = getTotalCurrentRequestedMipsForCloudlet(rcl, time);
				double totalCurrentAvailableMips = getTotalCurrentAvailableMipsForCloudlet(rcl, CurrentMipsShare);
				if (totalCurrentRequestedMips > totalCurrentAvailableMips)
				{
					return totalCurrentAvailableMips;
				}
				return totalCurrentRequestedMips;
			}

			/// <summary>
			/// Update under allocated mips for cloudlet.
			/// </summary>
			/// <param name="rcl"> the rgl </param>
			/// <param name="mips"> the mips </param>
			public virtual void updateUnderAllocatedMipsForCloudlet(ResCloudlet rcl, double mips)
			{
				if (UnderAllocatedMips.ContainsKey(rcl.Uid))
				{
					mips += UnderAllocatedMips[rcl.Uid].Value;
				}
				UnderAllocatedMips[rcl.Uid] = mips;
			}

			/// <summary>
			/// Get estimated cloudlet completion time.
			/// </summary>
			/// <param name="rcl"> the rcl </param>
			/// <param name="time"> the time </param>
			/// <returns> the estimated finish time </returns>
			public virtual double getEstimatedFinishTime(ResCloudlet rcl, double time)
			{
				return time + ((rcl.RemainingCloudletLength) / getTotalCurrentAllocatedMipsForCloudlet(rcl, time));
			}

			/// <summary>
			/// Gets the total current mips.
			/// </summary>
			/// <returns> the total current mips </returns>
			public virtual int TotalCurrentMips
			{
				get
				{
					int totalCurrentMips = 0;
					foreach (double mips in CurrentMipsShare)
					{
						totalCurrentMips += (int)mips;
					}
					return totalCurrentMips;
				}
			}

			/// <summary>
			/// Sets the total mips.
			/// </summary>
			/// <param name="mips"> the new total mips </param>
			public virtual double TotalMips
			{
				set
				{
					totalMips = value;
				}
				get
				{
					return totalMips;
				}
			}


			/// <summary>
			/// Sets the pes number.
			/// </summary>
			/// <param name="pesNumber"> the new pes number </param>
			public virtual int NumberOfPes
			{
				set
				{
					numberOfPes = value;
				}
				get
				{
					return numberOfPes;
				}
			}


			/// <summary>
			/// Sets the mips.
			/// </summary>
			/// <param name="mips"> the new mips </param>
			public virtual double Mips
			{
				set
				{
					this.mips = value;
				}
				get
				{
					return mips;
				}
			}


			/// <summary>
			/// Sets the under allocated mips.
			/// </summary>
			/// <param name="underAllocatedMips"> the under allocated mips </param>
			public virtual IDictionary<string, double?> UnderAllocatedMips
			{
				set
				{
					this.underAllocatedMips = value;
				}
				get
				{
					return underAllocatedMips;
				}
			}


			/// <summary>
			/// Gets the cache previous time.
			/// </summary>
			/// <returns> the cache previous time </returns>
			protected internal virtual double CachePreviousTime
			{
				get
				{
					return cachePreviousTime;
				}
				set
				{
					this.cachePreviousTime = value;
				}
			}


			/// <summary>
			/// Gets the cache current requested mips.
			/// </summary>
			/// <returns> the cache current requested mips </returns>
			protected internal virtual IList<double?> CacheCurrentRequestedMips
			{
				get
				{
					return cacheCurrentRequestedMips;
				}
				set
				{
					this.cacheCurrentRequestedMips = value;
				}
			}


	}



}