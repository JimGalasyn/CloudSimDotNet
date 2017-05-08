using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

	using ContainerList = org.cloudbus.cloudsim.container.lists.ContainerList;
	using ContainerVmList = org.cloudbus.cloudsim.container.lists.ContainerVmList;
	//using Percentile = org.apache.commons.math3.stat.descriptive.rank.Percentile;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
	using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
	using SimEntity = org.cloudbus.cloudsim.core.SimEntity;
	using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
	using CloudletList = org.cloudbus.cloudsim.lists.CloudletList;


	/// <summary>
	/// Created by sareh on 15/07/15.
	/// </summary>

	public class ContainerDatacenterBroker : SimEntity
	{


		/// <summary>
		/// The vm list.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends ContainerVm> vmList;
		protected internal IList<ContainerVm> vmList;

		/// <summary>
		/// The vms created list.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends ContainerVm> vmsCreatedList;
		protected internal IList<ContainerVm> vmsCreatedList;
	/// <summary>
	/// The containers created list.
	/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends Container> containersCreatedList;
		protected internal IList<Container> containersCreatedList;

		/// <summary>
		/// The cloudlet list.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends ContainerCloudlet> cloudletList;
		protected internal IList<ContainerCloudlet> cloudletList;
		/// <summary>
		/// The container list
		/// </summary>

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends Container> containerList;
		protected internal IList<Container> containerList;

		/// <summary>
		/// The cloudlet submitted list.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends ContainerCloudlet> cloudletSubmittedList;
		protected internal IList<ContainerCloudlet> cloudletSubmittedList;

		/// <summary>
		/// The cloudlet received list.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends ContainerCloudlet> cloudletReceivedList;
		protected internal IList<ContainerCloudlet> cloudletReceivedList;

		/// <summary>
		/// The cloudlets submitted.
		/// </summary>
		protected internal int cloudletsSubmitted;

		/// <summary>
		/// The vms requested.
		/// </summary>
		protected internal int vmsRequested;

		/// <summary>
		/// The vms acks.
		/// </summary>
		protected internal int vmsAcks;
		/// <summary>
		/// The containers acks.
		/// </summary>
		protected internal int containersAcks;
		/// <summary>
		/// The number of created containers
		/// </summary>

		protected internal int containersCreated;

		/// <summary>
		/// The vms destroyed.
		/// </summary>
		protected internal int vmsDestroyed;

		/// <summary>
		/// The datacenter ids list.
		/// </summary>
		protected internal IList<int?> datacenterIdsList;

		/// <summary>
		/// The datacenter requested ids list.
		/// </summary>
		protected internal IList<int?> datacenterRequestedIdsList;

		/// <summary>
		/// The vms to datacenters map.
		/// </summary>
		protected internal IDictionary<int?, int?> vmsToDatacentersMap;
	 /// <summary>
	 /// The vms to datacenters map.
	 /// </summary>
		protected internal IDictionary<int?, int?> containersToDatacentersMap;

		/// <summary>
		/// The datacenter characteristics list.
		/// </summary>
		protected internal IDictionary<int?, ContainerDatacenterCharacteristics> datacenterCharacteristicsList;

		/// <summary>
		/// The datacenter characteristics list.
		/// </summary>
		protected internal double overBookingfactor;

		protected internal int numberOfCreatedVMs;

		/// <summary>
		/// Created a new DatacenterBroker object.
		/// </summary>
		/// <param name="name"> name to be associated with this entity (as required by Sim_entity class from
		///             simjava package) </param>
		/// <exception cref="Exception"> the exception
		/// @pre name != null
		/// @post $none </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ContainerDatacenterBroker(String name, double overBookingfactor) throws Exception
		public ContainerDatacenterBroker(string name, double overBookingfactor) : base(name)
		{

			VmListProperty = new List<ContainerVm>();
			ContainerListProperty = new List<Container>();
			VmsCreatedList = new List<ContainerVm>();
			ContainersCreatedList = new List<Container>();
			CloudletListProperty = new List<ContainerCloudlet>();
			CloudletSubmittedList = new List<ContainerCloudlet>();
			CloudletReceivedList = new List<ContainerCloudlet>();
			cloudletsSubmitted = 0;
			VmsRequested = 0;
			VmsAcks = 0;
			ContainersAcks = 0;
			ContainersCreated = 0;
			VmsDestroyed = 0;
			OverBookingfactor = overBookingfactor;
			DatacenterIdsList = new List<int?>();
			DatacenterRequestedIdsList = new List<int?>();
			VmsToDatacentersMap = new Dictionary<int?, int?>();
			ContainersToDatacentersMap = new Dictionary<int?, int?>();
			DatacenterCharacteristicsList = new Dictionary<int?, ContainerDatacenterCharacteristics>();
			NumberOfCreatedVMs = 0;
		}

        /// <summary>
        /// This method is used to send to the broker the list with virtual machines that must be
        /// created.
        /// </summary>
        /// <param name="list"> the list
        /// @pre list !=null
        /// @post $none </param>
        //public virtual void submitVmList<T1>(IList<T1> list) where T1 : ContainerVm
        public virtual void submitVmList(IList<ContainerVm> list)
		{
			((List<ContainerVm>)VmListProperty).AddRange(list);
		}

		/// <summary>
		/// This method is used to send to the broker the list of cloudlets.
		/// </summary>
		/// <param name="list"> the list
		/// @pre list !=null
		/// @post $none </param>
		public virtual void submitCloudletList(IList<ContainerCloudlet> list)
		{
			((List<ContainerCloudlet>)CloudletListProperty).AddRange(list);
		}

		/// <summary>
		/// Specifies that a given cloudlet must run in a specific virtual machine.
		/// </summary>
		/// <param name="cloudletId"> ID of the cloudlet being bount to a vm </param>
		/// <param name="vmId">       the vm id
		/// @pre cloudletId > 0
		/// @pre id > 0
		/// @post $none </param>
		public virtual void bindCloudletToVm(int cloudletId, int vmId)
		{
            // TEST: (fixed) Is the indexer a proper replacement?
            //CloudletList.getById(CloudletListProperty, cloudletId).VmId = vmId;
            CloudletListProperty[cloudletId].VmId = vmId;
            //        Log.printConcatLine("The Vm ID is ",  CloudletList.getById(getCloudletList(), cloudletId).getVmId(), "should be", vmId);
        }
        /// <summary>
        /// Specifies that a given cloudlet must run in a specific virtual machine.
        /// </summary>
        /// <param name="cloudletId"> ID of the cloudlet being bount to a vm </param>
        /// <param name="containerId">       the vm id
        /// @pre cloudletId > 0
        /// @pre id > 0
        /// @post $none </param>
        public virtual void bindCloudletToContainer(int cloudletId, int containerId)
		{
            // TEST: (fixed) Is the indexer a proper replacement?
            //CloudletList.getById(CloudletListProperty, cloudletId).ContainerId = containerId;
            CloudletListProperty[cloudletId].ContainerId = containerId;

        }
		/// <summary>
		/// Processes events available for this Broker.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != null
		/// @post $none </param>
		public override void processEvent(SimEvent ev)
		{
			switch (ev.Tag)
			{
				// Resource characteristics request
				case CloudSimTags.RESOURCE_CHARACTERISTICS_REQUEST:
					processResourceCharacteristicsRequest(ev);
					break;
				// Resource characteristics answer
				case CloudSimTags.RESOURCE_CHARACTERISTICS:
					processResourceCharacteristics(ev);
					break;
				// VM Creation answer
				case CloudSimTags.VM_CREATE_ACK:
					processVmCreate(ev);
					break;
				// New VM Creation answer
				case containerCloudSimTags.VM_NEW_CREATE:
					processNewVmCreate(ev);
					break;
				// A finished cloudlet returned
				case CloudSimTags.CLOUDLET_RETURN:
					processCloudletReturn(ev);
					break;
				// if the simulation finishes
				case CloudSimTags.END_OF_SIMULATION:
					shutdownEntity();
					break;
				case containerCloudSimTags.CONTAINER_CREATE_ACK:
					processContainerCreate(ev);
					break;
				// other unknown tags are processed by this method
				default:
					processOtherEvent(ev);
					break;
			}
		}

		public virtual void processContainerCreate(SimEvent ev)
		{
			int[] data = (int[]) ev.Data;
			int vmId = data[0];
			int containerId = data[1];
			int result = data[2];

			if (result == CloudSimTags.TRUE)
			{
				if (vmId == -1)
				{
					Log.printConcatLine("Error : Where is the VM");
				}
				else
				{
				ContainersToVmsMap[containerId] = vmId;
				ContainersCreatedList.Add(ContainerList.getById(ContainerListProperty, containerId));

	//            ContainerVm p= ContainerVmList.getById(getVmsCreatedList(), vmId);
				int hostId = ContainerVmList.getById(VmsCreatedList, vmId).Host.Id;
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": The Container #", containerId, ", is created on Vm #",vmId, ", On Host#", hostId);
				ContainersCreated = ContainersCreated + 1;
				}
			}
			else
			{
				//Container container = ContainerList.getById(getContainerList(), containerId);
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Failed Creation of Container #", containerId);
			}

			incrementContainersAcks();
			if (ContainersAcks == ContainerListProperty.Count)
			{
				//Log.print(getContainersCreatedList().size() + "vs asli"+getContainerList().size());
				submitCloudlets();
				ContainerListProperty.Clear();
			}

		}

		/// <summary>
		/// Process the return of a request for the characteristics of a PowerDatacenter.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != $null
		/// @post $none </param>
		protected internal virtual void processResourceCharacteristics(SimEvent ev)
		{
			ContainerDatacenterCharacteristics characteristics = (ContainerDatacenterCharacteristics) ev.Data;
			DatacenterCharacteristicsList[characteristics.Id] = characteristics;

			if (DatacenterCharacteristicsList.Count == DatacenterIdsList.Count)
			{
				DatacenterCharacteristicsList.Clear();
				DatacenterRequestedIdsList = new List<int?>();
				createVmsInDatacenter(DatacenterIdsList[0].Value);
			}
		}

		/// <summary>
		/// Process a request for the characteristics of a PowerDatacenter.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != $null
		/// @post $none </param>
		protected internal virtual void processResourceCharacteristicsRequest(SimEvent ev)
		{
			DatacenterIdsList = CloudSim.CloudResourceList;
			DatacenterCharacteristicsList = new Dictionary<int?, ContainerDatacenterCharacteristics>();

			//Log.printConcatLine(CloudSim.clock(), ": ", getName(), ": Cloud Resource List received with ",
	//                getDatacenterIdsList().size(), " resource(s)");

			foreach (int? datacenterId in DatacenterIdsList)
			{
				sendNow(datacenterId.Value, CloudSimTags.RESOURCE_CHARACTERISTICS, Id);
			}
		}
		protected internal virtual void processNewVmCreate(SimEvent ev)
		{
			IDictionary<string, object> map = (IDictionary<string, object>) ev.Data;
			int datacenterId = (int) map["datacenterID"];
			int result = (int) map["result"];
			ContainerVm containerVm = (ContainerVm) map["vm"];
			int vmId = containerVm.Id;
			if (result == CloudSimTags.TRUE)
			{
				VmListProperty.Add(containerVm);
				VmsToDatacentersMap[vmId] = datacenterId;
				VmsCreatedList.Add(containerVm);
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": VM #", vmId, " has been created in Datacenter #", datacenterId, ", Host #", ContainerVmList.getById(VmsCreatedList, vmId).Host.Id);
			}
			else
			{
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Creation of VM #", vmId, " failed in Datacenter #", datacenterId);
			}
		}
		/// <summary>
		/// Process the ack received due to a request for VM creation.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processVmCreate(SimEvent ev)
		{
			int[] data = (int[]) ev.Data;
			int datacenterId = data[0];
			int vmId = data[1];
			int result = data[2];

			if (result == CloudSimTags.TRUE)
			{
				VmsToDatacentersMap[vmId] = datacenterId;
				VmsCreatedList.Add(ContainerVmList.getById(VmListProperty, vmId));
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": VM #", vmId, " has been created in Datacenter #", datacenterId, ", Host #", ContainerVmList.getById(VmsCreatedList, vmId).Host.Id);
				NumberOfCreatedVMs = NumberOfCreatedVMs + 1;
			}
			else
			{
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Creation of VM #", vmId, " failed in Datacenter #", datacenterId);
			}

			incrementVmsAcks();
	//        if (getVmsCreatedList().size() == getVmList().size() - getVmsDestroyed()) {
	//        If we have tried creating all of the vms in the data center, we submit the containers.
			if (VmListProperty.Count == vmsAcks)
			{

				submitContainers();
			}
	//        // all the requested VMs have been created
	//        if (getVmsCreatedList().size() == getVmList().size() - getVmsDestroyed()) {
	//            submitCloudlets();
	//        } else {
	//            // all the acks received, but some VMs were not created
	//            if (getVmsRequested() == getVmsAcks()) {
	//                // find id of the next datacenter that has not been tried
	//                for (int nextDatacenterId : getDatacenterIdsList()) {
	//                    if (!getDatacenterRequestedIdsList().contains(nextDatacenterId)) {
	//                        createVmsInDatacenter(nextDatacenterId);
	//                        return;
	//                    }
	//                }
	//
	//                // all datacenters already queried
	//                if (getVmsCreatedList().size() > 0) { // if some vm were created
	//                    submitCloudlets();
	//                } else { // no vms created. abort
	//                    Log.printLine(CloudSim.clock() + ": " + getName()
	//                            + ": none of the required VMs could be created. Aborting");
	//                    finishExecution();
	//                }
	//            }
	//        }
		}

		/// <summary>
		/// Process a cloudlet return event.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != $null
		/// @post $none </param>
		protected internal virtual void processCloudletReturn(SimEvent ev)
		{
			ContainerCloudlet cloudlet = (ContainerCloudlet) ev.Data;
			CloudletReceivedList.Add(cloudlet);
			Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Cloudlet ", cloudlet.CloudletId, " returned");
			Log.printConcatLine(CloudSim.clock(), ": ", Name, "The number of finished Cloudlets is:", CloudletReceivedList.Count);
			cloudletsSubmitted--;
			if (CloudletListProperty.Count == 0 && cloudletsSubmitted == 0)
			{ // all cloudlets executed
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": All Cloudlets executed. Finishing...");
				clearDatacenters();
				finishExecution();
			}
			else
			{ // some cloudlets haven't finished yet
				if (CloudletListProperty.Count > 0 && cloudletsSubmitted == 0)
				{
					// all the cloudlets sent finished. It means that some bount
					// cloudlet is waiting its VM be created
					clearDatacenters();
					createVmsInDatacenter(0);
				}

			}
		}

		/// <summary>
		/// Overrides this method when making a new and different type of Broker. This method is called
		/// by  for incoming unknown tags.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processOtherEvent(SimEvent ev)
		{
			if (ev == null)
			{
				Log.printConcatLine(Name, ".processOtherEvent(): ", "Error - an event is null.");
				return;
			}

			Log.printConcatLine(Name, ".processOtherEvent(): Error - event unknown by this DatacenterBroker.");
		}

		/// <summary>
		/// Create the virtual machines in a datacenter.
		/// </summary>
		/// <param name="datacenterId"> Id of the chosen PowerDatacenter
		/// @pre $none
		/// @post $none </param>
		protected internal virtual void createVmsInDatacenter(int datacenterId)
		{
			// send as much vms as possible for this datacenter before trying the next one
			int requestedVms = 0;
			string datacenterName = CloudSim.getEntityName(datacenterId);
			foreach (ContainerVm vm in VmListProperty)
			{
				if (!VmsToDatacentersMap.ContainsKey(vm.Id))
				{
					Log.printLine(string.Format("{0}: {1}: Trying to Create VM #{2:D} in {3}", CloudSim.clock(), Name, vm.Id, datacenterName));
					sendNow(datacenterId, CloudSimTags.VM_CREATE_ACK, vm);
					requestedVms++;
				}
			}

			DatacenterRequestedIdsList.Add(datacenterId);

			VmsRequested = requestedVms;
			VmsAcks = 0;
		}

		/// <summary>
		/// Submit cloudlets to the created VMs.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		protected internal virtual void submitCloudlets()
		{
			int containerIndex = 0;
			IList<ContainerCloudlet> successfullySubmitted = new List<ContainerCloudlet>();
			foreach (ContainerCloudlet cloudlet in CloudletListProperty)
			{
				//Log.printLine("Containers Created" + getContainersCreated());
				if (containerIndex < ContainersCreated)
				{
						//Log.printLine("Container Index" + containerIndex);
	//                    int containerId = getContainersCreatedList().get(containerIndex).getId();
	//                    bindCloudletToContainer(cloudlet.getCloudletId(), containerId);
					if (ContainersToVmsMap[cloudlet.ContainerId] != null)
					{
						int vmId = ContainersToVmsMap[cloudlet.ContainerId].Value;
	//                    bindCloudletToVm(cloudlet.getCloudletId(), vmId);
						cloudlet.VmId = vmId;
	//                    if(cloudlet.getVmId() != vmId){
	//                        Log.printConcatLine("The cloudlet Vm Id is ", cloudlet.getVmId(), "It should be", vmId);
	//
	//                    }
						containerIndex++;
						sendNow(DatacenterIdsList[0].Value, CloudSimTags.CLOUDLET_SUBMIT, cloudlet);
						cloudletsSubmitted++;
						CloudletSubmittedList.Add(cloudlet);
						successfullySubmitted.Add(cloudlet);
					}


					//Log.printLine("Container Id" + containerId);

					//Log.printConcatLine("VM ID is: ",cloudlet.getVmId(), "Container ID:", cloudlet.getContainerId(), "cloudletId:", cloudlet.getCloudletId());

	//                cloudlet.setVmId(v.getId());
					// if user didn't bind this cloudlet and it has not been executed yet
	//            if (cloudlet.getContainerId() == -1) {
	//                Log.print("User has forgotten to bound the cloudlet to container");
	//            } else { // submit to the specific vm
	//                vm = ContainerVmList.getById(getVmsCreatedList(), cloudlet.getVmId());
	//                if (vm == null) { // vm was not created
	//                    if (!Log.isDisabled()) {
	//                        Log.printConcatLine(CloudSim.clock(), ": ", getName(), ": Postponing execution of cloudlet ",
	//                                cloudlet.getCloudletId(), ": bount VM not available");
	//                    }
	//                    continue;
	//                }
	//            }
	//
	//            if (!Log.isDisabled()) {
	//                Log.printConcatLine(CloudSim.clock(), ": ", getName(), ": Sending cloudlet ",
	//                        cloudlet.getCloudletId(), " to VM #", cloudlet.getContainerId());
	//            }


	//            containerIndex = (containerIndex + 1) % getVmsCreatedList().size();

	//          int vmIndex = 0;
	//        List<ContainerCloudlet> successfullySubmitted = new ArrayList<ContainerCloudlet>();
	//        for (ContainerCloudlet cloudlet : getCloudletList()) {
	//            ContainerVm vm;
	//            // if user didn't bind this cloudlet and it has not been executed yet
	//            if (cloudlet.getVmId() == -1) {
	//                vm = getVmsCreatedList().get(vmIndex);
	//            } else { // submit to the specific vm
	//                vm = ContainerVmList.getById(getVmsCreatedList(), cloudlet.getVmId());
	//                if (vm == null) { // vm was not created
	//                    if (!Log.isDisabled()) {
	//                        Log.printConcatLine(CloudSim.clock(), ": ", getName(), ": Postponing execution of cloudlet ",
	//                                cloudlet.getCloudletId(), ": bount VM not available");
	//                    }
	//                    continue;
	//                }
	//            }
	//
	//            if (!Log.isDisabled()) {cloudlet.getCloudletId()
	//                Log.printConcatLine(CloudSim.clock(), ": ", getName(), ": Sending cloudlet ",
	//                        cloudlet.getCloudletId(), " to VM #", vm.getId());
	//            }
	//
	//            cloudlet.setVmId(vm.getId());
	//            sendNow(getVmsToDatacentersMap().get(vm.getId()), CloudSimTags.CLOUDLET_SUBMIT, cloudlet);
	//            cloudletsSubmitted++;
	//            vmIndex = (vmIndex + 1) % getVmsCreatedList().size();
	//            getCloudletSubmittedList().add(cloudlet);
	//            successfullySubmitted.add(cloudlet);
				}
			}

            // remove submitted cloudlets from waiting list
            //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
            //CloudletList.removeAll(successfullySubmitted);
            //CloudletListProperty.removeAll(successfullySubmitted);
            CloudletListProperty.Clear();
            successfullySubmitted.Clear();
		}

		/// <summary>
		///getOverBookingfactor
		/// Destroy the virtual machines running in datacenters.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		protected internal virtual void clearDatacenters()
		{
			foreach (ContainerVm vm in VmsCreatedList)
			{
	//            Log.printConcatLine(CloudSim.clock(), ": " + getName(), ": Destroying VM #", vm.getId());
				sendNow(VmsToDatacentersMap[vm.Id].Value, CloudSimTags.VM_DESTROY, vm);
			}

			VmsCreatedList.Clear();
		}


		/// 
		protected internal virtual void submitContainers()
		{
			IList<Container> successfullySubmitted = new List<Container>();
			int i = 0;
			foreach (Container container in ContainerListProperty)
			{
				ContainerCloudlet cloudlet = CloudletListProperty[i];
					//Log.printLine("Containers Created" + getContainersCreated());

					if (cloudlet.UtilizationModelCpu is UtilizationModelPlanetLabInMemory)
					{
						UtilizationModelPlanetLabInMemory temp = (UtilizationModelPlanetLabInMemory) cloudlet.UtilizationModelCpu;
						double[] cloudletUsage = temp.Data;
                    // TODO: Figure out Percentile
                    //Percentile percentile = new Percentile();
                    //double percentileUsage = percentile.evaluate(cloudletUsage, OverBookingfactor);
                    //Log.printLine("Container Index" + containerIndex);
                    //double newmips = percentileUsage * container.Mips;
                    //                    double newmips = percentileUsage * container.getMips();
                    //                    double maxUsage = Doubles.max(cloudletUsage);
                    //                    double newmips = maxUsage * container.getMips();
                    //container.WorkloadMips = newmips;
                    //                    bindCloudletToContainer(cloudlet.getCloudletId(), container.getId());
                    cloudlet.ContainerId = container.Id;
						if (cloudlet.ContainerId != container.Id)
						{
	//                        Log.printConcatLine("Binding Cloudlet: ", cloudlet.getCloudletId(), "To Container: ",container.getId() , "Now it is", cloudlet.getContainerId());
						}

					}
				i++;

			}

			foreach (Container container in ContainerListProperty)
			{
				successfullySubmitted.Add(container);

			}
			sendNow(DatacenterIdsList[0].Value, containerCloudSimTags.CONTAINER_SUBMIT, successfullySubmitted);

	//        List<Container> successfullySubmitted = new ArrayList<>();
	//        for (Container container : getContainerList()) {
	//
	//            if (!Log.isDisabled()) {
	//                Log.printConcatLine(CloudSim.clock(), ": ", getName(), ": Sending container ",
	//                        container.getId(), " to Datacenter");
	//            }
	//            cloudletsSubmitted++;
	//            vmIndex = (vmIndex + 1) % getVmsCreatedList().size();
	//            getCloudletSubmittedList().add(cloudlet);
	//            successfullySubmitted.add(cloudlet);
	//        }

			// remove submitted cloudlets from waiting list
		}


		/// <summary>
		/// Send an internal event communicating the end of the simulation.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		protected internal virtual void finishExecution()
		{
			sendNow(Id, CloudSimTags.END_OF_SIMULATION);
		}

		/*
		 * (non-Javadoc)
		 * @see cloudsim.core.SimEntity#shutdownEntity()
		 */
		public override void shutdownEntity()
		{
			Log.printConcatLine(Name, " is shutting down...");
		}

		/*
		 * (non-Javadoc)
		 * @see cloudsim.core.SimEntity#startEntity()
		 */
		public override void startEntity()
		{
			Log.printConcatLine(Name, " is starting...");
			schedule(Id, 0, CloudSimTags.RESOURCE_CHARACTERISTICS_REQUEST);
		}

        /// <summary>
        /// Gets the vm list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the vm list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends ContainerVm> java.util.List<T> getVmList()
        //public virtual IList<T> getVmList<T>() where T : ContainerVm
        public virtual IList<ContainerVm> VmListProperty
        {
			get
			{
				return (IList<ContainerVm>) vmList;
			}
			set
			{
				this.vmList = value;
			}
		}


        /// <summary>
        /// Gets the cloudlet list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the cloudlet list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends ContainerCloudlet> java.util.List<T> getCloudletList()
        //public virtual IList<T> getCloudletList<T>() where T : ContainerCloudlet
        public virtual IList<ContainerCloudlet> CloudletListProperty
        {
			get
			{
				return (IList<ContainerCloudlet>) cloudletList;
			}
			set
			{
				this.cloudletList = value;
			}
		}


        /// <summary>
        /// Gets the cloudlet submitted list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the cloudlet submitted list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends ContainerCloudlet> java.util.List<T> getCloudletSubmittedList()
        //public virtual IList<T> getCloudletSubmittedList<T>() where T : ContainerCloudlet
        public virtual IList<ContainerCloudlet> CloudletSubmittedList
        {
			get
			{
				return (IList<ContainerCloudlet>) cloudletSubmittedList;
			}
			set
			{
				this.cloudletSubmittedList = value;
			}
		}


        /// <summary>
        /// Gets the cloudlet received list.
        /// </summary>
        /// @param <T> the generic type </param>
        /// <returns> the cloudlet received list </returns>
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends ContainerCloudlet> java.util.List<T> getCloudletReceivedList()
        //public virtual IList<T> getCloudletReceivedList<T>() where T : ContainerCloudlet
        public virtual IList<ContainerCloudlet> CloudletReceivedList
        {
			get
			{
				return (IList<ContainerCloudlet>) cloudletReceivedList;
			}
			set
			{
				this.cloudletReceivedList = value;
			}
		}


		/// <summary>
		/// Gets the vm list.
		/// </summary>
		/// @param <T> the generic type </param>
		/// <returns> the vm list </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends ContainerVm> java.util.List<T> getVmsCreatedList()
		//public virtual IList<T> getVmsCreatedList<T>() where T : ContainerVm
        public virtual IList<ContainerVm> VmsCreatedList
        {
			get
			{
				return (IList<ContainerVm>) vmsCreatedList;
			}
			set
			{
				this.vmsCreatedList = value;
			}
		}


		/// <summary>
		/// Gets the vms requested.
		/// </summary>
		/// <returns> the vms requested </returns>
		protected internal virtual int VmsRequested
		{
			get
			{
				return vmsRequested;
			}
			set
			{
				this.vmsRequested = value;
			}
		}


		/// <summary>
		/// Gets the vms acks.
		/// </summary>
		/// <returns> the vms acks </returns>
		protected internal virtual int VmsAcks
		{
			get
			{
				return vmsAcks;
			}
			set
			{
				this.vmsAcks = value;
			}
		}


		/// <summary>
		/// Increment vms acks.
		/// </summary>
		protected internal virtual void incrementVmsAcks()
		{
			vmsAcks++;
		}
		/// <summary>
		/// Increment vms acks.
		/// </summary>
		protected internal virtual void incrementContainersAcks()
		{
			ContainersAcks = ContainersAcks + 1;
		}

		/// <summary>
		/// Gets the vms destroyed.
		/// </summary>
		/// <returns> the vms destroyed </returns>
		protected internal virtual int VmsDestroyed
		{
			get
			{
				return vmsDestroyed;
			}
			set
			{
				this.vmsDestroyed = value;
			}
		}


		/// <summary>
		/// Gets the datacenter ids list.
		/// </summary>
		/// <returns> the datacenter ids list </returns>
		protected internal virtual IList<int?> DatacenterIdsList
		{
			get
			{
				return datacenterIdsList;
			}
			set
			{
				this.datacenterIdsList = value;
			}
		}


		/// <summary>
		/// Gets the vms to datacenters map.
		/// </summary>
		/// <returns> the vms to datacenters map </returns>
		protected internal virtual IDictionary<int?, int?> VmsToDatacentersMap
		{
			get
			{
				return vmsToDatacentersMap;
			}
			set
			{
				this.vmsToDatacentersMap = value;
			}
		}


		/// <summary>
		/// Gets the datacenter characteristics list.
		/// </summary>
		/// <returns> the datacenter characteristics list </returns>
		protected internal virtual IDictionary<int?, ContainerDatacenterCharacteristics> DatacenterCharacteristicsList
		{
			get
			{
				return datacenterCharacteristicsList;
			}
			set
			{
				this.datacenterCharacteristicsList = value;
			}
		}


		/// <summary>
		/// Gets the datacenter requested ids list.
		/// </summary>
		/// <returns> the datacenter requested ids list </returns>
		protected internal virtual IList<int?> DatacenterRequestedIdsList
		{
			get
			{
				return datacenterRequestedIdsList;
			}
			set
			{
				this.datacenterRequestedIdsList = value;
			}
		}


        //------------------------------------------------

        //public virtual IList<T> getContainerList<T>() where T : Container
        public virtual IList<Container> ContainerListProperty
        {
			get
			{
    
				return (IList<Container>) containerList;
			}
			set
			{
				this.containerList = value;
			}
		}

        /// <summary>
        /// This method is used to send to the broker the list with virtual machines that must be
        /// created.
        /// </summary>
        /// <param name="list"> the list
        /// @pre list !=null
        /// @post $none </param>
        //public virtual void submitContainerList<T1>(IList<T1> list) where T1 : Container
        public virtual void submitContainerList<Container>(IList<Container> list)
        {
			((List<Container>)ContainerListProperty).AddRange(list);
		}


		public virtual IDictionary<int?, int?> ContainersToVmsMap
		{
			get
			{
				return containersToDatacentersMap;
			}
		}

		public virtual IDictionary<int?, int?> ContainersToDatacentersMap
		{
			set
			{
				this.containersToDatacentersMap = value;
			}
		}

        //public virtual IList<T> getContainersCreatedList<T>() where T : Container
        public virtual IList<Container> ContainersCreatedList
        {
			get
			{
				return (IList<Container>) containersCreatedList;
			}
			set
			{
				this.containersCreatedList = value;
			}
		}


		public virtual int ContainersAcks
		{
			get
			{
				return containersAcks;
			}
			set
			{
				this.containersAcks = value;
			}
		}


		public virtual int ContainersCreated
		{
			get
			{
				return containersCreated;
			}
			set
			{
				this.containersCreated = value;
			}
		}


		public virtual double OverBookingfactor
		{
			get
			{
				return overBookingfactor;
			}
			set
			{
				this.overBookingfactor = value;
			}
		}


		public virtual int NumberOfCreatedVMs
		{
			get
			{
				return numberOfCreatedVMs;
			}
			set
			{
				this.numberOfCreatedVMs = value;
			}
		}

	}



}