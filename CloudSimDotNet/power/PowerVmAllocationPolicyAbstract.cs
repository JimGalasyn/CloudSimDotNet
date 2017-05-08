using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power
{


	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

	/// <summary>
	/// An abstract power-aware VM allocation policy.
	/// 
	/// <br/>If you are using any algorithms, policies or workload included in the power package please cite
	/// the following paper:<br/>
	/// 
	/// <ul>
	/// <li><a href="http://dx.doi.org/10.1002/cpe.1867">Anton Beloglazov, and Rajkumar Buyya, "Optimal Online Deterministic Algorithms and Adaptive
	/// Heuristics for Energy and Performance Efficient Dynamic Consolidation of Virtual Machines in
	/// Cloud Data Centers", Concurrency and Computation: Practice and Experience (CCPE), Volume 24,
	/// Issue 13, Pages: 1397-1420, John Wiley & Sons, Ltd, New York, USA, 2012</a>
	/// </ul>
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 3.0
	/// </summary>
	public abstract class PowerVmAllocationPolicyAbstract : VmAllocationPolicy
	{

		/// <summary>
		/// The map map where each key is a VM id and
		/// each value is the host where the VM is placed. 
		/// </summary>
		private readonly IDictionary<string, Host> vmTable = new Dictionary<string, Host>();

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyAbstract.
		/// </summary>
		/// <param name="list"> the list </param>
		public PowerVmAllocationPolicyAbstract(IList<Host> list) : base(list)
		{
		}

		public override bool allocateHostForVm(Vm vm)
		{
			return allocateHostForVm(vm, findHostForVm(vm));
		}

		public override bool allocateHostForVm(Vm vm, Host host)
		{
			if (host == null)
			{
				Log.formatLine("%.2f: No suitable host found for VM #" + vm.Id + "\n", CloudSim.clock());
				return false;
			}
			if (host.vmCreate(vm))
			{ // if vm has been succesfully created in the host
				VmTable[vm.Uid] = host;
				Log.formatLine("%.2f: VM #" + vm.Id + " has been allocated to the host #" + host.Id, CloudSim.clock());
				return true;
			}
			Log.formatLine("%.2f: Creation of VM #" + vm.Id + " on the host #" + host.Id + " failed\n", CloudSim.clock());
			return false;
		}

		/// <summary>
		/// Finds the first host that has enough resources to host a given VM.
		/// </summary>
		/// <param name="vm"> the vm to find a host for it </param>
		/// <returns> the first host found that can host the VM </returns>
		public virtual PowerHost findHostForVm(Vm vm)
		{
			foreach (PowerHost host in this.HostListProperty)
			{
				if (host.isSuitableForVm(vm))
				{
					return host;
				}
			}
			return null;
		}

		public override void deallocateHostForVm(Vm vm)
		{
			Host host = VmTable[vm.Uid];
            VmTable.Remove(vm.Uid);
            if (host != null)
			{
				host.vmDestroy(vm);
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
		}

	}

}