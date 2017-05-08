namespace org.cloudbus.cloudsim.container.core
{

	public class containerCloudSimTags
	{
		/// <summary>
		/// Starting constant value for network-related tags
		/// 
		/// </summary>
		private const int ContainerSimBASE = 400;
		/// <summary>
		/// Denotes the receiving of a cloudlet  in the data center broker
		/// entity.
		/// </summary>
		public const int FIND_VM_FOR_CLOUDLET = ContainerSimBASE + 1;

		/// <summary>
		/// Denotes the creating a new VM is required in the data center.
		/// Invoked in the data center broker.
		/// </summary>
		public const int CREATE_NEW_VM = ContainerSimBASE + 2;
		/// <summary>
		/// Denotes the containers are submitted to the data center.
		/// Invoked in the data center broker.
		/// </summary>
		public const int CONTAINER_SUBMIT = ContainerSimBASE + 3;

		/// <summary>
		/// Denotes the containers are created in the data center.
		/// Invoked in the data center.
		/// </summary>
		public const int CONTAINER_CREATE_ACK = ContainerSimBASE + 4;
		/// <summary>
		/// Denotes the containers are migrated to another Vm.
		/// Invoked in the data center.
		/// </summary>
		public const int CONTAINER_MIGRATE = ContainerSimBASE + 10;
		/// <summary>
		/// Denotes a new VM is created in data center by the local scheduler
		/// Invoked in the data center.
		/// </summary>
		public const int VM_NEW_CREATE = ContainerSimBASE + 11;


		private containerCloudSimTags()
		{
            // TEST: (fixed) Auto-generated constructor stub
            /// <summary>
            /// Private Constructor </summary>
            throw new System.NotSupportedException("ContainerCloudSim Tags cannot be instantiated");

		}
	}

}