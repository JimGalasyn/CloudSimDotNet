using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.schedulers
{

	using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
	using ContainerPeList = org.cloudbus.cloudsim.container.lists.ContainerPeList;


	/// <summary>
	/// Created by sareh on 22/07/15.
	/// </summary>
	public class ContainerSchedulerTimeSharedOverSubscription : ContainerSchedulerTimeShared
	{
		/// <summary>
		/// Instantiates a new container scheduler time shared.
		/// </summary>
		/// <param name="pelist"> the pelist </param>
		public ContainerSchedulerTimeSharedOverSubscription(IList<ContainerPe> pelist) : base(pelist)
		{
		}


        protected internal override bool allocatePesForContainer(string containerUid, IList<double?> mipsShareRequested)
		{
			double totalRequestedMips = 0;

			// if the requested mips is bigger than the capacity of a single PE, we cap
			// the request to the PE's capacity
			IList<double?> mipsShareRequestedCapped = new List<double?>();
			double peMips = PeCapacity;
			foreach (double? mips in mipsShareRequested)
			{
				if (mips > peMips)
				{
					mipsShareRequestedCapped.Add(peMips);
					totalRequestedMips += peMips;
				}
				else
				{
					mipsShareRequestedCapped.Add(mips);
					totalRequestedMips += mips.Value;
				}
			}



			if (ContainersMigratingIn.Contains(containerUid))
			{
				// the destination host only experience 10% of the migrating VM's MIPS
				totalRequestedMips = 0.0;
			}
			else
			{

				MipsMapRequested[containerUid] = mipsShareRequested;
				PesInUse = PesInUse + mipsShareRequested.Count;

			}

			if (AvailableMips >= totalRequestedMips)
			{
				IList<double?> mipsShareAllocated = new List<double?>();
				foreach (double? mipsRequested in mipsShareRequestedCapped)
				{
	//                if (getContainersMigratingOut().contains(containerUid)) {
	//                    // performance degradation due to migration = 10% MIPS
	//                    mipsRequested *= 0.9;
	//                } else
					if (!ContainersMigratingIn.Contains(containerUid))
					{
						// the destination host only experience 10% of the migrating VM's MIPS
						mipsShareAllocated.Add(mipsRequested);
					}
				}

				MipsMap[containerUid] = mipsShareAllocated;
	//            getMipsMap().put(containerUid, mipsShareRequestedCapped);
				AvailableMips = AvailableMips - totalRequestedMips;
			}
			else
			{
				redistributeMipsDueToOverSubscription();
			}

			return true;
		}

		/// <summary>
		/// This method recalculates distribution of MIPs among VMs considering eventual shortage of MIPS
		/// compared to the amount requested by VMs.
		/// </summary>
		protected internal virtual void redistributeMipsDueToOverSubscription()
		{
			// First, we calculate the scaling factor - the MIPS allocation for all VMs will be scaled
			// proportionally
			double totalRequiredMipsByAllVms = 0;

			IDictionary<string, IList<double?>> mipsMapCapped = new Dictionary<string, IList<double?>>();
            //foreach (KeyValuePair<string, IList<double?>> entry in MipsMapRequested.SetOfKeyValuePairs())
            foreach (KeyValuePair<string, IList<double?>> entry in MipsMapRequested)
            {

				double requiredMipsByThisContainer = 0.0;
				string vmId = entry.Key;
				IList<double?> mipsShareRequested = entry.Value;
				IList<double?> mipsShareRequestedCapped = new List<double?>();
				double peMips = PeCapacity;
				foreach (double? mips in mipsShareRequested)
				{
					if (mips > peMips)
					{
						mipsShareRequestedCapped.Add(peMips);
						requiredMipsByThisContainer += peMips;
					}
					else
					{
						mipsShareRequestedCapped.Add(mips);
						requiredMipsByThisContainer += mips.Value;
					}
				}

				mipsMapCapped[vmId] = mipsShareRequestedCapped;

	//            if (getContainersMigratingIn().contains(entry.getKey())) {
	//                // the destination host only experience 10% of the migrating VM's MIPS
	//                requiredMipsByThisContainer *= 0.1;
	//            }
				totalRequiredMipsByAllVms += requiredMipsByThisContainer;
			}

			double totalAvailableMips = ContainerPeList.getTotalMips(PeListProperty);
			double scalingFactor = totalAvailableMips / totalRequiredMipsByAllVms;

			// Clear the old MIPS allocation
			MipsMap.Clear();

			// Update the actual MIPS allocated to the VMs
			foreach (KeyValuePair<string, IList<double?>> entry in mipsMapCapped)
			{
				string vmUid = entry.Key;
				IList<double?> requestedMips = entry.Value;

				IList<double?> updatedMipsAllocation = new List<double?>();
				foreach (double? mips in requestedMips)
				{
	//                if (getContainersMigratingOut().contains(vmUid)) {
						// the original amount is scaled
	//                    mips *= scalingFactor;
						// performance degradation due to migration = 10% MIPS
	//                    mips *= 0.9;
	//                } else
					if (!ContainersMigratingIn.Contains(vmUid))
					{
						// the destination host only experiences 10% of the migrating VM's MIPS
                        // TODO: Fix this loop.
						//mips *= scalingFactor;

						updatedMipsAllocation.Add(Math.Floor(mips.Value));
						// the final 10% of the requested MIPS are scaled
	//                    mips *= scalingFactor;
					}


				}

				requestedMips.Clear();

				// add in the new map
				MipsMap[vmUid] = updatedMipsAllocation;

			}

			mipsMapCapped.Clear();
			// As the host is oversubscribed, there no more available MIPS
			AvailableMips = 0;
		}

	}




}