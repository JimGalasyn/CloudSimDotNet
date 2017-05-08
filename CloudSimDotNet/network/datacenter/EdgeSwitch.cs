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
	using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
	using PredicateType = org.cloudbus.cloudsim.core.predicates.PredicateType;

	/// <summary>
	/// This class represents an Edge Switch in a Datacenter network. 
	/// It interacts with other switches in order to exchange packets.
	/// 
	/// <br/>Please refer to following publication for more details:<br/>
	/// <ul>
	/// <li><a href="http://dx.doi.org/10.1109/UCC.2011.24">Saurabh Kumar Garg and Rajkumar Buyya, NetworkCloudSim: Modelling Parallel Applications in Cloud
	/// Simulations, Proceedings of the 4th IEEE/ACM International Conference on Utility and Cloud
	/// Computing (UCC 2011, IEEE CS Press, USA), Melbourne, Australia, December 5-7, 2011.</a>
	/// </ul>
	/// 
	/// @author Saurabh Kumar Garg
	/// @since CloudSim Toolkit 3.0
	/// </summary>
	public class EdgeSwitch : Switch
	{

		/// <summary>
		/// Instantiates a EdgeSwitch specifying switches that are connected to its downlink
		/// and uplink ports, and corresponding bandwidths. 
		/// In this switch, downlink ports aren't connected to other switch but to hosts.
		/// </summary>
		/// <param name="name"> Name of the switch </param>
		/// <param name="level"> At which level the switch is with respect to hosts. </param>
		/// <param name="dc"> The Datacenter where the switch is connected to </param>
		public EdgeSwitch(string name, int level, NetworkDatacenter dc) : base(name, level, dc)
		{
			hostlist = new Dictionary<int?, NetworkHost>();
			uplinkswitchpktlist = new Dictionary<int?, IList<NetworkPacket>>();
			packetTohost = new Dictionary<int?, IList<NetworkPacket>>();
			uplinkbandwidth = NetworkConstants.BandWidthEdgeAgg;
			downlinkbandwidth = NetworkConstants.BandWidthEdgeHost;
			switching_delay = NetworkConstants.SwitchingDelayEdge;
			numport = NetworkConstants.EdgeSwitchPort;
			uplinkswitches = new List<Switch>();
		}

		protected internal override void processpacket_up(SimEvent ev)
		{
			// packet coming from down level router/host.
			// has to send up
			// check which switch to forward to
			// add packet in the switch list
			//
			// int src=ev.getSource();
			NetworkPacket hspkt = (NetworkPacket) ev.Data;
			int recvVMid = hspkt.pkt.reciever;
			CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.Network_Event_send));
			schedule(Id, switching_delay, CloudSimTags.Network_Event_send);

			// packet is recieved from host
			// packet is to be sent to aggregate level or to another host in the same level

			int hostid = dc.VmtoHostlist[recvVMid].Value;
			NetworkHost hs = hostlist[hostid];
			hspkt.recieverhostid = hostid;

			// packet needs to go to a host which is connected directly to switch
			if (hs != null)
			{
				// packet to be sent to host connected to the switch
				IList<NetworkPacket> packetlist = packetTohost[hostid];
				if (packetlist == null)
				{
                    packetlist = new List<NetworkPacket>();
					packetTohost[hostid] = packetlist;
				}
                packetlist.Add(hspkt);
				return;

			}
			// otherwise
			// packet is to be sent to upper switch
			// ASSUMPTION EACH EDGE is Connected to one aggregate level switch
			// if there are more than one Aggregate level switch one need to modify following code

			Switch sw = uplinkswitches[0];
			IList<NetworkPacket> pktlist = uplinkswitchpktlist[sw.Id];
			if (pktlist == null)
			{
				pktlist = new List<NetworkPacket>();
				uplinkswitchpktlist[sw.Id] = pktlist;
			}
			pktlist.Add(hspkt);
			return;

		}

		protected internal override void processpacketforward(SimEvent ev)
		{
			// search for the host and packets..send to them

			if (uplinkswitchpktlist != null)
			{
                // TEST: (fixed) entrySet == Dictionary?
                //foreach (KeyValuePair<int?, IList<NetworkPacket>> es in uplinkswitchpktlist.entrySet())
                foreach (KeyValuePair<int?, IList<NetworkPacket>> es in uplinkswitchpktlist)
                {
					int tosend = es.Key.Value;
					IList<NetworkPacket> hspktlist = es.Value;
					if (hspktlist.Count > 0)
					{
						// sharing bandwidth between packets
						double avband = uplinkbandwidth / hspktlist.Count;
						IEnumerator<NetworkPacket> it = hspktlist.GetEnumerator();
						while (it.MoveNext())
						{
							NetworkPacket hspkt = it.Current;
							double delay = 1000 * hspkt.pkt.data / avband;

							this.send(tosend, delay, CloudSimTags.Network_Event_UP, hspkt);
						}
						hspktlist.Clear();
					}
				}
			}
			if (packetTohost != null)
			{
                // TEST: (fixed) entrySet == Dictionary?
                //foreach (KeyValuePair<int?, IList<NetworkPacket>> es in packetTohost.entrySet())
                foreach (KeyValuePair<int?, IList<NetworkPacket>> es in packetTohost)
                {
					IList<NetworkPacket> hspktlist = es.Value;
					if (hspktlist.Count > 0)
					{
						double avband = downlinkbandwidth / hspktlist.Count;
						IEnumerator<NetworkPacket> it = hspktlist.GetEnumerator();
						while (it.MoveNext())
						{
							NetworkPacket hspkt = it.Current;
							// hspkt.recieverhostid=tosend;
							// hs.packetrecieved.add(hspkt);
							this.send(Id, hspkt.pkt.data / avband, CloudSimTags.Network_Event_Host, hspkt);
						}
						hspktlist.Clear();
					}
				}
			}

			// or to switch at next level.
			// clear the list

		}

	}

}