using System;
using System.Collections.Generic;

/// 
namespace org.cloudbus.cloudsim.container.containerProvisioners
{
    using System.Diagnostics;
    using Container = org.cloudbus.cloudsim.container.core.Container;

    /// <summary>
    /// @author Sareh Fotuhi Piraghaj
    /// </summary>
    public class ContainerPeProvisionerSimple : ContainerPeProvisioner
	{

		/// <summary>
		/// The pe table.
		/// </summary>
		private IDictionary<string, List<double?>> peTable;
		/// <param name="mips"> </param>
		/// <summary>
		/// Creates the PeProvisionerSimple object.
		/// </summary>
		/// <param name="availableMips"> the available mips
		/// @pre $none
		/// @post $none </param>
		public ContainerPeProvisionerSimple(double availableMips) : base(availableMips)
		{
			PeTable = new Dictionary<string, List<double?>>();
		}

		/* (non-Javadoc)
		 * @see ContainerVmPeProvisioner#allocateMipsForContainer
		 */
		public override bool allocateMipsForContainer(Container container, double mips)
		{
            // TEST: (fixed) Auto-generated method stub

            return allocateMipsForContainer(container.Uid, mips);
		}

		/* (non-Javadoc)
		 * @see ContainerVmPeProvisioner#allocateMipsForContainer(java.lang.String, double)
		 */
		public override bool allocateMipsForContainer(string containerUid, double mips)
		{
			if (AvailableMips < mips)
			{
				return false;
			}

			List<double?> allocatedMips;

			if (PeTable.ContainsKey(containerUid))
			{
				allocatedMips = PeTable[containerUid];
			}
			else
			{
				allocatedMips = new List<double?>();
			}

			allocatedMips.Add(mips);

			AvailableMips = AvailableMips - mips;
			PeTable[containerUid] = allocatedMips;

			return true;
		}

		/* (non-Javadoc)
		 * @see ContainerVmPeProvisioner#allocateMipsForContainer(Container, java.util.List)
		 */
		public override bool allocateMipsForContainer(Container container, List<double?> mips)
		{
			int totalMipsToAllocate = 0;
			foreach (double _mips in mips)
			{
				totalMipsToAllocate += (int)_mips;
			}

			if (AvailableMips + getTotalAllocatedMipsForContainer(container) < totalMipsToAllocate)
			{
				return false;
			}

			AvailableMips = AvailableMips + getTotalAllocatedMipsForContainer(container) - totalMipsToAllocate;
            
			PeTable[container.Uid] = mips;

			return true;
		}

		/*
		 * (non-Javadoc)
		 * @see containerProvisioners.PeProvisioner#deallocateMipsForAllContainers()
		 */
		public override void deallocateMipsForAllContainers()
		{
			base.deallocateMipsForAllContainers();
			PeTable.Clear();
		}

		/* (non-Javadoc)
		 * @see ContainerPeProvisioner#getAllocatedMipsForContainer
		 */
		public override IList<double?> getAllocatedMipsForContainer(Container container)
		{
			if (PeTable.ContainsKey(container.Uid))
			{
				return PeTable[container.Uid];
			}
			return null;
		}

		/* (non-Javadoc)
		 * @see ContainerVmPeProvisioner#getTotalAllocatedMipsForContainer
		 */
		public override double getTotalAllocatedMipsForContainer(Container container)
		{
			if (PeTable.ContainsKey(container.Uid))
			{
				double totalAllocatedMips = 0.0;
				foreach (double mips in PeTable[container.Uid])
				{
					totalAllocatedMips += mips;
				}
				return totalAllocatedMips;
			}
			return 0;
		}

		/* (non-Javadoc)
		 * @see ContainerVmPeProvisioner#getAllocatedMipsForContainerByVirtualPeId(Container, int)
		 */
		public override double getAllocatedMipsForContainerByVirtualPeId(Container container, int peId)
		{
			if (PeTable.ContainsKey(container.Uid))
			{
				try
				{
                    return PeTable[container.Uid][peId].Value;
				}
				catch (Exception e)
				{
                    Debug.WriteLine(e.ToString());
                    throw e;
				}
			}
			return 0;
		}

		/* (non-Javadoc)
		 * @see ContainerVmPeProvisioner#deallocateMipsForContainer(Container)
		 */
		public override void deallocateMipsForContainer(Container container)
		{
			if (PeTable.ContainsKey(container.Uid))
			{
				foreach (double mips in PeTable[container.Uid])
				{
					AvailableMips = AvailableMips + mips;
				}
				PeTable.Remove(container.Uid);
			}
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