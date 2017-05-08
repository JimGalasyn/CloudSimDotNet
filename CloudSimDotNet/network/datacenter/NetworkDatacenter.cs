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
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;

    /// <summary>
    /// NetworkDatacenter class is a <seealso cref="Datacenter"/> whose hostList are virtualized and networked. It contains
    /// all the information about internal network. For example, which VM is connected to what switch etc. It
    /// deals with processing of VM queries (i.e., handling of VMs) instead of processing
    /// Cloudlet-related queries. So, even though an AllocPolicy will be instantiated (in the init()
    /// method of the superclass, it will not be used, as processing of cloudlets are handled by the
    /// CloudletScheduler and processing of VirtualMachines are handled by the VmAllocationPolicy.
    /// 
    /// @todo If an AllocPolicy is not being used, why it is being created. Perhaps 
    /// a better class hierarchy should be created, introducing some abstract class
    /// or interface.
    /// 
    /// <br/>Please refer to following publication for more details:<br/>
    /// <ul>
    /// <li><a href="http://dx.doi.org/10.1109/UCC.2011.24">Saurabh Kumar Garg and Rajkumar Buyya, NetworkCloudSim: Modelling Parallel Applications in Cloud
    /// Simulations, Proceedings of the 4th IEEE/ACM International Conference on Utility and Cloud
    /// Computing (UCC 2011, IEEE CS Press, USA), Melbourne, Australia, December 5-7, 2011.</a>
    /// </ul>
    /// 
    /// @author Saurabh Kumar Garg
    /// @since CloudSim Toolkit 3.0
    /// </summary>
    public class NetworkDatacenter : Datacenter
	{
			/// <summary>
			/// A map between VMs and Switches, where each key
			/// is a VM id and the corresponding value is the id of the switch where the VM is connected to.
			/// </summary>
		public IDictionary<int?, int?> VmToSwitchid = new Dictionary<int?, int?>();

			/// <summary>
			/// A map between hosts and Switches, where each key
			/// is a host id and the corresponding value is the id of the switch where the host is connected to.
			/// </summary>
		public IDictionary<int?, int?> HostToSwitchid;

			/// <summary>
			/// A map of datacenter switches where each key is a switch id
			/// and the corresponding value is the switch itself.
			/// </summary>
		public IDictionary<int?, Switch> Switchlist;

			/// <summary>
			/// A map between VMs and Hosts, where each key
			/// is a VM id and the corresponding value is the id of the host where the VM is placed.
			/// </summary>
		public IDictionary<int?, int?> VmtoHostlist;

		/// <summary>
		/// Instantiates a new NetworkDatacenter object.
		/// </summary>
		/// <param name="name"> the name to be associated with this entity (as required by <seealso cref="org.cloudbus.cloudsim.core.SimEntity"/>) </param>
		/// <param name="characteristics"> the datacenter characteristics </param>
		/// <param name="vmAllocationPolicy"> the vmAllocationPolicy </param>
		/// <param name="storageList"> a List of storage elements, for data simulation </param>
		/// <param name="schedulingInterval"> the scheduling delay to process each datacenter received event
		/// </param>
		/// <exception cref="Exception">  when one of the following scenarios occur:
		///         <ul>
		///         <li>creating this entity before initializing CloudSim package
		///         <li>this entity name is <tt>null</tt> or empty
		///         <li>this entity has <tt>zero</tt> number of PEs (Processing Elements). <br>
		///         No PEs mean the Cloudlets can't be processed. A CloudResource must contain one or
		///         more Machines. A Machine must contain one or more PEs.
		///         </ul>
		/// 
		/// @pre name != null
		/// @pre resource != null
		/// @post $none </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public NetworkDatacenter(String name, org.cloudbus.cloudsim.DatacenterCharacteristics characteristics, org.cloudbus.cloudsim.VmAllocationPolicy vmAllocationPolicy, java.util.List<org.cloudbus.cloudsim.Storage> storageList, double schedulingInterval) throws Exception
		public NetworkDatacenter(string name, DatacenterCharacteristics characteristics, VmAllocationPolicy vmAllocationPolicy, IList<Storage> storageList, double schedulingInterval) : base(name, characteristics, vmAllocationPolicy, storageList, schedulingInterval)
		{
			VmToSwitchid = new Dictionary<int?, int?>();
			HostToSwitchid = new Dictionary<int?, int?>();
			VmtoHostlist = new Dictionary<int?, int?>();
			Switchlist = new Dictionary<int?, Switch>();
		}

		/// <summary>
		/// Gets a map of all EdgeSwitches in the Datacenter network. 
		/// One can design similar functions for other type of switches.
		/// </summary>
		/// <returns> a EdgeSwitches map, where each key is the switch id
		/// and each value it the switch itself. </returns>
		public virtual IDictionary<int?, Switch> EdgeSwitch
		{
			get
			{
				IDictionary<int?, Switch> edgeswitch = new Dictionary<int?, Switch>();
				foreach (KeyValuePair<int?, Switch> es in Switchlist.SetOfKeyValuePairs())
				{
					if (es.Value.level == NetworkConstants.EDGE_LEVEL)
					{
						edgeswitch[es.Key] = es.Value;
					}
				}
				return edgeswitch;
    
			}
		}

		/// <summary>
		/// Creates the given VM within the NetworkDatacenter. 
		/// It can be directly accessed by Datacenter Broker which manages allocation of Cloudlets.
		/// </summary>
		/// <param name="vm"> </param>
		/// <returns> true if the VW was created successfully, false otherwise </returns>
		public virtual bool processVmCreateNetwork(Vm vm)
		{

			bool result = VmAllocationPolicy.allocateHostForVm(vm);

			if (result)
			{
				VmToSwitchid[vm.Id] = ((NetworkHost) vm.Host).sw.Id;
				VmtoHostlist[vm.Id] = vm.Host.Id;
				Debug.WriteLine(vm.Id + " VM is created on " + vm.Host.Id);

				VmListProperty.Add(vm);

				vm.updateVmProcessing(CloudSim.clock(), VmAllocationPolicy.getHost(vm).VmScheduler.getAllocatedMipsForVm(vm));
			}
			return result;
		}

		protected internal override void processCloudletSubmit(SimEvent ev, bool ack)
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

				if (estimatedFinishTime > 0.0)
				{ // if this cloudlet is in the exec
					// time to process the cloudlet
					estimatedFinishTime += fileTransferTime;
					send(Id, estimatedFinishTime, CloudSimTags.VM_DATACENTER_EVENT);

					// event to update the stages
					send(Id, 0.0001, CloudSimTags.VM_DATACENTER_EVENT);
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

	}

}