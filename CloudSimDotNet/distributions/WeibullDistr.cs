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
    // TEST: (fixed) Figure out WeibullDistribution 
    //using WeibullDistribution = org.apache.commons.math3.distribution.WeibullDistribution;
    using MathNet.Numerics.Random;
    using MathNet.Numerics.Distributions;

    /// <summary>
    /// A pseudo random number generator following the 
    /// <a href="https://en.wikipedia.org/wiki/Weibull_distribution">Weibull distribution</a>.
    /// 
    /// @author Marcos Dias de Assuncao
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class WeibullDistr : ContinuousDistribution
	{
        /// <summary>
        /// The internal Weibull pseudo random number generator. </summary>
        //private readonly WeibullDistribution numGen;
        private readonly Weibull numGen;

        /// <summary>
        /// Instantiates a new Weibull pseudo random number generator.
        /// </summary>
        /// <param name="seed"> the seed </param>
        /// <param name="alpha"> the alpha </param>
        /// <param name="beta"> the beta </param>
        public WeibullDistr(Random seed, double alpha, double beta) : this(alpha, beta)
		{
            // TODO: assign seed param for WeibullDistr.
            //numGen.reseedRandomGenerator(seed.nextLong());
        }

        /// <summary>
        /// Instantiates a new Weibull pseudo random number generator.
        /// </summary>
        /// <param name="alpha"> the alpha </param>
        /// <param name="beta"> the beta </param>
        public WeibullDistr(double alpha, double beta)
		{
            //numGen = new WeibullDistribution(alpha, beta);
            // TEST: alpha, beta == scale, shape?
            numGen = new Weibull(alpha, beta);

        }

		public virtual double sample()
		{
            return numGen.RandomSource.NextDouble();
		}
	}
}