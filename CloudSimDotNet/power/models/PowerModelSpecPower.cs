using System;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power.models
{

	/// <summary>
	/// The abstract class of power models created based on data from 
	/// <a href="http://www.spec.org/power_ssj2008/">SPECpower benchmark</a>.
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
	public abstract class PowerModelSpecPower : PowerModel
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public double getPower(double utilization) throws IllegalArgumentException
		public virtual double getPower(double utilization)
		{
			if (utilization < 0 || utilization > 1)
			{
				throw new System.ArgumentException("Utilization value must be between 0 and 1");
			}
			if (utilization % 0.1 == 0)
			{
				return getPowerData((int)(utilization * 10));
			}
			int utilization1 = (int) Math.Floor(utilization * 10);
			int utilization2 = (int) Math.Ceiling(utilization * 10);
			double power1 = getPowerData(utilization1);
			double power2 = getPowerData(utilization2);
			double delta = (power2 - power1) / 10;
			double power = power1 + delta * (utilization - (double) utilization1 / 10) * 100;
			return power;
		}

		/// <summary>
		/// Gets the power consumption for a given utilization percentage.
		/// </summary>
		/// <param name="index"> the utilization percentage in the scale from [0 to 10], 
		/// where 10 means 100% of utilization. </param>
		/// <returns> the power consumption for the given utilization percentage </returns>
		protected internal abstract double getPowerData(int index);

	}

}