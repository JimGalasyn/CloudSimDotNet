using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.hostSelectionPolicies
{

    using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;
    using RandomGen = org.cloudbus.cloudsim.container.utils.RandomGen;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using System.Diagnostics;


    /// <summary>
    /// Created by sareh on 12/08/15.
    /// </summary>
    public class HostSelectionPolicyRandomSelection : HostSelectionPolicy
	{

		public override ContainerHost getHost(IList<ContainerHost> hostList, object obj, ISet<ContainerHost> excludedHostList) 
		{
			ContainerHost host = null;

			if (CloudSim.clock() > 1.0)
			{
				while (true)
				{
					if (hostList.Count > 0)
					{
						int randomNum = (new RandomGen()).getNum(hostList.Count);
	//                System.out.format("The Selection Algorithm has chosen: %d from %d%n",  randomNum, hostList.size());

						host = hostList[randomNum];
						if (excludedHostList.Contains(host))
						{
							continue;
						}
					}
					else
					{

						Debug.WriteLine("Error");
					}

					return host;
				}
			}
			else
			{

	//            At the simulation start all the VMs by leastFull algorithms.

				host = (new HostSelectionPolicyFirstFit()).getHost(hostList,obj,excludedHostList);

				return host;
			}
		}
	}
}