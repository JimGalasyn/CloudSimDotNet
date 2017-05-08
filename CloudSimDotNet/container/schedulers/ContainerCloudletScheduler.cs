using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.schedulers
{



	/// <summary>
	/// Created by sareh on 10/07/15.
	/// </summary>
	public abstract class ContainerCloudletScheduler
	{
			/// <summary>
			/// The previous time. </summary>
			private double previousTime;

			/// <summary>
			/// The current mips share. </summary>
			private IList<double?> currentMipsShare;

			/// <summary>
			/// The cloudlet waiting list. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends org.cloudbus.cloudsim.ResCloudlet> cloudletWaitingList;
			protected internal IList<ResCloudlet> cloudletWaitingList;

			/// <summary>
			/// The cloudlet exec list. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends org.cloudbus.cloudsim.ResCloudlet> cloudletExecList;
			protected internal IList<ResCloudlet> cloudletExecList;

			/// <summary>
			/// The cloudlet paused list. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends org.cloudbus.cloudsim.ResCloudlet> cloudletPausedList;
			protected internal IList<ResCloudlet> cloudletPausedList;

			/// <summary>
			/// The cloudlet finished list. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends org.cloudbus.cloudsim.ResCloudlet> cloudletFinishedList;
			protected internal IList<ResCloudlet> cloudletFinishedList;

			/// <summary>
			/// The cloudlet failed list. </summary>
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<? extends org.cloudbus.cloudsim.ResCloudlet> cloudletFailedList;
			protected internal IList<ResCloudlet> cloudletFailedList;

			/// <summary>
			/// Creates a new CloudletScheduler object. This method must be invoked before starting the
			/// actual simulation.
			/// 
			/// @pre $none
			/// @post $none
			/// </summary>
			public ContainerCloudletScheduler()
			{
				PreviousTime = 0.0;
				cloudletWaitingList = new List<ResCloudlet>();
				cloudletExecList = new List<ResCloudlet>();
				cloudletPausedList = new List<ResCloudlet>();
				cloudletFinishedList = new List<ResCloudlet>();
				cloudletFailedList = new List<ResCloudlet>();
			}

			/// <summary>
			/// Updates the processing of cloudlets running under management of this scheduler.
			/// </summary>
			/// <param name="currentTime"> current simulation time </param>
			/// <param name="mipsShare"> array with MIPS share of each processor available to the scheduler </param>
			/// <returns> time predicted completion time of the earliest finishing cloudlet, or 0 if there is no
			///         next events
			/// @pre currentTime >= 0
			/// @post $none </returns>
			public abstract double updateContainerProcessing(double currentTime, IList<double?> mipsShare);

			/// <summary>
			/// Receives an cloudlet to be executed in the VM managed by this scheduler.
			/// </summary>
			/// <param name="gl"> the submited cloudlet </param>
			/// <param name="fileTransferTime"> time required to move the required files from the SAN to the VM </param>
			/// <returns> expected finish time of this cloudlet, or 0 if it is in a waiting queue
			/// @pre gl != null
			/// @post $none </returns>
			public abstract double cloudletSubmit(Cloudlet gl, double fileTransferTime);

			/// <summary>
			/// Receives an cloudlet to be executed in the VM managed by this scheduler.
			/// </summary>
			/// <param name="gl"> the submited cloudlet </param>
			/// <returns> expected finish time of this cloudlet, or 0 if it is in a waiting queue
			/// @pre gl != null
			/// @post $none </returns>
			public abstract double cloudletSubmit(Cloudlet gl);

			/// <summary>
			/// Cancels execution of a cloudlet.
			/// </summary>
			/// <param name="clId"> ID of the cloudlet being cancealed </param>
			/// <returns> the canceled cloudlet, $null if not found
			/// @pre $none
			/// @post $none </returns>
			public abstract Cloudlet cloudletCancel(int clId);

			/// <summary>
			/// Pauses execution of a cloudlet.
			/// </summary>
			/// <param name="clId"> ID of the cloudlet being paused </param>
			/// <returns> $true if cloudlet paused, $false otherwise
			/// @pre $none
			/// @post $none </returns>
			public abstract bool cloudletPause(int clId);

			/// <summary>
			/// Resumes execution of a paused cloudlet.
			/// </summary>
			/// <param name="clId"> ID of the cloudlet being resumed </param>
			/// <returns> expected finish time of the cloudlet, 0.0 if queued
			/// @pre $none
			/// @post $none </returns>
			public abstract double cloudletResume(int clId);

			/// <summary>
			/// Processes a finished cloudlet.
			/// </summary>
			/// <param name="rcl"> finished cloudlet
			/// @pre rgl != $null
			/// @post $none </param>
			public abstract void cloudletFinish(ResCloudlet rcl);

			/// <summary>
			/// Gets the status of a cloudlet.
			/// </summary>
			/// <param name="clId"> ID of the cloudlet </param>
			/// <returns> status of the cloudlet, -1 if cloudlet not found
			/// @pre $none
			/// @post $none </returns>
			public abstract int getCloudletStatus(int clId);

			/// <summary>
			/// Informs about completion of some cloudlet in the VM managed by this scheduler.
			/// </summary>
			/// <returns> $true if there is at least one finished cloudlet; $false otherwise
			/// @pre $none
			/// @post $none </returns>
			public abstract bool FinishedCloudlets {get;}

			/// <summary>
			/// Returns the next cloudlet in the finished list, $null if this list is empty.
			/// </summary>
			/// <returns> a finished cloudlet
			/// @pre $none
			/// @post $none </returns>
			public abstract Cloudlet NextFinishedCloudlet {get;}

			/// <summary>
			/// Returns the number of cloudlets runnning in the virtual machine.
			/// </summary>
			/// <returns> number of cloudlets runnning
			/// @pre $none
			/// @post $none </returns>
			public abstract int runningCloudlets();

			/// <summary>
			/// Returns one cloudlet to migrate to another vm.
			/// </summary>
			/// <returns> one running cloudlet
			/// @pre $none
			/// @post $none </returns>
			public abstract Cloudlet migrateCloudlet();

			/// <summary>
			/// Get utilization created by all cloudlets.
			/// </summary>
			/// <param name="time"> the time </param>
			/// <returns> total utilization </returns>
			public abstract double getTotalUtilizationOfCpu(double time);

			/// <summary>
			/// Gets the current requested mips.
			/// </summary>
			/// <returns> the current mips </returns>
			public abstract IList<double?> CurrentRequestedMips {get;}

			/// <summary>
			/// Gets the total current mips for the Cloudlet.
			/// </summary>
			/// <param name="rcl"> the rcl </param>
			/// <param name="mipsShare"> the mips share </param>
			/// <returns> the total current mips </returns>
			public abstract double getTotalCurrentAvailableMipsForCloudlet(ResCloudlet rcl, IList<double?> mipsShare);

			/// <summary>
			/// Gets the total current requested mips for cloudlet.
			/// </summary>
			/// <param name="rcl"> the rcl </param>
			/// <param name="time"> the time </param>
			/// <returns> the total current requested mips for cloudlet </returns>
			public abstract double getTotalCurrentRequestedMipsForCloudlet(ResCloudlet rcl, double time);

			/// <summary>
			/// Gets the total current allocated mips for cloudlet.
			/// </summary>
			/// <param name="rcl"> the rcl </param>
			/// <param name="time"> the time </param>
			/// <returns> the total current allocated mips for cloudlet </returns>
			public abstract double getTotalCurrentAllocatedMipsForCloudlet(ResCloudlet rcl, double time);

			/// <summary>
			/// Gets the current requested ram.
			/// </summary>
			/// <returns> the current requested ram </returns>
			public abstract double CurrentRequestedUtilizationOfRam {get;}

			/// <summary>
			/// Gets the current requested bw.
			/// </summary>
			/// <returns> the current requested bw </returns>
			public abstract double CurrentRequestedUtilizationOfBw {get;}

			/// <summary>
			/// Gets the previous time.
			/// </summary>
			/// <returns> the previous time </returns>
			public virtual double PreviousTime
			{
				get
				{
					return previousTime;
				}
				set
				{
					this.previousTime = value;
				}
			}


			/// <summary>
			/// Sets the current mips share.
			/// </summary>
			/// <param name="currentMipsShare"> the new current mips share </param>
			protected internal virtual IList<double?> CurrentMipsShare
			{
				set
				{
					this.currentMipsShare = value;
				}
				get
				{
					return currentMipsShare;
				}
			}


			/// <summary>
			/// Gets the cloudlet waiting list.
			/// </summary>
			/// @param <T> the generic type </param>
			/// <returns> the cloudlet waiting list </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.ResCloudlet> java.util.List<T> getCloudletWaitingList()
			public virtual IList<ResCloudlet> CloudletWaitingList 
			{
				get
				{
					return (IList<ResCloudlet>) cloudletWaitingList;
				}
				set
				{
					this.cloudletWaitingList = value;
				}
			}


			/// <summary>
			/// Gets the cloudlet exec list.
			/// </summary>
			/// @param <T> the generic type </param>
			/// <returns> the cloudlet exec list </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.ResCloudlet> java.util.List<T> getCloudletExecList()
			public virtual IList<ResCloudlet> CloudletExecList
			{
				get
				{
					return (IList<ResCloudlet>) cloudletExecList;
				}
				set
				{
					this.cloudletExecList = value;
				}
			}


			/// <summary>
			/// Gets the cloudlet paused list.
			/// </summary>
			/// @param <T> the generic type </param>
			/// <returns> the cloudlet paused list </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.ResCloudlet> java.util.List<T> getCloudletPausedList()
			public virtual IList<ResCloudlet> CloudletPausedList
			{
				get
				{
					return (IList<ResCloudlet>) cloudletPausedList;
				}
				set
				{
					this.cloudletPausedList = value;
				}
			}


			/// <summary>
			/// Gets the cloudlet finished list.
			/// </summary>
			/// @param <T> the generic type </param>
			/// <returns> the cloudlet finished list </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.ResCloudlet> java.util.List<T> getCloudletFinishedList()
			public virtual IList<ResCloudlet> CloudletFinishedList
			{
				get
				{
					return (IList<ResCloudlet>) cloudletFinishedList;
				}
				set
				{
					this.cloudletFinishedList = value;
				}
			}


			/// <summary>
			/// Gets the cloudlet failed list.
			/// </summary>
			/// @param <T> the generic type </param>
			/// <returns> the cloudlet failed list. </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public <T extends org.cloudbus.cloudsim.ResCloudlet> java.util.List<T> getCloudletFailedList()
			public virtual IList<ResCloudlet> CloudletFailedList
			{
				get
				{
					return (IList<ResCloudlet>) cloudletFailedList;
				}
				set
				{
					this.cloudletFailedList = value;
				}
			}


	}



}