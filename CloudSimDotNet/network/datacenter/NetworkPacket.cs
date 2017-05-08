/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.network.datacenter
{

	/// <summary>
	/// NewtorkPacket represents the packet which travel from one server to another. Each packet contains
	/// IDs of the sender and receiver VM which are communicating, time at which it is sent and received, 
	/// type and virtual IDs of tasks.
	/// 
	/// <br/>Please refer to following publication for more details:<br/>
	/// <ul>
	/// <li><a href="http://dx.doi.org/10.1109/UCC.2011.24">Saurabh Kumar Garg and Rajkumar Buyya, NetworkCloudSim: Modelling Parallel Applications in Cloud
	/// Simulations, Proceedings of the 4th IEEE/ACM International Conference on Utility and Cloud
	/// Computing (UCC 2011, IEEE CS Press, USA), Melbourne, Australia, December 5-7, 2011.</a>
	/// </ul>
	/// 
	/// @author Saurabh Kumar Garg
	/// @since CloudSim Toolkit 1.0
	/// @todo Attributes should be private
	/// </summary>
	public class NetworkPacket
	{
		/// <summary>
		/// Information about the virtual send and receiver entities of the packet.
		/// </summary>
		internal HostPacket pkt;

		/// <summary>
		/// Id of the sender host.
		/// </summary>
		internal int senderhostid;

		/// <summary>
		/// Id of the receiver host.
		/// </summary>
		internal int recieverhostid;

		/// <summary>
		/// Id of the sender VM.
		/// @todo Isn't this data at <seealso cref="#pkt"/>?
		/// </summary>
		internal int sendervmid;

		/// <summary>
		/// Id of the receiver VM.
		/// @todo Isn't this data at <seealso cref="#pkt"/>?
		/// </summary>
		internal int recievervmid;

		/// <summary>
		/// Id of the sender cloudlet.
		/// @todo This field is not needed, since its value is being
		/// get from a <seealso cref="HostPacket"/> instance at <seealso cref="NetworkHost#sendpackets()"/>.
		/// So, such a data can be got form the <seealso cref="#pkt"/> attribute.
		/// </summary>
		internal int cloudletid;

		/// <summary>
		/// Time when the packet was sent. </summary>
		internal double stime;

        /// <summary>
        /// Time when the packet was received. </summary>
        // TEST: (fixed) Never assigned.
        //internal double rtime;

        public NetworkPacket(int id, HostPacket pkt2, int vmid, int cloudletid)
		{
				pkt = pkt2;
				sendervmid = vmid;
				this.cloudletid = cloudletid;
				senderhostid = id;
				stime = pkt.sendtime;
				recievervmid = pkt2.reciever;

		}
	}

}