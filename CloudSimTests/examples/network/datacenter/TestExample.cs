using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.examples.network.datacenter
{


    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using EdgeSwitch = org.cloudbus.cloudsim.network.datacenter.EdgeSwitch;
    using NetDatacenterBroker = org.cloudbus.cloudsim.network.datacenter.NetDatacenterBroker;
    using NetworkConstants = org.cloudbus.cloudsim.network.datacenter.NetworkConstants;
    using NetworkDatacenter = org.cloudbus.cloudsim.network.datacenter.NetworkDatacenter;
    using NetworkHost = org.cloudbus.cloudsim.network.datacenter.NetworkHost;
    using NetworkVm = org.cloudbus.cloudsim.network.datacenter.NetworkVm;
    using NetworkVmAllocationPolicy = org.cloudbus.cloudsim.network.datacenter.NetworkVmAllocationPolicy;
    using BwProvisionerSimple = org.cloudbus.cloudsim.provisioners.BwProvisionerSimple;
    using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using RamProvisionerSimple = org.cloudbus.cloudsim.provisioners.RamProvisionerSimple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    [TestClass]
    public class TestExample
    {
        /// <summary>
        /// The vmlist. </summary>
        private static IList<NetworkVm> vmlist;

        /// <summary>
        /// Creates main() to run this example.
        /// </summary>
        /// <param name="args">
        ///            the args </param>
        public async Task TestExampleMain()
        //public static void Main(string[] args)
        {

            Log.printLine("Starting CloudSimExample1...");

            try
            {
                int num_user = 1; // number of cloud users
                DateTime calendar = new DateTime();
                bool trace_flag = false; // mean trace events

                // Initialize the CloudSim library
                CloudSim.init(num_user, calendar, trace_flag);

                // Second step: Create Datacenters
                // Datacenters are the resource providers in CloudSim. We need at
                // list one of them to run a CloudSim simulation
                NetworkDatacenter datacenter0 = createDatacenter("Datacenter_0");

                // Third step: Create Broker
                NetDatacenterBroker broker = createBroker();
                //broker.LinkDC = datacenter0;
                // TODO: Is NetDatacenterBroker.LinkDC really a static property?
                NetDatacenterBroker.LinkDC = datacenter0;
                // broker.setLinkDC(datacenter0);
                // Fifth step: Create one Cloudlet

                vmlist = new List<NetworkVm>();

                // submit vm list to the broker
                // TODO: Convert vmlist
                //broker.submitVmList(vmlist);

                // Sixth step: Starts the simulation
                CloudSim.startSimulation();

                CloudSim.stopSimulation();

                // Final step: Print results when simulation is over
                IList<Cloudlet> newList = broker.CloudletReceivedListProperty;
                printCloudletList(newList);
                Console.WriteLine("numberofcloudlet " + newList.Count + " Cached " + NetDatacenterBroker.cachedcloudlet + " Data transfered " + NetworkConstants.totaldatatransfer);

                Log.printLine("CloudSimExample1 finished!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Log.printLine("Unwanted errors happen");
                throw e;
            }
        }

        /// <summary>
        /// Creates the datacenter.
        /// </summary>
        /// <param name="name">
        ///            the name
        /// </param>
        /// <returns> the datacenter </returns>
    	private static NetworkDatacenter createDatacenter(string name)
        {

            // Here are the steps needed to create a PowerDatacenter:
            // 1. We need to create a list to store
            // our machine

            IList<NetworkHost> hostList = new List<NetworkHost>();

            // 2. A Machine contains one or more PEs or CPUs/Cores.
            // In this example, it will have only one core.
            // List<Pe> peList = new ArrayList<Pe>();

            int mips = 1;

            // 3. Create PEs and add these into a list.
            // peList.add(new Pe(0, new PeProvisionerSimple(mips))); // need to
            // store Pe id and MIPS Rating

            // 4. Create Host with its id and list of PEs and add them to the list
            // of machines
            int ram = 2048; // host memory (MB)
            long storage = 1000000; // host storage
            int bw = 10000;
            for (int i = 0; i < NetworkConstants.EdgeSwitchPort * NetworkConstants.AggSwitchPort * NetworkConstants.RootSwitchPort; i++)
            {
                // 2. A Machine contains one or more PEs or CPUs/Cores.
                // In this example, it will have only one core.
                // 3. Create PEs and add these into an object of PowerPeList.
                IList<Pe> peList = new List<Pe>();
                peList.Add(new Pe(0, new PeProvisionerSimple(mips))); // need to
                                                                      // store
                                                                      // PowerPe
                                                                      // id and
                                                                      // MIPS
                                                                      // Rating
                peList.Add(new Pe(1, new PeProvisionerSimple(mips))); // need to
                                                                      // store
                                                                      // PowerPe
                                                                      // id and
                                                                      // MIPS
                                                                      // Rating
                peList.Add(new Pe(2, new PeProvisionerSimple(mips))); // need to
                                                                      // store
                                                                      // PowerPe
                                                                      // id and
                                                                      // MIPS
                                                                      // Rating
                peList.Add(new Pe(3, new PeProvisionerSimple(mips))); // need to
                                                                      // store
                                                                      // PowerPe
                                                                      // id and
                                                                      // MIPS
                                                                      // Rating
                peList.Add(new Pe(4, new PeProvisionerSimple(mips))); // need to
                                                                      // store
                                                                      // PowerPe
                                                                      // id and
                                                                      // MIPS
                                                                      // Rating
                peList.Add(new Pe(5, new PeProvisionerSimple(mips))); // need to
                                                                      // store
                                                                      // PowerPe
                                                                      // id and
                                                                      // MIPS
                                                                      // Rating
                peList.Add(new Pe(6, new PeProvisionerSimple(mips))); // need to
                                                                      // store
                                                                      // PowerPe
                                                                      // id and
                                                                      // MIPS
                                                                      // Rating
                peList.Add(new Pe(7, new PeProvisionerSimple(mips))); // need to
                                                                      // store
                                                                      // PowerPe
                                                                      // id and
                                                                      // MIPS
                                                                      // Rating

                // 4. Create PowerHost with its id and list of PEs and add them to
                // the list of machines
                hostList.Add(new NetworkHost(i, new RamProvisionerSimple(ram), new BwProvisionerSimple(bw), storage, peList, new VmSchedulerTimeShared(peList))); // This is our machine
            }
            // 5. Create a DatacenterCharacteristics object that stores the
            // properties of a data center: architecture, OS, list of
            // Machines, allocation policy: time- or space-shared, time zone
            // and its price (G$/Pe time unit).
            string arch = "x86"; // system architecture
            string os = "Linux"; // operating system
            string vmm = "Xen";
            double time_zone = 10.0; // time zone this resource located
            double cost = 3.0; // the cost of using processing in this resource
            double costPerMem = 0.05; // the cost of using memory in this resource
            double costPerStorage = 0.001; // the cost of using storage in this
                                           // resource
            double costPerBw = 0.0; // the cost of using bw in this resource
            LinkedList<Storage> storageList = new LinkedList<Storage>(); // we are
                                                                         // not
                                                                         // adding
                                                                         // SAN
                                                                         // devices by now

            // TODO: Convert hostList
            DatacenterCharacteristics characteristics = null; //new DatacenterCharacteristics(arch, os, vmm, hostList, time_zone, cost, costPerMem, costPerStorage, costPerBw);

            // 6. Finally, we need to create a NetworkDatacenter object.
            NetworkDatacenter datacenter = null;
            try
            {
                // TODO: Convert hostList
                datacenter = null; // new NetworkDatacenter(name, characteristics, new NetworkVmAllocationPolicy(hostList), storageList, 0);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                throw e;
            }
            // Create Internal Datacenter network
            CreateNetwork(2, datacenter);
            return datacenter;
        }

        // We strongly encourage users to develop their own broker policies, to
        // submit vms and cloudlets according
        // to the specific rules of the simulated scenario
        /// <summary>
        /// Creates the broker.
        /// </summary>
        /// <returns> the datacenter broker </returns>
        private static NetDatacenterBroker createBroker()
        {
            NetDatacenterBroker broker = null;
            try
            {
                broker = new NetDatacenterBroker("Broker");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                throw e;
                //return null;
            }
            return broker;
        }

        /// <summary>
        /// Prints the Cloudlet objects.
        /// </summary>
        /// <param name="list">
        ///            list of Cloudlets </param>
        /// <exception cref="IOException"> </exception>
        private static void printCloudletList(IList<Cloudlet> list)
        {
            int size = list.Count;
            Cloudlet cloudlet;
            string indent = "    ";
            Log.printLine();
            Log.printLine("========== OUTPUT ==========");
            Log.printLine("Cloudlet ID" + indent + "STATUS" + indent + "Data center ID" + indent + "VM ID" + indent + "Time" + indent + "Start Time" + indent + "Finish Time");

            // TODO: DecimalFormat 
            //DecimalFormat dft = new DecimalFormat("###.##");
            for (int i = 0; i < size; i++)
            {
                cloudlet = list[i];
                Log.print(indent + cloudlet.CloudletId + indent + indent);

                if (cloudlet.CloudletStatus == Cloudlet.SUCCESS)
                {
                    Log.print("SUCCESS");
                    //Log.printLine(indent + indent + cloudlet.ResourceId + indent + indent + indent + cloudlet.VmId + indent + indent + dft.format(cloudlet.ActualCPUTime) + indent + indent + dft.format(cloudlet.ExecStartTime) + indent + indent + dft.format(cloudlet.FinishTime));
                    Log.printLine(indent + indent + cloudlet.ResourceId + indent + indent + indent + cloudlet.VmId + indent + indent + cloudlet.ActualCPUTime + indent + indent + cloudlet.ExecStartTime + indent + indent + cloudlet.FinishTime);
                }
            }
        }

        internal static void CreateNetwork(int numhost, NetworkDatacenter dc)
        {
            // Edge Switch
            EdgeSwitch[] edgeswitch = new EdgeSwitch[1];

            for (int i = 0; i < 1; i++)
            {
                edgeswitch[i] = new EdgeSwitch("Edge" + i, NetworkConstants.EDGE_LEVEL, dc);
                // edgeswitch[i].uplinkswitches.add(null);
                dc.Switchlist[edgeswitch[i].Id] = edgeswitch[i];
                // aggswitch[(int)
                // (i/Constants.AggSwitchPort)].downlinkswitches.add(edgeswitch[i]);
            }

            foreach (Host hs in dc.HostListProperty)
            {
                NetworkHost hs1 = (NetworkHost)hs;
                hs1.bandwidth = NetworkConstants.BandWidthEdgeHost;
                int switchnum = (int)(hs.Id / NetworkConstants.EdgeSwitchPort);
                edgeswitch[switchnum].hostlist[hs.Id] = hs1;
                dc.HostToSwitchid[hs.Id] = edgeswitch[switchnum].Id;
                hs1.sw = edgeswitch[switchnum];
                IList<NetworkHost> hslist = hs1.sw.fintimelistHost[0D];
                if (hslist == null)
                {
                    hslist = new List<NetworkHost>();
                    hs1.sw.fintimelistHost[0D] = hslist;
                }
                hslist.Add(hs1);

            }
        }
    }
}