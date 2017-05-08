using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.hostSelectionPolicies
{

	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;


	/// <summary>
	/// Created by sareh on 12/08/15.
	/// </summary>
	public class HostSelectionPolicyFirstFit : HostSelectionPolicy
	{
        //public override ContainerHost getHost<T1>(IList<ContainerHost> hostList, object obj, ISet<T1> excludedHostList) where T1 : org.cloudbus.cloudsim.container.core.ContainerHost
        public override ContainerHost getHost(IList<ContainerHost> hostList, object obj, ISet<ContainerHost> excludedHostList)
        {
			ContainerHost host = null;
			foreach (ContainerHost host1 in hostList)
			{
				if (excludedHostList.Contains(host1))
				{
					continue;
				}
				host = host1;
				break;
			}
		return host;
		}
	}

}