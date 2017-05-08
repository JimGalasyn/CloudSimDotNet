using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.schedulers
{

	using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
	using ContainerVmPeList = org.cloudbus.cloudsim.container.lists.ContainerVmPeList;


	/// <summary>
	/// Created by sareh on 23/07/15.
	/// </summary>
	public class ContainerVmSchedulerTimeSharedOverSubscription : ContainerVmSchedulerTimeShared
	{
		public ContainerVmSchedulerTimeSharedOverSubscription(IList<ContainerVmPe> pelist) : base(pelist)
		{
		}

        // TEST: (fixed) Weird inheritance error.
        protected internal override bool allocatePesForVm(string vmUid, IList<double?> mipsShareRequested)
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

			MipsMapRequested[vmUid] = mipsShareRequested;
			PesInUse = PesInUse + mipsShareRequested.Count;

			if (VmsMigratingIn.Contains(vmUid))
			{
				// the destination host only experience 10% of the migrating VM's MIPS
				totalRequestedMips *= 0.1;
			}

			if (AvailableMips >= totalRequestedMips)
			{
				IList<double?> mipsShareAllocated = new List<double?>();
				foreach (double? mipsRequested in mipsShareRequestedCapped)
				{
					if (VmsMigratingOut.Contains(vmUid))
					{
						// performance degradation due to migration = 10% MIPS
                        // TODO: iteration variable
						//mipsRequested *= 0.9;
					}
					else if (VmsMigratingIn.Contains(vmUid))
					{
                        // the destination host only experience 10% of the migrating VM's MIPS
                        // TODO: iteration variable	
                        //mipsRequested *= 0.1;
                    }
                    mipsShareAllocated.Add(mipsRequested);
				}

				MipsMap[vmUid] = mipsShareAllocated;
				AvailableMips = AvailableMips - totalRequestedMips;
			}
			else
			{
				redistributeMipsDueToOverSubscription();
			}

			mipsShareRequestedCapped.Clear();
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
			foreach (KeyValuePair<string, IList<double?>> entry in MipsMapRequested)
			{

				double requiredMipsByThisVm = 0.0;
				string vmId = entry.Key;
				IList<double?> mipsShareRequested = entry.Value;
				IList<double?> mipsShareRequestedCapped = new List<double?>();
				double peMips = PeCapacity;
				foreach (double? mips in mipsShareRequested)
				{
					if (mips > peMips)
					{
						mipsShareRequestedCapped.Add(peMips);
						requiredMipsByThisVm += peMips;
					}
					else
					{
						mipsShareRequestedCapped.Add(mips);
						requiredMipsByThisVm += mips.Value;
					}
				}

				mipsMapCapped[vmId] = mipsShareRequestedCapped;

				if (VmsMigratingIn.Contains(entry.Key))
				{
					// the destination host only experience 10% of the migrating VM's MIPS
					requiredMipsByThisVm *= 0.1;
				}
				totalRequiredMipsByAllVms += requiredMipsByThisVm;
			}

			double totalAvailableMips = ContainerVmPeList.getTotalMips(PeListProperty);
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
					if (VmsMigratingOut.Contains(vmUid))
					{
                        // the original amount is scaled
                        // TODO: iteration variable
                        //mips *= scalingFactor;
                        // performance degradation due to migration = 10% MIPS
                        // TODO: iteration variable	
                        //mips *= 0.9;
                    }
                    else if (VmsMigratingIn.Contains(vmUid))
					{
                        // the destination host only experiences 10% of the migrating VM's MIPS
                        // TODO: iteration variable	
                        //mips *= 0.1;
                        // the final 10% of the requested MIPS are scaled
                        // TODO: iteration variable
                        //mips *= scalingFactor;
                    }
                    else
					{
                        // TODO: iteration variable
                        //mips *= scalingFactor;
					}

                    // TODO: iteration variable
                    updatedMipsAllocation.Add(Math.Floor(mips.Value));
				}

				// add in the new map
				MipsMap[vmUid] = updatedMipsAllocation;

			}

			mipsMapCapped.Clear();

			// As the host is oversubscribed, there no more available MIPS
			AvailableMips = 0;
		}



	}

}