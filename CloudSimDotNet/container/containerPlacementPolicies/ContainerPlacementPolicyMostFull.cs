using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerPlacementPolicies
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;


	/// <summary>
	/// Created by sareh fotuhi Piraghaj on 16/12/15.
	/// For container placement Most-Full policy.
	/// </summary>
	public class ContainerPlacementPolicyMostFull : ContainerPlacementPolicy
	{

        //public override ContainerVm getContainerVm<ContainerVm>(IList<ContainerVm> vmList, object obj, ISet<ContainerVm> excludedVmList)
        public override ContainerVm getContainerVm(IList<ContainerVm> vmList, object obj, ISet<ContainerVm> excludedVmList)
        {
            ContainerVm selectedVm = default(ContainerVm);
            double maxMips = double.Epsilon;

			foreach (ContainerVm containerVm1 in vmList)
			{
				if (excludedVmList.Contains(containerVm1))
				{
					continue;
				}

				double containerUsage = containerVm1.ContainerScheduler.AvailableMips;
				if (containerUsage > maxMips)
				{
					maxMips = containerUsage;
					selectedVm = containerVm1;

				}
			}

			return selectedVm;
		}
	}

}