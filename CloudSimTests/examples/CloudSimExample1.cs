using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.examples
{

    /*
	 * Title:        CloudSim Toolkit
	 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation
	 *               of Clouds
	 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
	 *
	 * Copyright (c) 2009, The University of Melbourne, Australia
	 */


    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using BwProvisionerSimple = org.cloudbus.cloudsim.provisioners.BwProvisionerSimple;
    using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using RamProvisionerSimple = org.cloudbus.cloudsim.provisioners.RamProvisionerSimple;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;


    /// <summary>
    /// A simple example showing how to create a data center with one host and run one cloudlet on it.
    /// </summary>
    [TestClass]
    public class CloudSimExample1
    {
        /// <summary>
        /// The cloudlet list. </summary>
        private static IList<Cloudlet> cloudletList;
        /// <summary>
        /// The vmlist. </summary>
        private static IList<Vm> vmlist;

        /// <summary>
        /// Creates main() to run this example.
        /// </summary>
        /// <param name="args"> the args </param>
        [TestMethod]
        public void CloudSimExample1Main()
        {
            Log.printLine("Starting CloudSimExample1...");

            // First step: Initialize the CloudSim package. It should be called before creating any entities.
            int num_user = 1; // number of cloud users
            DateTime calendar = new DateTime(); // Calendar whose fields have been initialized with the current date and time.
            bool trace_flag = false; // trace events

            /* Comment Start - Dinesh Bhagwat 
             * Initialize the CloudSim library. 
             * init() invokes initCommonVariable() which in turn calls initialize() (all these 3 methods are defined in CloudSim.cs).
             * initialize() creates two collections - an ArrayList of SimEntity Objects (named entities which denote the simulation entities) and 
             * a LinkedHashMap (named entitiesByName which denote the LinkedHashMap of the same simulation entities), with name of every SimEntity as the key.
             * initialize() creates two queues - a Queue of SimEvents (future) and another Queue of SimEvents (deferred). 
             * initialize() creates a HashMap of of Predicates (with integers as keys) - these predicates are used to select a particular event from the deferred queue. 
             * initialize() sets the simulation clock to 0 and running (a boolean flag) to false.
             * Once initialize() returns (note that we are in method initCommonVariable() now), a CloudSimShutDown (which is derived from SimEntity) instance is created 
             * (with numuser as 1, its name as CloudSimShutDown, id as -1, and state as RUNNABLE). Then this new entity is added to the simulation 
             * While being added to the simulation, its id changes to 0 (from the earlier -1). The two collections - entities and entitiesByName are updated with this SimEntity.
             * the shutdownId (whose default value was -1) is 0    
             * Once initCommonVariable() returns (note that we are in method init() now), a CloudInformationService (which is also derived from SimEntity) instance is created 
             * (with its name as CloudInformatinService, id as -1, and state as RUNNABLE). Then this new entity is also added to the simulation. 
             * While being added to the simulation, the id of the SimEntitiy is changed to 1 (which is the next id) from its earlier value of -1. 
             * The two collections - entities and entitiesByName are updated with this SimEntity.
             * the cisId(whose default value is -1) is 1
             * Comment End - Dinesh Bhagwat 
             */
            CloudSim.init(num_user, calendar, trace_flag);

            // Second step: Create Datacenters
            // Datacenters are the resource providers in CloudSim. We need at
            // list one of them to run a CloudSim simulation
            Datacenter datacenter0 = createDatacenter("Datacenter_0");

            // Third step: Create Broker
            DatacenterBroker broker = createBroker();
            int brokerId = broker.Id;

            // Fourth step: Create one virtual machine
            vmlist = new List<Vm>();

            // VM description
            int vmid = 0;
            int mips = 1000;
            long size = 10000; // image size (MB)
            int ram = 512; // vm memory (MB)
            long bw = 1000;

            int pesNumber = 1; // number of cpus
            string vmm = "Xen"; // VMM name

            // create VM
            Vm vm = new Vm(vmid, brokerId, mips, pesNumber, ram, bw, size, vmm, new CloudletSchedulerTimeShared());

            // add the VM to the vmList
            vmlist.Add(vm);

            // submit vm list to the broker
            broker.submitVmList(vmlist);

            // Fifth step: Create one Cloudlet
            cloudletList = new List<Cloudlet>();

            // Cloudlet properties
            int id = 0;
            long length = 400000;
            long fileSize = 300;
            long outputSize = 300;
            UtilizationModel utilizationModel = new UtilizationModelFull();

            Cloudlet cloudlet = new Cloudlet(id, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel);
            cloudlet.UserId = brokerId;
            cloudlet.VmId = vmid;

            // add the cloudlet to the list
            cloudletList.Add(cloudlet);

            // submit cloudlet list to the broker
            broker.submitCloudletList(cloudletList);

            // Sixth step: Starts the simulation
            CloudSim.startSimulation();

            CloudSim.stopSimulation();

            //Final step: Print results when simulation is over
            IList<Cloudlet> newList = broker.CloudletReceivedListProperty;

            //========== OUTPUT ==========
            //Cloudlet ID | STATUS  |  Data center ID | VM ID | Time  |  Start Time  |  Finish Time
            //    0       | SUCCESS |        2        |   0   | 400.0 |     0.1      |     400.1

            var testCloudlet = newList[0];
            Assert.AreEqual(testCloudlet.CloudletStatus, Cloudlet.SUCCESS);
            Assert.AreEqual(testCloudlet.CloudletId, 0);
            Assert.AreEqual(testCloudlet.ResourceId, 2);
            Assert.AreEqual(testCloudlet.VmId, 0);
            Assert.IsTrue(Math.Abs(testCloudlet.WallClockTime - 400) <= 0.01);
            Assert.IsTrue(Math.Abs(testCloudlet.SubmissionTime - 0.1) <= 0.01);
            //Assert.IsTrue(Math.Abs(testCloudlet.ActualCPUTime - 400) <= 0.01);
            //Assert.IsTrue(Math.Abs(testCloudlet.ExecStartTime - 0.1) <= 0.01);
            Assert.IsTrue(Math.Abs(testCloudlet.FinishTime - 400.1) <= 0.01);
        }

        /// <summary>
        /// Creates the datacenter.
        /// </summary>
        /// <param name="name"> the name
        /// </param>
        /// <returns> the datacenter </returns>
        private static Datacenter createDatacenter(string name)
        {

            // Here are the steps needed to create a PowerDatacenter:
            // 1. We need to create a list to store
            // our machine
            IList<Host> hostList = new List<Host>();

            // 2. A Machine contains one or more PEs or CPUs/Cores.
            // In this example, it will have only one core.
            IList<Pe> peList = new List<Pe>();

            int mips = 1000;

            // 3. Create PEs and add these into a list.
            peList.Add(new Pe(0, new PeProvisionerSimple(mips))); // need to store Pe id and MIPS Rating

            // 4. Create Host with its id and list of PEs and add them to the list
            // of machines
            int hostId = 0;
            int ram = 2048; // host memory (MB)
            long storage = 1000000; // host storage
            int bw = 10000;

            hostList.Add(new Host(hostId, new RamProvisionerSimple(ram), new BwProvisionerSimple(bw), storage, peList, new VmSchedulerTimeShared(peList)
               )); // This is our machine

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
            List<Storage> storageList = new List<Storage>(); // we are not adding SAN
                                                             // devices by now

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

        // We strongly encourage users to develop their own broker policies, to
        // submit vms and cloudlets according
        // to the specific rules of the simulated scenario
        /// <summary>
        /// Creates the broker.
        /// </summary>
        /// <returns> the datacenter broker </returns>
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
            }
            return broker;
        }
    }
}