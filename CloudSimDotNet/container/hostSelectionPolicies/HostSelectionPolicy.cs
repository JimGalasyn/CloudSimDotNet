using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.hostSelectionPolicies
{

	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;


	/// <summary>
	/// Created by sareh on 11/08/15.
	/// </summary>
	public abstract class HostSelectionPolicy
	{

        /// <summary>
        /// Gets the host
        /// </summary>
        /// <param name="hostList"> the host </param>
        /// <returns> the destination host to migrate </returns>
        //public abstract ContainerHost getHost<T1>(IList<ContainerHost> hostList, object obj, ISet<T1> excludedHostList) where T1 : org.cloudbus.cloudsim.container.core.ContainerHost;
        public abstract ContainerHost getHost(IList<ContainerHost> hostList, object obj, ISet<ContainerHost> excludedHostList);

    }

}