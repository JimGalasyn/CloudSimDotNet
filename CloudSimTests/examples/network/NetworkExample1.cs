using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation
 *               of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.examples.network
{
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using BwProvisionerSimple = org.cloudbus.cloudsim.provisioners.BwProvisionerSimple;
    using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using RamProvisionerSimple = org.cloudbus.cloudsim.provisioners.RamProvisionerSimple;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;

    /// <summary>
    /// A simple example showing how to create
    /// a datacenter with one host and a network
    /// topology and and run one cloudlet on it.
    /// </summary>
    [TestClass]
    public class NetworkExample1
    {
        /// <summary>
        /// The cloudlet list. </summary>
        private static IList<Cloudlet> cloudletList;

        /// <summary>
        /// The vmlist. </summary>
        private static IList<Vm> vmlist;

        /// <summary>
        /// Creates main() to run this example
        /// </summary>
        [TestMethod]
        public async Task NetworkExample1Main()
        {
            Log.printLine("Starting NetworkExample1...");

            // First step: Initialize the CloudSim package. It should be called
            // before creating any entities.
            int num_user = 1; // number of cloud users
            DateTime calendar = new DateTime();
            bool trace_flag = false; // mean trace events

            // Initialize the CloudSim library
            CloudSim.init(num_user, calendar, trace_flag);

            // Second step: Create Datacenters
            //Datacenters are the resource providers in CloudSim. We need at list one of them to run a CloudSim simulation
            Datacenter datacenter0 = createDatacenter("Datacenter_0");

            //Third step: Create Broker
            DatacenterBroker broker = createBroker();
            int brokerId = broker.Id;

            //Fourth step: Create one virtual machine
            vmlist = new List<Vm>();

            //VM description
            int vmid = 0;
            int mips = 250;
            long size = 10000; //image size (MB)
            int ram = 512; //vm memory (MB)
            long bw = 1000;
            int pesNumber = 1; //number of cpus
            string vmm = "Xen"; //VMM name

            //create VM
            Vm vm1 = new Vm(vmid, brokerId, mips, pesNumber, ram, bw, size, vmm, new CloudletSchedulerTimeShared());

            //add the VM to the vmList
            vmlist.Add(vm1);

            //submit vm list to the broker
            broker.submitVmList(vmlist);


            //Fifth step: Create one Cloudlet
            cloudletList = new List<Cloudlet>();
            //Cloudlet properties
            int id = 0;
            long length = 40000;
            long fileSize = 300;
            long outputSize = 300;
            UtilizationModel utilizationModel = new UtilizationModelFull();

            Cloudlet cloudlet1 = new Cloudlet(id, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel);
            cloudlet1.UserId = brokerId;

            //add the cloudlet to the list
            cloudletList.Add(cloudlet1);

            //submit cloudlet list to the broker
            broker.submitCloudletList(cloudletList);

            //Sixth step: configure network
            //load the network topology file
            //await NetworkTopology.buildNetworkTopology(@".\examples\network\topology.brite");
            await NetworkTopology.buildNetworkTopology(@".\resources\topology.brite");
            
            //maps CloudSim entities to BRITE entities
            //PowerDatacenter will correspond to BRITE node 0
            int briteNode = 0;
            NetworkTopology.mapNode(datacenter0.Id, briteNode);

            //Broker will correspond to BRITE node 3
            briteNode = 3;
            NetworkTopology.mapNode(broker.Id, briteNode);


            // Seventh step: Starts the simulation
            CloudSim.startSimulation();


            // Final step: Print results when simulation is over
            IList<Cloudlet> newList = broker.CloudletReceivedListProperty;

            CloudSim.stopSimulation();

            // ========== OUTPUT ==========
            // Cloudlet ID STATUS    Data center ID VM ID Time    Start Time    Finish Time
            //     0        SUCCESS        2            0        160        19.6        179.6

            var cloudlet = newList[0];
            Assert.AreEqual(cloudlet.CloudletStatus, Cloudlet.SUCCESS);
            Assert.AreEqual(cloudlet.ResourceId, 2);
            Assert.AreEqual(cloudlet.VmId, 0);
            Assert.AreEqual(cloudlet.WallClockTime, 160);
            Assert.IsTrue(Math.Abs(cloudlet.SubmissionTime - 19.6) <= 0.01);
            Assert.IsTrue(Math.Abs(cloudlet.FinishTime - 179.6) <= 0.01);
        }

        private static Datacenter createDatacenter(string name)
        {

            // Here are the steps needed to create a PowerDatacenter:
            // 1. We need to create a list to store
            //    our machine
            IList<Host> hostList = new List<Host>();

            // 2. A Machine contains one or more PEs or CPUs/Cores.
            // In this example, it will have only one core.
            IList<Pe> peList = new List<Pe>();

            int mips = 1000;

            // 3. Create PEs and add these into a list.
            peList.Add(new Pe(0, new PeProvisionerSimple(mips))); // need to store Pe id and MIPS Rating

            //4. Create Host with its id and list of PEs and add them to the list of machines
            int hostId = 0;
            int ram = 2048; //host memory (MB)
            long storage = 1000000; //host storage
            int bw = 10000;

            hostList.Add(new Host(hostId, new RamProvisionerSimple(ram), new BwProvisionerSimple(bw), storage, peList, new VmSchedulerTimeShared(peList)
                   )); // This is our machine

            // 5. Create a DatacenterCharacteristics object that stores the
            //    properties of a data center: architecture, OS, list of
            //    Machines, allocation policy: time- or space-shared, time zone
            //    and its price (G$/Pe time unit).
            string arch = "x86"; // system architecture
            string os = "Linux"; // operating system
            string vmm = "Xen";
            double time_zone = 10.0; // time zone this resource located
            double cost = 3.0; // the cost of using processing in this resource
            double costPerMem = 0.05; // the cost of using memory in this resource
            double costPerStorage = 0.001; // the cost of using storage in this resource
            double costPerBw = 0.0; // the cost of using bw in this resource
            List<Storage> storageList = new List<Storage>(); //we are not adding SAN devices by now
            DatacenterCharacteristics characteristics = new DatacenterCharacteristics(arch, os, vmm, hostList, time_zone, cost, costPerMem, costPerStorage, costPerBw);


            // 6. Finally, we need to create a PowerDatacenter object.
            Datacenter datacenter = null;
            try
            {
                datacenter = new Datacenter(name, characteristics, new VmAllocationPolicySimple(hostList), storageList, 0);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                throw e;
            }

            return datacenter;
        }

        //We strongly encourage users to develop their own broker policies, to submit vms and cloudlets according
        //to the specific rules of the simulated scenario
        private static DatacenterBroker createBroker()
        {

            DatacenterBroker broker = null;
            try
            {
                broker = new DatacenterBroker("Broker");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                return null;
            }
            return broker;
        }
    }
}