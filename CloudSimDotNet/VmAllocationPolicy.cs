using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	/// <summary>
	/// VmAllocationPolicy is an abstract class that represents the provisioning policy of hosts to
	/// virtual machines in a Datacenter. It allocates hosts for placing VMs. 
	/// It supports two-stage commit of reservation of hosts: first, we
	/// reserve the host and, once committed by the user, it is effectively allocated to he/she.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public abstract class VmAllocationPolicy
	{

		/// <summary>
		/// The host list. </summary>
		private IList<Host> hostList;

		/// <summary>
		/// Creates a new VmAllocationPolicy object.
		/// </summary>
		/// <param name="list"> Machines available in a <seealso cref="Datacenter"/>
		/// @pre $none
		/// @post $none </param>
		public VmAllocationPolicy(IList<Host> list)
		{
			HostListProperty = list;
		}

		/// <summary>
		/// Allocates a host for a given VM.
		/// </summary>
		/// <param name="vm"> the VM to allocate a host to </param>
		/// <returns> $true if the host could be allocated; $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateHostForVm(Vm vm);

		/// <summary>
		/// Allocates a specified host for a given VM.
		/// </summary>
		/// <param name="vm"> virtual machine which the host is reserved to </param>
		/// <param name="host"> host to allocate the the given VM </param>
		/// <returns> $true if the host could be allocated; $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateHostForVm(Vm vm, Host host);

		/// <summary>
		/// Optimize allocation of the VMs according to current utilization.
		/// </summary>
		/// <param name="vmList"> the vm list </param>
		/// <returns> the array list< hash map< string, object>>
		/// 
		/// @todo It returns a list of maps, where each map key is a string 
		/// and stores an object. What in fact are the keys and values of this
		/// Map? Neither this class or its subclasses implement the method
		/// or have clear documentation. The only sublcass is the <seealso cref="VmAllocationPolicySimple"/>. 
		///  </returns>
		public abstract IList<IDictionary<string, object>> optimizeAllocation(IList<Vm> vmList);

		/// <summary>
		/// Releases the host used by a VM.
		/// </summary>
		/// <param name="vm"> the vm to get its host released
		/// @pre $none
		/// @post $none </param>
		public abstract void deallocateHostForVm(Vm vm);

		/// <summary>
		/// Get the host that is executing the given VM.
		/// </summary>
		/// <param name="vm"> the vm </param>
		/// <returns> the Host with the given vmID; $null if not found
		/// 
		/// @pre $none
		/// @post $none </returns>
		public abstract Host getHost(Vm vm);

		/// <summary>
		/// Get the host that is executing the given VM belonging to the given user.
		/// </summary>
		/// <param name="vmId"> the vm id </param>
		/// <param name="userId"> the user id </param>
		/// <returns> the Host with the given vmID and userID; $null if not found
		/// @pre $none
		/// @post $none </returns>
		public abstract Host getHost(int vmId, int userId);

		/// <summary>
		/// Sets the host list.
		/// </summary>
		/// <param name="hostList"> the new host list </param>
		protected internal virtual IList<Host> HostListProperty
		{
			set
			{
				this.hostList = value;
			}
			get
			{
				return (IList<Host>) hostList;
			}
		}


	}

}