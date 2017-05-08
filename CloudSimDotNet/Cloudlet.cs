using System;
using System.Collections.Generic;
using System.Text;

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
	/// Cloudlet is an extension to the cloudlet. It stores, despite all the
	/// information encapsulated in the Cloudlet, the ID of the VM running it.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// @todo The documentation is wrong. Cloudlet isn't extending any class.
	/// </summary>
	public class Cloudlet
	{

		/// <summary>
		/// The cloudlet ID.
		/// </summary>
		private readonly int cloudletId;

		/// <summary>
		/// The User or Broker ID. It is advisable that broker set this ID with its
		/// own ID, so that CloudResource returns to it after the execution.
		/// 
		/// </summary>
		private int userId;

		/// <summary>
		/// The execution length of this Cloudlet (Unit: in Million Instructions
		/// (MI)). According to this length and the power of the processor (in
		/// Million Instruction Per Second - MIPS) where the cloudlet will be run,
		/// the cloudlet will take a given time to finish processing. For instance,
		/// for a cloudlet of 10000 MI running on a processor of 2000 MIPS, the
		/// cloudlet will spend 5 seconds using the processor in order to be
		/// completed (that may be uninterrupted or not, depending on the scheduling
		/// policy).
		/// </summary>
		/// <seealso cref= #setNumberOfPes(int)
		///  </seealso>
		private long cloudletLength;

		/// <summary>
		/// The input file size of this Cloudlet before execution (unit: in byte).
		/// This size has to be considered the program + input data sizes.
		/// </summary>
		private readonly long cloudletFileSize;

		/// <summary>
		/// The output file size of this Cloudlet after execution (unit: in byte).
		/// 
		/// @todo See
		/// <a href="https://groups.google.com/forum/#!topic/cloudsim/MyZ7OnrXuuI">this
		/// discussion</a>
		/// </summary>
		private readonly long cloudletOutputSize;

		/// <summary>
		/// The number of Processing Elements (Pe) required to execute this cloudlet
		/// (job).
		/// </summary>
		/// <seealso cref= #setNumberOfPes(int) </seealso>
		private int numberOfPes;

		/// <summary>
		/// The execution status of this Cloudlet.
		/// 
		/// @todo It would be an enum, to avoid using int constants.
		/// </summary>
		private int status;

		/// <summary>
		/// The execution start time of this Cloudlet. With new functionalities, such
		/// as CANCEL, PAUSED and RESUMED, this attribute only stores the latest
		/// execution time. Previous execution time are ignored.
		/// </summary>
		private double execStartTime;

		/// <summary>
		/// The time where this Cloudlet completes.
		/// </summary>
		private double finishTime;

		/// <summary>
		/// The ID of a reservation made for this cloudlet.
		/// 
		/// @todo This attribute doesn't appear to be used
		/// </summary>
		private int reservationId = -1;

		/// <summary>
		/// Indicates if transaction history records for this Cloudlet is to be
		/// outputted.
		/// </summary>
		private readonly bool record;

		/// <summary>
		/// Stores the operating system line separator.
		/// </summary>
		private string newline;

		/// <summary>
		/// The cloudlet transaction history.
		/// </summary>
		private StringBuilder history;

		/// <summary>
		/// The list of every resource where the cloudlet has been executed. In case
		/// it starts and finishes executing in a single cloud resource, without
		/// being migrated, this list will have only one item.
		/// </summary>
		private readonly IList<Resource> resList;

		/// <summary>
		/// The index of the last resource where the cloudlet was executed. If the
		/// cloudlet is migrated during its execution, this index is updated. The
		/// value -1 indicates the cloudlet has not been executed
		///     yet.
		/// </summary>
		private int index;

		/// <summary>
		/// The classType or priority of this Cloudlet for scheduling on a resource.
		/// </summary>
		private int classType;

		/// <summary>
		/// The Type of Service (ToS) of IPv4 for sending Cloudlet over the network.
		/// </summary>
		private int netToS;

		/// <summary>
		/// The format of decimal numbers.
		/// </summary>
		//private DecimalFormat num;

		// //////////////////////////////////////////
		// Below are CONSTANTS attributes
		/// <summary>
		/// The Cloudlet has been created and added to the CloudletList object.
		/// </summary>
		public const int CREATED = 0;

		/// <summary>
		/// The Cloudlet has been assigned to a CloudResource object to be executed
		/// as planned.
		/// </summary>
		public const int READY = 1;

		/// <summary>
		/// The Cloudlet has moved to a Cloud node.
		/// </summary>
		public const int QUEUED = 2;

		/// <summary>
		/// The Cloudlet is in execution in a Cloud node.
		/// </summary>
		public const int INEXEC = 3;

		/// <summary>
		/// The Cloudlet has been executed successfully.
		/// </summary>
		public const int SUCCESS = 4;

		/// <summary>
		/// The Cloudlet has failed.
		/// </summary>
		public const int FAILED = 5;

		/// <summary>
		/// The Cloudlet has been canceled.
		/// </summary>
		public const int CANCELED = 6;

		/// <summary>
		/// The Cloudlet has been paused. It can be resumed by changing the status
		/// into <tt>RESUMED</tt>.
		/// </summary>
		public const int PAUSED = 7;

		/// <summary>
		/// The Cloudlet has been resumed from <tt>PAUSED</tt> state.
		/// </summary>
		public const int RESUMED = 8;

		/// <summary>
		/// The cloudlet has failed due to a resource failure.
		/// </summary>
		public const int FAILED_RESOURCE_UNAVAILABLE = 9;

		/// <summary>
		/// The id of the vm that is planned to execute the cloudlet.
		/// </summary>
		protected internal int vmId;

		/// <summary>
		/// The cost of each byte of bandwidth (bw) consumed.
		/// </summary>
		protected internal double costPerBw;

		/// <summary>
		/// The total bandwidth (bw) cost for transferring the cloudlet by the
		/// network, according to the <seealso cref="#cloudletFileSize"/>.
		/// </summary>
		protected internal double accumulatedBwCost;

		// Utilization
		/// <summary>
		/// The utilization model that defines how the cloudlet will use the VM's
		/// CPU.
		/// </summary>
		private UtilizationModel utilizationModelCpu;

		/// <summary>
		/// The utilization model that defines how the cloudlet will use the VM's
		/// RAM.
		/// </summary>
		private UtilizationModel utilizationModelRam;

		/// <summary>
		/// The utilization model that defines how the cloudlet will use the VM's
		/// bandwidth (bw).
		/// </summary>
		private UtilizationModel utilizationModelBw;

		// Data cloudlet
		/// <summary>
		/// The required files to be used by the cloudlet (if any). The time to
		/// transfer these files by the network is considered when placing the
		/// cloudlet inside a given VM
		/// </summary>
		private IList<string> requiredFiles = null;

		/// <summary>
		/// Allocates a new Cloudlet object. The Cloudlet length, input and output
		/// file sizes should be greater than or equal to 1. By default this
		/// constructor sets the history of this object.
		/// </summary>
		/// <param name="cloudletId"> the unique ID of this Cloudlet </param>
		/// <param name="cloudletLength"> the length or size (in MI) of this cloudlet to be
		/// executed in a PowerDatacenter </param>
		/// <param name="cloudletFileSize"> the file size (in byte) of this cloudlet
		/// <tt>BEFORE</tt> submitting to a Datacenter </param>
		/// <param name="cloudletOutputSize"> the file size (in byte) of this cloudlet
		/// <tt>AFTER</tt> finish executing by a Datacenter </param>
		/// <param name="pesNumber"> the pes number </param>
		/// <param name="utilizationModelCpu"> the utilization model of cpu </param>
		/// <param name="utilizationModelRam"> the utilization model of ram </param>
		/// <param name="utilizationModelBw"> the utilization model of bw
		/// 
		/// @pre cloudletID >= 0
		/// @pre cloudletLength >= 0.0
		/// @pre cloudletFileSize >= 1
		/// @pre cloudletOutputSize >= 1
		/// @post $none </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Cloudlet(final int cloudletId, final long cloudletLength, final int pesNumber, final long cloudletFileSize, final long cloudletOutputSize, final UtilizationModel utilizationModelCpu, final UtilizationModel utilizationModelRam, final UtilizationModel utilizationModelBw)
		public Cloudlet(int cloudletId, long cloudletLength, int pesNumber, long cloudletFileSize, long cloudletOutputSize, UtilizationModel utilizationModelCpu, UtilizationModel utilizationModelRam, UtilizationModel utilizationModelBw) : this(cloudletId, cloudletLength, pesNumber, cloudletFileSize, cloudletOutputSize, utilizationModelCpu, utilizationModelRam, utilizationModelBw, false)
		{
			vmId = -1;
			accumulatedBwCost = 0;
			costPerBw = 0;
			requiredFiles = new List<string>();
		}

		/// <summary>
		/// Allocates a new Cloudlet object. The Cloudlet length, input and output
		/// file sizes should be greater than or equal to 1.
		/// </summary>
		/// <param name="cloudletId"> the unique ID of this cloudlet </param>
		/// <param name="cloudletLength"> the length or size (in MI) of this cloudlet to be
		/// executed in a PowerDatacenter </param>
		/// <param name="cloudletFileSize"> the file size (in byte) of this cloudlet
		/// <tt>BEFORE</tt> submitting to a PowerDatacenter </param>
		/// <param name="cloudletOutputSize"> the file size (in byte) of this cloudlet
		/// <tt>AFTER</tt> finish executing by a PowerDatacenter </param>
		/// <param name="record"> record the history of this object or not </param>
		/// <param name="fileList"> list of files required by this cloudlet </param>
		/// <param name="pesNumber"> the pes number </param>
		/// <param name="utilizationModelCpu"> the utilization model of cpu </param>
		/// <param name="utilizationModelRam"> the utilization model of ram </param>
		/// <param name="utilizationModelBw"> the utilization model of bw
		/// 
		/// @pre cloudletID >= 0
		/// @pre cloudletLength >= 0.0
		/// @pre cloudletFileSize >= 1
		/// @pre cloudletOutputSize >= 1
		/// @post $none </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Cloudlet(final int cloudletId, final long cloudletLength, final int pesNumber, final long cloudletFileSize, final long cloudletOutputSize, final UtilizationModel utilizationModelCpu, final UtilizationModel utilizationModelRam, final UtilizationModel utilizationModelBw, final boolean record, final java.util.List<String> fileList)
		public Cloudlet(int cloudletId, long cloudletLength, int pesNumber, long cloudletFileSize, long cloudletOutputSize, UtilizationModel utilizationModelCpu, UtilizationModel utilizationModelRam, UtilizationModel utilizationModelBw, bool record, IList<string> fileList) : this(cloudletId, cloudletLength, pesNumber, cloudletFileSize, cloudletOutputSize, utilizationModelCpu, utilizationModelRam, utilizationModelBw, record)
		{
			vmId = -1;
			accumulatedBwCost = 0.0;
			costPerBw = 0.0;

			requiredFiles = fileList;
		}

		/// <summary>
		/// Allocates a new Cloudlet object. The Cloudlet length, input and output
		/// file sizes should be greater than or equal to 1. By default this
		/// constructor sets the history of this object.
		/// </summary>
		/// <param name="cloudletId"> the unique ID of this Cloudlet </param>
		/// <param name="cloudletLength"> the length or size (in MI) of this cloudlet to be
		/// executed in a PowerDatacenter </param>
		/// <param name="cloudletFileSize"> the file size (in byte) of this cloudlet
		/// <tt>BEFORE</tt> submitting to a PowerDatacenter </param>
		/// <param name="cloudletOutputSize"> the file size (in byte) of this cloudlet
		/// <tt>AFTER</tt> finish executing by a PowerDatacenter </param>
		/// <param name="fileList"> list of files required by this cloudlet </param>
		/// <param name="pesNumber"> the pes number </param>
		/// <param name="utilizationModelCpu"> the utilization model of cpu </param>
		/// <param name="utilizationModelRam"> the utilization model of ram </param>
		/// <param name="utilizationModelBw"> the utilization model of bw
		/// 
		/// @pre cloudletID >= 0
		/// @pre cloudletLength >= 0.0
		/// @pre cloudletFileSize >= 1
		/// @pre cloudletOutputSize >= 1
		/// @post $none </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Cloudlet(final int cloudletId, final long cloudletLength, final int pesNumber, final long cloudletFileSize, final long cloudletOutputSize, final UtilizationModel utilizationModelCpu, final UtilizationModel utilizationModelRam, final UtilizationModel utilizationModelBw, final java.util.List<String> fileList)
		public Cloudlet(int cloudletId, long cloudletLength, int pesNumber, long cloudletFileSize, long cloudletOutputSize, UtilizationModel utilizationModelCpu, UtilizationModel utilizationModelRam, UtilizationModel utilizationModelBw, IList<string> fileList) : this(cloudletId, cloudletLength, pesNumber, cloudletFileSize, cloudletOutputSize, utilizationModelCpu, utilizationModelRam, utilizationModelBw, false)
		{
			vmId = -1;
			accumulatedBwCost = 0.0;
			costPerBw = 0.0;

			requiredFiles = fileList;
		}

		/// <summary>
		/// Allocates a new Cloudlet object. The Cloudlet length, input and output
		/// file sizes should be greater than or equal to 1.
		/// </summary>
		/// <param name="cloudletId"> the unique ID of this cloudlet </param>
		/// <param name="cloudletLength"> the length or size (in MI) of this cloudlet to be
		/// executed in a PowerDatacenter </param>
		/// <param name="cloudletFileSize"> the file size (in byte) of this cloudlet
		/// <tt>BEFORE</tt> submitting to a PowerDatacenter </param>
		/// <param name="cloudletOutputSize"> the file size (in byte) of this cloudlet
		/// <tt>AFTER</tt> finish executing by a PowerDatacenter </param>
		/// <param name="record"> record the history of this object or not </param>
		/// <param name="pesNumber"> the pes number </param>
		/// <param name="utilizationModelCpu"> the utilization model of cpu </param>
		/// <param name="utilizationModelRam"> the utilization model of ram </param>
		/// <param name="utilizationModelBw"> the utilization model of bw
		/// 
		/// @pre cloudletID >= 0
		/// @pre cloudletLength >= 0.0
		/// @pre cloudletFileSize >= 1
		/// @pre cloudletOutputSize >= 1
		/// @post $none </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Cloudlet(final int cloudletId, final long cloudletLength, final int pesNumber, final long cloudletFileSize, final long cloudletOutputSize, final UtilizationModel utilizationModelCpu, final UtilizationModel utilizationModelRam, final UtilizationModel utilizationModelBw, final boolean record)
		public Cloudlet(int cloudletId, long cloudletLength, int pesNumber, long cloudletFileSize, long cloudletOutputSize, UtilizationModel utilizationModelCpu, UtilizationModel utilizationModelRam, UtilizationModel utilizationModelBw, bool record)
		{
			userId = -1; // to be set by a Broker or user
			status = CREATED;
			this.cloudletId = cloudletId;
			numberOfPes = pesNumber;
			execStartTime = 0.0;
			finishTime = -1.0; // meaning this Cloudlet hasn't finished yet
			classType = 0;
			netToS = 0;

			// Cloudlet length, Input and Output size should be at least 1 byte.
			this.cloudletLength = Math.Max(1, cloudletLength);
			this.cloudletFileSize = Math.Max(1, cloudletFileSize);
			this.cloudletOutputSize = Math.Max(1, cloudletOutputSize);

			// Normally, a Cloudlet is only executed on a resource without being
			// migrated to others. Hence, to reduce memory consumption, set the
			// size of this ArrayList to be less than the default one.
			resList = new List<Resource>(2);
			index = -1;
			this.record = record;

			vmId = -1;
			accumulatedBwCost = 0.0;
			costPerBw = 0.0;

			requiredFiles = new List<string>();

			UtilizationModelCpu = utilizationModelCpu;
			UtilizationModelRam = utilizationModelRam;
			UtilizationModelBw = utilizationModelBw;
		}

		// ////////////////////// INTERNAL CLASS ///////////////////////////////////
		/// <summary>
		/// Internal class that keeps track of Cloudlet's movement in different
		/// CloudResources. Each time a cloudlet is run on a given VM, the cloudlet's
		/// execution history on each VM is registered at <seealso cref="Cloudlet#resList"/>
		/// </summary>
        // TODO: Protected vs internal?
		protected class Resource
		{

			/// <summary>
			/// Cloudlet's submission (arrival) time to a CloudResource.
			/// </summary>
			public double submissionTime = 0.0;

			/// <summary>
			/// The time this Cloudlet resides in a CloudResource (from arrival time
			/// until departure time, that may include waiting time).
			/// </summary>
			public double wallClockTime = 0.0;

			/// <summary>
			/// The total time the Cloudlet spent being executed in a CloudResource.
			/// </summary>
			public double actualCPUTime = 0.0;

			/// <summary>
			/// Cost per second a CloudResource charge to execute this Cloudlet.
			/// </summary>
			public double costPerSec = 0.0;

			/// <summary>
			/// Cloudlet's length finished so far.
			/// </summary>
			public long finishedSoFar = 0;

			/// <summary>
			/// a CloudResource id.
			/// </summary>
			public int resourceId = -1;

			/// <summary>
			/// a CloudResource name.
			/// </summary>
			public string resourceName = null;

		}

		// ////////////////////// End of Internal Class //////////////////////////
		/// <summary>
		/// Sets the id of the reservation made for this cloudlet.
		/// </summary>
		/// <param name="resId"> the reservation ID </param>
		/// <returns> <tt>true</tt> if the ID has successfully been set or
		/// <tt>false</tt> otherwise. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean setReservationId(final int resId)
		public virtual bool setReservationId(int resId)
		{
			if (resId <= 0)
			{
				return false;
			}
			reservationId = resId;
			return true;
		}

		/// <summary>
		/// Gets the reservation ID that owns this Cloudlet.
		/// </summary>
		/// <returns> a reservation ID
		/// @pre $none
		/// @post $none </returns>
		public virtual int ReservationId
		{
			get
			{
				return reservationId;
			}
		}

		/// <summary>
		/// Checks whether this Cloudlet is submitted by reserving or not.
		/// </summary>
		/// <returns> <tt>true</tt> if this Cloudlet has reserved before,
		/// <tt>false</tt> otherwise </returns>
		public virtual bool hasReserved()
		{
			if (reservationId == -1)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Sets the length or size (in MI) of this Cloudlet to be executed in a
		/// CloudResource. It has to be the length for each individual Pe,
		/// <tt>not</tt> the total length (the sum of length to be executed by each
		/// Pe).
		/// </summary>
		/// <param name="cloudletLength"> the length or size (in MI) of this Cloudlet to be
		/// executed in a CloudResource </param>
		/// <returns> <tt>true</tt> if it is successful, <tt>false</tt> otherwise
		/// </returns>
		/// <seealso cref= #getCloudletTotalLength() }
		/// @pre cloudletLength > 0
		/// @post $none </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean setCloudletLength(final long cloudletLength)
		public virtual bool setCloudletLength(long cloudletLength)
		{
			if (cloudletLength <= 0)
			{
				return false;
			}

			this.cloudletLength = cloudletLength;
			return true;
		}

		/// <summary>
		/// Sets the network service level (ToS) for sending this cloudlet over a
		/// network.
		/// </summary>
		/// <param name="netServiceLevel"> determines the type of service (ToS) this cloudlet
		/// receives in the network (applicable to selected PacketScheduler class
		/// only) </param>
		/// <returns> <code>true</code> if successful.
		/// @pre netServiceLevel >= 0
		/// @post $none
		/// 
		/// @todo The name of the setter is inconsistent with the attribute name,
		/// what might be misinterpreted by other developers. </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean setNetServiceLevel(final int netServiceLevel)
		public virtual bool setNetServiceLevel(int netServiceLevel)
		{
			bool success = false;
			if (netServiceLevel > 0)
			{
				netToS = netServiceLevel;
				success = true;
			}

			return success;
		}

		/// <summary>
		/// Gets the network service level (ToS) for sending this cloudlet over a
		/// network.
		/// </summary>
		/// <returns> the network service level
		/// @pre $none
		/// @post $none
		/// @todo The name of the getter is inconsistent with the attribute name,
		/// what might be misinterpreted by other developers. </returns>
		public virtual int NetServiceLevel
		{
			get
			{
				return netToS;
			}
		}

		/// <summary>
		/// Gets the time the cloudlet had to wait before start executing on a
		/// resource.
		/// </summary>
		/// <returns> the waiting time
		/// @pre $none
		/// @post $none </returns>
		public virtual double WaitingTime
		{
			get
			{
				if (index == -1)
				{
					return 0;
				}
    
				// use the latest resource submission time
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final double subTime = resList.get(index).submissionTime;
				double subTime = resList[index].submissionTime;
				return execStartTime - subTime;
			}
		}

		/// <summary>
		/// Sets the classType or priority of this Cloudlet for scheduling on a
		/// resource.
		/// </summary>
		/// <param name="classType"> classType of this Cloudlet </param>
		/// <returns> <tt>true</tt> if it is successful, <tt>false</tt> otherwise
		/// 
		/// @pre classType > 0
		/// @post $none </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean setClassType(final int classType)
		public virtual bool setClassType(int classType)
		{
			bool success = false;
			if (classType > 0)
			{
				this.classType = classType;
				success = true;
			}

			return success;
		}

		/// <summary>
		/// Gets the classtype or priority of this Cloudlet for scheduling on a
		/// resource.
		/// </summary>
		/// <returns> classtype of this cloudlet
		/// @pre $none
		/// @post $none </returns>
		public virtual int ClassType
		{
			get
			{
				return classType;
			}
		}

		/// <summary>
		/// Sets the number of PEs required to run this Cloudlet. <br>
		/// NOTE: The Cloudlet length is computed only for 1 Pe for simplicity. <br>
		/// For example, consider a Cloudlet that has a length of 500 MI and requires
		/// 2 PEs. This means each Pe will execute 500 MI of this Cloudlet.
		/// </summary>
		/// <param name="numberOfPes"> number of Pe </param>
		/// <returns> <tt>true</tt> if it is successful, <tt>false</tt> otherwise
		/// 
		/// @pre numPE > 0
		/// @post $none </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public boolean setNumberOfPes(final int numberOfPes)
		public virtual bool setNumberOfPes(int numberOfPes)
		{
			if (numberOfPes > 0)
			{
				this.numberOfPes = numberOfPes;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets the number of PEs required to run this Cloudlet.
		/// </summary>
		/// <returns> number of PEs
		/// 
		/// @pre $none
		/// @post $none </returns>
		public virtual int NumberOfPes
		{
			get
			{
				return numberOfPes;
			}
		}

		/// <summary>
		/// Gets the transaction history of this Cloudlet. The layout of this history
		/// is in a readable table column with <tt>time</tt> and <tt>description</tt>
		/// as headers.
		/// </summary>
		/// <returns> a String containing the history of this Cloudlet object.
		/// @pre $none
		/// @post $result != null </returns>
		public virtual string CloudletHistory
		{
			get
			{
				string msg = null;
				if (history == null)
				{
					msg = "No history is recorded for Cloudlet #" + cloudletId;
				}
				else
				{
					msg = history.ToString();
				}
    
				return msg;
			}
		}

		/// <summary>
		/// Gets the length of this Cloudlet that has been executed so far from the
		/// latest CloudResource. This method is useful when trying to move this
		/// Cloudlet into different CloudResources or to cancel it.
		/// </summary>
		/// <returns> the length of a partially executed Cloudlet or the full Cloudlet
		/// length if it is completed
		/// @pre $none
		/// @post $result >= 0.0 </returns>
		public virtual long CloudletFinishedSoFar
		{
			get
			{
				if (index == -1)
				{
					return cloudletLength;
				}
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final long finish = resList.get(index).finishedSoFar;
				long finish = resList[index].finishedSoFar;
				if (finish > cloudletLength)
				{
					return cloudletLength;
				}
    
				return finish;
			}
			set
			{
				// if value is -ve then ignore
				if (value < 0.0 || index < 0)
				{
					return;
				}
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Resource res = resList.get(index);
				Resource res = resList[index];
				res.finishedSoFar = value;
    
				if (record)
				{
					write("Sets the length's finished so far to " + value);
				}
			}
		}

		/// <summary>
		/// Checks whether this Cloudlet has finished execution or not.
		/// </summary>
		/// <returns> <tt>true</tt> if this Cloudlet has finished execution,
		/// <tt>false</tt> otherwise
		/// @pre $none
		/// @post $none </returns>
		public virtual bool Finished
		{
			get
			{
				if (index == -1)
				{
					return false;
				}
    
				bool completed = false;
    
				// if result is 0 or -ve then this Cloudlet has finished
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final long finish = resList.get(index).finishedSoFar;
				long finish = resList[index].finishedSoFar;
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final long result = cloudletLength - finish;
				long result = cloudletLength - finish;
				if (result <= 0.0)
				{
					completed = true;
				}
				return completed;
			}
		}


		/// <summary>
		/// Sets the user or owner ID of this Cloudlet. It is <tt>VERY</tt> important
		/// to set the user ID, otherwise this Cloudlet will not be executed in a
		/// CloudResource.
		/// </summary>
		/// <param name="id"> the user ID
		/// @pre id >= 0
		/// @post $none </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setUserId(final int id)
		public virtual int UserId
		{
			set
			{
				userId = value;
				if (record)
				{
					write("Assigns the Cloudlet to " + CloudSim.getEntityName(value) + " (ID #" + value + ")");
				}
			}
			get
			{
				return userId;
			}
		}


		/// <summary>
		/// Gets the latest resource ID that processes this Cloudlet.
		/// </summary>
		/// <returns> the resource ID or <tt>-1</tt> if none
		/// @pre $none
		/// @post $result >= -1 </returns>
		public virtual int ResourceId
		{
			get
			{
				if (index == -1)
				{
					return -1;
				}
				return resList[index].resourceId;
			}
		}

		/// <summary>
		/// Gets the input file size of this Cloudlet <tt>BEFORE</tt> submitting to a
		/// CloudResource.
		/// </summary>
		/// <returns> the input file size of this Cloudlet
		/// @pre $none
		/// @post $result >= 1 </returns>
		public virtual long CloudletFileSize
		{
			get
			{
				return cloudletFileSize;
			}
		}

		/// <summary>
		/// Gets the output size of this Cloudlet <tt>AFTER</tt> submitting and
		/// executing to a CloudResource.
		/// </summary>
		/// <returns> the Cloudlet output file size
		/// @pre $none
		/// @post $result >= 1 </returns>
		public virtual long CloudletOutputSize
		{
			get
			{
				return cloudletOutputSize;
			}
		}

		/// <summary>
		/// Sets the resource parameters for which the Cloudlet is going to be
		/// executed. From the second time this method is called, every call make the
		/// cloudlet to be migrated to the indicated resource.<br>
		/// 
		/// NOTE: This method <tt>should</tt> be called only by a resource entity,
		/// not the user or owner of this Cloudlet.
		/// </summary>
		/// <param name="resourceID"> the CloudResource ID </param>
		/// <param name="cost"> the cost running this CloudResource per second
		/// 
		/// @pre resourceID >= 0
		/// @pre cost > 0.0
		/// @post $none </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setResourceParameter(final int resourceID, final double cost)
		public virtual void setResourceParameter(int resourceID, double cost)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Resource res = new Resource();
			Resource res = new Resource();
			res.resourceId = resourceID;
			res.costPerSec = cost;
			res.resourceName = CloudSim.getEntityName(resourceID);

			// add into a list if moving to a new grid resource
			resList.Add(res);

			if (index == -1 && record)
			{
				write("Allocates this Cloudlet to " + res.resourceName + " (ID #" + resourceID + ") with cost = $" + cost + "/sec");
			}
			else if (record)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int id = resList.get(index).resourceId;
				int id = resList[index].resourceId;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String name = resList.get(index).resourceName;
				string name = resList[index].resourceName;
				write("Moves Cloudlet from " + name + " (ID #" + id + ") to " + res.resourceName + " (ID #" + resourceID + ") with cost = $" + cost + "/sec");
			}

			index++; // initially, index = -1
		}

		/// <summary>
		/// Sets the submission (arrival) time of this Cloudlet into a CloudResource.
		/// </summary>
		/// <param name="clockTime"> the submission time
		/// @pre clockTime >= 0.0
		/// @post $none </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setSubmissionTime(final double clockTime)
		public virtual double SubmissionTime
		{
			set
			{
				if (value < 0.0 || index < 0)
				{
					return;
				}
    
	//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
	//ORIGINAL LINE: final Resource res = resList.get(index);
				Resource res = resList[index];
				res.submissionTime = value;
    
				if (record)
				{
                    //write("Sets the submission time to " + num.format(value));
                    write("Sets the submission time to " + value);
                }
			}
			get
			{
				if (index == -1)
				{
					return 0.0;
				}
				return resList[index].submissionTime;
			}
		}


		/// <summary>
		/// Sets the execution start time of this Cloudlet inside a CloudResource.
		/// <br/>
		/// <b>NOTE:</b> With new functionalities, such as being able to cancel / to
		/// pause / to resume this Cloudlet, the execution start time only holds the
		/// latest one. Meaning, all previous execution start time are ignored.
		/// </summary>
		/// <param name="clockTime"> the latest execution start time
		/// @pre clockTime >= 0.0
		/// @post $none </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setExecStartTime(final double clockTime)
		public virtual double ExecStartTime
		{
			set
			{
				execStartTime = value;
				if (record)
				{
                    //write("Sets the execution start time to " + num.format(value));
                    write("Sets the execution start time to " + value);
                }
			}
			get
			{
				return execStartTime;
			}
		}


		/// <summary>
		/// Sets the Cloudlet's execution parameters. These parameters are set by the
		/// CloudResource before departure or sending back to the original Cloudlet's
		/// owner.
		/// </summary>
		/// <param name="wallTime"> the time of this Cloudlet resides in a CloudResource
		/// (from arrival time until departure time). </param>
		/// <param name="actualTime"> the total execution time of this Cloudlet in a
		/// CloudResource.
		/// </param>
		/// <seealso cref= Resource#wallClockTime </seealso>
		/// <seealso cref= Resource#actualCPUTime
		/// 
		/// @pre wallTime >= 0.0
		/// @pre actualTime >= 0.0
		/// @post $none </seealso>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void setExecParam(final double wallTime, final double actualTime)
		public virtual void setExecParam(double wallTime, double actualTime)
		{
			if (wallTime < 0.0 || actualTime < 0.0 || index < 0)
			{
				return;
			}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Resource res = resList.get(index);
			Resource res = resList[index];
			res.wallClockTime = wallTime;
			res.actualCPUTime = actualTime;

			if (record)
			{
                //write("Sets the wall clock time to " + num.format(wallTime) + " and the actual CPU time to " + num.format(actualTime));
                write("Sets the wall clock time to " + wallTime + " and the actual CPU time to " + actualTime);
            }
		}

        /// <summary>
        /// Sets the execution status code of this Cloudlet.
        /// </summary>
        /// <param name="newStatus"> the status code of this Cloudlet </param>
        /// <exception cref="Exception"> Invalid range of Cloudlet status
        /// @pre newStatus >= 0 && newStatus <= 8
        /// @
        /// post $none
        /// 
        /// @todo It has to throw an specific (unckecked) exception </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public void setCloudletStatus(final int newStatus) throws Exception
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        public virtual int CloudletStatus
        {
            set
            {
                // if the new status is same as current one, then ignore the rest
                if (status == value)
                {
                    return;
                }

                // throws an exception if the new status is outside the range
                if (value < Cloudlet.CREATED || value > Cloudlet.FAILED_RESOURCE_UNAVAILABLE)
                {
                    throw new Exception("Cloudlet.setCloudletStatus() : Error - Invalid integer range for Cloudlet status.");
                }

                if (value == Cloudlet.SUCCESS)
                {
                    finishTime = CloudSim.clock();
                }

                if (record)
                {
                    write("Sets Cloudlet status from " + CloudletStatusString + " to " + Cloudlet.getStatusString(value));
                }

                status = value;
            }
            get
            {
                return status;
            }
        }


        /// <summary>
        /// Gets the string representation of the current Cloudlet status code.
        /// </summary>
        /// <returns> the Cloudlet status code as a string or <tt>null</tt> if the
        /// status code is unknown
        /// @pre $none
        /// @post $none </returns>
        public virtual string CloudletStatusString
        {
            get
            {
                return Cloudlet.getStatusString(status);
            }
        }

        /// <summary>
        /// Gets the string representation of the given Cloudlet status code.
        /// </summary>
        /// <param name="status"> the Cloudlet status code </param>
        /// <returns> the Cloudlet status code as a string or <tt>null</tt> if the
        /// status code is unknown
        /// @pre $none
        /// @post $none </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public static String getStatusString(final int status)
        public static string getStatusString(int status)
        {
            string statusString = null;
            switch (status)
            {
                case Cloudlet.CREATED:
                    statusString = "Created";
                    break;

                case Cloudlet.READY:
                    statusString = "Ready";
                    break;

                case Cloudlet.INEXEC:
                    statusString = "InExec";
                    break;

                case Cloudlet.SUCCESS:
                    statusString = "Success";
                    break;

                case Cloudlet.QUEUED:
                    statusString = "Queued";
                    break;

                case Cloudlet.FAILED:
                    statusString = "Failed";
                    break;

                case Cloudlet.CANCELED:
                    statusString = "Canceled";
                    break;

                case Cloudlet.PAUSED:
                    statusString = "Paused";
                    break;

                case Cloudlet.RESUMED:
                    statusString = "Resumed";
                    break;

                case Cloudlet.FAILED_RESOURCE_UNAVAILABLE:
                    statusString = "Failed_resource_unavailable";
                    break;

                default:
                    break;
            }

            return statusString;
        }

        /// <summary>
        /// Gets the length of this Cloudlet.
        /// </summary>
        /// <returns> the length of this Cloudlet
        /// @pre $none
        /// @post $result >= 0.0 </returns>
        public virtual long CloudletLength
        {
            get
            {
                return cloudletLength;
            }

            set
            {
                cloudletLength = value;
            }
        }

        /// <summary>
        /// Gets the total length (across all PEs) of this Cloudlet. It considers the
        /// <seealso cref="#cloudletLength"/> of the cloudlet to be executed in each Pe and the
        /// <seealso cref="#numberOfPes"/>.<br/>
        /// 
        /// For example, setting the cloudletLenght as 10000 MI and
        /// <seealso cref="#numberOfPes"/> to 4, each Pe will execute 10000 MI. Thus, the
        /// entire cloudlet has a total length of 40000 MI.
        /// 
        /// </summary>
        /// <returns> the total length of this Cloudlet
        /// </returns>
        /// <seealso cref= #setCloudletLength(long)
        /// @pre $none
        /// @post $result >= 0.0 </seealso>
        public virtual long CloudletTotalLength
        {
            get
            {
                return CloudletLength * NumberOfPes;
            }
        }

        /// <summary>
        /// Gets the cost/sec of running the Cloudlet in the latest CloudResource.
        /// </summary>
        /// <returns> the cost associated with running this Cloudlet or <tt>0.0</tt> if
        /// none
        /// @pre $none
        /// @post $result >= 0.0 </returns>
        public virtual double CostPerSec
        {
            get
            {
                if (index == -1)
                {
                    return 0.0;
                }
                return resList[index].costPerSec;
            }
        }

        /// <summary>
        /// Gets the time of this Cloudlet resides in the latest CloudResource (from
        /// arrival time until departure time).
        /// </summary>
        /// <returns> the time of this Cloudlet resides in a CloudResource
        /// @pre $none
        /// @post $result >= 0.0 </returns>
        public virtual double WallClockTime
        {
            get
            {
                if (index == -1)
                {
                    return 0.0;
                }
                return resList[index].wallClockTime;
            }
        }

        /// <summary>
        /// Gets all the CloudResource names that executed this Cloudlet.
        /// </summary>
        /// <returns> an array of CloudResource names or <tt>null</tt> if it has none
        /// @pre $none
        /// @post $none </returns>
        public virtual string[] AllResourceName
        {
            get
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int size = resList.size();
                int size = resList.Count;
                string[] data = null;

                if (size > 0)
                {
                    data = new string[size];
                    for (int i = 0; i < size; i++)
                    {
                        data[i] = resList[i].resourceName;
                    }
                }

                return data;
            }
        }

        /// <summary>
        /// Gets all the CloudResource IDs that executed this Cloudlet.
        /// </summary>
        /// <returns> an array of CloudResource IDs or <tt>null</tt> if it has none
        /// @pre $none
        /// @post $none </returns>
        public virtual int[] AllResourceId
        {
            get
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final int size = resList.size();
                int size = resList.Count;
                int[] data = null;

                if (size > 0)
                {
                    data = new int[size];
                    for (int i = 0; i < size; i++)
                    {
                        data[i] = resList[i].resourceId;
                    }
                }

                return data;
            }
        }

        /// <summary>
        /// Gets the total execution time of this Cloudlet in a given CloudResource
        /// ID.
        /// </summary>
        /// <param name="resId"> a CloudResource entity ID </param>
        /// <returns> the total execution time of this Cloudlet in a CloudResource or
        /// <tt>0.0</tt> if not found
        /// @pre resId >= 0
        /// @post $result >= 0.0 </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public double getActualCPUTime(final int resId)
        public virtual double getActualCPUTime(int resId)
        {
            Resource resource = getResourceById(resId);
            if (resource != null)
            {
                return resource.actualCPUTime;
            }
            return 0.0;
        }

        /// <summary>
        /// Gets the cost running this Cloudlet in a given CloudResource ID.
        /// </summary>
        /// <param name="resId"> a CloudResource entity ID </param>
        /// <returns> the cost associated with running this Cloudlet or <tt>0.0</tt> if
        /// not found
        /// @pre resId >= 0
        /// @post $result >= 0.0 </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public double getCostPerSec(final int resId)
        public virtual double getCostPerSec(int resId)
        {
            Resource resource = getResourceById(resId);
            if (resource != null)
            {
                return resource.costPerSec;
            }
            return 0.0;
        }

        /// <summary>
        /// Gets the length of this Cloudlet that has been executed so far in a given
        /// CloudResource ID. This method is useful when trying to move this Cloudlet
        /// into different CloudResources or to cancel it.
        /// </summary>
        /// <param name="resId"> a CloudResource entity ID </param>
        /// <returns> the length of a partially executed Cloudlet or the full Cloudlet
        /// length if it is completed or <tt>0.0</tt> if not found
        /// @pre resId >= 0
        /// @post $result >= 0.0 </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public long getCloudletFinishedSoFar(final int resId)
        public virtual long getCloudletFinishedSoFar(int resId)
        {
            Resource resource = getResourceById(resId);
            if (resource != null)
            {
                return resource.finishedSoFar;
            }
            return 0;
        }

        /// <summary>
        /// Gets the submission (arrival) time of this Cloudlet in the given
        /// CloudResource ID.
        /// </summary>
        /// <param name="resId"> a CloudResource entity ID </param>
        /// <returns> the submission time or <tt>0.0</tt> if not found
        /// @pre resId >= 0
        /// @post $result >= 0.0 </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public double getSubmissionTime(final int resId)
        public virtual double getSubmissionTime(int resId)
        {
            Resource resource = getResourceById(resId);
            if (resource != null)
            {
                return resource.submissionTime;
            }
            return 0.0;
        }

        /// <summary>
        /// Gets the time of this Cloudlet resides in a given CloudResource ID (from
        /// arrival time until departure time).
        /// </summary>
        /// <param name="resId"> a CloudResource entity ID </param>
        /// <returns> the time of this Cloudlet resides in the CloudResource or
        /// <tt>0.0</tt> if not found
        /// @pre resId >= 0
        /// @post $result >= 0.0 </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public double getWallClockTime(final int resId)
        public virtual double getWallClockTime(int resId)
        {
            Resource resource = getResourceById(resId);
            if (resource != null)
            {
                return resource.wallClockTime;
            }
            return 0.0;
        }

        /// <summary>
        /// Gets the CloudResource name based on its ID.
        /// </summary>
        /// <param name="resId"> a CloudResource entity ID </param>
        /// <returns> the CloudResource name or <tt>null</tt> if not found
        /// @pre resId >= 0
        /// @post $none </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public String getResourceName(final int resId)
        public virtual string getResourceName(int resId)
        {
            Resource resource = getResourceById(resId);
            if (resource != null)
            {
                return resource.resourceName;
            }
            return null;
        }

        /// <summary>
        /// Gets the resource by id.
        /// </summary>
        /// <param name="resourceId"> the resource id </param>
        /// <returns> the resource by id </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public Resource getResourceById(final int resourceId)
        protected virtual Resource getResourceById(int resourceId)
        {
            foreach (Resource resource in resList)
            {
                if (resource.resourceId == resourceId)
                {
                    return resource;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the finish time of this Cloudlet in a CloudResource.
        /// </summary>
        /// <returns> the finish or completion time of this Cloudlet or <tt>-1</tt> if
        /// not finished yet.
        /// @pre $none
        /// @post $result >= -1 </returns>
        public virtual double FinishTime
        {
            get
            {
                return finishTime;
            }
        }

        // //////////////////////// PROTECTED METHODS //////////////////////////////
        /// <summary>
        /// Writes this particular history transaction of this Cloudlet into a log.
        /// </summary>
        /// <param name="str"> a history transaction of this Cloudlet
        /// @pre str != null
        /// @post $none </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: protected void write(final String str)
        protected internal virtual void write(string str)
        {
            if (!record)
            {
                return;
            }

            //if (num == null || history == null)
            if (history == null)
            { // Creates the history or
              // transactions of this Cloudlet
              //newline = System.getProperty("line.separator");
                newline = Environment.NewLine;
                //num = new DecimalFormat("#0.00#"); // with 3 decimal spaces
                history = new StringBuilder(1000);
                history.Append("Time below denotes the simulation time.");
                history.Append(newline);
                //history.Append(System.getProperty("line.separator"));
                history.Append("Time (sec)       Description Cloudlet #" + cloudletId);
                history.Append(newline);
                //history.Append(System.getProperty("line.separator"));
                history.Append("------------------------------------------");
                history.Append(newline);
                //history.Append(System.getProperty("line.separator"));
                //history.Append(num.format(CloudSim.clock()));
                history.Append(CloudSim.clock().ToString());
                history.Append("   Creates Cloudlet ID #" + cloudletId);
                history.Append(newline);
                //history.Append(System.getProperty("line.separator"));
            }

            //history.Append(num.format(CloudSim.clock()));
            history.Append(CloudSim.clock());
            history.Append("   " + str + newline);
        }

        /// <summary>
        /// Get the status of the Cloudlet.
        /// </summary>
        /// <returns> status of the Cloudlet
        /// @pre $none
        /// @post $none
        ///  </returns>
        public virtual int Status
        {
            get
            {
                return status;
            }
        }

        /// <summary>
        /// Gets the ID of this Cloudlet.
        /// </summary>
        /// <returns> Cloudlet Id
        /// @pre $none
        /// @post $none </returns>
        public virtual int CloudletId
        {
            get
            {
                return cloudletId;
            }
        }

        /// <summary>
        /// Gets the ID of the VM that will run this Cloudlet.
        /// </summary>
        /// <returns> VM Id, -1 if the Cloudlet was not assigned to a VM
        /// @pre $none
        /// @post $none </returns>
        public virtual int VmId
        {
            get
            {
                return vmId;
            }
            set
            {
                this.vmId = value;
            }
        }


        /// <summary>
        /// Returns the execution time of the Cloudlet.
        /// </summary>
        /// <returns> time in which the Cloudlet was running
        /// @pre $none
        /// @post $none </returns>
        public virtual double ActualCPUTime
        {
            get
            {
                return FinishTime - ExecStartTime;
            }
        }

        /// <summary>
        /// Sets the resource parameters for which this Cloudlet is going to be
        /// executed. <br>
        /// NOTE: This method <tt>should</tt> be called only by a resource entity,
        /// not the user or owner of this Cloudlet.
        /// </summary>
        /// <param name="resourceID"> the CloudResource ID </param>
        /// <param name="costPerCPU"> the cost per second of running this Cloudlet </param>
        /// <param name="costPerBw"> the cost per byte of data transfer to the Datacenter
        /// 
        /// @pre resourceID >= 0
        /// @pre cost > 0.0
        /// @post $none </param>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public void setResourceParameter(final int resourceID, final double costPerCPU, final double costPerBw)
        public virtual void setResourceParameter(int resourceID, double costPerCPU, double costPerBw)
        {
            setResourceParameter(resourceID, costPerCPU);
            this.costPerBw = costPerBw;
            accumulatedBwCost = costPerBw * CloudletFileSize;
        }

        /// <summary>
        /// Gets the total cost of processing or executing this Cloudlet
        /// <tt>Processing Cost = input data transfer + processing cost + output
        /// transfer cost</tt> .
        /// </summary>
        /// <returns> the total cost of processing Cloudlet
        /// @pre $none
        /// @post $result >= 0.0 </returns>
        public virtual double ProcessingCost
        {
            get
            {
                // cloudlet cost: execution cost...
                // double cost = getProcessingCost();
                double cost = 0;
                // ...plus input data transfer cost...
                cost += accumulatedBwCost;
                // ...plus output cost
                cost += costPerBw * CloudletOutputSize;
                return cost;
            }
        }

        // Data cloudlet
        /// <summary>
        /// Gets the required files.
        /// </summary>
        /// <returns> the required files </returns>
        public virtual IList<string> RequiredFiles
        {
            get
            {
                return requiredFiles;
            }
            set
            {
                this.requiredFiles = value;
            }
        }


        /// <summary>
        /// Adds the required filename to the list.
        /// </summary>
        /// <param name="fileName"> the required filename </param>
        /// <returns> <tt>true</tt> if succesful, <tt>false</tt> otherwise </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public boolean addRequiredFile(final String fileName)
        public virtual bool addRequiredFile(string fileName)
        {
            // if the list is empty
            if (RequiredFiles == null)
            {
                RequiredFiles = new List<string>();
            }

            // then check whether filename already exists or not
            bool result = false;
            for (int i = 0; i < RequiredFiles.Count; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final String temp = getRequiredFiles().get(i);
                string temp = RequiredFiles[i];
                if (temp.Equals(fileName))
                {
                    result = true;
                    break;
                }
            }

            if (!result)
            {
                RequiredFiles.Add(fileName);
            }

            return result;
        }

        /// <summary>
        /// Deletes the given filename from the list.
        /// </summary>
        /// <param name="filename"> the given filename to be deleted </param>
        /// <returns> <tt>true</tt> if succesful, <tt>false</tt> otherwise </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public boolean deleteRequiredFile(final String filename)
        public virtual bool deleteRequiredFile(string filename)
        {
            bool result = false;
            if (RequiredFiles == null)
            {
                return result;
            }

            for (int i = 0; i < RequiredFiles.Count; i++)
            {
                //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                //ORIGINAL LINE: final String temp = getRequiredFiles().get(i);
                string temp = RequiredFiles[i];

                if (temp.Equals(filename))
                {
                    RequiredFiles.RemoveAt(i);
                    result = true;

                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Checks whether this cloudlet requires any files or not.
        /// </summary>
        /// <returns> <tt>true</tt> if required, <tt>false</tt> otherwise </returns>
        public virtual bool requiresFiles()
        {
            bool result = false;
            if (RequiredFiles != null && RequiredFiles.Count > 0)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Gets the utilization model of cpu.
        /// </summary>
        /// <returns> the utilization model cpu </returns>
        public virtual UtilizationModel UtilizationModelCpu
        {
            get
            {
                return utilizationModelCpu;
            }
            set
            {
                this.utilizationModelCpu = value;
            }
        }


        /// <summary>
        /// Gets the utilization model of ram.
        /// </summary>
        /// <returns> the utilization model of ram </returns>
        public virtual UtilizationModel UtilizationModelRam
        {
            get
            {
                return utilizationModelRam;
            }
            set
            {
                this.utilizationModelRam = value;
            }
        }


        /// <summary>
        /// Gets the utilization model of bw.
        /// </summary>
        /// <returns> the utilization model of bw </returns>
        public virtual UtilizationModel UtilizationModelBw
        {
            get
            {
                return utilizationModelBw;
            }
            set
            {
                this.utilizationModelBw = value;
            }
        }


        /// <summary>
        /// Gets the utilization percentage of cpu.
        /// </summary>
        /// <param name="time"> the time </param>
        /// <returns> the utilization of cpu </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public double getUtilizationOfCpu(final double time)
        public virtual double getUtilizationOfCpu(double time)
        {
            return UtilizationModelCpu.getUtilization(time);
        }

        /// <summary>
        /// Gets the utilization percentage of memory.
        /// </summary>
        /// <param name="time"> the time </param>
        /// <returns> the utilization of memory </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public double getUtilizationOfRam(final double time)
        public virtual double getUtilizationOfRam(double time)
        {
            return UtilizationModelRam.getUtilization(time);
        }

        /// <summary>
        /// Gets the utilization percentage of bw.
        /// </summary>
        /// <param name="time"> the time </param>
        /// <returns> the utilization of bw </returns>
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        //ORIGINAL LINE: public double getUtilizationOfBw(final double time)
        public virtual double getUtilizationOfBw(double time)
        {
            return UtilizationModelBw.getUtilization(time);
        }

    }

}