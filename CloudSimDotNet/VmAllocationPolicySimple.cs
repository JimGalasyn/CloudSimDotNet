using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

	/// <summary>
	/// VmAllocationPolicySimple is an VmAllocationPolicy that chooses, as the host for a VM, the host
	/// with less PEs in use. It is therefore a Worst Fit policy, allocating VMs into the 
	/// host with most available PE.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class VmAllocationPolicySimple : VmAllocationPolicy
	{

		/// <summary>
		/// The map between each VM and its allocated host.
		/// The map key is a VM UID and the value is the allocated host for that VM. 
		/// </summary>
		private IDictionary<string, Host> vmTable;

		/// <summary>
		/// The map between each VM and the number of Pes used. 
		/// The map key is a VM UID and the value is the number of used Pes for that VM. 
		/// </summary>
		private IDictionary<string, int?> usedPes;

		/// <summary>
		/// The number of free Pes for each host from <seealso cref="#getHostList() "/>. </summary>
		private IList<int?> freePes;

		/// <summary>
		/// Creates a new VmAllocationPolicySimple object.
		/// </summary>
		/// <param name="list"> the list of hosts
		/// @pre $none
		/// @post $none </param>
		public VmAllocationPolicySimple(IList<Host> list)  : base(list)
		{

			FreePes = new List<int?>();
			foreach (Host host in HostListProperty)
			{
				FreePes.Add(host.NumberOfPes);

			}

			VmTable = new Dictionary<string, Host>();
			UsedPes = new Dictionary<string, int?>();
		}

		/// <summary>
		/// Allocates the host with less PEs in use for a given VM.
		/// </summary>
		/// <param name="vm"> {@inheritDoc} </param>
		/// <returns> {@inheritDoc}
		/// @pre $none
		/// @post $none </returns>
		public override bool allocateHostForVm(Vm vm)
		{
			int requiredPes = vm.NumberOfPes;
			bool result = false;
			int tries = 0;
			IList<int?> freePesTmp = new List<int?>();
			foreach (int? freePes in FreePes)
			{
				freePesTmp.Add(freePes);
			}

			if (!VmTable.ContainsKey(vm.Uid))
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

					Host host = HostListProperty[idx];
					result = host.vmCreate(vm);

					if (result)
					{ // if vm were succesfully created in the host
						VmTable[vm.Uid] = host;
						UsedPes[vm.Uid] = requiredPes;
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

			return result;
		}

		public override void deallocateHostForVm(Vm vm)
		{
            //Host host = VmTable.Remove(vm.Uid);
            Host host = VmTable[vm.Uid];
            VmTable.Remove(vm.Uid);
            int idx = HostListProperty.IndexOf(host);
            //int pes = UsedPes.Remove(vm.Uid);
            int? pes = UsedPes[vm.Uid];
            UsedPes.Remove(vm.Uid);
            if (host != null)
			{
				host.vmDestroy(vm);
				FreePes[idx] = FreePes[idx] + pes;
			}
		}

		public override Host getHost(Vm vm)
		{
			return VmTable[vm.Uid];
		}

		public override Host getHost(int vmId, int userId)
		{
			return VmTable[Vm.getUid(userId, vmId)];
		}

		/// <summary>
		/// Gets the vm table.
		/// </summary>
		/// <returns> the vm table </returns>
		public virtual IDictionary<string, Host> VmTable
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


		/// <summary>
		/// Gets the used pes.
		/// </summary>
		/// <returns> the used pes </returns>
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


		/// <summary>
		/// Gets the free pes.
		/// </summary>
		/// <returns> the free pes </returns>
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


		public override IList<IDictionary<string, object>> optimizeAllocation(IList<Vm> vmList)
		{
			// TEST: (fixed) Auto-generated method stub
			return null;
		}

		public override bool allocateHostForVm(Vm vm, Host host)
		{
			if (host.vmCreate(vm))
			{ // if vm has been succesfully created in the host
				VmTable[vm.Uid] = host;

				int requiredPes = vm.NumberOfPes;
				int idx = HostListProperty.IndexOf(host);
				UsedPes[vm.Uid] = requiredPes;
				FreePes[idx] = FreePes[idx] - requiredPes;

				Log.formatLine("%.2f: VM #" + vm.Id + " has been allocated to the host #" + host.Id, CloudSim.clock());
				return true;
			}

			return false;
		}
	}

}