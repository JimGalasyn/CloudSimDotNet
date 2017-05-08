using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerSelectionPolicies
{

	using PowerContainerHost = org.cloudbus.cloudsim.container.core.PowerContainerHost;
	using Container = org.cloudbus.cloudsim.container.core.Container;
	using PowerContainer = org.cloudbus.cloudsim.container.core.PowerContainer;

	/// <summary>
	/// Created by sareh on 3/08/15.
	/// </summary>
	public class PowerContainerSelectionPolicyMinimumMigrationTime : PowerContainerSelectionPolicy
	{


		/*
		 * (non-Javadoc)
		 * @see
		 * PowerContainerSelectionPolicy#getContainerToMigrate
		 */
		public override Container getContainerToMigrate(PowerContainerHost host)
		{
			IList<PowerContainer> migratableContainers = getMigratableContainers(host);
			if (migratableContainers.Count == 0)
			{
				return null;
			}
			Container containerToMigrate = null;
			double minMetric = double.MaxValue;
			foreach (Container container in migratableContainers)
			{
				if (container.InMigration)
				{
					continue;
				}
				double metric = container.Ram;
				if (metric < minMetric)
				{
					minMetric = metric;
					containerToMigrate = container;
				}
			}
			return containerToMigrate;
		}

	}

}