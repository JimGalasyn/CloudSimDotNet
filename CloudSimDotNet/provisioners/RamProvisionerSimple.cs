using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.provisioners
{


	/// <summary>
	/// RamProvisionerSimple is an extension of <seealso cref="RamProvisioner"/> which uses a best-effort policy to
	/// allocate memory to VMs: if there is available ram on the host, it allocates; otherwise, it fails. 
	/// Each host has to have its own instance of a RamProvisioner.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class RamProvisionerSimple : RamProvisioner
	{

		/// <summary>
		/// The RAM map, where each key is a VM id and each value
		/// is the amount of RAM allocated to that VM. 
		/// </summary>
		private IDictionary<string, int?> ramTable;

		/// <summary>
		/// Instantiates a new ram provisioner simple.
		/// </summary>
		/// <param name="availableRam"> The total ram capacity from the host that the provisioner can allocate to VMs.  </param>
		public RamProvisionerSimple(int availableRam) : base(availableRam)
		{
			RamTable = new Dictionary<string, int?>();
		}

		public override bool allocateRamForVm(Vm vm, int ram)
		{
			int maxRam = vm.Ram;
					/* If the requested amount of RAM to be allocated to the VM is greater than
					the amount of VM is in fact requiring, allocate only the
					amount defined in the Vm requirements.*/
			if (ram >= maxRam)
			{
				ram = maxRam;
			}

			deallocateRamForVm(vm);

			if (AvailableRam >= ram)
			{
				AvailableRam = AvailableRam - ram;
				RamTable[vm.Uid] = ram;
				vm.CurrentAllocatedRam = getAllocatedRamForVm(vm);
				return true;
			}

			vm.CurrentAllocatedRam = getAllocatedRamForVm(vm);

			return false;
		}

		public override int getAllocatedRamForVm(Vm vm)
		{
			if (RamTable.ContainsKey(vm.Uid))
			{
				return RamTable[vm.Uid].Value;
			}
			return 0;
		}

		public override void deallocateRamForVm(Vm vm)
		{
			if (RamTable.ContainsKey(vm.Uid))
			{
				int amountFreed = RamTable[vm.Uid].Value;
                RamTable.Remove(vm.Uid);
                AvailableRam = AvailableRam + amountFreed;
				vm.CurrentAllocatedRam = 0;
			}
		}

		public override void deallocateRamForAllVms()
		{
			base.deallocateRamForAllVms();
			RamTable.Clear();
		}

		public override bool isSuitableForVm(Vm vm, int ram)
		{
			int allocatedRam = getAllocatedRamForVm(vm);
			bool result = allocateRamForVm(vm, ram);
			deallocateRamForVm(vm);
			if (allocatedRam > 0)
			{
				allocateRamForVm(vm, allocatedRam);
			}
			return result;
		}

		/// <summary>
		/// Gets the map between VMs and allocated ram.
		/// </summary>
		/// <returns> the ram map </returns>
		protected internal virtual IDictionary<string, int?> RamTable
		{
			get
			{
				return ramTable;
			}
			set
			{
				this.ramTable = value;
			}
		}


	}

}