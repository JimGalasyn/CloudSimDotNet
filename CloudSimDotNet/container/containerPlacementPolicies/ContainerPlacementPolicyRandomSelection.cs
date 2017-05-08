using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerPlacementPolicies
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
	using RandomGen = org.cloudbus.cloudsim.container.utils.RandomGen;


	/// <summary>
	/// Created by sareh fotuhi Piraghaj on 16/12/15.
	/// For container placement Random policy.
	/// </summary>
	public class ContainerPlacementPolicyRandomSelection : ContainerPlacementPolicy
	{
        //public override ContainerVm getContainerVm<T1>(IList<ContainerVm> vmList, object obj, ISet<T1> excludedVmList) where T1 : org.cloudbus.cloudsim.container.core.ContainerVm
        public override ContainerVm getContainerVm(IList<ContainerVm> vmList, object obj, ISet<ContainerVm> excludedVmList)
        {
            ContainerVm containerVm = default(ContainerVm);
            while (true)
			{
				if (vmList.Count > 0)
				{
					int randomNum = (new RandomGen()).getNum(vmList.Count);
					containerVm = vmList[randomNum];
					if (excludedVmList.Contains(containerVm))
					{
						continue;
					}
				}
				else
				{

					Log.print(string.Format("Error: The VM list Size is: {0:D}", vmList.Count));
				}

				return containerVm;
			}
		}
	}

}