using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.schedulers
{

    using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
    using ContainerVmPeList = org.cloudbus.cloudsim.container.lists.ContainerVmPeList;
    using ContainerVmPeProvisioner = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPeProvisioner;
    using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
    using System;

    /// <summary>
    /// Created by sareh on 14/07/15.
    /// </summary>
    public class ContainerVmSchedulerTimeShared : ContainerVmScheduler
    {
        /// <summary>
        /// The mips map requested. </summary>
        private IDictionary<string, IList<double?>> mipsMapRequested;

        /// <summary>
        /// The pes in use. </summary>
        private int pesInUse;

        /// <summary>
        /// Instantiates a new vm scheduler time shared.
        /// </summary>
        /// <param name="pelist"> the pelist </param>
        public ContainerVmSchedulerTimeShared(IList<ContainerVmPe> pelist) : base(pelist)
        {
            MipsMapRequested = new Dictionary<string, IList<double?>>();
        }

        public override bool allocatePesForVm(ContainerVm containerVm, IList<double?> mipsShare)
        {
            //Log.printLine("VmSchedulerTimeShared: allocatePesForVm with mips share size......" + mipsShare.size());
            if (containerVm.InMigration)
            {
                if (!VmsMigratingIn.Contains(containerVm.Uid) && !VmsMigratingOut.Contains(containerVm.Uid))
                {
                    VmsMigratingOut.Add(containerVm.Uid);
                }
            }
            else
            {
                if (VmsMigratingOut.Contains(containerVm.Uid))
                {
                    VmsMigratingOut.Remove(containerVm.Uid);
                }
            }
            bool result = allocatePesForVm(containerVm.Uid, mipsShare);
            updatePeProvisioning();
            return result;
        }

        /// <summary>
        /// Allocate pes for vm.
        /// </summary>
        /// <param name="vmUid"> the vm uid </param>
        /// <param name="mipsShareRequested"> the mips share requested </param>
        /// <returns> true, if successful </returns>
        protected internal virtual bool allocatePesForVm(string vmUid, IList<double?> mipsShareRequested)
        {
            //Log.printLine("VmSchedulerTimeShared: allocatePesForVm for Vmuid......"+vmUid);
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
                if (VmsMigratingOut.Contains(vmUid))
                {
                    // performance degradation due to migration = 10% MIPS
                    // TODO: Figure out this iteration business.
                    //mipsRequested *= 0.9;
                }
                else if (VmsMigratingIn.Contains(vmUid))
                {
                    // the destination host only experience 10% of the migrating VM's MIPS
                    // TODO: Figure out this iteration business.	
                    //mipsRequested *= 0.1;
                }
                mipsShareAllocated.Add(mipsRequested);
            }

            MipsMap[vmUid] = mipsShareAllocated;
            AvailableMips = AvailableMips - totalRequestedMips;

            return true;
        }

        /// <summary>
        /// Update allocation of VMs on PEs.
        /// </summary>
        protected internal virtual void updatePeProvisioning()
        {
            // Log.printLine("VmSchedulerTimeShared: update the pe provisioning......");
            PeMap.Clear();
            // Log.printConcatLine("The Pe Map is being cleared ");
            foreach (ContainerVmPe peTemp in PeListProperty)
            {
                peTemp.ContainerVmPeProvisioner.deallocateMipsForAllContainerVms();
            }

            IEnumerator<ContainerVmPe> peIterator = PeListProperty.GetEnumerator();
            // TODO: Make sure this iterator business works.
            //ContainerVmPe containerVmPe = peIterator.next();
            ContainerVmPe containerVmPe = peIterator.Current;

            ContainerVmPeProvisioner containerVmPeProvisioner = containerVmPe.ContainerVmPeProvisioner;
            double availableMips = containerVmPeProvisioner.AvailableMips;

            foreach (KeyValuePair<string, IList<double?>> entry in MipsMap)
            {
                string vmUid = entry.Key;
                PeMap[vmUid] = new List<ContainerVmPe>();

                foreach (double mips in entry.Value)
                {
                    double mipsTemp = mips;
                    while (mipsTemp >= 0.1)
                    {
                        if (availableMips >= mipsTemp)
                        {
                            containerVmPeProvisioner.allocateMipsForContainerVm(vmUid, mipsTemp);
                            PeMap[vmUid].Add(containerVmPe);
                            // Log.formatLine("The allocated Mips is % f to Pe Id % d", mips, pe.getId());
                            availableMips -= mipsTemp;
                            // Log.print(getPeMap().get(vmUid));
                            break;
                        }
                        else
                        {
                            containerVmPeProvisioner.allocateMipsForContainerVm(vmUid, availableMips);
                            if (availableMips != 0)
                            {
                                PeMap[vmUid].Add(containerVmPe);
                            }

                            mipsTemp -= availableMips;
                            // Log.print(getPeMap().get(vmUid));
                            if (mipsTemp <= 0.1)
                            {
                                break;
                            }

                            //if (!peIterator.hasNext())
                            if (!peIterator.MoveNext())
                            {
                                Log.printConcatLine("There are not enough MIPS (", mipsTemp, ") to accommodate VM ", vmUid);
                                // System.exit(0);
                                throw new InvalidOperationException("Not enough MIPS available");
                            }

                            //containerVmPe = peIterator.next();
                            containerVmPe = peIterator.Current;

                            containerVmPeProvisioner = containerVmPe.ContainerVmPeProvisioner;
                            availableMips = containerVmPeProvisioner.AvailableMips;
                        }
                    }
                }
            }
            // Log.printConcatLine("These are the values",getPeMap().keySet());
        }

        public override void deallocatePesForVm(ContainerVm containerVm)
        {
            //Log.printLine("VmSchedulerTimeShared: deallocatePesForVm.....");
            MipsMapRequested.Remove(containerVm.Uid);
            PesInUse = 0;
            MipsMap.Clear();
            AvailableMips = ContainerVmPeList.getTotalMips(PeListProperty);

            foreach (ContainerVmPe pe in PeListProperty)
            {
                pe.ContainerVmPeProvisioner.deallocateMipsForContainerVm(containerVm);
            }
            //Log.printLine("VmSchedulerTimeShared: deallocatePesForVm. allocates again!!!!!!!....");
            foreach (KeyValuePair<string, IList<double?>> entry in MipsMapRequested)
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
        public override void deallocatePesForAllContainerVms()
        {
            base.deallocatePesForAllContainerVms();
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

        public virtual IDictionary<string, IList<double?>> MipsMapRequested
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

        public virtual int PesInUse
        {
            get
            {
                return pesInUse;
            }
            set
            {
                this.pesInUse = value;
            }
        }
    }
}