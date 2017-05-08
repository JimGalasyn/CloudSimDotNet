using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power
{


    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;
    using PredicateType = org.cloudbus.cloudsim.core.predicates.PredicateType;
    using System.Diagnostics;

    /// <summary>
    /// PowerDatacenter is a class that enables simulation of power-aware data centers.
    /// 
    /// <br/>If you are using any algorithms, policies or workload included in the power package please cite
    /// the following paper:<br/>
    /// 
    /// <ul>
    /// <li><a href="http://dx.doi.org/10.1002/cpe.1867">Anton Beloglazov, and Rajkumar Buyya, "Optimal Online Deterministic Algorithms and Adaptive
    /// Heuristics for Energy and Performance Efficient Dynamic Consolidation of Virtual Machines in
    /// Cloud Data Centers", Concurrency and Computation: Practice and Experience (CCPE), Volume 24,
    /// Issue 13, Pages: 1397-1420, John Wiley & Sons, Ltd, New York, USA, 2012</a>
    /// </ul>
    /// 
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 2.0
    /// </summary>
    public class PowerDatacenter : Datacenter
	{

		/// <summary>
		/// The datacenter consumed power. </summary>
		private double power;

		/// <summary>
		/// Indicates if migrations are disabled or not. </summary>
		private bool disableMigrations;

		/// <summary>
		/// The last time submitted cloudlets were processed. </summary>
		private double cloudletSubmitted;

		/// <summary>
		/// The VM migration count. </summary>
		private int migrationCount;

		/// <summary>
		/// Instantiates a new PowerDatacenter.
		/// </summary>
		/// <param name="name"> the datacenter name </param>
		/// <param name="characteristics"> the datacenter characteristics </param>
		/// <param name="schedulingInterval"> the scheduling interval </param>
		/// <param name="vmAllocationPolicy"> the vm provisioner </param>
		/// <param name="storageList"> the storage list </param>
		/// <exception cref="Exception"> the exception </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PowerDatacenter(String name, org.cloudbus.cloudsim.DatacenterCharacteristics characteristics, org.cloudbus.cloudsim.VmAllocationPolicy vmAllocationPolicy, java.util.List<org.cloudbus.cloudsim.Storage> storageList, double schedulingInterval) throws Exception
		public PowerDatacenter(string name, DatacenterCharacteristics characteristics, VmAllocationPolicy vmAllocationPolicy, IList<Storage> storageList, double schedulingInterval) : base(name, characteristics, vmAllocationPolicy, storageList, schedulingInterval)
		{

			Power = 0.0;
			DisableMigrations = false;
			CloudletSubmitted = -1;
			MigrationCount = 0;
		}

		protected internal override void updateCloudletProcessing()
		{
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
					IList<IDictionary<string, object>> migrationMap = VmAllocationPolicy.optimizeAllocation(VmListProperty);

					if (migrationMap != null)
					{
						foreach (IDictionary<string, object> migrate in migrationMap)
						{
							Vm vm = (Vm) migrate["vm"];
							PowerHost targetHost = (PowerHost) migrate["host"];
							PowerHost oldHost = (PowerHost) vm.Host;

							if (oldHost == null)
							{
								Log.formatLine("%.2f: Migration of VM #%d to Host #%d is started", currentTime, vm.Id, targetHost.Id);
							}
							else
							{
								Log.formatLine("%.2f: Migration of VM #%d from Host #%d to Host #%d is started", currentTime, vm.Id, oldHost.Id, targetHost.Id);
							}

							targetHost.addMigratingInVm(vm);
							incrementMigrationCount();

							/// <summary>
							/// VM migration delay = RAM / bandwidth * </summary>
							// we use BW / 2 to model BW available for migration purposes, the other
							// half of BW is for VM communication
							// around 16 seconds for 1024 MB using 1 Gbit/s network
							send(Id, vm.Ram / ((double) targetHost.Bw / (2 * 8000)), CloudSimTags.VM_MIGRATE, migrate);
						}
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

		/// <summary>
		/// Update cloudet processing without scheduling future events.
		/// </summary>
		/// <returns> the double </returns>
		/// <seealso cref= #updateCloudetProcessingWithoutSchedulingFutureEventsForce() 
		/// @todo There is an inconsistence in the return value of this
		/// method with return value of similar methods
		/// such as <seealso cref="#updateCloudetProcessingWithoutSchedulingFutureEventsForce()"/>,
		/// that returns <seealso cref="Double#MAX_VALUE"/> by default.
		/// The current method returns 0 by default. </seealso>
		protected internal virtual double updateCloudetProcessingWithoutSchedulingFutureEvents()
		{
			if (CloudSim.clock() > LastProcessTime)
			{
				return updateCloudetProcessingWithoutSchedulingFutureEventsForce();
			}
			return 0;
		}

		/// <summary>
		/// Update cloudet processing without scheduling future events.
		/// </summary>
		/// <returns> expected time of completion of the next cloudlet in all VMs of all hosts or
		///         <seealso cref="Double#MAX_VALUE"/> if there is no future events expected in this host </returns>
		protected internal virtual double updateCloudetProcessingWithoutSchedulingFutureEventsForce()
		{
			double currentTime = CloudSim.clock();
			double minTime = double.MaxValue;
			double timeDiff = currentTime - LastProcessTime;
			double timeFrameDatacenterEnergy = 0.0;

			Log.printLine("\n\n--------------------------------------------------------------\n\n");
			Log.formatLine("New resource usage for the time frame starting at %.2f:", currentTime);

			foreach (PowerHost host in this.HostListProperty)
			{
				Log.printLine();

				double time = host.updateVmsProcessing(currentTime); // inform VMs to update processing
				if (time < minTime)
				{
					minTime = time;
				}

				Log.formatLine("%.2f: [Host #%d] utilization is %.2f%%", currentTime, host.Id, host.UtilizationOfCpu * 100);
			}

			if (timeDiff > 0)
			{
				Log.formatLine("\nEnergy consumption for the last time frame from %.2f to %.2f:", LastProcessTime, currentTime);

				foreach (PowerHost host in this.HostListProperty)
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
			}

			Power = Power + timeFrameDatacenterEnergy;

			checkCloudletCompletion();

			/// <summary>
			/// Remove completed VMs * </summary>
			foreach (PowerHost host in this.HostListProperty)
			{
				foreach (Vm vm in host.CompletedVms)
				{
					VmAllocationPolicy.deallocateHostForVm(vm);
					VmListProperty.Remove(vm);
					Log.printLine("VM #" + vm.Id + " has been deallocated from host #" + host.Id);
				}
			}

			Log.printLine();

			LastProcessTime = currentTime;
			return minTime;
		}

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

		protected internal override void processCloudletSubmit(SimEvent ev, bool ack)
		{
			base.processCloudletSubmit(ev, ack);
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
		/// <returns> true, if PowerDatacenter is in migration; false otherwise </returns>
		protected internal virtual bool InMigration
		{
			get
			{
				bool result = false;
				foreach (Vm vm in VmListProperty)
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
		/// Checks if migrations are disabled.
		/// </summary>
		/// <returns> true, if  migrations are disable; false otherwise </returns>
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
		public virtual int MigrationCount
		{
			get
			{
				return migrationCount;
			}
			set
			{
				this.migrationCount = value;
			}
		}


		/// <summary>
		/// Increment migration count.
		/// </summary>
		protected internal virtual void incrementMigrationCount()
		{
			MigrationCount = MigrationCount + 1;
		}

	}

}