using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocators
{

	using ContainerDatacenter = org.cloudbus.cloudsim.container.core.ContainerDatacenter;
	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;


	/// <summary>
	/// Created by sareh on 14/07/15.
	/// </summary>
	public class PowerContainerVmAllocationSimple : PowerContainerVmAllocationAbstract
	{

		public PowerContainerVmAllocationSimple(IList<ContainerHost> list) : base(list)
		{
		}

		public override IList<IDictionary<string, object>> optimizeAllocation(IList<ContainerVm> vmList) 
		{
			return null;
		}

		public override ContainerDatacenter Datacenter { get; set; }
		//{
		//	set
		//	{
    
		//	}
		//}
	}

}