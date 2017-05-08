using System;
using System.Collections;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

    using ContainerAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerAllocationPolicy;
    using ContainerVmAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerVmAllocationPolicy;
    using CostumeCSVWriter = org.cloudbus.cloudsim.container.utils.CostumeCSVWriter;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
    using PredicateType = org.cloudbus.cloudsim.core.predicates.PredicateType;
    using System.Diagnostics;
    using System.IO;


    /// <summary>
    /// Created by sareh on 3/08/15.
    /// </summary>
    public class PowerContainerDatacenterCM : PowerContainerDatacenter
	{
		/// <summary>
		/// The disable container migrations.
		/// </summary>
		private bool disableMigrations;
		public int containerMigrationCount;
		private CostumeCSVWriter newlyCreatedVmWriter;
		private int newlyCreatedVms;
		private IList<int?> newlyCreatedVmsList;
		private double vmStartupDelay;
		private double containerStartupDelay;


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PowerContainerDatacenterCM(String name, ContainerDatacenterCharacteristics characteristics, org.cloudbus.cloudsim.container.resourceAllocators.ContainerVmAllocationPolicy vmAllocationPolicy, org.cloudbus.cloudsim.container.resourceAllocators.ContainerAllocationPolicy containerAllocationPolicy, java.util.List<org.cloudbus.cloudsim.Storage> storageList, double schedulingInterval, String experimentName, String logAddress, double vmStartupDelay, double containerStartupDelay) throws Exception
		public PowerContainerDatacenterCM(string name, ContainerDatacenterCharacteristics characteristics, ContainerVmAllocationPolicy vmAllocationPolicy, ContainerAllocationPolicy containerAllocationPolicy, IList<Storage> storageList, double schedulingInterval, string experimentName, string logAddress, double vmStartupDelay, double containerStartupDelay) : base(name, characteristics, vmAllocationPolicy, containerAllocationPolicy, storageList, schedulingInterval, experimentName, logAddress)
		{
			string newlyCreatedVmsAddress;
			int index = ExperimentName.LastIndexOf("_");
			newlyCreatedVmsAddress = string.Format("{0}/NewlyCreatedVms/{1}/{2}.csv", LogAddress, ExperimentName.Substring(0, index), ExperimentName);
			NewlyCreatedVmWriter = new CostumeCSVWriter(newlyCreatedVmsAddress);
			NewlyCreatedVms = 0;
			DisableMigrations = false;
			NewlyCreatedVmsList = new List<int?>();
			this.vmStartupDelay = vmStartupDelay;
			this.containerStartupDelay = containerStartupDelay;
		}

		protected internal override void updateCloudletProcessing()
		{

			//        Log.printLine("Power data center is Updating the cloudlet processing");
			if (CloudletSubmitted == -1 || CloudletSubmitted == CloudSim.clock())
			{
				CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.VM_DATACENTER_EVENT));
				schedule(Id, SchedulingInterval, CloudSimTags.VM_DATACENTER_EVENT);
				return;
			}
			double currentTime = CloudSim.clock();

			// if some time passed since last processing
			if (currentTime > LastProcessTime)
			{
				Debug.WriteLine(currentTime + " ");

				double minTime = updateCloudetProcessingWithoutSchedulingFutureEventsForce();

				if (!DisableMigrations)
				{
					IList<IDictionary<string, object>> migrationMap = VmAllocationPolicy.optimizeAllocation(ContainerVmListProperty);
					int previousContainerMigrationCount = ContainerMigrationCount;
					int previousVmMigrationCount = VmMigrationCount;
					if (migrationMap != null)
					{
						IList<ContainerVm> vmList = new List<ContainerVm>();
						foreach (IDictionary<string, object> migrate in migrationMap)
						{
							if (migrate.ContainsKey("container"))
							{
								Container container = (Container) migrate["container"];
								ContainerVm targetVm = (ContainerVm) migrate["vm"];
								ContainerVm oldVm = container.Vm;
								if (oldVm == null)
								{
									Log.formatLine("%.2f: Migration of Container #%d to Vm #%d is started", currentTime, container.Id, targetVm.Id);
								}
								else
								{
									Log.formatLine("%.2f: Migration of Container #%d from Vm #%d to VM #%d is started", currentTime, container.Id, oldVm.Id, targetVm.Id);
								}
								incrementContainerMigrationCount();
								targetVm.addMigratingInContainer(container);


								if (migrate.ContainsKey("NewEventRequired"))
								{
									if (!vmList.Contains(targetVm))
									{
										// A new VM is created  send a vm create request with delay :)
	//                                Send a request to create Vm after 100 second
	//                                            create a new event for this. or overright the vm create
										Log.formatLine("%.2f: Migration of Container #%d to newly created Vm #%d is started", currentTime, container.Id, targetVm.Id);
										targetVm.containerDestroyAll();
										send(Id, vmStartupDelay, CloudSimTags.VM_CREATE, migrate);
										vmList.Add(targetVm);

										send(Id, containerStartupDelay + vmStartupDelay, containerCloudSimTags.CONTAINER_MIGRATE, migrate);

									}
									else
									{
										Log.formatLine("%.2f: Migration of Container #%d to newly created Vm #%d is started", currentTime, container.Id, targetVm.Id);
	//                                    send a request for container migration after the vm is created
	//                                    it would be 100.4
										send(Id, containerStartupDelay + vmStartupDelay, containerCloudSimTags.CONTAINER_MIGRATE, migrate);


									}


								}
								else
								{
									send(Id, containerStartupDelay, containerCloudSimTags.CONTAINER_MIGRATE, migrate);


								}
							}
							else
							{
								ContainerVm vm = (ContainerVm) migrate["vm"];
								PowerContainerHost targetHost = (PowerContainerHost) migrate["host"];
								PowerContainerHost oldHost = (PowerContainerHost) vm.Host;

								if (oldHost == null)
								{
									Log.formatLine("%.2f: Migration of VM #%d to Host #%d is started", currentTime, vm.Id, targetHost.Id);
								}
								else
								{
									Log.formatLine("%.2f: Migration of VM #%d from Host #%d to Host #%d is started", currentTime, vm.Id, oldHost.Id, targetHost.Id);
								}

								targetHost.addMigratingInContainerVm(vm);
								incrementMigrationCount();

								/// <summary>
								/// VM migration delay = RAM / bandwidth * </summary>
								// we use BW / 2 to model BW available for migration purposes, the other
								// half of BW is for VM communication
								// around 16 seconds for 1024 MB using 1 Gbit/s network
								send(Id, vm.Ram / ((double) targetHost.Bw / (2 * 8000)), CloudSimTags.VM_MIGRATE, migrate);
							}


						}


						migrationMap.Clear();
						vmList.Clear();
					}
					ContainerMigrationList.Add((double)(ContainerMigrationCount - previousContainerMigrationCount));

					Log.printConcatLine(CloudSim.clock(), ": The Number Container of Migrations is:  ", ContainerMigrationCount - previousContainerMigrationCount);
					Log.printConcatLine(CloudSim.clock(), ": The Number of VM Migrations is:  ", VmMigrationCount - previousVmMigrationCount);
					string[] vmMig = new string[] {Convert.ToString(CloudSim.clock()), Convert.ToString(VmMigrationCount - previousVmMigrationCount)}; // <--declared statement
					string[] msg = new string[] {Convert.ToString(CloudSim.clock()), Convert.ToString(ContainerMigrationCount - previousContainerMigrationCount)}; // <--declared statement
					try
					{
						ContainerMigrationWriter.writeTofile(msg);
					}
					catch (IOException e)
					{
						Debug.WriteLine(e.ToString());
                        Debug.WriteLine(e.StackTrace);
					}
					try
					{
						VmMigrationWriter.writeTofile(vmMig);
					}
					catch (IOException e)
					{
                        Debug.WriteLine(e.ToString());
                        Debug.WriteLine(e.StackTrace);
					}


					int numberOfNewVms = NewlyCreatedVms;
					NewlyCreatedVmsList.Add(numberOfNewVms);
					string[] msg1 = new string[] {Convert.ToString(CloudSim.clock()), Convert.ToString(numberOfNewVms)}; // <--declared statement
					try
					{
						NewlyCreatedVmWriter.writeTofile(msg1);
					}
					catch (IOException e)
					{
                        Debug.WriteLine(e.ToString());
                        Debug.WriteLine(e.StackTrace);
					}
				}


				// schedules an event to the next time
				if (minTime != double.MaxValue)
				{
					CloudSim.cancelAll(Id, new PredicateType(CloudSimTags.VM_DATACENTER_EVENT));
					send(Id, SchedulingInterval, CloudSimTags.VM_DATACENTER_EVENT);
				}

				LastProcessTime = currentTime;

			}

		}

		protected internal override void processVmCreate(SimEvent ev, bool ack)
		{

	//    here we override the method
			if (ev.Data is IDictionary)
			{
				IDictionary<string, object> map = (IDictionary<string, object>) ev.Data;
				ContainerVm containerVm = (ContainerVm) map["vm"];
				ContainerHost host = (ContainerHost) map["host"];
				bool result = VmAllocationPolicy.allocateHostForVm(containerVm, host);
	//                set the containerVm in waiting state
				containerVm.InWaiting = true;
	//                containerVm.addMigratingInContainer((Container) map.get("container"));
				ack = true;
				if (ack)
				{
					IDictionary<string, object> data = new Dictionary<string, object>();
					data["vm"] = containerVm;
					data["result"] = containerVm;
					data["datacenterID"] = Id;

					if (result)
					{
						data["result"] = CloudSimTags.TRUE;
					}
					else
					{
						data["result"] = CloudSimTags.FALSE;
					}
					send(2, CloudSim.MinTimeBetweenEvents, containerCloudSimTags.VM_NEW_CREATE, data);
				}

				if (result)
				{
					Log.printLine(string.Format("{0} VM ID #{1:D} is created on Host #{2:D}", CloudSim.clock(), containerVm.Id, host.Id));
					incrementNewlyCreatedVmsCount();
					ContainerVmListProperty.Add(containerVm);


					if (containerVm.BeingInstantiated)
					{
						containerVm.BeingInstantiated = false;
					}

					containerVm.updateVmProcessing(CloudSim.clock(), VmAllocationPolicy.getHost(containerVm).ContainerVmScheduler.getAllocatedMipsForContainerVm(containerVm));
				}
			}
			else
			{
				base.processVmCreate(ev, ack);
			}
		}

		/// <summary>
		/// Increment migration count.
		/// </summary>
		protected internal virtual void incrementContainerMigrationCount()
		{
			ContainerMigrationCount = ContainerMigrationCount + 1;
		}

		/// <summary>
		/// Increment migration count.
		/// </summary>
		protected internal virtual void incrementNewlyCreatedVmsCount()
		{
			NewlyCreatedVms = NewlyCreatedVms + 1;
		}

		/// <summary>
		/// Checks if is disable migrations.
		/// </summary>
		/// <returns> true, if is disable migrations </returns>
		public virtual bool DisableMigrations
		{
			get
			{
				return disableMigrations;
			}
			set
			{
				this.disableMigrations = value;
			}
		}



		public virtual int ContainerMigrationCount
		{
			get
			{
				return containerMigrationCount;
			}
			set
			{
				this.containerMigrationCount = value;
			}
		}


		public virtual CostumeCSVWriter NewlyCreatedVmWriter
		{
			get
			{
				return newlyCreatedVmWriter;
			}
			set
			{
				this.newlyCreatedVmWriter = value;
			}
		}


		public virtual int NewlyCreatedVms
		{
			get
			{
				return newlyCreatedVms;
			}
			set
			{
				this.newlyCreatedVms = value;
			}
		}


		public virtual IList<int?> NewlyCreatedVmsList
		{
			get
			{
				return newlyCreatedVmsList;
			}
			set
			{
				this.newlyCreatedVmsList = value;
			}
		}



	}
}