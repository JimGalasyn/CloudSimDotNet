using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.vmSelectionPolicies
{


	using org.cloudbus.cloudsim.container.core;


	/// <summary>
	/// Created by sareh on 28/07/15.
	/// </summary>
	public abstract class PowerContainerVmSelectionPolicy
	{

			/// <summary>
			/// Gets the vms to migrate.
			/// </summary>
			/// <param name="host"> the host </param>
			/// <returns> the vms to migrate </returns>
			public abstract ContainerVm getVmToMigrate(PowerContainerHost host);

			/// <summary>
			/// Gets the migratable vms.
			/// </summary>
			/// <param name="host"> the host </param>
			/// <returns> the migratable vms </returns>
			protected internal virtual IList<PowerContainerVm> getMigratableVms(PowerContainerHost host)
			{
				IList<PowerContainerVm> migratableVms = new List<PowerContainerVm>();
				foreach (PowerContainerVm vm in host.VmListProperty)
				{
					if (!vm.InMigration)
					{
						migratableVms.Add(vm);
					}
				}
				return migratableVms;
			}

	}


}