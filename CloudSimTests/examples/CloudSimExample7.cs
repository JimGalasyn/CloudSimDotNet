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
    using System.Threading;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// An example showing how to pause and resume the simulation,
    /// and create simulation entities (a DatacenterBroker in this example)
    /// dynamically.
    /// </summary>
    [TestClass]
    public class CloudSimExample7
    {
        /// <summary>
        /// The cloudlet list. </summary>
        private static IList<Cloudlet> cloudletList;

        /// <summary>
        /// The vmlist. </summary>
        private static IList<Vm> vmlist;

        private static IList<Vm> createVM(int userId, int vms, int idShift)
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
                // TODO: list.AddLast
                //list.AddLast(vm[i]);
                list.Add(vm[i]);
            }

            return list;
        }

        private static IList<Cloudlet> createCloudlet(int userId, int cloudlets, int idShift)
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
                cloudlet[i] = new Cloudlet(idShift + i, length, pesNumber, fileSize, outputSize, utilizationModel, utilizationModel, utilizationModel);
                // setting the owner of these Cloudlets
                cloudlet[i].UserId = userId;
                // TODO: TEST list.AddLast
                //list.AddLast(cloudlet[i]);
                list.Add(cloudlet[i]);
            }

            return list;
        }

        /// <summary>
        /// Creates main() to run this example
        /// </summary>
        [TestMethod]
        public void CloudSimExample7Main()
        {
            Log.printLine("Starting CloudSimExample7...");

            try
            {
                // First step: Initialize the CloudSim package. It should be called
                // before creating any entities.
                int num_user = 2; // number of grid users
                DateTime calendar = new DateTime();
                bool trace_flag = false; // mean trace events

                // Initialize the CloudSim library
                CloudSim.init(num_user, calendar, trace_flag);

                // Second step: Create Datacenters
                //Datacenters are the resource providers in CloudSim. We need at list one of them to run a CloudSim simulation
                Datacenter datacenter0 = createDatacenter("Datacenter_0");
                Datacenter datacenter1 = createDatacenter("Datacenter_1");

                //Third step: Create Broker
                DatacenterBroker broker = createBroker("Broker_0");
                int brokerId = broker.Id;

                //Fourth step: Create VMs and Cloudlets and send them to broker
                vmlist = createVM(brokerId, 5, 0); //creating 5 vms
                cloudletList = createCloudlet(brokerId, 10, 0); // creating 10 cloudlets

                broker.submitVmList(vmlist);
                broker.submitCloudletList(cloudletList);

                // TODO: Spin up another thread to create a new broker at 200 clock time
                // A thread that will create a new broker at 200 clock time
                //Runnable monitor = () =>
                //{
                //    CloudSim.pauseSimulation(200);
                //    while (true)
                //    {
                //        if (CloudSim.Paused)
                //        {
                //            break;
                //        }
                //        try
                //        {
                //            Thread.Sleep(100);
                //        }
                //        catch (InterruptedException e)
                //        {
                //            Console.WriteLine(e.ToString());
                //            Console.Write(e.StackTrace);
                //        }
                //    }

                //    Log.printLine("\n\n\n" + CloudSim.clock() + ": The simulation is paused for 5 sec \n\n");

                //    try
                //    {
                //        Thread.Sleep(5000);
                //    }
                //    catch (InterruptedException e)
                //    {
                //        Console.WriteLine(e.ToString());
                //        Console.Write(e.StackTrace);
                //    }

                //    DatacenterBroker broker = createBroker("Broker_1");
                //    int brokerId = broker.Id;

                //    //Create VMs and Cloudlets and send them to broker
                //    vmlist = createVM(brokerId, 5, 100); //creating 5 vms
                //    cloudletList = createCloudlet(brokerId, 10, 100); // creating 10 cloudlets

                //    broker.submitVmList(vmlist);
                //    broker.submitCloudletList(cloudletList);

                //    CloudSim.resumeSimulation();
                //};

                //(new Thread(monitor)).Start();
                Thread.Sleep(1000);

                // Fifth step: Starts the simulation
                CloudSim.startSimulation();

                // Final step: Print results when simulation is over
                IList<Cloudlet> newList = broker.CloudletReceivedListProperty;

                CloudSim.stopSimulation();

                printCloudletList(newList);

                Log.printLine("CloudSimExample7 finished!");

                //========== OUTPUT ==========
                //Cloudlet ID STATUS    Data center ID VM ID Time    Start Time    Finish Time
                //    0        SUCCESS        2            0            320        0.1            320.1
                //    5        SUCCESS        2            0            320        0.1            320.1
                //    1        SUCCESS        2            1            320        0.1            320.1
                //    6        SUCCESS        2            1            320        0.1            320.1
                //    2        SUCCESS        2            2            320        0.1            320.1
                //    7        SUCCESS        2            2            320        0.1            320.1
                //    4        SUCCESS        2            4            320        0.1            320.1
                //    9        SUCCESS        2            4            320        0.1            320.1
                //    3        SUCCESS        2            3            320        0.1            320.1
                //    8        SUCCESS        2            3            320        0.1            320.1

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
        private static DatacenterBroker createBroker(string name)
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
}
