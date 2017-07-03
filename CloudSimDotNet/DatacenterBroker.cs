using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using SimEntity = org.cloudbus.cloudsim.core.SimEntity;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
    using CloudletList = org.cloudbus.cloudsim.lists.CloudletList;
    using VmList = org.cloudbus.cloudsim.lists.VmList;
    using CloudSimLib.util;

    /// <summary>
    /// DatacentreBroker represents a broker acting on behalf of a user. It hides VM management, as vm
    /// creation, submission of cloudlets to VMs and destruction of VMs.
    /// 
    /// @author Rodrigo N. Calheiros
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class DatacenterBroker : SimEntity
	{

		/// <summary>
		/// The list of VMs submitted to be managed by the broker. </summary>
		protected internal IList<Vm> vmList;

		/// <summary>
		/// The list of VMs created by the broker. </summary>
		protected internal IList<Vm> vmsCreatedList;

		/// <summary>
		/// The list of cloudlet submitted to the broker. </summary>
		/// <seealso cref= #submitCloudletList(java.util.List)  </seealso>
		protected internal IList<Cloudlet> cloudletList;

		/// <summary>
		/// The list of submitted cloudlets. </summary>
		protected internal IList<Cloudlet> cloudletSubmittedList;

		/// <summary>
		/// The list of received cloudlet. </summary>
		protected internal IList<Cloudlet> cloudletReceivedList;

		/// <summary>
		/// The number of submitted cloudlets. </summary>
		protected internal int cloudletsSubmitted;

		/// <summary>
		/// The number of requests to create VM. </summary>
		protected internal int vmsRequested;

		/// <summary>
		/// The number of acknowledges (ACKs) sent in response to
		/// VM creation requests. 
		/// </summary>
		protected internal int vmsAcks;

		/// <summary>
		/// The number of destroyed VMs. </summary>
		protected internal int vmsDestroyed;

		/// <summary>
		/// The id's list of available datacenters. </summary>
		protected internal IList<int?> datacenterIdsList;

		/// <summary>
		/// The list of datacenters where was requested to place VMs. </summary>
		protected internal IList<int?> datacenterRequestedIdsList;

		/// <summary>
		/// The vms to datacenters map, where each key is a VM id
		/// and each value is the datacenter id whwere the VM is placed. 
		/// </summary>
		protected internal IDictionary<int?, int?> vmsToDatacentersMap;

		/// <summary>
		/// The datacenter characteristics map where each key
		/// is a datacenter id and each value is its characteristics.. 
		/// </summary>
		protected internal IDictionary<int?, DatacenterCharacteristics> datacenterCharacteristicsList;

		/// <summary>
		/// Created a new DatacenterBroker object.
		/// </summary>
		/// <param name="name"> name to be associated with this entity (as required by <seealso cref="SimEntity"/> class) </param>
		/// <exception cref="Exception"> the exception
		/// @pre name != null
		/// @post $none </exception>
		public DatacenterBroker(string name) : base(name)
		{

			VmListProperty = new List<Vm>();
            VmsCreatedListProperty = new List<Vm>();
            CloudletListProperty = new List<Cloudlet>();
            CloudletSubmittedListProperty = new List<Cloudlet>();
            CloudletReceivedListProperty = new List<Cloudlet>();

			cloudletsSubmitted = 0;
			VmsRequested = 0;
			VmsAcks = 0;
			VmsDestroyed = 0;

			DatacenterIdsList = new List<int?>();
			DatacenterRequestedIdsList = new List<int?>();
			VmsToDatacentersMap = new Dictionary<int?, int?>();
			DatacenterCharacteristicsList = new Dictionary<int?, DatacenterCharacteristics>();
		}

        /// <summary>
        /// This method is used to send to the broker the list with virtual machines that must be
        /// created.
        /// </summary>
        /// <param name="list"> the list
        /// @pre list !=null
        /// @post $none </param>
        //public virtual void submitVmList<T1>(IList<T1> list) where T1 : Vm
        public virtual void submitVmList(IList<Vm> list)
        {
			((List<Vm>)VmListProperty).AddRange(list);
		}

        /// <summary>
        /// This method is used to send to the broker the list of cloudlets.
        /// </summary>
        /// <param name="list"> the list
        /// @pre list !=null
        /// @post $none
        /// 
        /// @todo The name of the method is confused with the <seealso cref="#submitCloudlets()"/>,
        /// that in fact submit cloudlets to VMs. The term "submit" is being used
        /// ambiguously. The method <seealso cref="#submitCloudlets()"/> would be named "sendCloudletsToVMs"
        /// 
        /// The method <seealso cref="#submitVmList(java.util.List)"/> may have
        /// be checked too. </param>
        //public virtual void submitCloudletList<T1>(IList<T1> list) where T1 : Cloudlet
        public virtual void submitCloudletList(IList<Cloudlet> list)
        {
			((List<Cloudlet>)CloudletListProperty).AddRange(list);
		}

		/// <summary>
		/// Specifies that a given cloudlet must run in a specific virtual machine.
		/// </summary>
		/// <param name="cloudletId"> ID of the cloudlet being bount to a vm </param>
		/// <param name="vmId"> the vm id
		/// @pre cloudletId > 0
		/// @pre id > 0
		/// @post $none </param>
		public virtual void bindCloudletToVm(int cloudletId, int vmId)
		{
			CloudletList.getById(CloudletListProperty, cloudletId).VmId = vmId;
		}

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
				// A finished cloudlet returned
				case CloudSimTags.CLOUDLET_RETURN:
					processCloudletReturn(ev);
					break;
				// if the simulation finishes
				case CloudSimTags.END_OF_SIMULATION:
					shutdownEntity();
					break;
				// other unknown tags are processed by this method
				default:
					processOtherEvent(ev);
					break;
			}
		}

		/// <summary>
		/// Process the return of a request for the characteristics of a Datacenter.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != $null
		/// @post $none </param>
		protected internal virtual void processResourceCharacteristics(SimEvent ev)
		{
			DatacenterCharacteristics characteristics = (DatacenterCharacteristics) ev.Data;
			DatacenterCharacteristicsList[characteristics.Id] = characteristics;

			if (DatacenterCharacteristicsList.Count == DatacenterIdsList.Count)
			{
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
			DatacenterCharacteristicsList = new Dictionary<int?, DatacenterCharacteristics>();

			Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Cloud Resource List received with ", DatacenterIdsList.Count, " resource(s)");

			foreach (int? datacenterId in DatacenterIdsList)
			{
				sendNow(datacenterId.Value, CloudSimTags.RESOURCE_CHARACTERISTICS, Id);
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
                VmsCreatedListProperty.Add(VmList.getById(VmListProperty, vmId));
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": VM #", vmId, " has been created in Datacenter #", datacenterId, ", Host #", VmList.getById(VmsCreatedListProperty, vmId).Host.Id);
			}
			else
			{
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Creation of VM #", vmId, " failed in Datacenter #", datacenterId);
			}

			incrementVmsAcks();

			// all the requested VMs have been created
			if (VmsCreatedListProperty.Count == VmListProperty.Count - VmsDestroyed)
			{
				submitCloudlets();
			}
			else
			{
				// all the acks received, but some VMs were not created
				if (VmsRequested == VmsAcks)
				{
					// find id of the next datacenter that has not been tried
					foreach (int nextDatacenterId in DatacenterIdsList)
					{
						if (!DatacenterRequestedIdsList.Contains(nextDatacenterId))
						{
							createVmsInDatacenter(nextDatacenterId);
							return;
						}
					}

					// all datacenters already queried
					if (VmsCreatedListProperty.Count > 0)
					{ // if some vm were created
						submitCloudlets();
					}
					else
					{ // no vms created. abort
						Log.printLine(CloudSim.clock() + ": " + Name + ": none of the required VMs could be created. Aborting");
						finishExecution();
					}
				}
			}
		}

		/// <summary>
		/// Process a cloudlet return event.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != $null
		/// @post $none </param>
		protected internal virtual void processCloudletReturn(SimEvent ev)
		{
			Cloudlet cloudlet = (Cloudlet) ev.Data;
            CloudletReceivedListProperty.Add(cloudlet);
			Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Cloudlet ", cloudlet.CloudletId, " received");
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
		/// Process non-default received events that aren't processed by
		/// the <seealso cref="#processEvent(org.cloudbus.cloudsim.core.SimEvent)"/> method.
		/// This method should be overridden by subclasses in other to process
		/// new defined events.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != null
		/// @post $none
		/// @todo to ensure the method will be overridden, it should be defined 
		/// as abstract in a super class from where new brokers have to be extended. </param>
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
		/// Create the submitted virtual machines in a datacenter.
		/// </summary>
		/// <param name="datacenterId"> Id of the chosen Datacenter
		/// @pre $none
		/// @post $none </param>
		/// <seealso cref= #submitVmList(java.util.List)  </seealso>
		protected internal virtual void createVmsInDatacenter(int datacenterId)
		{
			// send as much vms as possible for this datacenter before trying the next one
			int requestedVms = 0;
			string datacenterName = CloudSim.getEntityName(datacenterId);
			foreach (Vm vm in VmListProperty)
			{
				if (!VmsToDatacentersMap.ContainsKey(vm.Id))
				{
					Log.printLine(CloudSim.clock() + ": " + Name + ": Trying to Create VM #" + vm.Id + " in " + datacenterName);
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
		/// @post $none </summary>
		/// <seealso cref= #submitCloudletList(java.util.List)  </seealso>
		protected internal virtual void submitCloudlets()
		{
			int vmIndex = 0;
			IList<Cloudlet> successfullySubmitted = new List<Cloudlet>();
			foreach (Cloudlet cloudlet in CloudletListProperty)
			{
				Vm vm;
				// if user didn't bind this cloudlet and it has not been executed yet
				if (cloudlet.VmId == -1)
				{
					vm = VmsCreatedListProperty[vmIndex];
				}
				else
				{ // submit to the specific vm
					vm = VmList.getById(VmsCreatedListProperty, cloudlet.VmId);
					if (vm == null)
					{ // vm was not created
						if (!Log.Disabled)
						{
							Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Postponing execution of cloudlet ", cloudlet.CloudletId, ": bount VM not available");
						}
						continue;
					}
				}

				if (!Log.Disabled)
				{
					Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Sending cloudlet ", cloudlet.CloudletId, " to VM #", vm.Id);
				}

				cloudlet.VmId = vm.Id;
				sendNow(VmsToDatacentersMap[vm.Id].Value, CloudSimTags.CLOUDLET_SUBMIT, cloudlet);
				cloudletsSubmitted++;
				vmIndex = (vmIndex + 1) % VmsCreatedListProperty.Count;
				CloudletSubmittedListProperty.Add(cloudlet);
				successfullySubmitted.Add(cloudlet);
			}

            // remove submitted cloudlets from waiting list
            //JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'removeAll' method:
            //CloudletList.removeAll(successfullySubmitted);
            // TEST: (fixed) RemoveAll
            CloudletListProperty.RemoveAll<Cloudlet>(successfullySubmitted);
        }

		/// <summary>
		/// Destroy all virtual machines running in datacenters.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		protected internal virtual void clearDatacenters()
		{
			foreach (Vm vm in VmsCreatedListProperty)
			{
				Log.printConcatLine(CloudSim.clock(), ": " + Name, ": Destroying VM #", vm.Id);
				sendNow(VmsToDatacentersMap[vm.Id].Value, CloudSimTags.VM_DESTROY, vm);
			}

            VmsCreatedListProperty.Clear();
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

		public override void shutdownEntity()
		{
			Log.printConcatLine(Name, " is shutting down...");
		}

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
        public virtual IList<Vm> VmListProperty
        {
			get
			{
				return (IList<Vm>) vmList;
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
        public virtual IList<Cloudlet> CloudletListProperty
        {
			get
			{
				return (IList<Cloudlet>) cloudletList;
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
        public virtual IList<Cloudlet> CloudletSubmittedListProperty
        {
			get
			{
				return (IList<Cloudlet>) cloudletSubmittedList;
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
        public virtual IList<Cloudlet> CloudletReceivedListProperty
        {
			get
			{
				return (IList<Cloudlet>) cloudletReceivedList;
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
        public virtual IList<Vm> VmsCreatedListProperty
        {
			get
			{
				return (IList<Vm>) vmsCreatedList;
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
		/// Increment the number of acknowledges (ACKs) sent in response
		/// to requests of VM creation.
		/// </summary>
		protected internal virtual void incrementVmsAcks()
		{
			vmsAcks++;
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
		protected internal virtual IDictionary<int?, DatacenterCharacteristics> DatacenterCharacteristicsList
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


	}

}