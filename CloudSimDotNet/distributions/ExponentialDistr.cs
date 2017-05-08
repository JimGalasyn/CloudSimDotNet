/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.distributions
{
    // TEST: (fixed) Figure out ExponentialDistribution. 
    //using ExponentialDistribution = org.apache.commons.math3.distribution.ExponentialDistribution;
    using MathNet.Numerics.Random;
    using MathNet.Numerics.Distributions;

    /// <summary>
    /// A pseudo random number generator following the 
    /// <a href="https://en.wikipedia.org/wiki/Exponential_distribution">Exponential distribution</a>.
    /// 
    /// @author Marcos Dias de Assuncao
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class ExponentialDistr : ContinuousDistribution
	{

        /// <summary>
        /// The internal exponential number generator. </summary>
        //private readonly ExponentialDistribution numGen;
        private readonly Exponential numGen;

        /// <summary>
        /// Creates a new exponential pseudo random number generator.
        /// </summary>
        /// <param name="seed"> the seed to be used. </param>
        /// <param name="mean"> the mean for the distribution. </param>
        public ExponentialDistr(long seed, double mean) : this(mean)
		{
            // TEST: assign seed param for ExponentialDistr.
            //numGen.reseedRandomGenerator(seed);
        }

        /// <summary>
        /// Creates a new exponential pseudo random number generator.
        /// </summary>
        /// <param name="mean"> the mean for the distribution. </param>
        public ExponentialDistr(double mean)
		{
            //numGen = new ExponentialDistribution(mean);
            // TEST: Is mean == rate?
            numGen = new Exponential(mean);

        }

		public virtual double sample()
		{
            return numGen.RandomSource.NextDouble();
		}
	}
}