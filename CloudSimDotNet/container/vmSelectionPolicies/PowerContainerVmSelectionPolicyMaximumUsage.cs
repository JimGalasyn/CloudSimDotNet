using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.vmSelectionPolicies
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
	using PowerContainerHost = org.cloudbus.cloudsim.container.core.PowerContainerHost;
	using PowerContainerVm = org.cloudbus.cloudsim.container.core.PowerContainerVm;

	/// <summary>
	/// Created by sareh on 16/11/15.
	/// </summary>
	public class PowerContainerVmSelectionPolicyMaximumUsage : PowerContainerVmSelectionPolicy
	{
		/*
		 * (non-Javadoc)
		 * @see
		 * org.cloudbus.cloudsim.experiments.power.PowerVmSelectionPolicy#getVmsToMigrate(org.cloudbus
		 * .cloudsim.power.PowerHost)
		 */
		public override ContainerVm getVmToMigrate(PowerContainerHost host)
		{
			IList<PowerContainerVm> migratableContainers = getMigratableVms(host);
			if (migratableContainers.Count == 0)
			{
				return null;
			}
			ContainerVm VmsToMigrate = null;
			double maxMetric = double.Epsilon;
			foreach (ContainerVm vm in migratableContainers)
			{
				if (vm.InMigration)
				{
					continue;
				}
				double metric = vm.CurrentRequestedTotalMips;
				if (maxMetric < metric)
				{
					maxMetric = metric;
					VmsToMigrate = vm;
				}
			}
	//        Log.formatLine("The Container To migrate is #%d from VmID %d from host %d", containerToMigrate.getId(),containerToMigrate.getVm().getId(), host.getId());
			return VmsToMigrate;
		}


	}

}