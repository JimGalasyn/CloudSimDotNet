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
    using PeProvisioner = org.cloudbus.cloudsim.provisioners.PeProvisioner;

    /// <summary>
    /// VmSchedulerTimeShared is a Virtual Machine Monitor (VMM) allocation policy that allocates one or more PEs 
    /// from a PM to a VM, and allows sharing of PEs by multiple VMs. This class also implements 10% performance degradation due
    /// to VM migration. This scheduler does not support over-subscription.
    /// 
    /// Each host has to use is own instance of a VmScheduler
    /// that will so schedule the allocation of host's PEs for VMs running on it.
    /// 
    /// @author Rodrigo N. Calheiros
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class VmSchedulerTimeShared : VmScheduler
    {

        /// <summary>
        /// The map of requested mips, where each key is a VM
        /// and each value is a list of MIPS requested by that VM. 
        /// </summary>
        private IDictionary<string, IList<double?>> mipsMapRequested;

        /// <summary>
        /// The number of host's PEs in use. </summary>
        private int pesInUse;

        /// <summary>
        /// Instantiates a new vm time-shared scheduler.
        /// </summary>
        /// <param name="pelist"> the list of PEs of the host where the VmScheduler is associated to. </param>
        public VmSchedulerTimeShared(IList<Pe> pelist) : base(pelist)
        {
            MipsMapRequested = new Dictionary<string, IList<double?>>();
        }

        public override bool allocatePesForVm(Vm vm, IList<double?> mipsShareRequested)
        {
            /*
			 * @todo add the same to RAM and BW provisioners
			 */
            if (vm.InMigration)
            {
                if (!VmsMigratingIn.Contains(vm.Uid) && !VmsMigratingOut.Contains(vm.Uid))
                {
                    VmsMigratingOut.Add(vm.Uid);
                }
            }
            else
            {
                if (VmsMigratingOut.Contains(vm.Uid))
                {
                    VmsMigratingOut.Remove(vm.Uid);
                }
            }
            bool result = allocatePesForVm(vm.Uid, mipsShareRequested);
            updatePeProvisioning();
            return result;
        }

        /// <summary>
        /// Allocate PEs for a vm.
        /// </summary>
        /// <param name="vmUid"> the vm uid </param>
        /// <param name="mipsShareRequested"> the list of mips share requested by the vm </param>
        /// <returns> true, if successful </returns>
        protected internal virtual bool allocatePesForVm(string vmUid, IList<double?> mipsShareRequested)
        {
            double totalRequestedMips = 0;
            double peMips = PeCapacity;
            foreach (double? mips in mipsShareRequested)
            {
                // each virtual PE of a VM must require not more than the capacity of a physical PE
                if (mips > peMips)
                {
                    return false;
                }
                totalRequestedMips += mips.Value;
            }

            // This scheduler does not allow over-subscription
            if (AvailableMips < totalRequestedMips)
            {
                return false;
            }

            MipsMapRequested[vmUid] = mipsShareRequested;
            PesInUse = PesInUse + mipsShareRequested.Count;

            if (VmsMigratingIn.Contains(vmUid))
            {
                // the destination host only experience 10% of the migrating VM's MIPS
                totalRequestedMips *= 0.1;
            }

            IList<double?> mipsShareAllocated = new List<double?>();
            foreach (double? mipsRequested in mipsShareRequested)
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
                //mipsShareAllocated.Add(mipsRequested);
                mipsShareAllocated.Add(mipsRequestedAdjusted);
            }

            MipsMap[vmUid] = mipsShareAllocated;
            AvailableMips = AvailableMips - totalRequestedMips;

            return true;
        }

        /// <summary>
        /// Update allocation of VMs on PEs.
        /// @too The method is too long and may be refactored to make clearer its
        /// responsibility.
        /// </summary>
        protected internal virtual void updatePeProvisioning()
        {
            PeMap.Clear();
            foreach (Pe peTmp in PeListProperty)
            {
                peTmp.PeProvisioner.deallocateMipsForAllVms();
            }

            IEnumerator<Pe> peIterator = PeListProperty.GetEnumerator();
            //Pe pe = peIterator.next();
            peIterator.MoveNext();
            Pe pe = peIterator.Current;

            PeProvisioner peProvisioner = pe.PeProvisioner;
            double availableMips = peProvisioner.AvailableMips;

            foreach (KeyValuePair<string, IList<double?>> entry in MipsMap.SetOfKeyValuePairs())
            {
                string vmUid = entry.Key;
                PeMap[vmUid] = new List<Pe>();

                foreach (double mips in entry.Value)
                {
                    double mipsTemp = mips;
                    while (mipsTemp >= 0.1)
                    {
                        if (availableMips >= mipsTemp)
                        {
                            peProvisioner.allocateMipsForVm(vmUid, mipsTemp);
                            PeMap[vmUid].Add(pe);
                            availableMips -= mipsTemp;
                            break;
                        }
                        else
                        {
                            peProvisioner.allocateMipsForVm(vmUid, availableMips);
                            PeMap[vmUid].Add(pe);
                            mipsTemp -= availableMips;
                            if (mipsTemp <= 0.1)
                            {
                                break;
                            }

                            //if (!peIterator.hasNext())
                            if (!peIterator.MoveNext())
                            {

                                Log.printConcatLine("There is not enough MIPS (", mips, ") to accommodate VM ", vmUid);
                                throw new System.InvalidOperationException("Not enough MIPS");
                                // System.exit(0);
                            }

                            //pe = peIterator.next();
                            pe = peIterator.Current;
                            peProvisioner = pe.PeProvisioner;
                            availableMips = peProvisioner.AvailableMips;
                        }
                    }
                }
            }
        }

        public override void deallocatePesForVm(Vm vm)
        {
            MipsMapRequested.Remove(vm.Uid);
            PesInUse = 0;
            MipsMap.Clear();
            AvailableMips = PeList.getTotalMips(PeListProperty);

            foreach (Pe pe in PeListProperty)
            {
                pe.PeProvisioner.deallocateMipsForVm(vm);
            }

            foreach (KeyValuePair<string, IList<double?>> entry in MipsMapRequested.SetOfKeyValuePairs())
            {
                allocatePesForVm(entry.Key, entry.Value);
            }

            updatePeProvisioning();
        }

        /// <summary>
        /// Releases PEs allocated to all the VMs.
        /// 
        /// @pre $none
        /// @post $none
        /// </summary>
        public override void deallocatePesForAllVms()
        {
            base.deallocatePesForAllVms();
            MipsMapRequested.Clear();
            PesInUse = 0;
        }

        /// <summary>
        /// Returns maximum available MIPS among all the PEs. For the time shared policy it is just all
        /// the avaiable MIPS.
        /// </summary>
        /// <returns> max mips </returns>
        public override double MaxAvailableMips
        {
            get
            {
                return AvailableMips;
            }
        }

        /// <summary>
        /// Sets the number of PEs in use.
        /// </summary>
        /// <param name="pesInUse"> the new pes in use </param>
        protected internal virtual int PesInUse
        {
            set
            {
                this.pesInUse = value;
            }
            get
            {
                return pesInUse;
            }
        }


        /// <summary>
        /// Gets the mips map requested.
        /// </summary>
        /// <returns> the mips map requested </returns>
        protected internal virtual IDictionary<string, IList<double?>> MipsMapRequested
        {
            get
            {
                return mipsMapRequested;
            }
            set
            {
                this.mipsMapRequested = value;
            }
        }


    }

}