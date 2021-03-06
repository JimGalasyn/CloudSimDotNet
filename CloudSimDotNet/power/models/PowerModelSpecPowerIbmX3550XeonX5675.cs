﻿/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power.models
{

	/// <summary>
	/// The power model of an IBM server x3550 (2 x [Xeon X5675 3067 MHz, 6 cores], 16GB).<br/>
	/// <a href="http://www.spec.org/power_ssj2008/results/res2011q2/power_ssj2008-20110406-00368.html">
	/// http://www.spec.org/power_ssj2008/results/res2011q2/power_ssj2008-20110406-00368.html</a>
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
	public class PowerModelSpecPowerIbmX3550XeonX5675 : PowerModelSpecPower
	{
		/// <summary>
		/// The power consumption according to the utilization percentage. </summary>
		/// <seealso cref= #getPowerData(int)  </seealso>
		private readonly double[] power = new double[] {58.4, 98, 109, 118, 128, 140, 153, 170, 189, 205, 222};

		protected internal override double getPowerData(int index)
		{
			return power[index];
		}

	}

}