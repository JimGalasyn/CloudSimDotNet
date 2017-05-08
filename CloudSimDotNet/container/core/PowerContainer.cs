using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

	using ContainerCloudletScheduler = org.cloudbus.cloudsim.container.schedulers.ContainerCloudletScheduler;
	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;


	/// <summary>
	/// Created by sareh on 23/07/15.
	/// </summary>
	public class PowerContainer : Container
	{

			/// <summary>
			/// The Constant HISTORY_LENGTH. </summary>
            // Hides inherited field.
			//public const int HISTORY_LENGTH = 30;

			/// <summary>
			/// The utilization history. </summary>
			private readonly IList<double?> utilizationHistory = new List<double?>();


			/// <summary>
			/// The previous time. </summary>
			private double previousTime;


			/// <summary>
			/// Instantiates a new power vm.
			/// </summary>
			/// <param name="id"> the id </param>
			/// <param name="userId"> the user id </param>
			/// <param name="mips"> the mips </param>
			/// <param name="pesNumber"> the pes number </param>
			/// <param name="ram"> the ram </param>
			/// <param name="bw"> the bw </param>
			/// <param name="size"> the size </param>
			/// <param name="vmm"> the vmm </param>
			/// <param name="cloudletScheduler"> the cloudlet scheduler </param>
			/// <param name="schedulingInterval"> the scheduling interval </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public PowerContainer(final int id, final int userId, final double mips, final int pesNumber, final int ram, final long bw, final long size, final String vmm, final org.cloudbus.cloudsim.container.schedulers.ContainerCloudletScheduler cloudletScheduler, final double schedulingInterval)
			public PowerContainer(int id, int userId, double mips, int pesNumber, int ram, long bw, long size, string vmm, ContainerCloudletScheduler cloudletScheduler, double schedulingInterval) : base(id, userId, mips, pesNumber, ram, bw, size, vmm, cloudletScheduler, schedulingInterval)
			{
			}

			/// <summary>
			/// Updates the processing of cloudlets running on this VM.
			/// </summary>
			/// <param name="currentTime"> current simulation time </param>
			/// <param name="mipsShare"> array with MIPS share of each Pe available to the scheduler
			/// </param>
			/// <returns> time predicted completion time of the earliest finishing cloudlet, or 0 if there is
			/// 		no next events
			/// 
			/// @pre currentTime >= 0
			/// @post $none </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public double updateContainerProcessing(final double currentTime, final java.util.List<Nullable<double>> mipsShare)
			public override double updateContainerProcessing(double currentTime, IList<double?> mipsShare)
			{
				double time = base.updateContainerProcessing(currentTime, mipsShare);
				if (currentTime > PreviousTime && (currentTime - 0.2) % SchedulingInterval == 0)
				{
					double utilization = getTotalUtilizationOfCpu(ContainerCloudletScheduler.PreviousTime);
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
			public override double UtilizationMad
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
			public override double UtilizationMean
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
			public override double UtilizationVariance
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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void addUtilizationHistoryValue(final double utilization)
			public override void addUtilizationHistoryValue(double utilization)
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
			protected internal override IList<double?> UtilizationHistory
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
			public override double PreviousTime
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



			public virtual double[] UtilizationHistoryList
			{
				get
				{
					double[] utilizationHistoryList = new double[PowerContainer.HISTORY_LENGTH];
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