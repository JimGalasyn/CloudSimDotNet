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
	/// A VM allocation policy that uses Inter Quartile Range (IQR)  to compute
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
	public class PowerVmAllocationPolicyMigrationInterQuartileRange : PowerVmAllocationPolicyMigrationAbstract
	{

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
		private double safetyParameter = 0;

		/// <summary>
		/// The fallback VM allocation policy to be used when
		/// the IQR over utilization host detection doesn't have
		/// data to be computed. 
		/// </summary>
		private PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy;

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationInterQuartileRange.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="safetyParameter"> the safety parameter </param>
		/// <param name="utilizationThreshold"> the utilization threshold </param>
		public PowerVmAllocationPolicyMigrationInterQuartileRange(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy, double safetyParameter, PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy, double utilizationThreshold) : base(hostList, vmSelectionPolicy)
		{
			SafetyParameter = safetyParameter;
			FallbackVmAllocationPolicy = fallbackVmAllocationPolicy;
		}

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationInterQuartileRange.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="safetyParameter"> the safety parameter </param>
		public PowerVmAllocationPolicyMigrationInterQuartileRange(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy, double safetyParameter, PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy) : base(hostList, vmSelectionPolicy)
		{
			SafetyParameter = safetyParameter;
			FallbackVmAllocationPolicy = fallbackVmAllocationPolicy;
		}

		/// <summary>
		/// Checks if the host is over utilized, based on CPU utilization.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if the host is over utilized; false otherwise </returns>
		protected internal override bool isHostOverUtilized(PowerHost host)
		{
			PowerHostUtilizationHistory _host = (PowerHostUtilizationHistory) host;
			double upperThreshold = 0;
			try
			{
				upperThreshold = 1 - SafetyParameter * getHostUtilizationIqr(_host);
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
		/// Gets the host CPU utilization percentage IQR.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the host CPU utilization percentage IQR </returns>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected double getHostUtilizationIqr(PowerHostUtilizationHistory host) throws IllegalArgumentException
		protected internal virtual double getHostUtilizationIqr(PowerHostUtilizationHistory host)
		{
			double[] data = host.UtilizationHistory;
			if (MathUtil.countNonZeroBeginning(data) >= 12)
			{ // 12 has been suggested as a safe value
				return MathUtil.iqr(data);
			}
			throw new System.ArgumentException();
		}

		/// <summary>
		/// Sets the safety parameter.
		/// </summary>
		/// <param name="safetyParameter"> the new safety parameter </param>
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