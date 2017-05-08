using System;

namespace org.cloudbus.cloudsim.examples.container
{
    /// <summary>
    /// This Example is following the format for <seealso cref="org.cloudbus.cloudsim.examples.power.planetlab.Dvfs"/>
    /// It specifically studies the placement of containers.
    /// 
    /// @author Sareh Fotuhi Piraghaj
    /// </summary>
    public class ContainerOverbooking
    {
        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args"> the arguments </param>
        /// <exception cref="IOException"> Signals that an I/O exception has occurred. </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static void main(String[] args) throws java.io.IOException
        public static void ExampleMain(string[] args)
        //public static void Main(string[] args)
        {
            /// <summary>
            /// The experiments can be repeated for (repeat - runtime +1) times.
            /// Please set these values as the arguments of the main function or set them bellow:
            /// </summary>
            int runTime = int.Parse(args[0]);
            int repeat = int.Parse(args[1]);
            for (int i = 10; i < repeat; i += 10)
            {
                bool enableOutput = true;
                bool outputToFile = true;
                /// <summary>
                /// Getting the path of the planet lab workload that is included in the cloudSim Package
                /// </summary>
                // TODO: Figure out class loader
                string inputFolder = null; // typeof(ContainerOverbooking).ClassLoader.getResource("workload/planetlab").Path;
                /// <summary>
                /// The output folder for the logs. The log files would be located in this folder.
                /// </summary>
                string outputFolder = "/Results";
                /// <summary>
                /// The allocation policy for VMs. It has the under utilization and over utilization thresholds used for
                /// determining the underloaded and oberloaded hosts.
                /// </summary>
                string vmAllocationPolicy = "MSThreshold-Under_0.80_0.70"; // DVFS policy without VM migrations
                                                                           /// <summary>
                                                                           /// The selection policy for containers where a container migration is triggered.
                                                                           /// </summary>
                string containerSelectionPolicy = "Cor";
                /// <summary>
                /// The allocation policy used for allocating containers to VMs.
                /// </summary>
                string containerAllocationPolicy = "MostFull";
                //            String containerAllocationPolicy= "FirstFit";
                //            String containerAllocationPolicy= "LeastFull";
                //            String containerAllocationPolicy= "Simple";
                //            String containerAllocationPolicy = "Random";

                /// <summary>
                /// The host selection policy determines which hosts should be selected as the migration destination.
                /// </summary>
                string hostSelectionPolicy = "FirstFit";
                /// <summary>
                /// The VM Selection Policy is used for selecting VMs to migrate when a host status is determined as
                /// "Overloaded"
                /// </summary>
                string vmSelectionPolicy = "VmMaxC";

                // TODO: Fix HelperEx
                //new RunnerInitiator(enableOutput, outputToFile, inputFolder, outputFolder, vmAllocationPolicy, containerAllocationPolicy, vmSelectionPolicy, containerSelectionPolicy, hostSelectionPolicy, i, Convert.ToString(runTime), outputFolder);
            }
        }
    }
}