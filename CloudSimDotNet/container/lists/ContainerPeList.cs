using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.lists
{

	using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
	using Container = org.cloudbus.cloudsim.container.core.Container;

	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerPeList
	{


        /// <summary>
        /// Gets MIPS Rating for a specified Pe ID.
        /// </summary>
        /// <param name="id"> the Pe ID </param>
        /// <param name="peList"> the pe list </param>
        /// <returns> the MIPS rating if exists, otherwise returns -1
        /// @pre id >= 0
        /// @post $none </returns>
        //public static ContainerPe getById<T>(IList<T> peList, int id) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static ContainerPe getById(IList<ContainerPe> peList, int id)
        {
			foreach (ContainerPe pe in peList)
			{
				if (pe.Id == id)
				{
					return pe;
				}
			}
			return null;
		}

        /// <summary>
        /// Gets MIPS Rating for a specified Pe ID.
        /// </summary>
        /// <param name="id"> the Pe ID </param>
        /// <param name="peList"> the pe list </param>
        /// <returns> the MIPS rating if exists, otherwise returns -1
        /// @pre id >= 0
        /// @post $none </returns>
        //public static int getMips<T>(IList<T> peList, int id) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static int getMips(IList<ContainerPe> peList, int id)
        {
			ContainerPe pe = getById(peList, id);
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
        //public static int getTotalMips<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static int getTotalMips(IList<ContainerPe> peList)
        {
			int totalMips = 0;
			foreach (ContainerPe pe in peList)
			{
				totalMips += pe.Mips;
			}
			return totalMips;
		}

        /// <summary>
        /// Gets the max utilization among by all PEs.
        /// </summary>
        /// <param name="peList"> the pe list </param>
        /// <returns> the utilization </returns>
        //public static double getMaxUtilization<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static double getMaxUtilization(IList<ContainerPe> peList)
        {
			double maxUtilization = 0;
			foreach (ContainerPe pe in peList)
			{
				double utilization = pe.ContainerPeProvisionerProperty.Utilization;
				if (utilization > maxUtilization)
				{
					maxUtilization = utilization;
				}
			}
			return maxUtilization;
		}

        /// <summary>
        /// Gets the max utilization among by all PEs allocated to the VM.
        /// </summary>
        /// <param name="container"> the container </param>
        /// <param name="peList"> the pe list </param>
        /// <returns> the utilization </returns>
        //public static double getMaxUtilizationAmongVmsPes<T>(IList<T> peList, Container container) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static double getMaxUtilizationAmongVmsPes(IList<ContainerPe> peList, Container container)
        {
			double maxUtilization = 0;
			foreach (ContainerPe pe in peList)
			{
				if (pe.ContainerPeProvisionerProperty.getAllocatedMipsForContainer(container) == null)
				{
					continue;
				}
				double utilization = pe.ContainerPeProvisionerProperty.Utilization;
				if (utilization > maxUtilization)
				{
					maxUtilization = utilization;
				}
			}
			return maxUtilization;
		}

        /// <summary>
        /// Gets a Pe ID which is FREE.
        /// </summary>
        /// <param name="peList"> the pe list </param>
        /// <returns> a Pe ID if it is FREE, otherwise returns -1
        /// @pre $none
        /// @post $none </returns>
        //public static ContainerPe getFreePe<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static ContainerPe getFreePe(IList<ContainerPe> peList)
        {
			foreach (ContainerPe pe in peList)
			{
				if (pe.Status == ContainerPe.FREE)
				{
					return pe;
				}
			}
			return null;
		}

        /// <summary>
        /// Gets the number of <tt>FREE</tt> or non-busy Pe.
        /// </summary>
        /// <param name="peList"> the pe list </param>
        /// <returns> number of Pe
        /// @pre $none
        /// @post $result >= 0 </returns>
        //public static int getNumberOfFreePes<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static int getNumberOfFreePes(IList<ContainerPe> peList)
        {
			int cnt = 0;
			foreach (ContainerPe pe in peList)
			{
				if (pe.Status == ContainerPe.FREE)
				{
					cnt++;
				}
			}
			return cnt;
		}

        /// <summary>
        /// Sets the Pe status.
        /// </summary>
        /// <param name="status"> Pe status, either <tt>Pe.FREE</tt> or <tt>Pe.BUSY</tt> </param>
        /// <param name="id"> the id </param>
        /// <param name="peList"> the pe list </param>
        /// <returns> <tt>true</tt> if the Pe status has been changed, <tt>false</tt> otherwise (Pe id might
        ///         not be exist)
        /// @pre peID >= 0
        /// @post $none </returns>
        //public static bool setPeStatus<T>(IList<T> peList, int id, int status) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static bool setPeStatus(IList<ContainerPe> peList, int id, int status)
        {
			ContainerPe pe = getById(peList, id);
			if (pe != null)
			{
				pe.Status = status;
				return true;
			}
			return false;
		}

        /// <summary>
        /// Gets the number of <tt>BUSY</tt> Pe.
        /// </summary>
        /// <param name="peList"> the pe list </param>
        /// <returns> number of Pe
        /// @pre $none
        /// @post $result >= 0 </returns>
        //public static int getNumberOfBusyPes<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static int getNumberOfBusyPes(IList<ContainerPe> peList)
        {
			int cnt = 0;
			foreach (ContainerPe pe in peList)
			{
				if (pe.Status == ContainerPe.BUSY)
				{
					cnt++;
				}
			}
			return cnt;
		}

        /// <summary>
        /// Sets the status of PEs of this machine to FAILED. NOTE: <tt>resName</tt> and
        /// <tt>machineID</tt> are used for debugging purposes, which is <b>ON</b> by default. Use
        /// 
        /// </summary>
        /// <param name="resName"> the name of the resource </param>
        /// <param name="hostId"> the id of this machine </param>
        /// <param name="failed"> the new value for the "failed" parameter </param>
        //public static void setStatusFailed<T>(IList<T> peList, string resName, int hostId, bool failed) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static void setStatusFailed(IList<ContainerPe> peList, string resName, int hostId, bool failed)
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
        /// Sets the status of PEs of this machine to FAILED.
        /// </summary>
        /// <param name="failed"> the new value for the "failed" parameter </param>
        /// <param name="peList"> the pe list </param>
        //public static void setStatusFailed<T>(IList<T> peList, bool failed) where T : org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe
        public static void setStatusFailed(IList<ContainerPe> peList, bool failed)
        {
			// a loop to set the status of all the PEs in this machine
			foreach (ContainerPe pe in peList)
			{
				if (failed)
				{
					pe.Status = ContainerPe.FAILED;
				}
				else
				{
					pe.Status = ContainerPe.FREE;
				}
			}
		}

	}
}