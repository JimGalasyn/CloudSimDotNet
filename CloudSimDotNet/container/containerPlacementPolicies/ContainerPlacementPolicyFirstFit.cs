using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerPlacementPolicies
{


	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;


	/// <summary>
	/// Created by sareh fotuhi Piraghaj on 16/12/15.
	/// For container placement First Fit policy.
	/// </summary>

	public class ContainerPlacementPolicyFirstFit : ContainerPlacementPolicy
	{

        //public override ContainerVm getContainerVm<T1>(IList<ContainerVm> vmList, object obj, ISet<T1> excludedVmList) where T1 : org.cloudbus.cloudsim.container.core.ContainerVm
        public override ContainerVm getContainerVm(IList<ContainerVm> vmList, object obj, ISet<ContainerVm> excludedVmList)
        {
			ContainerVm containerVm = default(ContainerVm);
			foreach (ContainerVm containerVm1 in vmList)
			{
				if (excludedVmList.Contains(containerVm1))
				{
					continue;
				}
				containerVm = containerVm1;
				break;
			}
			return containerVm;
		}

	}

}