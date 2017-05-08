using System.Collections.Generic;
using System.Linq;

namespace org.cloudbus.cloudsim.container.lists
{

    using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;


    /// <summary>
    /// Created by sareh on 28/07/15.
    /// </summary>
    public class PowerContainerVmList : ContainerVmList
    {

        /// <summary>
        /// Sort by cpu utilization.
        /// </summary>
        /// <param name="vmList"> the vm list </param>
        //public static void sortByCpuUtilization<T>(IList<T> vmList) where T : org.cloudbus.cloudsim.container.core.ContainerVm
        public static void sortByCpuUtilization(IList<ContainerVm> vmList)
        {
            //vmList.Sort(new ComparatorAnonymousInnerClass());
            // TEST: (fixed) LINQ sort
            var comparer = new ComparatorAnonymousInnerClass();
            var sortedVmList = vmList.OrderBy(c => c, comparer).ToList();

            // TODO: return parameter
            vmList = sortedVmList;
        }

        private class ComparatorAnonymousInnerClass : IComparer<ContainerVm>
        {
            public ComparatorAnonymousInnerClass()
            {
            }

            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public int compare(T a, T b) throws ClassCastException
            public virtual int Compare(ContainerVm a, ContainerVm b)
            {
                double? aUtilization = a.getTotalUtilizationOfCpuMips(CloudSim.clock());
                double? bUtilization = b.getTotalUtilizationOfCpuMips(CloudSim.clock());
                return bUtilization.Value.CompareTo(aUtilization.Value);
            }
        }
    }
}