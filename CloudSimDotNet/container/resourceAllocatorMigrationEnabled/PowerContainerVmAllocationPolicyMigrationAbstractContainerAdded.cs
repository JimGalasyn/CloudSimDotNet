using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.resourceAllocatorMigrationEnabled
{

    using ContainerBwProvisionerSimple = org.cloudbus.cloudsim.container.containerProvisioners.ContainerBwProvisionerSimple;
    using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
    using ContainerRamProvisionerSimple = org.cloudbus.cloudsim.container.containerProvisioners.ContainerRamProvisionerSimple;
    using ContainerPeProvisionerSimple = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPeProvisionerSimple;
    using PowerContainerSelectionPolicy = org.cloudbus.cloudsim.container.containerSelectionPolicies.PowerContainerSelectionPolicy;
    using org.cloudbus.cloudsim.container.core;
    using org.cloudbus.cloudsim.container.lists;
    using PowerContainerAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.PowerContainerAllocationPolicy;
    using ContainerSchedulerTimeSharedOverSubscription = org.cloudbus.cloudsim.container.schedulers.ContainerSchedulerTimeSharedOverSubscription;
    using IDs = org.cloudbus.cloudsim.container.utils.IDs;
    using RandomGen = org.cloudbus.cloudsim.container.utils.RandomGen;
    using PowerContainerVmSelectionPolicy = org.cloudbus.cloudsim.container.vmSelectionPolicies.PowerContainerVmSelectionPolicy;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using ExecutionTimeMeasurer = org.cloudbus.cloudsim.util.ExecutionTimeMeasurer;
    using System.Diagnostics;
    using CloudSimLib.util;


    /// <summary>
    /// Created by sareh on 30/07/15.
    /// </summary>

    public abstract class PowerContainerVmAllocationPolicyMigrationAbstractContainerAdded : PowerContainerVmAllocationPolicyMigrationAbstract
	{

		private ContainerDatacenter datacenter;
		/// <summary>
		/// The container selection policy.
		/// </summary>
		private PowerContainerSelectionPolicy containerSelectionPolicy;
		protected internal int numberOfVmTypes;
		protected internal int[] vmPes;
		protected internal float[] vmRam;
		protected internal long vmBw;
		protected internal long vmSize;
		protected internal double[] vmMips;

		public PowerContainerVmAllocationPolicyMigrationAbstractContainerAdded(IList<ContainerHost> hostList, PowerContainerVmSelectionPolicy vmSelectionPolicy, PowerContainerSelectionPolicy containerSelectionPolicy, int numberOfVmTypes, int[] vmPes, float[] vmRam, long vmBw, long vmSize, double[] vmMips) : base(hostList, vmSelectionPolicy)
		{
	//        setDatacenter(datacenter);
			ContainerSelectionPolicy = containerSelectionPolicy;

			this.numberOfVmTypes = numberOfVmTypes;
			this.vmPes = vmPes;
			this.vmRam = vmRam;
			this.vmBw = vmBw;
			this.vmSize = vmSize;
			this.vmMips = vmMips;
		}

		public override IList<IDictionary<string, object>> optimizeAllocation(IList<ContainerVm> vmList)
		{

			ExecutionTimeMeasurer.start("optimizeAllocationTotal");

			ExecutionTimeMeasurer.start("optimizeAllocationHostSelection");
			IList<PowerContainerHostUtilizationHistory> overUtilizedHosts = OverUtilizedHosts;
			ExecutionTimeHistoryHostSelection.Add(ExecutionTimeMeasurer.end("optimizeAllocationHostSelection"));

			printOverUtilizedHosts(overUtilizedHosts);

			saveAllocation();

			ExecutionTimeMeasurer.start("optimizeAllocationContainerSelection");
			IList<Container> containersToMigrate = getContainersToMigrateFromHosts(overUtilizedHosts);
			ExecutionTimeHistoryVmSelection.Add(ExecutionTimeMeasurer.end("optimizeAllocationContainerSelection"));

			Log.printLine("Reallocation of Containers from the over-utilized hosts:");
			ExecutionTimeMeasurer.start("optimizeAllocationVmReallocation");
			IList<IDictionary<string, object>> migrationMap = getPlacementForLeftContainers(containersToMigrate, new HashSet<ContainerHost>(overUtilizedHosts));


			ExecutionTimeHistoryVmReallocation.Add(ExecutionTimeMeasurer.end("optimizeAllocationVmReallocation"));
			Log.printLine();

			((List<IDictionary<string, object>>)migrationMap).AddRange(getContainerMigrationMapFromUnderUtilizedHosts(overUtilizedHosts, migrationMap));

			restoreAllocation();

			ExecutionTimeHistoryTotal.Add(ExecutionTimeMeasurer.end("optimizeAllocationTotal"));

			return migrationMap;
		}

		protected internal virtual ICollection<IDictionary<string, object>> getContainerMigrationMapFromUnderUtilizedHosts(IList<PowerContainerHostUtilizationHistory> overUtilizedHosts, IList<IDictionary<string, object>> previouseMap)
		{

			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();
			IList<PowerContainerHost> switchedOffHosts = SwitchedOffHosts;

			// over-utilized hosts + hosts that are selected to migrate VMs to from over-utilized hosts
			ISet<PowerContainerHost> excludedHostsForFindingUnderUtilizedHost = new HashSet<PowerContainerHost>();
            // TEST: (fixed) UnionWith was addAll
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
                //PowerContainerHost underUtilizedHost = null; // getUnderUtilizedHost(excludedHostsForFindingUnderUtilizedHost);
				if (underUtilizedHost == null)
				{
					break;
				}

				Log.printConcatLine("Under-utilized host: host #", underUtilizedHost.Id, "\n");

				excludedHostsForFindingUnderUtilizedHost.Add(underUtilizedHost);
				excludedHostsForFindingNewContainerPlacement.Add(underUtilizedHost);

				IList<ContainerVm> vmsToMigrateFromUnderUtilizedHost = getVmsToMigrateFromUnderUtilizedHost(underUtilizedHost);
				if (vmsToMigrateFromUnderUtilizedHost.Count == 0)
				{
					continue;
				}

				Log.print("Reallocation of Containers from the under-utilized host: ");
				if (!Log.Disabled)
				{
					foreach (ContainerVm vm in vmsToMigrateFromUnderUtilizedHost)
					{
						Log.print(vm.Id + " ");
					}
				}
				Log.printLine();

                // TODO: Fix this generic type issue.
                IList<IDictionary<string, object>> newVmPlacement = null; 
                    //getNewVmPlacementFromUnderUtilizedHost(vmsToMigrateFromUnderUtilizedHost, excludedHostsForFindingNewContainerPlacement);
				//Sareh
				if (newVmPlacement == null)
				{
	//                Add the host to the placement founder option
					excludedHostsForFindingNewContainerPlacement.Remove(underUtilizedHost);

				}

				excludedHostsForFindingUnderUtilizedHost.UnionWith(extractHostListFromMigrationMap(newVmPlacement));
				//The migration mapp does not have a value for container since the whole vm would be migrated.
				((List<IDictionary<string, object>>)migrationMap).AddRange(newVmPlacement);
				Log.printLine();
			}

			excludedHostsForFindingUnderUtilizedHost.Clear();
			excludedHostsForFindingNewContainerPlacement.Clear();
			return migrationMap;
		}

		private IList<Container> getContainersToMigrateFromHosts(IList<PowerContainerHostUtilizationHistory> overUtilizedHosts)
		{
			IList<Container> containersToMigrate = new List<Container>();
			foreach (PowerContainerHostUtilizationHistory host in overUtilizedHosts)
			{
				while (true)
				{
					Container container = ContainerSelectionPolicy.getContainerToMigrate(host);
					if (container == null)
					{
						break;
					}
					containersToMigrate.Add(container);
					container.Vm.containerDestroy(container);
					if (!isHostOverUtilized(host))
					{
						break;
					}
				}
			}
			return containersToMigrate;
		}


		private IList<IDictionary<string, object>> getNewContainerPlacement(IList<Container> containersToMigrate, ISet<ContainerHost> excludedHosts)
		{

			IList<IDictionary<string, object>> migrationMap = new List<IDictionary<string, object>>();

			PowerContainerList.sortByCpuUtilization(containersToMigrate);
			foreach (Container container in containersToMigrate)
			{
				IDictionary<string, object> allocationMap = findHostForContainer(container, excludedHosts, false);

				if (allocationMap["host"] != null && allocationMap["vm"] != null)
				{
					ContainerVm vm = (ContainerVm) allocationMap["vm"];
					Log.printConcatLine("Container #", container.Id, " allocated to host #", ((PowerContainerHost) allocationMap["host"]).Id, "The VM ID is #", vm.Id);
					IDictionary<string, object> migrate = new Dictionary<string, object>();
					migrate["container"] = container;
					migrate["vm"] = vm;
					migrate["host"] = (PowerContainerHost) allocationMap["host"];
					migrationMap.Add(migrate);
				}
				else
				{
					IDictionary<string, object> migrate = new Dictionary<string, object>();
					migrate["NewVmRequired"] = container;
					migrationMap.Add(migrate);

				}

			}
			containersToMigrate.Clear();
			return migrationMap;
		}

		private IList<IDictionary<string, object>> getPlacementForLeftContainers(IList<Container> containersToMigrate, ISet<ContainerHost> excludedHostsList)
		{
			IList<IDictionary<string, object>> newMigrationMap = new List<IDictionary<string, object>>();

			if (containersToMigrate.Count == 0)
			{
				return newMigrationMap;
			}
			HashSet<ContainerHost> excludedHostsforOverUtilized = new HashSet<ContainerHost>();
			excludedHostsforOverUtilized.UnionWith(SwitchedOffHosts);
			excludedHostsforOverUtilized.UnionWith(excludedHostsList);
			IList<IDictionary<string, object>> migrationMap = getNewContainerPlacement(containersToMigrate, excludedHostsforOverUtilized);
			if (migrationMap.Count == 0)
			{
				return migrationMap;
			}
	//        List<Container> containerList = getExtraContainers(migrationMap);


			IList<Container> containerList = new List<Container>();
			foreach (IDictionary<string, object> map in migrationMap)
			{
				if (map.ContainsKey("NewVmRequired"))
				{
					containerList.Add((Container) map["NewVmRequired"]);

				}
				else
				{
					newMigrationMap.Add(map);
				}

			}
			if (containerList.Count == 0)
			{
				return newMigrationMap;
			}

			IList<ContainerHost> underUtilizedHostList = getUnderUtilizedHostList(excludedHostsList);

			IList<IDictionary<string, object>> migrationMapUnderUtilized = findMapInUnderUtilizedHosts(underUtilizedHostList,containerList);
			((List<IDictionary<string, object>>)newMigrationMap).AddRange(migrationMapUnderUtilized);
            // TODO: TEST: (Fixed) Make sure this removeAll equivalent works.
            //containerList.removeAll(getAssignedContainers(migrationMapUnderUtilized));
            containerList.RemoveAll<Container>(getAssignedContainers(migrationMapUnderUtilized));
            //containerList.Clear();
            if (containerList.Count != 0)
			{
				IList<IDictionary<string, object>> migrationMapSwitchedOff = findMapInSwitchedOffHosts(containerList);
				((List<IDictionary<string, object>>)newMigrationMap).AddRange(migrationMapSwitchedOff);

			}


	// Now Check if there are any containers left without VMs.
			//firsthost chosen


			return newMigrationMap;
		}
		protected internal virtual IList<IDictionary<string, object>> findMapInUnderUtilizedHosts(IList<ContainerHost> underUtilizedHostList, IList<Container> containerList)
		{
			IList<IDictionary<string, object>> newMigrationMap = new List<IDictionary<string, object>>();
			//        Create new Vms on underUtilized hosts;
			IList<IDictionary<string, object>> createdVmMap = new List<IDictionary<string, object>>();
			if (underUtilizedHostList.Count != 0)
			{
				foreach (ContainerHost host in underUtilizedHostList)
				{
	//                   We try to create the largest Vm possible
					IList<ContainerVm> VmList = createVms(host, true);
					if (VmList.Count != 0)
					{
						foreach (ContainerVm vm in VmList)
						{
							IDictionary<string, object> map = new Dictionary<string, object>();
							map["host"] = host;
							map["vm"] = vm;
							createdVmMap.Add(map);

						}
					}
				}
				if (createdVmMap.Count == 0)
				{

					return newMigrationMap;

				}

				//        if there are any new Vms on the underUtilized Hosts we assign the containers to them first!
				// Sort the underUtilized host by the utilization, so that we first assign vms to the more utilized ones
				foreach (Container container in containerList)
				{
					IDictionary<string, object> allocationMap = findAvailableHostForContainer(container, createdVmMap);
					if (allocationMap["host"] != null && allocationMap["vm"] != null)
					{
						ContainerVm vm = (ContainerVm) allocationMap["vm"];
						Log.printConcatLine("Container #", container.Id, " allocated to host #", ((PowerContainerHost) allocationMap["host"]).Id, "The VM ID is #", vm.Id);
						IDictionary<string, object> migrate = new Dictionary<string, object>();
	//                    vm.setInWaiting(true);
						migrate["NewEventRequired"] = container;
						migrate["container"] = container;
						migrate["vm"] = vm;
						migrate["host"] = (PowerContainerHost) allocationMap["host"];
						newMigrationMap.Add(migrate);

					}
				}
			}

		  return newMigrationMap;

		}
		protected internal virtual IList<Container> getAssignedContainers(IList<IDictionary<string, object>> migrationMap)
		{
			IList<Container> assignedContainers = new List<Container>();
			foreach (IDictionary<string, object> map in migrationMap)
			{
				if (map.ContainsKey("container"))
				{
					assignedContainers.Add((Container) map["container"]);
				}



			}

		return assignedContainers;
		}
		protected internal virtual ContainerVm createVMinHost(ContainerHost host, bool vmStatus)
		{

			for (int i = 0; i < numberOfVmTypes; i++)
			{
				ContainerVm vm = getNewVm(i);
				if (getUtilizationOfCpuMips((PowerContainerHost) host) != 0 && isHostOverUtilizedAfterAllocation((PowerContainerHost) host, vm))
				{
					continue;
				}

				if (allocateHostForVm(vm, host))
				{
					Log.printLine("The vm ID #" + vm.Id + "will be created ");
					vm.InWaiting = vmStatus;
					return vm;
				}
			}

			return null;
		}

		protected internal virtual IList<IDictionary<string, object>> findMapInSwitchedOffHosts(IList<Container> containerList)
		{
			Log.print(string.Format(" {0} :  Find Placement in the switched of hosts", CloudSim.clock()));
			IList<PowerContainerHost> switchedOffHostsList = SwitchedOffHosts;
			IList<IDictionary<string, object>> newMigrationMap = new List<IDictionary<string, object>>();

			if (containerList.Count == 0)
			{

				return newMigrationMap;
			}

			ContainerHost previousHost = null;
			ContainerVm previousVm = null;
			while (containerList.Count != 0)
			{
				if (switchedOffHostsList.Count == 0)
				{

					Log.print("There is no hosts to create VMs");
					break;
				}
				IList<Container> assignedContainer = new List<Container>();
				//choose a random host
				if (previousHost == null && previousVm == null)
				{
					if (switchedOffHostsList.Count == 0)
					{
						return newMigrationMap;
					}
					int hostIndex = (new RandomGen()).getNum(switchedOffHostsList.Count);
					previousHost = switchedOffHostsList[hostIndex];
                    // TEST: (fixed) Make sure this cast works.
                    switchedOffHostsList.Remove(previousHost as PowerContainerHost);
					previousVm = createVMinHost(previousHost, true);
					previousHost.containerVmCreate(previousVm);

					foreach (Container container in containerList)
					{
						if (previousVm.isSuitableForContainer(container))
						{
							previousVm.containerCreate(container);
							assignedContainer.Add(container);
							IDictionary<string, object> migrate = new Dictionary<string, object>();
	//                        previousVm.setInWaiting(true);
							migrate["NewEventRequired"] = container;
							migrate["container"] = container;
							migrate["vm"] = previousVm;
							migrate["host"] = previousHost;
							newMigrationMap.Add(migrate);
						}
						else
						{

							previousVm = createVMinHost(previousHost, true);
							if (previousVm == null)
							{
                                //switchedOffHostsList.Remove(previousHost);
                                // TEST: (fixed) Make sure this cast works.
                                switchedOffHostsList.Remove(previousHost as PowerContainerHost);
                                previousHost = null;
                                // TODO: TEST: (fixed) RemoveAll
                                //containerList.removeAll(assignedContainer);
                                containerList.RemoveAll<Container>(assignedContainer);

                                break;
							}
							previousVm.containerCreate(container);
							assignedContainer.Add(container);
							IDictionary<string, object> migrate = new Dictionary<string, object>();
	//                        previousVm.setInWaiting(true);
							migrate["NewEventRequired"] = container;
							migrate["container"] = container;
							migrate["vm"] = previousVm;
							migrate["host"] = previousHost;
							newMigrationMap.Add(migrate);
						}
					}

                    //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
                    // TEST: (fixed) RemoveAll
                    //containerList.removeAll(assignedContainer);
                    containerList.RemoveAll<Container>(assignedContainer);
                }
                else
				{

					foreach (Container container in containerList)
					{
						if (previousVm.isSuitableForContainer(container))
						{
							previousVm.containerCreate(container);
							assignedContainer.Add(container);
							IDictionary<string, object> migrate = new Dictionary<string, object>();
	//                        previousVm.setInWaiting(true);
							migrate["NewEventRequired"] = container;
							migrate["container"] = container;
							migrate["vm"] = previousVm;
							migrate["host"] = previousHost;
							newMigrationMap.Add(migrate);
						}
						else
						{

							previousVm = createVMinHost(previousHost, true);
							if (previousVm == null)
							{
                                // TEST: (fixed) Make sure this cast works.
                                switchedOffHostsList.Remove(previousHost as PowerContainerHost);
								previousHost = null;
                                //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
                                // TEST: (Fixed) Make sure this removeAll equivalent works.
                                //containerList.removeAll(assignedContainer);
                                containerList.RemoveAll<Container>(assignedContainer);
                                break;
							}
							previousVm.containerCreate(container);
							assignedContainer.Add(container);
							IDictionary<string, object> migrate = new Dictionary<string, object>();
	//                        previousVm.setInWaiting(true);
							migrate["NewEventRequired"] = container;
							migrate["container"] = container;
							migrate["vm"] = previousVm;
							migrate["host"] = previousHost;
							newMigrationMap.Add(migrate);

						}
					}

                    //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
                    // TEST: (fixed) RemoveAll
                    //containerList.removeAll(assignedContainer);
                    containerList.RemoveAll<Container>(assignedContainer);
                }
			}

			return newMigrationMap;
		}

		//    This method should be re written!
		protected internal virtual IDictionary<string, object> findAvailableHostForContainer(Container container, IList<IDictionary<string, object>> createdVm)
		{

			double minPower = double.MaxValue;
			PowerContainerHost allocatedHost = null;
			ContainerVm allocatedVm = null;
			IList<ContainerHost> underUtilizedHostList = new List<ContainerHost>();
			IList<ContainerVm> vmList = new List<ContainerVm>();
			foreach (IDictionary<string, object> dictionary in createdVm)
			{
				underUtilizedHostList.Add((ContainerHost)dictionary["host"]);

			}

            // TODO: Fix this generic type issue.
            //ContainerHostListProperty.sortByCpuUtilization(underUtilizedHostList);
			foreach (ContainerHost host1 in underUtilizedHostList)
			{

				PowerContainerHost host = (PowerContainerHost) host1;
				foreach (IDictionary<string, object> dictionary in createdVm)
				{
					if ((ContainerHost)dictionary["host"] == host1)
					{
					vmList.Add((ContainerVm)dictionary["vm"]);
					}

				}
				foreach (ContainerVm vm in vmList)
				{
	//                if vm is not created no need for checking!

					if (vm.isSuitableForContainer(container))
					{
						// if vm is overutilized or host would be overutilized after the allocation, this host is not chosen!
						if (!isVmOverUtilized(vm))
						{
							continue;
						}
						if (getUtilizationOfCpuMips(host) != 0 && isHostOverUtilizedAfterContainerAllocation(host, vm, container))
						{
							continue;
						}

						try
						{
							double powerAfterAllocation = getPowerAfterContainerAllocation(host, container, vm);
							if (powerAfterAllocation != -1)
							{
								double powerDiff = powerAfterAllocation - host.Power;
								if (powerDiff < minPower)
								{
									minPower = powerDiff;
									allocatedHost = host;
									allocatedVm = vm;
								}
							}
						}
						catch (Exception)
						{
							Log.print("Error: Exception in powerDiff algorithm containerAdded");
						}
					}
				}
			}


			IDictionary<string, object> map = new Dictionary<string, object>();
			map["vm"] = allocatedVm;
			map["host"] = allocatedHost;

			return map;


		}

		private ContainerVm getNewVm(int vmType)
		{

			List<ContainerPe> peList = new List<ContainerPe>();
	//        int vmType = new RandomGen().getNum(ConstantsEx.VM_TYPES);
	//            int vmType = i / (int) Math.ceil((double) containerVmsNumber / 4.0D);
	//            int vmType = 1;
	//        Log.print(vmType);
			for (int j = 0; j < vmPes[vmType]; ++j)
			{
				peList.Add(new ContainerPe(j, new ContainerPeProvisionerSimple((double) vmMips[vmType])));
			}
			int brokerId = 2;
			PowerContainerVm vm = new PowerContainerVm(
                IDs.pollId(typeof(ContainerVm)), 
                brokerId, 
                (double) vmMips[vmType], 
                vmRam[vmType], 
                vmBw, 
                vmSize, 
                "Xen", 
                new ContainerSchedulerTimeSharedOverSubscription(peList), 
                new ContainerRamProvisionerSimple(vmRam[vmType]), 
                new ContainerBwProvisionerSimple(vmBw), peList, 300);
			return vm;

		}


		/// <summary>
		/// Gets the under utilized host.
		/// </summary>
		/// <param name="excludedHosts"> the excluded hosts </param>
		/// <returns> the under utilized host </returns>
		protected internal virtual IList<ContainerHost> getUnderUtilizedHostList(ISet<ContainerHost> excludedHosts)
		{
			IList<ContainerHost> underUtilizedHostList = new List<ContainerHost>();
			double minUtilization = 1;
			foreach (PowerContainerHost host in ContainerHostListProperty)
			{
				if (excludedHosts.Contains(host))
				{
					continue;
				}
				double utilization = host.UtilizationOfCpu;
				if (utilization > 0 && utilization < minUtilization && !areAllVmsMigratingOutOrAnyVmMigratingIn(host) && !areAllContainersMigratingOutOrAnyContainersMigratingIn(host))
				{
					minUtilization = utilization;
					underUtilizedHostList.Add(host);
				}
			}
			return underUtilizedHostList;
		}

		public virtual IDictionary<string, object> findHostForContainer(Container container, ISet<ContainerHost> excludedHosts, bool checkForVM)
		{
			double minPower = double.MaxValue;
			PowerContainerHost allocatedHost = null;
			ContainerVm allocatedVm = null;

			foreach (PowerContainerHost host in ContainerHostListProperty)
			{
				if (excludedHosts.Contains(host))
				{
					continue;
				}
				foreach (ContainerVm vm in host.VmListProperty)
				{
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
						if (getUtilizationOfCpuMips(host) != 0 && isHostOverUtilizedAfterContainerAllocation(host, vm, container))
						{
							continue;
						}

						try
						{
							double powerAfterAllocation = getPowerAfterContainerAllocation(host, container, vm);
							if (powerAfterAllocation != -1)
							{
								double powerDiff = powerAfterAllocation - host.Power;
								if (powerDiff < minPower)
								{
									minPower = powerDiff;
									allocatedHost = host;
									allocatedVm = vm;
								}
							}
						}
						catch (Exception)
						{
						}
					}
				}
			}
			IDictionary<string, object> map = new Dictionary<string, object>();
			map["vm"] = allocatedVm;
			map["host"] = allocatedHost;

			return map;
		}

		protected internal virtual bool isVmOverUtilized(ContainerVm vm)
		{
			bool isOverUtilized = true;
			double util = 0;
	//        Log.printConcatLine("Checking if the vm is over utilized or not!");
			foreach (Container container in vm.ContainerListProperty)
			{
				util += container.getTotalUtilizationOfCpuMips(CloudSim.clock());
			}
			if (util > vm.Host.TotalMips / vm.Host.NumberOfPes * vm.NumberOfPes)
			{
				return false;
			}


			return isOverUtilized;
		}

		/// <summary>
		/// Gets the power after allocation.
		/// </summary>
		/// <param name="host">      the host </param>
		/// <param name="container"> the vm </param>
		/// <returns> the power after allocation </returns>
		protected internal virtual double getPowerAfterContainerAllocation(PowerContainerHost host, Container container, ContainerVm vm)
		{
			double power = 0;
			try
			{
				power = host.PowerModel.getPower(getMaxUtilizationAfterContainerAllocation(host, container, vm));
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                //Environment.Exit(0);
                throw e;
			}
			return power;
		}

		/// <summary>
		/// Gets the power after allocation. We assume that load is balanced between PEs. The only
		/// restriction is: VM's max MIPS < PE's MIPS
		/// </summary>
		/// <param name="host">      the host </param>
		/// <param name="container"> the vm </param>
		/// <returns> the power after allocation </returns>
		protected internal virtual double getMaxUtilizationAfterContainerAllocation(PowerContainerHost host, Container container, ContainerVm containerVm)
		{
			double requestedTotalMips = container.CurrentRequestedTotalMips;
			if (requestedTotalMips > containerVm.Mips)
			{
				requestedTotalMips = containerVm.Mips;
			}
			double hostUtilizationMips = getUtilizationOfCpuMips(host);
			double hostPotentialUtilizationMips = hostUtilizationMips + requestedTotalMips;
			double pePotentialUtilization = hostPotentialUtilizationMips / host.TotalMips;
			return pePotentialUtilization;
		}

		/// <summary>
		/// Gets the utilization of the CPU in MIPS for the current potentially allocated VMs.
		/// </summary>
		/// <param name="containerVm"> the host </param>
		/// <returns> the utilization of the CPU in MIPS </returns>
		protected internal virtual double getUtilizationOfCpuMipsofVm(ContainerVm containerVm)
		{
			double vmUtilizationMips = 0;
			foreach (Container container in containerVm.ContainerListProperty)
			{

				vmUtilizationMips += containerVm.getTotalAllocatedMipsForContainer(container);
			}
			return vmUtilizationMips;
		}

		/// <summary>
		/// Checks if is host over utilized after allocation.
		/// </summary>
		/// <param name="host">      the host </param>
		/// <param name="container"> the vm </param>
		/// <returns> true, if is host over utilized after allocation </returns>
		protected internal virtual bool isHostOverUtilizedAfterContainerAllocation(PowerContainerHost host, ContainerVm vm, Container container)
		{
			bool isHostOverUtilizedAfterAllocation = true;
			if (vm.containerCreate(container))
			{
				isHostOverUtilizedAfterAllocation = isHostOverUtilized(host);
				vm.containerDestroy(container);
			}
			return isHostOverUtilizedAfterAllocation;
		}


		/// <summary>
		/// Save allocation.
		/// </summary>
		protected internal override void saveAllocation()
		{
			SavedAllocation.Clear();
			foreach (ContainerHost host in ContainerHostListProperty)
			{
				foreach (ContainerVm vm in host.VmListProperty)
				{
					if (host.VmsMigratingIn.Contains(vm))
					{
						continue;
					}
					foreach (Container container in vm.ContainerListProperty)
					{
						if (vm.ContainersMigratingIn.Contains(container))
						{
							continue;
						}
						IDictionary<string, object> map = new Dictionary<string, object>();
						map["host"] = host;
						map["vm"] = vm;
						map["container"] = container;
						SavedAllocation.Add(map);
					}
				}
			}
			Log.printLine(string.Format("The length of the saved map is ....{0:D}", SavedAllocation.Count));

		}


		/// <summary>
		/// Restore allocation.
		/// </summary>
		protected internal override void restoreAllocation()
		{
			foreach (ContainerHost host in ContainerHostListProperty)
			{
				foreach (ContainerVm vm in host.VmListProperty)
				{
					vm.containerDestroyAll();
					vm.reallocateMigratingInContainers();
				}

				host.containerVmDestroyAll();
				host.reallocateMigratingInContainerVms();
			}
			foreach (IDictionary<string, object> map in SavedAllocation)
			{
				PowerContainerVm vm = (PowerContainerVm) map["vm"];

				PowerContainerHost host = (PowerContainerHost) map["host"];
				if (!host.VmListProperty.Contains(vm))
				{
					if (!host.containerVmCreate(vm))
					{
						Log.printConcatLine("Couldn't restore VM #", vm.Id, " on host #", host.Id);
                        //Environment.Exit(0);
                        throw new InvalidOperationException("Couldn't restore VM");
					}

                    //VmTable.put(vm.Uid, host);
                    VmTable[vm.Uid] = host;
                }
	//            vm.containerDestroyAll();
	//            vm.reallocateMigratingInContainers();
			}
	//        List<ContainerVm > restoredVms = new ArrayList<>();
			foreach (IDictionary<string, object> map in SavedAllocation)
			{
				PowerContainerVm vm = (PowerContainerVm) map["vm"];
				if (map["container"] != null && map.ContainsKey("container"))
				{
					Container container = (Container) map["container"];
	//                Log.print(container);

					if (!vm.ContainerListProperty.Contains(container))
					{
						if (!vm.containerCreate(container))
						{
							Log.printConcatLine("Couldn't restore Container #", container.Id, " on vm #", vm.Id);
                            //Environment.Exit(0);
                            throw new InvalidOperationException("Couldn't restore Container");
                        }
					}
					else
					{

						Log.print("The Container is in the VM already");
					}

					if (container.Vm == null)
					{
						Log.print("The Vm is null");

					}
					((PowerContainerAllocationPolicy) Datacenter.ContainerAllocationPolicy).ContainerTable[container.Uid] = vm;
	//            container.setVm(vm);

				}
			}


		}

		protected internal virtual IList<ContainerVm> createVms(ContainerHost host, bool vmStatus)
		{
			IList<ContainerVm> vmList = new List<ContainerVm>();
			while (true)
			{
				ContainerVm vm = createVMinHost(host, vmStatus);
				if (vm == null)
				{
					break;
				}
				vmList.Add(vm);
			}
			return vmList;


		}

		public override ContainerDatacenter Datacenter
		{
			get
			{
				return datacenter;
			}
			set
			{
				this.datacenter = value;
			}
		}


		public virtual PowerContainerSelectionPolicy ContainerSelectionPolicy
		{
			get
			{
				return containerSelectionPolicy;
			}
			set
			{
				this.containerSelectionPolicy = value;
			}
		}


	}

}