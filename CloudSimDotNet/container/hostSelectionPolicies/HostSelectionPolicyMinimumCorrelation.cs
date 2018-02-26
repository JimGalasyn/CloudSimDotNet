using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.hostSelectionPolicies
{

	using org.cloudbus.cloudsim.container.core;
	using Correlation = org.cloudbus.cloudsim.container.utils.Correlation;


	/// <summary>
	/// Created by sareh on 11/08/15.
	/// </summary>
	public class HostSelectionPolicyMinimumCorrelation : HostSelectionPolicy
	{

		private HostSelectionPolicy fallbackPolicy;

		/// <summary>
		/// Instantiates a new power vm selection policy maximum correlation.
		/// </summary>
		/// <param name="fallbackPolicy"> the fallback policy </param>
		public HostSelectionPolicyMinimumCorrelation(HostSelectionPolicy fallbackPolicy) : base()
		{
			FallbackPolicy = fallbackPolicy;
		}

		public override ContainerHost getHost(IList<ContainerHost> hostList, object obj, ISet<ContainerHost> excludedHostList)
		{

			double[] utilizationHistory;
			if (obj is Container)
			{

				utilizationHistory = ((PowerContainer) obj).UtilizationHistoryList;
			}
			else
			{

				utilizationHistory = ((PowerContainerVm) obj).UtilizationHistoryList;
			}
			Correlation correlation = new Correlation();
			double minCor = double.MaxValue;
			ContainerHost selectedHost = null;
			foreach (ContainerHost host in hostList)
			{
				if (excludedHostList.Contains(host))
				{
					continue;
				}
				if (host is PowerContainerHostUtilizationHistory)
				{
					double[] hostUtilization = ((PowerContainerHostUtilizationHistory) host).UtilizationHistory;
					if (hostUtilization.Length > 5)
					{

						double cor = correlation.getCor(hostUtilization, utilizationHistory);
						if (cor < minCor)
						{
							minCor = cor;
							selectedHost = host;

						}
					}

				}
			}
			if (selectedHost == null)
			{

			}
			return selectedHost;
		}


		public virtual HostSelectionPolicy FallbackPolicy
		{
			get
			{
				return fallbackPolicy;
			}
			set
			{
				this.fallbackPolicy = value;
			}
		}



	}

}