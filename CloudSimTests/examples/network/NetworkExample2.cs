﻿using System;
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
    /// two datacenters with one host and a
    /// network topology each and run two cloudlets
    /// on them.
    /// </summary>
    [TestClass]
    public class NetworkExample2
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
        public async Task NetworkExample2Main()
        {
            Log.printLine("Starting NetworkExample2...");

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
            Datacenter datacenter1 = createDatacenter("Datacenter_1");

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

            //create two VMs
            Vm vm1 = new Vm(vmid, brokerId, mips, pesNumber, ram, bw, size, vmm, new CloudletSchedulerTimeShared());

            //the second VM will have twice the priority of VM1 and so will receive twice CPU time
            vmid++;
            Vm vm2 = new Vm(vmid, brokerId, mips, pesNumber, ram, bw, size, vmm, new CloudletSchedulerTimeShared());
            //add the VMs to the vmList
            vmlist.Add(vm1);
            vmlist.Add(vm2);

            //submit vm list to the broker
            broker.submitVmList(vmlist);


            //Fifth step: Create two Cloudlets
            cloudletList = new List<Cloudlet>();

            //Cloudlet properties
            int id = 0;
            long length = 40000;
            long fileSize = 300;
            long outputSize = 300;
            UtilizationModel utilizationModel = new UtilizationModelFull();

            Cloudlet cloudlet1 = new Cloudlet(id, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel, true);
            cloudlet1.UserId = brokerId;

            id++;
            Cloudlet cloudlet2 = new Cloudlet(id, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel, true);
            cloudlet2.UserId = brokerId;

            //add the cloudlets to the list
            cloudletList.Add(cloudlet1);
            cloudletList.Add(cloudlet2);

            //submit cloudlet list to the broker
            broker.submitCloudletList(cloudletList);


            //bind the cloudlets to the vms. This way, the broker
            // will submit the bound cloudlets only to the specific VM
            broker.bindCloudletToVm(cloudlet1.CloudletId, vm1.Id);
            broker.bindCloudletToVm(cloudlet2.CloudletId, vm2.Id);


            //Sixth step: configure network
            //load the network topology file
            await NetworkTopology.buildNetworkTopology(@".\resources\topology.brite");

            //maps CloudSim entities to BRITE entities
            //Datacenter0 will correspond to BRITE node 0
            int briteNode = 0;
            NetworkTopology.mapNode(datacenter0.Id, briteNode);

            //Datacenter1 will correspond to BRITE node 2
            briteNode = 2;
            NetworkTopology.mapNode(datacenter1.Id, briteNode);

            //Broker will correspond to BRITE node 3
            briteNode = 3;
            NetworkTopology.mapNode(broker.Id, briteNode);


            // Sixth step: Starts the simulation
            CloudSim.startSimulation();


            // Final step: Print results when simulation is over
            IList<Cloudlet> newList = broker.CloudletReceivedListProperty;

            CloudSim.stopSimulation();

            //                ========== OUTPUT ==========
            //Cloudlet ID STATUS    Data center ID VM ID Time    Start Time    Finish Time
            //    0        SUCCESS        2            0        160        21.8        181.8
            //    1        SUCCESS        2            1        160        21.8        181.8

            var testCloudlet1 = newList[0];
            Assert.AreEqual(testCloudlet1.CloudletStatus, Cloudlet.SUCCESS);
            Assert.AreEqual(testCloudlet1.CloudletId, 0);
            Assert.AreEqual(testCloudlet1.ResourceId, 2);
            Assert.AreEqual(testCloudlet1.VmId, 0);
            Assert.IsTrue(Math.Abs(testCloudlet1.WallClockTime - 160) <= 0.01);
            Assert.IsTrue(Math.Abs(testCloudlet1.SubmissionTime - 21.8) <= 0.01);
            Assert.IsTrue(Math.Abs(testCloudlet1.FinishTime - 181.8) <= 0.01);

            var testCloudlet2 = newList[1];
            Assert.AreEqual(testCloudlet2.CloudletStatus, Cloudlet.SUCCESS);
            Assert.AreEqual(testCloudlet2.CloudletId, 1);
            Assert.AreEqual(testCloudlet2.ResourceId, 2);
            Assert.AreEqual(testCloudlet2.VmId, 1);
            Assert.IsTrue(Math.Abs(testCloudlet2.WallClockTime - 160) <= 0.01);
            Assert.IsTrue(Math.Abs(testCloudlet2.SubmissionTime - 21.8) <= 0.01);
            Assert.IsTrue(Math.Abs(testCloudlet2.FinishTime - 181.8) <= 0.01);
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

            // 3. Create PEs and add these into a list
            peList.Add(new Pe(0, new PeProvisionerSimple(mips))); // need to store Pe id and MIPS Rating

            //4. Create Host with its id and list of PEs and add them to the list of machines
            int hostId = 0;
            int ram = 2048; //host memory (MB)
            long storage = 1000000; //host storage
            int bw = 10000;
            //in this example, the VMAllocatonPolicy in use is Time Shared with priorities. It means that VMs
            //receive time shares accroding to their priority.
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
                throw e;
                //return null;
            }
            return broker;
        }
    }
}