using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.hostSelectionPolicies
{

	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;
	using PowerContainerHostUtilizationHistory = org.cloudbus.cloudsim.container.core.PowerContainerHostUtilizationHistory;


	/// <summary>
	/// Created by sareh on 11/08/15.
	/// </summary>
	public class HostSelectionPolicyLeastFull : HostSelectionPolicy
	{

        //public override ContainerHost getHost<T1>(IList<ContainerHost> hostList, object obj, ISet<T1> excludedHostList) where T1 : org.cloudbus.cloudsim.container.core.ContainerHost
        public override ContainerHost getHost(IList<ContainerHost> hostList, object obj, ISet<ContainerHost> excludedHostList)
        {
			double minUsage = double.MaxValue;
			ContainerHost selectedHost = null;
			foreach (ContainerHost host in hostList)
			{
				if (excludedHostList.Contains(host))
				{
					continue;
				}
				if (host is PowerContainerHostUtilizationHistory)
				{
					double hostUtilization = ((PowerContainerHostUtilizationHistory) host).UtilizationOfCpu;
					if (hostUtilization < minUsage)
					{
						minUsage = hostUtilization;
						selectedHost = host;

					}


				}
			}

			return selectedHost;
		}
	}

}