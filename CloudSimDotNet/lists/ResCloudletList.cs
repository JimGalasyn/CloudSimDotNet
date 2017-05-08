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
	/// ResCloudletList is a collection of operations on lists of ResCloudlets.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public class ResCloudletList
	{

        /// <summary>
        /// Gets a <seealso cref="ResCloudlet"/> with a given id and owned by a given user.
        /// This method needs a combination of Cloudlet Id and User Id because
        /// each Cloud User might have exactly the same Cloudlet Id.
        /// </summary>
        /// <param name="cloudletId"> a Cloudlet Id </param>
        /// <param name="userId"> an User Id </param>
        /// <param name="list"> the list of ResCloudlet </param>
        /// <returns> a Cloudlet or null if not found
        /// @pre cloudletId >= 0
        /// @pre userId >= 0
        /// @post $none
        /// 
        /// @todo The second phrase of the class documentation is not clear.  </returns>
        //public static ResCloudlet getByIdAndUserId<T>(IList<T> list, int cloudletId, int userId) where T : org.cloudbus.cloudsim.ResCloudlet
        public static ResCloudlet getByIdAndUserId(IList<ResCloudlet> list, int cloudletId, int userId)
        {
			foreach (var rcl in list)
			{
				if (rcl.CloudletId == cloudletId && rcl.UserId == userId)
				{
					return rcl;
				}
			}
			return null;
		}

		/// <summary>
		/// Finds the index of a ResCloudlet inside a list. 
		/// This method needs a combination of Cloudlet Id
		/// and User Id because each Cloud User might have exactly the same Cloudlet Id.
		/// </summary>
		/// <param name="cloudletId"> a Cloudlet Id </param>
		/// <param name="userId"> an User Id </param>
		/// <param name="list"> the list of ResCloudlets </param>
		/// <returns> the index in this list of the first occurrence of the specified Cloudlet, or
		///         <code>-1</code> if the list does not contain the Cloudlet.
		/// @pre cloudletId >= 0
		/// @pre userId >= 0
		/// @post $none </returns>
		public static int indexOf(IList<ResCloudlet> list, int cloudletId, int userId)
		{
			int i = 0;
			foreach (var rcl in list)
			{
				if (rcl.CloudletId == cloudletId && rcl.UserId == userId)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

        /// <summary>
        /// Moves a ResCloudlet object from a list to another.
        /// </summary>
        /// <param name="listFrom"> the source list </param>
        /// <param name="listTo"> the destination list </param>
        /// <param name="cloudlet"> the cloudlet to be moved from the source to the destination list </param>
        /// <returns> <b>true</b> if the moving operation successful, <b>false</b> otherwise
        /// @pre obj != null
        /// @pre list != null
        /// @post $result == true || $result == false </returns>
        //public static bool move<T>(IList<T> listFrom, IList<T> listTo, T cloudlet) where T : org.cloudbus.cloudsim.ResCloudlet
        public static bool move(IList<ResCloudlet> listFrom, IList<ResCloudlet> listTo, ResCloudlet cloudlet)
        {
			if (listFrom.Remove(cloudlet))
			{
				listTo.Add(cloudlet);
				return true;
			}
			return false;
		}

        /// <summary>
        /// Gets the position of a ResCloudlet with a given id.
        /// </summary>
        /// <param name="cloudletList"> the list of cloudlets. </param>
        /// <param name="id"> the cloudlet id </param>
        /// <returns> the position of the cloudlet with that id, or -1 if not found. </returns>
        //public static int getPositionById<T>(IList<T> cloudletList, int id) where T : org.cloudbus.cloudsim.ResCloudlet
        public static int getPositionById(IList<ResCloudlet> cloudletList, int id)
        {
			int i = 0;
				foreach (var cloudlet in cloudletList)
				{
				if (cloudlet.CloudletId == id)
				{
					return i;
				}
				i++;
				}
			return -1;
		}
	}

}