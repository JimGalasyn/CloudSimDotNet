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
    using CloudSimLib.util;

    /// <summary>
    /// CloudletSchedulerSpaceShared implements a policy of scheduling performed by a virtual machine
    /// to run its <seealso cref="Cloudlet Cloudlets"/>.
    /// It consider there will be only one cloudlet per VM. Other cloudlets will be in a waiting list.
    /// We consider that file transfer from cloudlets waiting happens before cloudlet execution. I.e.,
    /// even though cloudlets must wait for CPU, data transfer happens as soon as cloudlets are
    /// submitted.
    /// 
    /// @author Rodrigo N. Calheiros
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class CloudletSchedulerSpaceShared : CloudletScheduler
	{
		/// <summary>
		/// The number of PEs currently available for the VM using the scheduler,
		/// according to the mips share provided to it by
		/// <seealso cref="#updateVmProcessing(double, java.util.List)"/> method. 
		/// </summary>
		protected internal int currentCpus;

		/// <summary>
		/// The number of used PEs. </summary>
		protected internal int usedPes;

		/// <summary>
		/// Creates a new CloudletSchedulerSpaceShared object. This method must be invoked before
		/// starting the actual simulation.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public CloudletSchedulerSpaceShared() : base()
		{
			usedPes = 0;
			currentCpus = 0;
		}

		public override double updateVmProcessing(double currentTime, IList<double?> mipsShare)
		{
			CurrentMipsShare = mipsShare;
			double timeSpam = currentTime - PreviousTime; // time since last update
			double capacity = 0.0;
			int cpus = 0;

			foreach (double? mips in mipsShare)
			{ // count the CPUs available to the VMM
				capacity += mips.Value;
				if (mips > 0)
				{
					cpus++;
				}
			}
			currentCpus = cpus;
			capacity /= cpus; // average capacity of each cpu

			// each machine in the exec list has the same amount of cpu
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				rcl.updateCloudletFinishedSoFar((long)(capacity * timeSpam * rcl.NumberOfPes * Consts.MILLION));
			}

			// no more cloudlets in this scheduler
			if (CloudletExecList.Count == 0 && CloudletWaitingList.Count == 0)
			{
				PreviousTime = currentTime;
				return 0.0;
			}

			// update each cloudlet
			int finished = 0;
			IList<ResCloudlet> toRemove = new List<ResCloudlet>();
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				// finished anyway, rounding issue...
				if (rcl.RemainingCloudletLength == 0)
				{
					toRemove.Add(rcl);
					cloudletFinish(rcl);
					finished++;
				}
			}

            //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
            // TEST: (Fixed) Make sure this removeAll equivalent works.
            //CloudletExecList.removeAll(toRemove);
            CloudletExecList.RemoveAll<ResCloudlet>(toRemove);
            //CloudletExecList.Clear();

            // for each finished cloudlet, add a new one from the waiting list
            if (CloudletWaitingList.Count > 0)
			{
				for (int i = 0; i < finished; i++)
				{
					toRemove.Clear();
					foreach (ResCloudlet rcl in CloudletWaitingList)
					{
						if ((currentCpus - usedPes) >= rcl.NumberOfPes)
						{
							rcl.CloudletStatus = Cloudlet.INEXEC;
							for (int k = 0; k < rcl.NumberOfPes; k++)
							{
								rcl.setMachineAndPeId(0, i);
							}
							CloudletExecList.Add(rcl);
							usedPes += rcl.NumberOfPes;
							toRemove.Add(rcl);
							break;
						}
					}
                    //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
                    // TEST: (Fixed) Make sure this removeAll equivalent works.
                    //CloudletWaitingList.removeAll(toRemove);
                    CloudletWaitingList.RemoveAll<ResCloudlet>(toRemove);
                }
			}

			// estimate finish time of cloudlets in the execution queue
			double nextEvent = double.MaxValue;
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				double remainingLength = rcl.RemainingCloudletLength;
				double estimatedFinishTime = currentTime + (remainingLength / (capacity * rcl.NumberOfPes));
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

		public override Cloudlet cloudletCancel(int cloudletId)
		{
			// First, looks in the finished queue
			foreach (ResCloudlet rcl in CloudletFinishedList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					CloudletFinishedList.Remove(rcl);
					return rcl.Cloudlet;
				}
			}

			// Then searches in the exec list
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					CloudletExecList.Remove(rcl);
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
			}

			// Now, looks in the paused queue
			foreach (ResCloudlet rcl in CloudletPausedList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					CloudletPausedList.Remove(rcl);
					return rcl.Cloudlet;
				}
			}

			// Finally, looks in the waiting list
			foreach (ResCloudlet rcl in CloudletWaitingList)
			{
				if (rcl.CloudletId == cloudletId)
				{
					rcl.CloudletStatus = Cloudlet.CANCELED;
					CloudletWaitingList.Remove(rcl);
					return rcl.Cloudlet;
				}
			}

			return null;

		}

		public override bool cloudletPause(int cloudletId)
		{
			bool found = false;
			int position = 0;

			// first, looks for the cloudlet in the exec list
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
				// moves to the paused list
				ResCloudlet rgl = CloudletExecList[position];
                CloudletExecList.RemoveAt(position);
                if (rgl.RemainingCloudletLength == 0)
				{
					cloudletFinish(rgl);
				}
				else
				{
					rgl.CloudletStatus = Cloudlet.PAUSED;
					CloudletPausedList.Add(rgl);
				}
				return true;

			}

			// now, look for the cloudlet in the waiting list
			position = 0;
			found = false;
			foreach (ResCloudlet rcl in CloudletWaitingList)
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
				// moves to the paused list
				ResCloudlet rgl = CloudletWaitingList[position];
                CloudletWaitingList.RemoveAt(position);
                if (rgl.RemainingCloudletLength == 0)
				{
					cloudletFinish(rgl);
				}
				else
				{
					rgl.CloudletStatus = Cloudlet.PAUSED;
					CloudletPausedList.Add(rgl);
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
			usedPes -= rcl.NumberOfPes;
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
				ResCloudlet rcl = CloudletPausedList[position];
                CloudletPausedList.RemoveAt(position);

                // it can go to the exec list
                if ((currentCpus - usedPes) >= rcl.NumberOfPes)
				{
					rcl.CloudletStatus = Cloudlet.INEXEC;
					for (int i = 0; i < rcl.NumberOfPes; i++)
					{
						rcl.setMachineAndPeId(0, i);
					}

					long size = rcl.RemainingCloudletLength;
					size *= rcl.NumberOfPes;
					rcl.Cloudlet.CloudletLength = size;

					CloudletExecList.Add(rcl);
					usedPes += rcl.NumberOfPes;

					// calculate the expected time for cloudlet completion
					double capacity = 0.0;
					int cpus = 0;
					foreach (double? mips in CurrentMipsShare)
					{
						capacity += mips.Value;
						if (mips > 0)
						{
							cpus++;
						}
					}
					currentCpus = cpus;
					capacity /= cpus;

					long remainingLength = rcl.RemainingCloudletLength;
					double estimatedFinishTime = CloudSim.clock() + (remainingLength / (capacity * rcl.NumberOfPes));

					return estimatedFinishTime;
				}
				else
				{ // no enough free PEs: go to the waiting queue
					rcl.CloudletStatus = Cloudlet.QUEUED;

					long size = rcl.RemainingCloudletLength;
					size *= rcl.NumberOfPes;
					rcl.Cloudlet.CloudletLength = size;

					CloudletWaitingList.Add(rcl);
					return 0.0;
				}

			}

			// not found in the paused list: either it is in in the queue, executing or not exist
			return 0.0;

		}

		public override double cloudletSubmit(Cloudlet cloudlet, double fileTransferTime)
		{
			// it can go to the exec list
			if ((currentCpus - usedPes) >= cloudlet.NumberOfPes)
			{
				ResCloudlet rcl = new ResCloudlet(cloudlet);
				rcl.CloudletStatus = Cloudlet.INEXEC;
				for (int i = 0; i < cloudlet.NumberOfPes; i++)
				{
					rcl.setMachineAndPeId(0, i);
				}
				CloudletExecList.Add(rcl);
				usedPes += cloudlet.NumberOfPes;
			}
			else
			{ // no enough free PEs: go to the waiting queue
				ResCloudlet rcl = new ResCloudlet(cloudlet);
				rcl.CloudletStatus = Cloudlet.QUEUED;
				CloudletWaitingList.Add(rcl);
				return 0.0;
			}

			// calculate the expected time for cloudlet completion
			double capacity = 0.0;
			int cpus = 0;
			foreach (double? mips in CurrentMipsShare)
			{
				capacity += mips.Value;
				if (mips > 0)
				{
					cpus++;
				}
			}

			currentCpus = cpus;
			capacity /= cpus;

			// use the current capacity to estimate the extra amount of
			// time to file transferring. It must be added to the cloudlet length
			double extraSize = capacity * fileTransferTime;
			long length = cloudlet.CloudletLength;
			length += (long)extraSize;
			cloudlet.CloudletLength = length;
			return cloudlet.CloudletLength / capacity;
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

			foreach (ResCloudlet rcl in CloudletWaitingList)
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
                    var retval = CloudletFinishedList[0].Cloudlet;
                    CloudletFinishedList.RemoveAt(0);
                    return retval;
                    //return CloudletFinishedList.Remove(0).Cloudlet;
                }
				return null;
			}
		}

		public override int runningCloudlets()
		{
			return CloudletExecList.Count;
		}

		/// <summary>
		/// Returns the first cloudlet to migrate to another VM.
		/// </summary>
		/// <returns> the first running cloudlet
		/// @pre $none
		/// @post $none
		/// 
		/// @todo it doesn't check if the list is empty </returns>
		public override Cloudlet migrateCloudlet()
		{
			ResCloudlet rcl = CloudletExecList[0];
            CloudletExecList.RemoveAt(0);
            rcl.finalizeCloudlet();
			Cloudlet cl = rcl.Cloudlet;
			usedPes -= cl.NumberOfPes;
			return cl;
		}

		public override IList<double?> CurrentRequestedMips
		{
			get
			{
				IList<double?> mipsShare = new List<double?>();
				if (CurrentMipsShare != null)
				{
					foreach (double? mips in CurrentMipsShare)
					{
						mipsShare.Add(mips);
					}
				}
				return mipsShare;
			}
		}

		public override double getTotalCurrentAvailableMipsForCloudlet(ResCloudlet rcl, IList<double?> mipsShare)
		{
					/*@todo The param rcl is not being used.*/
			double capacity = 0.0;
			int cpus = 0;
			foreach (double? mips in mipsShare)
			{ // count the cpus available to the vmm
				capacity += mips.Value;
				if (mips > 0)
				{
					cpus++;
				}
			}
			currentCpus = cpus;
			capacity /= cpus; // average capacity of each cpu
			return capacity;
		}

		public override double getTotalCurrentAllocatedMipsForCloudlet(ResCloudlet rcl, double time)
		{
            //@todo the method isn't in fact implemented
            // TEST: (fixed) Auto-generated method stub
            return 0.0;
		}

		public override double getTotalCurrentRequestedMipsForCloudlet(ResCloudlet rcl, double time)
		{
            //@todo the method isn't in fact implemented
            // TEST: (fixed) Auto-generated method stub
            return 0.0;
		}

		public override double CurrentRequestedUtilizationOfRam
		{
			get
			{
                //@todo the method isn't in fact implemented
                // TEST: (fixed) Auto-generated method stub
                return 0;
			}
		}

		public override double CurrentRequestedUtilizationOfBw
		{
			get
			{
                //@todo the method isn't in fact implemented
                // TEST: (fixed) Auto-generated method stub
                return 0;
			}
		}
	}
}