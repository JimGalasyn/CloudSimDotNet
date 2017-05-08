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

	/// <summary>
	/// A pseudo random number generator following the
	/// <a href="http://en.wikipedia.org/wiki/Zipf's_law">Zipf</a> distribution.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class ZipfDistr : ContinuousDistribution
	{

		/// <summary>
		/// The internal random number generator. </summary>
		private readonly Random numGen;

		/// <summary>
		/// The shape. </summary>
		private readonly double shape;

		/// <summary>
		/// The den. </summary>
		private double den;

		/// <summary>
		/// Instantiates a new Zipf pseudo random number generator.
		/// </summary>
		/// <param name="seed"> the seed </param>
		/// <param name="shape"> the shape </param>
		/// <param name="population"> the population </param>
		public ZipfDistr(long seed, double shape, int population)
		{
			if (shape <= 0.0 || population < 1)
			{
				throw new System.ArgumentException("Mean must be greater than 0.0 and population greater than 0");
			}
			numGen = new Random((int)seed);
			this.shape = shape;

			computeDen(shape, population);
		}

		/// <summary>
		/// Instantiates a new Zipf pseudo random number generator.
		/// </summary>
		/// <param name="shape"> the shape </param>
		/// <param name="population"> the population </param>
		public ZipfDistr(double shape, int population)
		{
			if (shape <= 0.0)
			{
				throw new System.ArgumentException("Mean must be greated than 0.0 and population greater than 0");
			}
			numGen = new Random((int)DateTimeHelperClass.CurrentUnixTimeMillis());
			this.shape = shape;
			computeDen(shape, population);
		}

		public virtual double sample()
		{
			double variate = numGen.NextDouble();
			double num = 1;
			double nextNum = 1 + 1 / Math.Pow(2, shape);
			double j = 3;

			while (variate > nextNum / den)
			{
				num = nextNum;
				nextNum += 1 / Math.Pow(j, shape);
				j++;
			}

			return num / den;
		}

		/// <summary>
		/// Compute the den.
		/// </summary>
		/// <param name="shape"> the shape </param>
		/// <param name="population"> the population </param>
		private void computeDen(double shape, int population)
		{
			den = 0.0;
			for (int j = 1; j <= population; j++)
			{
				den += 1 / Math.Pow(j, shape);
			}
		}

	}

}