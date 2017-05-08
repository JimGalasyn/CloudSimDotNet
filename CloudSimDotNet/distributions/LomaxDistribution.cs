using System;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.distributions
{

	/// <summary>
	/// A pseudo random number generator following the 
	/// <a href="https://en.wikipedia.org/wiki/Lomax_distribution">
	/// Lomax distribution</a>.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class LomaxDistribution : ParetoDistr, ContinuousDistribution
	{

		/// <summary>
		/// The shift. </summary>
		private readonly double shift;

		/// <summary>
		/// Instantiates a new lomax pseudo random number generator.
		/// </summary>
		/// <param name="shape"> the shape </param>
		/// <param name="location"> the location </param>
		/// <param name="shift"> the shift </param>
		public LomaxDistribution(double shape, double location, double shift) : base(shape, location)
		{

			if (shift > location)
			{
				throw new System.ArgumentException("Shift must be smaller or equal than location");
			}

			this.shift = shift;
		}

		/// <summary>
		/// Instantiates a new lomax pseudo random number generator.
		/// </summary>
		/// <param name="seed"> the seed </param>
		/// <param name="shape"> the shape </param>
		/// <param name="location"> the location </param>
		/// <param name="shift"> the shift </param>
		public LomaxDistribution(Random seed, double shape, double location, double shift) : base(seed, shape, location)
		{

			if (shift > location)
			{
				throw new System.ArgumentException("Shift must be smaller or equal than location");
			}

			this.shift = shift;
		}

		public override double sample()
		{
			return base.sample() - shift;
		}

	}

}