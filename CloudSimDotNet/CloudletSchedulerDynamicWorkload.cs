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

	/// <summary>
	/// CloudletSchedulerDynamicWorkload implements a policy of scheduling performed by a virtual machine
	/// to run its <seealso cref="Cloudlet Cloudlets"/>, 
	/// assuming there is just one cloudlet which is working as an online service.
	/// It extends a TimeShared policy, but in fact, considering that there is just
	/// one cloudlet for the VM using this scheduler, the cloudlet will not
	/// compete for CPU with other ones.
	/// Each VM has to have its own instance of a CloudletScheduler.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// @todo The name of the class doesn't represent its goal. A clearer name would be
	/// CloudletSchedulerSingleService as its Test Suite
	/// </summary>
	public class CloudletSchedulerDynamicWorkload : CloudletSchedulerTimeShared
	{

		/// <summary>
		/// The individual MIPS capacity of each PE allocated to the VM using the scheduler,
		/// considering that all PEs have the same capacity. 
		/// @todo Despite of the class considers that all PEs have the same capacity,
		/// it accepts a list of PEs with different MIPS at the method 
		/// <seealso cref="#updateVmProcessing(double, java.util.List) "/>
		/// </summary>
		private double mips;

		/// <summary>
		/// The number of PEs allocated to the VM using the scheduler. </summary>
		private int numberOfPes;

		/// <summary>
		/// The total MIPS considering all PEs. </summary>
		private double totalMips;

		/// <summary>
		/// The under allocated MIPS. </summary>
		private IDictionary<string, double?> underAllocatedMips;

		/// <summary>
		/// The cache of the previous time when the <seealso cref="#getCurrentRequestedMips()"/> was called. </summary>
		private double cachePreviousTime;

		/// <summary>
		/// The cache of the last current requested MIPS. </summary>
		/// <seealso cref=  #getCurrentRequestedMips()  </seealso>
		private IList<double?> cacheCurrentRequestedMips;

		/// <summary>
		/// Instantiates a new VM scheduler
		/// </summary>
		/// <param name="mips"> The individual MIPS capacity of each PE allocated to the VM using the scheduler,
		/// considering that all PEs have the same capacity. </param>
		/// <param name="numberOfPes"> The number of PEs allocated to the VM using the scheduler. </param>
		public CloudletSchedulerDynamicWorkload(double mips, int numberOfPes) : base()
		{
			Mips = mips;
			NumberOfPes = numberOfPes;
					/*@todo There shouldn't be a setter to total mips, considering
					that it is computed from number of PEs and mips.
					If the number of pes of mips is set any time after here,
					the total mips will be wrong. Just the getTotalMips is enough,
					and it have to compute there the total, instead of storing into an attribute.*/
			TotalMips = NumberOfPes * Mips;
			UnderAllocatedMips = new Dictionary<string, double?>();
			CachePreviousTime = -1;
		}

		public override double updateVmProcessing(double currentTime, IList<double?> mipsShare)
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
					continue;
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

			return nextEvent;
		}

		public override double cloudletSubmit(Cloudlet cl)
		{
			return cloudletSubmit(cl, 0);
		}

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

		public override void cloudletFinish(ResCloudlet rcl)
		{
			rcl.CloudletStatus = Cloudlet.SUCCESS;
			rcl.finalizeCloudlet();
			CloudletFinishedList.Add(rcl);
		}

		public override double getTotalUtilizationOfCpu(double time)
		{
			double totalUtilization = 0;
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				totalUtilization += rcl.Cloudlet.getUtilizationOfCpu(time);
			}
			return totalUtilization;
		}

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

		public override double getTotalCurrentRequestedMipsForCloudlet(ResCloudlet rcl, double time)
		{
			return rcl.Cloudlet.getUtilizationOfCpu(time) * TotalMips;
		}

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
		/// <param name="mips"> the mips
		/// @todo It is not clear the goal of this method. The related test case
		/// doesn't make it clear too. The method doesn't appear to be used anywhere. </param>
		public virtual void updateUnderAllocatedMipsForCloudlet(ResCloudlet rcl, double mips)
		{
			if (UnderAllocatedMips.ContainsKey(rcl.Uid))
			{
				mips += UnderAllocatedMips[rcl.Uid].Value;
			}
			UnderAllocatedMips[rcl.Uid] = mips;
		}

		/// <summary>
		/// Get the estimated completion time of a given cloudlet.
		/// </summary>
		/// <param name="rcl"> the cloudlet </param>
		/// <param name="time"> the time </param>
		/// <returns> the estimated finish time </returns>
		public virtual double getEstimatedFinishTime(ResCloudlet rcl, double time)
		{
			return time + ((rcl.RemainingCloudletLength) / getTotalCurrentAllocatedMipsForCloudlet(rcl, time));
		}

		/// <summary>
		/// Gets the total current mips available for the VM using the scheduler.
		/// The total is computed from the <seealso cref="#getCurrentMipsShare()"/>
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
		/// Gets the cache of previous time.
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
		/// Gets the cache of current requested mips.
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