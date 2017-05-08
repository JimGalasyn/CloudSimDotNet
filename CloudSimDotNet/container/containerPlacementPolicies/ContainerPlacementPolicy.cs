using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerPlacementPolicies
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;


	/// <summary>
	///  Created by sareh fotuhi Piraghaj on 16/12/15.
	///  For writing any container placement policies this class should be extend.
	/// </summary>

	public abstract class ContainerPlacementPolicy
	{
        /// <summary>
        /// Gets the VM List, and the excluded VMs
        /// </summary>
        /// <param name="vmList"> the host </param>
        /// <returns> the destination vm to place container </returns>
        //public abstract ContainerVm getContainerVm<T1>(IList<ContainerVm> vmList, object obj, ISet<T1> excludedVmList) where T1 : org.cloudbus.cloudsim.container.core.ContainerVm;
        public abstract ContainerVm getContainerVm(IList<ContainerVm> vmList, object obj, ISet<ContainerVm> excludedVmList);

    }

}