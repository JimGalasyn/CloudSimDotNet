using System;

/*
 * Title:        CloudSim Toolkit
 * Descripimport java.util.Random;
mulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.distributions
{
    // TEST: (fixed) Figure out GammaDistribution.
    //using GammaDistribution = org.apache.commons.math3.distribution.GammaDistribution;
    using MathNet.Numerics.Random;
    using MathNet.Numerics.Distributions;

    /// <summary>
    /// A pseudo random number generator following the
    /// <a href="https://en.wikipedia.org/wiki/Gamma_distribution">Gamma</a> distribution.
    /// 
    /// @author Marcos Dias de Assuncao
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class GammaDistr : ContinuousDistribution
	{

        /// <summary>
        /// The internal Gamma pseudo random number generator. </summary>
        //private readonly GammaDistribution numGen;
        private readonly Gamma numGen;

        /// <summary>
        /// Instantiates a new Gamma pseudo random number generator.
        /// </summary>
        /// <param name="seed"> the seed </param>
        /// <param name="shape"> the shape </param>
        /// <param name="scale"> the scale </param>
        public GammaDistr(Random seed, int shape, double scale) : this(shape, scale)
		{
            // TODO: assign seed param for GammaDistr.
            //numGen.reseedRandomGenerator(seed.nextLong());
            //numGen.reseedRandomGenerator(seed.Next());
        }

        /// <summary>
        /// Instantiates a new Gamma pseudo random number generator.
        /// </summary>
        /// <param name="shape"> the shape </param>
        /// <param name="scale"> the scale </param>
        public GammaDistr(int shape, double scale)
		{
            //numGen = new GammaDistribution(shape, scale);
            // TEST: Is scale == rate? 
            numGen = new Gamma(shape, scale);
		}

		public virtual double sample()
		{
            //return numGen.sample();
            return numGen.RandomSource.NextDouble();
            //return 0;
        }
    }
}