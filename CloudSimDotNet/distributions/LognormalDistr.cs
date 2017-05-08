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
    // TEST: (fixed) Find LogNormalDistribution implementation.
    //using LogNormalDistribution = org.apache.commons.math3.distribution.LogNormalDistribution;
    using MathNet.Numerics.Random;
    using MathNet.Numerics.Distributions;

    /// <summary>
    /// A pseudo random number generator following the
    /// <a href="https://en.wikipedia.org/wiki/Log-normal_distribution">Lognormal</a> distribution.
    /// 
    /// @author Marcos Dias de Assuncao
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class LognormalDistr : ContinuousDistribution
	{
        /// <summary>
        /// The internal Log-normal pseudo random number generator. </summary>
        //private readonly LogNormalDistribution numGen;
        private readonly LogNormal numGen;
        
        /// <summary>
        /// Instantiates a new Log-normal pseudo random number generator.
        /// </summary>
        /// <param name="seed"> the seed </param>
        /// <param name="shape"> the shape </param>
        /// <param name="scale"> the scale </param>
        public LognormalDistr(Random seed, double shape, double scale) : this(shape, scale)
		{
            // TODO: assign seed param for LognormalDistr.
            //numGen.reseedRandomGenerator(seed.nextLong());
        }

        /// <summary>
        /// Instantiates a new Log-normal pseudo random number generator.
        /// </summary>
        /// <param name="shape"> the shape </param>
        /// <param name="scale"> the scale </param>
        public LognormalDistr(double shape, double scale)
		{
            //numGen = new LogNormalDistribution(scale, shape);
            // TEST: scale, shape == mu, sigma?
            numGen = new LogNormal(scale, shape);
        }

		public virtual double sample()
		{
            return numGen.RandomSource.NextDouble();
		}
	}
}