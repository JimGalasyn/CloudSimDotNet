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
    using CloudSimLib.util;

    /// <summary>
    /// CloudletSchedulerTimeShared implements a policy of scheduling performed by a virtual machine
    /// to run its <seealso cref="Cloudlet Cloudlets"/>.
    /// Cloudlets execute in time-shared manner in VM.
    /// Each VM has to have its own instance of a CloudletScheduler.
    /// 
    /// @author Rodrigo N. Calheiros
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class CloudletSchedulerTimeShared : CloudletScheduler
	{
		/// <summary>
		/// The number of PEs currently available for the VM using the scheduler,
		/// according to the mips share provided to it by
		/// <seealso cref="#updateVmProcessing(double, java.util.List)"/> method. 
		/// </summary>
		protected internal int currentCPUs;

		/// <summary>
		/// Creates a new CloudletSchedulerTimeShared object. This method must be invoked before starting
		/// the actual simulation.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public CloudletSchedulerTimeShared() : base()
		{
			currentCPUs = 0;
		}

		public override double updateVmProcessing(double currentTime, IList<double?> mipsShare)
		{
			CurrentMipsShare = mipsShare;
			double timeSpam = currentTime - PreviousTime;

			foreach (ResCloudlet rcl in CloudletExecList)
			{
				rcl.updateCloudletFinishedSoFar((long)(getCapacity(mipsShare) * timeSpam * rcl.NumberOfPes * Consts.MILLION));
			}

			if (CloudletExecList.Count == 0)
			{
				PreviousTime = currentTime;
				return 0.0;
			}

			// check finished cloudlets
			double nextEvent = double.MaxValue;
			IList<ResCloudlet> toRemove = new List<ResCloudlet>();
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				long remainingLength = rcl.RemainingCloudletLength;
				if (remainingLength == 0)
				{ // finished: remove from the list
					toRemove.Add(rcl);
					cloudletFinish(rcl);
					continue;
				}
			}
            // TEST: (fixed) Deal with this selective remove
            //CloudletExecList.removeAll(toRemove);
            CloudletExecList.RemoveAll<ResCloudlet>(toRemove);

            // estimate finish time of cloudlets
            foreach (ResCloudlet rcl in CloudletExecList)
			{
				double estimatedFinishTime = currentTime + (rcl.RemainingCloudletLength / (getCapacity(mipsShare) * rcl.NumberOfPes));
				if (estimatedFinishTime - currentTime < CloudSim.MinTimeBetweenEvents)
				{
					estimatedFinishTime = currentTime + CloudSim.MinTimeBetweenEvents;
				}

				if (estimatedFinishTime < nextEvent)
				{
					nextEvent = estimatedFinishTime;
				}
			}

			PreviousTime = currentTime;
			return nextEvent;
		}

		/// <summary>
		/// Gets the individual MIPS capacity available for each PE available for the scheduler,
		/// considering that all PEs have the same capacity.
		/// </summary>
		/// <param name="mipsShare"> list with MIPS share of each PE available to the scheduler </param>
		/// <returns> the capacity of each PE </returns>
		protected internal virtual double getCapacity(IList<double?> mipsShare)
		{
			double capacity = 0.0;
			int cpus = 0;
			foreach (double? mips in mipsShare)
			{
				capacity += mips.Value;
				if (mips > 0.0)
				{
					cpus++;
				}
			}
			currentCPUs = cpus;

			int pesInUse = 0;
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				pesInUse += rcl.NumberOfPes;
			}

			if (pesInUse > currentCPUs)
			{
				capacity /= pesInUse;
			}
			else
			{
				capacity /= currentCPUs;
			}
			return capacity;
		}

		public override Cloudlet cloudletCancel(int cloudletId)
		{
			bool found = false;
			int position = 0;

			// First, looks in the finished queue
			found = false;
			foreach (ResCloudlet rcl in CloudletFinishedList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					found = true;
					break;
				}
				position++;
			}

			if (found)
			{
                //return CloudletFinishedList.remove(position).Cloudlet;
                var cloudlet = CloudletFinishedList[position].Cloudlet;
                CloudletFinishedList.RemoveAt(position);
                return cloudlet;
            }

			// Then searches in the exec list
			position = 0;
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					found = true;
					break;
				}
				position++;
			}

			if (found)
			{
				ResCloudlet rcl = CloudletExecList[position];
                CloudletExecList.RemoveAt(position);
                if (rcl.RemainingCloudletLength == 0)
				{
					cloudletFinish(rcl);
				}
				else
				{
					rcl.CloudletStatus = Cloudlet.CANCELED;
				}
				return rcl.Cloudlet;
			}

			// Now, looks in the paused queue
			found = false;
			position = 0;
			foreach (ResCloudlet rcl in CloudletPausedList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					found = true;
					rcl.CloudletStatus = Cloudlet.CANCELED;
					break;
				}
				position++;
			}

			if (found)
			{
                //return CloudletPausedList.remove(position).Cloudlet;
                var cloudlet = CloudletPausedList[position].Cloudlet;
                CloudletPausedList.RemoveAt(position);
                return cloudlet;
            }

			return null;
		}

		public override bool cloudletPause(int cloudletId)
		{
			bool found = false;
			int position = 0;

			foreach (ResCloudlet rcl in CloudletExecList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					found = true;
					break;
				}
				position++;
			}

			if (found)
			{
				// remove cloudlet from the exec list and put it in the paused list
				ResCloudlet rcl = CloudletExecList[position];
                CloudletExecList.RemoveAt(position);
                if (rcl.RemainingCloudletLength == 0)
				{
					cloudletFinish(rcl);
				}
				else
				{
					rcl.CloudletStatus = Cloudlet.PAUSED;
					CloudletPausedList.Add(rcl);
				}
				return true;
			}
			return false;
		}

		public override void cloudletFinish(ResCloudlet rcl)
		{
			rcl.CloudletStatus = Cloudlet.SUCCESS;
			rcl.finalizeCloudlet();
			CloudletFinishedList.Add(rcl);
		}

		public override double cloudletResume(int cloudletId)
		{
			bool found = false;
			int position = 0;

			// look for the cloudlet in the paused list
			foreach (ResCloudlet rcl in CloudletPausedList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					found = true;
					break;
				}
				position++;
			}

			if (found)
			{
				ResCloudlet rgl = CloudletPausedList[position];
                CloudletPausedList.RemoveAt(position);
                rgl.CloudletStatus = Cloudlet.INEXEC;
				CloudletExecList.Add(rgl);

				// calculate the expected time for cloudlet completion
				// first: how many PEs do we have?

				double remainingLength = rgl.RemainingCloudletLength;
				double estimatedFinishTime = CloudSim.clock() + (remainingLength / (getCapacity(CurrentMipsShare) * rgl.NumberOfPes));

				return estimatedFinishTime;
			}

			return 0.0;
		}

		public override double cloudletSubmit(Cloudlet cloudlet, double fileTransferTime)
		{
			ResCloudlet rcl = new ResCloudlet(cloudlet);
			rcl.CloudletStatus = Cloudlet.INEXEC;
			for (int i = 0; i < cloudlet.NumberOfPes; i++)
			{
				rcl.setMachineAndPeId(0, i);
			}

			CloudletExecList.Add(rcl);

			// use the current capacity to estimate the extra amount of
			// time to file transferring. It must be added to the cloudlet length
			double extraSize = getCapacity(CurrentMipsShare) * fileTransferTime;
			long length = (long)(cloudlet.CloudletLength + extraSize);
			cloudlet.CloudletLength = length;

			return cloudlet.CloudletLength / getCapacity(CurrentMipsShare);
		}

		public override double cloudletSubmit(Cloudlet cloudlet)
		{
			return cloudletSubmit(cloudlet, 0.0);
		}

		public override int getCloudletStatus(int cloudletId)
		{
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					return rcl.CloudletStatus;
				}
			}
			foreach (ResCloudlet rcl in CloudletPausedList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					return rcl.CloudletStatus;
				}
			}
			return -1;
		}

		public override double getTotalUtilizationOfCpu(double time)
		{
					/*
					 * @todo 
					 */
			double totalUtilization = 0;
			foreach (ResCloudlet gl in CloudletExecList)
			{
				totalUtilization += gl.Cloudlet.getUtilizationOfCpu(time);
			}
			return totalUtilization;
		}

		public override bool FinishedCloudlets
		{
			get
			{
				return CloudletFinishedList.Count > 0;
			}
		}

		public override Cloudlet NextFinishedCloudlet
		{
			get
			{
				if (CloudletFinishedList.Count > 0)
				{
                    // TEST: (fixed) Resolve ResCloudlet vs. Cloudlet issue.
                    //return CloudletFinishedList.remove(0).Cloudlet;
                    var cloudlet = CloudletFinishedList[0];
                    CloudletFinishedList.RemoveAt(0);
                    return cloudlet.Cloudlet;
                }
				return null;
			}
		}

		public override int runningCloudlets()
		{
			return CloudletExecList.Count;
		}

		public override Cloudlet migrateCloudlet()
		{
			ResCloudlet rgl = CloudletExecList[0];
            CloudletExecList.RemoveAt(0);
            rgl.finalizeCloudlet();
			return rgl.Cloudlet;
		}

		public override IList<double?> CurrentRequestedMips
		{
			get
			{
				IList<double?> mipsShare = new List<double?>();
				return mipsShare;
			}
		}

		public override double getTotalCurrentAvailableMipsForCloudlet(ResCloudlet rcl, IList<double?> mipsShare)
		{
				/*@todo It isn't being used any the the given parameters.*/
				return getCapacity(CurrentMipsShare);
		}

		public override double getTotalCurrentAllocatedMipsForCloudlet(ResCloudlet rcl, double time)
		{
					//@todo The method is not implemented, in fact
			return 0.0;
		}

		public override double getTotalCurrentRequestedMipsForCloudlet(ResCloudlet rcl, double time)
		{
            //@todo The method is not implemented, in fact
            // TEST: (fixed) Auto-generated method stub
            return 0.0;
		}

		public override double CurrentRequestedUtilizationOfRam
		{
			get
			{
				double ram = 0;
				foreach (ResCloudlet cloudlet in cloudletExecList)
				{
					ram += cloudlet.Cloudlet.getUtilizationOfRam(CloudSim.clock());
				}
				return ram;
			}
		}

		public override double CurrentRequestedUtilizationOfBw
		{
			get
			{
				double bw = 0;
				foreach (ResCloudlet cloudlet in cloudletExecList)
				{
					bw += cloudlet.Cloudlet.getUtilizationOfBw(CloudSim.clock());
				}
				return bw;
			}
		}

	}

}