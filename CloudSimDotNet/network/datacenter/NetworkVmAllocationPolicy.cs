using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.network.datacenter
{
    using System;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

    /// <summary>
    /// NetworkVmAllocationPolicy is an <seealso cref="VmAllocationPolicy"/> that chooses, 
    /// as the host for a VM, the host with less PEs in use.
    /// 
    /// @author Rodrigo N. Calheiros
    /// @author Anton Beloglazov
    /// @author Saurabh Kumar Garg
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class NetworkVmAllocationPolicy : VmAllocationPolicy
	{

		/// <summary>
		/// The vm map where each key is a VM id and
		/// each value is the host where the VM is placed. 
		/// </summary>
		private IDictionary<string, Host> vmTable;

		/// <summary>
		/// The used PEs map, where each key is a VM id
		/// and each value is the number of required PEs the VM is using. 
		/// </summary>
		private IDictionary<string, int?> usedPes;

		/// <summary>
		/// The free pes. </summary>
		private IList<int?> freePes;

		/// <summary>
		/// Creates a new VmAllocationPolicySimple object.
		/// </summary>
		/// <param name="list"> list Machines available in a <seealso cref="Datacenter"/>
		/// 
		/// @pre $none
		/// @post $none </param>
		public NetworkVmAllocationPolicy(IList<Host> list) : base(list)
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
		/// <param name="vm"> {@inheritDoc}
		/// </param>
		/// <returns> {@inheritDoc}
		/// 
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

                    // TEST: (fixed) Make sure this cast works.
                    NetworkHost host = this.HostListProperty[idx] as NetworkHost;
                    if( host == null )
                    {
                        throw new InvalidCastException("Can't cast this.HostListProperty[idx] as NetworkHost");
                    }
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

			/// <summary>
			/// Gets the max utilization among the PEs of a given VM placed at a given host. </summary>
			/// <param name="host"> The host where the VM is placed </param>
			/// <param name="vm"> The VM to get the max PEs utilization </param>
			/// <returns> The max utilization among the PEs of the VM </returns>
		protected internal virtual double getMaxUtilizationAfterAllocation(NetworkHost host, Vm vm)
		{
			IList<double?> allocatedMipsForVm = null;
			NetworkHost allocatedHost = (NetworkHost) vm.Host;

			if (allocatedHost != null)
			{
				allocatedMipsForVm = vm.Host.getAllocatedMipsForVm(vm);
			}

			if (!host.allocatePesForVm(vm, vm.CurrentRequestedMips))
			{
				return -1;
			}

			double maxUtilization = host.getMaxUtilizationAmongVmsPes(vm);

			host.deallocatePesForVm(vm);

			if (allocatedHost != null && allocatedMipsForVm != null)
			{
				vm.Host.allocatePesForVm(vm, allocatedMipsForVm);
			}

			return maxUtilization;
		}

		public override void deallocateHostForVm(Vm vm)
		{
			Host host = VmTable[vm.Uid];
            VmTable.Remove(vm.Uid);
            int idx = HostListProperty.IndexOf(host);
			int pes = UsedPes[vm.Uid].Value;
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
			/*@todo Auto-generated method stub.
					The method is doing nothing.*/
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