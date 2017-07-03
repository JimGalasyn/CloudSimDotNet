using System;
using System.Collections.Generic;

/*
 * Title: CloudSim Toolkit Description: CloudSim (Cloud Simulation) Toolkit for Modeling and
 * Simulation of Clouds Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * 
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{
    using System.Diagnostics;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using SimEntity = org.cloudbus.cloudsim.core.SimEntity;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;

    /// <summary>
    /// Datacenter class is a CloudResource whose hostList are virtualized. It deals with processing of
    /// VM queries (i.e., handling of VMs) instead of processing Cloudlet-related queries. 
    /// 
    /// So, even though an AllocPolicy will be instantiated (in the init() method of the superclass, 
    /// it will not be used, as processing of cloudlets are handled by the CloudletScheduler and 
    /// processing of VirtualMachines are handled by the VmAllocationPolicy.
    /// 
    /// @author Rodrigo N. Calheiros
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 1.0
    /// 
    /// @todo In fact, there isn't the method init() in the super class, as stated in
    /// the documentation here. An AllocPolicy isn't being instantiated there.
    /// The last phrase of the class documentation appears to be out-of-date or wrong.
    /// </summary>
    public class Datacenter : SimEntity
	{

		/// <summary>
		/// The characteristics. </summary>
		private DatacenterCharacteristics characteristics;

		/// <summary>
		/// The regional Cloud Information Service (CIS) name. </summary>
		/// <seealso cref= org.cloudbus.cloudsim.core.CloudInformationService </seealso>
		private string regionalCisName;

		/// <summary>
		/// The vm provisioner. </summary>
		private VmAllocationPolicy vmAllocationPolicy;

		/// <summary>
		/// The last time some cloudlet was processed in the datacenter. </summary>
		private double lastProcessTime;

		/// <summary>
		/// The storage list. </summary>
		private IList<Storage> storageList;

		/// <summary>
		/// The vm list. </summary>
		private IList<Vm> vmList;

		/// <summary>
		/// The scheduling delay to process each datacenter received event. </summary>
		private double schedulingInterval;

		/// <summary>
		/// Allocates a new Datacenter object.
		/// </summary>
		/// <param name="name"> the name to be associated with this entity (as required by the super class) </param>
		/// <param name="characteristics"> the characteristics of the datacenter to be created </param>
		/// <param name="storageList"> a List of storage elements, for data simulation </param>
		/// <param name="vmAllocationPolicy"> the policy to be used to allocate VMs into hosts </param>
		/// <param name="schedulingInterval"> the scheduling delay to process each datacenter received event </param>
		/// <exception cref="Exception"> when one of the following scenarios occur:
		///  <ul>
		///    <li>creating this entity before initializing CloudSim package
		///    <li>this entity name is <tt>null</tt> or empty
		///    <li>this entity has <tt>zero</tt> number of PEs (Processing Elements). <br/>
		///    No PEs mean the Cloudlets can't be processed. A CloudResource must contain 
		///    one or more Machines. A Machine must contain one or more PEs.
		///  </ul>
		/// 
		/// @pre name != null
		/// @pre resource != null
		/// @post $none </exception>
		public Datacenter(string name, DatacenterCharacteristics characteristics, VmAllocationPolicy vmAllocationPolicy, IList<Storage> storageList, double schedulingInterval) : base(name)
		{

			Characteristics = characteristics;
			VmAllocationPolicy = vmAllocationPolicy;
			LastProcessTime = 0.0;
			StorageList = storageList;
			VmListProperty = new List<Vm>();
			SchedulingInterval = schedulingInterval;

			foreach (Host host in Characteristics.HostListProperty)
			{
				host.Datacenter = this;
			}

			// If this resource doesn't have any PEs then no useful at all
			if (Characteristics.NumberOfPes == 0)
			{
						throw new Exception(base.Name + " : Error - this entity has no PEs. Therefore, can't process any Cloudlets.");
			}

			// stores id of this class
			Characteristics.Id = base.Id;
		}

		/// <summary>
		/// Overrides this method when making a new and different type of resource. <br>
		/// <b>NOTE:</b> You do not need to override <seealso cref="#body()"/> method, if you use this method.
		/// 
		/// @pre $none
		/// @post $none
		/// 
		/// @todo This method doesn't appear to be used
		/// </summary>
		protected internal virtual void registerOtherEntity()
		{
			// empty. This should be override by a child class
		}

		public override void processEvent(SimEvent ev)
		{
			int srcId = -1;

			switch (ev.Tag)
			{
			// Resource characteristics inquiry
				case CloudSimTags.RESOURCE_CHARACTERISTICS:
					srcId = ((int?) ev.Data).Value;
					sendNow(srcId, ev.Tag, Characteristics);
					break;

				// Resource dynamic info inquiry
				case CloudSimTags.RESOURCE_DYNAMICS:
					srcId = ((int?) ev.Data).Value;
					sendNow(srcId, ev.Tag, 0);
					break;

				case CloudSimTags.RESOURCE_NUM_PE:
					srcId = ((int?) ev.Data).Value;
					int numPE = Characteristics.NumberOfPes;
					sendNow(srcId, ev.Tag, numPE);
					break;

				case CloudSimTags.RESOURCE_NUM_FREE_PE:
					srcId = ((int?) ev.Data).Value;
					int freePesNumber = Characteristics.NumberOfFreePes;
					sendNow(srcId, ev.Tag, freePesNumber);
					break;

				// New Cloudlet arrives
				case CloudSimTags.CLOUDLET_SUBMIT:
					processCloudletSubmit(ev, false);
					break;

				// New Cloudlet arrives, but the sender asks for an ack
				case CloudSimTags.CLOUDLET_SUBMIT_ACK:
					processCloudletSubmit(ev, true);
					break;

				// Cancels a previously submitted Cloudlet
				case CloudSimTags.CLOUDLET_CANCEL:
					processCloudlet(ev, CloudSimTags.CLOUDLET_CANCEL);
					break;

				// Pauses a previously submitted Cloudlet
				case CloudSimTags.CLOUDLET_PAUSE:
					processCloudlet(ev, CloudSimTags.CLOUDLET_PAUSE);
					break;

				// Pauses a previously submitted Cloudlet, but the sender
				// asks for an acknowledgement
				case CloudSimTags.CLOUDLET_PAUSE_ACK:
					processCloudlet(ev, CloudSimTags.CLOUDLET_PAUSE_ACK);
					break;

				// Resumes a previously submitted Cloudlet
				case CloudSimTags.CLOUDLET_RESUME:
					processCloudlet(ev, CloudSimTags.CLOUDLET_RESUME);
					break;

				// Resumes a previously submitted Cloudlet, but the sender
				// asks for an acknowledgement
				case CloudSimTags.CLOUDLET_RESUME_ACK:
					processCloudlet(ev, CloudSimTags.CLOUDLET_RESUME_ACK);
					break;

				// Moves a previously submitted Cloudlet to a different resource
				case CloudSimTags.CLOUDLET_MOVE:
					processCloudletMove((int[]) ev.Data, CloudSimTags.CLOUDLET_MOVE);
					break;

				// Moves a previously submitted Cloudlet to a different resource
				case CloudSimTags.CLOUDLET_MOVE_ACK:
					processCloudletMove((int[]) ev.Data, CloudSimTags.CLOUDLET_MOVE_ACK);
					break;

				// Checks the status of a Cloudlet
				case CloudSimTags.CLOUDLET_STATUS:
					processCloudletStatus(ev);
					break;

				// Ping packet
				case CloudSimTags.INFOPKT_SUBMIT:
					processPingRequest(ev);
					break;

				case CloudSimTags.VM_CREATE:
					processVmCreate(ev, false);
					break;

				case CloudSimTags.VM_CREATE_ACK:
					processVmCreate(ev, true);
					break;

				case CloudSimTags.VM_DESTROY:
					processVmDestroy(ev, false);
					break;

				case CloudSimTags.VM_DESTROY_ACK:
					processVmDestroy(ev, true);
					break;

				case CloudSimTags.VM_MIGRATE:
					processVmMigrate(ev, false);
					break;

				case CloudSimTags.VM_MIGRATE_ACK:
					processVmMigrate(ev, true);
					break;

				case CloudSimTags.VM_DATA_ADD:
					processDataAdd(ev, false);
					break;

				case CloudSimTags.VM_DATA_ADD_ACK:
					processDataAdd(ev, true);
					break;

				case CloudSimTags.VM_DATA_DEL:
					processDataDelete(ev, false);
					break;

				case CloudSimTags.VM_DATA_DEL_ACK:
					processDataDelete(ev, true);
					break;

				case CloudSimTags.VM_DATACENTER_EVENT:
					updateCloudletProcessing();
					checkCloudletCompletion();
					break;

				// other unknown tags are processed by this method
				default:
					processOtherEvent(ev);
					break;
			}
		}

		/// <summary>
		/// Process a file deletion request.
		/// </summary>
		/// <param name="ev"> information about the event just happened </param>
		/// <param name="ack"> indicates if the event's sender expects to receive 
		/// an acknowledge message when the event finishes to be processed </param>
		protected internal virtual void processDataDelete(SimEvent ev, bool ack)
		{
			if (ev == null)
			{
				return;
			}

			object[] data = (object[]) ev.Data;
			if (data == null)
			{
				return;
			}

			string filename = (string) data[0];
			int req_source = ((int?) data[1]).Value;
			int tag = -1;

			// check if this file can be deleted (do not delete is right now)
			int msg = deleteFileFromStorage(filename);
			if (msg == DataCloudTags.FILE_DELETE_SUCCESSFUL)
			{
				tag = DataCloudTags.CTLG_DELETE_MASTER;
			}
			else
			{ // if an error occured, notify user
				tag = DataCloudTags.FILE_DELETE_MASTER_RESULT;
			}

			if (ack)
			{
				// send back to sender
				object[] pack = new object[2];
				pack[0] = filename;
				pack[1] = Convert.ToInt32(msg);

				sendNow(req_source, tag, pack);
			}
		}

		/// <summary>
		/// Process a file inclusion request.
		/// </summary>
		/// <param name="ev"> information about the event just happened </param>
		/// <param name="ack"> indicates if the event's sender expects to receive 
		/// an acknowledge message when the event finishes to be processed </param>
		protected internal virtual void processDataAdd(SimEvent ev, bool ack)
		{
			if (ev == null)
			{
				return;
			}

			object[] pack = (object[]) ev.Data;
			if (pack == null)
			{
				return;
			}

			File file = (File) pack[0]; // get the file
			file.MasterCopy = true; // set the file into a master copy
			int sentFrom = ((int?) pack[1]).Value; // get sender ID

			/// <summary>
			///****
			/// // DEBUG Log.printLine(super.get_name() + ".addMasterFile(): " + file.getName() +
			/// " from " + CloudSim.getEntityName(sentFrom));
			/// ******
			/// </summary>

			object[] data = new object[3];
			data[0] = file.Name;

			int msg = addFile(file); // add the file

			if (ack)
			{
				data[1] = Convert.ToInt32(-1); // no sender id
				data[2] = Convert.ToInt32(msg); // the result of adding a master file
				sendNow(sentFrom, DataCloudTags.FILE_ADD_MASTER_RESULT, data);
			}
		}

		/// <summary>
		/// Processes a ping request.
		/// </summary>
		/// <param name="ev"> information about the event just happened
		/// 
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processPingRequest(SimEvent ev)
		{
			InfoPacket pkt = (InfoPacket) ev.Data;
			pkt.Tag = CloudSimTags.INFOPKT_RETURN;
			pkt.DestId = pkt.SrcId;

			// sends back to the sender
			sendNow(pkt.SrcId, CloudSimTags.INFOPKT_RETURN, pkt);
		}

		/// <summary>
		/// Process the event for an User/Broker who wants to know the status of a Cloudlet. This
		/// Datacenter will then send the status back to the User/Broker.
		/// </summary>
		/// <param name="ev"> information about the event just happened
		/// 
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processCloudletStatus(SimEvent ev)
		{
			int cloudletId = 0;
			int userId = 0;
			int vmId = 0;
			int status = -1;

			try
			{
				// if a sender using cloudletXXX() methods
				int[] data = (int[]) ev.Data;
				cloudletId = data[0];
				userId = data[1];
				vmId = data[2];

				status = VmAllocationPolicy.getHost(vmId, userId).getVm(vmId,userId).CloudletScheduler.getCloudletStatus(cloudletId);
			}

			// if a sender using normal send() methods
			catch (System.InvalidCastException)
			{
				try
				{
					Cloudlet cl = (Cloudlet) ev.Data;
					cloudletId = cl.CloudletId;
					userId = cl.UserId;

					status = VmAllocationPolicy.getHost(vmId, userId).getVm(vmId,userId).CloudletScheduler.getCloudletStatus(cloudletId);
				}
				catch (Exception e)
				{
					Log.printConcatLine(Name, ": Error in processing CloudSimTags.CLOUDLET_STATUS");
					Log.printLine(e.Message);
					return;
				}
			}
			catch (Exception e)
			{
				Log.printConcatLine(Name, ": Error in processing CloudSimTags.CLOUDLET_STATUS");
				Log.printLine(e.Message);
				return;
			}

			int[] array = new int[3];
			array[0] = Id;
			array[1] = cloudletId;
			array[2] = status;

			int tag = CloudSimTags.CLOUDLET_STATUS;
			sendNow(userId, tag, array);
		}

		/// <summary>
		/// Process non-default received events that aren't processed by
		/// the <seealso cref="#processEvent(org.cloudbus.cloudsim.core.SimEvent)"/> method.
		/// This method should be overridden by subclasses in other to process
		/// new defined events.
		/// </summary>
		/// <param name="ev"> information about the event just happened
		/// 
		/// @pre $none
		/// @post $none </param>
		protected internal virtual void processOtherEvent(SimEvent ev)
		{
			if (ev == null)
			{
				Log.printConcatLine(Name, ".processOtherEvent(): Error - an event is null.");
			}
		}

		/// <summary>
		/// Process the event for an User/Broker who wants to create a VM in this Datacenter. This
		/// Datacenter will then send the status back to the User/Broker.
		/// </summary>
		/// <param name="ev"> information about the event just happened </param>
		/// <param name="ack"> indicates if the event's sender expects to receive 
		/// an acknowledge message when the event finishes to be processed
		/// 
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processVmCreate(SimEvent ev, bool ack)
		{
			Vm vm = (Vm) ev.Data;

			bool result = VmAllocationPolicy.allocateHostForVm(vm);

			if (ack)
			{
				int[] data = new int[3];
				data[0] = Id;
				data[1] = vm.Id;

				if (result)
				{
					data[2] = CloudSimTags.TRUE;
				}
				else
				{
					data[2] = CloudSimTags.FALSE;
				}
				send(vm.UserId, CloudSim.MinTimeBetweenEvents, CloudSimTags.VM_CREATE_ACK, data);
			}

			if (result)
			{
				VmListProperty.Add(vm);

				if (vm.BeingInstantiated)
				{
					vm.BeingInstantiated = false;
				}

				vm.updateVmProcessing(CloudSim.clock(), VmAllocationPolicy.getHost(vm).VmScheduler.getAllocatedMipsForVm(vm));
			}

		}

		/// <summary>
		/// Process the event for an User/Broker who wants to destroy a VM previously created in this
		/// Datacenter. This Datacenter may send, upon request, the status back to the
		/// User/Broker.
		/// </summary>
		/// <param name="ev"> information about the event just happened </param>
		/// <param name="ack"> indicates if the event's sender expects to receive 
		/// an acknowledge message when the event finishes to be processed
		/// 
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processVmDestroy(SimEvent ev, bool ack)
		{
			Vm vm = (Vm) ev.Data;
			VmAllocationPolicy.deallocateHostForVm(vm);

			if (ack)
			{
				int[] data = new int[3];
				data[0] = Id;
				data[1] = vm.Id;
				data[2] = CloudSimTags.TRUE;

				sendNow(vm.UserId, CloudSimTags.VM_DESTROY_ACK, data);
			}

			VmListProperty.Remove(vm);
		}

		/// <summary>
		/// Process the event for an User/Broker who wants to migrate a VM. This Datacenter will
		/// then send the status back to the User/Broker.
		/// </summary>
		/// <param name="ev"> information about the event just happened </param>
		/// <param name="ack"> indicates if the event's sender expects to receive 
		/// an acknowledge message when the event finishes to be processed
		/// 
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processVmMigrate(SimEvent ev, bool ack)
		{
			object tmp = ev.Data;
            // TODO: Is this check necessary?
            //if (!(tmp is IDictionary<object, ?>))
			//{
			//	throw new System.InvalidCastException("The data object must be Map<String, Object>");
			//}

			IDictionary<string, object> migrate = (Dictionary<string, object>) tmp;

			Vm vm = (Vm) migrate["vm"];
			Host host = (Host) migrate["host"];

			VmAllocationPolicy.deallocateHostForVm(vm);
			host.removeMigratingInVm(vm);
			bool result = VmAllocationPolicy.allocateHostForVm(vm, host);
			if (!result)
			{
				Log.printLine("[Datacenter.processVmMigrate] VM allocation to the destination host failed");
                //Environment.Exit(0);
                throw new Exception("Datacenter.processVmMigrate: VM allocation to the destination host failed");
			}

			if (ack)
			{
				int[] data = new int[3];
				data[0] = Id;
				data[1] = vm.Id;

				if (result)
				{
					data[2] = CloudSimTags.TRUE;
				}
				else
				{
					data[2] = CloudSimTags.FALSE;
				}
				sendNow(ev.Source, CloudSimTags.VM_CREATE_ACK, data);
			}

			Log.formatLine("%.2f: Migration of VM #%d to Host #%d is completed", CloudSim.clock(), vm.Id, host.Id);
			vm.InMigration = false;
		}

		/// <summary>
		/// Processes a Cloudlet based on the event type.
		/// </summary>
		/// <param name="ev"> information about the event just happened </param>
		/// <param name="type"> event type
		/// 
		/// @pre ev != null
		/// @pre type > 0
		/// @post $none </param>
		protected internal virtual void processCloudlet(SimEvent ev, int type)
		{
			int cloudletId = 0;
			int userId = 0;
			int vmId = 0;

			try
			{ // if the sender using cloudletXXX() methods
				int[] data = (int[]) ev.Data;
				cloudletId = data[0];
				userId = data[1];
				vmId = data[2];
			}

			// if the sender using normal send() methods
			catch (System.InvalidCastException)
			{
				try
				{
					Cloudlet cl = (Cloudlet) ev.Data;
					cloudletId = cl.CloudletId;
					userId = cl.UserId;
					vmId = cl.VmId;
				}
				catch (Exception e)
				{
					Log.printConcatLine(base.Name, ": Error in processing Cloudlet");
					Log.printLine(e.Message);
					return;
				}
			}
			catch (Exception e)
			{
				Log.printConcatLine(base.Name, ": Error in processing a Cloudlet.");
				Log.printLine(e.Message);
				return;
			}

			// begins executing ....
			switch (type)
			{
				case CloudSimTags.CLOUDLET_CANCEL:
					processCloudletCancel(cloudletId, userId, vmId);
					break;

				case CloudSimTags.CLOUDLET_PAUSE:
					processCloudletPause(cloudletId, userId, vmId, false);
					break;

				case CloudSimTags.CLOUDLET_PAUSE_ACK:
					processCloudletPause(cloudletId, userId, vmId, true);
					break;

				case CloudSimTags.CLOUDLET_RESUME:
					processCloudletResume(cloudletId, userId, vmId, false);
					break;

				case CloudSimTags.CLOUDLET_RESUME_ACK:
					processCloudletResume(cloudletId, userId, vmId, true);
					break;
				default:
					break;
			}

		}

		/// <summary>
		/// Process the event for an User/Broker who wants to move a Cloudlet.
		/// </summary>
		/// <param name="receivedData"> information about the migration </param>
		/// <param name="type"> event type
		/// 
		/// @pre receivedData != null
		/// @pre type > 0
		/// @post $none </param>
		protected internal virtual void processCloudletMove(int[] receivedData, int type)
		{
			updateCloudletProcessing();

			int[] array = receivedData;
			int cloudletId = array[0];
			int userId = array[1];
			int vmId = array[2];
			int vmDestId = array[3];
			int destId = array[4];

			// get the cloudlet
			Cloudlet cl = VmAllocationPolicy.getHost(vmId, userId).getVm(vmId,userId).CloudletScheduler.cloudletCancel(cloudletId);

			bool failed = false;
			if (cl == null)
			{ // cloudlet doesn't exist
				failed = true;
			}
			else
			{
				// has the cloudlet already finished?
				if (cl.CloudletStatusString == "Success")
				{ // if yes, send it back to user
					int[] data = new int[3];
					data[0] = Id;
					data[1] = cloudletId;
					data[2] = 0;
					sendNow(cl.UserId, CloudSimTags.CLOUDLET_SUBMIT_ACK, data);
					sendNow(cl.UserId, CloudSimTags.CLOUDLET_RETURN, cl);
				}

				// prepare cloudlet for migration
				cl.VmId = vmDestId;

				// the cloudlet will migrate from one vm to another does the destination VM exist?
				if (destId == Id)
				{
					Vm vm = VmAllocationPolicy.getHost(vmDestId, userId).getVm(vmDestId,userId);
					if (vm == null)
					{
						failed = true;
					}
					else
					{
						// time to transfer the files
						double fileTransferTime = predictFileTransferTime(cl.RequiredFiles);
						vm.CloudletScheduler.cloudletSubmit(cl, fileTransferTime);
					}
				}
				else
				{ // the cloudlet will migrate from one resource to another
					int tag = ((type == CloudSimTags.CLOUDLET_MOVE_ACK) ? CloudSimTags.CLOUDLET_SUBMIT_ACK : CloudSimTags.CLOUDLET_SUBMIT);
					sendNow(destId, tag, cl);
				}
			}

			if (type == CloudSimTags.CLOUDLET_MOVE_ACK)
			{ // send ACK if requested
				int[] data = new int[3];
				data[0] = Id;
				data[1] = cloudletId;
				if (failed)
				{
					data[2] = 0;
				}
				else
				{
					data[2] = 1;
				}
				sendNow(cl.UserId, CloudSimTags.CLOUDLET_SUBMIT_ACK, data);
			}
		}

		/// <summary>
		/// Processes a Cloudlet submission.
		/// </summary>
		/// <param name="ev"> information about the event just happened </param>
		/// <param name="ack"> indicates if the event's sender expects to receive 
		/// an acknowledge message when the event finishes to be processed
		/// 
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processCloudletSubmit(SimEvent ev, bool ack)
		{
			updateCloudletProcessing();

			try
			{
				// gets the Cloudlet object
				Cloudlet cl = (Cloudlet) ev.Data;

				// checks whether this Cloudlet has finished or not
				if (cl.Finished)
				{
					string name = CloudSim.getEntityName(cl.UserId);
					Log.printConcatLine(Name, ": Warning - Cloudlet #", cl.CloudletId, " owned by ", name, " is already completed/finished.");
					Log.printLine("Therefore, it is not being executed again");
					Log.printLine();

					// NOTE: If a Cloudlet has finished, then it won't be processed.
					// So, if ack is required, this method sends back a result.
					// If ack is not required, this method don't send back a result.
					// Hence, this might cause CloudSim to be hanged since waiting
					// for this Cloudlet back.
					if (ack)
					{
						int[] data = new int[3];
						data[0] = Id;
						data[1] = cl.CloudletId;
						data[2] = CloudSimTags.FALSE;

						// unique tag = operation tag
						int tag = CloudSimTags.CLOUDLET_SUBMIT_ACK;
						sendNow(cl.UserId, tag, data);
					}

					sendNow(cl.UserId, CloudSimTags.CLOUDLET_RETURN, cl);

					return;
				}

				// process this Cloudlet to this CloudResource
				cl.setResourceParameter(Id, Characteristics.CostPerSecond, Characteristics.CostPerBw);

				int userId = cl.UserId;
				int vmId = cl.VmId;

				// time to transfer the files
				double fileTransferTime = predictFileTransferTime(cl.RequiredFiles);

				Host host = VmAllocationPolicy.getHost(vmId, userId);
				Vm vm = host.getVm(vmId, userId);
				CloudletScheduler scheduler = vm.CloudletScheduler;
				double estimatedFinishTime = scheduler.cloudletSubmit(cl, fileTransferTime);

				// if this cloudlet is in the exec queue
				if (estimatedFinishTime > 0.0 && !double.IsInfinity(estimatedFinishTime))
				{
					estimatedFinishTime += fileTransferTime;
					send(Id, estimatedFinishTime, CloudSimTags.VM_DATACENTER_EVENT);
				}

				if (ack)
				{
					int[] data = new int[3];
					data[0] = Id;
					data[1] = cl.CloudletId;
					data[2] = CloudSimTags.TRUE;

					// unique tag = operation tag
					int tag = CloudSimTags.CLOUDLET_SUBMIT_ACK;
					sendNow(cl.UserId, tag, data);
				}
			}
			catch (System.InvalidCastException c)
			{
				Log.printLine(Name + ".processCloudletSubmit(): " + "ClassCastException error.");
				Debug.WriteLine(c.ToString());
                Debug.WriteLine(c.StackTrace);
			}
			catch (Exception e)
			{
				Log.printLine(Name + ".processCloudletSubmit(): " + "Exception error.");
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
			}

			checkCloudletCompletion();
		}

		/// <summary>
		/// Predict the total time to transfer a list of files.
		/// </summary>
		/// <param name="requiredFiles"> the files to be transferred </param>
		/// <returns> the predicted time </returns>
		protected internal virtual double predictFileTransferTime(IList<string> requiredFiles)
		{
			double time = 0.0;

			IEnumerator<string> iter = requiredFiles.GetEnumerator();
			while (iter.MoveNext())
			{
				string fileName = iter.Current;
				for (int i = 0; i < StorageList.Count; i++)
				{
					Storage tempStorage = StorageList[i];
					File tempFile = tempStorage.getFile(fileName);
					if (tempFile != null)
					{
						time += tempFile.Size / tempStorage.MaxTransferRate;
						break;
					}
				}
			}
			return time;
		}

		/// <summary>
		/// Processes a Cloudlet resume request.
		/// </summary>
		/// <param name="cloudletId"> ID of the cloudlet to be resumed </param>
		/// <param name="userId"> ID of the cloudlet's owner </param>
		/// <param name="ack"> indicates if the event's sender expects to receive 
		/// an acknowledge message when the event finishes to be processed </param>
		/// <param name="vmId"> the id of the VM where the cloudlet has to be resumed
		/// 
		/// @pre $none
		/// @post $none </param>
		protected internal virtual void processCloudletResume(int cloudletId, int userId, int vmId, bool ack)
		{
			double eventTime = VmAllocationPolicy.getHost(vmId, userId).getVm(vmId,userId).CloudletScheduler.cloudletResume(cloudletId);

			bool status = false;
			if (eventTime > 0.0)
			{ // if this cloudlet is in the exec queue
				status = true;
				if (eventTime > CloudSim.clock())
				{
					schedule(Id, eventTime, CloudSimTags.VM_DATACENTER_EVENT);
				}
			}

			if (ack)
			{
				int[] data = new int[3];
				data[0] = Id;
				data[1] = cloudletId;
				if (status)
				{
					data[2] = CloudSimTags.TRUE;
				}
				else
				{
					data[2] = CloudSimTags.FALSE;
				}
				sendNow(userId, CloudSimTags.CLOUDLET_RESUME_ACK, data);
			}
		}

		/// <summary>
		/// Processes a Cloudlet pause request.
		/// </summary>
		/// <param name="cloudletId"> ID of the cloudlet to be paused </param>
		/// <param name="userId"> ID of the cloudlet's owner </param>
		/// <param name="ack"> indicates if the event's sender expects to receive 
		/// an acknowledge message when the event finishes to be processed </param>
		/// <param name="vmId"> the id of the VM where the cloudlet has to be paused
		/// 
		/// @pre $none
		/// @post $none </param>
		protected internal virtual void processCloudletPause(int cloudletId, int userId, int vmId, bool ack)
		{
			bool status = VmAllocationPolicy.getHost(vmId, userId).getVm(vmId,userId).CloudletScheduler.cloudletPause(cloudletId);

			if (ack)
			{
				int[] data = new int[3];
				data[0] = Id;
				data[1] = cloudletId;
				if (status)
				{
					data[2] = CloudSimTags.TRUE;
				}
				else
				{
					data[2] = CloudSimTags.FALSE;
				}
				sendNow(userId, CloudSimTags.CLOUDLET_PAUSE_ACK, data);
			}
		}

		/// <summary>
		/// Processes a Cloudlet cancel request.
		/// </summary>
		/// <param name="cloudletId"> ID of the cloudlet to be canceled </param>
		/// <param name="userId"> ID of the cloudlet's owner </param>
		/// <param name="vmId"> the id of the VM where the cloudlet has to be canceled
		/// 
		/// @pre $none
		/// @post $none </param>
		protected internal virtual void processCloudletCancel(int cloudletId, int userId, int vmId)
		{
			Cloudlet cl = VmAllocationPolicy.getHost(vmId, userId).getVm(vmId,userId).CloudletScheduler.cloudletCancel(cloudletId);
			sendNow(userId, CloudSimTags.CLOUDLET_CANCEL, cl);
		}

		/// <summary>
		/// Updates processing of each cloudlet running in this Datacenter. It is necessary because
		/// Hosts and VirtualMachines are simple objects, not entities. So, they don't receive events and
		/// updating cloudlets inside them must be called from the outside.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		protected internal virtual void updateCloudletProcessing()
		{
			// if some time passed since last processing
			// R: for term is to allow loop at simulation start. Otherwise, one initial
			// simulation step is skipped and schedulers are not properly initialized
			if (CloudSim.clock() < 0.111 || CloudSim.clock() > LastProcessTime + CloudSim.MinTimeBetweenEvents)
			{
				IList<Host> list = VmAllocationPolicy.HostListProperty;
				double smallerTime = double.MaxValue;
				// for each host...
				for (int i = 0; i < list.Count; i++)
				{
					Host host = list[i];
					// inform VMs to update processing
					double time = host.updateVmsProcessing(CloudSim.clock());
					// what time do we expect that the next cloudlet will finish?
					if (time < smallerTime)
					{
						smallerTime = time;
					}
				}
				// gurantees a minimal interval before scheduling the event
				if (smallerTime < CloudSim.clock() + CloudSim.MinTimeBetweenEvents + 0.01)
				{
					smallerTime = CloudSim.clock() + CloudSim.MinTimeBetweenEvents + 0.01;
				}
				if (smallerTime != double.MaxValue)
				{
					schedule(Id, (smallerTime - CloudSim.clock()), CloudSimTags.VM_DATACENTER_EVENT);
				}
				LastProcessTime = CloudSim.clock();
			}
		}

		/// <summary>
		/// Verifies if some cloudlet inside this Datacenter already finished. 
		/// If yes, send it to the User/Broker
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		protected internal virtual void checkCloudletCompletion()
		{
			IList<Host> list = VmAllocationPolicy.HostListProperty;
			for (int i = 0; i < list.Count; i++)
			{
				Host host = list[i];
				foreach (Vm vm in host.VmListProperty)
				{
					while (vm.CloudletScheduler.FinishedCloudlets)
					{
						Cloudlet cl = vm.CloudletScheduler.NextFinishedCloudlet;
						if (cl != null)
						{
							sendNow(cl.UserId, CloudSimTags.CLOUDLET_RETURN, cl);
						}
					}
				}
			}
		}

		/// <summary>
		/// Adds a file into the resource's storage before the experiment starts. 
		/// If the file is a master file, then it will be registered to the RC 
		/// when the experiment begins.
		/// </summary>
		/// <param name="file"> a DataCloud file </param>
		/// <returns> a tag number denoting whether this operation is a success or not </returns>
		public virtual int addFile(File file)
		{
			if (file == null)
			{
				return DataCloudTags.FILE_ADD_ERROR_EMPTY;
			}

			if (contains(file.Name))
			{
				return DataCloudTags.FILE_ADD_ERROR_EXIST_READ_ONLY;
			}

			// check storage space first
			if (StorageList.Count <= 0)
			{
				return DataCloudTags.FILE_ADD_ERROR_STORAGE_FULL;
			}

			Storage tempStorage = null;
			int msg = DataCloudTags.FILE_ADD_ERROR_STORAGE_FULL;

			for (int i = 0; i < StorageList.Count; i++)
			{
				tempStorage = StorageList[i];
				if (tempStorage.AvailableSpace >= file.Size)
				{
					tempStorage.addFile(file);
					msg = DataCloudTags.FILE_ADD_SUCCESSFUL;
					break;
				}
			}

			return msg;
		}

		/// <summary>
		/// Checks whether the datacenter has the given file.
		/// </summary>
		/// <param name="file"> a file to be searched </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		protected internal virtual bool contains(File file)
		{
			if (file == null)
			{
				return false;
			}
			return contains(file.Name);
		}

		/// <summary>
		/// Checks whether the datacenter has the given file.
		/// </summary>
		/// <param name="fileName"> a file name to be searched </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		protected internal virtual bool contains(string fileName)
		{
			if (string.ReferenceEquals(fileName, null) || fileName.Length == 0)
			{
				return false;
			}

			IEnumerator<Storage> it = StorageList.GetEnumerator();
			Storage storage = null;
			bool result = false;

			while (it.MoveNext())
			{
				storage = it.Current;
				if (storage.contains(fileName))
				{
					result = true;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Deletes the file from the storage. 
		/// Also, check whether it is possible to delete the file from the storage.
		/// </summary>
		/// <param name="fileName"> the name of the file to be deleted </param>
		/// <returns> the tag denoting the status of the operation,
		/// either <seealso cref="DataCloudTags#FILE_DELETE_ERROR"/> or 
		///  <seealso cref="DataCloudTags#FILE_DELETE_SUCCESSFUL"/> </returns>
		private int deleteFileFromStorage(string fileName)
		{
			Storage tempStorage = null;
			File tempFile = null;
			int msg = DataCloudTags.FILE_DELETE_ERROR;

			for (int i = 0; i < StorageList.Count; i++)
			{
				tempStorage = StorageList[i];
				tempFile = tempStorage.getFile(fileName);
				tempStorage.deleteFile(fileName, tempFile);
				msg = DataCloudTags.FILE_DELETE_SUCCESSFUL;
			} // end for

			return msg;
		}

		public override void shutdownEntity()
		{
			Log.printConcatLine(Name, " is shutting down...");
		}

		public override void startEntity()
		{
			Log.printConcatLine(Name, " is starting...");
			// this resource should register to regional CIS.
			// However, if not specified, then register to system CIS (the
			// default CloudInformationService) entity.
			int gisID = CloudSim.getEntityId(regionalCisName);
			if (gisID == -1)
			{
				gisID = CloudSim.CloudInfoServiceEntityId;
			}

			// send the registration to CIS
			sendNow(gisID, CloudSimTags.REGISTER_RESOURCE, Id);
			// Below method is for a child class to override
			registerOtherEntity();
		}

        /// <summary>
        /// Gets the host list.
        /// </summary>
        /// <returns> the host list </returns>
        public virtual IList<Host> HostListProperty
        {
			get
			{
				return Characteristics.HostListProperty;
			}
		}

		/// <summary>
		/// Gets the datacenter characteristics.
		/// </summary>
		/// <returns> the datacenter characteristics </returns>
		protected internal virtual DatacenterCharacteristics Characteristics
		{
			get
			{
				return characteristics;
			}
			set
			{
				this.characteristics = value;
			}
		}


		/// <summary>
		/// Gets the regional Cloud Information Service (CIS) name. 
		/// </summary>
		/// <returns> the regional CIS name </returns>
		protected internal virtual string RegionalCisName
		{
			get
			{
				return regionalCisName;
			}
			set
			{
				this.regionalCisName = value;
			}
		}


		/// <summary>
		/// Gets the vm allocation policy.
		/// </summary>
		/// <returns> the vm allocation policy </returns>
		public virtual VmAllocationPolicy VmAllocationPolicy
		{
			get
			{
				return vmAllocationPolicy;
			}
			set
			{
				this.vmAllocationPolicy = value;
			}
		}


		/// <summary>
		/// Gets the last time some cloudlet was processed in the datacenter.
		/// </summary>
		/// <returns> the last process time </returns>
		protected internal virtual double LastProcessTime
		{
			get
			{
				return lastProcessTime;
			}
			set
			{
				this.lastProcessTime = value;
			}
		}


		/// <summary>
		/// Gets the storage list.
		/// </summary>
		/// <returns> the storage list </returns>
		protected internal virtual IList<Storage> StorageList
		{
			get
			{
				return storageList;
			}
			set
			{
				this.storageList = value;
			}
		}


        /// <summary>
        /// Gets the vm list.
        /// </summary>
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
		/// Gets the scheduling interval.
		/// </summary>
		/// <returns> the scheduling interval </returns>
		protected internal virtual double SchedulingInterval
		{
			get
			{
				return schedulingInterval;
			}
			set
			{
				this.schedulingInterval = value;
			}
		}


	}

}