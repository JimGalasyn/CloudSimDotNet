namespace org.cloudbus.cloudsim.examples.container
{
    using PowerModel = org.cloudbus.cloudsim.power.models.PowerModel;
    using PowerModelSpecPowerHpProLiantMl110G4Xeon3040 = org.cloudbus.cloudsim.power.models.PowerModelSpecPowerHpProLiantMl110G4Xeon3040;
    using PowerModelSpecPowerHpProLiantMl110G5Xeon3075 = org.cloudbus.cloudsim.power.models.PowerModelSpecPowerHpProLiantMl110G5Xeon3075;
    using PowerModelSpecPowerIbmX3550XeonX5670 = org.cloudbus.cloudsim.power.models.PowerModelSpecPowerIbmX3550XeonX5670;

    /// <summary>
    /// In this class the specifications of the Cloudlets, Containers, VMs and Hosts are coded.
    /// Regarding to the hosts, the powermodel of each type of the hosts are all included in this class.
    /// </summary>
    public class ConstantsExamples
    {
        /// <summary>
        /// Simulation  parameters including the interval and limit
        /// </summary>
        public const bool ENABLE_OUTPUT = true;
        public const bool OUTPUT_CSV = false;
        public const double SCHEDULING_INTERVAL = 300.0D;
        public const double SIMULATION_LIMIT = 87400.0D;
        /// <summary>
        /// Cloudlet specs
        /// </summary>
        public const int CLOUDLET_LENGTH = 30;
        public const int CLOUDLET_PES = 1;

        /// <summary>
        /// Startup delay for VMs and the containers are mentioned here.
        /// </summary>
        public const double CONTAINER_STARTTUP_DELAY = 0.4; //the amount is in seconds
        public const double VM_STARTTUP_DELAY = 100; //the amoun is in seconds

        /// <summary>
        /// The available virtual machine types along with the specs.
        /// </summary>

        public const int VM_TYPES = 4;
        public static readonly double[] VM_MIPS = new double[] { 37274 / 2, 37274 / 2, 37274 / 2, 37274 / 2 };
        public static readonly int[] VM_PES = new int[] { 2, 4, 1, 8 };
        public static readonly float[] VM_RAM = new float[] { (float)1024, (float)2048, (float)4096, (float)8192 }; //**MB*
        public const int VM_BW = 100000;
        public const int VM_SIZE = 2500;

        /// <summary>
        /// The available types of container along with the specs.
        /// </summary>

        public const int CONTAINER_TYPES = 3;
        public static readonly int[] CONTAINER_MIPS = new int[] { 4658, 9320, 18636 };
        public static readonly int[] CONTAINER_PES = new int[] { 1, 1, 1 };
        public static readonly int[] CONTAINER_RAM = new int[] { 128, 256, 512 };
        public const int CONTAINER_BW = 2500;

        /// <summary>
        /// The available types of hosts along with the specs.
        /// </summary>

        public const int HOST_TYPES = 3;
        public static readonly int[] HOST_MIPS = new int[] { 37274, 37274, 37274 };
        public static readonly int[] HOST_PES = new int[] { 4, 8, 16 };
        public static readonly int[] HOST_RAM = new int[] { 65536, 131072, 262144 };
        public const int HOST_BW = 1000000;
        public const int HOST_STORAGE = 1000000;
        public static readonly PowerModel[] HOST_POWER = new PowerModel[]
        {
            new PowerModelSpecPowerHpProLiantMl110G4Xeon3040(),
            new PowerModelSpecPowerHpProLiantMl110G5Xeon3075(),
            new PowerModelSpecPowerIbmX3550XeonX5670()
        };

        /// <summary>
        /// The population of hosts, containers, and VMs are specified.
        /// The containers population is equal to the cloudlets population as each cloudlet is mapped to each container.
        /// However, depending on the simualtion scenario the container's population can also be different from cloudlet's
        /// population.
        /// </summary>


        public const int NUMBER_HOSTS = 20;
        public const int NUMBER_VMS = 25;
        public const int NUMBER_CLOUDLETS = 50;

        //-----------------------The Addresses


        //    public static String logAddress;
        //
        //    public String getLogAddress() {
        //        return logAddress;
        //    }
        //
        //    public void setLogAddress(String logAddress) {
        //        this.logAddress = logAddress;
        //    }
        //

        public ConstantsExamples()
        {
        }
    }
}