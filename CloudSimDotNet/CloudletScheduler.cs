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

	using NetworkCloudletSpaceSharedScheduler = org.cloudbus.cloudsim.network.datacenter.NetworkCloudletSpaceSharedScheduler;


	/// <summary>
	/// CloudletScheduler is an abstract class that represents the policy of scheduling performed by a
	/// virtual machine to run its <seealso cref="Cloudlet Cloudlets"/>. 
	/// So, classes extending this must execute Cloudlets. Also, the interface for
	/// cloudlet management is also implemented in this class.
	/// Each VM has to have its own instance of a CloudletScheduler.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public abstract class CloudletScheduler
	{

		/// <summary>
		/// The previous time. </summary>
		private double previousTime;

		/// <summary>
		/// The list of current mips share available for the VM using the scheduler. </summary>
		private IList<double?> currentMipsShare;

		/// <summary>
		/// The list of cloudlet waiting to be executed on the VM. </summary>
		protected internal IList<ResCloudlet> cloudletWaitingList;

		/// <summary>
		/// The list of cloudlets being executed on the VM. </summary>
		protected internal IList<ResCloudlet> cloudletExecList;

		/// <summary>
		/// The list of paused cloudlets. </summary>
		protected internal IList<ResCloudlet> cloudletPausedList;

		/// <summary>
		/// The list of finished cloudlets. </summary>
		protected internal IList<ResCloudlet> cloudletFinishedList;

		/// <summary>
		/// The list of failed cloudlets. </summary>
		protected internal IList<ResCloudlet> cloudletFailedList;

		/// <summary>
		/// Creates a new CloudletScheduler object. 
		/// A CloudletScheduler must be created before starting the actual simulation.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public CloudletScheduler()
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
		/// <param name="mipsShare"> list with MIPS share of each Pe available to the scheduler </param>
		/// <returns> the predicted completion time of the earliest finishing cloudlet, 
		/// or 0 if there is no next events
		/// @pre currentTime >= 0
		/// @post $none </returns>
		public abstract double updateVmProcessing(double currentTime, IList<double?> mipsShare);

		/// <summary>
		/// Receives an cloudlet to be executed in the VM managed by this scheduler.
		/// </summary>
		/// <param name="gl"> the submited cloudlet (@todo it's a strange param name) </param>
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
		/// <param name="clId"> ID of the cloudlet being canceled </param>
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
		/// @post $none
		/// 
		/// @todo cloudlet status should be an enum </returns>
		public abstract int getCloudletStatus(int clId);

		/// <summary>
		/// Informs if there is any cloudlet that finished to execute in the VM managed by this scheduler.
		/// </summary>
		/// <returns> $true if there is at least one finished cloudlet; $false otherwise
		/// @pre $none
		/// @post $none
		/// @todo the method name would be isThereFinishedCloudlets to be clearer </returns>
		public abstract bool FinishedCloudlets {get;}

		/// <summary>
		/// Returns the next cloudlet in the finished list.
		/// </summary>
		/// <returns> a finished cloudlet or $null if the respective list is empty
		/// @pre $none
		/// @post $none </returns>
		public abstract Cloudlet NextFinishedCloudlet {get;}

		/// <summary>
		/// Returns the number of cloudlets running in the virtual machine.
		/// </summary>
		/// <returns> number of cloudlets running
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
		/// Gets total CPU utilization percentage of all cloudlets, according to CPU UtilizationModel of 
		/// each one.
		/// </summary>
		/// <param name="time"> the time to get the current CPU utilization </param>
		/// <returns> total utilization </returns>
		public abstract double getTotalUtilizationOfCpu(double time);

		/// <summary>
		/// Gets the current requested mips.
		/// </summary>
		/// <returns> the current mips </returns>
		public abstract IList<double?> CurrentRequestedMips {get;}

		/// <summary>
		/// Gets the total current available mips for the Cloudlet.
		/// </summary>
		/// <param name="rcl"> the rcl </param>
		/// <param name="mipsShare"> the mips share </param>
		/// <returns> the total current mips
		/// @todo In fact, this method is returning different data depending 
		/// of the subclass. It is expected that the way the method use to compute
		/// the resulting value can be different in every subclass,
		/// but is not supposed that each subclass returns a complete different 
		/// result for the same method of the superclass.
		/// In some class such as <seealso cref="NetworkCloudletSpaceSharedScheduler"/>,
		/// the method returns the average MIPS for the available PEs,
		/// in other classes such as <seealso cref="CloudletSchedulerDynamicWorkload"/> it returns
		/// the MIPS' sum of all PEs. </returns>
		public abstract double getTotalCurrentAvailableMipsForCloudlet(ResCloudlet rcl, IList<double?> mipsShare);

		/// <summary>
		/// Gets the total current requested mips for a given cloudlet.
		/// </summary>
		/// <param name="rcl"> the rcl </param>
		/// <param name="time"> the time </param>
		/// <returns> the total current requested mips for the given cloudlet </returns>
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
		public virtual IList<double?> CurrentMipsShare
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