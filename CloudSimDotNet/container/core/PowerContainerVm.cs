using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

	using ContainerBwProvisioner = org.cloudbus.cloudsim.container.containerProvisioners.ContainerBwProvisioner;
	using ContainerPe = org.cloudbus.cloudsim.container.containerProvisioners.ContainerPe;
	using ContainerRamProvisioner = org.cloudbus.cloudsim.container.containerProvisioners.ContainerRamProvisioner;
	using ContainerScheduler = org.cloudbus.cloudsim.container.schedulers.ContainerScheduler;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;


	/// <summary>
	/// Created by sareh on 14/07/15.
	/// </summary>
	public class PowerContainerVm : ContainerVm
	{

		/// <summary>
		/// The Constant HISTORY_LENGTH.
		/// </summary>
		public const int HISTORY_LENGTH = 30;

		/// <summary>
		/// The utilization history.
		/// </summary>
		private readonly IList<double?> utilizationHistory = new List<double?>();

		/// <summary>
		/// The previous time.
		/// </summary>
		private double previousTime;

		/// <summary>
		/// The scheduling interval.
		/// </summary>
		private double schedulingInterval;

		/// <summary>
		/// Instantiates a new power vm.
		/// </summary>
		/// <param name="id">                 the id </param>
		/// <param name="userId">             the user id </param>
		/// <param name="mips">               the mips </param>
		/// <param name="ram">                the ram </param>
		/// <param name="bw">                 the bw </param>
		/// <param name="size">               the size </param>
		/// <param name="vmm">                the vmm </param>
		/// <param name="containerScheduler"> the cloudlet scheduler </param>
		/// <param name="schedulingInterval"> the scheduling interval </param>
		public PowerContainerVm(int id, int userId, double mips, float ram, long bw, long size, string vmm, ContainerScheduler containerScheduler, ContainerRamProvisioner containerRamProvisioner, ContainerBwProvisioner containerBwProvisioner, IList<ContainerPe> peList, double schedulingInterval) : base(id, userId, mips, ram, bw, size, vmm, containerScheduler, containerRamProvisioner, containerBwProvisioner, peList)
		{
			SchedulingInterval = schedulingInterval;
		}

		/// <summary>
		/// Updates the processing of cloudlets running on this VM.
		/// </summary>
		/// <param name="currentTime"> current simulation time </param>
		/// <param name="mipsShare">   array with MIPS share of each Pe available to the scheduler </param>
		/// <returns> time predicted completion time of the earliest finishing cloudlet, or 0 if there is
		/// no next events
		/// @pre currentTime >= 0
		/// @post $none </returns>
		public override double updateVmProcessing(double currentTime, IList<double?> mipsShare)
		{
			double time = base.updateVmProcessing(currentTime, mipsShare);
			if (currentTime > PreviousTime && (currentTime - 0.2) % SchedulingInterval == 0)
			{
				double utilization = 0;

				foreach (Container container in ContainerListProperty)
				{
					// The containers which are going to migrate to the vm shouldn't be added to the utilization
					if (!ContainersMigratingIn.Contains(container))
					{
						time = container.ContainerCloudletScheduler.PreviousTime;
						utilization += container.getTotalUtilizationOfCpu(time);
					}
				}

				if (CloudSim.clock() != 0 || utilization != 0)
				{
					addUtilizationHistoryValue(utilization);
				}
				PreviousTime = currentTime;
			}
			return time;
		}

		/// <summary>
		/// Gets the utilization MAD in MIPS.
		/// </summary>
		/// <returns> the utilization mean in MIPS </returns>
		public virtual double UtilizationMad
		{
			get
			{
				double mad = 0;
				if (UtilizationHistory.Count > 0)
				{
					int n = HISTORY_LENGTH;
					if (HISTORY_LENGTH > UtilizationHistory.Count)
					{
						n = UtilizationHistory.Count;
					}
					double median = MathUtil.median(UtilizationHistory);
					double[] deviationSum = new double[n];
					for (int i = 0; i < n; i++)
					{
						deviationSum[i] = Math.Abs(median - UtilizationHistory[i].Value);
					}
					mad = MathUtil.median(deviationSum);
				}
				return mad;
			}
		}

		/// <summary>
		/// Gets the utilization mean in percents.
		/// </summary>
		/// <returns> the utilization mean in MIPS </returns>
		public virtual double UtilizationMean
		{
			get
			{
				double mean = 0;
				if (UtilizationHistory.Count > 0)
				{
					int n = HISTORY_LENGTH;
					if (HISTORY_LENGTH > UtilizationHistory.Count)
					{
						n = UtilizationHistory.Count;
					}
					for (int i = 0; i < n; i++)
					{
						mean += UtilizationHistory[i].Value;
					}
					mean /= n;
				}
				return mean * Mips;
			}
		}

		/// <summary>
		/// Gets the utilization variance in MIPS.
		/// </summary>
		/// <returns> the utilization variance in MIPS </returns>
		public virtual double UtilizationVariance
		{
			get
			{
				double mean = UtilizationMean;
				double variance = 0;
				if (UtilizationHistory.Count > 0)
				{
					int n = HISTORY_LENGTH;
					if (HISTORY_LENGTH > UtilizationHistory.Count)
					{
						n = UtilizationHistory.Count;
					}
					for (int i = 0; i < n; i++)
					{
						double tmp = UtilizationHistory[i].Value * Mips - mean;
						variance += tmp * tmp;
					}
					variance /= n;
				}
				return variance;
			}
		}

		/// <summary>
		/// Adds the utilization history value.
		/// </summary>
		/// <param name="utilization"> the utilization </param>
		public virtual void addUtilizationHistoryValue(double utilization)
		{
			UtilizationHistory.Insert(0, utilization);
			if (UtilizationHistory.Count > HISTORY_LENGTH)
			{
				UtilizationHistory.RemoveAt(HISTORY_LENGTH);
			}
		}

		/// <summary>
		/// Gets the utilization history.
		/// </summary>
		/// <returns> the utilization history </returns>
		public virtual IList<double?> UtilizationHistory
		{
			get
			{
				return utilizationHistory;
			}
		}

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
		/// Gets the scheduling interval.
		/// </summary>
		/// <returns> the schedulingInterval </returns>
		public virtual double SchedulingInterval
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


		public virtual double[] UtilizationHistoryList
		{
			get
			{
				double[] utilizationHistoryList = new double[PowerContainerVm.HISTORY_LENGTH];
		//            if any thing happens check if you need to have mips and the trim
				for (int i = 0; i < UtilizationHistory.Count; i++)
				{
					utilizationHistoryList[i] += UtilizationHistory[i].Value * Mips;
				}
    
				return MathUtil.trimZeroTail(utilizationHistoryList);
			}
		}
	}



}