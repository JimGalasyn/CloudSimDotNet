using System.Collections.Generic;

/// 
namespace org.cloudbus.cloudsim.container.resourceAllocators
{


	using Container = org.cloudbus.cloudsim.container.core.Container;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;


	/// <summary>
	/// @author sareh
	/// 
	/// </summary>
	public class ContainerAllocationPolicySimple : ContainerAllocationPolicy
	{
		/// <summary>
		/// The vm table. </summary>
		private IDictionary<string, ContainerVm> containerVmTable;

		/// <summary>
		/// The used pes. </summary>
		private IDictionary<string, int?> usedPes;

		/// <summary>
		/// The free pes. </summary>
		private IList<int?> freePes;
		/// <summary>
		/// Creates the new VmAllocationPolicySimple object.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public ContainerAllocationPolicySimple() : base()
		{
			FreePes = new List<int?>();
			ContainerVmTable = new Dictionary<string, ContainerVm>();
			UsedPes = new Dictionary<string, int?>();
		}


		public override bool allocateVmForContainer(Container container, IList<ContainerVm> containerVmList)
		{
	//		the available container list is updated. It gets is from the data center.
			ContainerVmList = containerVmList;
			foreach (ContainerVm containerVm in ContainerVmList)
			{
				FreePes.Add(containerVm.NumberOfPes);

			}
			int requiredPes = container.NumberOfPes;
			bool result = false;
			int tries = 0;
			IList<int?> freePesTmp = new List<int?>();
			foreach (int? freePes in FreePes)
			{
				freePesTmp.Add(freePes);
			}

			if (!ContainerVmTable.ContainsKey(container.Uid))
			{ // if this vm was not created
				do
				{ // we still trying until we find a host or until we try all of them
					int moreFree = int.MinValue;
					int idx = -1;

					// we want the host with less pes in use
					for (int i = 0; i < freePesTmp.Count; i++)
					{
						if (freePesTmp[i] > moreFree)
						{
							moreFree = freePesTmp[i].Value;
							idx = i;
						}
					}

					ContainerVm containerVm = ContainerVmList[idx];
					result = containerVm.containerCreate(container);

					if (result)
					{ // if vm were succesfully created in the host
						ContainerVmTable[container.Uid] = containerVm;
						UsedPes[container.Uid] = requiredPes;
						FreePes[idx] = FreePes[idx] - requiredPes;
						result = true;
						break;
					}
					else
					{
						freePesTmp[idx] = int.MinValue;
					}
					tries++;
				} while (!result && tries < FreePes.Count);

			}

			freePesTmp.Clear();

			return result;
		}

		public override bool allocateVmForContainer(Container container, ContainerVm containerVm)
		{
			if (containerVm.containerCreate(container))
			{ // if vm has been succesfully created in the host
				ContainerVmTable[container.Uid] = containerVm;

				int requiredPes = container.NumberOfPes;
                // TEST: (fixed) Type issue -- containerVm was intended?
                int idx = ContainerVmList.IndexOf(containerVm);
				UsedPes[container.Uid] = requiredPes;
				FreePes[idx] = FreePes[idx] - requiredPes;

				Log.formatLine("%.2f: Container #" + container.Id + " has been allocated to the Vm #" + containerVm.Id, CloudSim.clock());
				return true;
			}

			return false;
		}


		public override IList<IDictionary<string, object>> optimizeAllocation(IList<Container> containerList)
		{
			return null;
		}

		public override void deallocateVmForContainer(Container container)
		{

			ContainerVm containerVm = ContainerVmTable[container.Uid];
            ContainerVmTable.Remove(container.Uid);
            int idx = ContainerVmList.IndexOf(containerVm);
			int pes = UsedPes[container.Uid].Value;
            UsedPes.Remove(container.Uid);
            if (containerVm != null)
			{
				containerVm.containerDestroy(container);
				FreePes[idx] = FreePes[idx] + pes;
			}

		}

		public override ContainerVm getContainerVm(Container container)
		{
			return ContainerVmTable[container.Uid];
		}

		public override ContainerVm getContainerVm(int containerId, int userId)
		{
			return ContainerVmTable[Container.getUid(userId, containerId)];
		}

		protected internal virtual IDictionary<string, ContainerVm> ContainerVmTable
		{
			get
			{
				return containerVmTable;
			}
			set
			{
				this.containerVmTable = value;
			}
		}


		protected internal virtual IDictionary<string, int?> UsedPes
		{
			get
			{
				return usedPes;
			}
			set
			{
				this.usedPes = value;
			}
		}


		protected internal virtual IList<int?> FreePes
		{
			get
			{
				return freePes;
			}
			set
			{
				this.freePes = value;
			}
		}

	}

}