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

	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;

	/// <summary>
	/// A VM allocation policy that uses Median Absolute Deviation (MAD) to compute
	/// a dynamic threshold in order to detect host over utilization.
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
	/// @since CloudSim Toolkit 3.0
	/// </summary>
	public class PowerVmAllocationPolicyMigrationMedianAbsoluteDeviation : PowerVmAllocationPolicyMigrationAbstract
	{

		/// <summary>
		/// The safety parameter in percentage (at scale from 0 to 1).
		/// It is a tuning parameter used by the allocation policy to define
		/// when a host is overloaded. The overload detection is based
		/// on a dynamic defined host utilization threshold. This threshold 
		/// is computed based on the host's usage history Median absolute deviation 
		/// (MAD, that is similar to the Standard Deviation).
		/// This safety parameter is used to increase or decrease the MAD value
		/// when computing the utilization threshold.
		/// As the safety parameter increases, the threshold decreases, 
		/// what may lead to less SLA violations. So, as higher is that parameter, 
		/// safer the algorithm will be when defining a host as overloaded. 
		/// For instance, considering a safety parameter of 1.5 (150%),
		/// a host's resource usage mean is 0.5 (50%) 
		/// and a MAD of 0.2 (thus, the usage may vary from 0.3 to 0.7). 
		/// To compute the usage threshold, the MAD is increased by 50%, being equals to 0.3. 
		/// Finally, the threshold will be 1 - 0.3 = 0.7. 
		/// Thus, only when the host utilization threshold exceeds 70%, 
		/// the host is considered overloaded. 
		/// Here, more safe or less safe doesn't means a more accurate or less accurate
		/// overload detection. Safer means the algorithm will use a lower host
		/// utilization threshold that may lead to lower SLA violations but higher
		/// resource wastage. Thus this parameter has to be tuned in order to 
		/// trade-off between SLA violation and resource wastage.
		/// </summary>
		private double safetyParameter = 0;

		/// <summary>
		/// The fallback VM allocation policy to be used when
		/// the MAD over utilization host detection doesn't have
		/// data to be computed. 
		/// </summary>
		private PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy;

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationMedianAbsoluteDeviation.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="safetyParameter"> the safety parameter </param>
		/// <param name="utilizationThreshold"> the utilization threshold </param>
		public PowerVmAllocationPolicyMigrationMedianAbsoluteDeviation(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy, double safetyParameter, PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy, double utilizationThreshold) : base(hostList, vmSelectionPolicy)
		{
			SafetyParameter = safetyParameter;
			FallbackVmAllocationPolicy = fallbackVmAllocationPolicy;
		}

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationMedianAbsoluteDeviation.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="safetyParameter"> the safety parameter </param>
		public PowerVmAllocationPolicyMigrationMedianAbsoluteDeviation(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy, double safetyParameter, PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy) : base(hostList, vmSelectionPolicy)
		{
			SafetyParameter = safetyParameter;
			FallbackVmAllocationPolicy = fallbackVmAllocationPolicy;
		}

		/// <summary>
		/// Checks if a host is over utilized.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if the host is over utilized; false otherwise </returns>
		protected internal override bool isHostOverUtilized(PowerHost host)
		{
			PowerHostUtilizationHistory _host = (PowerHostUtilizationHistory) host;
			double upperThreshold = 0;
			try
			{
					upperThreshold = 1 - SafetyParameter * getHostUtilizationMad(_host);
			}
			catch (System.ArgumentException)
			{
				return FallbackVmAllocationPolicy.isHostOverUtilized(host);
			}
			addHistoryEntry(host, upperThreshold);
			double totalRequestedMips = 0;
			foreach (Vm vm in host.VmListProperty)
			{
				totalRequestedMips += vm.CurrentRequestedTotalMips;
			}
			double utilization = totalRequestedMips / host.TotalMips;
			return utilization > upperThreshold;
		}

		/// <summary>
		/// Gets the host utilization MAD.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the host utilization MAD </returns>
		protected internal virtual double getHostUtilizationMad(PowerHostUtilizationHistory host)
		{
			double[] data = host.UtilizationHistory;
			if (MathUtil.countNonZeroBeginning(data) >= 12)
			{ // 12 has been suggested as a safe value
				return MathUtil.mad(data);
			}
			throw new System.ArgumentException();
		}

		/// <summary>
		/// Sets the safety parameter.
		/// </summary>
		/// <param name="safetyParameter"> the new safety parameter
		/// @todo It should raise an InvalidArgumentException instead of calling System.exit(0) </param>
		protected internal virtual double SafetyParameter
		{
			set
			{
				if (value < 0)
				{
					Log.printConcatLine("The safety parameter cannot be less than zero. The passed value is: ", value);
                    //Environment.Exit(0);
                    throw new ArgumentException("cannot be less than zero.", "SafetyParameter");
				}
				this.safetyParameter = value;
			}
			get
			{
				return safetyParameter;
			}
		}


		/// <summary>
		/// Sets the fallback vm allocation policy.
		/// </summary>
		/// <param name="fallbackVmAllocationPolicy"> the new fallback vm allocation policy </param>
		public virtual PowerVmAllocationPolicyMigrationAbstract FallbackVmAllocationPolicy
		{
			set
			{
				this.fallbackVmAllocationPolicy = value;
			}
			get
			{
				return fallbackVmAllocationPolicy;
			}
		}


	}

}