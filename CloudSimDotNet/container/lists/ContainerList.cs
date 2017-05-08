using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.lists
{

	using Container = org.cloudbus.cloudsim.container.core.Container;

	/// <summary>
	/// Created by sareh on 17/07/15.
	/// </summary>
	public class ContainerList
	{
        //public static T getById<T>(IList<T> containerList, int id) where T : org.cloudbus.cloudsim.container.core.Container
        public static Container getById(IList<Container> containerList, int id)
        {
			foreach (var container in containerList)
			{
				if (container.Id == id)
				{
					return container;
				}
			}
			return null;
		}

        /// <summary>
        /// Return a reference to a Vm object from its ID and user ID.
        /// </summary>
        /// <param name="id">            ID of required VM </param>
        /// <param name="userId">        the user ID </param>
        /// <param name="containerList"> the vm list </param>
        /// <returns> Vm with the given ID, $null if not found
        /// @pre $none
        /// @post $none </returns>
        //public static T getByIdAndUserId<T>(IList<T> containerList, int id, int userId) where T : org.cloudbus.cloudsim.container.core.Container
        public static Container getByIdAndUserId(IList<Container> containerList, int id, int userId)
        {
			foreach (var container in containerList)
			{
				if (container.Id == id && container.UserId == userId)
				{
					return container;
				}
			}
			return null;
		}


	}

}