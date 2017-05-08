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
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
    using PredicateType = org.cloudbus.cloudsim.core.predicates.PredicateType;
    using System.Diagnostics;

    /// <summary>
    /// This class allows to simulate Root switch which connects Datacenters to external network. 
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
    public class RootSwitch : Switch
	{

		/// <summary>
		/// Instantiates a Root Switch specifying what other switches are connected to its downlink
		/// ports, and corresponding bandwidths.
		/// </summary>
		/// <param name="name"> Name of the root switch </param>
		/// <param name="level"> At which level the switch is with respect to hosts. </param>
		/// <param name="dc"> The Datacenter where the switch is connected to </param>
		public RootSwitch(string name, int level, NetworkDatacenter dc) : base(name, level, dc)
		{
			downlinkswitchpktlist = new Dictionary<int?, IList<NetworkPacket>>();
			downlinkswitches = new List<Switch>();

			downlinkbandwidth = NetworkConstants.BandWidthAggRoot;
			latency = NetworkConstants.SwitchingDelayRoot;
			numport = NetworkConstants.RootSwitchPort;
		}

		protected internal override void processpacket_up(SimEvent ev)
		{

			// packet coming from down level router.
			// has to send up
			// check which switch to forward to
			// add packet in the switch list

			NetworkPacket hspkt = (NetworkPacket) ev.Data;
			int recvVMid = hspkt.pkt.reciever;
			CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.Network_Event_send));
			schedule(Id, switching_delay, CloudSimTags.Network_Event_send);

			if (level == NetworkConstants.ROOT_LEVEL)
			{
				// get id of edge router
				int edgeswitchid = dc.VmToSwitchid[recvVMid].Value;
				// search which aggregate switch has it
				int aggSwtichid = -1;
				;
				foreach (Switch sw in downlinkswitches)
				{
					foreach (Switch edge in sw.downlinkswitches)
					{
						if (edge.Id == edgeswitchid)
						{
							aggSwtichid = sw.Id;
							break;
						}
					}
				}
				if (aggSwtichid < 0)
				{
					Debug.WriteLine(" No destination for this packet");
				}
				else
				{
					IList<NetworkPacket> pktlist = downlinkswitchpktlist[aggSwtichid];
					if (pktlist == null)
					{
						pktlist = new List<NetworkPacket>();
						downlinkswitchpktlist[aggSwtichid] = pktlist;
					}
					pktlist.Add(hspkt);
				}
			}
		}
	}

}