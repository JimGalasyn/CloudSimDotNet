using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power.lists
{
    using System.Linq;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using VmList = org.cloudbus.cloudsim.lists.VmList;

    /// <summary>
    /// PowerVmList is a collection of operations on lists of power-enabled VMs.
    /// 
    /// <br/>If you are using any algorithms, policies or workload included in the power package please cite
    /// the following paper:<br/>
    /// 
    /// <ul>
    /// <li><a href="http://dx.doi.org/10.1002/cpe.1867">Anton Beloglazov, and Rajkumar Buyya, "Optimal Online Deterministic Algorithms and Adaptive
    /// Heuristics for Energy and Performance Efficient Dynamic Consolidation of Virtual Machines in
    /// Cloud Data Centers", Concurrency and Computation: Practice and Experience (CCPE), Volume 24,
    /// Issue 13, Pages: 1397-1420, John Wiley & Sons, Ltd, New York, USA, 2012</a>
    /// </ul>
    /// 
    /// @author Anton Beloglazov
    /// 
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 2.0
    /// @todo It is a list, so it would be better inside the org.cloudbus.cloudsim.lists package.
    /// This class in fact doesn't use a list or PowerVm, but a list of Vm.
    /// The used methods are just of the Vm class, thus doesn't have
    /// a reason to create another class. This classes don't either stores lists of VM,
    /// they only perform operations on lists given by parameter.
    /// So, the method of this class would be moved to the VmListProperty class
    /// and the class erased.
    /// </summary>
    public class PowerVmList : VmList
	{

		/// <summary>
		/// Sort a given list of VMs by cpu utilization.
		/// </summary>
		/// <param name="vmList"> the vm list to be sorted </param>
		public static void sortByCpuUtilization(IList<Vm> vmList)
		{
            //vmList.Sort(new ComparatorAnonymousInnerClass());
            // TEST: (fixed) LINQ sort
            var comparer = new ComparatorAnonymousInnerClass();
            var sortedVmList = vmList.OrderBy(c => c, comparer).ToList();

            // TODO: return parameter
        }

        private class ComparatorAnonymousInnerClass : IComparer<Vm>
		{
			public ComparatorAnonymousInnerClass()
			{
			}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int compare(T a, T b) throws ClassCastException
			public virtual int Compare(Vm a, Vm b)
			{
				double? aUtilization = a.getTotalUtilizationOfCpuMips(CloudSim.clock());
				double? bUtilization = b.getTotalUtilizationOfCpuMips(CloudSim.clock());
				return bUtilization.Value.CompareTo(aUtilization.Value);
			}
		}

	}

}