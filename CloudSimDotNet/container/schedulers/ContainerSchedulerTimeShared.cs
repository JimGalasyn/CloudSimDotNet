using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.schedulers
{

    using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
    using ContainerPeList = org.cloudbus.cloudsim.container.lists.ContainerPeList;
    using ContainerPeProvisioner = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPeProvisioner;
    using Container = org.cloudbus.cloudsim.container.core.Container;
    using System;

    /// <summary>
    /// Created by sareh on 9/07/15.
    /// </summary>
    public class ContainerSchedulerTimeShared : ContainerScheduler
	{
		/// <summary>
		/// The mips map requested.
		/// </summary>
		private IDictionary<string, IList<double?>> mipsMapRequested;

		/// <summary>
		/// The pes in use.
		/// </summary>
		private int pesInUse;

		/// <summary>
		/// Instantiates a new container scheduler time shared.
		/// </summary>
		/// <param name="pelist"> the pelist </param>
		public ContainerSchedulerTimeShared(IList<ContainerPe> pelist) : base(pelist)
		{
			MipsMapRequested = new Dictionary<string, IList<double?>>();
		}


		public override bool allocatePesForContainer(Container container, IList<double?> mipsShareRequested)
		{
			/// <summary>
			/// TODO: add the same to RAM and BW provisioners
			/// </summary>
	//        Log.printLine("ContainerSchedulerTimeShared: allocatePesForContainer with mips share size......" + mipsShareRequested.size());
	//        if (container.isInMigration()) {
	//            if (!getContainersMigratingIn().contains(container.getUid()) && !getContainersMigratingOut().contains(container.getUid())) {
	//                getContainersMigratingOut().add(container.getUid());
	//            }
	//        } else {
	//            if (getContainersMigratingOut().contains(container.getUid())) {
	//                getContainersMigratingOut().remove(container.getUid());
	//            }
	//        }
			bool result = allocatePesForContainer(container.Uid, mipsShareRequested);
			updatePeProvisioning();
			return result;
		}

		/// <summary>
		/// Update allocation of VMs on PEs.
		/// </summary>
		protected internal virtual void updatePeProvisioning()
		{
	//        Log.printLine("VmSchedulerTimeShared: update the pe provisioning......");
			PeMap.Clear();
			foreach (ContainerPe peTemp in PeListProperty)
			{
                peTemp.ContainerPeProvisionerProperty.deallocateMipsForAllContainers();
            }

            IEnumerator<ContainerPe> peIterator = PeListProperty.GetEnumerator();
            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
            // TODO: Check this iterator business.
            //ContainerPe pe = peIterator.next();
            ContainerPe pe = peIterator.Current;
            
            ContainerPeProvisioner containerPeProvisioner = pe.ContainerPeProvisionerProperty;
			double availableMips = containerPeProvisioner.AvailableMips;

            //foreach (KeyValuePair<string, IList<double?>> entry in MipsMap.entrySet())
            foreach (KeyValuePair<string, IList<double?>> entry in MipsMap)
            {
				string containerUid = entry.Key;
				PeMap[containerUid] = new List<ContainerPe>();

				foreach (double mips in entry.Value)
				{
					while (mips >= 0.1)
					{
						if (availableMips >= mips)
						{
							containerPeProvisioner.allocateMipsForContainer(containerUid, mips);
							PeMap[containerUid].Add(pe);
							availableMips -= mips;
							break;
						}
						else
						{
							containerPeProvisioner.allocateMipsForContainer(containerUid, availableMips);
							if (availableMips != 0)
							{
								PeMap[containerUid].Add(pe);
							}
                            // TODO: fix this loop
							//mips -= availableMips;
							if (mips <= 0.1)
							{
								break;
							}
                            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                            //if (!peIterator.hasNext())
                            if (!peIterator.MoveNext())
                            {
								Log.printConcatLine("There is not enough MIPS (", mips, ") to accommodate VM ", containerUid);
                                // System.exit(0);
                                throw new InvalidOperationException("Not enough MIPS");
							}
                            //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                            //pe = peIterator.next();
                            pe = peIterator.Current;
                            // TEST: (fixed) Figure out pe.ContainerPeProvisioner problem.
                            containerPeProvisioner = pe.ContainerPeProvisionerProperty;
                            availableMips = containerPeProvisioner.AvailableMips;
						}
					}
				}
			}
		}

		/// <summary>
		/// Allocate pes for vm.
		/// </summary>
		/// <param name="containerUid">       the containerUid </param>
		/// <param name="mipsShareRequested"> the mips share requested </param>
		/// <returns> true, if successful </returns>
		protected internal virtual bool allocatePesForContainer(string containerUid, IList<double?> mipsShareRequested)
		{
	//        Log.printLine("ContainerSchedulerTimeShared: allocatePesForContainerVm for containerUid......"+containerUid);
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

			MipsMapRequested[containerUid] = mipsShareRequested;
			PesInUse = PesInUse + mipsShareRequested.Count;

			if (ContainersMigratingIn.Contains(containerUid))
			{
				// the destination host only experience nothing of the migrating Containers MIPS
				totalRequestedMips = 0;
	//            totalRequestedMips *= 0.1;
			}

			IList<double?> mipsShareAllocated = new List<double?>();
			foreach (double? mipsRequested in mipsShareRequested)
			{
	//            if (getContainersMigratingOut().contains(containerUid)) {
	//                // It should handle the container load fully
	////                mipsRequested *= 0.9;
	//            } else
				if (ContainersMigratingIn.Contains(containerUid))
				{
					// not responsible for those moving in
                    // TODO: Figure out this loop biz.
					//mipsRequested = 0.0;
				}
				mipsShareAllocated.Add(mipsRequested);
			}

			MipsMap[containerUid] = mipsShareAllocated;
			AvailableMips = AvailableMips - totalRequestedMips;

			return true;
		}

		public override void deallocatePesForContainer(Container container)
		{
	//        Log.printLine("containerSchedulerTimeShared: deallocatePesForContainer.....");
			MipsMapRequested.Remove(container.Uid);
			PesInUse = 0;
			MipsMap.Clear();
			AvailableMips = ContainerPeList.getTotalMips(PeListProperty);

			foreach (ContainerPe pe in PeListProperty)
			{
                pe.ContainerPeProvisionerProperty.deallocateMipsForContainer(container);
            }
            //        Log.printLine("SchedulerTimeShared: deallocatePesForContainerVm. allocates again acording to the left!!!!!!!....");
            foreach (KeyValuePair<string, IList<double?>> entry in MipsMapRequested)
			{
				allocatePesForContainer(entry.Key, entry.Value);
			}

			updatePeProvisioning();


		}

		public override void deallocatePesForAllContainers()
		{
			base.deallocatePesForAllContainers();
			MipsMapRequested.Clear();
			PesInUse = 0;
		}

		public override double MaxAvailableMips
		{
			get
			{
				return AvailableMips;
			}
		}


		/// <summary>
		/// Sets the pes in use.
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