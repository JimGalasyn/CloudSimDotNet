using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.util
{


	/// <summary>
	/// Measurement of execution times of CloudSim's methods.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 3.0
	/// </summary>
	public class ExecutionTimeMeasurer
	{

		/// <summary>
		/// A map of execution times where each key
		/// represents the name of the method/process being its
		/// execution time computed and each key is the
		/// time the method/process started (in milliseconds). 
		/// Usually, this name is the method/process name, making
		/// easy to identify the execution times into the map.
		/// 
		/// @todo The name of the attribute doesn't match with what it stores.
		/// It in fact stores the method/process start time,
		/// no the time it spent executing.
		/// </summary>
		private static readonly IDictionary<string, long?> executionTimes = new Dictionary<string, long?>();

		/// <summary>
		/// Start measuring the execution time of a method/process.
		/// Usually this method has to be called at the first line of the method
		/// that has to be its execution time measured.
		/// </summary>
		/// <param name="name"> the name of the method/process being measured. </param>
		/// <seealso cref= #executionTimes </seealso>
		public static void start(string name)
		{
			ExecutionTimes[name] = DateTimeHelperClass.CurrentUnixTimeMillis();
		}

		/// <summary>
		/// Finalizes measuring the execution time of a method/process.
		/// </summary>
		/// <param name="name"> the name of the method/process being measured. </param>
		/// <returns> the time the method/process spent in execution (in seconds) </returns>
		/// <seealso cref= #executionTimes </seealso>
		public static double end(string name)
		{
			double time = (DateTimeHelperClass.CurrentUnixTimeMillis() - ExecutionTimes[name].Value) / 1000.0;
			ExecutionTimes.Remove(name);
			return time;
		}

		/// <summary>
		/// Gets map the execution times.
		/// </summary>
		/// <returns> the execution times map </returns>
		/// <seealso cref= #executionTimes </seealso>
		public static IDictionary<string, long?> ExecutionTimes
		{
			get
			{
				return executionTimes;
			}
		}

	}

}