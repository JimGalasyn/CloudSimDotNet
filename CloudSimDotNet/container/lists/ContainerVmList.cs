using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.lists
{

	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;

	/// <summary>
	/// Created by sareh on 15/07/15.
	/// </summary>
	public class ContainerVmList
	{

        //public static T getById<T>(IList<T> vmList, int id) where T : org.cloudbus.cloudsim.container.core.ContainerVm
        public static ContainerVm getById(IList<ContainerVm> vmList, int id)
        {
			foreach (var vm in vmList)
			{
				if (vm.Id == id)
				{
					return vm;
				}
			}
			return null;
		}

        /// <summary>
        /// Return a reference to a Vm object from its ID and user ID.
        /// </summary>
        /// <param name="id">     ID of required VM </param>
        /// <param name="userId"> the user ID </param>
        /// <param name="vmList"> the vm list </param>
        /// <returns> Vm with the given ID, $null if not found
        /// @pre $none
        /// @post $none </returns>
        //public static T getByIdAndUserId<T>(IList<T> vmList, int id, int userId) where T : org.cloudbus.cloudsim.container.core.ContainerVm
        public static ContainerVm getByIdAndUserId(IList<ContainerVm> vmList, int id, int userId)
        {
			foreach (var vm in vmList)
			{
				if (vm.Id == id && vm.UserId == userId)
				{
					return vm;
				}
			}
			return null;
		}

	}


}