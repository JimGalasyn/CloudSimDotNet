using System;
using System.Collections.Generic;
using System.Text;

/*
 * ** Network and Service Differentiation Extensions to CloudSim 3.0 **
 *
 * Gokul Poduval & Chen-Khong Tham
 * Computer Communication Networks (CCN) Lab
 * Dept of Electrical & Computer Engineering
 * National University of Singapore
 * August 2004
 *
 * Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * Copyright (c) 2004, The University of Melbourne, Australia and National
 * University of Singapore
 * InfoPacket.java - Implementation of a Information Packet.
 *
 */

namespace org.cloudbus.cloudsim
{


	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
	using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;

	/// <summary>
	/// InfoPacket class can be used to gather information from the network layer. An InfoPacket
	/// traverses the network topology similar to a <seealso cref="gridsim.net.NetPacket"/>, but it collects
	/// information like bandwidths, and Round Trip Time etc. It is the equivalent of ICMP in physical
	/// networks.
	/// <p/>
	/// You can set all the parameters to an InfoPacket that can be applied to a NetPacket. So if you
	/// want to find out the kind of information that a particular type of NetPacket is experiencing, set
	/// the size and network class of an InfoPacket to the same as the NetPacket, and send it to the same
	/// destination from the same source.
	/// 
	/// @author Gokul Poduval
	/// @author Chen-Khong Tham, National University of Singapore
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class InfoPacket : Packet
	{

		/// <summary>
		/// The packet name. </summary>
		private readonly string name;

		/// <summary>
		/// The size of the packet. </summary>
		private long size;

		/// <summary>
		/// The id of the packet. </summary>
		private readonly int packetId;

		/// <summary>
		/// The original sender id. </summary>
		private readonly int srcId;

		/// <summary>
		/// The destination id. </summary>
		private int destId;

		/// <summary>
		/// The last hop. </summary>
		private int last;

		/// <summary>
		/// Whether the packet is going or returning, according to 
		/// constants <seealso cref=" CloudSimTags#INFOPKT_SUBMIT"/>
		/// and <seealso cref=" CloudSimTags#INFOPKT_RETURN"/>. 
		/// </summary>
		private int tag;

		/// <summary>
		/// The number of hops. </summary>
		private int hopsNumber;

		/// <summary>
		/// The original ping size. </summary>
		private long pingSize;

		/// <summary>
		/// The level of service type. 
		///        @todo Is it the Type of Service (ToS) of IPv4, like in
		///        the <seealso cref="Cloudlet#netToS"/>? If yes, so the names would
		///        be standardized. 
		/// </summary>
		private int netServiceType;

		/// <summary>
		/// The bandwidth bottleneck. </summary>
		private double bandwidth;

		/// <summary>
		/// The list of entity IDs. The entities are elements 
		///        where the packet traverses, such as Routers or CloudResources. 
		/// </summary>
		private List<int?> entities;

		/// <summary>
		/// A list containing the time the packet arrived at every entity it has traversed </summary>
		private List<double?> entryTimes;

		/// <summary>
		/// The list of exit times. </summary>
		private List<double?> exitTimes;

		/// <summary>
		/// The baud rate of each output link of entities where the packet traverses. </summary>
		private List<double?> baudRates;

		/// <summary>
		/// The formatting for decimal points. </summary>
		//private DecimalFormat num;

		/// <summary>
		/// Constructs a new Information packet.
		/// </summary>
		/// <param name="name"> Name of this packet </param>
		/// <param name="packetID"> The ID of this packet </param>
		/// <param name="size"> size of the packet </param>
		/// <param name="srcID"> The ID of the entity that sends out this packet </param>
		/// <param name="destID"> The ID of the entity to which this packet is destined </param>
		/// <param name="netServiceType"> the class of traffic this packet belongs to
		/// @pre name != null
		/// @post $none </param>
		public InfoPacket(string name, int packetID, long size, int srcID, int destID, int netServiceType)
		{
			this.name = name;
			packetId = packetID;
			srcId = srcID;
			destId = destID;
			this.size = size;
			this.netServiceType = netServiceType;

			init();
		}

		/// <summary>
		/// Initialises common attributes.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		private void init()
		{
			last = srcId;
			tag = CloudSimTags.INFOPKT_SUBMIT;
			bandwidth = -1;
			hopsNumber = 0;
			pingSize = size;

			if (!string.ReferenceEquals(name, null))
			{
				entities = new List<int?>();
				entryTimes = new List<double?>();
				exitTimes = new List<double?>();
				baudRates = new List<double?>();
				//num = new DecimalFormat("#0.000#"); // 4 decimal spaces
			}
		}

		/// <summary>
		/// Returns the ID of this packet.
		/// </summary>
		/// <returns> packet ID
		/// @pre $none
		/// @post $none </returns>
		public int Id
		{
			get
			{
				return packetId;
			}
		}

		/// <summary>
		/// Sets original size of ping request.
		/// </summary>
		/// <param name="size"> ping data size (in bytes)
		/// @pre size >= 0
		/// @post $none </param>
		public virtual long OriginalPingSize
		{
			set
			{
				pingSize = value;
			}
			get
			{
				return pingSize;
			}
		}


		/// <summary>
		/// Returns a human-readable information of this packet.
		/// </summary>
		/// <returns> description of this packet
		/// @pre $none
		/// @post $none </returns>
		public override string ToString()
		{
			if (string.ReferenceEquals(name, null))
			{
				return "Empty InfoPacket that contains no ping information.";
			}

			int SIZE = 1000; // number of chars
			StringBuilder sb = new StringBuilder(SIZE);
			sb.Append("Ping information for " + name + "\n");
			sb.Append("Entity Name\tEntry Time\tExit Time\t Bandwidth\n");
			sb.Append("----------------------------------------------------------\n");

			string tab = "    "; // 4 spaces
			for (int i = 0; i < entities.Count; i++)
			{
				int resID = entities[i].Value;
				sb.Append(CloudSim.getEntityName(resID) + "\t\t");

				string entry = getData(entryTimes, i);
				string exit = getData(exitTimes, i);
				string bw = getData(baudRates, i);

				sb.Append(entry + tab + tab + exit + tab + tab + bw + "\n");
			}

            //sb.Append("\nRound Trip Time : " + num.format(TotalResponseTime));
            sb.Append("\nRound Trip Time : " + TotalResponseTime);
            sb.Append(" seconds");
			sb.Append("\nNumber of Hops  : " + NumHop);
			sb.Append("\nBottleneck Bandwidth : " + bandwidth + " bits/s");
			return sb.ToString();
		}

		/// <summary>
		/// Gets the data of a given index in a list.
		/// </summary>
		/// <param name="v"> a list </param>
		/// <param name="index"> the location in a list </param>
		/// <returns> the data
		/// @pre v != null
		/// @post index > 0 </returns>
		private string getData(List<double?> v, int index)
		{
			string result;
			try
			{
				double? obj = v[index];
				double id = obj.Value;
                // TODO: format id as expected.
                //result = num.format(id);
                result = id.ToString();

            }
			catch (Exception)
			{
				result = "    N/A";
			}

			return result;
		}

		/// <summary>
		/// Gets the size of the packet.
		/// </summary>
		/// <returns> size of the packet.
		/// @pre $none
		/// @post $none </returns>
		public long Size
		{
			get
			{
				return size;
			}
		}

		/// <summary>
		/// Sets the size of the packet.
		/// </summary>
		/// <param name="size"> size of the packet </param>
		/// <returns> <tt>true</tt> if it is successful, <tt>false</tt> otherwise
		/// @pre size >= 0
		/// @post $none </returns>
		public bool setSize(long size)
		{
			if (size < 0)
			{
				return false;
			}

			this.size = size;
			return true;
		}

		/// <summary>
		/// Gets the id of the entity to which the packet is destined.
		/// </summary>
		/// <returns> the desination ID
		/// @pre $none
		/// @post $none </returns>
		public int DestId
		{
			get
			{
				return destId;
			}
			set
			{
				destId = value;
			}
		}

		/// <summary>
		/// Gets the id of the entity that sent out the packet.
		/// </summary>
		/// <returns> the source ID
		/// @pre $none
		/// @post $none </returns>
		public int SrcId
		{
			get
			{
				return srcId;
			}
		}

		/// <summary>
		/// Returns the number of hops that the packet has traversed. Since the packet takes a round
		/// trip, the same router may have been traversed twice.
		/// </summary>
		/// <returns> the number of hops this packet has traversed
		/// @pre $none
		/// @post $none </returns>
		public virtual int NumHop
		{
			get
			{
				int PAIR = 2;
				return ((hopsNumber - PAIR) + 1) / PAIR;
			}
		}

		/// <summary>
		/// Gets the total time that the packet has spent in the network. 
		/// This is basically the Round-Trip-Time (RTT).
		/// Dividing this by half should be the approximate latency.
		/// <p/>
		/// RTT is taken as the "final entry time" - "first exit time".
		/// </summary>
		/// <returns> total round time
		/// @pre $none
		/// @post $none </returns>
		public virtual double TotalResponseTime
		{
			get
			{
				if (exitTimes == null || entryTimes == null)
				{
					return 0;
				}
    
				double time = 0;
				try
				{
					double startTime = exitTimes[0].Value;
					double receiveTime = entryTimes[entryTimes.Count - 1].Value;
					time = receiveTime - startTime;
				}
				catch (Exception)
				{
					time = 0;
				}
    
				return time;
			}
		}

		/// <summary>
		/// Returns the bottleneck bandwidth between the source and the destination.
		/// </summary>
		/// <returns> the bottleneck bandwidth
		/// @pre $none
		/// @post $none </returns>
		public virtual double BaudRate
		{
			get
			{
				return bandwidth;
			}
		}

		/// <summary>
		/// Add an entity where the InfoPacket traverses.
		/// This method should be called by network entities that count as hops, 
		/// for instance Routers or CloudResources. It should not be called by links etc.
		/// </summary>
		/// <param name="id"> the id of the hop that this InfoPacket is traversing
		/// @pre id > 0
		/// @post $none </param>
		public virtual void addHop(int id)
		{
			if (entities == null)
			{
				return;
			}

			hopsNumber++;
			entities.Add(Convert.ToInt32(id));
		}

		/// <summary>
		/// Register the time the packet arrives at an entity such as a Router or CloudResource.
		/// This method should be called by routers and other entities when the InfoPacket reaches them
		/// along with the current simulation time.
		/// </summary>
		/// <param name="time"> current simulation time, use <seealso cref="gridsim.CloudSim#clock()"/> to obtain this
		/// @pre time >= 0
		/// @post $none
		///  </param>
		public virtual void addEntryTime(double time)
		{
			if (entryTimes == null)
			{
				return;
			}

			if (time < 0)
			{
				time = 0.0;
			}

			entryTimes.Add(Convert.ToDouble(time));
		}

		/// <summary>
		/// Register the time the packet leaves an entity such as a Router or CloudResource.
		/// This method should be called by routers and other entities when the InfoPacket is leaving
		/// them. It should also supply the current simulation time.
		/// </summary>
		/// <param name="time"> current simulation time, use <seealso cref="gridsim.CloudSim#clock()"/> to obtain this
		/// @pre time >= 0
		/// @post $none </param>
		public virtual void addExitTime(double time)
		{
			if (exitTimes == null)
			{
				return;
			}

			if (time < 0)
			{
				time = 0.0;
			}

			exitTimes.Add(Convert.ToDouble(time));
		}

		/// <summary>
		/// Register the baud rate of the output link where the current entity that holds the InfoPacket
		/// will send it next.
		/// Every entity that the InfoPacket traverses should add the baud rate of the link on which this
		/// packet will be sent out next.
		/// </summary>
		/// <param name="baudRate"> the entity's baud rate in bits/s
		/// @pre baudRate > 0
		/// @post $none </param>
		public virtual void addBaudRate(double baudRate)
		{
			if (baudRates == null)
			{
				return;
			}

			baudRates.Add(new double?(baudRate));
			if (bandwidth < 0 || baudRate < bandwidth)
			{
				bandwidth = baudRate;
			}
		}

		/// <summary>
		/// Returns the list of all the bandwidths that this packet has traversed.
		/// </summary>
		/// <returns> a Double Array of links bandwidths
		/// @pre $none
		/// @post $none </returns>
		public virtual double?[] DetailBaudRate
		{
			get
			{
				if (baudRates == null)
				{
					return null;
				}
    
				return baudRates.ToArray();
			}
		}

		/// <summary>
		/// Returns the list of all the hops that this packet has traversed.
		/// </summary>
		/// <returns> an Integer Array of hop ids
		/// @pre $none
		/// @post $none
		/// @todo Why does not return an array of Integer (that is the type of the 
		/// entities attribute)? In fact, this method does not appear to be used anywhere. </returns>
		public virtual int?[] DetailHops
		{
			get
			{
				if (entities == null)
				{
					return null;
				}
    
				return entities.ToArray();
			}
		}

		/// <summary>
		/// Returns the list of all entry times that the packet has traversed.
		/// </summary>
		/// <returns> an Integer Array of entry time
		/// @pre $none
		/// @post $none
		/// @todo Why does not return an array of Double (that is the type of the 
		/// entyTimes attribute)? In fact, this method does not appear to be used anywhere. </returns>
		public virtual double?[] DetailEntryTimes
		{
			get
			{
				if (entryTimes == null)
				{
					return null;
				}
    
				return entryTimes.ToArray();
			}
		}

		/// <summary>
		/// Returns the list of all exit times from all entities that the packet has traversed.
		/// </summary>
		/// <returns> an Integer Array of exit time
		/// @pre $none
		/// @post $none </returns>
		public virtual double?[] DetailExitTimes
		{
			get
			{
				if (exitTimes == null)
				{
					return null;
				}
    
				return exitTimes.ToArray();
			}
		}

		/// <summary>
		/// Gets the entity ID of the last hop that this packet has traversed.
		/// </summary>
		/// <returns> an entity ID
		/// @pre $none
		/// @post $none </returns>
		public int Last
		{
			get
			{
				return last;
			}
			set
			{
				this.last = value;
			}
		}


		/// <summary>
		/// Gets the network service type of the packet.
		/// </summary>
		/// <returns> the network service type
		/// @pre $none
		/// @post $none </returns>
		public int NetServiceType
		{
			get
			{
				return netServiceType;
			}
			set
			{
				this.netServiceType = value;
			}
		}


		/// <summary>
		/// Gets the packet tag.
		/// </summary>
		/// <returns> this packet tag
		/// @pre $none
		/// @post $none </returns>
		public int Tag
		{
			get
			{
				return tag;
			}
            set
            {
                tag = value;
            }
		}

		/// <summary>
		/// Sets the tag of the packet.
		/// </summary>
		/// <param name="tag"> the packet's tag </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise
		/// @pre tag > 0
		/// @post $none </returns>
		public virtual bool setTag(int tag)
		{
			bool flag = false;
			switch (tag)
			{
				case CloudSimTags.INFOPKT_SUBMIT:
				case CloudSimTags.INFOPKT_RETURN:
					this.tag = tag;
					flag = true;
					break;

				default:
					flag = false;
					break;
			}

			return flag;
		}


	}

}