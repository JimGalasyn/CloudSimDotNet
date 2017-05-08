using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.network.datacenter
{


	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
	using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using CloudSimLib.util;

	/// <summary>
	/// CloudletSchedulerSpaceShared implements a policy of scheduling performed by a virtual machine
	/// to run its <seealso cref="Cloudlet Cloudlets"/>. 
	/// It consider that there will be only one cloudlet per VM. Other cloudlets will be in a waiting list.
	/// We consider that file transfer from cloudlets waiting happens before cloudlet execution. I.e.,
	/// even though cloudlets must wait for CPU, data transfer happens as soon as cloudlets are
	/// submitted.
	/// 
	/// Each VM has to have its own instance of a CloudletScheduler.
	/// 
	/// @author Saurabh Kumar Garg
	/// @author Saurabh Kumar Garg
	/// @since CloudSim Toolkit 3.0
	/// @todo Attributes should be private
	/// </summary>
	public class NetworkCloudletSpaceSharedScheduler : CloudletScheduler
	{
		/// <summary>
		/// The current CPUs. </summary>
		protected internal int currentCpus;

		/// <summary>
		/// The used PEs. </summary>
		protected internal int usedPes;

			/// <summary>
			/// The map of packets to send, where each key is a destination VM
			/// and each value is the list of packets to sent to that VM.
			/// </summary>
		public IDictionary<int?, IList<HostPacket>> pkttosend;

			/// <summary>
			/// The map of packets received, where each key is a sender VM
			/// and each value is the list of packets sent by that VM.
			/// </summary>
		public IDictionary<int?, IList<HostPacket>> pktrecv;

		/// <summary>
		/// Creates a new CloudletSchedulerSpaceShared object. 
		/// This method must be invoked before starting the actual simulation.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public NetworkCloudletSpaceSharedScheduler() : base()
		{
			cloudletWaitingList = new List<ResCloudlet>();
			cloudletExecList = new List<ResCloudlet>();
			cloudletPausedList = new List<ResCloudlet>();
			cloudletFinishedList = new List<ResCloudlet>();
			usedPes = 0;
			currentCpus = 0;
			pkttosend = new Dictionary<int?, IList<HostPacket>>();
			pktrecv = new Dictionary<int?, IList<HostPacket>>();
		}

		public override double updateVmProcessing(double currentTime, IList<double?> mipsShare)
		{
					/*@todo Method to long. Several "extract method" refactorings may be performed.*/
			CurrentMipsShare = mipsShare;
			// update
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

			foreach (ResCloudlet rcl in CloudletExecList)
			{ // each machine in the
				// exec list has the
				// same amount of cpu

				NetworkCloudlet cl = (NetworkCloudlet) rcl.Cloudlet;

				// check status
				// if execution stage
				// update the cloudlet finishtime
				// CHECK WHETHER IT IS WAITING FOR THE PACKET
				// if packet received change the status of job and update the time.
				//
				if ((cl.currStagenum != -1))
				{
					if (cl.currStagenum == NetworkConstants.FINISH)
					{
						break;
					}
					TaskStage st = cl.stages[cl.currStagenum];
					if (st.type == NetworkConstants.EXECUTION)
					{

						// update the time
						cl.timespentInStage = Math.Round(CloudSim.clock() - cl.timetostartStage);
						if (cl.timespentInStage >= st.time)
						{
							changetonextstage(cl, st);
							// change the stage
						}
					}
					if (st.type == NetworkConstants.WAIT_RECV)
					{
						IList<HostPacket> pktlist = pktrecv[st.peer];
						IList<HostPacket> pkttoremove = new List<HostPacket>();
						if (pktlist != null)
						{
							IEnumerator<HostPacket> it = pktlist.GetEnumerator();
							HostPacket pkt = null;
                            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                            // TEST: (fixed) Fix iterator stuff
                            //if (it.hasNext())
                            if (it.MoveNext())
                            {
                                //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                                //pkt = it.next();
                                pkt = it.Current;
                                // Asumption packet will not arrive in the same cycle
                                if (pkt.reciever == cl.VmId)
								{
									pkt.recievetime = CloudSim.clock();
									st.time = CloudSim.clock() - pkt.sendtime;
									changetonextstage(cl, st);
									pkttoremove.Add(pkt);
								}
							}
                            //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
                            // TEST: (fixed) Figure out removeAll
                            //pktlist.removeAll(pkttoremove);
                            pktlist.RemoveAll<HostPacket>(pkttoremove);
                            // if(pkt!=null)
                            // else wait for recieving the packet
                        }
					}

				}
				else
				{
					cl.currStagenum = 0;
					cl.timetostartStage = CloudSim.clock();

					if (cl.stages[0].type == NetworkConstants.EXECUTION)
					{
						NetDatacenterBroker.linkDC.schedule(NetDatacenterBroker.linkDC.Id, cl.stages[0].time, CloudSimTags.VM_DATACENTER_EVENT);
					}
					else
					{
						NetDatacenterBroker.linkDC.schedule(NetDatacenterBroker.linkDC.Id, 0.0001, CloudSimTags.VM_DATACENTER_EVENT);
						// /sendstage///
					}
				}

			}

			if (CloudletExecList.Count == 0 && CloudletWaitingList.Count == 0)
			{ // no
				// more cloudlets in this scheduler
				PreviousTime = currentTime;
				return 0.0;
			}

			// update each cloudlet
			int finished = 0;
			IList<ResCloudlet> toRemove = new List<ResCloudlet>();
			foreach (ResCloudlet rcl in CloudletExecList)
			{
				// rounding issue...
				if (((NetworkCloudlet)(rcl.Cloudlet)).currStagenum == NetworkConstants.FINISH)
				{
					// stage is changed and packet to send
					((NetworkCloudlet)(rcl.Cloudlet)).finishtime = CloudSim.clock();
					toRemove.Add(rcl);
					cloudletFinish(rcl);
					finished++;
				}
			}
            // TEST: (fixed) removeAll biz
            //CloudletExecList.removeAll(toRemove);
            CloudletExecList.RemoveAll<ResCloudlet>(toRemove);
            // add all the CloudletExecList in waitingList.
            // sort the waitinglist

            // for each finished cloudlet, add a new one from the waiting list
            if (CloudletWaitingList.Count != 0)
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
                    // TEST: (fixed) removeAll biz
                    //CloudletWaitingList.removeAll(toRemove);
                    CloudletWaitingList.RemoveAll<ResCloudlet>(toRemove);
                } // for(cont)
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

			/// <summary>
			/// Changes a cloudlet to the next stage.
			/// 
			/// @todo It has to be corrected the method name case. Method too long
			/// to understand what is its responsibility.
			/// </summary>
		private void changetonextstage(NetworkCloudlet cl, TaskStage st)
		{
			cl.timespentInStage = 0;
			cl.timetostartStage = CloudSim.clock();
			int currstage = cl.currStagenum;
			if (currstage >= (cl.stages.Count - 1))
			{
				cl.currStagenum = NetworkConstants.FINISH;
			}
			else
			{
				cl.currStagenum = currstage + 1;
				int i = 0;
				for (i = cl.currStagenum; i < cl.stages.Count; i++)
				{
					if (cl.stages[i].type == NetworkConstants.WAIT_SEND)
					{
						HostPacket pkt = new HostPacket(cl.VmId, cl.stages[i].peer, cl.stages[i].data, CloudSim.clock(), -1, cl.CloudletId, cl.stages[i].vpeer);
						IList<HostPacket> pktlist = pkttosend[cl.VmId];
						if (pktlist == null)
						{
							pktlist = new List<HostPacket>();
						}
						pktlist.Add(pkt);
						pkttosend[cl.VmId] = pktlist;

					}
					else
					{
						break;
					}

				}
				NetDatacenterBroker.linkDC.schedule(NetDatacenterBroker.linkDC.Id, 0.0001, CloudSimTags.VM_DATACENTER_EVENT);
				if (i == cl.stages.Count)
				{
					cl.currStagenum = NetworkConstants.FINISH;
				}
				else
				{
					cl.currStagenum = i;
					if (cl.stages[i].type == NetworkConstants.EXECUTION)
					{
						NetDatacenterBroker.linkDC.schedule(NetDatacenterBroker.linkDC.Id, cl.stages[i].time, CloudSimTags.VM_DATACENTER_EVENT);
					}
				}
			}
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
					if (rcl.RemainingCloudletLength == 0.0)
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
                if (rgl.RemainingCloudletLength == 0.0)
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
                if (rgl.RemainingCloudletLength == 0.0)
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

			// not found in the paused list: either it is in in the queue, executing
			// or not exist
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
			cloudletSubmit(cloudlet, 0);
			return 0;
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
                    //return CloudletFinishedList.remove(0).Cloudlet;
                    var cloudlet = CloudletFinishedList[0].Cloudlet;
                    CloudletFinishedList.RemoveAt(0);
                    return cloudlet;
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
					//@todo The method doesn't appear to be implemented in fact
			return 0.0;
		}

		public override double getTotalCurrentRequestedMipsForCloudlet(ResCloudlet rcl, double time)
		{
					//@todo The method doesn't appear to be implemented in fact
			return 0.0;
		}

		public override double CurrentRequestedUtilizationOfBw
		{
			get
			{
						//@todo The method doesn't appear to be implemented in fact
				return 0;
			}
		}

		public override double CurrentRequestedUtilizationOfRam
		{
			get
			{
						//@todo The method doesn't appear to be implemented in fact
				return 0;
			}
		}

	}

}