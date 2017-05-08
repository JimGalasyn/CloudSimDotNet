using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.container.resourceAllocators
{


	using Container = org.cloudbus.cloudsim.container.core.Container;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;

	/// <summary>
	/// ContainerAllocationPolicy is an abstract class that represents the provisioning policy of vms to
	/// ContainerContainerGoogle in a Datacentre.
	/// 
	/// @author Sareh Fotuhi Piraghaj
	/// @since CloudSim Toolkit 3.0
	/// </summary>


	public abstract class ContainerAllocationPolicy
	{
			/// <summary>
			/// The Vm list.
			/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends org.cloudbus.cloudsim.container.core.ContainerVm> containerVmList;
			private IList<ContainerVm> containerVmList;

			/// <summary>
			/// Allocates a new VmAllocationPolicy object.
			/// 
			/// @pre $none
			/// @post $none
			/// </summary>
			public ContainerAllocationPolicy()
			{
				ContainerVmList = new List<ContainerVm>();
			}

			/// <summary>
			/// Allocates a host for a given VM. The host to be allocated is the one that was already
			/// reserved.
			/// </summary>
			/// <param name="container"> virtual machine which the host is reserved to </param>
			/// <returns> $true if the host could be allocated; $false otherwise
			/// @pre $none
			/// @post $none </returns>
			public abstract bool allocateVmForContainer(Container container, IList<ContainerVm> containerVmList);

			/// <summary>
			/// Allocates a specified host for a given VM.
			/// </summary>
			/// <param name="vm"> virtual machine which the host is reserved to </param>
			/// <returns> $true if the host could be allocated; $false otherwise
			/// @pre $none
			/// @post $none </returns>
			public abstract bool allocateVmForContainer(Container container, ContainerVm vm);

			/// <summary>
			/// Optimize allocation of the VMs according to current utilization.
			/// </summary>
			/// //     * <param name="vmList">           the vm list </param>
			/// //     * <param name="utilizationBound"> the utilization bound </param>
			/// //     * <param name="time">             the time </param>
			/// <returns> the array list< hash map< string, object>> </returns>
			public abstract IList<IDictionary<string, object>> optimizeAllocation(IList<Container> containerList);

			/// <summary>
			/// Releases the host used by a VM.
			/// </summary>
			/// <param name="container"> the container
			/// @pre $none
			/// @post $none </param>
			public abstract void deallocateVmForContainer(Container container);

			/// <summary>
			/// Get the host that is executing the given VM belonging to the given user.
			/// </summary>
			/// <param name="container"> the container </param>
			/// <returns> the Host with the given vmID and userID; $null if not found
			/// @pre $none
			/// @post $none </returns>
			public abstract ContainerVm getContainerVm(Container container);

			/// <summary>
			/// Get the host that is executing the given VM belonging to the given user.
			/// </summary>
			/// <param name="containerId">   the vm id </param>
			/// <param name="userId"> the user id </param>
			/// <returns> the Host with the given vmID and userID; $null if not found
			/// @pre $none
			/// @post $none </returns>
			public abstract ContainerVm getContainerVm(int containerId, int userId);

			/// <summary>
			/// Sets the host list.
			/// </summary>
			/// <param name="containerVmList"> the new host list </param>
			protected internal virtual IList<ContainerVm> ContainerVmList 
			{
				set
				{
					this.containerVmList = value;
				}
				get
				{
					return (IList<ContainerVm>) this.containerVmList;
				}
			}


	}




}