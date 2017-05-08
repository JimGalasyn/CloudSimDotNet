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
    // TEST: (fixed) Figure out UniformRealDistribution
    //using UniformRealDistribution = org.apache.commons.math3.distribution.UniformRealDistribution;
    using MathNet.Numerics.Random;
    using MathNet.Numerics.Distributions;

    /// <summary>
    /// A pseudo random number generator following the 
    /// <a href="https://en.wikipedia.org/wiki/Uniform_distribution_(continuous)">
    /// Uniform continuous distribution</a>.
    /// 
    /// @author Marcos Dias de Assuncao
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class UniformDistr : ContinuousDistribution
	{
        /// <summary>
        /// The internal uniform pseudo random number generator. </summary>
        //private readonly UniformRealDistribution numGen;
        private readonly ContinuousUniform numGen;

        /// <summary>
        /// Creates new uniform pseudo random number generator.
        /// </summary>
        /// <param name="min"> minimum value </param>
        /// <param name="max"> maximum value </param>
        public UniformDistr(double min, double max)
		{
            //numGen = new UniformRealDistribution(min, max);
            numGen = new ContinuousUniform(min, max);
        }

		/// <summary>
		/// Creates new uniform pseudo random number generator.
		/// </summary>
		/// <param name="min"> minimum value </param>
		/// <param name="max"> maximum value </param>
		/// <param name="seed"> simulation seed to be used </param>
		public UniformDistr(double min, double max, long seed) : this(min, max)
		{
            // TODO: assign seed param for UniformDistr.
            //numGen.reseedRandomGenerator(seed);
        }

        public virtual double sample()
		{
            return numGen.RandomSource.NextDouble();
		}

		/// <summary>
		/// Generates a new pseudo random number based on the generator and values provided as
		/// parameters.
		/// </summary>
		/// <param name="rd"> the random number generator </param>
		/// <param name="min"> the minimum value </param>
		/// <param name="max"> the maximum value </param>
		/// <returns> the next random number in the sequence </returns>
		public static double sample(Random rd, double min, double max)
		{
			if (min >= max)
			{
				throw new System.ArgumentException("Maximum must be greater than the minimum.");
			}

			return (rd.NextDouble() * (max - min)) + min;
		}

		/// <summary>
		/// Sets the random number generator's seed.
		/// </summary>
		/// <param name="seed"> the new seed for the generator </param>
		public virtual long Seed
		{
			set
			{
				//numGen.reseedRandomGenerator(value);
			}
		}
	}
}