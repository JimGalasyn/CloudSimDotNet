using System.Collections.Generic;
using System.Linq;

namespace org.cloudbus.cloudsim.container.core
{


	using ContainerVmPeList = org.cloudbus.cloudsim.container.lists.ContainerVmPeList;


	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerHostList
	{

		/// <summary>
		/// Gets the Machine object for a particular ID.
		/// </summary>
		/// @param <T>      the generic type </param>
		/// <param name="hostList"> the host list </param>
		/// <param name="id">       the host ID </param>
		/// <returns> the Machine object or <tt>null</tt> if no machine exists
		/// @pre id >= 0
		/// @post $none
		/// @see </returns>
		public static T getById<T>(IList<T> hostList, int id) where T : ContainerHost
		{
			foreach (T host in hostList)
			{
				if (host.Id == id)
				{
					return host;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the total number of PEs for all Machines.
		/// </summary>
		/// @param <T>      the generic type </param>
		/// <param name="hostList"> the host list </param>
		/// <returns> number of PEs
		/// @pre $none
		/// @post $result >= 0 </returns>
		public static int getNumberOfPes<T>(IList<T> hostList) where T : ContainerHost
		{
			int numberOfPes = 0;
			foreach (T host in hostList)
			{
				numberOfPes += host.PeListProperty.Count;
			}
			return numberOfPes;
		}

		/// <summary>
		/// Gets the total number of <tt>FREE</tt> or non-busy PEs for all Machines.
		/// </summary>
		/// @param <T>      the generic type </param>
		/// <param name="hostList"> the host list </param>
		/// <returns> number of PEs
		/// @pre $none
		/// @post $result >= 0 </returns>
		public static int getNumberOfFreePes<T>(IList<T> hostList) where T : ContainerHost
		{
			int numberOfFreePes = 0;
			foreach (T host in hostList)
			{
				numberOfFreePes += ContainerVmPeList.getNumberOfFreePes(host.PeListProperty);
			}
			return numberOfFreePes;
		}

		/// <summary>
		/// Gets the total number of <tt>BUSY</tt> PEs for all Machines.
		/// </summary>
		/// @param <T>      the generic type </param>
		/// <param name="hostList"> the host list </param>
		/// <returns> number of PEs
		/// @pre $none
		/// @post $result >= 0 </returns>
		public static int getNumberOfBusyPes<T>(IList<T> hostList) where T : ContainerHost
		{
			int numberOfBusyPes = 0;
			foreach (T host in hostList)
			{
				numberOfBusyPes += ContainerVmPeList.getNumberOfBusyPes(host.PeListProperty);
			}
			return numberOfBusyPes;
		}

		/// <summary>
		/// Gets a Machine with free Pe.
		/// </summary>
		/// @param <T>      the generic type </param>
		/// <param name="hostList"> the host list </param>
		/// <returns> a machine object or <tt>null</tt> if not found
		/// @pre $none
		/// @post $none </returns>
		public static T getHostWithFreePe<T>(IList<T> hostList) where T : ContainerHost
		{
			return getHostWithFreePe(hostList, 1);
		}

		/// <summary>
		/// Gets a Machine with a specified number of free Pe.
		/// </summary>
		/// @param <T>       the generic type </param>
		/// <param name="hostList">  the host list </param>
		/// <param name="pesNumber"> the pes number </param>
		/// <returns> a machine object or <tt>null</tt> if not found
		/// @pre $none
		/// @post $none </returns>
		public static T getHostWithFreePe<T>(IList<T> hostList, int pesNumber) where T : ContainerHost
		{
			foreach (T host in hostList)
			{
				if (ContainerVmPeList.getNumberOfFreePes(host.PeListProperty) >= pesNumber)
				{
					return host;
				}
			}
			return null;
		}

		/// <summary>
		/// Sets the particular Pe status on a Machine.
		/// </summary>
		/// @param <T>      the generic type </param>
		/// <param name="hostList"> the host list </param>
		/// <param name="status">   Pe status, either <tt>Pe.FREE</tt> or <tt>Pe.BUSY</tt> </param>
		/// <param name="hostId">   the host id </param>
		/// <param name="peId">     the pe id </param>
		/// <returns> <tt>true</tt> if the Pe status has changed, <tt>false</tt> otherwise (Machine id or
		/// Pe id might not be exist)
		/// @pre machineID >= 0
		/// @pre peID >= 0
		/// @post $none </returns>
		public static bool setPeStatus<T>(IList<T> hostList, int status, int hostId, int peId) where T : ContainerHost
		{
			T host = getById(hostList, hostId);
			if (host == null)
			{
				return false;
			}
			return host.setPeStatus(peId, status);
		}

		/// <summary>
		/// Sort by cpu utilization.
		/// </summary>
		/// <param name="hostList"> the vm list </param>
		public static void sortByCpuUtilization(IList<ContainerHost> hostList)
		{   
            // TEST: (fixed) LINQ sort
            //hostList.Sort(new ComparatorAnonymousInnerClass());
            var comparer = new ComparatorAnonymousInnerClass();
            var sortedHostList = hostList.OrderBy(h => h, comparer).ToList();
            hostList = sortedHostList;
        }

		private class ComparatorAnonymousInnerClass : IComparer<ContainerHost>
		{
			public ComparatorAnonymousInnerClass()
			{
			}

			public virtual int Compare(ContainerHost a, ContainerHost b)
			{
				double? aUtilization = ((PowerContainerHost) a).UtilizationOfCpu;
				double? bUtilization = ((PowerContainerHost) b).UtilizationOfCpu;
				return bUtilization.Value.CompareTo(aUtilization.Value);
			}
		}

		public static void sortByCpuUtilizationDescending(IList<ContainerHost> hostList)
		{
            //hostList.Sort(Collections.reverseOrder(new ComparatorAnonymousInnerClass2()));
            // TEST: (fixed) LINQ sort
            //hostList.Sort(new ComparatorAnonymousInnerClass());
            var comparer = new ComparatorAnonymousInnerClass2();
            var sortedHostList = hostList.OrderByDescending(h => h, comparer).ToList();
            hostList = sortedHostList;
        }

        private class ComparatorAnonymousInnerClass2 : IComparer<ContainerHost>
		{
			public ComparatorAnonymousInnerClass2()
			{
			}

			public virtual int Compare(ContainerHost a, ContainerHost b)
			{
				double? aUtilization = ((PowerContainerHost) a).UtilizationOfCpu;
				double? bUtilization = ((PowerContainerHost) b).UtilizationOfCpu;
				return bUtilization.Value.CompareTo(aUtilization.Value);
			}
		}
	}
}