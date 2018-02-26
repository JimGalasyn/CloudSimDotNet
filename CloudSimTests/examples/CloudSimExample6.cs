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
    using BwProvisionerSimple = org.cloudbus.cloudsim.provisioners.BwProvisionerSimple;
    using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using RamProvisionerSimple = org.cloudbus.cloudsim.provisioners.RamProvisionerSimple;
    using System;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// An example showing how to create
    /// scalable simulations.
    /// </summary>
    [TestClass]
    public class CloudSimExample6
    {
        /// <summary>
        /// The cloudlet list. </summary>
        private static IList<Cloudlet> cloudletList;

        /// <summary>
        /// The vmlist. </summary>
        private static IList<Vm> vmlist;

        private static IList<Vm> createVM(int userId, int vms)
        {
            //Creates a container to store VMs. This list is passed to the broker later
            List<Vm> list = new List<Vm>();

            //VM Parameters
            long size = 10000; //image size (MB)
            int ram = 512; //vm memory (MB)
            int mips = 1000;
            long bw = 1000;
            int pesNumber = 1; //number of cpus
            string vmm = "Xen"; //VMM name

            //create VMs
            Vm[] vm = new Vm[vms];

            for (int i = 0; i < vms; i++)
            {
                vm[i] = new Vm(i, userId, mips, pesNumber, ram, bw, size, vmm, new CloudletSchedulerTimeShared());
                //for creating a VM with a space shared scheduling policy for cloudlets:
                //vm[i] = Vm(i, userId, mips, pesNumber, ram, bw, size, priority, vmm, new CloudletSchedulerSpaceShared());

                // TODO: Figure out list.AddLast(vm[i])
                //list.AddLast(vm[i]);
                list.Add(vm[i]);
            }

            return list;
        }
        
        private static IList<Cloudlet> createCloudlet(int userId, int cloudlets)
        {
            // Creates a container to store Cloudlets
            List<Cloudlet> list = new List<Cloudlet>();

            //cloudlet parameters
            long length = 1000;
            long fileSize = 300;
            long outputSize = 300;
            int pesNumber = 1;
            UtilizationModel utilizationModel = new UtilizationModelFull();

            Cloudlet[] cloudlet = new Cloudlet[cloudlets];

            for (int i = 0; i < cloudlets; i++)
            {
                cloudlet[i] = new Cloudlet(i, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel, true);
                // setting the owner of these Cloudlets
                cloudlet[i].UserId = userId;
                // TODO: Figure out list.AddLast
                //list.AddLast(cloudlet[i]);
                list.Add(cloudlet[i]);
            }

            return list;
        }

        /// <summary>
        /// Creates main() to run this example
        /// </summary>
        [TestMethod]
        public void CloudSimExample6Main()
        {
            Log.printLine("Starting CloudSimExample6...");

            try
            {
                // First step: Initialize the CloudSim package. It should be called
                // before creating any entities.
                int num_user = 1; // number of grid users
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

                //Fourth step: Create VMs and Cloudlets and send them to broker
                vmlist = createVM(brokerId, 20); //creating 20 vms
                cloudletList = createCloudlet(brokerId, 40); // creating 40 cloudlets

                broker.submitVmList(vmlist);
                broker.submitCloudletList(cloudletList);

                // Fifth step: Starts the simulation
                CloudSim.startSimulation();

                // Final step: Print results when simulation is over
                IList<Cloudlet> newList = broker.CloudletReceivedListProperty;

                CloudSim.stopSimulation();

                // TODO: TEST: Convert to proper test.
                printCloudletList(newList);


                //                ========== OUTPUT ==========
                //Cloudlet ID STATUS    Data center ID VM ID Time    Start Time    Finish Time
                //     4        SUCCESS        2            4            3        0.2            3.2
                //    16        SUCCESS        2            4            3        0.2            3.2
                //    28        SUCCESS        2            4            3        0.2            3.2
                //     5        SUCCESS        2            5            3        0.2            3.2
                //    17        SUCCESS        2            5            3        0.2            3.2
                //    29        SUCCESS        2            5            3        0.2            3.2
                //     6        SUCCESS        3            6            3        0.2            3.2
                //    18        SUCCESS        3            6            3        0.2            3.2
                //    30        SUCCESS        3            6            3        0.2            3.2
                //     7        SUCCESS        3            7            3        0.2            3.2
                //    19        SUCCESS        3            7            3        0.2            3.2
                //    31        SUCCESS        3            7            3        0.2            3.2
                //     8        SUCCESS        3            8            3        0.2            3.2
                //    20        SUCCESS        3            8            3        0.2            3.2
                //    32        SUCCESS        3            8            3        0.2            3.2
                //    10        SUCCESS        3            10           3        0.2            3.2
                //    22        SUCCESS        3            10           3        0.2            3.2
                //    34        SUCCESS        3            10           3        0.2            3.2
                //     9        SUCCESS        3            9            3        0.2            3.2
                //    21        SUCCESS        3            9            3        0.2            3.2
                //    33        SUCCESS        3            9            3        0.2            3.2
                //    11        SUCCESS        3            11           3        0.2            3.2
                //    23        SUCCESS        3            11           3        0.2            3.2
                //    35        SUCCESS        3            11           3        0.2            3.2
                //     0        SUCCESS        2            0            4        0.2            4.2
                //    12        SUCCESS        2            0            4        0.2            4.2
                //    24        SUCCESS        2            0            4        0.2            4.2
                //    36        SUCCESS        2            0            4        0.2            4.2
                //     1        SUCCESS        2            1            4        0.2            4.2
                //    13        SUCCESS        2            1            4        0.2            4.2
                //    25        SUCCESS        2            1            4        0.2            4.2
                //    37        SUCCESS        2            1            4        0.2            4.2
                //     2        SUCCESS        2            2            4        0.2            4.2
                //    14        SUCCESS        2            2            4        0.2            4.2
                //    26        SUCCESS        2            2            4        0.2            4.2
                //    38        SUCCESS        2            2            4        0.2            4.2
                //     3        SUCCESS        2            3            4        0.2            4.2
                //    15        SUCCESS        2            3            4        0.2            4.2
                //    27        SUCCESS        2            3            4        0.2            4.2
                //    39        SUCCESS        2            3            4        0.2            4.2

                foreach(var testCloudlet in newList)
                {
                    Assert.AreEqual(testCloudlet.CloudletStatus, Cloudlet.SUCCESS);
                    //Assert.AreEqual(testCloudlet.CloudletId, 0);
                    //Assert.AreEqual(testCloudlet.ResourceId, 2);
                    //Assert.AreEqual(testCloudlet.VmId, 0);
                    Assert.IsTrue(Math.Abs(testCloudlet.WallClockTime - 3) <= 0.01 ||
                        Math.Abs(testCloudlet.WallClockTime - 4) <= 0.01);
                    Assert.IsTrue(Math.Abs(testCloudlet.SubmissionTime - 0.2) <= 0.01);
                    Assert.IsTrue(Math.Abs(testCloudlet.FinishTime - 3.2) <= 0.01 ||
                        Math.Abs(testCloudlet.FinishTime - 4.2) <= 0.01);
                }

                Log.printLine("CloudSimExample6 finished!");
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
            int ram = 2048; //host memory (MB)
            long storage = 1000000; //host storage
            int bw = 10000;

            hostList.Add(new Host(hostId, new RamProvisionerSimple(ram), new BwProvisionerSimple(bw), storage, peList1, new VmSchedulerTimeShared(peList1)
                   )); // This is our first machine


            hostId++;

            hostList.Add(new Host(hostId, new RamProvisionerSimple(ram), new BwProvisionerSimple(bw), storage, peList2, new VmSchedulerTimeShared(peList2)
                   )); // Second machine


            //To create a host with a space-shared allocation policy for PEs to VMs:
            //hostList.add(
            //		new Host(
            //			hostId,
            //			new CpuProvisionerSimple(peList1),
            //			new RamProvisionerSimple(ram),
            //			new BwProvisionerSimple(bw),
            //			storage,
            //			new VmSchedulerSpaceShared(peList1)
            //		)
            //	);

            //To create a host with a oportunistic space-shared allocation policy for PEs to VMs:
            //hostList.add(
            //		new Host(
            //			hostId,
            //			new CpuProvisionerSimple(peList1),
            //			new RamProvisionerSimple(ram),
            //			new BwProvisionerSimple(bw),
            //			storage,
            //			new VmSchedulerOportunisticSpaceShared(peList1)
            //		)
            //	);


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
}
