using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocators
{

	using Container = org.cloudbus.cloudsim.container.core.Container;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;


	/// <summary>
	/// Created by sareh on 16/07/15.
	/// </summary>
	public abstract class PowerContainerAllocationPolicy : ContainerAllocationPolicy
	{

			/// <summary>
			/// The container table. </summary>
			private readonly IDictionary<string, ContainerVm> containerTable = new Dictionary<string, ContainerVm>();

			/// <summary>
			/// Instantiates a new power vm allocation policy abstract.
			/// 
			/// </summary>
			public PowerContainerAllocationPolicy() : base()
			{
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#allocateHostForVm(org.cloudbus.cloudsim.Vm)
			 */
			public override bool allocateVmForContainer(Container container, IList<ContainerVm> containerVmList)
			{
				ContainerVmList = containerVmList;
				return allocateVmForContainer(container, findVmForContainer(container));
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#allocateHostForVm(org.cloudbus.cloudsim.Vm,
			 * org.cloudbus.cloudsim.Host)
			 */
			public override bool allocateVmForContainer(Container container, ContainerVm containerVm)
			{
				if (containerVm == null)
				{
					Log.formatLine("%.2f: No suitable VM found for Container#" + container.Id + "\n", CloudSim.clock());
					return false;
				}
				if (containerVm.containerCreate(container))
				{ // if vm has been succesfully created in the host
					ContainerTable[container.Uid] = containerVm;
	//                container.setVm(containerVm);
					Log.formatLine("%.2f: Container #" + container.Id + " has been allocated to the VM #" + containerVm.Id, CloudSim.clock());
					return true;
				}
				Log.formatLine("%.2f: Creation of Container #" + container.Id + " on the Vm #" + containerVm.Id + " failed\n", CloudSim.clock());
				return false;
			}

			/// <summary>
			/// Find host for vm.
			/// </summary>
			/// <param name="container"> the vm </param>
			/// <returns> the power host </returns>
			public virtual ContainerVm findVmForContainer(Container container)
			{
				foreach (ContainerVm containerVm in ContainerVmList)
				{
	//                Log.printConcatLine("Trying vm #",containerVm.getId(),"For container #", container.getId());
					if (containerVm.isSuitableForContainer(container))
					{
						return containerVm;
					}
				}
				return null;
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#deallocateHostForVm(org.cloudbus.cloudsim.Vm)
			 */
			public override void deallocateVmForContainer(Container container)
			{
				ContainerVm containerVm = ContainerTable[container.Uid];
                ContainerTable.Remove(container.Uid);
                if (containerVm != null)
				{
					containerVm.containerDestroy(container);
				}
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#getHost(org.cloudbus.cloudsim.Vm)
			 */
			public override ContainerVm getContainerVm(Container container)
			{
				return ContainerTable[container.Uid];
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#getHost(int, int)
			 */
			public override ContainerVm getContainerVm(int containerId, int userId)
			{
				return ContainerTable[Container.getUid(userId, containerId)];
			}

			/// <summary>
			/// Gets the vm table.
			/// </summary>
			/// <returns> the vm table </returns>
			public virtual IDictionary<string, ContainerVm> ContainerTable
			{
				get
				{
					return containerTable;
				}
			}

	}




}