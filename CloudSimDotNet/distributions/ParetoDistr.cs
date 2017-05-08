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
    // TEST: (fixed) Figure out ParetoDistribution implementaiton.
    //using ParetoDistribution = org.apache.commons.math3.distribution.ParetoDistribution;
    using MathNet.Numerics.Random;
    using MathNet.Numerics.Distributions;
    
    /// <summary>
    /// A pseudo random number generator following the
    /// <a href="https://en.wikipedia.org/wiki/Pareto_distribution">Pareto</a> distribution.
    /// 
    /// @author Marcos Dias de Assuncao
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class ParetoDistr : ContinuousDistribution
	{

        /// <summary>
        /// The internal Pareto pseudo random number generator. </summary>
        //private readonly ParetoDistribution numGen;
        private readonly Pareto numGen;

        /// <summary>
        /// Instantiates a new Pareto pseudo random number generator.
        /// </summary>
        /// <param name="seed"> the seed </param>
        /// <param name="shape"> the shape </param>
        /// <param name="location"> the location </param>
        public ParetoDistr(Random seed, double shape, double location) : this(shape, location)
		{
            // TODO: assign seed param for ParetoDistr.
            //numGen.reseedRandomGenerator(seed.nextLong());
        }

        /// <summary>
        /// Instantiates a new Pareto pseudo random number generator.
        /// </summary>
        /// <param name="shape"> the shape </param>
        /// <param name="location"> the location </param>
        public ParetoDistr(double shape, double location)
		{
            //numGen = new ParetoDistribution(location, shape);
            // TEST: location, shape == scale, shape?
            numGen = new Pareto(location, shape);
        }

		public virtual double sample()
		{
            return numGen.RandomSource.NextDouble();
		}
	}
}