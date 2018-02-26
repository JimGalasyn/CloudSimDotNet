using System;

namespace org.cloudbus.cloudsim.examples.container
{
    /// <summary>
    /// This Example is following the format for <seealso cref="org.cloudbus.cloudsim.examples.power.planetlab.Dvfs"/>
    /// It specifically studies the initial placement of containers.
    /// 
    /// @author Sareh Fotuhi Piraghaj
    /// </summary>
    public class ContainerInitialPlacementTest
    {
        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args"> the arguments </param>
        /// <exception cref="IOException"> Signals that an I/O exception has occurred. </exception>
        public static void ExampleMain(string[] args)
        //public static void Main(string[] args)
        {
            /// <summary>
            /// The experiments can be repeated for (repeat - runtime +1) times.
            /// Please set these values as the arguments of the main function or set them bellow:
            /// </summary>
            int runTime = int.Parse(args[0]);
            int repeat = int.Parse(args[1]);
            for (int i = runTime; i < repeat; ++i)
            {
                bool enableOutput = true;
                bool outputToFile = true;
                /// <summary>
                /// Getting the path of the planet lab workload that is included in the cloudSim Package
                /// </summary>
                // TODO: Figure out class loading
                string inputFolder = null; // typeof(ContainerOverbooking).ClassLoader.getResource("workload/planetlab").Path;
                /// <summary>
                /// The output folder for the logs. The log files would be located in this folder.
                /// </summary>
                string outputFolder = "~/Results";
                /// <summary>
                /// The allocation policy for VMs.
                /// </summary>
                string vmAllocationPolicy = "MSThreshold-Under_0.80_0.70";
                /// <summary>
                /// The selection policy for containers where a container migration is triggered.
                /// </summary>
                string containerSelectionPolicy = "MaxUsage";
                /// <summary>
                /// The allocation policy used for allocating containers to VMs.
                /// </summary>
                string containerAllocationPolicy = "MostFull";
                /// <summary>
                /// The host selection policy determines which hosts should be selected as the migration destination.
                /// </summary>
                string hostSelectionPolicy = "FirstFit";
                /// <summary>
                /// The VM Selection Policy is used for selecting VMs to migrate when a host status is determined as
                /// "Overloaded"
                /// </summary>
                string vmSelectionPolicy = "VmMaxC";
                /// <summary>
                /// The container overbooking factor is used for overbooking resources of the VM. In this specific case
                /// the overbooking is performed on CPU only.
                /// </summary>

                int OverBookingFactor = 80;

                // TODO: Fix HelperEx
                //new RunnerInitiator(enableOutput, outputToFile, inputFolder, outputFolder, vmAllocationPolicy, containerAllocationPolicy, vmSelectionPolicy, containerSelectionPolicy, hostSelectionPolicy, OverBookingFactor, Convert.ToString(i), outputFolder);
            }
        }
    }
}