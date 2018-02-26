using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.examples.container
{
    using org.cloudbus.cloudsim.container.containerPlacementPolicies;
    using PowerContainerSelectionPolicy = org.cloudbus.cloudsim.container.containerSelectionPolicies.PowerContainerSelectionPolicy;
    using PowerContainerSelectionPolicyCor = org.cloudbus.cloudsim.container.containerSelectionPolicies.PowerContainerSelectionPolicyCor;
    using PowerContainerSelectionPolicyMaximumUsage = org.cloudbus.cloudsim.container.containerSelectionPolicies.PowerContainerSelectionPolicyMaximumUsage;
    using org.cloudbus.cloudsim.container.core;
    using org.cloudbus.cloudsim.container.hostSelectionPolicies;
    using PowerContainerVmAllocationPolicyMigrationAbstractHostSelection = org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled.PowerContainerVmAllocationPolicyMigrationAbstractHostSelection;
    using PowerContainerVmAllocationPolicyMigrationStaticThresholdMC = org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled.PowerContainerVmAllocationPolicyMigrationStaticThresholdMC;
    using PowerContainerVmAllocationPolicyMigrationStaticThresholdMCUnderUtilized = org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled.PowerContainerVmAllocationPolicyMigrationStaticThresholdMCUnderUtilized;
    using ContainerAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerAllocationPolicy;
    using ContainerAllocationPolicyRS = org.cloudbus.cloudsim.container.resourceAllocators.ContainerAllocationPolicyRS;
    using ContainerVmAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerVmAllocationPolicy;
    using PowerContainerAllocationPolicySimple = org.cloudbus.cloudsim.container.resourceAllocators.PowerContainerAllocationPolicySimple;
    using PowerContainerVmSelectionPolicy = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicy;
    using PowerContainerVmSelectionPolicyMaximumCorrelation = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicyMaximumCorrelation;
    using PowerContainerVmSelectionPolicyMaximumUsage = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicyMaximumUsage;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using System.Text;


    /// <summary>
    /// The RunnerAbs Class is the modified version of <seealso cref="org.cloudbus.cloudsim.examples.power.RunnerAbstract"/>
    /// Created by sareh on 18/08/15.
    /// </summary>
    public abstract class RunnerAbs
    {
        private static bool enableOutput;

        protected internal static ContainerDatacenterBroker broker;
        /// <summary>
        /// The vm list.
        /// </summary>
        protected internal static IList<ContainerVm> vmList;
        /// <summary>
        /// The container list.
        /// </summary>
        protected internal static IList<Container> containerList;

        /// <summary>
        /// The host list.
        /// </summary>
        protected internal static IList<ContainerHost> hostList;

        /// <summary>
        /// The Cloudlet List
        /// </summary>
        protected internal static IList<ContainerCloudlet> cloudletList;
        /// <summary>
        /// The overBooking Factor for containers
        /// </summary>
        private double overBookingFactor;


        private string experimentName;
        private string logAddress;


        private string runTime;


        public RunnerAbs(bool enableOutput, bool outputToFile, string inputFolder, string outputFolder, string vmAllocationPolicy, string containerAllocationPolicy, string vmSelectionPolicy, string containerSelectionPolicy, string hostSelectionPolicy, double overBookingFactor, string runTime, string logAddress)
        {
            OverBookingFactor = overBookingFactor;
            RunTime = runTime;
            LogAddress = logAddress;
            ExperimentName = this.getExperimentName(hostSelectionPolicy, vmAllocationPolicy, vmSelectionPolicy, containerSelectionPolicy, containerAllocationPolicy, OverBookingFactor.ToString(), runTime);

            try
            {
                this.initLogOutput(enableOutput, outputToFile, outputFolder, vmAllocationPolicy, vmSelectionPolicy, containerSelectionPolicy, hostSelectionPolicy);
            }
            catch (Exception var10)
            {
                Console.WriteLine(var10.ToString());
                Console.Write(var10.StackTrace);
                Environment.Exit(0);
            }

            this.init(inputFolder + "/", OverBookingFactor);
            this.start(ExperimentName, outputFolder, this.getVmAllocationPolicy(vmAllocationPolicy, vmSelectionPolicy, containerSelectionPolicy, hostSelectionPolicy), getContainerAllocationPolicy(containerAllocationPolicy));
        }

        public virtual string LogAddress
        {
            get
            {
                return logAddress;
            }
            set
            {
                this.logAddress = value;
            }
        }


        public virtual string RunTime
        {
            get
            {
                return runTime;
            }
            set
            {
                this.runTime = value;
            }
        }

        protected internal virtual void initLogOutput(bool enableOutput, bool outputToFile, string outputFolder, string vmAllocationPolicy, string vmSelectionPolicy, string containerSelectionPolicy, string hostSelectionPolicy)
        {
            this.EnableOutput = enableOutput;
            Log.Disabled = !this.EnableOutput;
            //        OutputStream out = new FileOutputStream("/home/sareh/Dropbox/programming/Results/log.txt");
            //OutputStream out = new BufferedOutputStream(new FileOutputStream("/home/sareh/Dropbox/programming/Results/log1.txt"), 100000);
            //        Log.setOutput(out);
            if (this.EnableOutput && outputToFile)
            {
                int index = ExperimentName.LastIndexOf("_", StringComparison.Ordinal);

                // TODO: File IO
#if false
                //            File folder1 = new File(outputFolder + "/stats/"+getExperimentName().substring(0, index));
                File folder = new File(outputFolder);
                File parent = folder.ParentFile;
                if (!parent.exists() && !parent.mkdirs())
                {
                    throw new System.InvalidOperationException("Couldn't create dir: " + parent);
                }
                if (!folder.exists())
                {
                    folder.mkdir();
                }
                File folder2 = new File(outputFolder + "/log/" + ExperimentName.Substring(0, index));
                File parent2 = folder2.ParentFile;
                if (!parent2.exists() && !parent2.mkdirs())
                {
                    throw new System.InvalidOperationException("Couldn't create dir: " + parent2);
                }
                if (!folder2.exists())
                {
                    folder2.mkdir();
                }
                File folder3 = new File(outputFolder + "/ContainerMigration/" + ExperimentName.Substring(0, index));
                File parent3 = folder3.ParentFile;
                if (!parent3.exists() && !parent3.mkdirs())
                {
                    throw new System.InvalidOperationException("Couldn't create dir: " + parent3);
                }
                if (!folder3.exists())
                {
                    folder3.mkdir();
                }

                File folder4 = new File(outputFolder + "/NewlyCreatedVms/" + ExperimentName.Substring(0, index));
                File parent4 = folder4.ParentFile;
                if (!parent4.exists() && !parent4.mkdirs())
                {
                    throw new System.InvalidOperationException("Couldn't create dir: " + parent4);
                }
                if (!folder4.exists())
                {
                    folder4.mkdir();
                }
                File folder5 = new File(outputFolder + "/EnergyConsumption/" + ExperimentName.Substring(0, index));
                File parent5 = folder5.ParentFile;
                if (!parent5.exists() && !parent5.mkdirs())
                {
                    throw new System.InvalidOperationException("Couldn't create dir: " + parent5);
                }

                if (!folder5.exists())
                {
                    folder5.mkdir();
                }

                File file = new File(outputFolder + "/log/" + ExperimentName.Substring(0, index) + "/" + this.getExperimentName(new string[] { hostSelectionPolicy, vmAllocationPolicy, vmSelectionPolicy, containerSelectionPolicy, OverBookingFactor.ToString(), RunTime }) + ".txt");
                file.createNewFile();
                Log.Output = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write);
#endif
            }

        }

        public virtual string ExperimentName
        {
            get
            {
                return experimentName;
            }
            set
            {
                this.experimentName = value;
            }
        }


        protected internal abstract void init(string var1, double overBookingFactor);

        protected internal virtual void start(string experimentName, string outputFolder, ContainerVmAllocationPolicy vmAllocationPolicy, ContainerAllocationPolicy containerAllocationPolicy)
        {
            Console.WriteLine("Starting " + experimentName);

            try
            {
                // TODO: Fix HelperEx
                PowerContainerDatacenter e = null; // (PowerContainerDatacenter)HelperEx.createDatacenter("datacenter", typeof(PowerContainerDatacenterCM), hostList, vmAllocationPolicy, containerAllocationPolicy, ExperimentName, ConstantsExamples.SCHEDULING_INTERVAL, LogAddress, ConstantsExamples.VM_STARTTUP_DELAY, ConstantsExamples.CONTAINER_STARTTUP_DELAY);
                //            PowerContainerDatacenter e = (PowerContainerDatacenter) HelperEx.createDatacenter("Datacenter", PowerContainerDatacenter.class, hostList, vmAllocationPolicy, containerAllocationPolicy);
                vmAllocationPolicy.Datacenter = e;
                e.DisableVmMigrations = false;
                broker.submitVmList(vmList);
                broker.submitContainerList(containerList);
                // TODO: subList replacement
                //broker.submitCloudletList(cloudletList.subList(0, containerList.size()));
                ;
                CloudSim.terminateSimulation(86400.0D);
                double lastClock = CloudSim.startSimulation();
                var newList = broker.CloudletReceivedList;
                Log.printLine("Received " + newList.Count + " cloudlets");
                CloudSim.stopSimulation();

                //            HelperEx.printResults(e, broker.getVmsCreatedList(),broker.getContainersCreatedList() ,lastClock, experimentName, true, outputFolder);
                // TODO: Fix HelperEx
                //HelperEx.printResultsNew(e, broker, lastClock, experimentName, true, outputFolder);
            }
            catch (Exception var8)
            {
                Console.WriteLine(var8.ToString());
                Console.Write(var8.StackTrace);
                Log.printLine("The simulation has been terminated due to an unexpected error");
                Environment.Exit(0);
            }

            Log.printLine("Finished " + experimentName);
        }

        protected internal virtual string getExperimentName(params string[] args)
        {
            StringBuilder experimentName = new StringBuilder();

            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].Length > 0)
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


        protected internal virtual ContainerVmAllocationPolicy getVmAllocationPolicy(string vmAllocationPolicyName, string vmSelectionPolicyName, string containerSelectionPolicyName, string hostSelectionPolicyName)
        {
            object vmAllocationPolicy = null;
            PowerContainerVmSelectionPolicy vmSelectionPolicy = null;
            PowerContainerSelectionPolicy containerSelectionPolicy = null;
            HostSelectionPolicy hostSelectionPolicy = null;
            if (vmSelectionPolicyName.Length > 0 && containerSelectionPolicyName.Length > 0 && hostSelectionPolicyName.Length > 0)
            {
                vmSelectionPolicy = this.getVmSelectionPolicy(vmSelectionPolicyName);
                containerSelectionPolicy = this.getContainerSelectionPolicy(containerSelectionPolicyName);
                hostSelectionPolicy = this.getHostSelectionPolicy(hostSelectionPolicyName);
            }


            if (vmAllocationPolicyName.StartsWith("MSThreshold-Over_", StringComparison.Ordinal))
            {
                double overUtilizationThreshold = double.Parse(vmAllocationPolicyName.Substring(18));
                vmAllocationPolicy = new PowerContainerVmAllocationPolicyMigrationStaticThresholdMC(hostList, vmSelectionPolicy, containerSelectionPolicy, hostSelectionPolicy, overUtilizationThreshold, ConstantsExamples.VM_TYPES, ConstantsExamples.VM_PES, ConstantsExamples.VM_RAM, ConstantsExamples.VM_BW, ConstantsExamples.VM_SIZE, ConstantsExamples.VM_MIPS);
            }
            else if (vmAllocationPolicyName.StartsWith("MSThreshold-Under_", StringComparison.Ordinal))
            {
                double overUtilizationThreshold = double.Parse(vmAllocationPolicyName.Substring(18, 4));
                double underUtilizationThreshold = double.Parse(vmAllocationPolicyName.Substring(24));
                vmAllocationPolicy = new PowerContainerVmAllocationPolicyMigrationStaticThresholdMCUnderUtilized(hostList, vmSelectionPolicy, containerSelectionPolicy, hostSelectionPolicy, overUtilizationThreshold, underUtilizationThreshold, ConstantsExamples.VM_TYPES, ConstantsExamples.VM_PES, ConstantsExamples.VM_RAM, ConstantsExamples.VM_BW, ConstantsExamples.VM_SIZE, ConstantsExamples.VM_MIPS);

            }
            else if (vmAllocationPolicyName.StartsWith("VMThreshold-Under_", StringComparison.Ordinal))
            {

                double overUtilizationThreshold = double.Parse(vmAllocationPolicyName.Substring(18, 4));
                double underUtilizationThreshold = double.Parse(vmAllocationPolicyName.Substring(24));
                vmAllocationPolicy = new PowerContainerVmAllocationPolicyMigrationAbstractHostSelection(hostList, vmSelectionPolicy, hostSelectionPolicy, overUtilizationThreshold, underUtilizationThreshold);


            }
            else
            {
                Console.WriteLine("Unknown VM allocation policy: " + vmAllocationPolicyName);
                Environment.Exit(0);
            }

            return (ContainerVmAllocationPolicy)vmAllocationPolicy;
        }

        protected internal virtual ContainerAllocationPolicy getContainerAllocationPolicy(string containerAllocationPolicyName)
        {
            ContainerAllocationPolicy containerAllocationPolicy;
            if (string.ReferenceEquals(containerAllocationPolicyName, "Simple"))
            {

                containerAllocationPolicy = new PowerContainerAllocationPolicySimple(); // DVFS policy without VM migrations
            }
            else
            {

                ContainerPlacementPolicy placementPolicy = getContainerPlacementPolicy(containerAllocationPolicyName);
                containerAllocationPolicy = new ContainerAllocationPolicyRS(placementPolicy); // DVFS policy without VM migrations
            }

            return containerAllocationPolicy;

        }

        protected internal virtual ContainerPlacementPolicy getContainerPlacementPolicy(string name)
        {
            ContainerPlacementPolicy placementPolicy;
            switch (name)
            {
                case "LeastFull":
                    placementPolicy = new ContainerPlacementPolicyLeastFull();
                    break;
                case "MostFull":
                    placementPolicy = new ContainerPlacementPolicyMostFull();
                    break;

                case "FirstFit":
                    placementPolicy = new ContainerPlacementPolicyFirstFit();
                    break;
                case "Random":
                    placementPolicy = new ContainerPlacementPolicyRandomSelection();
                    break;
                default:
                    placementPolicy = null;
                    Console.WriteLine("The container placement policy is not defined");
                    break;
            }
            return placementPolicy;
        }

        protected internal virtual HostSelectionPolicy getHostSelectionPolicy(string hostSelectionPolicyName)
        {
            object hostSelectionPolicy = null;
            if (string.ReferenceEquals(hostSelectionPolicyName, "FirstFit"))
            {

                hostSelectionPolicy = new HostSelectionPolicyFirstFit();


            }
            else if (string.ReferenceEquals(hostSelectionPolicyName, "LeastFull"))
            {

                hostSelectionPolicy = new HostSelectionPolicyLeastFull();


            }
            else if (string.ReferenceEquals(hostSelectionPolicyName, "MostFull"))
            {

                hostSelectionPolicy = new HostSelectionPolicyMostFull();


            }
            //        else if (hostSelectionPolicyName == "MinCor") {

            //            hostSelectionPolicy = new HostSelectionPolicyMinimumCorrelation();


            //        }
            else if (string.ReferenceEquals(hostSelectionPolicyName, "RandomSelection"))
            {

                hostSelectionPolicy = new HostSelectionPolicyRandomSelection();


            }
            // else if(vmSelectionPolicyName.equals("mmt")) {
            //            vmSelectionPolicy = new PowerVmSelectionPolicyMinimumMigrationTime();
            //        } else if(vmSelectionPolicyName.equals("mu")) {
            //            vmSelectionPolicy = new PowerVmSelectionPolicyMinimumUtilization();
            //        } else if(vmSelectionPolicyName.equals("rs")) {
            //            vmSelectionPolicy = new PowerVmSelectionPolicyRandomSelection();
            //        }
            else
            {
                Console.WriteLine("Unknown Host selection policy: " + hostSelectionPolicyName);
                Environment.Exit(0);
            }

            return (HostSelectionPolicy)hostSelectionPolicy;
        }


        protected internal virtual PowerContainerSelectionPolicy getContainerSelectionPolicy(string containerSelectionPolicyName)
        {
            object containerSelectionPolicy = null;
            if (containerSelectionPolicyName.Equals("Cor"))
            {
                containerSelectionPolicy = new PowerContainerSelectionPolicyCor(new PowerContainerSelectionPolicyMaximumUsage());
            }
            else if (containerSelectionPolicyName.Equals("MaxUsage"))
            {
                containerSelectionPolicy = new PowerContainerSelectionPolicyMaximumUsage();


            }
            // else if(vmSelectionPolicyName.equals("mmt")) {
            //            vmSelectionPolicy = new PowerVmSelectionPolicyMinimumMigrationTime();
            //        } else if(vmSelectionPolicyName.equals("mu")) {
            //            vmSelectionPolicy = new PowerVmSelectionPolicyMinimumUtilization();
            //        } else if(vmSelectionPolicyName.equals("rs")) {
            //            vmSelectionPolicy = new PowerVmSelectionPolicyRandomSelection();
            //        }
            else
            {
                Console.WriteLine("Unknown Container selection policy: " + containerSelectionPolicyName);
                Environment.Exit(0);
            }

            return (PowerContainerSelectionPolicy)containerSelectionPolicy;
        }


        protected internal virtual PowerContainerVmSelectionPolicy getVmSelectionPolicy(string vmSelectionPolicyName)
        {
            object vmSelectionPolicy = null;
            if (vmSelectionPolicyName.Equals("VmMaxC"))
            {
                vmSelectionPolicy = new PowerContainerVmSelectionPolicyMaximumCorrelation(new PowerContainerVmSelectionPolicyMaximumUsage());
            }
            else if (vmSelectionPolicyName.Equals("VmMaxU"))
            {
                vmSelectionPolicy = new PowerContainerVmSelectionPolicyMaximumUsage();
            }
            // else if(vmSelectionPolicyName.equals("mmt")) {
            //            vmSelectionPolicy = new PowerVmSelectionPolicyMinimumMigrationTime();
            //        } else if(vmSelectionPolicyName.equals("mu")) {
            //            vmSelectionPolicy = new PowerVmSelectionPolicyMinimumUtilization();
            //        } else if(vmSelectionPolicyName.equals("rs")) {
            //            vmSelectionPolicy = new PowerVmSelectionPolicyRandomSelection();
            //        }
            else
            {
                Console.WriteLine("Unknown VM selection policy: " + vmSelectionPolicyName);
                Environment.Exit(0);
            }

            return (PowerContainerVmSelectionPolicy)vmSelectionPolicy;
        }

        public virtual bool EnableOutput
        {
            set
            {
                // TODO: really a static property?
                //this.enableOutput = value;
                RunnerAbs.enableOutput = value;
            }
            get
            {
                return enableOutput;
            }
        }
        public virtual double OverBookingFactor
        {
            get
            {
                return overBookingFactor;
            }
            set
            {
                this.overBookingFactor = value;
            }
        }
    }
}
        

