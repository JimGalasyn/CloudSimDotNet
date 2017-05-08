using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled
{

	using org.cloudbus.cloudsim.container.core;
	using HostSelectionPolicy = org.cloudbus.cloudsim.container.hostSelectionPolicies.HostSelectionPolicy;
	using PowerContainerVmSelectionPolicy = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicy;

	/// <summary>
	/// Created by sareh on 17/11/15.
	/// </summary>
	public class PowerContainerVmAllocationPolicyMigrationAbstractHostSelection : PowerContainerVmAllocationPolicyMigrationAbstract
	{

		private HostSelectionPolicy hostSelectionPolicy;
		private double utilizationThreshold = 0.9;
		private double underUtilizationThreshold = 0.7;

		/// <summary>
		/// Instantiates a new power vm allocation policy migration abstract.
		/// </summary>
		/// <param name="hostSelectionPolicy"> </param>
		/// <param name="hostList">            the host list </param>
		/// <param name="vmSelectionPolicy">   the vm selection policy </param>
		public PowerContainerVmAllocationPolicyMigrationAbstractHostSelection(IList<ContainerHost> hostList, PowerContainerVmSelectionPolicy vmSelectionPolicy, HostSelectionPolicy hostSelectionPolicy, double OlThreshold, double UlThreshold) : base(hostList, vmSelectionPolicy)
		{
			HostSelectionPolicy = hostSelectionPolicy;
			UtilizationThreshold = OlThreshold;
			UnderUtilizationThreshold = UlThreshold;
		}

		public override PowerContainerHost findHostForVm(ContainerVm vm, ISet<ContainerHost> excludedHosts) 
		/// <summary>
		/// Find host for vm.
		/// </summary>
		/// <param name="vm">            the vm </param>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the power host </returns>
		{
			PowerContainerHost allocatedHost = null;
			bool? find = false;
			ISet<ContainerHost> excludedHost1 = new HashSet<ContainerHost>();
			excludedHost1.UnionWith(excludedHosts);
			while (!find.Value)
			{
				ContainerHost host = HostSelectionPolicy.getHost(ContainerHostListProperty, vm, excludedHost1);
				if (host == null)
				{
					return allocatedHost;
				}
				if (host.isSuitableForContainerVm(vm))
				{
					find = true;
					allocatedHost = (PowerContainerHost) host;
				}
				else
				{
					excludedHost1.Add(host);
					if (ContainerHostListProperty.Count == excludedHost1.Count)
					{

						return null;

					}
				}

			}
			return allocatedHost;
		}


		public virtual HostSelectionPolicy HostSelectionPolicy
		{
			get
			{
				return hostSelectionPolicy;
			}
			set
			{
				this.hostSelectionPolicy = value;
			}
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


		public virtual double UnderUtilizationThreshold
		{
			get
			{
				return underUtilizationThreshold;
			}
			set
			{
				this.underUtilizationThreshold = value;
			}
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


		/// <summary>
		/// Gets the under utilized host.
		/// </summary>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the under utilized host </returns>
		protected internal virtual IList<ContainerHost> getUnderUtilizedHostList(ISet<ContainerHost> excludedHosts) 
		{
			IList<ContainerHost> underUtilizedHostList = new List<ContainerHost>();
			foreach (PowerContainerHost host in ContainerHostListProperty)
			{
				if (excludedHosts.Contains(host))
				{
					continue;
				}
				double utilization = host.UtilizationOfCpu;
				if (!areAllVmsMigratingOutOrAnyVmMigratingIn(host) && utilization < UnderUtilizationThreshold && !areAllContainersMigratingOutOrAnyContainersMigratingIn(host))
				{
					underUtilizedHostList.Add(host);
				}
			}
			return underUtilizedHostList;
		}


		public override ContainerDatacenter Datacenter { get; set; }
		//{
		//	set
		//	{
		//	}
		//}
	}

}