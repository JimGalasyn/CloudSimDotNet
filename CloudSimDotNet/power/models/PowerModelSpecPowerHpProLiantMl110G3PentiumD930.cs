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
	/// The power model of an HP ProLiant ML110 G3 (1 x [Pentium D930 3000 MHz, 2 cores], 4GB).<br/>
	/// <a href="http://www.spec.org/power_ssj2008/results/res2011q1/power_ssj2008-20110127-00342.html">
	/// http://www.spec.org/power_ssj2008/results/res2011q1/power_ssj2008-20110127-00342.html</a>
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
	public class PowerModelSpecPowerHpProLiantMl110G3PentiumD930 : PowerModelSpecPower
	{
		/// <summary>
		/// The power consumption according to the utilization percentage. </summary>
		/// <seealso cref= #getPowerData(int)  </seealso>
		private readonly double[] power = new double[] {105, 112, 118, 125, 131, 137, 147, 153, 157, 164, 169};

		protected internal override double getPowerData(int index)
		{
			return power[index];
		}

	}

}