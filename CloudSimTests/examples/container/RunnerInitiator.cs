using System;

namespace org.cloudbus.cloudsim.examples.container
{
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

    /// <summary>
    /// This is the modified version of <seealso cref="org.cloudbus.cloudsim.examples.power.planetlab.PlanetLabRunner"/> in CloudSim Package.
    /// Created by sareh on 18/08/15.
    /// </summary>

    public class RunnerInitiator : RunnerAbs
    {
        /// <summary>
        /// Instantiates a new runner.
        /// </summary>
        /// <param name="enableOutput">       the enable output </param>
        /// <param name="outputToFile">       the output to file </param>
        /// <param name="inputFolder">        the input folder </param>
        /// <param name="outputFolder">       the output folder </param>
        ///                           //     * <param name="workload"> the workload </param>
        /// <param name="vmAllocationPolicy"> the vm allocation policy </param>
        /// <param name="vmSelectionPolicy">  the vm selection policy </param>
        public RunnerInitiator(bool enableOutput, bool outputToFile, string inputFolder, string outputFolder, string vmAllocationPolicy, string containerAllocationPolicy, string vmSelectionPolicy, string containerSelectionPolicy, string hostSelectionPolicy, double overBookingFactor, string runTime, string logAddress) : base(enableOutput, outputToFile, inputFolder, outputFolder, vmAllocationPolicy, containerAllocationPolicy, vmSelectionPolicy, containerSelectionPolicy, hostSelectionPolicy, overBookingFactor, runTime, logAddress)
        {



        }

        /*
		 * (non-Javadoc)
		 *
		 * @see RunnerAbs
		 */
        protected internal override void init(string inputFolder, double overBookingFactor)
        {
            try
            {
                CloudSim.init(1, new DateTime(), false);
                //            setOverBookingFactor(overBookingFactor);
                broker = HelperEx.createBroker(overBookingFactor);
                int brokerId = broker.Id;
                cloudletList = HelperEx.createContainerCloudletList(brokerId, inputFolder, ConstantsExamples.NUMBER_CLOUDLETS);
                containerList = HelperEx.createContainerList(brokerId, ConstantsExamples.NUMBER_CLOUDLETS);
                vmList = HelperEx.createVmList(brokerId, ConstantsExamples.NUMBER_VMS);
                hostList = HelperEx.createHostList(ConstantsExamples.NUMBER_HOSTS);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Log.printLine("The simulation has been terminated due to an unexpected error");
                throw e;
                //Environment.Exit(0);
            }
        }
    }
}