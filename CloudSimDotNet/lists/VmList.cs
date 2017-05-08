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
	/// VmListProperty is a collection of operations on lists of VMs.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public class VmList
	{

        /// <summary>
        /// Gets a <seealso cref="Vm"/> with a given id.
        /// </summary>
        /// <param name="id"> ID of required VM </param>
        /// <param name="vmList"> list of existing VMs </param>
        /// <returns> a Vm with the given ID or $null if not found
        /// @pre $none
        /// @post $none
        /// 
        /// @todo It may be considered the use of a HashMap in order to improve 
        /// VM search, instead of a List. The map key can be the vm id
        /// and the value the VM itself. However, it has to be assessed
        /// the feasibility to have VMs with the same ID and the need
        /// to find VMs by its id and user id, as in the method
        /// <seealso cref="#getByIdAndUserId(java.util.List, int, int)"/>.
        /// The first concern could be dealt by ensuring that all
        /// VMs have different ID (in fact, I don't know if 
        /// VM id uniqueness is a CloudSim requirement)
        /// and creating a map by VM id.
        /// The second concern could be dealt by creating 
        /// a HashMap<UserID, List<VmIDs>>.
        /// The third concern is, that changing 
        /// the class of these lists may have a potential
        /// effect on the entire project and in the creation of simulations
        /// that has to be priorly assessed. </returns>
        //public static T getById<T>(IList<T> vmList, int id) where T : org.cloudbus.cloudsim.Vm
        public static Vm getById(IList<Vm> vmList, int id)
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
        /// Gets a <seealso cref="Vm"/> with a given id and owned by a given user.
        /// </summary>
        /// <param name="vmList"> list of existing VMs </param>
        /// <param name="id"> ID of required VM </param>
        /// <param name="userId"> the user ID of the VM's owner </param>
        /// <returns> Vm with the given ID, $null if not found
        /// @pre $none
        /// @post $none </returns>
        //public static T getByIdAndUserId<T>(IList<T> vmList, int id, int userId) where T : org.cloudbus.cloudsim.Vm
        public static Vm getByIdAndUserId(IList<Vm> vmList, int id, int userId)
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