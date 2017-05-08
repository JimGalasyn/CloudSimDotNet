using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerPlacementPolicies
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;


	/// <summary>
	/// Created by sareh fotuhi Piraghaj on 16/12/15.
	/// For container placement Least-Full policy.
	/// </summary>
	public class ContainerPlacementPolicyLeastFull : ContainerPlacementPolicy
	{

		public override ContainerVm getContainerVm(IList<ContainerVm> vmList, object obj, ISet<ContainerVm> excludedVmList)
		{
            ContainerVm selectedVm = default(ContainerVm);
            double minMips = double.MaxValue;
			foreach (ContainerVm containerVm1 in vmList)
			{
				if (excludedVmList.Contains(containerVm1))
				{
					continue;
				}
				double containerUsage = containerVm1.ContainerScheduler.AvailableMips;
				if (containerUsage < minMips)
				{
					minMips = containerUsage;
					selectedVm = containerVm1;

				}
			}
			return selectedVm;
		}
	}

}