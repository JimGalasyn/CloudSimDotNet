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
    using PeList = org.cloudbus.cloudsim.lists.PeList;
    using VmList = org.cloudbus.cloudsim.lists.VmList;
    using BwProvisioner = org.cloudbus.cloudsim.provisioners.BwProvisioner;
    using RamProvisioner = org.cloudbus.cloudsim.provisioners.RamProvisioner;

    /// <summary>
    /// NetworkHost class extends <seealso cref="Host"/> to support simulation of networked datacenters. It executes
    /// actions related to management of packets (sent and received) other than that of virtual machines
    /// (e.g., creation and destruction). A host has a defined policy for provisioning memory and bw, as
    /// well as an allocation policy for PE's to virtual machines.
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
    public class NetworkHost : Host
    {
        public IList<NetworkPacket> packetTosendLocal;

        public IList<NetworkPacket> packetTosendGlobal;

        /// <summary>
        /// List of received packets.
        /// </summary>
        public IList<NetworkPacket> packetrecieved;

        /// <summary>
        /// @todo the attribute is not being used 
        /// and is redundant with the ram capacity defined in <seealso cref="Host#ramProvisioner"/>
        /// </summary>
        public double memory;

        /// <summary>
        /// Edge switch in which the Host is connected. 
        /// </summary>
        public Switch sw;

        /// <summary>
        /// @todo What exactly is this bandwidth?
        /// Because it is redundant with the bw capacity defined in <seealso cref="Host#bwProvisioner"/>
        /// </summary>
        public double bandwidth;

        /// <summary>
        /// Time when last job will finish on CPU1. 
        /// @todo it is not being used.
        ///        
        /// </summary>
        public IList<double?> CPUfinTimeCPU = new List<double?>();

        /// <summary>
        /// @todo it is not being used.
        ///        
        /// </summary>
        public double fintime = 0;

        public NetworkHost(int id, RamProvisioner ramProvisioner, BwProvisioner bwProvisioner, long storage, IList<Pe> peList, VmScheduler vmScheduler) : base(id, ramProvisioner, bwProvisioner, storage, peList, vmScheduler)
        {

            packetrecieved = new List<NetworkPacket>();
            packetTosendGlobal = new List<NetworkPacket>();
            packetTosendLocal = new List<NetworkPacket>();

        }

        public override double updateVmsProcessing(double currentTime)
        {
            double smallerTime = double.MaxValue;
            // insert in each vm packet recieved
            recvpackets();
            foreach (Vm vm in base.VmListProperty)
            {
                double time = ((NetworkVm)vm).updateVmProcessing(currentTime, VmScheduler.getAllocatedMipsForVm(vm));
                if (time > 0.0 && time < smallerTime)
                {
                    smallerTime = time;
                }
            }
            // send the packets to other hosts/VMs
            sendpackets();

            return smallerTime;

        }

        /// <summary>
        /// Receives packets and forward them to the corresponding VM.
        /// </summary>
        private void recvpackets()
        {
            foreach (NetworkPacket hs in packetrecieved)
            {
                hs.pkt.recievetime = CloudSim.clock();

                // insert the packet in recievedlist of VM
                Vm vm = VmList.getById(VmListProperty, hs.pkt.reciever);
                IList<HostPacket> pktlist = ((NetworkCloudletSpaceSharedScheduler)vm.CloudletScheduler).pktrecv[hs.pkt.sender];

                if (pktlist == null)
                {
                    pktlist = new List<HostPacket>();
                    ((NetworkCloudletSpaceSharedScheduler)vm.CloudletScheduler).pktrecv[hs.pkt.sender] = pktlist;

                }
                pktlist.Add(hs.pkt);

            }
            packetrecieved.Clear();
        }

        /// <summary>
        /// Sends packets checks whether a packet belongs to a local VM or to a 
        /// VM hosted on other machine.
        /// </summary>
        private void sendpackets()
        {
            foreach (Vm vm in base.VmListProperty)
            {
                // TEST: (fixed) .entrySet() == Dictionary
                foreach (KeyValuePair<int?, IList<HostPacket>> es in ((NetworkCloudletSpaceSharedScheduler)vm.CloudletScheduler).pkttosend)
                {
                    IList<HostPacket> pktlist = es.Value;
                    foreach (HostPacket pkt in pktlist)
                    {
                        NetworkPacket hpkt = new NetworkPacket(Id, pkt, vm.Id, pkt.sender);
                        Vm vm2 = VmList.getById(this.VmListProperty, hpkt.recievervmid);
                        if (vm2 != null)
                        {
                            packetTosendLocal.Add(hpkt);
                        }
                        else
                        {
                            packetTosendGlobal.Add(hpkt);
                        }
                    }
                    pktlist.Clear();
                }
            }

            bool flag = false;

            foreach (NetworkPacket hs in packetTosendLocal)
            {
                flag = true;
                // TEST: (fixed) Is this the intended assignment?
                //hs.stime = hs.rtime;
                hs.stime = hs.pkt.recievetime;
                hs.pkt.recievetime = CloudSim.clock();
                // insertthe packet in recievedlist
                Vm vm = VmList.getById(VmListProperty, hs.pkt.reciever);

                IList<HostPacket> pktlist = ((NetworkCloudletSpaceSharedScheduler)vm.CloudletScheduler).pktrecv[hs.pkt.sender];
                if (pktlist == null)
                {
                    pktlist = new List<HostPacket>();
                    ((NetworkCloudletSpaceSharedScheduler)vm.CloudletScheduler).pktrecv[hs.pkt.sender] = pktlist;
                }
                pktlist.Add(hs.pkt);
            }
            if (flag)
            {
                foreach (Vm vm in base.VmListProperty)
                {
                    vm.updateVmProcessing(CloudSim.clock(), VmScheduler.getAllocatedMipsForVm(vm));
                }
            }

            // Sending packet to other VMs therefore packet is forwarded to a Edge switch
            packetTosendLocal.Clear();
            double avband = bandwidth / packetTosendGlobal.Count;
            foreach (NetworkPacket hs in packetTosendGlobal)
            {
                double delay = (1000 * hs.pkt.data) / avband;
                NetworkConstants.totaldatatransfer += (int)hs.pkt.data;
                //NetworkConstants.totaldatatransfer += hs.pkt.data;

                // send to switch with delay
                CloudSim.send(Datacenter.Id, sw.Id, delay, CloudSimTags.Network_Event_UP, hs);
            }
            packetTosendGlobal.Clear();
        }

        /// <summary>
        /// Gets the maximum utilization among the PEs of a given VM. </summary>
        /// <param name="vm"> The VM to get its PEs maximum utilization </param>
        /// <returns> The maximum utilization among the PEs of the VM. </returns>
        public virtual double getMaxUtilizationAmongVmsPes(Vm vm)
        {
            return PeList.getMaxUtilizationAmongVmsPes(PeListProperty, vm);
        }

    }

}