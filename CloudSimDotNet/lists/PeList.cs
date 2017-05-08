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
	/// PeList is a collection of operations on lists of PEs.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public class PeList
	{

        /// <summary>
        /// Gets a <seealso cref="Pe"/> with a given id.
        /// </summary>
        /// <param name="peList"> the PE list where to get a given PE </param>
        /// <param name="id"> the id of the PE to be get </param>
        /// <returns> the PE with the given id or null if not found
        /// @pre id >= 0
        /// @post $none </returns>
        //public static Pe getById<T>(IList<T> peList, int id) where T : org.cloudbus.cloudsim.Pe
        // TODO: Implement as Dictionary
        public static Pe getById(IList<Pe> peList, int id)
        {
			/*@todo such kind of search would be made using a HashMap
			(to avoid always iterating over the list),
			where the key is the id of the object and the value the object
			itself. The same occurs for lists of hosts and VMs.*/
			foreach (Pe pe in peList)
			{
				if (pe.Id == id)
				{
					return pe;
				}
			}
			return null;
		}

        /// <summary>
        /// Gets MIPS Rating of a PE with a given ID.
        /// </summary>
        /// <param name="peList"> the PE list where to get a given PE </param>
        /// <param name="id"> the id of the PE to be get </param>
        /// <returns> the MIPS rating of the PE or -1 if the PE was not found
        /// @pre id >= 0
        /// @post $none </returns>
        //public static int getMips<T>(IList<T> peList, int id) where T : org.cloudbus.cloudsim.Pe
        public static int getMips(IList<Pe> peList, int id)
        {
			Pe pe = getById(peList, id);
			if (pe != null)
			{
				return pe.Mips;
			}
			return -1;
		}

        /// <summary>
        /// Gets total MIPS Rating for all PEs.
        /// </summary>
        /// <param name="peList"> the pe list </param>
        /// <returns> the total MIPS Rating
        /// @pre $none
        /// @post $none </returns>
        //public static int getTotalMips<T>(IList<T> peList) where T : org.cloudbus.cloudsim.Pe
        // TODO: REplace loop with LINQ Sum
        public static int getTotalMips(IList<Pe> peList)
        {
            int totalMips = 0;
			foreach (Pe pe in peList)
			{
				totalMips += pe.Mips;
			}
			return totalMips;
		}

        /// <summary>
        /// Gets the max utilization percentage among all PEs.
        /// </summary>
        /// <param name="peList"> the pe list </param>
        /// <returns> the max utilization percentage </returns>
        //public static double getMaxUtilization<T>(IList<T> peList) where T : org.cloudbus.cloudsim.Pe
        public static double getMaxUtilization(IList<Pe> peList)
        {
			double maxUtilization = 0;
			foreach (Pe pe in peList)
			{
				double utilization = pe.PeProvisioner.Utilization;
				if (utilization > maxUtilization)
				{
					maxUtilization = utilization;
				}
			}
			return maxUtilization;
		}

        /// <summary>
        /// Gets the max utilization percentage among all PEs allocated to a VM.
        /// </summary>
        /// <param name="vm"> the vm to get the maximum utilization percentage </param>
        /// <param name="peList"> the pe list </param>
        /// <returns> the max utilization percentage </returns>
        //public static double getMaxUtilizationAmongVmsPes<T>(IList<T> peList, Vm vm) where T : org.cloudbus.cloudsim.Pe
        public static double getMaxUtilizationAmongVmsPes(IList<Pe> peList, Vm vm)
        {
			double maxUtilization = 0;
			foreach (Pe pe in peList)
			{
				if (pe.PeProvisioner.getAllocatedMipsForVm(vm) == null)
				{
					continue;
				}
				double utilization = pe.PeProvisioner.Utilization;
				if (utilization > maxUtilization)
				{
					maxUtilization = utilization;
				}
			}
			return maxUtilization;
		}

        /// <summary>
        /// Gets the first <tt>FREE</tt> PE which.
        /// </summary>
        /// <param name="peList"> the PE list </param>
        /// <returns> the first free PE or null if not found
        /// @pre $none
        /// @post $none </returns>
        //public static Pe getFreePe<T>(IList<T> peList) where T : org.cloudbus.cloudsim.Pe
        public static Pe getFreePe(IList<Pe> peList)
        {
            foreach (Pe pe in peList)
			{
				if (pe.Status == Pe.FREE)
				{
					return pe;
				}
			}
			return null;
		}

        /// <summary>
        /// Gets the number of <tt>FREE</tt> (non-busy) PEs.
        /// </summary>
        /// <param name="peList"> the PE list </param>
        /// <returns> number of free PEs
        /// @pre $none
        /// @post $result >= 0 </returns>
        //public static int getNumberOfFreePes<T>(IList<T> peList) where T : org.cloudbus.cloudsim.Pe
        public static int getNumberOfFreePes(IList<Pe> peList)
        {
			int cnt = 0;
			foreach (Pe pe in peList)
			{
				if (pe.Status == Pe.FREE)
				{
					cnt++;
				}
			}
			return cnt;
		}

        /// <summary>
        /// Sets a PE status.
        /// </summary>
        /// <param name="status"> the PE status, either <tt>Pe.FREE</tt> or <tt>Pe.BUSY</tt> </param>
        /// <param name="id"> the id of the PE to be set </param>
        /// <param name="peList"> the PE list </param>
        /// <returns> <tt>true</tt> if the PE status has been changed, <tt>false</tt> otherwise (PE id might
        ///         not be exist)
        /// @pre peID >= 0
        /// @post $none </returns>
        //public static bool setPeStatus<T>(IList<T> peList, int id, int status) where T : org.cloudbus.cloudsim.Pe
        public static bool setPeStatus(IList<Pe> peList, int id, int status)
        {
			Pe pe = getById(peList, id);
			if (pe != null)
			{
				pe.Status = status;
				return true;
			}
			return false;
		}

        /// <summary>
        /// Gets the number of <tt>BUSY</tt> PEs.
        /// </summary>
        /// <param name="peList"> the PE list </param>
        /// <returns> number of busy PEs
        /// @pre $none
        /// @post $result >= 0 </returns>
        //public static int getNumberOfBusyPes<T>(IList<T> peList) where T : org.cloudbus.cloudsim.Pe
        public static int getNumberOfBusyPes(IList<Pe> peList)
        {
			int cnt = 0;
			foreach (Pe pe in peList)
			{
				if (pe.Status == Pe.BUSY)
				{
					cnt++;
				}
			}
			return cnt;
		}

		/// <summary>
		/// Sets the status of PEs of a host to FAILED or FREE. NOTE: <tt>resName</tt> and
		/// <tt>hostId</tt> are used for debugging purposes, which is <b>ON</b> by default. 
		/// Use <seealso cref="#setStatusFailed(boolean)"/> if you do not want this information.
		/// </summary>
		/// <param name="peList"> the host's PE list to be set as failed or free </param>
		/// <param name="resName"> the name of the resource </param>
		/// <param name="hostId"> the id of the host </param>
		/// <param name="failed"> true if the host's PEs have to be set as FAILED, false
		/// if they have to be set as FREE. </param>
		/// <seealso cref= #setStatusFailed(java.util.List, boolean)  </seealso>
		//public static void setStatusFailed<T>(IList<T> peList, string resName, int hostId, bool failed) where T : org.cloudbus.cloudsim.Pe
        public static void setStatusFailed(IList<Pe> peList, string resName, int hostId, bool failed)
        {
			string status = null;
			if (failed)
			{
				status = "FAILED";
			}
			else
			{
				status = "WORKING";
			}

			Log.printConcatLine(resName, " - Machine: ", hostId, " is ", status);

			setStatusFailed(peList, failed);
		}

        /// <summary>
        /// Sets the status of PEs of a host to FAILED or FREE.
        /// </summary>
        /// <param name="peList"> the host's PE list to be set as failed or free </param>
        /// <param name="failed"> true if the host's PEs have to be set as FAILED, false
        /// if they have to be set as FREE. </param>
        //public static void setStatusFailed<T>(IList<T> peList, bool failed) where T : org.cloudbus.cloudsim.Pe
        public static void setStatusFailed(IList<Pe> peList, bool failed)
        {
			// a loop to set the status of all the PEs in this machine
			foreach (Pe pe in peList)
			{
				if (failed)
				{
					pe.Status = Pe.FAILED;
				}
				else
				{
					pe.Status = Pe.FREE;
				}
			}
		}

	}
}