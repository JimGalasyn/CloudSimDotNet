using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	/// <summary>
	/// Implements a model, according to which a Cloudlet generates
	/// random resource utilization every time frame.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// @todo This class is the only one that stores the utilization history and
	/// make use of the time attribute at the <seealso cref="#getUtilization(double) "/> method.
	/// Check the other classes to implement the same behavior
	/// (that can be placed in the super class)
	/// </summary>
	public class UtilizationModelStochastic : UtilizationModel
	{

		/// <summary>
		/// The random generator. </summary>
		private Random randomGenerator;

		/// <summary>
		/// The utilization history map, where each key is a time and
		/// each value is the utilization percentage in that time. 
		/// </summary>
		private IDictionary<double?, double?> history;

		/// <summary>
		/// Instantiates a new utilization model stochastic.
		/// </summary>
		public UtilizationModelStochastic()
		{
			History = new Dictionary<double?, double?>();
			RandomGenerator = new Random();
		}

		/// <summary>
		/// Instantiates a new utilization model stochastic.
		/// </summary>
		/// <param name="seed"> the seed </param>
		public UtilizationModelStochastic(long seed)
		{
			History = new Dictionary<double?, double?>();
			RandomGenerator = new Random((int)seed);
		}

		public virtual double getUtilization(double time)
		{
			if (History.ContainsKey(time))
			{
				return History[time].Value;
			}

			double utilization = RandomGenerator.NextDouble();
			History[time] = utilization;
			return utilization;
		}

		/// <summary>
		/// Gets the utilization history.
		/// </summary>
		/// <returns> the history </returns>
		protected internal virtual IDictionary<double?, double?> History
		{
			get
			{
				return history;
			}
			set
			{
				this.history = value;
			}
		}


		/// <summary>
		/// Save the utilization history to a file.
		/// </summary>
		/// <param name="filename"> the filename </param>
		/// <exception cref="Exception"> the exception </exception>
		public virtual void saveHistory(string filename)
		{
            // TODO: Actual file implementation.
			//System.IO.FileStream fos = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write);
			//ObjectOutputStream oos = new ObjectOutputStream(fos);
			//oos.writeObject(History);
			//oos.close();
		}

		/// <summary>
		/// Load an utilization history from a file.
		/// </summary>
		/// <param name="filename"> the filename </param>
		/// <exception cref="Exception"> the exception </exception>
		public virtual void loadHistory(string filename)
		{
            // TODO: real IO
			//System.IO.FileStream fis = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
			//ObjectInputStream ois = new ObjectInputStream(fis);
			//History = (IDictionary<double?, double?>) ois.readObject();
			//ois.close();
		}

		/// <summary>
		/// Sets the random generator.
		/// </summary>
		/// <param name="randomGenerator"> the new random generator </param>
		public virtual Random RandomGenerator
		{
			set
			{
				this.randomGenerator = value;
			}
			get
			{
				return randomGenerator;
			}
		}


	}

}