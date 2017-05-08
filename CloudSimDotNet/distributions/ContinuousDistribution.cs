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
	/// Interface to be implemented by a random number generator.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public interface ContinuousDistribution
	{

		/// <summary>
		/// Generate a new pseudo random number.
		/// </summary>
		/// <returns> the next pseudo random number in the sequence,
		/// following the implemented distribution. </returns>
		double sample();

	}

}