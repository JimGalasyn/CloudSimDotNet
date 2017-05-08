using System;
using System.Collections.Generic;

/*
 * Title: CloudSim Toolkit Description: CloudSim (Cloud Simulation) Toolkit for Modeling and
 * Simulation of Clouds Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * 
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	using PeList = org.cloudbus.cloudsim.lists.PeList;

	/// <summary>
	/// This is a Time-Shared VM Scheduler, which allows over-subscription. In other words, the scheduler
	/// still allows the allocation of VMs that require more CPU capacity than is available.
	/// Oversubscription results in performance degradation.
	/// 
	/// @author Anton Beloglazov
	/// @author Rodrigo N. Calheiros
	/// @since CloudSim Toolkit 3.0
	/// </summary>
	public class VmSchedulerTimeSharedOverSubscription : VmSchedulerTimeShared
	{

		/// <summary>
		/// Instantiates a new vm scheduler time shared over subscription.
		/// </summary>
		/// <param name="pelist"> the list of PEs of the host where the VmScheduler is associated to. </param>
		public VmSchedulerTimeSharedOverSubscription(IList<Pe> pelist)  : base(pelist)
		{
		}

		/// <summary>
		/// Allocates PEs for vm. The policy allows over-subscription. In other words, the policy still
		/// allows the allocation of VMs that require more CPU capacity than is available.
		/// Oversubscription results in performance degradation.
		/// It cannot be allocated more CPU capacity for each virtual PE than the MIPS 
		/// capacity of a single physical PE.
		/// </summary>
		/// <param name="vmUid"> the vm uid </param>
		/// <param name="mipsShareRequested"> the list of mips share requested </param>
		/// <returns> true, if successful </returns>
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
                    var mipsRequestedAdjusted = mipsRequested;

                    if (VmsMigratingOut.Contains(vmUid))
					{
                        // performance degradation due to migration = 10% MIPS
                        //mipsRequested *= 0.9;
                        mipsRequestedAdjusted *= 0.9;

                    }
					else if (VmsMigratingIn.Contains(vmUid))
					{
                        // the destination host only experience 10% of the migrating VM's MIPS
                        //mipsRequested *= 0.1;
                        mipsRequestedAdjusted *= 0.1;

                    }
					mipsShareAllocated.Add(mipsRequestedAdjusted);
				}

				MipsMap[vmUid] = mipsShareAllocated;
				AvailableMips = AvailableMips - totalRequestedMips;
			}
			else
			{
				redistributeMipsDueToOverSubscription();
			}

			return true;
		}

		/// <summary>
		/// Recalculates distribution of MIPs among VMs, considering eventual shortage of MIPS
		/// compared to the amount requested by VMs.
		/// </summary>
		protected internal virtual void redistributeMipsDueToOverSubscription()
		{
			// First, we calculate the scaling factor - the MIPS allocation for all VMs will be scaled
			// proportionally
			double totalRequiredMipsByAllVms = 0;

			IDictionary<string, IList<double?>> mipsMapCapped = new Dictionary<string, IList<double?>>();
            //foreach (KeyValuePair<string, IList<double?>> entry in MipsMapRequested.entrySet())
            // TEST: (fixed) What's deal with entrySet?
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

			double totalAvailableMips = PeList.getTotalMips(PeListProperty);
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
                    double mipsAdjusted = mips.Value;

					if (VmsMigratingOut.Contains(vmUid))
					{
                        // the original amount is scaled
                        mipsAdjusted *= scalingFactor;
                        // performance degradation due to migration = 10% MIPS
                        mipsAdjusted *= 0.9;
					}
					else if (VmsMigratingIn.Contains(vmUid))
					{
                        // the destination host only experiences 10% of the migrating VM's MIPS
                        mipsAdjusted *= 0.1;
                        // the final 10% of the requested MIPS are scaled
                        mipsAdjusted *= scalingFactor;
					}
					else
					{
                        mipsAdjusted *= scalingFactor;
					}

					updatedMipsAllocation.Add(Math.Floor(mipsAdjusted));
				}

				// add in the new map
				MipsMap[vmUid] = updatedMipsAllocation;
			}

			// As the host is oversubscribed, there no more available MIPS
			AvailableMips = 0;
		}
	}
}