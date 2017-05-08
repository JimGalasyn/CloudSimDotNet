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
	/// A VM allocation policy that uses Local Regression (LR) to predict host utilization (load)
	/// and define if a host is overloaded or not.
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
	public class PowerVmAllocationPolicyMigrationLocalRegression : PowerVmAllocationPolicyMigrationAbstract
	{

		/// <summary>
		/// The scheduling interval that defines the periodicity of VM migrations. </summary>
		private double schedulingInterval;

		/// <summary>
		/// The safety parameter in percentage (at scale from 0 to 1).
		/// It is a tuning parameter used by the allocation policy to 
		/// estimate host utilization (load). The host overload detection is based
		/// on this estimation.
		/// This parameter is used to tune the estimation
		/// to up or down. If the parameter is set as 1.2, for instance, 
		/// the estimated host utilization is increased in 20%, giving
		/// the host a safety margin of 20% to grow its usage in order to try
		/// avoiding SLA violations. As this parameter decreases, more
		/// aggressive will be the consolidation (packing) of VMs inside a host,
		/// what may lead to optimization of resource usage, but rising of SLA 
		/// violations. Thus, the parameter has to be set in order to balance
		/// such factors.
		/// </summary>
		private double safetyParameter;

		/// <summary>
		/// The fallback VM allocation policy to be used when
		/// the Local REgression over utilization host detection doesn't have
		/// data to be computed. 
		/// </summary>
		private PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy;

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationLocalRegression.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="schedulingInterval"> the scheduling interval </param>
		/// <param name="fallbackVmAllocationPolicy"> the fallback vm allocation policy </param>
		/// <param name="utilizationThreshold"> the utilization threshold </param>
		public PowerVmAllocationPolicyMigrationLocalRegression(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy, double safetyParameter, double schedulingInterval, PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy, double utilizationThreshold) : base(hostList, vmSelectionPolicy)
		{
			SafetyParameter = safetyParameter;
			SchedulingInterval = schedulingInterval;
			FallbackVmAllocationPolicy = fallbackVmAllocationPolicy;
		}

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationLocalRegression.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="schedulingInterval"> the scheduling interval </param>
		/// <param name="fallbackVmAllocationPolicy"> the fallback vm allocation policy </param>
		public PowerVmAllocationPolicyMigrationLocalRegression(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy, double safetyParameter, double schedulingInterval, PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy) : base(hostList, vmSelectionPolicy)
		{
			SafetyParameter = safetyParameter;
			SchedulingInterval = schedulingInterval;
			FallbackVmAllocationPolicy = fallbackVmAllocationPolicy;
		}

		/// <summary>
		/// Checks if a host is over utilized.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if is host over utilized; false otherwise </returns>
		protected internal override bool isHostOverUtilized(PowerHost host)
		{
			PowerHostUtilizationHistory _host = (PowerHostUtilizationHistory) host;
			double[] utilizationHistory = _host.UtilizationHistory;
			int length = 10; // we use 10 to make the regression responsive enough to latest values
			if (utilizationHistory.Length < length)
			{
				return FallbackVmAllocationPolicy.isHostOverUtilized(host);
			}
			double[] utilizationHistoryReversed = new double[length];
			for (int i = 0; i < length; i++)
			{
				utilizationHistoryReversed[i] = utilizationHistory[length - i - 1];
			}
			double[] estimates = null;
			try
			{
				estimates = getParameterEstimates(utilizationHistoryReversed);
			}
			catch (System.ArgumentException)
			{
				return FallbackVmAllocationPolicy.isHostOverUtilized(host);
			}
			double migrationIntervals = Math.Ceiling(getMaximumVmMigrationTime(_host) / SchedulingInterval);
			double predictedUtilization = estimates[0] + estimates[1] * (length + migrationIntervals);
			predictedUtilization *= SafetyParameter;

			addHistoryEntry(host, predictedUtilization);

			return predictedUtilization >= 1;
		}

		/// <summary>
		/// Gets utilization estimates.
		/// </summary>
		/// <param name="utilizationHistoryReversed"> the utilization history in reverse order </param>
		/// <returns> the utilization estimates </returns>
		protected internal virtual double[] getParameterEstimates(double[] utilizationHistoryReversed)
		{
			return MathUtil.getLoessParameterEstimates(utilizationHistoryReversed);
		}

		/// <summary>
		/// Gets the maximum vm migration time.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the maximum vm migration time </returns>
		protected internal virtual double getMaximumVmMigrationTime(PowerHost host)
		{
			int maxRam = int.MinValue;
			foreach (Vm vm in host.VmListProperty)
			{
				int ram = vm.Ram;
				if (ram > maxRam)
				{
					maxRam = ram;
				}
			}
			return maxRam / ((double) host.Bw / (2 * 8000));
		}

		/// <summary>
		/// Sets the scheduling interval.
		/// </summary>
		/// <param name="schedulingInterval"> the new scheduling interval </param>
		protected internal virtual double SchedulingInterval
		{
			set
			{
				this.schedulingInterval = value;
			}
			get
			{
				return schedulingInterval;
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


		public virtual double SafetyParameter
		{
			get
			{
				return safetyParameter;
			}
			set
			{
				this.safetyParameter = value;
			}
		}


	}

}