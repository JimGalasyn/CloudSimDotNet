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
	/// Implements a power model where the power consumption is the cube of the resource usage.
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
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public class PowerModelCubic : PowerModel
	{
		/// <summary>
		/// The max power that can be consumed. </summary>
		private double maxPower;

		/// <summary>
		/// The constant that represents the power consumption
		/// for each fraction of resource used. 
		/// </summary>
		private double constant;

		/// <summary>
		/// The static power consumption that is not dependent of resource usage. 
		/// It is the amount of energy consumed even when the host is idle.
		/// </summary>
		private double staticPower;

		/// <summary>
		/// Instantiates a new power model cubic.
		/// </summary>
		/// <param name="maxPower"> the max power </param>
		/// <param name="staticPowerPercent"> the static power percent </param>
		public PowerModelCubic(double maxPower, double staticPowerPercent)
		{
			MaxPower = maxPower;
			StaticPower = staticPowerPercent * maxPower;
			Constant = (maxPower - StaticPower) / Math.Pow(100, 3);
		}

		public virtual double getPower(double utilization)
		{
			if (utilization < 0 || utilization > 1)
			{
				throw new System.ArgumentException("Utilization value must be between 0 and 1");
			}
			if (utilization == 0)
			{
				return 0;
			}
			return StaticPower + Constant * Math.Pow(utilization * 100, 3);
		}

		/// <summary>
		/// Gets the max power.
		/// </summary>
		/// <returns> the max power </returns>
		public virtual double MaxPower
		{
			get
			{
				return maxPower;
			}
			set
			{
				this.maxPower = value;
			}
		}


		/// <summary>
		/// Gets the constant.
		/// </summary>
		/// <returns> the constant </returns>
		protected internal virtual double Constant
		{
			get
			{
				return constant;
			}
			set
			{
				this.constant = value;
			}
		}


		/// <summary>
		/// Gets the static power.
		/// </summary>
		/// <returns> the static power </returns>
		protected internal virtual double StaticPower
		{
			get
			{
				return staticPower;
			}
			set
			{
				this.staticPower = value;
			}
		}


	}

}