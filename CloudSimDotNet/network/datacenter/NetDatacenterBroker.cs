using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.network.datacenter
{
    using System.Diagnostics;
    using System.Linq;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using SimEntity = org.cloudbus.cloudsim.core.SimEntity;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
    using UniformDistr = org.cloudbus.cloudsim.distributions.UniformDistr;
    using VmList = org.cloudbus.cloudsim.lists.VmList;

    /// <summary>
    /// NetDatacentreBroker represents a broker acting on behalf of Datacenter provider. It hides VM
    /// management, as vm creation, submission of cloudlets to these VMs and destruction of VMs. <br/>
    /// <tt>NOTE</tt>: This class is an example only. It works on behalf of a provider not for users. 
    /// One has to implement interaction with user broker to this broker.
    /// 
    /// @author Saurabh Kumar Garg
    /// @since CloudSim Toolkit 3.0
    /// @todo The class is not a broker acting on behalf of users, but on behalf
    /// of a provider. Maybe this distinction would be explicit by 
    /// different class hierarchy, such as UserDatacenterBroker and ProviderDatacenterBroker.
    /// </summary>
    public class NetDatacenterBroker : SimEntity
	{

        // TEST: (fixed) remove unnecessary variables

        /// <summary>
        /// The list of submitted VMs. </summary>
        //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
        //ORIGINAL LINE: private java.util.List<? extends org.cloudbus.cloudsim.Vm> vmList;
        private IList<Vm> vmList;

		/// <summary>
		/// The list of created VMs. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends org.cloudbus.cloudsim.Vm> vmsCreatedList;
		private IList<NetworkVm> vmsCreatedList;

		/// <summary>
		/// The list of submitted <seealso cref="NetworkCloudlet NetworkCloudlets"/>. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends NetworkCloudlet> cloudletList;
		private IList<NetworkCloudlet> cloudletList;

		/// <summary>
		/// The list of submitted <seealso cref="AppCloudlet AppCloudlets"/>. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends AppCloudlet> appCloudletList;
		private IList<AppCloudlet> appCloudletList;

		/// <summary>
		/// The list of submitted <seealso cref="AppCloudlet AppCloudlets"/>.
		/// @todo attribute appears to be redundant with <seealso cref="#appCloudletList"/>
		/// </summary>
		private readonly IDictionary<int?, int?> appCloudletRecieved;

		/// <summary>
		/// The list of submitted <seealso cref="Cloudlet Cloudlets"/>.
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends org.cloudbus.cloudsim.Cloudlet> cloudletSubmittedList;
		private IList<Cloudlet> cloudletSubmittedList;

		/// <summary>
		/// The list of received <seealso cref="Cloudlet Cloudlets"/>.
		/// @todo attribute appears to be redundant with <seealso cref="#cloudletSubmittedList"/>
		/// </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: private java.util.List<? extends org.cloudbus.cloudsim.Cloudlet> cloudletReceivedList;
		private IList<Cloudlet> cloudletReceivedList;

		/// <summary>
		/// The number of submitted cloudlets. </summary>
		private int cloudletsSubmitted;

		/// <summary>
		/// The number of VMs requested. </summary>
		private int vmsRequested;

		/// <summary>
		/// The acks sent to VMs. </summary>
		private int vmsAcks;

		/// <summary>
		/// The number of VMs destroyed. </summary>
		private int vmsDestroyed;

		/// <summary>
		/// The list of datacenter IDs. </summary>
		private IList<int?> datacenterIdsList;

		/// <summary>
		/// The datacenter requested IDs list. 
		/// @todo attribute appears to be redundant with <seealso cref="#datacenterIdsList"/>
		/// </summary>
		private IList<int?> datacenterRequestedIdsList;

		/// <summary>
		/// The VMs to datacenters map where each key is a VM id
		/// and the corresponding value is the datacenter where the VM is placed. 
		/// </summary>
		private IDictionary<int?, int?> vmsToDatacentersMap;

		/// <summary>
		/// The datacenter characteristics map where each key 
		/// is the datacenter id and each value is the datacenter itself. 
		/// </summary>
		private IDictionary<int?, DatacenterCharacteristics> datacenterCharacteristicsList;

		public static NetworkDatacenter linkDC;

		public bool createvmflag = true;

		public static int cachedcloudlet = 0;

		/// <summary>
		/// Creates a new DatacenterBroker object.
		/// </summary>
		/// <param name="name"> name to be associated with this entity
		/// </param>
		/// <exception cref="Exception"> the exception
		/// 
		/// @pre name != null
		/// @post $none </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NetDatacenterBroker(String name) throws Exception
		public NetDatacenterBroker(string name) : base(name)
		{

			VmListProperty = new List<NetworkVm>();
            VmsCreatedListProperty = new List<NetworkVm>();
            CloudletListProperty = new List<NetworkCloudlet>();
            AppCloudletListProperty = new List<AppCloudlet>();
            CloudletSubmittedListProperty = new List<Cloudlet>();
            CloudletReceivedListProperty = new List<Cloudlet>();
			appCloudletRecieved = new Dictionary<int?, int?>();

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
		/// Sends to the broker the list with virtual machines that must be
		/// created.
		/// </summary>
		/// <param name="list"> the list
		/// 
		/// @pre list !=null
		/// @post $none </param>
		public virtual void submitVmList(IList<Vm> list)
		{
			((List<Vm>)VmListProperty).AddRange(list);
		}

		/// <summary>
		/// Sends to the broker the list of cloudlets.
		/// </summary>
		/// <param name="list"> the list
		/// 
		/// @pre list !=null
		/// @post $none </param>
		public virtual void submitCloudletList(IList<NetworkCloudlet> list)
		{
			((List<NetworkCloudlet>)CloudletListProperty).AddRange(list);
		}

		public static NetworkDatacenter LinkDC
		{
            get
            {
                return linkDC;
            }

			set
			{
				linkDC = value;
			}
		}

		/// <summary>
		/// Processes events available for this Broker.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// 
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

				// A finished cloudlet returned
				case CloudSimTags.CLOUDLET_RETURN:
					processCloudletReturn(ev);
					break;
				// if the simulation finishes
				case CloudSimTags.END_OF_SIMULATION:
					shutdownEntity();
					break;
				case CloudSimTags.NextCycle:
					if (NetworkConstants.BASE)
					{
						createVmsInDatacenterBase(linkDC.Id);
					}

					break;
				// other unknown tags are processed by this method
				default:
					processOtherEvent(ev);
					break;
			}
		}

		/// <summary>
		/// Processes the return of a request for the characteristics of a Datacenter.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// 
		/// @pre ev != $null
		/// @post $none </param>
		protected internal virtual void processResourceCharacteristics(SimEvent ev)
		{
			DatacenterCharacteristics characteristics = (DatacenterCharacteristics) ev.Data;
			DatacenterCharacteristicsList[characteristics.Id] = characteristics;

			if (DatacenterCharacteristicsList.Count == DatacenterIdsList.Count)
			{
				DatacenterRequestedIdsList = new List<int?>();
				createVmsInDatacenterBase(DatacenterIdsList[0].Value);
			}
		}

		/// <summary>
		/// Processes a request for the characteristics of a Datacenter.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// 
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
		/// Processes the ack received due to a request for VM creation.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// 
		/// @pre ev != null
		/// @post $none </param>

		/// <summary>
		/// Processes a cloudlet return event.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// 
		/// @pre ev != $null
		/// @post $none </param>
		protected internal virtual void processCloudletReturn(SimEvent ev)
		{
			Cloudlet cloudlet = (Cloudlet) ev.Data;
			CloudletReceivedListProperty.Add(cloudlet);
			cloudletsSubmitted--;
			// all cloudlets executed
			if (CloudletListProperty.Count == 0 && cloudletsSubmitted == 0 && NetworkConstants.iteration > 10)
			{
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": All Cloudlets executed. Finishing...");
				clearDatacenters();
				finishExecution();
			}
			else
			{ // some cloudlets haven't finished yet
				if (AppCloudletListProperty.Count > 0 && cloudletsSubmitted == 0)
				{
					// all the cloudlets sent finished. It means that some bount
					// cloudlet is waiting its VM be created
					clearDatacenters();
					createVmsInDatacenterBase(0);
				}

			}
		}

		/// <summary>
		/// Processes non-default received events that aren't processed by
		/// the <seealso cref="#processEvent(org.cloudbus.cloudsim.core.SimEvent)"/> method.
		/// This method should be overridden by subclasses in other to process
		/// new defined events.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// 
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processOtherEvent(SimEvent ev)
		{
			if (ev == null)
			{
				Log.printConcatLine(Name, ".processOtherEvent(): Error - an event is null.");
				return;
			}

			Log.printConcatLine(Name, ".processOtherEvent(): ", "Error - event unknown by this DatacenterBroker.");
		}

		/// <summary>
		/// Creates virtual machines in a datacenter and submit/schedule cloudlets to them.
		/// </summary>
		/// <param name="datacenterId"> Id of the Datacenter to create the VMs
		/// 
		/// @pre $none
		/// @post $none </param>
		protected internal virtual void createVmsInDatacenterBase(int datacenterId)
		{
			// send as much vms as possible for this datacenter before trying the
			// next one
			int requestedVms = 0;

			// All host will have two VMs (assumption) VM is the minimum unit
			if (createvmflag)
			{
				CreateVMs(datacenterId);
				createvmflag = false;
			}

			// generate Application execution Requests
			for (int i = 0; i < 100; i++)
			{
				this.AppCloudletListProperty.Add(new WorkflowApp(AppCloudlet.APP_Workflow, NetworkConstants.currentAppId, 0, 0, Id));
				NetworkConstants.currentAppId++;

			}
			int k = 0;

			// schedule the application on VMs
			foreach (AppCloudlet app in this.AppCloudletListProperty)
			{

				IList<int?> vmids = new List<int?>();
				int numVms = linkDC.VmListProperty.Count;
				UniformDistr ufrnd = new UniformDistr(0, numVms, 5);
				for (int i = 0; i < app.numbervm; i++)
				{

					int vmid = (int) ufrnd.sample();
					vmids.Add(vmid);

				}

				if (vmids != null)
				{
					if (vmids.Count > 0)
					{

						app.createCloudletList(vmids);
						for (int i = 0; i < app.numbervm; i++)
						{
							app.clist[i].UserId = Id;
							appCloudletRecieved[app.appID] = app.numbervm;
							this.CloudletSubmittedListProperty.Add(app.clist[i]);
							cloudletsSubmitted++;

							// Sending cloudlet
							sendNow(VmsToDatacentersMap[this.VmListProperty[0].Id].Value, CloudSimTags.CLOUDLET_SUBMIT, app.clist[i]);
						}
						Debug.WriteLine("app" + (k++));
					}
				}

			}
            AppCloudletListProperty = new List<AppCloudlet>();
			if (NetworkConstants.iteration < 10)
			{

				NetworkConstants.iteration++;
				this.schedule(Id, NetworkConstants.nexttime, CloudSimTags.NextCycle);
			}

			VmsRequested = requestedVms;
			VmsAcks = 0;
		}

			/// <summary>
			/// Creates virtual machines in a datacenter </summary>
			/// <param name="datacenterId"> The id of the datacenter where to create the VMs. </param>
		private void CreateVMs(int datacenterId)
		{
			// two VMs per host
			int numVM = linkDC.HostListProperty.Count * NetworkConstants.maxhostVM;
			for (int i = 0; i < numVM; i++)
			{
				int vmid = i;
				int mips = 1;
				long size = 10000; // image size (MB)
				int ram = 512; // vm memory (MB)
				long bw = 1000;
				int pesNumber = NetworkConstants.HOST_PEs / NetworkConstants.maxhostVM;
				string vmm = "Xen"; // VMM name

				// create VM
				NetworkVm vm = new NetworkVm(vmid, Id, mips, pesNumber, ram, bw, size, vmm, new NetworkCloudletSpaceSharedScheduler());
				linkDC.processVmCreateNetwork(vm);
                // add the VM to the vmList
                VmListProperty.Add(vm);
				VmsToDatacentersMap[vmid] = datacenterId;
                // TEST: (fixed) Is the indexer a proper replacement?    
                //VmsCreatedListProperty.Add(VmList.getById(VmListProperty, vmid));
                VmsCreatedListProperty.Add(VmListProperty[vmid]);
            }
        }

		/// <summary>
		/// Sends request to destroy all VMs running on the datacenter.
		/// 
		/// @pre $none
		/// @post $none /** Destroy the virtual machines running in datacenters.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		protected internal virtual void clearDatacenters()
		{
			foreach (Vm vm in VmsCreatedListProperty)
			{
				Log.printConcatLine(CloudSim.clock(), ": ", Name, ": Destroying VM #", vm.Id);
				sendNow(VmsToDatacentersMap[vm.Id].Value, CloudSimTags.VM_DESTROY, vm);
			}

            VmsCreatedListProperty.Clear();
		}

		/// <summary>
		/// Sends an internal event communicating the end of the simulation.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		private void finishExecution()
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
        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.Vm> java.util.List<T> getVmList()
        //public virtual IList<NetworkVm> VmListProperty
        public virtual IList<NetworkVm> VmListProperty
        {
			get
			{
				return (IList<NetworkVm>) vmList;
			}
			set
			{
                // TEST: (fixed) Does this cast work?
                var fubar = value.Cast<Vm>().ToList();
				this.vmList = fubar;
			}
		}


		/// <summary>
		/// Gets the cloudlet list.
		/// </summary>
		/// @param <T> the generic type </param>
		/// <returns> the cloudlet list </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends NetworkCloudlet> java.util.List<T> getCloudletList()
		public virtual IList<NetworkCloudlet> CloudletListProperty
		{
			get
			{
				return (IList<NetworkCloudlet>) cloudletList;
			}
			set
			{
				this.cloudletList = value;
			}
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends AppCloudlet> java.util.List<T> getAppCloudletList()
		public virtual IList<AppCloudlet> AppCloudletListProperty
		{
			get
			{
				return (IList<AppCloudlet>) appCloudletList;
			}
			set
			{
				this.appCloudletList = value;
			}
		}


		/// <summary>
		/// Gets the cloudlet submitted list.
		/// </summary>
		/// @param <T> the generic type </param>
		/// <returns> the cloudlet submitted list </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.Cloudlet> java.util.List<T> getCloudletSubmittedList()
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
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.Cloudlet> java.util.List<T> getCloudletReceivedList()
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
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.Vm> java.util.List<T> getVmsCreatedList()
		public virtual IList<NetworkVm> VmsCreatedListProperty
		{
			get
			{
				return (IList<NetworkVm>) vmsCreatedList;
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