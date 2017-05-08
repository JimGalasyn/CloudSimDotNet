using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled
{

	using PowerContainerSelectionPolicy = org.cloudbus.cloudsim.container.containerSelectionPolicies.PowerContainerSelectionPolicy;
	using ContainerDatacenter = org.cloudbus.cloudsim.container.core.ContainerDatacenter;
	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;
	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
	using PowerContainerHost = org.cloudbus.cloudsim.container.core.PowerContainerHost;
	using HostSelectionPolicy = org.cloudbus.cloudsim.container.hostSelectionPolicies.HostSelectionPolicy;
	using PowerContainerVmSelectionPolicy = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicy;

	/// <summary>
	/// Created by sareh on 18/08/15.
	/// </summary>
	public class PowerContainerVmAllocationPolicyMigrationStaticThresholdMCUnderUtilized : PowerContainerVmAllocationPolicyMigrationAbstractContainerHostSelectionUnderUtilizedAdded
	{


		/// <summary>
		/// The utilization threshold.
		/// </summary>
		private double utilizationThreshold = 0.9;

		/// <summary>
		/// Instantiates a new power vm allocation policy migration mad.
		/// </summary>
		/// <param name="hostList">             the host list </param>
		/// <param name="vmSelectionPolicy">    the vm selection policy </param>
		/// <param name="utilizationThreshold"> the utilization threshold </param>
		public PowerContainerVmAllocationPolicyMigrationStaticThresholdMCUnderUtilized(IList<ContainerHost> hostList, PowerContainerVmSelectionPolicy vmSelectionPolicy, PowerContainerSelectionPolicy containerSelectionPolicy, HostSelectionPolicy hostSelectionPolicy, double utilizationThreshold, double underUtilizationThresh, int numberOfVmTypes, int[] vmPes, float[] vmRam, long vmBw, long vmSize, double[] vmMips) : base(hostList, vmSelectionPolicy, containerSelectionPolicy, hostSelectionPolicy,underUtilizationThresh, numberOfVmTypes, vmPes, vmRam, vmBw, vmSize, vmMips)
		{
			UtilizationThreshold = utilizationThreshold;
		}

		/// <summary>
		/// Checks if is host over utilized.
		/// </summary>
		/// <param name="host"> the _host </param>
		/// <returns> true, if is host over utilized </returns>
		protected internal override bool isHostOverUtilized(PowerContainerHost host)
		{
			addHistoryEntry(host, UtilizationThreshold);
			double totalRequestedMips = 0;
			foreach (ContainerVm vm in host.VmListProperty)
			{
				totalRequestedMips += vm.CurrentRequestedTotalMips;
			}
			double utilization = totalRequestedMips / host.TotalMips;
			return utilization > UtilizationThreshold;
		}

		protected internal override bool isHostUnderUtilized(PowerContainerHost host)
		{
			return false;
		}

		/// <summary>
		/// Sets the utilization threshold.
		/// </summary>
		/// <param name="utilizationThreshold"> the new utilization threshold </param>
		protected internal virtual double UtilizationThreshold
		{
			set
			{
				this.utilizationThreshold = value;
			}
			get
			{
				return utilizationThreshold;
			}
		}

		public override ContainerDatacenter Datacenter
		{
			set
			{
				base.Datacenter = value;
			}
		}



	}

}