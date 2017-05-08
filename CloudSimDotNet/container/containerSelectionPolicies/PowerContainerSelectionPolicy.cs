using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerSelectionPolicies
{


	using Container = org.cloudbus.cloudsim.container.core.Container;
	using PowerContainer = org.cloudbus.cloudsim.container.core.PowerContainer;
	using PowerContainerHost = org.cloudbus.cloudsim.container.core.PowerContainerHost;
	using PowerContainerVm = org.cloudbus.cloudsim.container.core.PowerContainerVm;


	/// <summary>
	/// Created by sareh on 31/07/15.
	/// </summary>
	public abstract class PowerContainerSelectionPolicy
	{

		/// <summary>
		/// Gets the containers to migrate.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the container to migrate </returns>
		public abstract Container getContainerToMigrate(PowerContainerHost host);

		/// <summary>
		/// Gets the migratable containers.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the migratable containers </returns>
		protected internal virtual IList<PowerContainer> getMigratableContainers(PowerContainerHost host)
		{
			IList<PowerContainer> migratableContainers = new List<PowerContainer>();
			foreach (PowerContainerVm vm in host.VmListProperty)
			{
				if (!vm.InMigration)
				{
					foreach (Container container in vm.ContainerListProperty)
					{

						if (!container.InMigration && !vm.ContainersMigratingIn.Contains(container))
						{
							migratableContainers.Add((PowerContainer) container);
						}

					}



				}
			}
			return migratableContainers;
		}

	}

}