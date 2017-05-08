using System;
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
	/// PeProvisionerSimple is an extension of <seealso cref="PeProvisioner"/> which uses a best-effort policy to
	/// allocate virtual PEs to VMs: 
	/// if there is available mips on the physical PE, it allocates to a virtual PE; otherwise, it fails. 
	/// Each host's PE has to have its own instance of a PeProvisioner.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public class PeProvisionerSimple : PeProvisioner
	{

		/// <summary>
		/// The PE map, where each key is a VM id and each value
		/// is the list of PEs (in terms of their amount of MIPS) 
		/// allocated to that VM. 
		/// </summary>
		private IDictionary<string, IList<double?>> peTable;

		/// <summary>
		/// Instantiates a new pe provisioner simple.
		/// </summary>
		/// <param name="availableMips"> The total mips capacity of the PE that the provisioner can allocate to VMs. 
		/// 
		/// @pre $none
		/// @post $none </param>
		public PeProvisionerSimple(double availableMips) : base(availableMips)
		{
			PeTable = new Dictionary<string, IList<double?>>();
		}

		public override bool allocateMipsForVm(Vm vm, double mips)
		{
			return allocateMipsForVm(vm.Uid, mips);
		}

		public override bool allocateMipsForVm(string vmUid, double mips)
		{
			if (AvailableMips < mips)
			{
				return false;
			}

			IList<double?> allocatedMips;

			if (PeTable.ContainsKey(vmUid))
			{
				allocatedMips = PeTable[vmUid];
			}
			else
			{
				allocatedMips = new List<double?>();
			}

			allocatedMips.Add(mips);

			AvailableMips = AvailableMips - mips;
			PeTable[vmUid] = allocatedMips;

			return true;
		}

		public override bool allocateMipsForVm(Vm vm, IList<double?> mips)
		{
			int totalMipsToAllocate = 0;
			foreach (double _mips in mips)
			{
				totalMipsToAllocate += (int)_mips;
			}

			if (AvailableMips + getTotalAllocatedMipsForVm(vm) < totalMipsToAllocate)
			{
				return false;
			}

			AvailableMips = AvailableMips + getTotalAllocatedMipsForVm(vm) - totalMipsToAllocate;

			PeTable[vm.Uid] = mips;

			return true;
		}

		public override void deallocateMipsForAllVms()
		{
			base.deallocateMipsForAllVms();
			PeTable.Clear();
		}

		public override double getAllocatedMipsForVmByVirtualPeId(Vm vm, int peId)
		{
			if (PeTable.ContainsKey(vm.Uid))
			{
				try
				{
					return PeTable[vm.Uid][peId].Value;
				}
				catch (Exception)
				{
				}
			}
			return 0;
		}

		public override IList<double?> getAllocatedMipsForVm(Vm vm)
		{
			if (PeTable.ContainsKey(vm.Uid))
			{
				return PeTable[vm.Uid];
			}
			return null;
		}

		public override double getTotalAllocatedMipsForVm(Vm vm)
		{
			if (PeTable.ContainsKey(vm.Uid))
			{
				double totalAllocatedMips = 0.0;
				foreach (double mips in PeTable[vm.Uid])
				{
					totalAllocatedMips += mips;
				}
				return totalAllocatedMips;
			}
			return 0;
		}

		public override void deallocateMipsForVm(Vm vm)
		{
			if (PeTable.ContainsKey(vm.Uid))
			{
				foreach (double mips in PeTable[vm.Uid])
				{
					AvailableMips = AvailableMips + mips;
				}
				PeTable.Remove(vm.Uid);
			}
		}

		/// <summary>
		/// Gets the pe map.
		/// </summary>
		/// <returns> the pe map </returns>
		protected internal virtual IDictionary<string, IList<double?>> PeTable
		{
			get
			{
				return peTable;
			}
			set
			{
				this.peTable = (IDictionary<string, IList<double?>>) value;
			}
		}


	}

}