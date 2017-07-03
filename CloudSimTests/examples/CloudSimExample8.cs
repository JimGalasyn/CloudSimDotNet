using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation
 *               of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.examples
{
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using SimEntity = org.cloudbus.cloudsim.core.SimEntity;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
    using BwProvisionerSimple = org.cloudbus.cloudsim.provisioners.BwProvisionerSimple;
    using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using RamProvisionerSimple = org.cloudbus.cloudsim.provisioners.RamProvisionerSimple;
    using System;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// An example showing how to create simulation entities
    /// (a DatacenterBroker in this example) in run-time using
    /// a globar manager entity (GlobalBroker).
    /// </summary>
    [TestClass]
    public class CloudSimExample8
    {
        /// <summary>
        /// The cloudlet list. </summary>
        private static IList<Cloudlet> cloudletList;

        /// <summary>
        /// The vmList. </summary>
        private static IList<Vm> vmList;

        internal static IList<Vm> createVM(int userId, int vms, int idShift)
        {
            //Creates a container to store VMs. This list is passed to the broker later
            List<Vm> list = new List<Vm>();

            //VM Parameters
            long size = 10000; //image size (MB)
            int ram = 512; //vm memory (MB)
            int mips = 250;
            long bw = 1000;
            int pesNumber = 1; //number of cpus
            string vmm = "Xen"; //VMM name

            //create VMs
            Vm[] vm = new Vm[vms];

            for (int i = 0; i < vms; i++)
            {
                vm[i] = new Vm(idShift + i, userId, mips, pesNumber, ram, bw, size, vmm, new CloudletSchedulerTimeShared());
                // TODO: TEST list.AddLast(vm[i]);
                //list.AddLast(vm[i]);
                list.Add(vm[i]);
            }

            return list;
        }

        internal static IList<Cloudlet> createCloudlet(int userId, int cloudlets, int idShift)
        {
            // Creates a container to store Cloudlets
            List<Cloudlet> list = new List<Cloudlet>();

            //cloudlet parameters
            long length = 40000;
            long fileSize = 300;
            long outputSize = 300;
            int pesNumber = 1;
            UtilizationModel utilizationModel = new UtilizationModelFull();

            Cloudlet[] cloudlet = new Cloudlet[cloudlets];

            for (int i = 0; i < cloudlets; i++)
            {
                cloudlet[i] = new Cloudlet(idShift + i, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel, true);
                // setting the owner of these Cloudlets
                cloudlet[i].UserId = userId;
                // TODO: TEST list.AddLast(cloudlet[i]);
                //list.AddLast(cloudlet[i]);
                list.Add(cloudlet[i]);
            }

            return list;
        }

        /// <summary>
        /// Creates main() to run this example
        /// </summary>
        [TestMethod]
        public void CloudSimExample8Main()
        {
            Log.printLine("Starting CloudSimExample8...");

            try
            {
                // First step: Initialize the CloudSim package. It should be called
                // before creating any entities.
                int num_user = 2; // number of grid users
                DateTime calendar = new DateTime();
                bool trace_flag = false; // mean trace events

                // Initialize the CloudSim library
                CloudSim.init(num_user, calendar, trace_flag);

                GlobalBroker globalBroker = new GlobalBroker("GlobalBroker");

                // Second step: Create Datacenters
                //Datacenters are the resource providers in CloudSim. We need at list one of them to run a CloudSim simulation
                Datacenter datacenter0 = createDatacenter("Datacenter_0");
                Datacenter datacenter1 = createDatacenter("Datacenter_1");

                //Third step: Create Broker
                DatacenterBroker broker = createBroker("Broker_0");
                int brokerId = broker.Id;

                //Fourth step: Create VMs and Cloudlets and send them to broker
                vmList = createVM(brokerId, 5, 0); //creating 5 vms
                cloudletList = createCloudlet(brokerId, 10, 0); // creating 10 cloudlets

                broker.submitVmList(vmList);
                broker.submitCloudletList(cloudletList);

                // Fifth step: Starts the simulation
                CloudSim.startSimulation();

                // Final step: Print results when simulation is over
                IList<Cloudlet> newList = broker.CloudletReceivedListProperty;
                ((List<Cloudlet>)newList).AddRange(globalBroker.Broker.CloudletReceivedListProperty);

                CloudSim.stopSimulation();

                printCloudletList(newList);

                //========== OUTPUT ==========
                //Cloudlet ID STATUS    Data center ID VM ID Time    Start Time    Finish Time
                //      0      SUCCESS        3          0    320        0.1            320.1
                //      5      SUCCESS        3          0    320        0.1            320.1
                //      1      SUCCESS        3          1    320        0.1            320.1
                //      6      SUCCESS        3          1    320        0.1            320.1
                //      2      SUCCESS        3          2    320        0.1            320.1
                //      7      SUCCESS        3          2    320        0.1            320.1
                //      4      SUCCESS        3          4    320        0.1            320.1
                //      9      SUCCESS        3          4    320        0.1            320.1
                //      3      SUCCESS        3          3    320        0.1            320.1
                //      8      SUCCESS        3          3    320        0.1            320.1
                //    101      SUCCESS        3        101    320        200.1            520.1
                //    106      SUCCESS        3        101    320        200.1            520.1
                //    103      SUCCESS        3        103    320        200.1            520.1
                //    108      SUCCESS        3        103    320        200.1            520.1
                //    100      SUCCESS        3        100    320        200.1            520.1
                //    105      SUCCESS        3        100    320        200.1            520.1
                //    102      SUCCESS        3        102    320        200.1            520.1
                //    107      SUCCESS        3        102    320        200.1            520.1
                //    104      SUCCESS        3        104    320        200.1            520.1
                //    109      SUCCESS        3        104    320        200.1            520.1

                foreach(var testCloudlet in newList)
                {
                    Assert.AreEqual(testCloudlet.CloudletStatus, Cloudlet.SUCCESS);
                    //Assert.AreEqual(testCloudlet.CloudletId, 0);
                    //Assert.AreEqual(testCloudlet.ResourceId, 2);
                    //Assert.AreEqual(testCloudlet.VmId, 0);
                    Assert.IsTrue(Math.Abs(testCloudlet.WallClockTime - 320) <= 0.01);
                    Assert.IsTrue(Math.Abs(testCloudlet.SubmissionTime - 0.1) <= 0.01 ||
                        Math.Abs(testCloudlet.SubmissionTime - 200.1) <= 0.01);
                    Assert.IsTrue(Math.Abs(testCloudlet.FinishTime - 320.1) <= 0.01 ||
                        Math.Abs(testCloudlet.FinishTime - 520.1) <= 0.01);
                }


                Log.printLine("CloudSimExample8 finished!");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                Log.printLine("The simulation has been terminated due to an unexpected error");
                throw e;
            }
        }

        private static Datacenter createDatacenter(string name)
        {
            // Here are the steps needed to create a PowerDatacenter:
            // 1. We need to create a list to store one or more
            //    Machines
            IList<Host> hostList = new List<Host>();

            // 2. A Machine contains one or more PEs or CPUs/Cores. Therefore, should
            //    create a list to store these PEs before creating
            //    a Machine.
            IList<Pe> peList1 = new List<Pe>();

            int mips = 1000;

            // 3. Create PEs and add these into the list.
            //for a quad-core machine, a list of 4 PEs is required:
            peList1.Add(new Pe(0, new PeProvisionerSimple(mips))); // need to store Pe id and MIPS Rating
            peList1.Add(new Pe(1, new PeProvisionerSimple(mips)));
            peList1.Add(new Pe(2, new PeProvisionerSimple(mips)));
            peList1.Add(new Pe(3, new PeProvisionerSimple(mips)));

            //Another list, for a dual-core machine
            IList<Pe> peList2 = new List<Pe>();

            peList2.Add(new Pe(0, new PeProvisionerSimple(mips)));
            peList2.Add(new Pe(1, new PeProvisionerSimple(mips)));

            //4. Create Hosts with its id and list of PEs and add them to the list of machines
            int hostId = 0;
            int ram = 16384; //host memory (MB)
            long storage = 1000000; //host storage
            int bw = 10000;

            hostList.Add(new Host(hostId, new RamProvisionerSimple(ram), new BwProvisionerSimple(bw), storage, peList1, new VmSchedulerTimeShared(peList1)
                   )); // This is our first machine

            hostId++;

            hostList.Add(new Host(hostId, new RamProvisionerSimple(ram), new BwProvisionerSimple(bw), storage, peList2, new VmSchedulerTimeShared(peList2)
                   )); // Second machine

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
            double costPerStorage = 0.1; // the cost of using storage in this resource
            double costPerBw = 0.1; // the cost of using bw in this resource
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
        internal static DatacenterBroker createBroker(string name)
        {
            DatacenterBroker broker = null;
            try
            {
                broker = new DatacenterBroker(name);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                throw e;
            }
            return broker;
        }

        /// <summary>
        /// Prints the Cloudlet objects </summary>
        /// <param name="list">  list of Cloudlets </param>
        private static void printCloudletList(IList<Cloudlet> list)
        {
            int size = list.Count;
            Cloudlet cloudlet;

            string indent = "    ";
            Log.printLine();
            Log.printLine("========== OUTPUT ==========");
            Log.printLine("Cloudlet ID" + indent + "STATUS" + indent + "Data center ID" + indent + "VM ID" + indent + indent + "Time" + indent + "Start Time" + indent + "Finish Time");

            // TODO: DecimalFormat 
            //DecimalFormat dft = new DecimalFormat("###.##");
            for (int i = 0; i < size; i++)
            {
                cloudlet = list[i];
                Log.print(indent + cloudlet.CloudletId + indent + indent);

                if (cloudlet.CloudletStatus == Cloudlet.SUCCESS)
                {
                    Log.print("SUCCESS");

                    //Log.printLine(indent + indent + cloudlet.ResourceId + indent + indent + indent + cloudlet.VmId + indent + indent + indent + dft.format(cloudlet.ActualCPUTime) + indent + indent + dft.format(cloudlet.ExecStartTime) + indent + indent + indent + dft.format(cloudlet.FinishTime));
                    Log.printLine(indent + indent + cloudlet.ResourceId + indent + indent + indent + cloudlet.VmId + indent + indent + indent + cloudlet.ActualCPUTime + indent + indent + cloudlet.ExecStartTime + indent + indent + indent + cloudlet.FinishTime);
                }
            }
        }
    }

    public class GlobalBroker : SimEntity
    {
        private const int CREATE_BROKER = 0;
        private IList<Vm> vmList;
        private IList<Cloudlet> cloudletList;
        private DatacenterBroker broker;

        public GlobalBroker(string name) : base(name)
        {   
        }

        public override void processEvent(SimEvent ev)
        {
            switch (ev.Tag)
            {
                case CREATE_BROKER:
                    Broker = CloudSimExample8.createBroker(base.Name + "_");

                    //Create VMs and Cloudlets and send them to broker
                    VmList = CloudSimExample8.createVM(Broker.Id, 5, 100); //creating 5 vms
                    CloudletList = CloudSimExample8.createCloudlet(Broker.Id, 10, 100); // creating 10 cloudlets

                    broker.submitVmList(VmList);
                    broker.submitCloudletList(CloudletList);

                    CloudSim.resumeSimulation();

                    break;

                default:
                    Log.printLine(Name + ": unknown event type");
                    break;
            }
        }

        public override void startEntity()
        {
            Log.printLine(base.Name + " is starting...");
            schedule(Id, 200, CREATE_BROKER);
        }

        public override void shutdownEntity()
        {
        }

        public virtual IList<Vm> VmList
        {
            get
            {
                return vmList;
            }
            set
            {
                this.vmList = value;
            }
        }

        public virtual IList<Cloudlet> CloudletList
        {
            get
            {
                return cloudletList;
            }
            set
            {
                this.cloudletList = value;
            }
        }

        public virtual DatacenterBroker Broker
        {
            get
            {
                return broker;
            }
            set
            {
                this.broker = value;
            }
        }
    }
}