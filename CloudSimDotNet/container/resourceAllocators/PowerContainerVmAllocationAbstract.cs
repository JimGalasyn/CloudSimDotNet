using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocators
{


	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

	/// <summary>
	/// Created by sareh on 14/07/15.
	/// </summary>
	public abstract class PowerContainerVmAllocationAbstract : ContainerVmAllocationPolicy
	{

			/// <summary>
			/// The vm table. </summary>
			private readonly IDictionary<string, ContainerHost> vmTable = new Dictionary<string, ContainerHost>();

			/// <summary>
			/// Instantiates a new power vm allocation policy abstract.
			/// </summary>
			/// <param name="list"> the list </param>
			public PowerContainerVmAllocationAbstract(IList<ContainerHost> list) : base(list)
			{
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#allocateHostForVm(org.cloudbus.cloudsim.Vm)
			 */
			public override bool allocateHostForVm(ContainerVm containerVm)
			{
				return allocateHostForVm(containerVm, findHostForVm(containerVm));
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#allocateHostForVm(org.cloudbus.cloudsim.Vm,
			 * org.cloudbus.cloudsim.Host)
			 */
			public override bool allocateHostForVm(ContainerVm containerVm, ContainerHost host)
			{
				if (host == null)
				{
					Log.formatLine("%.2f: No suitable host found for VM #" + containerVm.Id + "\n", CloudSim.clock());
					return false;
				}
				if (host.containerVmCreate(containerVm))
				{ // if vm has been succesfully created in the host
					VmTable[containerVm.Uid] = host;
					Log.formatLine("%.2f: VM #" + containerVm.Id + " has been allocated to the host #" + host.Id, CloudSim.clock());
					return true;
				}
				Log.formatLine("%.2f: Creation of VM #" + containerVm.Id + " on the host #" + host.Id + " failed\n", CloudSim.clock());
				return false;
			}

			/// <summary>
			/// Find host for vm.
			/// </summary>
			/// <param name="containerVm"> the vm </param>
			/// <returns> the power host </returns>
			public virtual ContainerHost findHostForVm(ContainerVm containerVm)
			{
				foreach (ContainerHost host in this.ContainerHostListProperty)
				{
					if (host.isSuitableForContainerVm(containerVm))
					{
						return host;
					}
				}
				return null;
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#deallocateHostForVm(org.cloudbus.cloudsim.Vm)
			 */
			public override void deallocateHostForVm(ContainerVm containerVm)
			{
				ContainerHost host = VmTable[containerVm.Uid];
                VmTable.Remove(containerVm.Uid);
                if (host != null)
				{
					host.containerVmDestroy(containerVm);
				}
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#getHost(org.cloudbus.cloudsim.Vm)
			 */
			public override ContainerHost getHost(ContainerVm vm)
			{
				return VmTable[vm.Uid];
			}

			/*
			 * (non-Javadoc)
			 * @see org.cloudbus.cloudsim.VmAllocationPolicy#getHost(int, int)
			 */
			public override ContainerHost getHost(int vmId, int userId)
			{
				return VmTable[ContainerVm.getUid(userId, vmId)];
			}

			/// <summary>
			/// Gets the vm table.
			/// </summary>
			/// <returns> the vm table </returns>
			public virtual IDictionary<string, ContainerHost> VmTable
			{
				get
				{
					return vmTable;
				}
			}

		public virtual IList<ContainerVm> OverUtilizedVms
		{
			get
			{
				IList<ContainerVm> vmList = new List<ContainerVm>();
				foreach (ContainerHost host in ContainerHostListProperty)
				{
					foreach (ContainerVm vm in host.VmListProperty)
					{
						if (vm.getTotalUtilizationOfCpuMips(CloudSim.clock()) > vm.TotalMips)
						{
							vmList.Add(vm);
    
						}
    
					}
    
				}
				return vmList;
			}
		}


	}
}