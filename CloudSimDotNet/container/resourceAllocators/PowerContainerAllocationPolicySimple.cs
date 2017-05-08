using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocators
{

	using Container = org.cloudbus.cloudsim.container.core.Container;


	/// <summary>
	/// Created by sareh on 16/07/15.
	/// </summary>
	public class PowerContainerAllocationPolicySimple : PowerContainerAllocationPolicy
	{


		public PowerContainerAllocationPolicySimple() : base()
		{
		}

		public override IList<IDictionary<string, object>> optimizeAllocation(IList<Container> containerList) 
		{
			return null;
		}
	}

}