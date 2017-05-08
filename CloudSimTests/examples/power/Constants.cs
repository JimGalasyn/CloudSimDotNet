namespace org.cloudbus.cloudsim.examples.power
{

    using PowerModel = org.cloudbus.cloudsim.power.models.PowerModel;
    using PowerModelSpecPowerHpProLiantMl110G4Xeon3040 = org.cloudbus.cloudsim.power.models.PowerModelSpecPowerHpProLiantMl110G4Xeon3040;
    using PowerModelSpecPowerHpProLiantMl110G5Xeon3075 = org.cloudbus.cloudsim.power.models.PowerModelSpecPowerHpProLiantMl110G5Xeon3075;

    /// <summary>
    /// If you are using any algorithms, policies or workload included in the power package, please cite
    /// the following paper:
    /// 
    /// Anton Beloglazov, and Rajkumar Buyya, "Optimal Online Deterministic Algorithms and Adaptive
    /// Heuristics for Energy and Performance Efficient Dynamic Consolidation of Virtual Machines in
    /// Cloud Data Centers", Concurrency and Computation: Practice and Experience (CCPE), Volume 24,
    /// Issue 13, Pages: 1397-1420, John Wiley & Sons, Ltd, New York, USA, 2012
    /// 
    /// @author Anton Beloglazov
    /// @since Jan 6, 2012
    /// </summary>
    public class Constants
    {
        public const bool ENABLE_OUTPUT = true;
        public const bool OUTPUT_CSV = false;

        public const double SCHEDULING_INTERVAL = 300;
        public const double SIMULATION_LIMIT = 24 * 60 * 60;

        public static readonly int CLOUDLET_LENGTH = 2500 * (int)SIMULATION_LIMIT;
        public const int CLOUDLET_PES = 1;

        /*
		 * VM instance types:
		 *   High-Memory Extra Large Instance: 3.25 EC2 Compute Units, 8.55 GB // too much MIPS
		 *   High-CPU Medium Instance: 2.5 EC2 Compute Units, 0.85 GB
		 *   Extra Large Instance: 2 EC2 Compute Units, 3.75 GB
		 *   Small Instance: 1 EC2 Compute Unit, 1.7 GB
		 *   Micro Instance: 0.5 EC2 Compute Unit, 0.633 GB
		 *   We decrease the memory size two times to enable oversubscription
		 *
		 */
        public const int VM_TYPES = 4;
        public static readonly int[] VM_MIPS = new int[] { 2500, 2000, 1000, 500 };
        public static readonly int[] VM_PES = new int[] { 1, 1, 1, 1 };
        public static readonly int[] VM_RAM = new int[] { 870, 1740, 1740, 613 };
        public const int VM_BW = 100000; // 100 Mbit/s
        public const int VM_SIZE = 2500; // 2.5 GB

        /*
		 * Host types:
		 *   HP ProLiant ML110 G4 (1 x [Xeon 3040 1860 MHz, 2 cores], 4GB)
		 *   HP ProLiant ML110 G5 (1 x [Xeon 3075 2660 MHz, 2 cores], 4GB)
		 *   We increase the memory size to enable over-subscription (x4)
		 */
        public const int HOST_TYPES = 2;
        public static readonly int[] HOST_MIPS = new int[] { 1860, 2660 };
        public static readonly int[] HOST_PES = new int[] { 2, 2 };
        public static readonly int[] HOST_RAM = new int[] { 4096, 4096 };
        public const int HOST_BW = 1000000; // 1 Gbit/s
        public const int HOST_STORAGE = 1000000; // 1 GB

        public static readonly PowerModel[] HOST_POWER = new PowerModel[]
        {
            new PowerModelSpecPowerHpProLiantMl110G4Xeon3040(),
            new PowerModelSpecPowerHpProLiantMl110G5Xeon3075()
        };
    }
}