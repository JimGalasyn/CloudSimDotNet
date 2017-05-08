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
	/// A VM allocation policy that uses Local Regression Robust (LRR) to predict host utilization (load)
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
	public class PowerVmAllocationPolicyMigrationLocalRegressionRobust : PowerVmAllocationPolicyMigrationLocalRegression
	{

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationLocalRegressionRobust.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="schedulingInterval"> the scheduling interval </param>
		/// <param name="fallbackVmAllocationPolicy"> the fallback vm allocation policy </param>
		/// <param name="utilizationThreshold"> the utilization threshold </param>
		public PowerVmAllocationPolicyMigrationLocalRegressionRobust(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy, double safetyParameter, double schedulingInterval, PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy, double utilizationThreshold) : base(hostList, vmSelectionPolicy, safetyParameter, schedulingInterval, fallbackVmAllocationPolicy, utilizationThreshold)
		{
		}

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationLocalRegressionRobust.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="schedulingInterval"> the scheduling interval </param>
		/// <param name="fallbackVmAllocationPolicy"> the fallback vm allocation policy </param>
		public PowerVmAllocationPolicyMigrationLocalRegressionRobust(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy, double safetyParameter, double schedulingInterval, PowerVmAllocationPolicyMigrationAbstract fallbackVmAllocationPolicy) : base(hostList, vmSelectionPolicy, safetyParameter, schedulingInterval, fallbackVmAllocationPolicy)
		{
		}

		/// <summary>
		/// Gets the utilization estimates.
		/// </summary>
		/// <param name="utilizationHistoryReversed"> the utilization history reversed </param>
		/// <returns> the utilization estimates </returns>
		protected internal override double[] getParameterEstimates(double[] utilizationHistoryReversed)
		{
			return MathUtil.getRobustLoessParameterEstimates(utilizationHistoryReversed);
		}

	}

}