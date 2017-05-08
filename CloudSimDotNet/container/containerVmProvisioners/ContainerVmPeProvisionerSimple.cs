using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerVmProvisioners
{


	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;


	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerVmPeProvisionerSimple : ContainerVmPeProvisioner
	{


		/// <summary>
		/// The pe table. </summary>
		private IDictionary<string, List<double?>> peTable;

		/// <summary>
		/// Creates the PeProvisionerSimple object.
		/// </summary>
		/// <param name="availableMips"> the available mips
		/// 
		/// @pre $none
		/// @post $none </param>
		public ContainerVmPeProvisionerSimple(double availableMips) : base(availableMips)
		{
			PeTable = new Dictionary<string, List<double?>>();
		}




		public override bool allocateMipsForContainerVm(ContainerVm containerVm, double mips)
		{

			return allocateMipsForContainerVm(containerVm.Uid, mips);
		}

		public override bool allocateMipsForContainerVm(string containerVmUid, double mips)
		{
			if (AvailableMips < mips)
			{
				return false;
			}

			List<double?> allocatedMips;

			if (PeTable.ContainsKey(containerVmUid))
			{
				allocatedMips = PeTable[containerVmUid];
			}
			else
			{
				allocatedMips = new List<double?>();
			}

			allocatedMips.Add(mips);

			AvailableMips = AvailableMips - mips;
			PeTable[containerVmUid] = allocatedMips;

			return true;
		}

		public override bool allocateMipsForContainerVm(ContainerVm containerVm, IList<double?> mips)
		{
			int totalMipsToAllocate = 0;
			foreach (double _mips in mips)
			{
				totalMipsToAllocate += (int)_mips;
			}

			if (AvailableMips + getTotalAllocatedMipsForContainerVm(containerVm) < totalMipsToAllocate)
			{
				return false;
			}

			AvailableMips = AvailableMips + getTotalAllocatedMipsForContainerVm(containerVm) - totalMipsToAllocate;

            // TEST: (fixed) Fix this ambiguity issue.
            PeTable[containerVm.Uid] = (List<double?>)mips;

			return true;
		}

		public override IList<double?> getAllocatedMipsForContainerVm(ContainerVm containerVm)
		{
			if (PeTable.ContainsKey(containerVm.Uid))
			{
				return PeTable[containerVm.Uid];
			}
			return null;
		}

		public override double getTotalAllocatedMipsForContainerVm(ContainerVm containerVm)
		{
			if (PeTable.ContainsKey(containerVm.Uid))
			{
				double totalAllocatedMips = 0.0;
				foreach (double mips in PeTable[containerVm.Uid])
				{
					totalAllocatedMips += mips;
				}
				return totalAllocatedMips;
			}
			return 0;
		}

		public override double getAllocatedMipsForContainerVmByVirtualPeId(ContainerVm containerVm, int peId)
		{
			if (PeTable.ContainsKey(containerVm.Uid))
			{
				try
				{
					return PeTable[containerVm.Uid][peId].Value;
				}
				catch (Exception)
				{
				}
			}
			return 0;
		}

		public override void deallocateMipsForContainerVm(ContainerVm containerVm)
		{
			if (PeTable.ContainsKey(containerVm.Uid))
			{
				foreach (double mips in PeTable[containerVm.Uid])
				{
					AvailableMips = AvailableMips + mips;
				}
				PeTable.Remove(containerVm.Uid);
			}
		}

		public override void deallocateMipsForAllContainerVms()
		{
			base.deallocateMipsForAllContainerVms();
			PeTable.Clear();
		}
		/// <summary>
		/// Gets the pe table.
		/// </summary>
		/// <returns> the peTable </returns>
		protected internal virtual IDictionary<string, List<double?>> PeTable
		{
			get
			{
				return peTable;
			}
			set
			{
				this.peTable = (IDictionary<string, List<double?>>) value;
			}
		}

	}

}