using System.Collections.Generic;
using System.Linq;

namespace org.cloudbus.cloudsim.container.lists
{

	using Container = org.cloudbus.cloudsim.container.core.Container;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;


	/// <summary>
	/// Created by sareh on 31/07/15.
	/// </summary>
	public class PowerContainerList
	{


        /// <summary>
        /// Sort by cpu utilization.
        /// </summary>
        /// <param name="containerList"> the vm list </param>
        //public static void sortByCpuUtilization<T>(IList<T> containerList) where T : org.cloudbus.cloudsim.container.core.Container
        public static void sortByCpuUtilization(IList<Container> containerList)
        {
            //containerList.Sort(new ComparatorAnonymousInnerClass());
            // TEST: (fixed) LINQ sort
            var comparer = new ComparatorAnonymousInnerClass();
            var sortedcontainerList = containerList.OrderBy(c => c, comparer).ToList();
            containerList = sortedcontainerList;
        }

        private class ComparatorAnonymousInnerClass : IComparer<Container>
		{
			public ComparatorAnonymousInnerClass()
			{
			}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int compare(T a, T b) throws ClassCastException
			public virtual int Compare(Container a, Container b)
			{
				double? aUtilization = a.getTotalUtilizationOfCpuMips(CloudSim.clock());
				double? bUtilization = b.getTotalUtilizationOfCpuMips(CloudSim.clock());
				return bUtilization.Value.CompareTo(aUtilization.Value);
			}
		}

	}

}