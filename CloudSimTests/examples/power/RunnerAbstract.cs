using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.examples.power
{
    using System.Diagnostics;
    using System.Text;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using PowerDatacenter = org.cloudbus.cloudsim.power.PowerDatacenter;
    using PowerHost = org.cloudbus.cloudsim.power.PowerHost;
    using PowerVmAllocationPolicyMigrationAbstract = org.cloudbus.cloudsim.power.PowerVmAllocationPolicyMigrationAbstract;
    using PowerVmAllocationPolicyMigrationInterQuartileRange = org.cloudbus.cloudsim.power.PowerVmAllocationPolicyMigrationInterQuartileRange;
    using PowerVmAllocationPolicyMigrationLocalRegression = org.cloudbus.cloudsim.power.PowerVmAllocationPolicyMigrationLocalRegression;
    using PowerVmAllocationPolicyMigrationLocalRegressionRobust = org.cloudbus.cloudsim.power.PowerVmAllocationPolicyMigrationLocalRegressionRobust;
    using PowerVmAllocationPolicyMigrationMedianAbsoluteDeviation = org.cloudbus.cloudsim.power.PowerVmAllocationPolicyMigrationMedianAbsoluteDeviation;
    using PowerVmAllocationPolicyMigrationStaticThreshold = org.cloudbus.cloudsim.power.PowerVmAllocationPolicyMigrationStaticThreshold;
    using PowerVmAllocationPolicySimple = org.cloudbus.cloudsim.power.PowerVmAllocationPolicySimple;
    using PowerVmSelectionPolicy = org.cloudbus.cloudsim.power.PowerVmSelectionPolicy;
    using PowerVmSelectionPolicyMaximumCorrelation = org.cloudbus.cloudsim.power.PowerVmSelectionPolicyMaximumCorrelation;
    using PowerVmSelectionPolicyMinimumMigrationTime = org.cloudbus.cloudsim.power.PowerVmSelectionPolicyMinimumMigrationTime;
    using PowerVmSelectionPolicyMinimumUtilization = org.cloudbus.cloudsim.power.PowerVmSelectionPolicyMinimumUtilization;
    using PowerVmSelectionPolicyRandomSelection = org.cloudbus.cloudsim.power.PowerVmSelectionPolicyRandomSelection;

    /// <summary>
    /// The Class RunnerAbstract.
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
    public abstract class RunnerAbstract
    {
        /// <summary>
        /// The enable output. </summary>
        private static bool enableOutput;

        /// <summary>
        /// The broker. </summary>
        protected internal static DatacenterBroker broker;

        /// <summary>
        /// The cloudlet list. </summary>
        protected internal static IList<Cloudlet> cloudletList;

        /// <summary>
        /// The vm list. </summary>
        protected internal static IList<Vm> vmList;

        /// <summary>
        /// The host list. </summary>
        protected internal static IList<PowerHost> hostList;

        /// <summary>
        /// Run.
        /// </summary>
        /// <param name="enableOutput"> the enable output </param>
        /// <param name="outputToFile"> the output to file </param>
        /// <param name="inputFolder"> the input folder </param>
        /// <param name="outputFolder"> the output folder </param>
        /// <param name="workload"> the workload </param>
        /// <param name="vmAllocationPolicy"> the vm allocation policy </param>
        /// <param name="vmSelectionPolicy"> the vm selection policy </param>
        /// <param name="parameter"> the parameter </param>
        public RunnerAbstract(bool enableOutput, bool outputToFile, string inputFolder, string outputFolder, string workload, string vmAllocationPolicy, string vmSelectionPolicy, string parameter)
        {
            try
            {
                initLogOutput(enableOutput, outputToFile, outputFolder, workload, vmAllocationPolicy, vmSelectionPolicy, parameter);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                //Environment.Exit(0);
                throw e;
            }
            init(inputFolder + "/" + workload);
            // TODO: correct number of args for start method
            //start(getExperimentName(workload, vmAllocationPolicy, vmSelectionPolicy, parameter), outputFolder, getVmAllocationPolicy(vmAllocationPolicy, vmSelectionPolicy, parameter));
        }

        /// <summary>
        /// Inits the log output.
        /// </summary>
        /// <param name="enableOutput"> the enable output </param>
        /// <param name="outputToFile"> the output to file </param>
        /// <param name="outputFolder"> the output folder </param>
        /// <param name="workload"> the workload </param>
        /// <param name="vmAllocationPolicy"> the vm allocation policy </param>
        /// <param name="vmSelectionPolicy"> the vm selection policy </param>
        /// <param name="parameter"> the parameter </param>
        /// <exception cref="IOException"> Signals that an I/O exception has occurred. </exception>
        /// <exception cref="FileNotFoundException"> the file not found exception </exception>
        protected void initLogOutput(bool enableOutput, bool outputToFile, string outputFolder, string workload, string vmAllocationPolicy, string vmSelectionPolicy, string parameter)
        {

            EnableOutput = enableOutput;
            Log.Disabled = !EnableOutput;
            if (EnableOutput && outputToFile)
            {
                // TODO: file IO
#if false
                File folder = new File(outputFolder);
                if (!folder.exists())
                {
                    folder.mkdir();
                }

                File folder2 = new File(outputFolder + "/log");
                if (!folder2.exists())
                {
                    folder2.mkdir();
                }

                File file = new File(outputFolder + "/log/" + getExperimentName(workload, vmAllocationPolicy, vmSelectionPolicy, parameter) + ".txt");
                file.createNewFile();
                Log.Output = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.Write);
#endif
            }
        }

        /// <summary>
        /// Inits the simulation.
        /// </summary>
        /// <param name="inputFolder"> the input folder </param>
        protected abstract void init(string inputFolder);

        /// <summary>
        /// Starts the simulation.
        /// </summary>
        /// <param name="experimentName"> the experiment name </param>
        /// <param name="outputFolder"> the output folder </param>
        /// <param name="vmAllocationPolicy"> the vm allocation policy </param>
        protected void start(string experimentName, string outputFolder, VmAllocationPolicy vmAllocationPolicy)
        {
            Debug.WriteLine("Starting " + experimentName);

            try
            {
                // TODO: Fix HelperEx
                PowerDatacenter datacenter = null; //(PowerDatacenter)Helper.createDatacenter("Datacenter", typeof(PowerDatacenter), hostList, vmAllocationPolicy);

                datacenter.DisableMigrations = false;

                broker.submitVmList(vmList);
                broker.submitCloudletList(cloudletList);

                CloudSim.terminateSimulation(Constants.SIMULATION_LIMIT);
                double lastClock = CloudSim.startSimulation();

                IList<Cloudlet> newList = broker.CloudletReceivedListProperty;
                Log.printLine("Received " + newList.Count + " cloudlets");

                CloudSim.stopSimulation();

                // TODO: Fix HelperEx
                //Helper.printResults(datacenter, vmList, lastClock, experimentName, Constants.OUTPUT_CSV, outputFolder);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                Log.printLine("The simulation has been terminated due to an unexpected error");
                //Environment.Exit(0);
                throw e;
            }
            Log.printLine("Finished " + experimentName);
        }

        /// <summary>
        /// Gets the experiment name.
        /// </summary>
        /// <param name="args"> the args </param>
        /// <returns> the experiment name </returns>
        protected string getExperimentName(string args)
        {
            StringBuilder experimentName = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                // TODO: Char.IsWhiteSpace == Empty
                //if (args[i].Empty)
                if (!Char.IsWhiteSpace(args[i]))
                {
                    continue;
                }
                if (i != 0)
                {
                    experimentName.Append("_");
                }
                experimentName.Append(args[i]);
            }
            return experimentName.ToString();
        }

        /// <summary>
        /// Gets the vm allocation policy.
        /// </summary>
        /// <param name="vmAllocationPolicyName"> the vm allocation policy name </param>
        /// <param name="vmSelectionPolicyName"> the vm selection policy name </param>
        /// <param name="parameterName"> the parameter name </param>
        /// <returns> the vm allocation policy </returns>
        protected VmAllocationPolicy getVmAllocationPolicy(string vmAllocationPolicyName, string vmSelectionPolicyName, string parameterName)
        {
            VmAllocationPolicy vmAllocationPolicy = null;
            PowerVmSelectionPolicy vmSelectionPolicy = null;
            //if (!vmSelectionPolicyName.Empty)
            if (!String.IsNullOrEmpty(vmSelectionPolicyName))
            {
                vmSelectionPolicy = getVmSelectionPolicy(vmSelectionPolicyName);
            }
            double parameter = 0;
            //if (!parameterName.Empty)
            if(!String.IsNullOrEmpty(parameterName))
            {
                parameter = Convert.ToDouble(parameterName);
            }
            if (vmAllocationPolicyName.Equals("iqr"))
            {
                // TODO: hostList type
                //PowerVmAllocationPolicyMigrationAbstract fallbackVmSelectionPolicy = new PowerVmAllocationPolicyMigrationStaticThreshold(hostList, vmSelectionPolicy, 0.7);
                //vmAllocationPolicy = new PowerVmAllocationPolicyMigrationInterQuartileRange(hostList, vmSelectionPolicy, parameter, fallbackVmSelectionPolicy);
            }
            else if (vmAllocationPolicyName.Equals("mad"))
            {
                // TODO: hostList type
                //PowerVmAllocationPolicyMigrationAbstract fallbackVmSelectionPolicy = new PowerVmAllocationPolicyMigrationStaticThreshold(hostList, vmSelectionPolicy, 0.7);
                //vmAllocationPolicy = new PowerVmAllocationPolicyMigrationMedianAbsoluteDeviation(hostList, vmSelectionPolicy, parameter, fallbackVmSelectionPolicy);
            }
            else if (vmAllocationPolicyName.Equals("lr"))
            {
                // TODO: hostList type
                //PowerVmAllocationPolicyMigrationAbstract fallbackVmSelectionPolicy = new PowerVmAllocationPolicyMigrationStaticThreshold(hostList, vmSelectionPolicy, 0.7);
                //vmAllocationPolicy = new PowerVmAllocationPolicyMigrationLocalRegression(hostList, vmSelectionPolicy, parameter, Constants.SCHEDULING_INTERVAL, fallbackVmSelectionPolicy);
            }
            else if (vmAllocationPolicyName.Equals("lrr"))
            {
                // TODO: hostList type
                //PowerVmAllocationPolicyMigrationAbstract fallbackVmSelectionPolicy = new PowerVmAllocationPolicyMigrationStaticThreshold(hostList, vmSelectionPolicy, 0.7);
                //vmAllocationPolicy = new PowerVmAllocationPolicyMigrationLocalRegressionRobust(hostList, vmSelectionPolicy, parameter, Constants.SCHEDULING_INTERVAL, fallbackVmSelectionPolicy);
            }
            else if (vmAllocationPolicyName.Equals("thr"))
            {
                // TODO: hostList type
                //vmAllocationPolicy = new PowerVmAllocationPolicyMigrationStaticThreshold(hostList, vmSelectionPolicy, parameter);
            }
            else if (vmAllocationPolicyName.Equals("dvfs"))
            {
                // TODO: hostList type
                //vmAllocationPolicy = new PowerVmAllocationPolicySimple(hostList);
            }
            else
            {
                Debug.WriteLine("Unknown VM allocation policy: " + vmAllocationPolicyName);
                //Environment.Exit(0);
                throw new InvalidOperationException("Unknown VM allocation policy: " + vmAllocationPolicyName);
            }
            return vmAllocationPolicy;
        }

        /// <summary>
        /// Gets the vm selection policy.
        /// </summary>
        /// <param name="vmSelectionPolicyName"> the vm selection policy name </param>
        /// <returns> the vm selection policy </returns>
        protected internal virtual PowerVmSelectionPolicy getVmSelectionPolicy(string vmSelectionPolicyName)
        {
            PowerVmSelectionPolicy vmSelectionPolicy = null;
            if (vmSelectionPolicyName.Equals("mc"))
            {
                vmSelectionPolicy = new PowerVmSelectionPolicyMaximumCorrelation(new PowerVmSelectionPolicyMinimumMigrationTime());
            }
            else if (vmSelectionPolicyName.Equals("mmt"))
            {
                vmSelectionPolicy = new PowerVmSelectionPolicyMinimumMigrationTime();
            }
            else if (vmSelectionPolicyName.Equals("mu"))
            {
                vmSelectionPolicy = new PowerVmSelectionPolicyMinimumUtilization();
            }
            else if (vmSelectionPolicyName.Equals("rs"))
            {
                vmSelectionPolicy = new PowerVmSelectionPolicyRandomSelection();
            }
            else
            {
                Debug.WriteLine("Unknown VM selection policy: " + vmSelectionPolicyName);
                //Environment.Exit(0);
                throw new InvalidOperationException("Unknown VM selection policy: " + vmSelectionPolicyName);
            }
            return vmSelectionPolicy;
        }

        /// <summary>
        /// Sets the enable output.
        /// </summary>
        /// <param name="enableOutput"> the new enable output </param>
        public virtual bool EnableOutput
        {
            set
            {
                RunnerAbstract.enableOutput = value;
            }
            get
            {
                return enableOutput;
            }
        }
    }
}
