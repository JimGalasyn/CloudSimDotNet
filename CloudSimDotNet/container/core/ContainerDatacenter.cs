using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{


    using ContainerAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerAllocationPolicy;
    using ContainerVmAllocationPolicy = org.cloudbus.cloudsim.container.resourceAllocators.ContainerVmAllocationPolicy;
    using org.cloudbus.cloudsim;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using SimEntity = org.cloudbus.cloudsim.core.SimEntity;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
    using System.Diagnostics;


    /// <summary>
    /// Created by sareh on 10/07/15.
    /// </summary>
    public class ContainerDatacenter : SimEntity
    {

        /// <summary>
        /// The characteristics.
        /// </summary>
        private ContainerDatacenterCharacteristics characteristics;

        /// <summary>
        /// The regional cis name.
        /// </summary>
        private string regionalCisName;

        /// <summary>
        /// The vm provisioner.
        /// </summary>
        private ContainerVmAllocationPolicy vmAllocationPolicy;
        /// <summary>
        /// The container provisioner.
        /// </summary>
        private ContainerAllocationPolicy containerAllocationPolicy;

        /// <summary>
        /// The last process time.
        /// </summary>
        private double lastProcessTime;

        /// <summary>
        /// The storage list.
        /// </summary>
        private IList<Storage> storageList;

        /// <summary>
        /// The vm list.
        /// </summary>
        private IList<ContainerVm> containerVmList;
        /// <summary>
        /// The container list.
        /// </summary>
        private IList<Container> containerList;

        /// <summary>
        /// The scheduling interval.
        /// </summary>
        private double schedulingInterval;
        /// <summary>
        /// The scheduling interval.
        /// </summary>
        private string experimentName;
        /// <summary>
        /// The log address.
        /// </summary>
        private string logAddress;


        /// <summary>
        /// Allocates a new PowerDatacenter object. </summary>
        /// <param name="name"> </param>
        /// <param name="characteristics"> </param>
        /// <param name="vmAllocationPolicy"> </param>
        /// <param name="containerAllocationPolicy"> </param>
        /// <param name="storageList"> </param>
        /// <param name="schedulingInterval"> </param>
        /// <param name="experimentName"> </param>
        /// <param name="logAddress"> </param>
        /// <exception cref="Exception"> </exception>
        public ContainerDatacenter(
            string name,
            ContainerDatacenterCharacteristics characteristics,
            ContainerVmAllocationPolicy vmAllocationPolicy,
            ContainerAllocationPolicy containerAllocationPolicy,
            IList<Storage> storageList,
            double schedulingInterval,
            string experimentName,
            string logAddress) : base(name)
        {

            Characteristics = characteristics;
            VmAllocationPolicy = vmAllocationPolicy;
            ContainerAllocationPolicy = containerAllocationPolicy;
            LastProcessTime = 0.0;
            StorageList = storageList;
            ContainerVmListProperty = new List<ContainerVm>();
            ContainerList = new List<Container>();
            SchedulingInterval = schedulingInterval;
            ExperimentName = experimentName;
            LogAddress = logAddress;

            foreach (ContainerHost host in Characteristics.HostListProperty)
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
        /// <b>NOTE:</b> You do not need to override {} method, if you use this method.
        /// 
        /// @pre $none
        /// @post $none
        /// </summary>
        protected internal virtual void registerOtherEntity()
        {
            // empty. This should be override by a child class
        }

        /// <summary>
        /// Processes events or services that are available for this PowerDatacenter.
        /// </summary>
        /// <param name="ev"> a Sim_event object
        /// @pre ev != null
        /// @post $none </param>
        public override void processEvent(SimEvent ev)
        {
            int srcId = -1;

            switch (ev.Tag)
            {
                // Resource characteristics inquiry
                case CloudSimTags.RESOURCE_CHARACTERISTICS:
                    srcId = ((int?)ev.Data).Value;
                    sendNow(srcId, ev.Tag, Characteristics);
                    break;

                // Resource dynamic info inquiry
                case CloudSimTags.RESOURCE_DYNAMICS:
                    srcId = ((int?)ev.Data).Value;
                    sendNow(srcId, ev.Tag, 0);
                    break;

                case CloudSimTags.RESOURCE_NUM_PE:
                    srcId = ((int?)ev.Data).Value;
                    int numPE = Characteristics.NumberOfPes;
                    sendNow(srcId, ev.Tag, numPE);
                    break;

                case CloudSimTags.RESOURCE_NUM_FREE_PE:
                    srcId = ((int?)ev.Data).Value;
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
                    processCloudletMove((int[])ev.Data, CloudSimTags.CLOUDLET_MOVE);
                    break;

                // Moves a previously submitted Cloudlet to a different resource
                case CloudSimTags.CLOUDLET_MOVE_ACK:
                    processCloudletMove((int[])ev.Data, CloudSimTags.CLOUDLET_MOVE_ACK);
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
                case containerCloudSimTags.CONTAINER_SUBMIT:
                    processContainerSubmit(ev, true);
                    break;

                case containerCloudSimTags.CONTAINER_MIGRATE:
                    processContainerMigrate(ev, false);
                    // other unknown tags are processed by this method
                    break;

                default:
                    processOtherEvent(ev);
                    break;
            }
        }

        public virtual void processContainerSubmit(SimEvent ev, bool ack)
        {
            IList<Container> containerList = (IList<Container>)ev.Data;

            foreach (Container container in containerList)
            {
                bool result = ContainerAllocationPolicy.allocateVmForContainer(container, ContainerVmListProperty);
                if (ack)
                {
                    int[] data = new int[3];
                    data[1] = container.Id;
                    if (result)
                    {
                        data[2] = CloudSimTags.TRUE;
                    }
                    else
                    {
                        data[2] = CloudSimTags.FALSE;
                    }
                    if (result)
                    {
                        ContainerVm containerVm = ContainerAllocationPolicy.getContainerVm(container);
                        data[0] = containerVm.Id;
                        if (containerVm.Id == -1)
                        {

                            Log.printConcatLine("The ContainerVM ID is not known (-1) !");
                        }
                        //                    Log.printConcatLine("Assigning the container#" + container.getUid() + "to VM #" + containerVm.getUid());
                        ContainerList.Add(container);
                        if (container.BeingInstantiated)
                        {
                            container.BeingInstantiated = false;
                        }
                        container.updateContainerProcessing(CloudSim.clock(), ContainerAllocationPolicy.getContainerVm(container).ContainerScheduler.getAllocatedMipsForContainer(container));
                    }
                    else
                    {
                        data[0] = -1;
                        //notAssigned.add(container);
                        Log.printLine(string.Format("Couldn't find a vm to host the container #{0}", container.Uid));

                    }
                    send(ev.Source, CloudSim.MinTimeBetweenEvents, containerCloudSimTags.CONTAINER_CREATE_ACK, data);

                }
            }

        }

        /// <summary>
        /// Process data del.
        /// </summary>
        /// <param name="ev">  the ev </param>
        /// <param name="ack"> the ack </param>
        protected internal virtual void processDataDelete(SimEvent ev, bool ack)
        {
            if (ev == null)
            {
                return;
            }

            object[] data = (object[])ev.Data;
            if (data == null)
            {
                return;
            }

            string filename = (string)data[0];
            int req_source = ((int?)data[1]).Value;
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
        /// Process data add.
        /// </summary>
        /// <param name="ev">  the ev </param>
        /// <param name="ack"> the ack </param>
        protected internal virtual void processDataAdd(SimEvent ev, bool ack)
        {
            if (ev == null)
            {
                return;
            }

            object[] pack = (object[])ev.Data;
            if (pack == null)
            {
                return;
            }

            File file = (File)pack[0]; // get the file
            file.MasterCopy = true; // set the file into a master copy
            int sentFrom = ((int?)pack[1]).Value; // get sender ID

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
        /// <param name="ev"> a Sim_event object
        /// @pre ev != null
        /// @post $none </param>
        protected internal virtual void processPingRequest(SimEvent ev)
        {
            InfoPacket pkt = (InfoPacket)ev.Data;
            pkt.Tag = CloudSimTags.INFOPKT_RETURN;
            pkt.DestId = pkt.SrcId;

            // sends back to the sender
            sendNow(pkt.SrcId, CloudSimTags.INFOPKT_RETURN, pkt);
        }

        /// <summary>
        /// Process the event for an User/Broker who wants to know the status of a Cloudlet. This
        /// PowerDatacenter will then send the status back to the User/Broker.
        /// </summary>
        /// <param name="ev"> a Sim_event object
        /// @pre ev != null
        /// @post $none </param>
        protected internal virtual void processCloudletStatus(SimEvent ev)
        {
            int cloudletId = 0;
            int userId = 0;
            int vmId = 0;
            int containerId = 0;
            int status = -1;

            try
            {
                // if a sender using cloudletXXX() methods
                int[] data = (int[])ev.Data;
                cloudletId = data[0];
                userId = data[1];
                vmId = data[2];
                containerId = data[3];
                //Log.printLine("Data Center is processing the cloudletStatus Event ");
                status = VmAllocationPolicy.getHost(vmId, userId).getContainerVm(vmId, userId).getContainer(containerId, userId).ContainerCloudletScheduler.getCloudletStatus(cloudletId);
            }

            // if a sender using normal send() methods
            catch (System.InvalidCastException)
            {
                try
                {
                    ContainerCloudlet cl = (ContainerCloudlet)ev.Data;
                    cloudletId = cl.CloudletId;
                    userId = cl.UserId;
                    containerId = cl.ContainerId;

                    status = VmAllocationPolicy.getHost(vmId, userId).getContainerVm(vmId, userId).getContainer(containerId, userId).ContainerCloudletScheduler.getCloudletStatus(cloudletId);
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
        /// Here all the method related to VM requests will be received and forwarded to the related
        /// method.
        /// </summary>
        /// <param name="ev"> the received event
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
        /// Process the event for a User/Broker who wants to create a VM in this PowerDatacenter. This
        /// PowerDatacenter will then send the status back to the User/Broker.
        /// </summary>
        /// <param name="ev">  a Sim_event object </param>
        /// <param name="ack"> the ack
        /// @pre ev != null
        /// @post $none </param>
        protected internal virtual void processVmCreate(SimEvent ev, bool ack)
        {
            ContainerVm containerVm = (ContainerVm)ev.Data;

            bool result = VmAllocationPolicy.allocateHostForVm(containerVm);

            if (ack)
            {
                int[] data = new int[3];
                data[0] = Id;
                data[1] = containerVm.Id;

                if (result)
                {
                    data[2] = CloudSimTags.TRUE;
                }
                else
                {
                    data[2] = CloudSimTags.FALSE;
                }
                send(containerVm.UserId, CloudSim.MinTimeBetweenEvents, CloudSimTags.VM_CREATE_ACK, data);
            }

            if (result)
            {
                ContainerVmListProperty.Add(containerVm);

                if (containerVm.BeingInstantiated)
                {
                    containerVm.BeingInstantiated = false;
                }

                containerVm.updateVmProcessing(CloudSim.clock(), VmAllocationPolicy.getHost(containerVm).ContainerVmScheduler.getAllocatedMipsForContainerVm(containerVm));
            }

        }

        /// <summary>
        /// Process the event for a User/Broker who wants to destroy a VM previously created in this
        /// PowerDatacenter. This PowerDatacenter may send, upon request, the status back to the
        /// User/Broker.
        /// </summary>
        /// <param name="ev">  a Sim_event object </param>
        /// <param name="ack"> the ack
        /// @pre ev != null
        /// @post $none </param>
        protected internal virtual void processVmDestroy(SimEvent ev, bool ack)
        {
            ContainerVm containerVm = (ContainerVm)ev.Data;
            VmAllocationPolicy.deallocateHostForVm(containerVm);

            if (ack)
            {
                int[] data = new int[3];
                data[0] = Id;
                data[1] = containerVm.Id;
                data[2] = CloudSimTags.TRUE;

                sendNow(containerVm.UserId, CloudSimTags.VM_DESTROY_ACK, data);
            }

            ContainerVmListProperty.Remove(containerVm);
        }

        /// <summary>
        /// Process the event for a User/Broker who wants to migrate a VM. This PowerDatacenter will
        /// then send the status back to the User/Broker.
        /// </summary>
        /// <param name="ev"> a Sim_event object
        /// @pre ev != null
        /// @post $none </param>
        protected internal virtual void processVmMigrate(SimEvent ev, bool ack)
        {
            object tmp = ev.Data;
            // TEST: (fixed) Is this check necessary?
            //if (!(tmp is IDictionary<object, ?>))
            //{
            //    throw new System.InvalidCastException("The data object must be Map<String, Object>");
            //}
            if (!(tmp is Dictionary<string, object>))
            {
                throw new ArgumentException("The data object must be Dictionary<String, Object>", "ev.Data");
            }

            IDictionary<string, object> migrate = (Dictionary<string, object>)tmp;

            ContainerVm containerVm = (ContainerVm)migrate["vm"];
            ContainerHost host = (ContainerHost)migrate["host"];

            VmAllocationPolicy.deallocateHostForVm(containerVm);
            host.removeMigratingInContainerVm(containerVm);
            bool result = VmAllocationPolicy.allocateHostForVm(containerVm, host);
            if (!result)
            {
                Log.printLine("[Datacenter.processVmMigrate] VM allocation to the destination host failed");
                //Environment.Exit(0);
                throw new InvalidOperationException("Datacenter.processVmMigrate] VM allocation to the destination host failed");
            }

            if (ack)
            {
                int[] data = new int[3];
                data[0] = Id;
                data[1] = containerVm.Id;

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

            Log.formatLine("%.2f: Migration of VM #%d to Host #%d is completed", CloudSim.clock(), containerVm.Id, host.Id);
            containerVm.InMigration = false;
        }

        /// <summary>
        /// Process the event for a User/Broker who wants to migrate a VM. This PowerDatacenter will
        /// then send the status back to the User/Broker.
        /// </summary>
        /// <param name="ev"> a Sim_event object
        /// @pre ev != null
        /// @post $none </param>
        protected internal virtual void processContainerMigrate(SimEvent ev, bool ack)
        {

            object tmp = ev.Data;
            // TEST: (fixed) Is this check necessary?
            //if (!(tmp is IDictionary<object, ?>))
            //{
            //    throw new System.InvalidCastException("The data object must be Map<String, Object>");
            //}
            if (!(tmp is Dictionary<string, object>))
            {
                throw new ArgumentException("The data object must be Dictionary<String, Object>", "ev.Data");
            }

            IDictionary<string, object> migrate = (Dictionary<string, object>)tmp;

            Container container = (Container)migrate["container"];
            ContainerVm containerVm = (ContainerVm)migrate["vm"];

            ContainerAllocationPolicy.deallocateVmForContainer(container);
            if (containerVm.ContainersMigratingIn.Contains(container))
            {
                containerVm.removeMigratingInContainer(container);
            }
            bool result = ContainerAllocationPolicy.allocateVmForContainer(container, containerVm);
            if (!result)
            {
                Log.printLine("[Datacenter.processContainerMigrate]Container allocation to the destination vm failed");
                //Environment.Exit(0);
                throw new Exception("[Datacenter.processContainerMigrate]Container allocation to the destination vm failed");
            }
            if (containerVm.InWaiting)
            {
                containerVm.InWaiting = false;

            }

            if (ack)
            {
                int[] data = new int[3];
                data[0] = Id;
                data[1] = container.Id;

                if (result)
                {
                    data[2] = CloudSimTags.TRUE;
                }
                else
                {
                    data[2] = CloudSimTags.FALSE;
                }
                sendNow(ev.Source, containerCloudSimTags.CONTAINER_CREATE_ACK, data);
            }

            Log.formatLine("%.2f: Migration of container #%d to Vm #%d is completed", CloudSim.clock(), container.Id, container.Vm.Id);
            container.InMigration = false;
        }

        /// <summary>
        /// Processes a Cloudlet based on the event type.
        /// </summary>
        /// <param name="ev">   a Sim_event object </param>
        /// <param name="type"> event type
        /// @pre ev != null
        /// @pre type > 0
        /// @post $none </param>
        protected internal virtual void processCloudlet(SimEvent ev, int type)
        {
            int cloudletId = 0;
            int userId = 0;
            int vmId = 0;
            int containerId = 0;

            try
            { // if the sender using cloudletXXX() methods
                int[] data = (int[])ev.Data;
                cloudletId = data[0];
                userId = data[1];
                vmId = data[2];
                containerId = data[3];
            }

            // if the sender using normal send() methods
            catch (System.InvalidCastException)
            {
                try
                {
                    ContainerCloudlet cl = (ContainerCloudlet)ev.Data;
                    cloudletId = cl.CloudletId;
                    userId = cl.UserId;
                    vmId = cl.VmId;
                    containerId = cl.ContainerId;
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
                    processCloudletCancel(cloudletId, userId, vmId, containerId);
                    break;

                case CloudSimTags.CLOUDLET_PAUSE:
                    processCloudletPause(cloudletId, userId, vmId, containerId, false);
                    break;

                case CloudSimTags.CLOUDLET_PAUSE_ACK:
                    processCloudletPause(cloudletId, userId, vmId, containerId, true);
                    break;

                case CloudSimTags.CLOUDLET_RESUME:
                    processCloudletResume(cloudletId, userId, vmId, containerId, false);
                    break;

                case CloudSimTags.CLOUDLET_RESUME_ACK:
                    processCloudletResume(cloudletId, userId, vmId, containerId, true);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Process the event for a User/Broker who wants to move a Cloudlet.
        /// </summary>
        /// <param name="receivedData"> information about the migration </param>
        /// <param name="type">         event tag
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
            int containerId = array[3];
            int vmDestId = array[4];
            int containerDestId = array[5];
            int destId = array[6];

            // get the cloudlet
            Cloudlet cl = VmAllocationPolicy.getHost(vmId, userId).getContainerVm(vmId, userId).getContainer(containerId, userId).ContainerCloudletScheduler.cloudletCancel(cloudletId);

            bool failed = false;
            if (cl == null)
            { // cloudlet doesn't exist
                failed = true;
            }
            else
            {
                // has the cloudlet already finished?
                if (cl.CloudletStatusString.Equals("Success"))
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
                    ContainerVm containerVm = VmAllocationPolicy.getHost(vmDestId, userId).getContainerVm(vmDestId, userId);
                    if (containerVm == null)
                    {
                        failed = true;
                    }
                    else
                    {
                        // time to transfer the files
                        double fileTransferTime = predictFileTransferTime(cl.RequiredFiles);
                        containerVm.getContainer(containerDestId, userId).ContainerCloudletScheduler.cloudletSubmit(cl, fileTransferTime);
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
        /// <param name="ev">  a SimEvent object </param>
        /// <param name="ack"> an acknowledgement
        /// @pre ev != null
        /// @post $none </param>
        protected internal virtual void processCloudletSubmit(SimEvent ev, bool ack)
        {
            updateCloudletProcessing();

            try
            {
                ContainerCloudlet cl = (ContainerCloudlet)ev.Data;

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
                int containerId = cl.ContainerId;

                // time to transfer the files
                double fileTransferTime = predictFileTransferTime(cl.RequiredFiles);

                ContainerHost host = VmAllocationPolicy.getHost(vmId, userId);
                ContainerVm vm = host.getContainerVm(vmId, userId);
                Container container = vm.getContainer(containerId, userId);
                double estimatedFinishTime = container.ContainerCloudletScheduler.cloudletSubmit(cl, fileTransferTime);

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
                Log.printLine(string.Format("{0}.processCloudletSubmit(): ClassCastException error.", Name));
                Debug.WriteLine(c.ToString());
                Debug.WriteLine(c.StackTrace);
            }
            catch (Exception e)
            {
                Log.printLine(string.Format("{0}.processCloudletSubmit(): Exception error.", Name));
                Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
            }

            checkCloudletCompletion();
        }

        /// <summary>
        /// Predict file transfer time.
        /// </summary>
        /// <param name="requiredFiles"> the required files </param>
        /// <returns> the double </returns>
        protected internal virtual double predictFileTransferTime(IList<string> requiredFiles)
        {
            double time = 0.0;

            foreach (string fileName in requiredFiles)
            {
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
        /// <param name="cloudletId"> resuming cloudlet ID </param>
        /// <param name="userId">     ID of the cloudlet's owner </param>
        /// <param name="ack">        $true if an ack is requested after operation </param>
        /// <param name="vmId">       the vm id
        /// @pre $none
        /// @post $none </param>
        protected internal virtual void processCloudletResume(int cloudletId, int userId, int vmId, int containerId, bool ack)
        {
            double eventTime = VmAllocationPolicy.getHost(vmId, userId).getContainerVm(vmId, userId).getContainer(containerId, userId).ContainerCloudletScheduler.cloudletResume(cloudletId);

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
        /// <param name="cloudletId"> resuming cloudlet ID </param>
        /// <param name="userId">     ID of the cloudlet's owner </param>
        /// <param name="ack">        $true if an ack is requested after operation </param>
        /// <param name="vmId">       the vm id
        /// @pre $none
        /// @post $none </param>
        protected internal virtual void processCloudletPause(int cloudletId, int userId, int vmId, int containerId, bool ack)
        {
            bool status = VmAllocationPolicy.getHost(vmId, userId).getContainerVm(vmId, userId).getContainer(containerId, userId).ContainerCloudletScheduler.cloudletPause(cloudletId);

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
        /// <param name="cloudletId"> resuming cloudlet ID </param>
        /// <param name="userId">     ID of the cloudlet's owner </param>
        /// <param name="vmId">       the vm id
        /// @pre $none
        /// @post $none </param>
        protected internal virtual void processCloudletCancel(int cloudletId, int userId, int vmId, int containerId)
        {
            Cloudlet cl = VmAllocationPolicy.getHost(vmId, userId).getContainerVm(vmId, userId).getContainer(containerId, userId).ContainerCloudletScheduler.cloudletCancel(cloudletId);
            sendNow(userId, CloudSimTags.CLOUDLET_CANCEL, cl);
        }

        /// <summary>
        /// Updates processing of each cloudlet running in this PowerDatacenter. It is necessary because
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
                IList<ContainerHost> list = VmAllocationPolicy.ContainerHostListProperty;
                double smallerTime = double.MaxValue;
                // for each host...
                for (int i = 0; i < list.Count; i++)
                {
                    ContainerHost host = list[i];
                    // inform VMs to update processing
                    double time = host.updateContainerVmsProcessing(CloudSim.clock());
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
        /// Verifies if some cloudlet inside this PowerDatacenter already finished. If yes, send it to
        /// the User/Broker
        /// 
        /// @pre $none
        /// @post $none
        /// </summary>
        protected internal virtual void checkCloudletCompletion()
        {
            IList<ContainerHost> list = VmAllocationPolicy.ContainerHostListProperty;
            for (int i = 0; i < list.Count; i++)
            {
                ContainerHost host = list[i];
                foreach (ContainerVm vm in host.VmListProperty)
                {
                    foreach (Container container in vm.ContainerListProperty)
                    {
                        while (container.ContainerCloudletScheduler.FinishedCloudlets)
                        {
                            Cloudlet cl = container.ContainerCloudletScheduler.NextFinishedCloudlet;
                            if (cl != null)
                            {
                                sendNow(cl.UserId, CloudSimTags.CLOUDLET_RETURN, cl);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds a file into the resource's storage before the experiment starts. If the file is a master
        /// file, then it will be registered to the RC when the experiment begins.
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
        /// Checks whether the resource has the given file.
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
        /// Checks whether the resource has the given file.
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
        /// Deletes the file from the storage. Also, check whether it is possible to delete the file from
        /// the storage.
        /// </summary>
        /// <param name="fileName"> the name of the file to be deleted </param>
        /// <returns> the error message </returns>
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
            // this resource should register to regional GIS.
            // However, if not specified, then register to system GIS (the
            // default CloudInformationService) entity.
            int gisID = CloudSim.getEntityId(regionalCisName);
            if (gisID == -1)
            {
                gisID = CloudSim.CloudInfoServiceEntityId;
            }

            // send the registration to GIS
            sendNow(gisID, CloudSimTags.REGISTER_RESOURCE, Id);
            // Below method is for a child class to override
            registerOtherEntity();
        }

        /// <summary>
        /// Gets the host list.
        /// </summary>
        /// <returns> the host list </returns>
        public virtual IList<ContainerHost> HostListProperty
        {
            get
            {
                return Characteristics.HostListProperty;
            }
        }

        /// <summary>
        /// Gets the characteristics.
        /// </summary>
        /// <returns> the characteristics </returns>
        protected internal virtual ContainerDatacenterCharacteristics Characteristics
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
        /// Gets the regional cis name.
        /// </summary>
        /// <returns> the regional cis name </returns>
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
        public virtual ContainerVmAllocationPolicy VmAllocationPolicy
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
        /// Gets the last process time.
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
        public IList<ContainerVm> ContainerVmListProperty
        {
            get

            {
                return (IList<ContainerVm>)containerVmList;
            }
            set

            {
                this.containerVmList = value;
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



        public virtual ContainerAllocationPolicy ContainerAllocationPolicy
        {
            get
            {
                return containerAllocationPolicy;
            }
            set
            {
                this.containerAllocationPolicy = value;
            }
        }



        public virtual IList<Container> ContainerList
        {
            get

            {
                return (IList<Container>)containerList;
            }
            set

            {
                this.containerList = value;
            }
        }



        public virtual string ExperimentName
        {
            get
            {
                return experimentName;
            }
            set
            {
                this.experimentName = value;
            }
        }


        public virtual string LogAddress
        {
            get
            {
                return logAddress;
            }
            set
            {
                this.logAddress = value;
            }
        }
    }
}