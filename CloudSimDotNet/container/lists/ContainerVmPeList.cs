using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.lists
{

	using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;

	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public class ContainerVmPeList
	{


        /// <summary>
        /// Gets MIPS Rating for a specified Pe ID.
        /// </summary>
        /// <param name="id"> the Pe ID </param>
        /// <param name="peList"> the pe list </param>
        /// <returns> the MIPS rating if exists, otherwise returns -1
        /// @pre id >= 0
        /// @post $none </returns>
        //public static ContainerVmPe getById<T>(IList<T> peList, int id) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static ContainerVmPe getById(IList<ContainerVmPe> peList, int id)
        {
			foreach (ContainerVmPe pe in peList)
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
        //public static int getMips<T>(IList<T> peList, int id) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static int getMips(IList<ContainerVmPe> peList, int id)
        {
			ContainerVmPe pe = getById(peList, id);
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
        //public static int getTotalMips<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static int getTotalMips(IList<ContainerVmPe> peList)
        {
			int totalMips = 0;
			foreach (ContainerVmPe pe in peList)
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
        //public static double getMaxUtilization<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static double getMaxUtilization(IList<ContainerVmPe> peList)
        {
			double maxUtilization = 0;
			foreach (ContainerVmPe pe in peList)
			{
				double utilization = pe.ContainerVmPeProvisioner.Utilization;
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
        /// <param name="vm"> the vm </param>
        /// <param name="peList"> the pe list </param>
        /// <returns> the utilization </returns>
        //public static double getMaxUtilizationAmongVmsPes<T>(IList<T> peList, ContainerVm vm) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static double getMaxUtilizationAmongVmsPes(IList<ContainerVmPe> peList, ContainerVm vm)
        {
			double maxUtilization = 0;
			foreach (ContainerVmPe pe in peList)
			{
				if (pe.ContainerVmPeProvisioner.getAllocatedMipsForContainerVm(vm) == null)
				{
					continue;
				}
				double utilization = pe.ContainerVmPeProvisioner.Utilization;
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
        //public static ContainerVmPe getFreePe<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static ContainerVmPe getFreePe(IList<ContainerVmPe> peList)
        {
			foreach (ContainerVmPe pe in peList)
			{
				if (pe.Status == Pe.FREE)
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
        //public static int getNumberOfFreePes<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static int getNumberOfFreePes(IList<ContainerVmPe> peList)
        {
			int cnt = 0;
			foreach (ContainerVmPe pe in peList)
			{
				if (pe.Status == Pe.FREE)
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
        //public static bool setPeStatus<T>(IList<T> peList, int id, int status) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static bool setPeStatus(IList<ContainerVmPe> peList, int id, int status)
        {
			ContainerVmPe pe = getById(peList, id);
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
        //public static int getNumberOfBusyPes<T>(IList<T> peList) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static int getNumberOfBusyPes(IList<ContainerVmPe> peList)
        {
			int cnt = 0;
			foreach (ContainerVmPe pe in peList)
			{
				if (pe.Status == Pe.BUSY)
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
        //public static void setStatusFailed<T>(IList<T> peList, string resName, int hostId, bool failed) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static void setStatusFailed(IList<ContainerVmPe> peList, string resName, int hostId, bool failed)
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
        //public static void setStatusFailed<T>(IList<T> peList, bool failed) where T : org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe
        public static void setStatusFailed(IList<ContainerVmPe> peList, bool failed)
        {
			// a loop to set the status of all the PEs in this machine
			foreach (ContainerVmPe pe in peList)
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