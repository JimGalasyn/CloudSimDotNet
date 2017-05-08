using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled
{

	using PowerContainerSelectionPolicy = org.cloudbus.cloudsim.container.containerSelectionPolicies.PowerContainerSelectionPolicy;
	using org.cloudbus.cloudsim.container.core;
	using HostSelectionPolicy = org.cloudbus.cloudsim.container.hostSelectionPolicies.HostSelectionPolicy;
	using PowerContainerList = org.cloudbus.cloudsim.container.lists.PowerContainerList;
	using PowerContainerVmList = org.cloudbus.cloudsim.container.lists.PowerContainerVmList;
	using PowerContainerVmSelectionPolicy = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicy;

	/// <summary>
	/// Created by sareh on 11/08/15.
	/// </summary>
	public abstract class PowerContainerVmAllocationPolicyMigrationAbstractContainerHostSelection : PowerContainerVmAllocationPolicyMigrationAbstractContainerAdded
	{

		private HostSelectionPolicy hostSelectionPolicy;

		public PowerContainerVmAllocationPolicyMigrationAbstractContainerHostSelection(IList<ContainerHost> hostList, PowerContainerVmSelectionPolicy vmSelectionPolicy, PowerContainerSelectionPolicy containerSelectionPolicy, HostSelectionPolicy hostSelectionPolicy, int numberOfVmTypes, int[] vmPes, float[] vmRam, long vmBw, long vmSize, double[] vmMips) : base(hostList, vmSelectionPolicy, containerSelectionPolicy, numberOfVmTypes, vmPes, vmRam, vmBw, vmSize, vmMips)
		{
			HostSelectionPolicy = hostSelectionPolicy;
		}

		public override IDictionary<string, object> findHostForContainer(Container container, ISet<ContainerHost> excludedHosts, bool checkForVM)
		{

			PowerContainerHost allocatedHost = null;
			ContainerVm allocatedVm = null;
			IDictionary<string, object> map = new Dictionary<string, object>();
			ISet<ContainerHost> excludedHost1 = new HashSet<ContainerHost>();
			if (excludedHosts.Count == ContainerHostListProperty.Count)
			{
				return map;
			}
            // TEST: (fixed) UnionWith == addAll?
            excludedHost1.UnionWith(excludedHosts);
			while (true)
			{
				if (ContainerHostListProperty.Count == 0)
				{
					return map;
				}
				ContainerHost host = HostSelectionPolicy.getHost(ContainerHostListProperty, container, excludedHost1);
				bool findVm = false;
				IList<ContainerVm> vmList = host.VmListProperty;
				PowerContainerVmList.sortByCpuUtilization(vmList);
				for (int i = 0; i < vmList.Count; i++)
				{
					ContainerVm vm = vmList[vmList.Count - 1 - i];
					if (checkForVM)
					{
						if (vm.InWaiting)
						{

							continue;
						}

					}
					if (vm.isSuitableForContainer(container))
					{

						// if vm is overutilized or host would be overutilized after the allocation, this host is not chosen!
						if (!isVmOverUtilized(vm))
						{
							continue;
						}
						if (getUtilizationOfCpuMips((PowerContainerHost) host) != 0 && isHostOverUtilizedAfterContainerAllocation((PowerContainerHost) host, vm, container))
						{
							continue;
						}
						vm.containerCreate(container);
						allocatedVm = vm;
						findVm = true;
						allocatedHost = (PowerContainerHost) host;
						break;


					}
				}
				if (findVm)
				{

					map["vm"] = allocatedVm;
					map["host"] = allocatedHost;
					map["container"] = container;
					excludedHost1.Clear();
					return map;


				}
				else
				{
					excludedHost1.Add(host);
					if (ContainerHostListProperty.Count == excludedHost1.Count)
					{
						excludedHost1.Clear();
						return map;
					}
				}

			}


		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: @Override protected Collection<? extends Map<String, Object>> getContainerMigrationMapFromUnderUtilizedHosts(List<PowerContainerHostUtilizationHistory> overUtilizedHosts, List<Map<String, Object>> previouseMap)
		protected internal override ICollection<IDictionary<string, object>> getContainerMigrationMapFromUnderUtilizedHosts(IList<PowerContainerHostUtilizationHistory> overUtilizedHosts, IList<IDictionary<string, object>> previouseMap)
		{


			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();
			IList<PowerContainerHost> switchedOffHosts = SwitchedOffHosts;

			// over-utilized hosts + hosts that are selected to migrate VMs to from over-utilized hosts
			ISet<PowerContainerHost> excludedHostsForFindingUnderUtilizedHost = new HashSet<PowerContainerHost>();
            // TEST: (fixed) UnionWith vs addAll
            excludedHostsForFindingUnderUtilizedHost.UnionWith(overUtilizedHosts);
			excludedHostsForFindingUnderUtilizedHost.UnionWith(switchedOffHosts);
			excludedHostsForFindingUnderUtilizedHost.UnionWith(extractHostListFromMigrationMap(previouseMap));

			// over-utilized + under-utilized hosts
			ISet<PowerContainerHost> excludedHostsForFindingNewContainerPlacement = new HashSet<PowerContainerHost>();
			excludedHostsForFindingNewContainerPlacement.UnionWith(overUtilizedHosts);
			excludedHostsForFindingNewContainerPlacement.UnionWith(switchedOffHosts);

			int numberOfHosts = ContainerHostListProperty.Count;

			while (true)
			{
				if (numberOfHosts == excludedHostsForFindingUnderUtilizedHost.Count)
				{
					break;
				}

                // TEST: (fixed) Does this HashSet conversion work?
                var set = new HashSet<ContainerHost>(excludedHostsForFindingUnderUtilizedHost);
                PowerContainerHost underUtilizedHost = getUnderUtilizedHost(set);

                //PowerContainerHost underUtilizedHost = getUnderUtilizedHost(excludedHostsForFindingUnderUtilizedHost);
                if (underUtilizedHost == null)
				{
					break;
				}

				Log.printConcatLine("Under-utilized host: host #", underUtilizedHost.Id, "\n");

				excludedHostsForFindingUnderUtilizedHost.Add(underUtilizedHost);
				excludedHostsForFindingNewContainerPlacement.Add(underUtilizedHost);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: List<? extends Container> containersToMigrateFromUnderUtilizedHost = getContainersToMigrateFromUnderUtilizedHost(underUtilizedHost);
				IList<Container> containersToMigrateFromUnderUtilizedHost = getContainersToMigrateFromUnderUtilizedHost(underUtilizedHost);
				if (containersToMigrateFromUnderUtilizedHost.Count == 0)
				{
					continue;
				}

				Log.print("Reallocation of Containers from the under-utilized host: ");
				if (!Log.Disabled)
				{
					foreach (Container container in containersToMigrateFromUnderUtilizedHost)
					{
						Log.print(container.Id + " ");
					}
				}
				Log.printLine();

                // TEST: (fixed) Does this HashSet conversion work?
                var set2 = new HashSet<ContainerHost>(excludedHostsForFindingNewContainerPlacement);

                IList<IDictionary<string, object>> newContainerPlacement = getNewContainerPlacementFromUnderUtilizedHost(
                    containersToMigrateFromUnderUtilizedHost,
                    set2);
				//Sareh
				if (newContainerPlacement == null)
				{
	//                Add the host to the placement founder option
					excludedHostsForFindingNewContainerPlacement.Remove(underUtilizedHost);

				}

				excludedHostsForFindingUnderUtilizedHost.UnionWith(extractHostListFromMigrationMap(newContainerPlacement));
				//The migration mapp does not have a value for container since the whole vm would be migrated.
				((List<IDictionary<string, object>>)migrationMap).AddRange(newContainerPlacement);
				Log.printLine();
			}

			switchedOffHosts.Clear();
			excludedHostsForFindingUnderUtilizedHost.Clear();
			excludedHostsForFindingNewContainerPlacement.Clear();


			return migrationMap;


		}

		/// <summary>
		/// Gets the vms to migrate from under utilized host.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the vms to migrate from under utilized host </returns>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected List<? extends Container> getContainersToMigrateFromUnderUtilizedHost(PowerContainerHost host)
		protected internal virtual IList<Container> getContainersToMigrateFromUnderUtilizedHost(PowerContainerHost host)
		{
			IList<Container> containersToMigrate = new List<Container>();
			foreach (ContainerVm vm in host.VmListProperty)
			{
				if (!vm.InMigration)
				{
					foreach (Container container in vm.ContainerListProperty)
					{
						if (!container.InMigration)
						{
							containersToMigrate.Add(container);
						}
					}
				}
			}
			return containersToMigrate;
		}

		/// <summary>
		/// Gets the new vm placement from under utilized host.
		/// </summary>
		/// <param name="containersToMigrate"> the vms to migrate </param>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the new vm placement from under utilized host </returns>
		protected internal virtual IList<IDictionary<string, object>> getNewContainerPlacementFromUnderUtilizedHost(IList<Container> containersToMigrate, ISet<ContainerHost> excludedHosts)
		{
			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();
			PowerContainerList.sortByCpuUtilization(containersToMigrate);
			foreach (Container container in containersToMigrate)
			{
				IDictionary<string, object> allocatedMap = findHostForContainer(container, excludedHosts, true);
				if (allocatedMap["vm"] != null && allocatedMap["host"] != null)
				{

					Log.printConcatLine("Container# ",container.Id,"allocated to VM # ", ((ContainerVm)allocatedMap["vm"]).Id, " on host# ", ((ContainerHost)allocatedMap["host"]).Id);
					migrationMap.Add(allocatedMap);
				}
				else
				{
					Log.printLine("Not all Containers can be reallocated from the host, reallocation cancelled");
					allocatedMap.Clear();
					migrationMap.Clear();
					break;
				}
			}
			return migrationMap;
		}



	protected internal override IDictionary<string, object> findAvailableHostForContainer(Container container, IList<IDictionary<string, object>> createdVm)
	{


		PowerContainerHost allocatedHost = null;
		ContainerVm allocatedVm = null;
		IDictionary<string, object> map = new Dictionary<string, object>();
		ISet<ContainerHost> excludedHost1 = new HashSet<ContainerHost>();
		IList<ContainerHost> underUtilizedHostList = new List<ContainerHost>();
		foreach (IDictionary<string, object> map1 in createdVm)
		{
			ContainerHost host = (ContainerHost) map1["host"];
			if (!underUtilizedHostList.Contains(host))
			{
				underUtilizedHostList.Add(host);
			}

		}


		while (true)
		{

			ContainerHost host = HostSelectionPolicy.getHost(underUtilizedHostList, container, excludedHost1);
			IList<ContainerVm> vmList = new List<ContainerVm>();

			foreach (IDictionary<string, object> map2 in createdVm)
			{
				if (map2["host"] == host)
				{
					vmList.Add((ContainerVm) map2["vm"]);
				}

			}



			bool findVm = false;

			PowerContainerVmList.sortByCpuUtilization(vmList);
			for (int i = 0; i < vmList.Count; i++)
			{

				ContainerVm vm = vmList[vmList.Count - 1 - i];
				if (vm.isSuitableForContainer(container))
				{

					// if vm is overutilized or host would be overutilized after the allocation, this host is not chosen!
					if (!isVmOverUtilized(vm))
					{
						continue;
					}

					vm.containerCreate(container);
					allocatedVm = vm;
					findVm = true;
					allocatedHost = (PowerContainerHost) host;
					break;


				}
			}
			if (findVm)
			{

				map["vm"] = allocatedVm;
				map["host"] = allocatedHost;
				map["container"] = container;
				excludedHost1.Clear();
				return map;


			}
			else
			{
				if (host != null)
				{
				excludedHost1.Add(host);
				}
				if (underUtilizedHostList.Count == excludedHost1.Count)
				{
					excludedHost1.Clear();
					return map;
				}
			}

		}


	}

		public virtual HostSelectionPolicy HostSelectionPolicy
		{
			set
			{
				this.hostSelectionPolicy = value;
			}
			get
			{
				return hostSelectionPolicy;
			}
		}

	}

}