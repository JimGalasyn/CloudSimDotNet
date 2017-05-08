using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

    //import cloudSimGr.containerCloudSim.Experiments.HelperEx;
    //import cloudSimGr.containerCloudSim.Experiments.Paper1.RunnerAbs;
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
    /// Created by sareh on 20/07/15.
    /// </summary>
    public class PowerContainerDatacenter : ContainerDatacenter
	{


		/// <summary>
		/// The power.
		/// </summary>
		private double power;

		/// <summary>
		/// The disable migrations.
		/// </summary>
		private bool disableVmMigrations;

		/// <summary>
		/// The cloudlet submited.
		/// </summary>
		private double cloudletSubmitted;

		/// <summary>
		/// The migration count.
		/// </summary>
		private int vmMigrationCount;

		private IList<double?> activeVmList;
		private int numberOfVms;

		private int numberOfContainers;

		private IList<double?> activeHostList;
		private IList<double?> datacenterEnergyList;
		private IList<double?> containerMigrationList;



		private CostumeCSVWriter vmMigrationWriter;
		private CostumeCSVWriter containerMigrationWriter;
		private CostumeCSVWriter datacenterEnergyWriter;

		/// <summary>
		/// Instantiates a new datacenter.
		/// </summary>
		/// <param name="name">               the name </param>
		/// <param name="characteristics">    the res config </param>
		/// <param name="schedulingInterval"> the scheduling interval </param>
		///                           //         * <param name="utilizationBound"> the utilization bound </param>
		/// <param name="vmAllocationPolicy"> the vm provisioner </param>
		/// <param name="storageList">        the storage list </param>
		/// <exception cref="Exception"> the exception </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PowerContainerDatacenter(String name, ContainerDatacenterCharacteristics characteristics, org.cloudbus.cloudsim.container.resourceAllocators.ContainerVmAllocationPolicy vmAllocationPolicy, org.cloudbus.cloudsim.container.resourceAllocators.ContainerAllocationPolicy containerAllocationPolicy, java.util.List<org.cloudbus.cloudsim.Storage> storageList, double schedulingInterval, String experimentName, String logAddress) throws Exception
		public PowerContainerDatacenter(string name, ContainerDatacenterCharacteristics characteristics, ContainerVmAllocationPolicy vmAllocationPolicy, ContainerAllocationPolicy containerAllocationPolicy, IList<Storage> storageList, double schedulingInterval, string experimentName, string logAddress) : base(name, characteristics, vmAllocationPolicy, containerAllocationPolicy, storageList, schedulingInterval, experimentName, logAddress)
		{
			string containerMigrationAddress;
			string vmMigrationAddress;
			int index = ExperimentName.LastIndexOf("_");
			containerMigrationAddress = string.Format("{0}/ContainerMigration/{1}/{2}.csv",LogAddress, ExperimentName.Substring(0,index),ExperimentName);
			string energyConsumptionAddress = string.Format("{0}/EnergyConsumption/{1}/{2}.csv", LogAddress, ExperimentName.Substring(0,index),ExperimentName);
			vmMigrationAddress = string.Format("{0}/ContainerMigration/{1}/VM-{2}.csv", LogAddress, ExperimentName.Substring(0,index),ExperimentName);
			ContainerMigrationWriter = new CostumeCSVWriter(containerMigrationAddress);
			VmMigrationWriter = new CostumeCSVWriter(vmMigrationAddress);
			DatacenterEnergyWriter = new CostumeCSVWriter(energyConsumptionAddress);
			Power = 0.0;
			DisableVmMigrations = false;
			CloudletSubmitted = -1;
			VmMigrationCount = 0;
			setActiveHostList(new List<double?>());
			ActiveVmList = new List<double?>();
			DatacenterEnergyList = new List<double?>();
			ContainerMigrationList = new List<double?>();
			NumberOfVms = 0;
			NumberOfContainers = 0;
		}

		/// <summary>
		/// Updates processing of each cloudlet running in this PowerDatacenter. It is necessary because
		/// Hosts and VirtualMachines are simple objects, not entities. So, they don't receive events and
		/// updating cloudlets inside them must be called from the outside.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
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

				if (!DisableVmMigrations)
				{
					IList<IDictionary<string, object>> migrationMap = VmAllocationPolicy.optimizeAllocation(ContainerVmListProperty);
					int previousMigrationCount = VmMigrationCount;
					if (migrationMap != null)
					{
						foreach (IDictionary<string, object> migrate in migrationMap)
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
					Log.printConcatLine(CloudSim.clock(), ": The number of Migrations is:  ", VmMigrationCount - previousMigrationCount);
	//                String[] msg={Double.toString (CloudSim.clock()), Integer.toString (getVmMigrationCount() - previousMigrationCount)  } ;                   // <--declared statement
	//                try {
	//                    getVmMigrationWriter().writeTofile(msg);
	//                } catch (IOException e) {
	//                    e.printStackTrace();
	//                }
	//                Log.printConcatLine(CloudSim.clock(),": The total number of Migrations is:  ",getVmMigrationCount());
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

		/// <summary>
		/// Update cloudet processing without scheduling future events.
		/// </summary>
		/// <returns> the double </returns>
		protected internal virtual double updateCloudetProcessingWithoutSchedulingFutureEvents()
		{
	//        Log.printLine("Power data center is Updating CloudetProcessingWithoutSchedulingFutureEvents");
			if (CloudSim.clock() > LastProcessTime)
			{
				return updateCloudetProcessingWithoutSchedulingFutureEventsForce();
			}
			return 0;
		}

		/// <summary>
		/// Update cloudet processing without scheduling future events.
		/// </summary>
		/// <returns> the double </returns>
		protected internal virtual double updateCloudetProcessingWithoutSchedulingFutureEventsForce()
		{
	//        Log.printLine("Power data center is Updating CloudetProcessingWithoutSchedulingFutureEventsForce");
			double currentTime = CloudSim.clock();
			double minTime = double.MaxValue;
			double timeDiff = currentTime - LastProcessTime;
			double timeFrameDatacenterEnergy = 0.0;

			Log.printLine("\n\n--------------------------------------------------------------\n\n");
			Log.formatLine("Power data center: New resource usage for the time frame starting at %.2f:", currentTime);

			foreach (PowerContainerHost host in this.HostListProperty)
			{
				Log.printLine();

				double time = host.updateContainerVmsProcessing(currentTime); // inform VMs to update processing
				if (time < minTime)
				{
					minTime = time;
				}

				Log.formatLine("%.2f: [Host #%d] utilization is %.2f%%", currentTime, host.Id, host.UtilizationOfCpu * 100);
			}

			if (timeDiff > 0)
			{
				Log.formatLine("\nEnergy consumption for the last time frame from %.2f to %.2f:", LastProcessTime, currentTime);

                // TEST: (fixed) Deal with this generic type inheritance issue.
                foreach (PowerContainerHost host in this.HostListProperty)
				{
					double previousUtilizationOfCpu = host.PreviousUtilizationOfCpu;
					double utilizationOfCpu = host.UtilizationOfCpu;
					double timeFrameHostEnergy = host.getEnergyLinearInterpolation(previousUtilizationOfCpu, utilizationOfCpu, timeDiff);
					timeFrameDatacenterEnergy += timeFrameHostEnergy;

					Log.printLine();
					Log.formatLine("%.2f: [Host #%d] utilization at %.2f was %.2f%%, now is %.2f%%", currentTime, host.Id, LastProcessTime, previousUtilizationOfCpu * 100, utilizationOfCpu * 100);
					Log.formatLine("%.2f: [Host #%d] energy is %.2f W*sec", currentTime, host.Id, timeFrameHostEnergy);
				}

				Log.formatLine("\n%.2f: Data center's energy is %.2f W*sec\n", currentTime, timeFrameDatacenterEnergy);
				DatacenterEnergyList.Add(timeFrameDatacenterEnergy);

			}

			Power = Power + timeFrameDatacenterEnergy;

			string[] msg = new string[] {Convert.ToString(currentTime),Convert.ToString(Power)};
			try
			{
				DatacenterEnergyWriter.writeTofile(msg);
			}
			catch (IOException e)
			{
				Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
			}


			checkCloudletCompletion();

			int numberOfActiveHosts = 0;
			/// <summary>
			/// Remove completed VMs * </summary>
			foreach (PowerContainerHost host in this.HostListProperty)
			{
				foreach (ContainerVm vm in host.CompletedVms)
				{
					VmAllocationPolicy.deallocateHostForVm(vm);
					ContainerVmListProperty.Remove(vm);
					Log.printLine(string.Format("VM #{0:D} has been deallocated from host #{1:D}", vm.Id, host.Id));
				}
				if (host.VmListProperty.Count != 0)
				{

					numberOfActiveHosts++;
				}
			}
			updateNumberOfVmsContainers();
			getActiveHostList().Add((double) numberOfActiveHosts);
			int numberOfActiveVms = NumberOfVms;
			ActiveVmList.Add((double) numberOfActiveVms);
			int numberOfContainers = NumberOfContainers;
			/// <summary>
			/// Check how many containers are in the system up and running </summary>
			Log.print(string.Format("The number of Containers Up and running is {0:D}", numberOfContainers));
			Log.printLine();
			Log.print(string.Format("The number of Vms Up and running is {0:D}", numberOfActiveVms));
			Log.printLine();
			Log.print(string.Format("The number of Hosts Up and running is {0:D}", numberOfActiveHosts));
			Log.printLine();

			LastProcessTime = currentTime;
			return minTime;
		}

		/*
		 * (non-Javadoc)
		 * @see org.cloudbus.cloudsim.Datacenter#processVmMigrate(org.cloudbus.cloudsim.core.SimEvent,
		 * boolean)
		 */
		protected internal override void processVmMigrate(SimEvent ev, bool ack)
		{
			updateCloudetProcessingWithoutSchedulingFutureEvents();
			base.processVmMigrate(ev, ack);
			SimEvent @event = CloudSim.findFirstDeferred(Id, new PredicateType(CloudSimTags.VM_MIGRATE));
			if (@event == null || @event.eventTime() > CloudSim.clock())
			{
				updateCloudetProcessingWithoutSchedulingFutureEventsForce();
			}
		}
	//    /*
	//         * (non-Javadoc)
	//         * @see org.cloudbus.cloudsim.Datacenter#processVmMigrate(org.cloudbus.cloudsim.core.SimEvent,
	//         * boolean)
	//         */
	//    @Override
	//    protected void processContainerMigrate(SimEvent ev, boolean ack) {
	//        updateCloudetProcessingWithoutSchedulingFutureEvents();
	//        super.processContainerMigrate(ev, ack);
	//        SimEvent event = CloudSim.findFirstDeferred(getId(), new PredicateType(containerCloudSimCloudTags.CONTAINER_MIGRATE));
	//        if (event == null || event.eventTime() > CloudSim.clock()) {
	//            updateCloudetProcessingWithoutSchedulingFutureEventsForce();
	//        }
	//    }
		/*
		 * (non-Javadoc)
		 * @see cloudsim.Datacenter#processCloudletSubmit(cloudsim.core.SimEvent, boolean)
		 */
		protected internal override void processCloudletSubmit(SimEvent ev, bool ack)
		{
	//        Log.printLine("Power data center is processing cloudlet submit");
			base.processCloudletSubmit(ev, ack);
	//        Log.printLine("Power Data Center : Set Cloudlet Submited to " + CloudSim.clock());
			CloudletSubmitted = CloudSim.clock();
		}

		/// <summary>
		/// Gets the power.
		/// </summary>
		/// <returns> the power </returns>
		public virtual double Power
		{
			get
			{
				return power;
			}
			set
			{
				this.power = value;
			}
		}


		/// <summary>
		/// Checks if PowerDatacenter is in migration.
		/// </summary>
		/// <returns> true, if PowerDatacenter is in migration </returns>
		protected internal virtual bool InMigration
		{
			get
			{
				bool result = false;
				foreach (ContainerVm vm in ContainerVmListProperty)
				{
					if (vm.InMigration)
					{
						result = true;
						break;
					}
				}
				return result;
			}
		}

		/// <summary>
		/// Checks if is disable migrations.
		/// </summary>
		/// <returns> true, if is disable migrations </returns>
		public virtual bool DisableVmMigrations
		{
			get
			{
				return disableVmMigrations;
			}
			set
			{
				this.disableVmMigrations = value;
			}
		}


		/// <summary>
		/// Checks if is cloudlet submited.
		/// </summary>
		/// <returns> true, if is cloudlet submited </returns>
		protected internal virtual double CloudletSubmitted
		{
			get
			{
				return cloudletSubmitted;
			}
			set
			{
				this.cloudletSubmitted = value;
			}
		}


		/// <summary>
		/// Gets the migration count.
		/// </summary>
		/// <returns> the migration count </returns>
		public virtual int VmMigrationCount
		{
			get
			{
				return vmMigrationCount;
			}
			set
			{
				this.vmMigrationCount = value;
			}
		}


		/// <summary>
		/// Increment migration count.
		/// </summary>
		protected internal virtual void incrementMigrationCount()
		{
			VmMigrationCount = VmMigrationCount + 1;
		}

		public virtual CostumeCSVWriter ContainerMigrationWriter
		{
			get
			{
				return containerMigrationWriter;
			}
			set
			{
				this.containerMigrationWriter = value;
			}
		}



		public virtual CostumeCSVWriter DatacenterEnergyWriter
		{
			get
			{
				return (CostumeCSVWriter) datacenterEnergyWriter;
			}
			set
			{
				this.datacenterEnergyWriter = value;
			}
		}


		public virtual IList<double?> ActiveVmList
		{
			get
			{
				return activeVmList;
			}
			set
			{
				this.activeVmList = value;
			}
		}


		public virtual IList<double?> getActiveHostList()
		{
			return activeHostList;
		}

		public virtual void setActiveHostList(List<double?> activeHostList)
		{
			this.activeHostList = activeHostList;
		}

		public virtual IList<double?> DatacenterEnergyList
		{
			get
			{
				return datacenterEnergyList;
			}
			set
			{
				this.datacenterEnergyList = value;
			}
		}

		public virtual IList<double?> ContainerMigrationList
		{
			get
			{
				return containerMigrationList;
			}
			set
			{
				this.containerMigrationList = value;
			}
		}


		public virtual CostumeCSVWriter VmMigrationWriter
		{
			get
			{
				return vmMigrationWriter;
			}
			set
			{
				this.vmMigrationWriter = value;
			}
		}



		public virtual void updateNumberOfVmsContainers()
		{
			NumberOfVms = 0;
			NumberOfContainers = 0;
			IList<ContainerVm> temp = new List<ContainerVm>();
			foreach (ContainerHost host in HostListProperty)
			{
				foreach (ContainerVm vm in host.VmListProperty)
				{
					if (!temp.Contains(vm))
					{
						int tempNumbers = this.NumberOfVms + 1;
						NumberOfVms = tempNumbers;
						tempNumbers = this.NumberOfContainers + vm.NumberOfContainers;
						NumberOfContainers = tempNumbers;
						temp.Add(vm);

					}
				}
			}
			temp.Clear();
		}
		public virtual int NumberOfVms
		{
			get
			{
    
				return numberOfVms;
			}
			set
			{
				this.numberOfVms = value;
			}
		}
		public virtual int NumberOfContainers
		{
			get
			{
    
				return numberOfContainers;
			}
			set
			{
				this.numberOfContainers = value;
			}
		}







	}




}