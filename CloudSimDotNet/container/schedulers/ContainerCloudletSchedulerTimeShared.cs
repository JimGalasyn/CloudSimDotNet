using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.schedulers
{

	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimLib.util;


	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerCloudletSchedulerTimeShared : ContainerCloudletScheduler
	{

		/// <summary>
		/// The current cp us.
		/// </summary>
		protected internal int currentCPUs;

		/// <summary>
		/// Creates a new ContainerCloudletSchedulerTimeShared object. This method must be invoked before starting
		/// the actual simulation.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public ContainerCloudletSchedulerTimeShared() : base()
		{
			currentCPUs = 0;
		}

		/// <summary>
		/// Updates the processing of cloudlets running under management of this scheduler.
		/// </summary>
		/// <param name="currentTime"> current simulation time </param>
		/// <param name="mipsShare">   array with MIPS share of each processor available to the scheduler </param>
		/// <returns> time predicted completion time of the earliest finishing cloudlet, or 0 if there is
		/// no next events
		/// @pre currentTime >= 0
		/// @post $none </returns>
		public override double updateContainerProcessing(double currentTime, IList<double?> mipsShare)
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
				}
			}
            // TEST: (fixed) Find removeAll equivalent.
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

			toRemove.Clear();
			PreviousTime = currentTime;
			return nextEvent;
		}

		/// <summary>
		/// Gets the capacity.
		/// </summary>
		/// <param name="mipsShare"> the mips share </param>
		/// <returns> the capacity </returns>
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

		/// <summary>
		/// Cancels execution of a cloudlet.
		/// </summary>
		/// <param name="cloudletId"> ID of the cloudlet being cancealed </param>
		/// <returns> the canceled cloudlet, $null if not found
		/// @pre $none
		/// @post $none </returns>
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
                //return CloudletFinishedList.Remove(position).Cloudlet;
                ResCloudlet resCloudlet = CloudletFinishedList[position];
                CloudletFinishedList.RemoveAt(position);
                return resCloudlet.Cloudlet;
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
                ResCloudlet resCloudlet = CloudletPausedList[position];
                CloudletPausedList.RemoveAt(position);
                return resCloudlet.Cloudlet;
            }

            return null;
		}

		/// <summary>
		/// Pauses execution of a cloudlet.
		/// </summary>
		/// <param name="cloudletId"> ID of the cloudlet being paused </param>
		/// <returns> $true if cloudlet paused, $false otherwise
		/// @pre $none
		/// @post $none </returns>
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
		/// Resumes execution of a paused cloudlet.
		/// </summary>
		/// <param name="cloudletId"> ID of the cloudlet being resumed </param>
		/// <returns> expected finish time of the cloudlet, 0.0 if queued
		/// @pre $none
		/// @post $none </returns>
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


		/// <summary>
		/// Receives an cloudlet to be executed in the VM managed by this scheduler.
		/// </summary>
		/// <param name="cloudlet">         the submited cloudlet </param>
		/// <param name="fileTransferTime"> time required to move the required files from the SAN to the VM </param>
		/// <returns> expected finish time of this cloudlet
		/// @pre gl != null
		/// @post $none </returns>
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

		/*
		 * (non-Javadoc)
		 * @see cloudsim.CloudletScheduler#cloudletSubmit(cloudsim.Cloudlet)
		 */
		public override double cloudletSubmit(Cloudlet cloudlet)
		{
			return cloudletSubmit(cloudlet, 0.0);
		}

		/// <summary>
		/// Gets the status of a cloudlet.
		/// </summary>
		/// <param name="cloudletId"> ID of the cloudlet </param>
		/// <returns> status of the cloudlet, -1 if cloudlet not found
		/// @pre $none
		/// @post $none </returns>
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

		/// <summary>
		/// Get utilization created by all cloudlets.
		/// </summary>
		/// <param name="time"> the time </param>
		/// <returns> total utilization </returns>
		public override double getTotalUtilizationOfCpu(double time)
		{
			double totalUtilization = 0;
			foreach (ResCloudlet gl in CloudletExecList)
			{
				totalUtilization += gl.Cloudlet.getUtilizationOfCpu(time);
			}
			return totalUtilization;
		}

		/// <summary>
		/// Informs about completion of some cloudlet in the VM managed by this scheduler.
		/// </summary>
		/// <returns> $true if there is at least one finished cloudlet; $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public override bool FinishedCloudlets
		{
			get
			{
				return CloudletFinishedList.Count > 0;
			}
		}

		/// <summary>
		/// Returns the next cloudlet in the finished list, $null if this list is empty.
		/// </summary>
		/// <returns> a finished cloudlet
		/// @pre $none
		/// @post $none </returns>
		public override Cloudlet NextFinishedCloudlet
		{
			get
			{
				if (CloudletFinishedList.Count > 0)
				{
                    //return CloudletFinishedList.remove(0).Cloudlet;
                    var cloudlet = CloudletFinishedList[0].Cloudlet;
                    CloudletFinishedList.RemoveAt(0);
                    return cloudlet;
                }
				return null;
			}
		}

		/// <summary>
		/// Returns the number of cloudlets runnning in the virtual machine.
		/// </summary>
		/// <returns> number of cloudlets runnning
		/// @pre $none
		/// @post $none </returns>
		public override int runningCloudlets()
		{
			return CloudletExecList.Count;
		}

		/// <summary>
		/// Returns one cloudlet to migrate to another vm.
		/// </summary>
		/// <returns> one running cloudlet
		/// @pre $none
		/// @post $none </returns>
		public override Cloudlet migrateCloudlet()
		{
			ResCloudlet rgl = CloudletExecList[0];
            CloudletExecList.RemoveAt(0);
            rgl.finalizeCloudlet();
			return rgl.Cloudlet;
		}


		/*
		 * (non-Javadoc)
		 * @see cloudsim.CloudletScheduler#getCurrentRequestedMips()
		 */
		public override IList<double?> CurrentRequestedMips
		{
			get
			{
				return new List<double?>();
			}
		}

		/*
		 * (non-Javadoc)
		 * @see cloudsim.CloudletScheduler#getTotalCurrentAvailableMipsForCloudlet(cloudsim.ResCloudlet,
		 * java.util.List)
		 */
		public override double getTotalCurrentAvailableMipsForCloudlet(ResCloudlet rcl, IList<double?> mipsShare)
		{
			return getCapacity(CurrentMipsShare);
		}

		/*
		 * (non-Javadoc)
		 * @see cloudsim.CloudletScheduler#getTotalCurrentAllocatedMipsForCloudlet(cloudsim.ResCloudlet,
		 * double)
		 */
		public override double getTotalCurrentAllocatedMipsForCloudlet(ResCloudlet rcl, double time)
		{
			return 0.0;
		}

		/*
		 * (non-Javadoc)
		 * @see cloudsim.CloudletScheduler#getTotalCurrentRequestedMipsForCloudlet(cloudsim.ResCloudlet,
		 * double)
		 */
		public override double getTotalCurrentRequestedMipsForCloudlet(ResCloudlet rcl, double time)
		{
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