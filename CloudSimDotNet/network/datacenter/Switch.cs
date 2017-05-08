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
    using SimEntity = org.cloudbus.cloudsim.core.SimEntity;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
    using PredicateType = org.cloudbus.cloudsim.core.predicates.PredicateType;
    using VmList = org.cloudbus.cloudsim.lists.VmList;
    using System.Diagnostics;

    /// <summary>
    /// Represents a Network Switch.
    /// @todo attributes should be private
    /// </summary>
    public class Switch : SimEntity
	{

		/// <summary>
		/// The switch id </summary>
		public int id;

			/// <summary>
			/// The level (layer) of the switch in the network topology.
			/// </summary>
		public int level;

			/// <summary>
			/// The id of the datacenter where the switch is connected to.
			/// @todo It doesn't appear to be used
			/// </summary>
		public int datacenterid;

			/// <summary>
			/// Map of packets sent to switches on the uplink,
			/// where each key is a switch id and the corresponding
			/// value is the packets sent to that switch.
			/// </summary>
		public IDictionary<int?, IList<NetworkPacket>> uplinkswitchpktlist;

			/// <summary>
			/// Map of packets sent to switches on the downlink,
			/// where each key is a switch id and the corresponding
			/// value is the packets sent to that switch.
			/// </summary>
		public IDictionary<int?, IList<NetworkPacket>> downlinkswitchpktlist;

			/// <summary>
			/// Map of hosts connected to the switch, where each key is the host ID
			/// and the corresponding value is the host itself.
			/// </summary>
		public IDictionary<int?, NetworkHost> hostlist;

			/// <summary>
			/// List of uplink switches.
			/// </summary>
		public IList<Switch> uplinkswitches;

			/// <summary>
			/// List of downlink switches.
			/// </summary>
		public IList<Switch> downlinkswitches;

			/// <summary>
			/// Map of packets sent to hosts connected in the switch,
			/// where each key is a host id and the corresponding
			/// value is the packets sent to that host.
			/// </summary>
		public IDictionary<int?, IList<NetworkPacket>> packetTohost;

        /// <summary>
        /// The switch type: edge switch or aggregation switch.
        /// @todo should be an enum
        /// </summary>
        // TEST: (fixed) Never used.
        //internal int type;

        /// <summary>
        /// Bandwitdh of uplink.
        /// </summary>
        public double uplinkbandwidth;

			/// <summary>
			/// Bandwitdh of downlink.
			/// </summary>
		public double downlinkbandwidth;

			/// <summary>
			/// The latency of the network where the switch is connected to.
			/// @todo Its value is being defined by a constant, but not every subclass
			/// is setting the attribute accordingly.
			/// The constants should be used as default values, but the class
			/// should have a setter for this attribute.
			/// </summary>
		public double latency;

		public double numport;

			/// <summary>
			/// The datacenter where the switch is connected to.
			/// @todo It doesn't appear to be used
			/// </summary>
		public NetworkDatacenter dc;

		/// <summary>
		/// Something is running on these hosts. 
		/// @todo The attribute is only used at the TestExample class. 
		/// </summary>
		public SortedDictionary<double?, IList<NetworkHost>> fintimelistHost = new SortedDictionary<double?, IList<NetworkHost>>();

		/// <summary>
		/// Something is running on these hosts. 
		/// @todo The attribute doesn't appear to be used 
		/// </summary>
		public SortedDictionary<double?, IList<NetworkVm>> fintimelistVM = new SortedDictionary<double?, IList<NetworkVm>>();

			/// <summary>
			/// List of  received packets.
			/// </summary>
		public List<NetworkPacket> pktlist = new List<NetworkPacket>();

		/// <summary>
		/// @todo The attribute doesn't appear to be used 
		/// </summary>
		public IList<Vm> BagofTaskVm = new List<Vm>();

			/// <summary>
			/// The time the switch spends to process a received packet.
			/// This time is considered constant no matter how many packets 
			/// the switch have to process.
			/// 
			/// @todo The value of this attribute is being defined by
			/// constants such as <seealso cref="NetworkConstants#SwitchingDelayRoot"/>,
			/// but not all sub classes are setting a value to it.
			/// The constants should be used as default values, but the class
			/// should have a setter for this attribute.
			/// </summary>
		public double switching_delay;

			/// <summary>
			/// A map of VMs connected to this switch.
			/// @todo The list doesn't appear to be updated (VMs added to it) anywhere. 
			/// </summary>
		public IDictionary<int?, NetworkVm> Vmlist = new Dictionary<int?, NetworkVm>();

		public Switch(string name, int level, NetworkDatacenter dc) : base(name)
		{
			this.level = level;
			this.dc = dc;
		}

		public override void startEntity()
		{
			Log.printConcatLine(Name, " is starting...");
			schedule(Id, 0, CloudSimTags.RESOURCE_CHARACTERISTICS_REQUEST);
		}

		public override void processEvent(SimEvent ev)
		{
			// Log.printLine(CloudSim.clock()+"[Broker]: event received:"+ev.getTag());
			switch (ev.Tag)
			{
			// Resource characteristics request
				case CloudSimTags.Network_Event_UP:
					// process the packet from down switch or host
					processpacket_up(ev);
					break;
				case CloudSimTags.Network_Event_DOWN:
					// process the packet from uplink
					processpacket_down(ev);
					break;
				case CloudSimTags.Network_Event_send:
					processpacketforward(ev);
					break;

				case CloudSimTags.Network_Event_Host:
					processhostpacket(ev);
					break;
				// Resource characteristics answer
				case CloudSimTags.RESOURCE_Register:
					registerHost(ev);
					break;
				// other unknown tags are processed by this method
				default:
					processOtherEvent(ev);
					break;
			}
		}

			/// <summary>
			/// Process a packet sent to a host. </summary>
			/// <param name="ev"> The packet sent. </param>
		protected internal virtual void processhostpacket(SimEvent ev)
		{
			// Send packet to host
			NetworkPacket hspkt = (NetworkPacket) ev.Data;
			NetworkHost hs = hostlist[hspkt.recieverhostid];
			hs.packetrecieved.Add(hspkt);
		}

		/// <summary>
		/// Sends a packet to switches connected through a downlink port.
		/// </summary>
		/// <param name="ev"> Event/packet to process </param>
		protected internal virtual void processpacket_down(SimEvent ev)
		{
			// packet coming from up level router
			// has to send downward.
			// check which switch to forward to
			// add packet in the switch list
			// add packet in the host list
			// int src=ev.getSource();
			NetworkPacket hspkt = (NetworkPacket) ev.Data;
			int recvVMid = hspkt.pkt.reciever;
			CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.Network_Event_send));
			schedule(Id, latency, CloudSimTags.Network_Event_send);
			if (level == NetworkConstants.EDGE_LEVEL)
			{
				// packet is to be recieved by host
				int hostid = dc.VmtoHostlist[recvVMid].Value;
				hspkt.recieverhostid = hostid;
				IList<NetworkPacket> pktlist = packetTohost[hostid];
				if (pktlist == null)
				{
					pktlist = new List<NetworkPacket>();
					packetTohost[hostid] = pktlist;
				}
				pktlist.Add(hspkt);
				return;
			}
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

		/// <summary>
		/// Sends a packet to switches connected through a uplink port.
		/// </summary>
		/// <param name="ev"> Event/packet to process </param>
		protected internal virtual void processpacket_up(SimEvent ev)
		{
			// packet coming from down level router.
			// has to be sent up.
			// check which switch to forward to
			// add packet in the switch list
			//
			// int src=ev.getSource();
			NetworkPacket hspkt = (NetworkPacket) ev.Data;
			int recvVMid = hspkt.pkt.reciever;
			CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.Network_Event_send));
			schedule(Id, switching_delay, CloudSimTags.Network_Event_send);
			if (level == NetworkConstants.EDGE_LEVEL)
			{
				// packet is recieved from host
				// packet is to be sent to aggregate level or to another host in the
				// same level

				int hostid = dc.VmtoHostlist[recvVMid].Value;
				NetworkHost hs = hostlist[hostid];
				hspkt.recieverhostid = hostid;
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
				// packet is to be sent to upper switch
				// ASSUMPTION EACH EDGE is Connected to one aggregate level switch

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

			/// <summary>
			/// Register a host that is connected to the switch. </summary>
			/// <param name="ev">  </param>
		private void registerHost(SimEvent ev)
		{
			NetworkHost hs = (NetworkHost) ev.Data;
			hostlist[hs.Id] = hs;
		}

			/// <summary>
			/// Process a received packet. </summary>
			/// <param name="ev"> The packet received. </param>
		protected internal virtual void processpacket(SimEvent ev)
		{
			// send packet to itself with switching delay (discarding other)
			CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.Network_Event_UP));
			schedule(Id, switching_delay, CloudSimTags.Network_Event_UP);
			pktlist.Add((NetworkPacket) ev.Data);

			// add the packet in the list

		}

			/// <summary>
			/// Process non-default received events that aren't processed by
			/// the <seealso cref="#processEvent(org.cloudbus.cloudsim.core.SimEvent)"/> method.
			/// This method should be overridden by subclasses in other to process
			/// new defined events.
			/// 
			/// @todo the method should be protected to allow sub classes to override it,
			/// once it does nothing here.
			/// </summary>
		private void processOtherEvent(SimEvent ev)
		{

		}

		/// <summary>
		/// Sends a packet to hosts connected to the switch
		/// </summary>
		/// <param name="ev"> Event/packet to process </param>
		protected internal virtual void processpacketforward(SimEvent ev)
		{
			// search for the host and packets..send to them
			if (downlinkswitchpktlist != null)
			{
				foreach (KeyValuePair<int?, IList<NetworkPacket>> es in downlinkswitchpktlist.SetOfKeyValuePairs())
				{
					int tosend = es.Key.Value;
					IList<NetworkPacket> hspktlist = es.Value;
					if (hspktlist.Count > 0)
					{
						double avband = downlinkbandwidth / hspktlist.Count;
						IEnumerator<NetworkPacket> it = hspktlist.GetEnumerator();
						while (it.MoveNext())
						{
							NetworkPacket hspkt = it.Current;
							double delay = 1000 * hspkt.pkt.data / avband;

							this.send(tosend, delay, CloudSimTags.Network_Event_DOWN, hspkt);
						}
						hspktlist.Clear();
					}
				}
			}
			if (uplinkswitchpktlist != null)
			{
				foreach (KeyValuePair<int?, IList<NetworkPacket>> es in uplinkswitchpktlist.SetOfKeyValuePairs())
				{
					int tosend = es.Key.Value;
					IList<NetworkPacket> hspktlist = es.Value;
					if (hspktlist.Count > 0)
					{
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
				foreach (KeyValuePair<int?, IList<NetworkPacket>> es in packetTohost.SetOfKeyValuePairs())
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

			/// <summary>
			/// Gets the host of a given VM. </summary>
			/// <param name="vmid"> The id of the VM </param>
			/// <returns> the host of the VM </returns>
		protected internal virtual NetworkHost getHostwithVM(int vmid)
		{
			foreach (KeyValuePair<int?, NetworkHost> es in hostlist.SetOfKeyValuePairs())
			{
				Vm vm = VmList.getById(es.Value.VmListProperty, vmid);
				if (vm != null)
				{
					return es.Value;
				}
			}
			return null;
		}

			/// <summary>
			/// Gets a list with a given number of free VMs.
			/// </summary>
			/// <param name="numVMReq"> The number of free VMs to get. </param>
			/// <returns> the list of free VMs. </returns>
		protected internal virtual IList<NetworkVm> getfreeVmlist(int numVMReq)
		{
			IList<NetworkVm> freehostls = new List<NetworkVm>();
			foreach (KeyValuePair<int?, NetworkVm> et in Vmlist.SetOfKeyValuePairs())
			{
				if (et.Value.Free)
				{
					freehostls.Add(et.Value);
				}
				if (freehostls.Count == numVMReq)
				{
					break;
				}
			}

			return freehostls;
		}

			/// <summary>
			/// Gets a list with a given number of free hosts.
			/// </summary>
			/// <param name="numhost"> The number of free hosts to get. </param>
			/// <returns> the list of free hosts. </returns>
		protected internal virtual IList<NetworkHost> getfreehostlist(int numhost)
		{
			IList<NetworkHost> freehostls = new List<NetworkHost>();
			foreach (KeyValuePair<int?, NetworkHost> et in hostlist.SetOfKeyValuePairs())
			{
				if (et.Value.NumberOfFreePes == et.Value.NumberOfPes)
				{
					freehostls.Add(et.Value);
				}
				if (freehostls.Count == numhost)
				{
					break;
				}
			}

			return freehostls;
		}

		public override void shutdownEntity()
		{
			Log.printConcatLine(Name, " is shutting down...");
		}

	}

}