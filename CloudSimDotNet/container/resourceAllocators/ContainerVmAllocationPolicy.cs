using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocators
{


	using ContainerDatacenter = org.cloudbus.cloudsim.container.core.ContainerDatacenter;
	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;


	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public abstract class ContainerVmAllocationPolicy
	{


		/// <summary>
		/// The host list.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends org.cloudbus.cloudsim.container.core.ContainerHost> containerHostList;
		private IList<ContainerHost> containerHostList;

		/// <summary>
		/// Allocates a new VmAllocationPolicy object.
		/// </summary>
		/// <param name="containerHostList"> Machines available in this Datacentre
		/// @pre $none
		/// @post $none </param>
		public ContainerVmAllocationPolicy(IList<ContainerHost> containerHostList)
		{
			ContainerHostListProperty = containerHostList;
		}

		/// <summary>
		/// Allocates a host for a given VM. The host to be allocated is the one that was already
		/// reserved.
		/// </summary>
		/// <param name="vm"> virtual machine which the host is reserved to </param>
		/// <returns> $true if the host could be allocated; $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateHostForVm(ContainerVm vm);

		/// <summary>
		/// Allocates a specified host for a given VM.
		/// </summary>
		/// <param name="vm"> virtual machine which the host is reserved to </param>
		/// <returns> $true if the host could be allocated; $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public abstract bool allocateHostForVm(ContainerVm vm, ContainerHost host);

		/// <summary>
		/// Optimize allocation of the VMs according to current utilization.
		/// <para>
		/// </para>
		/// </summary>
		/// //     * <param name="vmList">           the vm list </param>
		/// //     * <param name="utilizationBound"> the utilization bound </param>
		/// //     * <param name="time">             the time
		/// </param>
		/// <returns> the array list< hash map< string, object>> </returns>
		public abstract IList<IDictionary<string, object>> optimizeAllocation(IList<ContainerVm> vmList);

		/// <summary>
		/// Releases the host used by a VM.
		/// </summary>
		/// <param name="containerVm"> the vm
		/// @pre $none
		/// @post $none </param>
		public abstract void deallocateHostForVm(ContainerVm containerVm);

		/// <summary>
		/// Get the host that is executing the given VM belonging to the given user.
		/// </summary>
		/// <param name="containerVm"> the vm </param>
		/// <returns> the Host with the given vmID and userID; $null if not found
		/// @pre $none
		/// @post $none </returns>
		public abstract ContainerHost getHost(ContainerVm containerVm);

		/// <summary>
		/// Get the host that is executing the given VM belonging to the given user.
		/// </summary>
		/// <param name="vmId">   the vm id </param>
		/// <param name="userId"> the user id </param>
		/// <returns> the Host with the given vmID and userID; $null if not found
		/// @pre $none
		/// @post $none </returns>
		public abstract ContainerHost getHost(int vmId, int userId);

        /// <summary>
        /// Sets the host list.
        /// </summary>
        /// <param name="containerHostList"> the new host list </param>
        protected internal virtual IList<ContainerHost> ContainerHostListProperty
		{
			set
			{
				this.containerHostList = value;
			}
			get
			{
				return (IList<ContainerHost>) containerHostList;
			}
		}


		public abstract ContainerDatacenter Datacenter { get; set; }


	}




}