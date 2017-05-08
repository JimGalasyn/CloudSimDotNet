using System;

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

	/// <summary>
	/// CloudSim ResCloudlet represents a Cloudlet submitted to CloudResource for processing. This class
	/// keeps track the time for all activities in the CloudResource for a specific Cloudlet. Before a
	/// Cloudlet exits the CloudResource, it is RECOMMENDED to call this method
	/// <seealso cref="#finalizeCloudlet()"/>.
	/// <p/>
	/// It contains a Cloudlet object along with its arrival time and the ID of the machine and the Pe
	/// (Processing Element) allocated to it. It acts as a placeholder for maintaining the amount of
	/// resource share allocated at various times for simulating any scheduling using internal events.
	/// 
	/// @author Manzur Murshed
	/// @author Rajkumar Buyya
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class ResCloudlet
	{

		/// <summary>
		/// The Cloudlet object. </summary>
		private readonly Cloudlet cloudlet;

		/// <summary>
		/// The Cloudlet arrival time for the first time. </summary>
		private double arrivalTime;

		/// <summary>
		/// The estimation of Cloudlet finished time. </summary>
		private double finishedTime;

		/// <summary>
		/// The length of Cloudlet finished so far. </summary>
		private long cloudletFinishedSoFar;

		/// <summary>
		/// Cloudlet execution start time. This attribute will only hold the latest time since a Cloudlet
		/// can be canceled, paused or resumed.
		/// </summary>
		private double startExecTime;

		/// <summary>
		/// The total time to complete this Cloudlet. </summary>
		private double totalCompletionTime;

		// The below attributes are only to be used by the SpaceShared policy.

		/// <summary>
		/// The machine id this Cloudlet is assigned to. </summary>
		private int machineId;

		/// <summary>
		/// The Pe id this Cloudlet is assigned to. </summary>
		private int peId;

		/// <summary>
		/// The an array of machine IDs. </summary>
		private int[] machineArrayId = null;

		/// <summary>
		/// The an array of Pe IDs. </summary>
		private int[] peArrayId = null;

		/// <summary>
		/// The index of machine and Pe arrays. </summary>
		private int index;

		// NOTE: Below attributes are related to AR stuff

		/// <summary>
		/// The Constant NOT_FOUND. </summary>
		private const int NOT_FOUND = -1;

		/// <summary>
		/// The reservation start time. </summary>
		private readonly long startTime;

		/// <summary>
		/// The reservation duration time. </summary>
		private readonly int duration;

		/// <summary>
		/// The reservation id. </summary>
		private readonly int reservId;

		/// <summary>
		/// The num Pe needed to execute this Cloudlet. </summary>
		private int pesNumber;

		/// <summary>
		/// Allocates a new ResCloudlet object upon the arrival of a Cloudlet object. The arriving time
		/// is determined by <seealso cref="gridsim.CloudSim#clock()"/>.
		/// </summary>
		/// <param name="cloudlet"> a cloudlet object </param>
		/// <seealso cref= gridsim.CloudSim#clock()
		/// @pre cloudlet != null
		/// @post $none </seealso>
		public ResCloudlet(Cloudlet cloudlet)
		{
			// when a new ResCloudlet is created, then it will automatically set
			// the submission time and other properties, such as remaining length
			this.cloudlet = cloudlet;
			startTime = 0;
			reservId = NOT_FOUND;
			duration = 0;

			init();
		}

		/// <summary>
		/// Allocates a new ResCloudlet object upon the arrival of a Cloudlet object. Use this
		/// constructor to store reserved Cloudlets, i.e. Cloudlets that done reservation before. The
		/// arriving time is determined by <seealso cref="gridsim.CloudSim#clock()"/>.
		/// </summary>
		/// <param name="cloudlet"> a cloudlet object </param>
		/// <param name="startTime"> a reservation start time. Can also be interpreted as starting time to
		///            execute this Cloudlet. </param>
		/// <param name="duration"> a reservation duration time. Can also be interpreted as how long to execute
		///            this Cloudlet. </param>
		/// <param name="reservID"> a reservation ID that owns this Cloudlet </param>
		/// <seealso cref= gridsim.CloudSim#clock()
		/// @pre cloudlet != null
		/// @pre startTime > 0
		/// @pre duration > 0
		/// @pre reservID > 0
		/// @post $none </seealso>
		public ResCloudlet(Cloudlet cloudlet, long startTime, int duration, int reservID)
		{
			this.cloudlet = cloudlet;
			this.startTime = startTime;
			reservId = reservID;
			this.duration = duration;

			init();
		}

		/// <summary>
		/// Gets the Cloudlet or reservation start time.
		/// </summary>
		/// <returns> Cloudlet's starting time
		/// @pre $none
		/// @post $none </returns>
		public virtual long StartTime
		{
			get
			{
				return startTime;
			}
		}

		/// <summary>
		/// Gets the reservation duration time.
		/// </summary>
		/// <returns> reservation duration time
		/// @pre $none
		/// @post $none </returns>
		public virtual int DurationTime
		{
			get
			{
				return duration;
			}
		}

		/// <summary>
		/// Gets the number of PEs required to execute this Cloudlet.
		/// </summary>
		/// <returns> number of Pe
		/// @pre $none
		/// @post $none </returns>
		public virtual int NumberOfPes
		{
			get
			{
				return pesNumber;
			}
		}

		/// <summary>
		/// Gets the reservation ID that owns this Cloudlet.
		/// </summary>
		/// <returns> a reservation ID
		/// @pre $none
		/// @post $none </returns>
		public virtual int ReservationID
		{
			get
			{
				return reservId;
			}
		}

		/// <summary>
		/// Checks whether this Cloudlet is submitted by reserving or not.
		/// </summary>
		/// <returns> <tt>true</tt> if this Cloudlet has reserved before, <tt>false</tt> otherwise
		/// @pre $none
		/// @post $none </returns>
		public virtual bool hasReserved()
		{
			if (reservId == NOT_FOUND)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Initialises all local attributes.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		private void init()
		{
			// get number of PEs required to run this Cloudlet
			pesNumber = cloudlet.NumberOfPes;

			// if more than 1 Pe, then create an array
			if (pesNumber > 1)
			{
				machineArrayId = new int[pesNumber];
				peArrayId = new int[pesNumber];
			}

			arrivalTime = CloudSim.clock();
			cloudlet.SubmissionTime = arrivalTime;

			// default values
			finishedTime = NOT_FOUND; // Cannot finish in this hourly slot.
			machineId = NOT_FOUND;
			peId = NOT_FOUND;
			index = 0;
			totalCompletionTime = 0.0;
			startExecTime = 0.0;

			// In case a Cloudlet has been executed partially by some other grid
			// hostList.
			cloudletFinishedSoFar = cloudlet.CloudletFinishedSoFar * Consts.MILLION;
		}

		/// <summary>
		/// Gets this Cloudlet entity Id.
		/// </summary>
		/// <returns> the Cloudlet entity Id
		/// @pre $none
		/// @post $none </returns>
		public virtual int CloudletId
		{
			get
			{
				return cloudlet.CloudletId;
			}
		}

		/// <summary>
		/// Gets the user or owner of this Cloudlet.
		/// </summary>
		/// <returns> the Cloudlet's user Id
		/// @pre $none
		/// @post $none </returns>
		public virtual int UserId
		{
			get
			{
				return cloudlet.UserId;
			}
		}

		/// <summary>
		/// Gets the Cloudlet's length.
		/// </summary>
		/// <returns> Cloudlet's length
		/// @pre $none
		/// @post $none </returns>
		public virtual long CloudletLength
		{
			get
			{
				return cloudlet.CloudletLength;
			}
		}

		/// <summary>
		/// Gets the total Cloudlet's length (across all PEs).
		/// </summary>
		/// <returns> total Cloudlet's length
		/// @pre $none
		/// @post $none </returns>
		public virtual long CloudletTotalLength
		{
			get
			{
				return cloudlet.CloudletTotalLength;
			}
		}

		/// <summary>
		/// Gets the Cloudlet's class type.
		/// </summary>
		/// <returns> class type of the Cloudlet
		/// @pre $none
		/// @post $none </returns>
		public virtual int CloudletClassType
		{
			get
			{
				return cloudlet.ClassType;
			}
		}

		/// <summary>
		/// Sets the Cloudlet status.
		/// </summary>
		/// <param name="status"> the Cloudlet status </param>
		/// <returns> <tt>true</tt> if the new status has been set, <tt>false</tt> otherwise
		/// @pre status >= 0
		/// @post $none </returns>
		public virtual bool setCloudletStatus(int status)
		{
			// gets Cloudlet's previous status
			int prevStatus = cloudlet.CloudletStatus;

			// if the status of a Cloudlet is the same as last time, then ignore
			if (prevStatus == status)
			{
				return false;
			}

			bool success = true;
			try
			{
				double clock = CloudSim.clock(); // gets the current clock

				// sets Cloudlet's current status
				cloudlet.CloudletStatus = status;

				// if a previous Cloudlet status is INEXEC
				if (prevStatus == Cloudlet.INEXEC)
				{
					// and current status is either CANCELED, PAUSED or SUCCESS
					if (status == Cloudlet.CANCELED || status == Cloudlet.PAUSED || status == Cloudlet.SUCCESS)
					{
						// then update the Cloudlet completion time
						totalCompletionTime += (clock - startExecTime);
						index = 0;
						return true;
					}
				}

				if (prevStatus == Cloudlet.RESUMED && status == Cloudlet.SUCCESS)
				{
					// then update the Cloudlet completion time
					totalCompletionTime += (clock - startExecTime);
					return true;
				}

				// if a Cloudlet is now in execution
				if (status == Cloudlet.INEXEC || (prevStatus == Cloudlet.PAUSED && status == Cloudlet.RESUMED))
				{
					startExecTime = clock;
					cloudlet.ExecStartTime = startExecTime;
				}

			}
			catch (Exception)
			{
				success = false;
			}

			return success;
		}

		/// <summary>
		/// Gets the Cloudlet's execution start time.
		/// </summary>
		/// <returns> Cloudlet's execution start time
		/// @pre $none
		/// @post $none </returns>
		public virtual double ExecStartTime
		{
			get
			{
				return cloudlet.ExecStartTime;
			}
		}

		/// <summary>
		/// Sets this Cloudlet's execution parameters. These parameters are set by the CloudResource
		/// before departure or sending back to the original Cloudlet's owner.
		/// </summary>
		/// <param name="wallClockTime"> the time of this Cloudlet resides in a CloudResource (from arrival time
		///            until departure time). </param>
		/// <param name="actualCPUTime"> the total execution time of this Cloudlet in a CloudResource.
		/// @pre wallClockTime >= 0.0
		/// @pre actualCPUTime >= 0.0
		/// @post $none </param>
		public virtual void setExecParam(double wallClockTime, double actualCPUTime)
		{
			cloudlet.setExecParam(wallClockTime, actualCPUTime);
		}

		/// <summary>
		/// Sets the machine and Pe (Processing Element) ID.
		/// </summary>
		/// <param name="machineId"> machine ID </param>
		/// <param name="peId"> Pe ID
		/// @pre machineID >= 0
		/// @pre peID >= 0
		/// @post $none
		/// 
		/// @todo the machineId param and attribute mean a VM or a PM id?
		/// Only the term machine is ambiguous. 
		/// At <seealso cref=" CloudletSchedulerTimeShared#cloudletSubmit(org.cloudbus.cloudsim.Cloudlet)"/>
		/// it is stated it is a VM. </param>
		public virtual void setMachineAndPeId(int machineId, int peId)
		{
			// if this job only requires 1 Pe
			this.machineId = machineId;
			this.peId = peId;

			// if this job requires many PEs
			if (peArrayId != null && pesNumber > 1)
			{
				machineArrayId[index] = machineId;
				peArrayId[index] = peId;
				index++;
			}
		}

		/// <summary>
		/// Gets machine ID.
		/// </summary>
		/// <returns> machine ID or <tt>-1</tt> if it is not specified before
		/// @pre $none
		/// @post $result >= -1 </returns>
		public virtual int MachineId
		{
			get
			{
				return machineId;
			}
		}

		/// <summary>
		/// Gets Pe ID.
		/// </summary>
		/// <returns> Pe ID or <tt>-1</tt> if it is not specified before
		/// @pre $none
		/// @post $result >= -1 </returns>
		public virtual int PeId
		{
			get
			{
				return peId;
			}
		}

		/// <summary>
		/// Gets a list of Pe IDs. <br>
		/// NOTE: To get the machine IDs corresponding to these Pe IDs, use <seealso cref="#getMachineIdList()"/>.
		/// </summary>
		/// <returns> an array containing Pe IDs.
		/// @pre $none
		/// @post $none </returns>
		public virtual int[] PeIdList
		{
			get
			{
				return peArrayId;
			}
		}

		/// <summary>
		/// Gets a list of Machine IDs. <br>
		/// NOTE: To get the Pe IDs corresponding to these machine IDs, use <seealso cref="#getPeIdList()"/>.
		/// </summary>
		/// <returns> an array containing Machine IDs.
		/// @pre $none
		/// @post $none </returns>
		public virtual int[] MachineIdList
		{
			get
			{
				return machineArrayId;
			}
		}

		/// <summary>
		/// Gets the remaining cloudlet length that has to be execute yet,
		/// considering the <seealso cref="#getCloudletTotalLength()"/>.
		/// </summary>
		/// <returns> cloudlet length
		/// @pre $none
		/// @post $result >= 0 </returns>
		public virtual long RemainingCloudletLength
		{
			get
			{
				long length = cloudlet.CloudletTotalLength * Consts.MILLION - cloudletFinishedSoFar;
    
				// Remaining Cloudlet length can't be negative number.
				if (length < 0)
				{
					return 0;
				}
    
				return (long) Math.Floor((double)(length / Consts.MILLION));
			}
		}

		/// <summary>
		/// Finalizes all relevant information before <tt>exiting</tt> the CloudResource entity. This
		/// method sets the final data of:
		/// <ul>
		/// <li>wall clock time, i.e. the time of this Cloudlet resides in a CloudResource (from arrival
		/// time until departure time).
		/// <li>actual CPU time, i.e. the total execution time of this Cloudlet in a CloudResource.
		/// <li>Cloudlet's finished time so far
		/// </ul>
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public virtual void finalizeCloudlet()
		{
			// Sets the wall clock time and actual CPU time
			double wallClockTime = CloudSim.clock() - arrivalTime;
			cloudlet.setExecParam(wallClockTime, totalCompletionTime);

			long finished = 0;
			//if (cloudlet.getCloudletTotalLength() * Consts.MILLION < cloudletFinishedSoFar) {
			if (cloudlet.CloudletStatus == Cloudlet.SUCCESS)
			{
				finished = cloudlet.CloudletLength;
			}
			else
			{
				finished = cloudletFinishedSoFar / Consts.MILLION;
			}

			cloudlet.CloudletFinishedSoFar = finished;
		}

		/// <summary>
		/// Updates the length of cloudlet that has already been completed.
		/// </summary>
		/// <param name="miLength"> cloudlet length in Instructions (I)
		/// @pre miLength >= 0.0
		/// @post $none </param>
		public virtual void updateCloudletFinishedSoFar(long miLength)
		{
			cloudletFinishedSoFar += miLength;
		}

		/// <summary>
		/// Gets arrival time of a cloudlet.
		/// </summary>
		/// <returns> arrival time
		/// @pre $none
		/// @post $result >= 0.0
		/// 
		/// @todo It is being used different words for the same term.
		/// Here it is used arrival time while at Resource inner classe of the Cloudlet class
		/// it is being used submissionTime. It needs to be checked if they are 
		/// the same term or different ones in fact. </returns>
		public virtual double CloudletArrivalTime
		{
			get
			{
				return arrivalTime;
			}
		}

		/// <summary>
		/// Sets the finish time for this Cloudlet. If time is negative, then it is being ignored.
		/// </summary>
		/// <param name="time"> finish time
		/// @pre time >= 0.0
		/// @post $none </param>
		public virtual double FinishTime
		{
			set
			{
				if (value < 0.0)
				{
					return;
				}
    
				finishedTime = value;
			}
		}

		/// <summary>
		/// Gets the Cloudlet's finish time.
		/// </summary>
		/// <returns> finish time of a cloudlet or <tt>-1.0</tt> if it cannot finish in this hourly slot
		/// @pre $none
		/// @post $result >= -1.0 </returns>
		public virtual double ClouddletFinishTime
		{
			get
			{
				return finishedTime;
			}
		}

		/// <summary>
		/// Gets the related Cloudlet object.
		/// </summary>
		/// <returns> cloudlet object
		/// @pre $none
		/// @post $result != null </returns>
		public virtual Cloudlet Cloudlet
		{
			get
			{
				return cloudlet;
			}
		}

		/// <summary>
		/// Gets the Cloudlet status.
		/// </summary>
		/// <returns> Cloudlet status
		/// @pre $none
		/// @post $none </returns>
		public virtual int CloudletStatus
		{
			get
			{
				return cloudlet.CloudletStatus;
			}

            // TEST: (fixed) Had to add this.
            set
            {
                cloudlet.CloudletStatus = value;
            }
		}

		/// <summary>
		/// Get am Unique Identifier (UID) of the cloudlet.
		/// </summary>
		/// <returns> The UID </returns>
		public virtual string Uid
		{
			get
			{
				return UserId + "-" + CloudletId;
			}
		}

	}

}