using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.hostSelectionPolicies
{

	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;
	using PowerContainerHostUtilizationHistory = org.cloudbus.cloudsim.container.core.PowerContainerHostUtilizationHistory;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;


	/// <summary>
	/// Created by sareh on 11/08/15.
	/// </summary>
	public class HostSelectionPolicyMostFull : HostSelectionPolicy
	{

		public override ContainerHost getHost(IList<ContainerHost> hostList, object obj, ISet<ContainerHost> excludedHostList)
		{
			ContainerHost selectedHost = null;
			if (CloudSim.clock() > 1.0)
			{
			double maxUsage = double.Epsilon;
			foreach (ContainerHost host in hostList)
			{
				if (excludedHostList.Contains(host))
				{
					continue;
				}

				if (host is PowerContainerHostUtilizationHistory)
				{
					double hostUtilization = ((PowerContainerHostUtilizationHistory) host).UtilizationOfCpu;
					if (hostUtilization > maxUsage)
					{
						maxUsage = hostUtilization;
						selectedHost = host;

					}


				}
			}

			return selectedHost;
			}
		else
		{

	//            At the simulation start all the VMs by leastFull algorithms.

				selectedHost = (new HostSelectionPolicyFirstFit()).getHost(hostList,obj,excludedHostList);

				return selectedHost;
		}



		}


	}

}