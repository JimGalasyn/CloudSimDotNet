using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.examples.container
{


    /*
	 * Title:        ContainerCloudSimExample1 Toolkit
	 * Description:  ContainerCloudSimExample1 (containerized cloud simulation) Toolkit for Modeling and Simulation
	 *               of Containerized Clouds
	 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
	 *
	 * Copyright (c) 2009, The University of Melbourne, Australia
	 */


    using ContainerBwProvisionerSimple = org.cloudbus.cloudsim.container.containerProvisioners.ContainerBwProvisionerSimple;
    using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
    using ContainerRamProvisionerSimple = org.cloudbus.cloudsim.container.containerProvisioners.ContainerRamProvisionerSimple;
    using ContainerPeProvisionerSimple = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPeProvisionerSimple;
    using ContainerVmBwProvisionerSimple = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmBwProvisionerSimple;
    using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
    using ContainerVmPeProvisionerSimple = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPeProvisionerSimple;
    using ContainerVmRamProvisionerSimple = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmRamProvisionerSimple;
    using org.cloudbus.cloudsim.container.core;
    using HostSelectionPolicy = org.cloudbus.cloudsim.container.hostSelectionPolicies.HostSelectionPolicy;
    using HostSelectionPolicyFirstFit = org.cloudbus.cloudsim.container.hostSelectionPolicies.HostSelectionPolicyFirstFit;
    using PowerContainerVmAllocationPolicyMigrationAbstractHostSelection = org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled.PowerContainerVmAllocationPolicyMigrationAbstractHostSelection;
    using ContainerAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerAllocationPolicy;
    using ContainerVmAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerVmAllocationPolicy;
    using PowerContainerAllocationPolicySimple = org.cloudbus.cloudsim.container.resourceAllocators.PowerContainerAllocationPolicySimple;
    using ContainerCloudletSchedulerDynamicWorkload = org.cloudbus.cloudsim.container.schedulers.ContainerCloudletSchedulerDynamicWorkload;
    using ContainerSchedulerTimeSharedOverSubscription = org.cloudbus.cloudsim.container.schedulers.ContainerSchedulerTimeSharedOverSubscription;
    using ContainerVmSchedulerTimeSharedOverSubscription = org.cloudbus.cloudsim.container.schedulers.ContainerVmSchedulerTimeSharedOverSubscription;
    using IDs = org.cloudbus.cloudsim.container.utils.IDs;
    using PowerContainerVmSelectionPolicy = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicy;
    using PowerContainerVmSelectionPolicyMaximumUsage = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicyMaximumUsage;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text;


    /// <summary>
    /// A simple example showing how to create a data center with one host, one VM, one container and run one cloudlet on it.
    /// </summary>
    [TestClass]
    public class ContainerCloudSimExample1
    {
        /// <summary>
        /// The cloudlet list.
        /// </summary>
        private static IList<ContainerCloudlet> cloudletList;

        /// <summary>
        /// The vmlist.
        /// </summary>
        private static IList<ContainerVm> vmList;

        /// <summary>
        /// The vmlist.
        /// </summary>

        private static IList<Container> containerList;

        /// <summary>
        /// The hostList.
        /// </summary>

        private static IList<ContainerHost> hostList;

        /// <summary>
        /// Creates main() to run this example.
        /// </summary>
        /// <param name="args"> the args </param>
        [TestMethod]
        public void ContainerCloudSimExample1Main()
        {
            Log.printLine("Starting ContainerCloudSimExample1...");

            try
            {
                /// <summary>
                /// number of cloud Users
                /// </summary>
                int num_user = 1;
                /// <summary>
                ///  The fields of calender have been initialized with the current date and time.
                /// </summary>
                DateTime calendar = new DateTime();
                /// <summary>
                /// Deactivating the event tracing
                /// </summary>
                bool trace_flag = false;
                /// <summary>
                /// 1- Like CloudSim the first step is initializing the CloudSim Package before creating any entities.
                /// 
                /// </summary>


                CloudSim.init(num_user, calendar, trace_flag);
                /// <summary>
                /// 2-  Defining the container allocation Policy. This policy determines how Containers are
                /// allocated to VMs in the data center.
                /// 
                /// </summary>


                ContainerAllocationPolicy containerAllocationPolicy = new PowerContainerAllocationPolicySimple();

                /// <summary>
                /// 3-  Defining the VM selection Policy. This policy determines which VMs should be selected for migration
                /// when a host is identified as over-loaded.
                /// 
                /// </summary>

                PowerContainerVmSelectionPolicy vmSelectionPolicy = new PowerContainerVmSelectionPolicyMaximumUsage();


                /// <summary>
                /// 4-  Defining the host selection Policy. This policy determines which hosts should be selected as
                /// migration destination.
                /// 
                /// </summary>
                HostSelectionPolicy hostSelectionPolicy = new HostSelectionPolicyFirstFit();
                /// <summary>
                /// 5- Defining the thresholds for selecting the under-utilized and over-utilized hosts.
                /// </summary>

                double overUtilizationThreshold = 0.80;
                double underUtilizationThreshold = 0.70;
                /// <summary>
                /// 6- The host list is created considering the number of hosts, and host types which are specified
                /// in the <seealso cref="ConstantsExamples"/>.
                /// </summary>
                hostList = new List<ContainerHost>();
                hostList = createHostList(ConstantsExamples.NUMBER_HOSTS);
                cloudletList = new List<ContainerCloudlet>();
                vmList = new List<ContainerVm>();
                /// <summary>
                /// 7- The container allocation policy  which defines the allocation of VMs to containers.
                /// </summary>
                ContainerVmAllocationPolicy vmAllocationPolicy = new PowerContainerVmAllocationPolicyMigrationAbstractHostSelection(hostList, vmSelectionPolicy, hostSelectionPolicy, overUtilizationThreshold, underUtilizationThreshold);
                /// <summary>
                /// 8- The overbooking factor for allocating containers to VMs. This factor is used by the broker for the
                /// allocation process.
                /// </summary>
                int overBookingFactor = 80;
                ContainerDatacenterBroker broker = createBroker(overBookingFactor);
                int brokerId = broker.Id;
                /// <summary>
                /// 9- Creating the cloudlet, container and VM lists for submitting to the broker.
                /// </summary>
                cloudletList = createContainerCloudletList(brokerId, ConstantsExamples.NUMBER_CLOUDLETS);
                containerList = createContainerList(brokerId, ConstantsExamples.NUMBER_CLOUDLETS);
                vmList = createVmList(brokerId, ConstantsExamples.NUMBER_VMS);
                /// <summary>
                /// 10- The address for logging the statistics of the VMs, containers in the data center.
                /// </summary>
                string logAddress = "~/Results";

                PowerContainerDatacenter e = (PowerContainerDatacenter)createDatacenter("datacenter", typeof(PowerContainerDatacenterCM), hostList, vmAllocationPolicy, containerAllocationPolicy, String.Empty, ConstantsExamples.SCHEDULING_INTERVAL, logAddress, ConstantsExamples.VM_STARTTUP_DELAY, ConstantsExamples.CONTAINER_STARTTUP_DELAY);


                /// <summary>
                /// 11- Submitting the cloudlet's , container's , and VM's lists to the broker.
                /// </summary>
                // TODO: Find subList replacement
                //broker.submitCloudletList(cloudletList.subList(0, containerList.Count));
                broker.submitContainerList(containerList);
                broker.submitVmList(vmList);
                /// <summary>
                /// 12- Determining the simulation termination time according to the cloudlet's workload.
                /// </summary>
                CloudSim.terminateSimulation(86400.00);
                /// <summary>
                /// 13- Starting the simualtion.
                /// </summary>
                CloudSim.startSimulation();
                /// <summary>
                /// 14- Stopping the simualtion.
                /// </summary>
                CloudSim.stopSimulation();
                /// <summary>
                /// 15- Printing the results when the simulation is finished.
                /// </summary>
                IList<ContainerCloudlet> newList = broker.CloudletReceivedList;
                printCloudletList(newList);

                Log.printLine("ContainerCloudSimExample1 finished!");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                Log.printLine("Unwanted errors happen");
                throw e;
            }
        }

        /// <summary>
        /// It creates a specific name for the experiment which is used for creating the Log address folder.
        /// </summary>

        private static string getExperimentName(string args)
        {
            StringBuilder experimentName = new StringBuilder();

            for (int i = 0; i < args.Length; ++i)
            {
                //if (!args[i].Empty)
                // TODO: TEST Char.IsWhiteSpace == Empty?
                if (!Char.IsWhiteSpace(args[i]))
                {
                    if (i != 0)
                    {
                        experimentName.Append("_");
                    }

                    experimentName.Append(args[i]);
                }
            }

            return experimentName.ToString();
        }

        /// <summary>
        /// Creates the broker.
        /// </summary>
        /// <param name="overBookingFactor"> </param>
        /// <returns> the datacenter broker </returns>
        private static ContainerDatacenterBroker createBroker(int overBookingFactor)
        {

            ContainerDatacenterBroker broker = null;

            try
            {
                broker = new ContainerDatacenterBroker("Broker", overBookingFactor);
            }
            catch (Exception var2)
            {
                Debug.WriteLine(var2.ToString());
                Debug.WriteLine(var2.StackTrace);
                //Environment.Exit(0);
                throw var2;
            }

            return broker;
        }

        /// <summary>
        /// Prints the Cloudlet objects.
        /// </summary>
        /// <param name="list"> list of Cloudlets </param>
        private static void printCloudletList(IList<ContainerCloudlet> list)
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

                if (cloudlet.CloudletStatusString == "Success")
                {
                    Log.print("SUCCESS");

                    //Log.printLine(indent + indent + cloudlet.ResourceId + indent + indent + indent + cloudlet.VmId + indent + indent + dft.format(cloudlet.ActualCPUTime) + indent + indent + dft.format(cloudlet.ExecStartTime) + indent + indent + dft.format(cloudlet.FinishTime));
                    Log.printLine(indent + indent + cloudlet.ResourceId + indent + indent + indent + cloudlet.VmId + indent + indent + cloudlet.ActualCPUTime + indent + indent + cloudlet.ExecStartTime + indent + indent + cloudlet.FinishTime);
                }
            }
        }

        /// <summary>
        /// Create the Virtual machines and add them to the list
        /// </summary>
        /// <param name="brokerId"> </param>
        /// <param name="containerVmsNumber"> </param>
        private static List<ContainerVm> createVmList(int brokerId, int containerVmsNumber)
        {
            List<ContainerVm> containerVms = new List<ContainerVm>();

            for (int i = 0; i < containerVmsNumber; ++i)
            {
                List<ContainerPe> peList = new List<ContainerPe>();
                int vmType = i / (int)Math.Ceiling((double)containerVmsNumber / 4.0D);
                for (int j = 0; j < ConstantsExamples.VM_PES[vmType]; ++j)
                {
                    peList.Add(new ContainerPe(j, new ContainerPeProvisionerSimple((double)ConstantsExamples.VM_MIPS[vmType])));
                }
                containerVms.Add(new PowerContainerVm(IDs.pollId(typeof(ContainerVm)), brokerId, (double)ConstantsExamples.VM_MIPS[vmType], (float)ConstantsExamples.VM_RAM[vmType], ConstantsExamples.VM_BW, ConstantsExamples.VM_SIZE, "Xen", new ContainerSchedulerTimeSharedOverSubscription(peList), new ContainerRamProvisionerSimple(ConstantsExamples.VM_RAM[vmType]), new ContainerBwProvisionerSimple(ConstantsExamples.VM_BW), peList, ConstantsExamples.SCHEDULING_INTERVAL));


            }

            return containerVms;
        }

        /// <summary>
        /// Create the host list considering the specs listed in the <seealso cref="ConstantsExamples"/>.
        /// </summary>
        /// <param name="hostsNumber">
        /// @return </param>


        public static IList<ContainerHost> createHostList(int hostsNumber)
        {
            List<ContainerHost> hostList = new List<ContainerHost>();
            for (int i = 0; i < hostsNumber; ++i)
            {
                int hostType = i / (int)Math.Ceiling((double)hostsNumber / 3.0D);
                List<ContainerVmPe> peList = new List<ContainerVmPe>();
                for (int j = 0; j < ConstantsExamples.HOST_PES[hostType]; ++j)
                {
                    peList.Add(new ContainerVmPe(j, new ContainerVmPeProvisionerSimple((double)ConstantsExamples.HOST_MIPS[hostType])));
                }

                hostList.Add(new PowerContainerHostUtilizationHistory(IDs.pollId(typeof(ContainerHost)), new ContainerVmRamProvisionerSimple(ConstantsExamples.HOST_RAM[hostType]), new ContainerVmBwProvisionerSimple(1000000L), 1000000L, peList, new ContainerVmSchedulerTimeSharedOverSubscription(peList), ConstantsExamples.HOST_POWER[hostType]));
            }

            return hostList;
        }


        /// <summary>
        /// Create the data center
        /// </summary>
        /// <param name="name"> </param>
        /// <param name="datacenterClass"> </param>
        /// <param name="hostList"> </param>
        /// <param name="vmAllocationPolicy"> </param>
        /// <param name="containerAllocationPolicy"> </param>
        /// <param name="experimentName"> </param>
        /// <param name="logAddress">
        /// @return </param>
        /// <exception cref="Exception"> </exception>

        public static ContainerDatacenter createDatacenter(string name, Type datacenterClass, IList<ContainerHost> hostList, ContainerVmAllocationPolicy vmAllocationPolicy, ContainerAllocationPolicy containerAllocationPolicy, string experimentName, double schedulingInterval, string logAddress, double VMStartupDelay, double ContainerStartupDelay)
        {
            string arch = "x86";
            string os = "Linux";
            string vmm = "Xen";
            double time_zone = 10.0D;
            double cost = 3.0D;
            double costPerMem = 0.05D;
            double costPerStorage = 0.001D;
            double costPerBw = 0.0D;
            ContainerDatacenterCharacteristics characteristics = new ContainerDatacenterCharacteristics(arch, os, vmm, hostList, time_zone, cost, costPerMem, costPerStorage, costPerBw);
            ContainerDatacenter datacenter = new PowerContainerDatacenterCM(name, characteristics, vmAllocationPolicy, containerAllocationPolicy, new List<Storage>(), schedulingInterval, experimentName, logAddress, VMStartupDelay, ContainerStartupDelay);

            return datacenter;
        }

        /// <summary>
        /// create the containers for hosting the cloudlets and binding them together.
        /// </summary>
        /// <param name="brokerId"> </param>
        /// <param name="containersNumber">
        /// @return </param>

        public static IList<Container> createContainerList(int brokerId, int containersNumber)
        {
            List<Container> containers = new List<Container>();

            for (int i = 0; i < containersNumber; ++i)
            {
                int containerType = i / (int)Math.Ceiling((double)containersNumber / 3.0D);

                containers.Add(new PowerContainer(IDs.pollId(typeof(Container)), brokerId, (double)ConstantsExamples.CONTAINER_MIPS[containerType], ConstantsExamples.CONTAINER_PES[containerType], ConstantsExamples.CONTAINER_RAM[containerType], ConstantsExamples.CONTAINER_BW, 0L, "Xen", new ContainerCloudletSchedulerDynamicWorkload(ConstantsExamples.CONTAINER_MIPS[containerType], ConstantsExamples.CONTAINER_PES[containerType]), ConstantsExamples.SCHEDULING_INTERVAL));
            }

            return containers;
        }

        /// <summary>
        /// Creating the cloudlet list that are going to run on containers
        /// </summary>
        /// <param name="brokerId"> </param>
        /// <param name="numberOfCloudlets">
        /// @return </param>
        /// <exception cref="FileNotFoundException"> </exception>
        public static IList<ContainerCloudlet> createContainerCloudletList(int brokerId, int numberOfCloudlets)
        {
            // TODO: File IO
            //string inputFolderName = typeof(ContainerCloudSimExample1).ClassLoader.getResource("workload/planetlab").Path;
            List<ContainerCloudlet> cloudletList = new List<ContainerCloudlet>();
            long fileSize = 300L;
            long outputSize = 300L;
            UtilizationModelNull utilizationModelNull = new UtilizationModelNull();
            // TODO: File IO
            //java.io.File inputFolder1 = new java.io.File(inputFolderName);
            //java.io.File[] files1 = inputFolder1.listFiles();
            int createdCloudlets = 0;
            //foreach (java.io.File aFiles1 in files1)
            for (int j = 0; j < 808; ++j)
            {
                // TODO: File IO
                //java.io.File inputFolder = new java.io.File(aFiles1.ToString());
                //java.io.File[] files = inputFolder.listFiles();
                //for (int i = 0; i < files.Length; ++i)
                for (int i = 0; i < 808; ++i)
                {
                    if (createdCloudlets < numberOfCloudlets)
                    {
                        ContainerCloudlet cloudlet = null;

                        try
                        {
                            //cloudlet = new ContainerCloudlet(IDs.pollId(typeof(ContainerCloudlet)), ConstantsExamples.CLOUDLET_LENGTH, 1, fileSize, outputSize, new UtilizationModelPlanetLabInMemoryExtended(files[i].AbsolutePath, 300.0D), utilizationModelNull, utilizationModelNull);
                            cloudlet = new ContainerCloudlet(IDs.pollId(typeof(ContainerCloudlet)), ConstantsExamples.CLOUDLET_LENGTH, 1, fileSize, outputSize, new UtilizationModelPlanetLabInMemoryExtended("FUBAR", 300.0D), utilizationModelNull, utilizationModelNull);
                        }
                        catch (Exception var13)
                        {
                            Debug.WriteLine(var13.ToString());
                            Debug.WriteLine(var13.StackTrace);
                            //Environment.Exit(0);
                            throw var13;
                        }

                        cloudlet.UserId = brokerId;
                        cloudletList.Add(cloudlet);
                        createdCloudlets += 1;
                    }
                    else
                    {
                        return cloudletList;
                    }
                }
            }

            return cloudletList;
        }
    }
}