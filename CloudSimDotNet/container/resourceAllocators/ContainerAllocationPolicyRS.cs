using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocators
{

	using ContainerPlacementPolicy = org.cloudbus.cloudsim.container.containerPlacementPolicies.ContainerPlacementPolicy;
	using Container = org.cloudbus.cloudsim.container.core.Container;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;

	/// <summary>
	/// Created by sareh on 16/12/15.
	/// </summary>
	public class ContainerAllocationPolicyRS : PowerContainerAllocationPolicySimple
	{
		/// <summary>
		/// The vm table. </summary>


		private ContainerPlacementPolicy containerPlacementPolicy;


		public ContainerAllocationPolicyRS(ContainerPlacementPolicy containerPlacementPolicy1) : base()
		{
			ContainerPlacementPolicy = containerPlacementPolicy1;
		}


		public override ContainerVm findVmForContainer(Container container)
		{

			ISet<ContainerVm> excludedVmList = new HashSet<ContainerVm>();
			int tries = 0;
			bool found = false;
			do
			{

				ContainerVm containerVm = ContainerPlacementPolicy.getContainerVm(ContainerVmList, container,excludedVmList);
				if (containerVm == null)
				{

					return null;
				}
				if (containerVm.isSuitableForContainer(container))
				{
					found = true;
					return containerVm;
				}
				else
				{
						excludedVmList.Add(containerVm);
						tries++;
				}

			} while (!found & tries < ContainerVmList.Count);

			return null;
		}



		public virtual ContainerPlacementPolicy ContainerPlacementPolicy
		{
			get
			{
				return this.containerPlacementPolicy;
			}
			set
			{
				this.containerPlacementPolicy = value;
			}
		}



	}

}