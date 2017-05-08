/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.core
{

	/// <summary>
	/// Contains various static command tags that indicate a type of action that needs to be undertaken
	/// by CloudSim entities when they receive or send events. <b>NOTE:</b> To avoid conflicts with other
	/// tags, CloudSim reserves negative numbers, 0 - 299, and 9600.
	/// 
	/// @todo There aren't negative reserved tags, but only positive tags (with 2 exceptions).
	/// 
	/// @author Manzur Murshed
	/// @author Rajkumar Buyya
	/// @author Anthony Sulistio
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class CloudSimTags
	{

		/// <summary>
		/// Starting constant value for cloud-related tags. * </summary>
		private const int BASE = 0;

		/// <summary>
		/// Starting constant value for network-related tags. * </summary>
		private const int NETBASE = 100;

		/// <summary>
		/// Denotes boolean <tt>true</tt> in <tt>int</tt> value. </summary>
		public const int TRUE = 1;

		/// <summary>
		/// Denotes boolean <tt>false</tt> in <tt>int</tt> value. </summary>
		public const int FALSE = 0;

		/// <summary>
		/// Denotes the default baud rate for CloudSim entities. </summary>
		public const int DEFAULT_BAUD_RATE = 9600;

		/// <summary>
		/// Schedules an entity without any delay. </summary>
		public const double SCHEDULE_NOW = 0.0;

		/// <summary>
		/// Denotes the end of simulation. </summary>
		public const int END_OF_SIMULATION = -1;

		/// <summary>
		/// Denotes an abrupt end of simulation. That is, one event of this type is enough for
		/// <seealso cref="CloudSimShutdown"/> to trigger the end of the simulation
		/// </summary>
		public const int ABRUPT_END_OF_SIMULATION = -2;

		/// <summary>
		/// Denotes insignificant simulation entity or time. This tag will not be used for identification
		/// purposes.
		/// </summary>
		public const int INSIGNIFICANT = BASE + 0;

		/// <summary>
		/// Sends an Experiment object between UserEntity and Broker entity </summary>
		public const int EXPERIMENT = BASE + 1;

		/// <summary>
		/// Denotes a cloud resource to be registered. This tag is normally used between
		/// <seealso cref="CloudInformationService"/> and CloudResouce entities.
		/// </summary>
		public const int REGISTER_RESOURCE = BASE + 2;

		/// <summary>
		/// Denotes a cloud resource to be registered, that can support advance reservation. This tag is
		/// normally used between <seealso cref="CloudInformationService"/> and CloudResouce entity.
		/// </summary>
		public const int REGISTER_RESOURCE_AR = BASE + 3;

		/// <summary>
		/// Denotes a list of all hostList's, including the ones that can support advance reservation. This
		/// tag is normally used between <seealso cref="CloudInformationService"/> and CloudSim entity.
		/// </summary>
		public const int RESOURCE_LIST = BASE + 4;

		/// <summary>
		/// Denotes a list of hostList's that only support advance reservation. This tag is normally used
		/// between <seealso cref="CloudInformationService"/> and CloudSim entity.
		/// </summary>
		public const int RESOURCE_AR_LIST = BASE + 5;

		/// <summary>
		/// Denotes cloud resource characteristics information. This tag is normally used between CloudSim
		/// and CloudResource entity.
		/// </summary>
		public const int RESOURCE_CHARACTERISTICS = BASE + 6;

		/// <summary>
		/// Denotes cloud resource allocation policy. This tag is normally used between CloudSim and
		/// CloudResource entity.
		/// </summary>
		public const int RESOURCE_DYNAMICS = BASE + 7;

		/// <summary>
		/// Denotes a request to get the total number of Processing Elements (PEs) of a resource. This
		/// tag is normally used between CloudSim and CloudResource entity.
		/// </summary>
		public const int RESOURCE_NUM_PE = BASE + 8;

		/// <summary>
		/// Denotes a request to get the total number of free Processing Elements (PEs) of a resource.
		/// This tag is normally used between CloudSim and CloudResource entity.
		/// </summary>
		public const int RESOURCE_NUM_FREE_PE = BASE + 9;

		/// <summary>
		/// Denotes a request to record events for statistical purposes. This tag is normally used
		/// between CloudSim and CloudStatistics entity.
		/// </summary>
		public const int RECORD_STATISTICS = BASE + 10;

		/// <summary>
		/// Denotes a request to get a statistical list. </summary>
		public const int RETURN_STAT_LIST = BASE + 11;

		/// <summary>
		/// Denotes a request to send an Accumulator object based on category into an event scheduler.
		/// This tag is normally used between ReportWriter and CloudStatistics entity.
		/// </summary>
		public const int RETURN_ACC_STATISTICS_BY_CATEGORY = BASE + 12;

		/// <summary>
		/// Denotes a request to register a CloudResource entity to a regional 
		/// <seealso cref="CloudInformationService"/> (CIS) entity.
		/// </summary>
		public const int REGISTER_REGIONAL_GIS = BASE + 13;

		/// <summary>
		/// Denotes a request to get a list of other regional CIS entities from the system CIS entity.
		/// </summary>
		public const int REQUEST_REGIONAL_GIS = BASE + 14;

		/// <summary>
		/// Denotes request for cloud resource characteristics information. This tag is normally used
		/// between CloudSim and CloudResource entity.
		/// </summary>
		public const int RESOURCE_CHARACTERISTICS_REQUEST = BASE + 15;

		/// <summary>
		/// This tag is used by an entity to send ping requests. </summary>
		public const int INFOPKT_SUBMIT = NETBASE + 5;

		/// <summary>
		/// This tag is used to return the ping request back to sender. </summary>
		public const int INFOPKT_RETURN = NETBASE + 6;

		/// <summary>
		/// Denotes the return of a Cloudlet back to sender. 
		/// This tag is normally used by CloudResource entity.
		/// </summary>
		public const int CLOUDLET_RETURN = BASE + 20;

		/// <summary>
		/// Denotes the submission of a Cloudlet. 
		/// This tag is normally used between CloudSim User and CloudResource entity.
		/// </summary>
		public const int CLOUDLET_SUBMIT = BASE + 21;

		/// <summary>
		/// Denotes the submission of a Cloudlet with an acknowledgement. This tag is normally used
		/// between CloudSim User and CloudResource entity.
		/// </summary>
		public const int CLOUDLET_SUBMIT_ACK = BASE + 22;

		/// <summary>
		/// Cancels a Cloudlet submitted in the CloudResource entity. </summary>
		public const int CLOUDLET_CANCEL = BASE + 23;

		/// <summary>
		/// Denotes the status of a Cloudlet. </summary>
		public const int CLOUDLET_STATUS = BASE + 24;

		/// <summary>
		/// Pauses a Cloudlet submitted in the CloudResource entity. </summary>
		public const int CLOUDLET_PAUSE = BASE + 25;

		/// <summary>
		/// Pauses a Cloudlet submitted in the CloudResource entity with an acknowledgement.
		/// </summary>
		public const int CLOUDLET_PAUSE_ACK = BASE + 26;

		/// <summary>
		/// Resumes a Cloudlet submitted in the CloudResource entity. </summary>
		public const int CLOUDLET_RESUME = BASE + 27;

		/// <summary>
		/// Resumes a Cloudlet submitted in the CloudResource entity with an acknowledgement.
		/// </summary>
		public const int CLOUDLET_RESUME_ACK = BASE + 28;

		/// <summary>
		/// Moves a Cloudlet to another CloudResource entity. </summary>
		public const int CLOUDLET_MOVE = BASE + 29;

		/// <summary>
		/// Moves a Cloudlet to another CloudResource entity with an acknowledgement.
		/// </summary>
		public const int CLOUDLET_MOVE_ACK = BASE + 30;

		/// <summary>
		/// Denotes a request to create a new VM in a <seealso cref="Datacenter"/> with acknowledgement 
		/// information sent by the Datacenter.
		/// </summary>
		public const int VM_CREATE = BASE + 31;

		/// <summary>
		/// Denotes a request to create a new VM in a <seealso cref="Datacenter"/> 
		/// with acknowledgement information sent by the Datacenter.
		/// </summary>
		public const int VM_CREATE_ACK = BASE + 32;

		/// <summary>
		/// Denotes a request to destroy a new VM in a <seealso cref="Datacenter"/>.
		/// </summary>
		public const int VM_DESTROY = BASE + 33;

		/// <summary>
		/// Denotes a request to destroy a new VM in a <seealso cref="Datacenter"/> 
		/// with acknowledgement information sent by the Datacener.
		/// </summary>
		public const int VM_DESTROY_ACK = BASE + 34;

		/// <summary>
		/// Denotes a request to migrate a new VM in a <seealso cref="Datacenter"/>.
		/// </summary>
		public const int VM_MIGRATE = BASE + 35;

		/// <summary>
		/// Denotes a request to migrate a new VM in a <seealso cref="Datacenter"/>  
		/// with acknowledgement information sent by the Datacener.
		/// </summary>
		public const int VM_MIGRATE_ACK = BASE + 36;

		/// <summary>
		/// Denotes an event to send a file from a user to a <seealso cref="Datacenter"/>.
		/// </summary>
		public const int VM_DATA_ADD = BASE + 37;

		/// <summary>
		/// Denotes an event to send a file from a user to a <seealso cref="Datacenter"/>
		/// with acknowledgement information sent by the Datacener.
		/// </summary>
		public const int VM_DATA_ADD_ACK = BASE + 38;

		/// <summary>
		/// Denotes an event to remove a file from a <seealso cref="Datacenter"/> .
		/// </summary>
		public const int VM_DATA_DEL = BASE + 39;

		/// <summary>
		/// Denotes an event to remove a file from a <seealso cref="Datacenter"/>
		/// with acknowledgement information sent by the Datacener.
		/// </summary>
		public const int VM_DATA_DEL_ACK = BASE + 40;

		/// <summary>
		/// Denotes an internal event generated in a <seealso cref="Datacenter"/>.
		/// </summary>
		public const int VM_DATACENTER_EVENT = BASE + 41;

		/// <summary>
		/// Denotes an internal event generated in a Broker.
		/// </summary>
		public const int VM_BROKER_EVENT = BASE + 42;

		public const int Network_Event_UP = BASE + 43;

		public const int Network_Event_send = BASE + 44;

		public const int RESOURCE_Register = BASE + 45;

		public const int Network_Event_DOWN = BASE + 46;

		public const int Network_Event_Host = BASE + 47;

		public const int NextCycle = BASE + 48;

		/// <summary>
		/// Private Constructor. </summary>
		private CloudSimTags()
		{
			throw new System.NotSupportedException("CloudSimTags cannot be instantiated");
		}

	}

}