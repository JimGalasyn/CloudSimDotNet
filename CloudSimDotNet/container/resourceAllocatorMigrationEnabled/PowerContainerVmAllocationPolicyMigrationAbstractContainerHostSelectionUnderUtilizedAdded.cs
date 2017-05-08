using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled
{

	using ContainerHost = org.cloudbus.cloudsim.container.core.ContainerHost;
	using ContainerHostList = org.cloudbus.cloudsim.container.core.ContainerHostList;
	using PowerContainerHost = org.cloudbus.cloudsim.container.core.PowerContainerHost;
	using PowerContainerSelectionPolicy = org.cloudbus.cloudsim.container.containerSelectionPolicies.PowerContainerSelectionPolicy;
	using HostSelectionPolicy = org.cloudbus.cloudsim.container.hostSelectionPolicies.HostSelectionPolicy;
	using PowerContainerVmSelectionPolicy = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicy;

	/// <summary>
	/// Created by sareh on 13/08/15.
	/// </summary>
	public abstract class PowerContainerVmAllocationPolicyMigrationAbstractContainerHostSelectionUnderUtilizedAdded : PowerContainerVmAllocationPolicyMigrationAbstractContainerHostSelection
	{

		private double underUtilizationThr;

		public PowerContainerVmAllocationPolicyMigrationAbstractContainerHostSelectionUnderUtilizedAdded(IList<ContainerHost> hostList, PowerContainerVmSelectionPolicy vmSelectionPolicy, PowerContainerSelectionPolicy containerSelectionPolicy, HostSelectionPolicy hostSelectionPolicy, double underUtilizationThr, int numberOfVmTypes, int[] vmPes, float[] vmRam, long vmBw, long vmSize, double[] vmMips) : base(hostList, vmSelectionPolicy, containerSelectionPolicy, hostSelectionPolicy, numberOfVmTypes, vmPes, vmRam, vmBw, vmSize, vmMips)
		{
			UnderUtilizationThr = underUtilizationThr;
		}


		protected internal override PowerContainerHost getUnderUtilizedHost(ISet<ContainerHost> excludedHosts)
		/// <summary>
		/// Gets the under utilized host.
		/// Checks if the utilization is under the threshold then counts it as underUtilized :) </summary>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the under utilized host </returns>
		{

			IList<ContainerHost> underUtilizedHostList = getUnderUtilizedHostList(excludedHosts);
			if (underUtilizedHostList.Count == 0)
			{

				return null;
			}

            ContainerHostList.sortByCpuUtilizationDescending(underUtilizedHostList);
	//        Log.print(String.format("The under Utilized Hosts are %d", underUtilizedHostList.size()));
			PowerContainerHost underUtilizedHost = (PowerContainerHost) underUtilizedHostList[0];

			return underUtilizedHost;
		}

		protected internal override IList<ContainerHost> getUnderUtilizedHostList(ISet<ContainerHost> excludedHosts)
		/// <summary>
		/// Gets the under utilized host.
		/// </summary>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the under utilized host </returns>
		{
			IList<ContainerHost> underUtilizedHostList = new List<ContainerHost>();
			foreach (PowerContainerHost host in ContainerHostListProperty)
			{
				if (excludedHosts.Contains(host))
				{
					continue;
				}
				double utilization = host.UtilizationOfCpu;
				if (!areAllVmsMigratingOutOrAnyVmMigratingIn(host) && utilization < UnderUtilizationThr && !areAllContainersMigratingOutOrAnyContainersMigratingIn(host))
				{
					underUtilizedHostList.Add(host);
				}
			}
			return underUtilizedHostList;
		}

		public virtual double UnderUtilizationThr
		{
			get
			{
				return underUtilizationThr;
			}
			set
			{
				this.underUtilizationThr = value;
			}
		}

	}

}