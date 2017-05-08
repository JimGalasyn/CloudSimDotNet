using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.examples.power
{
    using PowerDatacenter = org.cloudbus.cloudsim.power.PowerDatacenter;
    using PowerDatacenterBroker = org.cloudbus.cloudsim.power.PowerDatacenterBroker;
    using PowerHost = org.cloudbus.cloudsim.power.PowerHost;
    using PowerHostUtilizationHistory = org.cloudbus.cloudsim.power.PowerHostUtilizationHistory;
    using PowerVm = org.cloudbus.cloudsim.power.PowerVm;
    using PowerVmAllocationPolicyMigrationAbstract = org.cloudbus.cloudsim.power.PowerVmAllocationPolicyMigrationAbstract;
    using BwProvisionerSimple = org.cloudbus.cloudsim.provisioners.BwProvisionerSimple;
    using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using RamProvisionerSimple = org.cloudbus.cloudsim.provisioners.RamProvisionerSimple;
    using MathUtil = org.cloudbus.cloudsim.util.MathUtil;
    using System.Text;
    using System.IO;

    /// <summary>
    /// The Class Helper.
    /// 
    /// If you are using any algorithms, policies or workload included in the power package, please cite
    /// the following paper:
    /// 
    /// Anton Beloglazov, and Rajkumar Buyya, "Optimal Online Deterministic Algorithms and Adaptive
    /// Heuristics for Energy and Performance Efficient Dynamic Consolidation of Virtual Machines in
    /// Cloud Data Centers", Concurrency and Computation: Practice and Experience (CCPE), Volume 24,
    /// Issue 13, Pages: 1397-1420, John Wiley & Sons, Ltd, New York, USA, 2012
    /// 
    /// @author Anton Beloglazov
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Creates the vm list.
        /// </summary>
        /// <param name="brokerId"> the broker id </param>
        /// <param name="vmsNumber"> the vms number
        /// </param>
        /// <returns> the list< vm> </returns>
        public static IList<Vm> createVmList(int brokerId, int vmsNumber)
        {
            IList<Vm> vms = new List<Vm>();
            for (int i = 0; i < vmsNumber; i++)
            {
                int vmType = i / (int)Math.Ceiling((double)vmsNumber / Constants.VM_TYPES);
                vms.Add(new PowerVm(i, brokerId, Constants.VM_MIPS[vmType], Constants.VM_PES[vmType], Constants.VM_RAM[vmType], Constants.VM_BW, Constants.VM_SIZE, 1, "Xen", new CloudletSchedulerDynamicWorkload(Constants.VM_MIPS[vmType], Constants.VM_PES[vmType]), Constants.SCHEDULING_INTERVAL));
            }
            return vms;
        }

        /// <summary>
        /// Creates the host list.
        /// </summary>
        /// <param name="hostsNumber"> the hosts number
        /// </param>
        /// <returns> the list< power host> </returns>
        public static IList<PowerHost> createHostList(int hostsNumber)
        {
            IList<PowerHost> hostList = new List<PowerHost>();
            for (int i = 0; i < hostsNumber; i++)
            {
                int hostType = i % Constants.HOST_TYPES;

                IList<Pe> peList = new List<Pe>();
                for (int j = 0; j < Constants.HOST_PES[hostType]; j++)
                {
                    peList.Add(new Pe(j, new PeProvisionerSimple(Constants.HOST_MIPS[hostType])));
                }
                hostList.Add(new PowerHostUtilizationHistory(i, new RamProvisionerSimple(Constants.HOST_RAM[hostType]), new BwProvisionerSimple(Constants.HOST_BW), Constants.HOST_STORAGE, peList, new VmSchedulerTimeSharedOverSubscription(peList), Constants.HOST_POWER[hostType]));
            }
            return hostList;
        }

        /// <summary>
        /// Creates the broker.
        /// </summary>
        /// <returns> the datacenter broker </returns>
        public static DatacenterBroker createBroker()
        {
            DatacenterBroker broker = null;
            try
            {
                broker = new PowerDatacenterBroker("Broker");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Environment.Exit(0);
            }
            return broker;
        }

        /// <summary>
        /// Creates the datacenter.
        /// </summary>
        /// <param name="name"> the name </param>
        /// <param name="datacenterClass"> the datacenter class </param>
        /// <param name="hostList"> the host list </param>
        /// <param name="vmAllocationPolicy"> the vm allocation policy </param>
        /// <param name="simulationLength">
        /// </param>
        /// <returns> the power datacenter
        /// </returns>
        /// <exception cref="Exception"> the exception </exception>
        public static Datacenter createDatacenter(string name, Type datacenterClass, IList<PowerHost> hostList, VmAllocationPolicy vmAllocationPolicy)
        {
            string arch = "x86"; // system architecture
            string os = "Linux"; // operating system
            string vmm = "Xen";
            double time_zone = 10.0; // time zone this resource located
            double cost = 3.0; // the cost of using processing in this resource
            double costPerMem = 0.05; // the cost of using memory in this resource
            double costPerStorage = 0.001; // the cost of using storage in this resource
            double costPerBw = 0.0; // the cost of using bw in this resource

            DatacenterCharacteristics characteristics = new DatacenterCharacteristics(arch, os, vmm, hostList, time_zone, cost, costPerMem, costPerStorage, costPerBw);

            Datacenter datacenter = null;
            try
            {
                datacenter = datacenterClass.getConstructor(typeof(string), typeof(DatacenterCharacteristics), typeof(VmAllocationPolicy), typeof(IList), Double.TYPE).newInstance(name, characteristics, vmAllocationPolicy, new LinkedList<Storage>(), Constants.SCHEDULING_INTERVAL);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Environment.Exit(0);
            }

            return datacenter;
        }

        /// <summary>
        /// Gets the times before host shutdown.
        /// </summary>
        /// <param name="hosts"> the hosts </param>
        /// <returns> the times before host shutdown </returns>
        public static IList<double?> getTimesBeforeHostShutdown(IList<Host> hosts)
        {
            IList<double?> timeBeforeShutdown = new List<double?>();
            foreach (Host host in hosts)
            {
                bool previousIsActive = true;
                double lastTimeSwitchedOn = 0;
                foreach (HostStateHistoryEntry entry in ((HostDynamicWorkload)host).StateHistory)
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
        /// Gets the times before vm migration.
        /// </summary>
        /// <param name="vms"> the vms </param>
        /// <returns> the times before vm migration </returns>
        public static IList<double?> getTimesBeforeVmMigration(IList<Vm> vms)
        {
            IList<double?> timeBeforeVmMigration = new List<double?>();
            foreach (Vm vm in vms)
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
        /// Prints the results.
        /// </summary>
        /// <param name="datacenter"> the datacenter </param>
        /// <param name="lastClock"> the last clock </param>
        /// <param name="experimentName"> the experiment name </param>
        /// <param name="outputInCsv"> the output in csv </param>
        /// <param name="outputFolder"> the output folder </param>
        public static void printResults(PowerDatacenter datacenter, IList<Vm> vms, double lastClock, string experimentName, bool outputInCsv, string outputFolder)
        {
            Log.enable();
            IList<Host> hosts = datacenter.HostListProperty;

            int numberOfHosts = hosts.Count;
            int numberOfVms = vms.Count;

            double totalSimulationTime = lastClock;
            double energy = datacenter.Power / (3600 * 1000);
            int numberOfMigrations = datacenter.MigrationCount;

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
            if (outputInCsv)
            {
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
                data.Append(string.Format("{0:D}", numberOfMigrations) + delimeter);
                data.Append(string.Format("{0:F10}", sla) + delimeter);
                data.Append(string.Format("{0:F10}", slaTimePerActiveHost) + delimeter);
                data.Append(string.Format("{0:F10}", slaDegradationDueToMigration) + delimeter);
                data.Append(string.Format("{0:F10}", slaOverall) + delimeter);
                data.Append(string.Format("{0:F10}", slaAverage) + delimeter);
                // data.append(String.format("%.5f", slaTimePerVmWithMigration) + delimeter);
                // data.append(String.format("%.5f", slaTimePerVmWithoutMigration) + delimeter);
                // data.append(String.format("%.5f", slaTimePerHost) + delimeter);
                data.Append(string.Format("{0:D}", numberOfHostShutdowns) + delimeter);
                data.Append(string.Format("{0:F2}", meanTimeBeforeHostShutdown) + delimeter);
                data.Append(string.Format("{0:F2}", stDevTimeBeforeHostShutdown) + delimeter);
                data.Append(string.Format("{0:F2}", meanTimeBeforeVmMigration) + delimeter);
                data.Append(string.Format("{0:F2}", stDevTimeBeforeVmMigration) + delimeter);

                if (datacenter.VmAllocationPolicy is PowerVmAllocationPolicyMigrationAbstract)
                {
                    PowerVmAllocationPolicyMigrationAbstract vmAllocationPolicy = (PowerVmAllocationPolicyMigrationAbstract)datacenter.VmAllocationPolicy;

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
                    data.Append(string.Format("{0:F5}", executionTimeTotalMean) + delimeter);
                    data.Append(string.Format("{0:F5}", executionTimeTotalStDev) + delimeter);

                    writeMetricHistory(hosts, vmAllocationPolicy, outputFolder + "/metrics/" + experimentName + "_metric");
                }

                data.Append("\n");

                writeDataRow(data.ToString(), outputFolder + "/stats/" + experimentName + "_stats.csv");
                writeDataColumn(timeBeforeHostShutdown, outputFolder + "/time_before_host_shutdown/" + experimentName + "_time_before_host_shutdown.csv");
                writeDataColumn(timeBeforeVmMigration, outputFolder + "/time_before_vm_migration/" + experimentName + "_time_before_vm_migration.csv");
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
                Log.printLine(string.Format("Number of VM migrations: {0:D}", numberOfMigrations));
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

                if (datacenter.VmAllocationPolicy is PowerVmAllocationPolicyMigrationAbstract)
                {
                    PowerVmAllocationPolicyMigrationAbstract vmAllocationPolicy = (PowerVmAllocationPolicyMigrationAbstract)datacenter.VmAllocationPolicy;

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

            Log.Disabled = true;
        }

        /// <summary>
        /// Parses the experiment name.
        /// </summary>
        /// <param name="name"> the name </param>
        /// <returns> the string </returns>
        public static string parseExperimentName(string name)
        {
            Scanner scanner = new Scanner(name);
            StringBuilder csvName = new StringBuilder();
            scanner.useDelimiter("_");
            for (int i = 0; i < 4; i++)
            {
                if (scanner.hasNext())
                {
                    csvName.Append(scanner.next() + ",");
                }
                else
                {
                    csvName.Append(",");
                }
            }
            scanner.close();
            return csvName.ToString();
        }
        /// <summary>
        /// Gets the sla time per active host.
        /// </summary>
        /// <param name="hosts"> the hosts </param>
        /// <returns> the sla time per active host </returns>
        protected internal static double getSlaTimePerActiveHost(IList<Host> hosts)
        {
            double slaViolationTimePerHost = 0;
            double totalTime = 0;

            foreach (Host _host in hosts)
            {
                HostDynamicWorkload host = (HostDynamicWorkload)_host;
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
        protected internal static double getSlaTimePerHost(IList<Host> hosts)
        {
            double slaViolationTimePerHost = 0;
            double totalTime = 0;

            foreach (Host _host in hosts)
            {
                HostDynamicWorkload host = (HostDynamicWorkload)_host;
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
        protected internal static IDictionary<string, double?> getSlaMetrics(IList<Vm> vms)
        {
            IDictionary<string, double?> metrics = new Dictionary<string, double?>();
            IList<double?> slaViolation = new List<double?>();
            double totalAllocated = 0;
            double totalRequested = 0;
            double totalUnderAllocatedDueToMigration = 0;

            foreach (Vm vm in vms)
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
        /// <param name="data"> the data </param>
        /// <param name="outputPath"> the output path </param>
        public static void writeDataColumn(IList<Number> data, string outputPath)
        {
            File file = new File(outputPath);
            try
            {
                file.createNewFile();
            }
            catch (IOException e1)
            {
                Console.WriteLine(e1.ToString());
                Console.Write(e1.StackTrace);
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
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Write data row.
        /// </summary>
        /// <param name="data"> the data </param>
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
                Console.WriteLine(e1.ToString());
                Console.Write(e1.StackTrace);
                Environment.Exit(0);
            }
            try
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
                writer.BaseStream.WriteByte(data);
                writer.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Write metric history.
        /// </summary>
        /// <param name="hosts"> the hosts </param>
        /// <param name="vmAllocationPolicy"> the vm allocation policy </param>
        /// <param name="outputPath"> the output path </param>
        public static void writeMetricHistory(IList<Host> hosts, PowerVmAllocationPolicyMigrationAbstract vmAllocationPolicy, string outputPath)
        {
            // for (Host host : hosts) {
            for (int j = 0; j < 10; j++)
            {
                Host host = hosts[j];

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
                    Console.WriteLine(e1.ToString());
                    Console.Write(e1.StackTrace);
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
                    Console.WriteLine(e.ToString());
                    Console.Write(e.StackTrace);
                    Environment.Exit(0);
                }
            }
        }

        /// <summary>
        /// Prints the Cloudlet objects.
        /// </summary>
        /// <param name="list"> list of Cloudlets </param>
        public static void printCloudletList(IList<Cloudlet> list)
        {
            int size = list.Count;
            Cloudlet cloudlet;

            string indent = "\t";
            Log.printLine();
            Log.printLine("========== OUTPUT ==========");
            Log.printLine("Cloudlet ID" + indent + "STATUS" + indent + "Resource ID" + indent + "VM ID" + indent + "Time" + indent + "Start Time" + indent + "Finish Time");

            // TODO: DecimalFormat
            //DecimalFormat dft = new DecimalFormat("###.##");
            for (int i = 0; i < size; i++)
            {
                cloudlet = list[i];
                Log.print(indent + cloudlet.CloudletId);

                if (cloudlet.CloudletStatus == Cloudlet.SUCCESS)
                {
                    //Log.printLine(indent + "SUCCESS" + indent + indent + cloudlet.ResourceId + indent + cloudlet.VmId + indent + dft.format(cloudlet.ActualCPUTime) + indent + dft.format(cloudlet.ExecStartTime) + indent + indent + dft.format(cloudlet.FinishTime));
                    Log.printLine(indent + "SUCCESS" + indent + indent + cloudlet.ResourceId + indent + cloudlet.VmId + indent + cloudlet.ActualCPUTime + indent + cloudlet.ExecStartTime + indent + indent + cloudlet.FinishTime);
                }
            }
        }

        /// <summary>
        /// Prints the metric history.
        /// </summary>
        /// <param name="hosts"> the hosts </param>
        /// <param name="vmAllocationPolicy"> the vm allocation policy </param>
        public static void printMetricHistory(IList<Host> hosts, PowerVmAllocationPolicyMigrationAbstract vmAllocationPolicy)
        {
            for (int i = 0; i < 10; i++)
            {
                Host host = hosts[i];

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
    }
}
