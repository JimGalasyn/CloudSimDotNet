using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.vmSelectionPolicies
{

	using org.cloudbus.cloudsim.container.core;

	/// <summary>
	/// Created by sareh on 30/07/15.
	/// </summary>
	public class PowerContainerVmSelectionPolicyMinimumMigrationTime : PowerContainerVmSelectionPolicy
	{



		public override ContainerVm getVmToMigrate(PowerContainerHost host)
		{
			IList<PowerContainerVm> migratableVms = getMigratableVms(host);
			if (migratableVms.Count == 0)
			{
				return null;
			}
			ContainerVm vmToMigrate = null;
			double minMetric = double.MaxValue;
			foreach (ContainerVm vm in migratableVms)
			{
				if (vm.InMigration)
				{
					continue;
				}
				double metric = vm.Ram;
				if (metric < minMetric)
				{
					minMetric = metric;
					vmToMigrate = vm;
				}
			}
			return vmToMigrate;
		}



	}

}