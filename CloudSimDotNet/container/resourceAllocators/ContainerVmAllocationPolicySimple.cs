using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocators
{

	using ContainerDatacenter = org.cloudbus.cloudsim.container.core.ContainerDatacenter;
	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;


	/// <summary>
	/// Created by sareh on 14/07/15.
	/// </summary>
	public class ContainerVmAllocationPolicySimple : ContainerVmAllocationPolicy
	{

		/// <summary>
		/// The vm table.
		/// </summary>
		private IDictionary<string, ContainerHost> vmTable;

		/// <summary>
		/// The used pes.
		/// </summary>
		private IDictionary<string, int?> usedPes;

		/// <summary>
		/// The free pes.
		/// </summary>
		private IList<int?> freePes;

		/// <summary>
		/// Creates the new VmAllocationPolicySimple object.
		/// </summary>
		/// <param name="list"> the list
		/// @pre $none
		/// @post $none </param>
		public ContainerVmAllocationPolicySimple(IList<ContainerHost> list) : base(list)
		{

			FreePes = new List<int?>();
			foreach (ContainerHost host in ContainerHostListProperty)
			{
				FreePes.Add(host.NumberOfPes);

			}

			VmTable = new Dictionary<string, ContainerHost>();
			UsedPes = new Dictionary<string, int?>();
		}

		public override bool allocateHostForVm(ContainerVm containerVm)
		{
			int requiredPes = containerVm.NumberOfPes;
			bool result = false;
			int tries = 0;
			IList<int?> freePesTmp = new List<int?>();
			foreach (int? freePes in FreePes)
			{
				freePesTmp.Add(freePes);
			}

			if (!VmTable.ContainsKey(containerVm.Uid))
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

					ContainerHost host = ContainerHostListProperty[idx];
					result = host.containerVmCreate(containerVm);

					if (result)
					{ // if vm were succesfully created in the host
						VmTable[containerVm.Uid] = host;
						UsedPes[containerVm.Uid] = requiredPes;
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

		public override bool allocateHostForVm(ContainerVm containerVm, ContainerHost host)
		{
			if (host.containerVmCreate(containerVm))
			{ // if vm has been succesfully created in the host
				VmTable[containerVm.Uid] = host;

				int requiredPes = containerVm.NumberOfPes;
				int idx = ContainerHostListProperty.IndexOf(host);
				UsedPes[containerVm.Uid] = requiredPes;
				FreePes[idx] = FreePes[idx] - requiredPes;

				Log.formatLine("%.2f: VM #" + containerVm.Id + " has been allocated to the host #" + host.Id, CloudSim.clock());
				return true;
			}

			return false;
		}


		public override IList<IDictionary<string, object>> optimizeAllocation(IList<ContainerVm> vmList)
		{
			return null;
		}

		public override void deallocateHostForVm(ContainerVm containerVm)
		{
			ContainerHost host = VmTable[containerVm.Uid];
            VmTable.Remove(containerVm.Uid);
            int idx = ContainerHostListProperty.IndexOf(host);
			int pes = UsedPes[containerVm.Uid].Value;
            UsedPes.Remove(containerVm.Uid);
            if (host != null)
			{
				host.containerVmDestroy(containerVm);
				FreePes[idx] = FreePes[idx] + pes;
			}
		}

		public override ContainerHost getHost(ContainerVm containerVm)
		{
			return VmTable[containerVm.Uid];
		}

		public override ContainerHost getHost(int vmId, int userId)
		{
			return VmTable[ContainerVm.getUid(userId, vmId)];
		}

		public override ContainerDatacenter Datacenter { get; set; }
		//{
		//	set
		//	{
    
		//	}
		//}


		public virtual IDictionary<string, ContainerHost> VmTable
		{
			get
			{
				return vmTable;
			}
			set
			{
				this.vmTable = value;
			}
		}


		public virtual IDictionary<string, int?> UsedPes
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


		public virtual IList<int?> FreePes
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