using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.lists
{


	/// <summary>
	/// HostList is a collection of operations on lists of hosts (PMs).
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public class HostList
	{

        /// <summary>
        /// Gets a <seealso cref="Host"/> with a given id.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <param name="hostList"> the list of existing hosts </param>
        /// <param name="id"> the host ID </param>
        /// <returns> a Host with the given ID or $null if not found
        /// 
        /// @pre id >= 0
        /// @post $none </returns>
        //public static T getById<T>(IList<T> hostList, int id) where T : org.cloudbus.cloudsim.Host
        public static Host getById(IList<Host> hostList, int id)
        {
			foreach (var host in hostList)
			{
				if (host.Id == id)
				{
					return host;
				}
			}
			return null;
		}

        /// <summary>
        /// Gets the total number of PEs for all Hosts.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <param name="hostList"> the list of existing hosts </param>
        /// <returns> total number of PEs for all PMs
        /// @pre $none
        /// @post $result >= 0 </returns>
        //public static int getNumberOfPes<T>(IList<T> hostList) where T : org.cloudbus.cloudsim.Host
        public static int getNumberOfPes(IList<Host> hostList)
        {
			int numberOfPes = 0;
			foreach (var host in hostList)
			{
				numberOfPes += host.PeListProperty.Count; // host.PeList.size();
			}
			return numberOfPes;
		}

        /// <summary>
        /// Gets the total number of <tt>FREE</tt> (non-busy) PEs for all Hosts.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <param name="hostList"> the list of existing hosts </param>
        /// <returns> total number of free PEs
        /// @pre $none
        /// @post $result >= 0 </returns>
        //public static int getNumberOfFreePes<T>(IList<T> hostList) where T : org.cloudbus.cloudsim.Host
        public static int getNumberOfFreePes(IList<Host> hostList)
        {
			int numberOfFreePes = 0;
			foreach (var host in hostList)
			{
				numberOfFreePes += PeList.getNumberOfFreePes(host.PeListProperty);
			}
			return numberOfFreePes;
		}

        /// <summary>
        /// Gets the total number of <tt>BUSY</tt> PEs for all Hosts.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <param name="hostList"> the list of existing hosts </param>
        /// <returns> total number of busy PEs
        /// @pre $none
        /// @post $result >= 0 </returns>
        //public static int getNumberOfBusyPes<T>(IList<T> hostList) where T : org.cloudbus.cloudsim.Host
        public static int getNumberOfBusyPes(IList<Host> hostList)
        {
			int numberOfBusyPes = 0;
			foreach (var host in hostList)
			{
				numberOfBusyPes += PeList.getNumberOfBusyPes(host.PeListProperty);
			}
			return numberOfBusyPes;
		}

        /// <summary>
        /// Gets the first host with free PEs.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <param name="hostList"> the list of existing hosts </param>
        /// <returns> a Host object or <tt>null</tt> if not found
        /// @pre $none
        /// @post $none </returns>
        //public static T getHostWithFreePe<T>(IList<T> hostList) where T : org.cloudbus.cloudsim.Host
        public static Host getHostWithFreePe(IList<Host> hostList)
        {
			return getHostWithFreePe(hostList, 1);
		}

        /// <summary>
        /// Gets the first Host with a specified number of free PEs.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <param name="hostList"> the list of existing hosts </param>
        /// <param name="pesNumber"> the pes number </param>
        /// <returns> a Host object or <tt>null</tt> if not found
        /// @pre $none
        /// @post $none </returns>
        //public static T getHostWithFreePe<T>(IList<T> hostList, int pesNumber) where T : org.cloudbus.cloudsim.Host
        public static Host getHostWithFreePe(IList<Host> hostList, int pesNumber)
        {
			foreach (var host in hostList)
			{
				if (PeList.getNumberOfFreePes(host.PeListProperty) >= pesNumber)
				{
					return host;
				}
			}
			return null;
		}

        /// <summary>
        /// Sets the status of a particular PE on a given Host.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <param name="hostList"> the list of existing hosts </param>
        /// <param name="status"> the PE status, either <tt>Pe.FREE</tt> or <tt>Pe.BUSY</tt> </param>
        /// <param name="hostId"> the host id </param>
        /// <param name="peId"> the id of the PE to set the status </param>
        /// <returns> <tt>true</tt> if the PE status has changed, <tt>false</tt> otherwise (host id or
        ///         PE id might not be exist)
        /// @pre hostId >= 0
        /// @pre peId >= 0
        /// @post $none </returns>
        //public static bool setPeStatus<T>(IList<T> hostList, int status, int hostId, int peId) where T : org.cloudbus.cloudsim.Host
        public static bool setPeStatus(IList<Host> hostList, int status, int hostId, int peId)
        {
			var host = getById(hostList, hostId);
			if (host == null)
			{
				return false;
			}
			return host.setPeStatus(peId, status);
		}
	}
}