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
	/// This class represents an Aggregate Switch in a Datacenter network. 
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
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class AggregateSwitch : Switch
	{

		/// <summary>
		/// Instantiates a Aggregate Switch specifying the switches that are connected to its
		/// downlink and uplink ports and corresponding bandwidths.
		/// </summary>
		/// <param name="name"> Name of the switch </param>
		/// <param name="level"> At which level the switch is with respect to hosts. </param>
		/// <param name="dc"> The Datacenter where the switch is connected to </param>
		public AggregateSwitch(string name, int level, NetworkDatacenter dc) : base(name, level, dc)
		{
			downlinkswitchpktlist = new Dictionary<int?, IList<NetworkPacket>>();
			uplinkswitchpktlist = new Dictionary<int?, IList<NetworkPacket>>();
			uplinkbandwidth = NetworkConstants.BandWidthAggRoot;
			downlinkbandwidth = NetworkConstants.BandWidthEdgeAgg;
			latency = NetworkConstants.SwitchingDelayAgg;
			numport = NetworkConstants.AggSwitchPort;
			uplinkswitches = new List<Switch>();
			downlinkswitches = new List<Switch>();
		}

		protected internal override void processpacket_down(SimEvent ev)
		{
			// packet coming from up level router.
			// has to send downward
			// check which switch to forward to
			// add packet in the switch list
			// add packet in the host list
			NetworkPacket hspkt = (NetworkPacket) ev.Data;
			int recvVMid = hspkt.pkt.reciever;
			CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.Network_Event_send));
			schedule(Id, latency, CloudSimTags.Network_Event_send);

			if (level == NetworkConstants.Agg_LEVEL)
			{
				// packet is coming from root so need to be sent to edgelevel swich
				// find the id for edgelevel switch
				int switchid = dc.VmToSwitchid[recvVMid].Value;
				IList<NetworkPacket> pktlist = downlinkswitchpktlist[switchid];
				if (pktlist == null)
				{
					pktlist = new List<NetworkPacket>();
					downlinkswitchpktlist[switchid] = pktlist;
				}
				pktlist.Add(hspkt);
				return;
			}

		}

		protected internal override void processpacket_up(SimEvent ev)
		{
			// packet coming from down level router.
			// has to send up
			// check which switch to forward to
			// add packet in the switch list
			//
			// int src=ev.getSource();
			NetworkPacket hspkt = (NetworkPacket) ev.Data;
			int recvVMid = hspkt.pkt.reciever;
			CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.Network_Event_send));
			schedule(Id, switching_delay, CloudSimTags.Network_Event_send);

			if (level == NetworkConstants.Agg_LEVEL)
			{
				// packet is coming from edge level router so need to be sent to
				// either root or another edge level swich
				// find the id for edgelevel switch
				int switchid = dc.VmToSwitchid[recvVMid].Value;
				bool flagtoswtich = false;
				foreach (Switch sw in downlinkswitches)
				{
					if (switchid == sw.Id)
					{
						flagtoswtich = true;
					}
				}
				if (flagtoswtich)
				{
					IList<NetworkPacket> pktlist = downlinkswitchpktlist[switchid];
					if (pktlist == null)
					{
						pktlist = new List<NetworkPacket>();
						downlinkswitchpktlist[switchid] = pktlist;
					}
					pktlist.Add(hspkt);
				}
				else // send to up
				{
					Switch sw = uplinkswitches[0];
					IList<NetworkPacket> pktlist = uplinkswitchpktlist[sw.Id];
					if (pktlist == null)
					{
						pktlist = new List<NetworkPacket>();
						uplinkswitchpktlist[sw.Id] = pktlist;
					}
					pktlist.Add(hspkt);
				}
			}
		}

	}

}