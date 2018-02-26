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
	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;

	/// <summary>
	/// A class of VM that stores its CPU utilization percentage history. The history is used by VM allocation
	/// and selection policies.
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
	public class PowerVm : Vm
	{

		/// <summary>
		/// The Constant HISTORY_LENGTH. </summary>
		public const int HISTORY_LENGTH = 30;

		/// <summary>
		/// The CPU utilization percentage history. </summary>
		private readonly IList<double?> utilizationHistory = new List<double?>();

		/// <summary>
		/// The previous time that cloudlets were processed. </summary>
		private double previousTime;

		/// <summary>
		/// The scheduling interval to update the processing of cloudlets
		/// running in this VM. 
		/// </summary>
		private double schedulingInterval;

		/// <summary>
		/// Instantiates a new PowerVm.
		/// </summary>
		/// <param name="id"> the id </param>
		/// <param name="userId"> the user id </param>
		/// <param name="mips"> the mips </param>
		/// <param name="pesNumber"> the pes number </param>
		/// <param name="ram"> the ram </param>
		/// <param name="bw"> the bw </param>
		/// <param name="size"> the size </param>
		/// <param name="priority"> the priority </param>
		/// <param name="vmm"> the vmm </param>
		/// <param name="cloudletScheduler"> the cloudlet scheduler </param>
		/// <param name="schedulingInterval"> the scheduling interval </param>
		public PowerVm(int id, int userId, double mips, int pesNumber, int ram, long bw, long size, int priority, string vmm, CloudletScheduler cloudletScheduler, double schedulingInterval) : base(id, userId, mips, pesNumber, ram, bw, size, vmm, cloudletScheduler)
		{
			SchedulingInterval = schedulingInterval;
		}

		public override double updateVmProcessing(double currentTime, IList<double?> mipsShare)
		{
			double time = base.updateVmProcessing(currentTime, mipsShare);
			if (currentTime > PreviousTime && (currentTime - 0.1) % SchedulingInterval == 0)
			{
				double utilization = getTotalUtilizationOfCpu(CloudletScheduler.PreviousTime);
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
		/// <returns> the utilization MAD in MIPS </returns>
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
		/// Adds a CPU utilization percentage history value.
		/// </summary>
		/// <param name="utilization"> the CPU utilization percentage to add </param>
		public virtual void addUtilizationHistoryValue(double utilization)
		{
			UtilizationHistory.Insert(0, utilization);
			if (UtilizationHistory.Count > HISTORY_LENGTH)
			{
				UtilizationHistory.RemoveAt(HISTORY_LENGTH);
			}
		}

		/// <summary>
		/// Gets the CPU utilization percentage history.
		/// </summary>
		/// <returns> the CPU utilization percentage history </returns>
		protected internal virtual IList<double?> UtilizationHistory
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


	}

}