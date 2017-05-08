using CloudSimLib.util;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	/// <summary>
	/// VmSchedulerSpaceShared is a VMM allocation policy that allocates one or more PEs from a host to a 
	/// Virtual Machine Monitor (VMM), and doesn't allow sharing of PEs. 
	/// The allocated PEs will be used until the VM finishes running. 
	/// If there is no enough free PEs as required by a VM,
	/// or whether the available PEs doesn't have enough capacity, the allocation fails. 
	/// In the case of fail, no PE is allocated to the requesting VM.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class VmSchedulerSpaceShared : VmScheduler
	{

		/// <summary>
		/// A map between each VM and its allocated PEs, where the key is a VM ID and
		/// the value a list of PEs allocated to VM. 
		/// </summary>
		private IDictionary<string, IList<Pe>> peAllocationMap;

		/// <summary>
		/// The list of free PEs yet available in the host. </summary>
		private IList<Pe> freePes;

		/// <summary>
		/// Instantiates a new vm space-shared scheduler.
		/// </summary>
		/// <param name="pelist"> the pelist </param>
		public VmSchedulerSpaceShared(IList<Pe> pelist) : base(pelist)
		{
			PeAllocationMap = new Dictionary<string, IList<Pe>>();
			FreePes = new List<Pe>();
			((List<Pe>)FreePes).AddRange(pelist);
		}

		public override bool allocatePesForVm(Vm vm, IList<double?> mipsShare)
		{
			// if there is no enough free PEs, fails
			if (FreePes.Count < mipsShare.Count)
			{
				return false;
			}

			IList<Pe> selectedPes = new List<Pe>();
			IEnumerator<Pe> peIterator = FreePes.GetEnumerator();
            // TEST: (fixed) Make sure this loop works.
            //Pe pe = peIterator.next();
            peIterator.MoveNext();
            Pe pe = peIterator.Current;
            double totalMips = 0;
			foreach (double? mips in mipsShare)
			{
				if (mips <= pe.Mips)
				{
					selectedPes.Add(pe);
                    //if (!peIterator.hasNext())
                    if (!peIterator.MoveNext())
                    {
						break;
					}
                    //pe = peIterator.next();
                    pe = peIterator.Current;

                    totalMips += mips.Value;
				}
			}
			if (mipsShare.Count > selectedPes.Count)
			{
				return false;
			}

            //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
            //FreePes.removeAll(selectedPes);
            // TEST: (fixed) RemoveAll
            FreePes.RemoveAll<Pe>(selectedPes);

            PeAllocationMap[vm.Uid] = selectedPes;
            //MipsMap.put(vm.Uid, mipsShare);
            MipsMap[vm.Uid] = mipsShare;
            AvailableMips = AvailableMips - totalMips;
			return true;
		}

		public override void deallocatePesForVm(Vm vm)
		{
			((List<Pe>)FreePes).AddRange(PeAllocationMap[vm.Uid]);
			PeAllocationMap.Remove(vm.Uid);

			double totalMips = 0;
			foreach (double mips in MipsMap[vm.Uid])
			{
				totalMips += mips;
			}
			AvailableMips = AvailableMips + totalMips;

			MipsMap.Remove(vm.Uid);
		}

		/// <summary>
		/// Sets the pe allocation map.
		/// </summary>
		/// <param name="peAllocationMap"> the pe allocation map </param>
		protected internal virtual IDictionary<string, IList<Pe>> PeAllocationMap
		{
			set
			{
				this.peAllocationMap = value;
			}
			get
			{
				return peAllocationMap;
			}
		}


		/// <summary>
		/// Sets the free pes list.
		/// </summary>
		/// <param name="freePes"> the new free pes list </param>
		protected internal virtual IList<Pe> FreePes
		{
			set
			{
				this.freePes = value;
			}
			get
			{
				return freePes;
			}
		}


	}

}