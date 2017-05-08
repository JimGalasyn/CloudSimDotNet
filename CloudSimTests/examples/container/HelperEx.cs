using System;
using System.Collections;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.examples.container
{
    // TODO: CSVWriter replacement
    //using CSVWriter = com.opencsv.CSVWriter;
    using org.cloudbus.cloudsim;
    using ContainerBwProvisionerSimple = org.cloudbus.cloudsim.container.containerProvisioners.ContainerBwProvisionerSimple;
    using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
    using ContainerRamProvisionerSimple = org.cloudbus.cloudsim.container.containerProvisioners.ContainerRamProvisionerSimple;
    using ContainerPeProvisionerSimple = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPeProvisionerSimple;
    using ContainerVmBwProvisionerSimple = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmBwProvisionerSimple;
    using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
    using ContainerVmPeProvisionerSimple = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPeProvisionerSimple;
    using ContainerVmRamProvisionerSimple = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmRamProvisionerSimple;
    using org.cloudbus.cloudsim.container.core;
    using PowerContainerVmAllocationPolicyMigrationAbstract = org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled.PowerContainerVmAllocationPolicyMigrationAbstract;
    using ContainerAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerAllocationPolicy;
    using ContainerVmAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerVmAllocationPolicy;
    using ContainerCloudletSchedulerDynamicWorkload = org.cloudbus.cloudsim.container.schedulers.ContainerCloudletSchedulerDynamicWorkload;
    using ContainerSchedulerTimeSharedOverSubscription = org.cloudbus.cloudsim.container.schedulers.ContainerSchedulerTimeSharedOverSubscription;
    using ContainerVmSchedulerTimeSharedOverSubscription = org.cloudbus.cloudsim.container.schedulers.ContainerVmSchedulerTimeSharedOverSubscription;
    using IDs = org.cloudbus.cloudsim.container.utils.IDs;
    using MathUtil = org.cloudbus.cloudsim.util.MathUtil;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// This is the modified version of <seealso cref="org.cloudbus.cloudsim.examples.power.Helper"/> class in cloudsim.
    /// Created by sareh on 15/07/15.
    /// </summary>

    public class HelperEx
    {
        public HelperEx()
        {
            //        System.out.print();

        }

        public static IList<ContainerCloudlet> createContainerCloudletList(int brokerId, string inputFolderName, int numberOfCloudlets)
        {
            //ArrayList cloudletList = new ArrayList();
            List<ContainerCloudlet> cloudletList = new List<ContainerCloudlet>();
            long fileSize = 300L;
            long outputSize = 300L;
            UtilizationModelNull utilizationModelNull = new UtilizationModelNull();
            // TODO: File IO
#if false
            File inputFolder1 = new File(inputFolderName);
            File[] files1 = inputFolder1.listFiles();
            int createdCloudlets = 0;
            foreach (File aFiles1 in files1)
            {
                File inputFolder = new File(aFiles1.ToString());
                File[] files = inputFolder.listFiles();
                for (int i = 0; i < files.Length; ++i)
                {
                    if (createdCloudlets < numberOfCloudlets)
                    {
                        ContainerCloudlet cloudlet = null;

                        try
                        {
                            cloudlet = new ContainerCloudlet(IDs.pollId(typeof(ContainerCloudlet)), 216000000L * 1000, 1, fileSize, outputSize, new UtilizationModelPlanetLabInMemoryExtended(files[i].AbsolutePath, 300.0D), utilizationModelNull, utilizationModelNull);
                        }
                        catch (Exception var13)
                        {
                            Debug.WriteLine(var13.ToString());
                            Debug.WriteLine(var13.StackTrace);
                            //Environment.Exit(0);
                            throw var13;
                        }

                        cloudlet.UserId = brokerId;
                        //            cloudlet.setVmId(i);
                        cloudletList.Add(cloudlet);
                        createdCloudlets += 1;
                    }
                    else
                    {

                        return cloudletList;
                    }
                }

            }
#endif
            return cloudletList;
        }

        // create the containers for hosting the cloudlets and binding them together.
        public static IList<Container> createContainerList(int brokerId, int containersNumber)
        {
            //ArrayList containers = new ArrayList();
            List<Container> containers = new List<Container>();

            for (int i = 0; i < containersNumber; ++i)
            {
                //            int containerType = new RandomGen().getNum(ConstantsExamples.CONTAINER_TYPES);
                int containerType = i / (int)Math.Ceiling((double)containersNumber / 3.0D);
                //            int containerType = 0;

                containers.Add(new PowerContainer(IDs.pollId(typeof(Container)), brokerId, (double)ConstantsExamples.CONTAINER_MIPS[containerType], ConstantsExamples.CONTAINER_PES[containerType], ConstantsExamples.CONTAINER_RAM[containerType], ConstantsExamples.CONTAINER_BW, 0L, "Xen", new ContainerCloudletSchedulerDynamicWorkload(ConstantsExamples.CONTAINER_MIPS[containerType], ConstantsExamples.CONTAINER_PES[containerType]), ConstantsExamples.SCHEDULING_INTERVAL));
            }

            return containers;
        }
        // create the containers for hosting the cloudlets and binding them together.
        public static IList<ContainerVm> createVmList(int brokerId, int containerVmsNumber)
        {
            //ArrayList containerVms = new ArrayList();
            List<ContainerVm> containerVms = new List<ContainerVm>();

            for (int i = 0; i < containerVmsNumber; ++i)
            {
                //ArrayList peList = new ArrayList();
                List<ContainerPe> peList = new List<ContainerPe>();

                //            int vmType = new RandomGen().getNum(ConstantsExamples.VM_TYPES);
                //            Log.print(vmType);
                //            Log.print("\n");
                int vmType = i / (int)Math.Ceiling((double)containerVmsNumber / 4.0D);
                //            int vmType = 1;
                //            int vmType = 1;

                for (int j = 0; j < ConstantsExamples.VM_PES[vmType]; ++j)
                {
                    peList.Add(new ContainerPe(j, new ContainerPeProvisionerSimple((double)ConstantsExamples.VM_MIPS[vmType])));
                }
                containerVms.Add(new PowerContainerVm(IDs.pollId(typeof(ContainerVm)), brokerId, (double)ConstantsExamples.VM_MIPS[vmType], (float)ConstantsExamples.VM_RAM[vmType], ConstantsExamples.VM_BW, ConstantsExamples.VM_SIZE, "Xen", new ContainerSchedulerTimeSharedOverSubscription(peList), new ContainerRamProvisionerSimple(ConstantsExamples.VM_RAM[vmType]), new ContainerBwProvisionerSimple(ConstantsExamples.VM_BW), peList, ConstantsExamples.SCHEDULING_INTERVAL));


            }

            return containerVms;
        }


        public static IList<ContainerHost> createHostList(int hostsNumber)
        {
            //ArrayList hostList = new ArrayList();
            List<ContainerHost> hostList = new List<ContainerHost>();

            for (int i = 0; i < hostsNumber; ++i)
            {
                //            int hostType =  new RandomGen().getNum(ConstantsExamples.HOST_TYPES);
                int hostType = i / (int)Math.Ceiling((double)hostsNumber / 3.0D);
                //            int hostType = i % 2;
                //            int hostType = 2;
                //ArrayList peList = new ArrayList();
                List<ContainerVmPe> peList = new List<ContainerVmPe>();

                for (int j = 0; j < ConstantsExamples.HOST_PES[hostType]; ++j)
                {
                    peList.Add(new ContainerVmPe(j, new ContainerVmPeProvisionerSimple((double)ConstantsExamples.HOST_MIPS[hostType])));
                }

                //            hostList.add(new PowerContainerHost(i, new ContainerVmRamProvisionerSimple(ConstantsExamples.HOST_RAM[hostType]),
                //                    new ContainerVmBwProvisionerSimple(1000000L), 1000000L, peList, new ContainerVmSchedulerTimeSharedOverSubscription(peList), ConstantsExamples.HOST_POWER[hostType]));
                hostList.Add(new PowerContainerHostUtilizationHistory(IDs.pollId(typeof(ContainerHost)), new ContainerVmRamProvisionerSimple(ConstantsExamples.HOST_RAM[hostType]), new ContainerVmBwProvisionerSimple(1000000L), 1000000L, peList, new ContainerVmSchedulerTimeSharedOverSubscription(peList), ConstantsExamples.HOST_POWER[hostType]));
            }

            return hostList;
        }


        // Broker

        public static ContainerDatacenterBroker createBroker(double overBookingFactor)
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

        //    // Data Center
        //    public static PowerContainerDatacenter createDatacenter(String name, List<ContainerHost> hostList, ContainerVmAllocationPolicy vmAllocationPolicy, ContainerAllocationPolicy containerAllocationPolicy) throws Exception {
        //        String arch = "x86";
        //        String os = "Linux";
        //        String vmm = "Xen";
        //        double time_zone = 10.0D;
        //        double cost = 3.0D;
        //        double costPerMem = 0.05D;
        //        double costPerStorage = 0.001D;
        //        double costPerBw = 0.0D;
        //        ContainerDatacenterCharacteristics characteristics = new ContainerDatacenterCharacteristics(arch, os, vmm, hostList, time_zone, cost, costPerMem, costPerStorage, costPerBw);
        //        PowerContainerDatacenter datacenter = null;
        ////        datacenter = new PowerContainerDatacenter(name,characteristics, vmAllocationPolicy, containerAllocationPolicy , new LinkedList(),Double.valueOf(300.0D));
        //        datacenter = new PowerContainerDatacenterCM(name,characteristics, vmAllocationPolicy, containerAllocationPolicy , new LinkedList(),Double.valueOf(300.0D));
        //
        //        return datacenter;
        //    }
        // Data Center
        //    public static ContainerDatacenter createDatacenter(String name, Class<? extends ContainerDatacenter> datacenterClass, List<ContainerHost> hostList, ContainerVmAllocationPolicy vmAllocationPolicy, ContainerAllocationPolicy containerAllocationPolicy, String experimentName, String logAddress) throws Exception {
        //        String arch = "x86";
        //        String os = "Linux";
        //        String vmm = "Xen";
        //        double time_zone = 10.0D;
        //        double cost = 3.0D;
        //        double costPerMem = 0.05D;
        //        double costPerStorage = 0.001D;
        //        double costPerBw = 0.0D;
        //        ContainerDatacenterCharacteristics characteristics = new ContainerDatacenterCharacteristics(arch, os, vmm, hostList, time_zone, cost, costPerMem, costPerStorage, costPerBw);
        //        ContainerDatacenter datacenter = null;
        //        try {
        //            datacenter = datacenterClass.getConstructor(
        //                    String.class,
        //                    ContainerDatacenterCharacteristics.class,
        //                    ContainerVmAllocationPolicy.class,
        //                    ContainerAllocationPolicy.class,
        //                    List.class,
        //                    Double.TYPE, String.class, String.class
        //            ).newInstance(
        //                    name,
        //                    characteristics,
        //                    vmAllocationPolicy,
        //                    containerAllocationPolicy,
        //                    new LinkedList<Storage>(),
        //                    ConstantsExamples.SCHEDULING_INTERVAL, experimentName, logAddress);
        //        } catch (Exception e) {
        //            e.printStackTrace();
        //            System.exit(0);
        //        }
        ////        datacenter = new PowerContainerDatacenter(name,characteristics, vmAllocationPolicy, containerAllocationPolicy , new LinkedList(),Double.valueOf(300.0D));
        ////        datacenter = new PowerContainerDatacenterCM(name,characteristics, vmAllocationPolicy, containerAllocationPolicy , new LinkedList(),Double.valueOf(300.0D));
        //
        //        return datacenter;
        //    }


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

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static ContainerDatacenter createDatacenter(String name, Class datacenterClass, List<ContainerHost> hostList, ContainerVmAllocationPolicy vmAllocationPolicy, ContainerAllocationPolicy containerAllocationPolicy, String experimentName, double schedulingInterval, String logAddress, double VMStartupDelay, double ContainerStartupDelay) throws Exception
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
        /// Prints the results.
        /// </summary>
        /// <param name="datacenter">     the datacenter </param>
        /// <param name="lastClock">      the last clock </param>
        /// <param name="experimentName"> the experiment name </param>
        /// <param name="outputInCsv">    the output in csv </param>
        /// <param name="outputFolder">   the output folder </param>
        public static void printResults(PowerContainerDatacenter datacenter, IList<ContainerVm> vms, IList<Container> containers, double lastClock, string experimentName, bool outputInCsv, string outputFolder)
        {
            Log.enable();
            IList<ContainerHost> hosts = datacenter.HostListProperty;

            int numberOfHosts = hosts.Count;
            int numberOfVms = vms.Count;

            double totalSimulationTime = lastClock;
            double energy = datacenter.Power / (3600 * 1000);
            int numberOfVmMigrations = datacenter.VmMigrationCount;

            IDictionary<string, double?> slaMetrics = getSlaMetrics(vms);

            double slaOverall = slaMetrics["overall"].Value;
            double slaAverage = slaMetrics["average"].Value;
            double slaDegradationDueToMigration = slaMetrics["underallocated_migration"].Value;
            // double slaTimePerVmWithMigration = slaMetrics.get("sla_time_per_vm_with_migration");
            // double slaTimePerVmWithoutMigration =
            // slaMetrics.get("sla_time_per_vm_without_migration");
            // double slaTimePerHost = getSlaTimePerHost(hosts);
            double slaTimePerActiveHost = getSlaTimePerActiveHost(hosts);

            double sla = slaTimePerActiveHost * slaDegradationDueToMigration;

            IList<double?> timeBeforeHostShutdown = getTimesBeforeHostShutdown(hosts);

            int numberOfHostShutdowns = timeBeforeHostShutdown.Count;

            double meanTimeBeforeHostShutdown = Double.NaN;
            double stDevTimeBeforeHostShutdown = Double.NaN;
            if (timeBeforeHostShutdown.Count > 0)
            {
                meanTimeBeforeHostShutdown = MathUtil.mean(timeBeforeHostShutdown);
                stDevTimeBeforeHostShutdown = MathUtil.stDev(timeBeforeHostShutdown);
            }

            IList<double?> timeBeforeVmMigration = getTimesBeforeVmMigration(vms);
            double meanTimeBeforeVmMigration = Double.NaN;
            double stDevTimeBeforeVmMigration = Double.NaN;
            if (timeBeforeVmMigration.Count > 0)
            {
                meanTimeBeforeVmMigration = MathUtil.mean(timeBeforeVmMigration);
                stDevTimeBeforeVmMigration = MathUtil.stDev(timeBeforeVmMigration);
            }
            IList<double?> timeBeforeContainerMigration = getTimesBeforeContainerMigration(containers);
            double meanTimeBeforeContainerMigration = Double.NaN;
            double stDevTimeBeforeContainerMigration = Double.NaN;
            if (timeBeforeContainerMigration.Count > 0)
            {
                meanTimeBeforeContainerMigration = MathUtil.mean(timeBeforeContainerMigration);
                stDevTimeBeforeContainerMigration = MathUtil.stDev(timeBeforeContainerMigration);
            }

            // TODO: File IO
#if false
            if (outputInCsv)
            {
                // We create the logging folders
                File folder = new File(outputFolder);
                if (!folder.exists())
                {
                    folder.mkdir();
                }
                File folder1 = new File(outputFolder + "/stats");
                if (!folder1.exists())
                {
                    folder1.mkdir();
                }
                File folder2 = new File(outputFolder + "/time_before_host_shutdown");
                if (!folder2.exists())
                {
                    folder2.mkdir();
                }
                File folder3 = new File(outputFolder + "/time_before_vm_migration");
                if (!folder3.exists())
                {
                    folder3.mkdir();
                }
                File folder4 = new File(outputFolder + "/metrics");
                if (!folder4.exists())
                {
                    folder4.mkdir();
                }


                StringBuilder data = new StringBuilder();
                string delimeter = ",";

                data.Append(experimentName + delimeter);
                data.Append(parseExperimentName(experimentName));
                data.Append(string.Format("{0:D}", numberOfHosts) + delimeter);
                data.Append(string.Format("{0:D}", numberOfVms) + delimeter);
                data.Append(string.Format("{0:F2}", totalSimulationTime) + delimeter);
                data.Append(string.Format("{0:F5}", energy) + delimeter);
                //            data.append(String.format("%d", numberOfMigrations) + delimeter);
                data.Append(string.Format("{0:F10}", sla) + delimeter);
                data.Append(string.Format("{0:F10}", slaTimePerActiveHost) + delimeter);
                data.Append(string.Format("{0:F10}", slaDegradationDueToMigration) + delimeter);
                data.Append(string.Format("{0:F10}", slaOverall) + delimeter);
                data.Append(string.Format("{0:F10}", slaAverage) + delimeter);
                //             data.append(String.format("%.5f", slaTimePerVmWithMigration) + delimeter);
                // data.append(String.format("%.5f", slaTimePerVmWithoutMigration) + delimeter);
                // data.append(String.format("%.5f", slaTimePerHost) + delimeter);
                data.Append(string.Format("{0:D}", numberOfHostShutdowns) + delimeter);
                data.Append(string.Format("{0:F2}", meanTimeBeforeHostShutdown) + delimeter);
                data.Append(string.Format("{0:F2}", stDevTimeBeforeHostShutdown) + delimeter);
                data.Append(string.Format("{0:F2}", meanTimeBeforeVmMigration) + delimeter);
                data.Append(string.Format("{0:F2}", stDevTimeBeforeVmMigration) + delimeter);

                if (datacenter.VmAllocationPolicy is PowerContainerVmAllocationPolicyMigrationAbstract)
                {
                    PowerContainerVmAllocationPolicyMigrationAbstract vmAllocationPolicy = (PowerContainerVmAllocationPolicyMigrationAbstract)datacenter.VmAllocationPolicy;

                    double executionTimeVmSelectionMean = MathUtil.mean(vmAllocationPolicy.ExecutionTimeHistoryVmSelection);
                    double executionTimeVmSelectionStDev = MathUtil.stDev(vmAllocationPolicy.ExecutionTimeHistoryVmSelection);
                    double executionTimeHostSelectionMean = MathUtil.mean(vmAllocationPolicy.ExecutionTimeHistoryHostSelection);
                    double executionTimeHostSelectionStDev = MathUtil.stDev(vmAllocationPolicy.ExecutionTimeHistoryHostSelection);
                    double executionTimeVmReallocationMean = MathUtil.mean(vmAllocationPolicy.ExecutionTimeHistoryVmReallocation);
                    double executionTimeVmReallocationStDev = MathUtil.stDev(vmAllocationPolicy.ExecutionTimeHistoryVmReallocation);
                    double executionTimeTotalMean = MathUtil.mean(vmAllocationPolicy.ExecutionTimeHistoryTotal);
                    double executionTimeTotalStDev = MathUtil.stDev(vmAllocationPolicy.ExecutionTimeHistoryTotal);

                    data.Append(string.Format("{0:F5}", executionTimeVmSelectionMean) + delimeter);
                    data.Append(string.Format("{0:F5}", executionTimeVmSelectionStDev) + delimeter);
                    data.Append(string.Format("{0:F5}", executionTimeHostSelectionMean) + delimeter);
                    data.Append(string.Format("{0:F5}", executionTimeHostSelectionStDev) + delimeter);
                    data.Append(string.Format("{0:F5}", executionTimeVmReallocationMean) + delimeter);
                    data.Append(string.Format("{0:F5}", executionTimeVmReallocationStDev) + delimeter);
                    data.writeMetricHistory(hosts, vmAllocationPolicy, outputFolder + "/metrics/" + experimentName + "_metric");
                }

                data.append("\n");

                writeDataRow(data.ToString(), outputFolder + "/stats/" + experimentName + "_stats.csv");
                writeDataColumn(timeBeforeHostShutdown, outputFolder + "/time_before_host_shutdown/" + experimentName + "_time_before_host_shutdown.csv");
                writeDataColumn(timeBeforeContainerMigration, outputFolder + "/time_before_vm_migration/" + experimentName + "_time_before_vm_migration.csv");

            }
            else
            {
                Log.Disabled = false;
                Log.printLine();
                Log.printLine(string.Format("Experiment name: " + experimentName));
                Log.printLine(string.Format("Number of hosts: " + numberOfHosts));
                Log.printLine(string.Format("Number of VMs: " + numberOfVms));
                Log.printLine(string.Format("Total simulation time: {0:F2} sec", totalSimulationTime));
                Log.printLine(string.Format("Energy consumption: {0:F2} kWh", energy));
                //            Log.printLine(String.format("Number of VM migrations: %d", numberOfMigrations));
                Log.printLine(string.Format("SLA: {0:F5}%", sla * 100));
                Log.printLine(string.Format("SLA perf degradation due to migration: {0:F2}%", slaDegradationDueToMigration * 100));
                Log.printLine(string.Format("SLA time per active host: {0:F2}%", slaTimePerActiveHost * 100));
                Log.printLine(string.Format("Overall SLA violation: {0:F2}%", slaOverall * 100));
                Log.printLine(string.Format("Average SLA violation: {0:F2}%", slaAverage * 100));
                // Log.printLine(String.format("SLA time per VM with migration: %.2f%%",
                // slaTimePerVmWithMigration * 100));
                // Log.printLine(String.format("SLA time per VM without migration: %.2f%%",
                // slaTimePerVmWithoutMigration * 100));
                // Log.printLine(String.format("SLA time per host: %.2f%%", slaTimePerHost * 100));
                Log.printLine(string.Format("Number of host shutdowns: {0:D}", numberOfHostShutdowns));
                Log.printLine(string.Format("Mean time before a host shutdown: {0:F2} sec", meanTimeBeforeHostShutdown));
                Log.printLine(string.Format("StDev time before a host shutdown: {0:F2} sec", stDevTimeBeforeHostShutdown));
                Log.printLine(string.Format("Mean time before a VM migration: {0:F2} sec", meanTimeBeforeVmMigration));
                Log.printLine(string.Format("StDev time before a VM migration: {0:F2} sec", stDevTimeBeforeVmMigration));
                Log.printLine(string.Format("Mean time before a Container migration: {0:F2} sec", meanTimeBeforeContainerMigration));
                Log.printLine(string.Format("StDev time before a Container migration: {0:F2} sec", stDevTimeBeforeContainerMigration));

                if (datacenter.VmAllocationPolicy is PowerContainerVmAllocationPolicyMigrationAbstract)
                {
                    PowerContainerVmAllocationPolicyMigrationAbstract vmAllocationPolicy = (PowerContainerVmAllocationPolicyMigrationAbstract)datacenter.VmAllocationPolicy;

                    double executionTimeVmSelectionMean = MathUtil.mean(vmAllocationPolicy.ExecutionTimeHistoryVmSelection);
                    double executionTimeVmSelectionStDev = MathUtil.stDev(vmAllocationPolicy.ExecutionTimeHistoryVmSelection);
                    double executionTimeHostSelectionMean = MathUtil.mean(vmAllocationPolicy.ExecutionTimeHistoryHostSelection);
                    double executionTimeHostSelectionStDev = MathUtil.stDev(vmAllocationPolicy.ExecutionTimeHistoryHostSelection);
                    double executionTimeVmReallocationMean = MathUtil.mean(vmAllocationPolicy.ExecutionTimeHistoryVmReallocation);
                    double executionTimeVmReallocationStDev = MathUtil.stDev(vmAllocationPolicy.ExecutionTimeHistoryVmReallocation);
                    double executionTimeTotalMean = MathUtil.mean(vmAllocationPolicy.ExecutionTimeHistoryTotal);
                    double executionTimeTotalStDev = MathUtil.stDev(vmAllocationPolicy.ExecutionTimeHistoryTotal);

                    Log.printLine(string.Format("Execution time - VM selection mean: {0:F5} sec", executionTimeVmSelectionMean));
                    Log.printLine(string.Format("Execution time - VM selection stDev: {0:F5} sec", executionTimeVmSelectionStDev));
                    Log.printLine(string.Format("Execution time - host selection mean: {0:F5} sec", executionTimeHostSelectionMean));
                    Log.printLine(string.Format("Execution time - host selection stDev: {0:F5} sec", executionTimeHostSelectionStDev));
                    Log.printLine(string.Format("Execution time - VM reallocation mean: {0:F5} sec", executionTimeVmReallocationMean));
                    Log.printLine(string.Format("Execution time - VM reallocation stDev: {0:F5} sec", executionTimeVmReallocationStDev));
                    Log.printLine(string.Format("Execution time - total mean: {0:F5} sec", executionTimeTotalMean));
                    Log.printLine(string.Format("Execution time - total stDev: {0:F5} sec", executionTimeTotalStDev));
                }
                Log.printLine();
            }
#endif

            Log.Disabled = true;
        }

        /// <summary>
        /// Gets the times before vm migration.
        /// </summary>
        /// <param name="vms"> the vms </param>
        /// <returns> the times before vm migration </returns>
        public static IList<double?> getTimesBeforeVmMigration(IList<ContainerVm> vms)
        {
            IList<double?> timeBeforeVmMigration = new List<double?>();
            foreach (ContainerVm vm in vms)
            {
                bool previousIsInMigration = false;
                double lastTimeMigrationFinished = 0;
                foreach (VmStateHistoryEntry entry in vm.StateHistory)
                {
                    if (previousIsInMigration == true && entry.InMigration == false)
                    {
                        timeBeforeVmMigration.Add(entry.Time - lastTimeMigrationFinished);
                    }
                    if (previousIsInMigration == false && entry.InMigration == true)
                    {
                        lastTimeMigrationFinished = entry.Time;
                    }
                    previousIsInMigration = entry.InMigration;
                }
            }
            return timeBeforeVmMigration;
        }

        /// <summary>
        /// Gets the times before vm migration.
        /// </summary>
        /// <param name="containers"> the vms </param>
        /// <returns> the times before vm migration </returns>
        public static IList<double?> getTimesBeforeContainerMigration(IList<Container> containers)
        {
            IList<double?> timeBeforeVmMigration = new List<double?>();
            foreach (Container container in containers)
            {
                bool previousIsInMigration = false;
                double lastTimeMigrationFinished = 0;
                foreach (VmStateHistoryEntry entry in container.StateHistory)
                {
                    if (previousIsInMigration == true && entry.InMigration == false)
                    {
                        timeBeforeVmMigration.Add(entry.Time - lastTimeMigrationFinished);
                    }
                    if (previousIsInMigration == false && entry.InMigration == true)
                    {
                        lastTimeMigrationFinished = entry.Time;
                    }
                    previousIsInMigration = entry.InMigration;
                }
            }
            return timeBeforeVmMigration;
        }

        /// <summary>
        /// Gets the times before host shutdown.
        /// </summary>
        /// <param name="hosts"> the hosts </param>
        /// <returns> the times before host shutdown </returns>
        public static IList<double?> getTimesBeforeHostShutdown(IList<ContainerHost> hosts)
        {
            IList<double?> timeBeforeShutdown = new List<double?>();
            foreach (ContainerHost host in hosts)
            {
                bool previousIsActive = true;
                double lastTimeSwitchedOn = 0;
                foreach (HostStateHistoryEntry entry in ((ContainerHostDynamicWorkload)host).StateHistory)
                {
                    if (previousIsActive == true && entry.Active == false)
                    {
                        timeBeforeShutdown.Add(entry.Time - lastTimeSwitchedOn);
                    }
                    if (previousIsActive == false && entry.Active == true)
                    {
                        lastTimeSwitchedOn = entry.Time;
                    }
                    previousIsActive = entry.Active;
                }
            }
            return timeBeforeShutdown;
        }


        /// <summary>
        /// Parses the experiment name.
        /// </summary>
        /// <param name="name"> the name </param>
        /// <returns> the string </returns>
        public static string parseExperimentName(string name)
        {
            // TODO: File IO
            //Scanner scanner = new Scanner(name);
            //StringBuilder csvName = new StringBuilder();
            //scanner.useDelimiter("_");
            //for (int i = 0; i < 8; i++)
            //{
            //    if (scanner.hasNext())
            //    {
            //        csvName.Append(scanner.next() + ",");
            //    }
            //    else
            //    {
            //        csvName.Append(",");
            //    }
            //}
            //scanner.close();
            //return csvName.ToString();
            return null;
        }

        protected internal static double getSlaTimePerActiveHost(IList<ContainerHost> hosts)
        {
            double slaViolationTimePerHost = 0;
            double totalTime = 0;

            foreach (ContainerHost _host in hosts)
            {
                ContainerHostDynamicWorkload host = (ContainerHostDynamicWorkload)_host;
                double previousTime = -1;
                double previousAllocated = 0;
                double previousRequested = 0;
                bool previousIsActive = true;

                foreach (HostStateHistoryEntry entry in host.StateHistory)
                {
                    if (previousTime != -1 && previousIsActive)
                    {
                        double timeDiff = entry.Time - previousTime;
                        totalTime += timeDiff;
                        if (previousAllocated < previousRequested)
                        {
                            slaViolationTimePerHost += timeDiff;
                        }
                    }

                    previousAllocated = entry.AllocatedMips;
                    previousRequested = entry.RequestedMips;
                    previousTime = entry.Time;
                    previousIsActive = entry.Active;
                }
            }

            return slaViolationTimePerHost / totalTime;
        }

        /// <summary>
        /// Gets the sla time per host.
        /// </summary>
        /// <param name="hosts"> the hosts </param>
        /// <returns> the sla time per host </returns>
        protected internal static double getSlaTimePerHost(IList<ContainerHost> hosts)
        {
            double slaViolationTimePerHost = 0;
            double totalTime = 0;

            foreach (ContainerHost _host in hosts)
            {
                ContainerHostDynamicWorkload host = (ContainerHostDynamicWorkload)_host;
                double previousTime = -1;
                double previousAllocated = 0;
                double previousRequested = 0;

                foreach (HostStateHistoryEntry entry in host.StateHistory)
                {
                    if (previousTime != -1)
                    {
                        double timeDiff = entry.Time - previousTime;
                        totalTime += timeDiff;
                        if (previousAllocated < previousRequested)
                        {
                            slaViolationTimePerHost += timeDiff;
                        }
                    }

                    previousAllocated = entry.AllocatedMips;
                    previousRequested = entry.RequestedMips;
                    previousTime = entry.Time;
                }
            }

            return slaViolationTimePerHost / totalTime;
        }

        /// <summary>
        /// Gets the sla metrics.
        /// </summary>
        /// <param name="vms"> the vms </param>
        /// <returns> the sla metrics </returns>
        protected internal static IDictionary<string, double?> getSlaMetrics(IList<ContainerVm> vms)
        {
            IDictionary<string, double?> metrics = new Dictionary<string, double?>();
            IList<double?> slaViolation = new List<double?>();
            double totalAllocated = 0;
            double totalRequested = 0;
            double totalUnderAllocatedDueToMigration = 0;

            foreach (ContainerVm vm in vms)
            {
                double vmTotalAllocated = 0;
                double vmTotalRequested = 0;
                double vmUnderAllocatedDueToMigration = 0;
                double previousTime = -1;
                double previousAllocated = 0;
                double previousRequested = 0;
                bool previousIsInMigration = false;

                foreach (VmStateHistoryEntry entry in vm.StateHistory)
                {
                    if (previousTime != -1)
                    {
                        double timeDiff = entry.Time - previousTime;
                        vmTotalAllocated += previousAllocated * timeDiff;
                        vmTotalRequested += previousRequested * timeDiff;

                        if (previousAllocated < previousRequested)
                        {
                            slaViolation.Add((previousRequested - previousAllocated) / previousRequested);
                            if (previousIsInMigration)
                            {
                                vmUnderAllocatedDueToMigration += (previousRequested - previousAllocated) * timeDiff;
                            }
                        }
                    }

                    previousAllocated = entry.AllocatedMips;
                    previousRequested = entry.RequestedMips;
                    previousTime = entry.Time;
                    previousIsInMigration = entry.InMigration;
                }

                totalAllocated += vmTotalAllocated;
                totalRequested += vmTotalRequested;
                totalUnderAllocatedDueToMigration += vmUnderAllocatedDueToMigration;
            }

            metrics["overall"] = (totalRequested - totalAllocated) / totalRequested;
            //if (slaViolation.Empty)
            if (slaViolation.Count == 0)
            {
                metrics["average"] = 0.0;
            }
            else
            {
                metrics["average"] = MathUtil.mean(slaViolation);
            }
            metrics["underallocated_migration"] = totalUnderAllocatedDueToMigration / totalRequested;
            // metrics.put("sla_time_per_vm_with_migration", slaViolationTimePerVmWithMigration /
            // totalTime);
            // metrics.put("sla_time_per_vm_without_migration", slaViolationTimePerVmWithoutMigration /
            // totalTime);

            return metrics;
        }

        /// <summary>
        /// Write data column.
        /// </summary>
        /// <param name="data">       the data </param>
        /// <param name="outputPath"> the output path </param>
        public static void writeDataColumn(IList<Number> data, string outputPath)
        {
            File file = new File(outputPath);
            File parent = file.ParentFile;
            if (!parent.exists() && !parent.mkdirs())
            {
                throw new System.InvalidOperationException("Couldn't create dir: " + parent);
            }
            try
            {
                file.createNewFile();
            }
            catch (IOException e1)
            {
                Debug.WriteLine(e1.ToString());
                Debug.WriteLine(e1.StackTrace);
                //Environment.Exit(0);
                throw e1;
            }
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
                foreach (Number value in data)
                {
                    writer.Write(value.ToString() + "\n");
                }
                writer.Close();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                //Environment.Exit(0);
                throw e;
            }
        }

        /// <summary>
        /// Write data row.
        /// </summary>
        /// <param name="data">       the data </param>
        /// <param name="outputPath"> the output path </param>
        public static void writeDataRow(string data, string outputPath)
        {
            File file = new File(outputPath);
            try
            {
                file.createNewFile();
            }
            catch (IOException e1)
            {
                Debug.WriteLine(e1.ToString());
                Debug.WriteLine(e1.StackTrace);
                //Environment.Exit(0);
                throw e1;
            }
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
                writer.BaseStream.WriteByte(data);
                writer.Close();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// Write metric history.
        /// </summary>
        /// <param name="hosts">              the hosts </param>
        /// <param name="vmAllocationPolicy"> the vm allocation policy </param>
        /// <param name="outputPath">         the output path </param>
        public static void writeMetricHistory<T1>(IList<T1> hosts, PowerContainerVmAllocationPolicyMigrationAbstract vmAllocationPolicy, string outputPath) where T1 : ContainerHost
        {
            // for (Host host : hosts) {
            for (int j = 0; j < 10; j++)
            {
                ContainerHost host = hosts[j];

                if (!vmAllocationPolicy.TimeHistory.ContainsKey(host.Id))
                {
                    continue;
                }
                File file = new File(outputPath + "_" + host.Id + ".csv");
                try
                {
                    file.createNewFile();
                }
                catch (IOException e1)
                {
                    Debug.WriteLine(e1.ToString());
                    Debug.WriteLine(e1.StackTrace);
                    //Environment.Exit(0);
                    throw e1;
                }
                try
                {
                    System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
                    IList<double?> timeData = vmAllocationPolicy.TimeHistory[host.Id];
                    IList<double?> utilizationData = vmAllocationPolicy.UtilizationHistory[host.Id];
                    IList<double?> metricData = vmAllocationPolicy.MetricHistory[host.Id];

                    for (int i = 0; i < timeData.Count; i++)
                    {
                        writer.BaseStream.WriteByte(string.Format("{0:F2},{1:F2},{2:F2}\n", timeData[i], utilizationData[i], metricData[i]));
                    }
                    writer.Close();
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e.ToString());
                    Debug.WriteLine(e.StackTrace);
                    //Environment.Exit(0);
                    throw e;
                }
            }
        }

        /// <summary>
        /// Prints the metric history.
        /// </summary>
        /// <param name="hosts">              the hosts </param>
        /// <param name="vmAllocationPolicy"> the vm allocation policy </param>
        public static void printMetricHistory<T1>(IList<T1> hosts, PowerContainerVmAllocationPolicyMigrationAbstract vmAllocationPolicy) where T1 : ContainerHost
        {
            for (int i = 0; i < 10; i++)
            {
                ContainerHost host = hosts[i];

                Log.printLine("Host #" + host.Id);
                Log.printLine("Time:");
                if (!vmAllocationPolicy.TimeHistory.ContainsKey(host.Id))
                {
                    continue;
                }
                foreach (double? time in vmAllocationPolicy.TimeHistory[host.Id])
                {
                    Log.format("%.2f, ", time);
                }
                Log.printLine();

                foreach (double? utilization in vmAllocationPolicy.UtilizationHistory[host.Id])
                {
                    Log.format("%.2f, ", utilization);
                }
                Log.printLine();

                foreach (double? metric in vmAllocationPolicy.MetricHistory[host.Id])
                {
                    Log.format("%.2f, ", metric);
                }
                Log.printLine();
            }
        }

        public static void printResultsNew(PowerContainerDatacenter datacenter, ContainerDatacenterBroker broker, double lastClock, string experimentName, bool outputInCsv, string outputFolder)
        {
            IList<ContainerVm> vms = broker.VmsCreatedList;
            IList<Container> containers = broker.ContainersCreatedList;
            Log.enable();
            IList<ContainerHost> hosts = datacenter.HostListProperty;
            IDictionary<string, double?> slaMetrics = getSlaMetrics(vms);
            string[] msg = new string[] { "ExperimentName", "hostSelectionPolicy", "vmAllocationPolicy", "OLThreshold", "ULThreshold", "VMSPolicy", "ContainerSpolicy", "ContainerPlacement", "Percentile", "numberOfHosts", "numberOfVms", "totalSimulationTime", "slaOverall", "slaAverage", "slaTimePerActiveHost", "meanTimeBeforeHostShutdown", "stDevTimeBeforeHostShutdown", "medTimeBeforeHostShutdown", "meanTimeBeforeContainerMigration", "stDevTimeBeforeContainerMigration", "medTimeBeforeContainerMigration", "meanActiveVm", "stDevActiveVm", "medActiveVm", "meanActiveHosts", "stDevActiveHosts", "medActiveHosts", "meanNumberOfContainerMigrations", "stDevNumberOfContainerMigrations", "medNumberOfContainerMigrations", "meanDatacenterEnergy", "stDevDatacenterEnergy", "medDatacenterEnergy", "totalContainerMigration", "totalVmMigration", "totalVmCreated", "numberOfOverUtilization", "energy", "CreatedContainers", "CreatedVms" };

            int numberOfHosts = hosts.Count;
            int numberOfVms = vms.Count;
            double totalSimulationTime = lastClock;
            double slaOverall = slaMetrics["overall"].Value;
            double slaAverage = slaMetrics["average"].Value;
            double slaTimePerActiveHost = getSlaTimePerActiveHost(hosts);
            IList<double?> timeBeforeHostShutdown = getTimesBeforeHostShutdown(hosts);
            int numberOfHostShutdowns = timeBeforeHostShutdown.Count;
            double meanTimeBeforeHostShutdown = Double.NaN;
            double stDevTimeBeforeHostShutdown = Double.NaN;
            double medTimeBeforeHostShutdown = Double.NaN;
            if (timeBeforeHostShutdown.Count > 0)
            {
                meanTimeBeforeHostShutdown = MathUtil.mean(timeBeforeHostShutdown);
                stDevTimeBeforeHostShutdown = MathUtil.stDev(timeBeforeHostShutdown);
                medTimeBeforeHostShutdown = MathUtil.median(timeBeforeHostShutdown);
            }

            IList<double?> timeBeforeContainerMigration = getTimesBeforeContainerMigration(containers);
            double meanTimeBeforeContainerMigration = Double.NaN;
            double stDevTimeBeforeContainerMigration = Double.NaN;
            double medTimeBeforeContainerMigration = Double.NaN;
            if (timeBeforeContainerMigration.Count > 0)
            {
                meanTimeBeforeContainerMigration = MathUtil.mean(timeBeforeContainerMigration);
                stDevTimeBeforeContainerMigration = MathUtil.stDev(timeBeforeContainerMigration);
                medTimeBeforeContainerMigration = MathUtil.median(timeBeforeContainerMigration);
            }
            IList<double?> activeVm = datacenter.ActiveVmList;
            double meanActiveVm = Double.NaN;
            double stDevActiveVm = Double.NaN;
            double medActiveVm = Double.NaN;
            if (activeVm.Count > 0)
            {
                meanActiveVm = MathUtil.mean(activeVm);
                stDevActiveVm = MathUtil.stDev(activeVm);
                medActiveVm = MathUtil.median(activeVm);
            }
            IList<double?> activeHost = datacenter.ActiveHostList;
            double meanActiveHosts = Double.NaN;
            double stDevActiveHosts = Double.NaN;
            double medActiveHosts = Double.NaN;
            if (activeHost.Count > 0)
            {
                meanActiveHosts = MathUtil.mean(activeHost);
                stDevActiveHosts = MathUtil.stDev(activeHost);
                medActiveHosts = MathUtil.median(activeHost);
            }
            IList<double?> numberOfContainerMigrations = datacenter.ContainerMigrationList;
            double meanNumberOfContainerMigrations = Double.NaN;
            double stDevNumberOfContainerMigrations = Double.NaN;
            double medNumberOfContainerMigrations = Double.NaN;
            if (numberOfContainerMigrations.Count > 0)
            {
                meanNumberOfContainerMigrations = MathUtil.mean(numberOfContainerMigrations);
                stDevNumberOfContainerMigrations = MathUtil.stDev(numberOfContainerMigrations);
                medNumberOfContainerMigrations = MathUtil.median(numberOfContainerMigrations);
            }
            IList<double?> datacenterEnergy = datacenter.DatacenterEnergyList;
            double meanDatacenterEnergy = Double.NaN;
            double stDevDatacenterEnergy = Double.NaN;
            double medDatacenterEnergy = Double.NaN;
            if (datacenterEnergy.Count > 0)
            {
                meanDatacenterEnergy = MathUtil.mean(datacenterEnergy);
                stDevDatacenterEnergy = MathUtil.stDev(datacenterEnergy);
                medDatacenterEnergy = MathUtil.median(datacenterEnergy);
            }
            int totalContainerMigration = 0;
            int totalVmMigration = 0;
            int totalVmCreated = 0;
            if (datacenter is PowerContainerDatacenterCM)
            {
                totalContainerMigration = ((PowerContainerDatacenterCM)datacenter).ContainerMigrationCount;
                totalVmMigration = ((PowerContainerDatacenterCM)datacenter).VmMigrationCount;
                totalVmCreated = ((PowerContainerDatacenterCM)datacenter).NewlyCreatedVms;


            }
            PowerContainerVmAllocationPolicyMigrationAbstract vmAllocationPolicy = (PowerContainerVmAllocationPolicyMigrationAbstract)datacenter.VmAllocationPolicy;
            int numberOfOverUtilization = getNumberofOverUtilization(hosts, vmAllocationPolicy);

            double energy = datacenter.Power / (3600 * 1000);

            //Now we create the log we need
            StringBuilder data = new StringBuilder();
            string delimeter = ",";

            data.Append(experimentName + delimeter);
            data.Append(parseExperimentName(experimentName));
            data.Append(string.Format("{0:D}", numberOfHosts) + delimeter);
            data.Append(string.Format("{0:D}", numberOfVms) + delimeter);
            data.Append(string.Format("{0:F2}", totalSimulationTime) + delimeter);
            data.Append(string.Format("{0:F10}", slaOverall) + delimeter);
            data.Append(string.Format("{0:F10}", slaAverage) + delimeter);
            data.Append(string.Format("{0:F10}", slaTimePerActiveHost) + delimeter);
            data.Append(string.Format("{0:F10}", meanTimeBeforeHostShutdown) + delimeter);
            data.Append(string.Format("{0:F10}", stDevTimeBeforeHostShutdown) + delimeter);
            data.Append(string.Format("{0:F10}", medTimeBeforeHostShutdown) + delimeter);
            data.Append(string.Format("{0:F10}", meanTimeBeforeContainerMigration) + delimeter);
            data.Append(string.Format("{0:F10}", stDevTimeBeforeContainerMigration) + delimeter);
            data.Append(string.Format("{0:F10}", medTimeBeforeContainerMigration) + delimeter);
            data.Append(string.Format("{0:F10}", meanActiveVm) + delimeter);
            data.Append(string.Format("{0:F10}", stDevActiveVm) + delimeter);
            data.Append(string.Format("{0:F10}", medActiveVm) + delimeter);
            data.Append(string.Format("{0:F10}", meanActiveHosts) + delimeter);
            data.Append(string.Format("{0:F10}", stDevActiveHosts) + delimeter);
            data.Append(string.Format("{0:F10}", medActiveHosts) + delimeter);
            data.Append(string.Format("{0:F10}", meanNumberOfContainerMigrations) + delimeter);
            data.Append(string.Format("{0:F10}", stDevNumberOfContainerMigrations) + delimeter);
            data.Append(string.Format("{0:F10}", medNumberOfContainerMigrations) + delimeter);
            data.Append(string.Format("{0:F10}", meanDatacenterEnergy) + delimeter);
            data.Append(string.Format("{0:F10}", stDevDatacenterEnergy) + delimeter);
            data.Append(string.Format("{0:F10}", medDatacenterEnergy) + delimeter);
            data.Append(string.Format("{0:D}", totalContainerMigration) + delimeter);
            data.Append(string.Format("{0:D}", totalVmMigration) + delimeter);
            data.Append(string.Format("{0:D}", totalVmCreated) + delimeter);
            data.Append(string.Format("{0:D}", numberOfOverUtilization) + delimeter);
            data.Append(string.Format("{0:F5}", energy) + delimeter);
            data.Append(string.Format("{0:D}", broker.ContainersCreated) + delimeter);
            data.Append(string.Format("{0:D}", broker.NumberOfCreatedVMs) + delimeter);

            //        data.append(String.format("%.10f", sla) + delimeter);
            //        data.append(String.format("%.10f", slaDegradationDueToMigration) + delimeter);

            int index = experimentName.LastIndexOf("_");
            File folder1 = new File(outputFolder + "/stats/");
            File parent1 = folder1.ParentFile;
            if (!parent1.exists() && !parent1.mkdirs())
            {
                throw new System.InvalidOperationException("Couldn't create dir: " + parent1);
            }

            if (!folder1.exists())
            {
                folder1.mkdir();
            }
            string beforShutDown = outputFolder + "/time_before_host_shutdown/" + experimentName.substring(0, index);
            File folder2 = new File(beforShutDown);
            File parent2 = folder2.ParentFile;
            if (!parent2.exists() && !parent2.mkdirs())
            {
                throw new System.InvalidOperationException("Couldn't create dir: " + parent2);
            }


            if (!folder2.exists())
            {
                folder2.mkdir();
            }


            string beforeMigrate = outputFolder + "/time_before_vm_migration/" + experimentName.substring(0, index);
            File folder3 = new File(beforeMigrate);

            File parent3 = folder3.ParentFile;
            if (!parent3.exists() && !parent3.mkdirs())
            {
                throw new System.InvalidOperationException("Couldn't create dir: " + parent3);
            }

            if (!folder3.exists())
            {
                folder3.mkdir();
            }



            //        int index = experimentName.lastIndexOf("_");

            string fileAddress = string.Format("{0}/stats/{1}_stats.csv", outputFolder, experimentName.substring(0, index));


            File f = new File(fileAddress);
            CSVWriter writer = new CSVWriter(new System.IO.StreamWriter(fileAddress, true), ',', CSVWriter.NO_QUOTE_CHARACTER);
            File parent = f.ParentFile;
            if (!parent.exists() && !parent.mkdirs())
            {
                throw new System.InvalidOperationException("Couldn't create dir: " + parent);
            }

            if (!f.exists())
            {
                f.createNewFile();
                //            writer.writeNext("\n")
            }
            int temp = index;
            if (experimentName.substring(index).StartsWith("_1", StringComparison.Ordinal) && experimentName.length() - 2 == temp)
            {
                //            CSVWriter writer1 = new CSVWriter(new FileWriter(fileAddress, true), ',',CSVWriter.NO_QUOTE_CHARACTER);
                writer.writeNext(msg);
            }
            writer.writeNext(new string[] { data.ToString() });
            writer.flush();
            writer.close();




            writeDataColumn(timeBeforeHostShutdown, beforShutDown + "/" + experimentName + "_time_before_host_shutdown.csv");
            writeDataColumn(timeBeforeContainerMigration, beforeMigrate + "/" + experimentName + "_time_before_vm_migration.csv");




        }


        public static int getNumberofOverUtilization(IList<ContainerHost> hosts, PowerContainerVmAllocationPolicyMigrationAbstract vmAllocationPolicy)
        {
            int numberOfOverUtilization = 0;
            for (int j = 0; j < hosts.Count; j++)
            {
                ContainerHost host = hosts[j];

                if (!vmAllocationPolicy.TimeHistory.ContainsKey(host.Id))
                {
                    continue;
                }


                IList<double?> timeData = vmAllocationPolicy.TimeHistory[host.Id];
                IList<double?> utilizationData = vmAllocationPolicy.UtilizationHistory[host.Id];
                IList<double?> metricData = vmAllocationPolicy.MetricHistory[host.Id];

                for (int i = 0; i < timeData.Count; i++)
                {
                    if (utilizationData[i] > metricData[i])
                    {
                        numberOfOverUtilization++;
                    }
                }
            }

            return numberOfOverUtilization;
        }
    }
}