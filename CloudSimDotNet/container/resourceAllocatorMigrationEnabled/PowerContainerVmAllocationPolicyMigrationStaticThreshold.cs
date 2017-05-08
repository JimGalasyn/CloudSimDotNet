using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled
{
	using org.cloudbus.cloudsim.container.core;
	using PowerContainerVmSelectionPolicy = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicy;

	/// <summary>
	/// Created by sareh on 30/07/15.
	/// </summary>
	public class PowerContainerVmAllocationPolicyMigrationStaticThreshold : PowerContainerVmAllocationPolicyMigrationAbstract
	{

		/// <summary>
		/// The utilization threshold. </summary>
		private double utilizationThreshold = 0.9;

		/// <summary>
		/// Instantiates a new power vm allocation policy migration mad.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="utilizationThreshold"> the utilization threshold </param>
		public PowerContainerVmAllocationPolicyMigrationStaticThreshold(IList<ContainerHost> hostList, PowerContainerVmSelectionPolicy vmSelectionPolicy, double utilizationThreshold) : base(hostList, vmSelectionPolicy)
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

        // TEST: (fixed) Ported implementation had only empty set accessor.
        public override ContainerDatacenter Datacenter { get; set; }
	}

}