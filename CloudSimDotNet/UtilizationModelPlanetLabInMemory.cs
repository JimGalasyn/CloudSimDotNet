using System;

namespace org.cloudbus.cloudsim
{


	/// <summary>
	/// Defines the resource utilization model based on 
	/// a <a href="https://www.planet-lab.org">PlanetLab</a>
	/// datacenter trace file.
	/// </summary>
	public class UtilizationModelPlanetLabInMemory : UtilizationModel
	{

		/// <summary>
		/// The scheduling interval. </summary>
		private double schedulingInterval;

		/// <summary>
		/// The data (5 min * 288 = 24 hours). </summary>
		private readonly double[] data;

		/// <summary>
		/// Instantiates a new PlanetLab resource utilization model from a trace file.
		/// </summary>
		/// <param name="inputPath"> The path of a PlanetLab datacenter trace. </param>
		/// <param name="schedulingInterval"> </param>
		/// <exception cref="NumberFormatException"> the number format exception </exception>
		/// <exception cref="IOException"> Signals that an I/O exception has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UtilizationModelPlanetLabInMemory(String inputPath, double schedulingInterval) throws NumberFormatException, java.io.IOException
		public UtilizationModelPlanetLabInMemory(string inputPath, double schedulingInterval)
		{
			data = new double[289];
			SchedulingInterval = schedulingInterval;
            // TODO: Proper stream IO 
            System.IO.StreamReader input = null; // new System.IO.StreamReader(inputPath);
			int n = data.Length;
			for (int i = 0; i < n - 1; i++)
			{
				data[i] = Convert.ToInt32(input.ReadLine()) / 100.0;
			}
			data[n - 1] = data[n - 2];
			//input.Close();
		}

		/// <summary>
		/// Instantiates a new PlanetLab resource utilization model with variable data samples
		/// from a trace file.
		/// </summary>
		/// <param name="inputPath"> The path of a PlanetLab datacenter trace. </param>
		/// <param name="dataSamples"> number of samples in the file </param>
		/// <exception cref="NumberFormatException"> the number format exception </exception>
		/// <exception cref="IOException"> Signals that an I/O exception has occurred. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UtilizationModelPlanetLabInMemory(String inputPath, double schedulingInterval, int dataSamples) throws NumberFormatException, java.io.IOException
		public UtilizationModelPlanetLabInMemory(string inputPath, double schedulingInterval, int dataSamples)
		{
			SchedulingInterval = schedulingInterval;
			data = new double[dataSamples];
            // TODO: Proper stream IO 
            System.IO.StreamReader input = null; // new System.IO.StreamReader(inputPath);
			int n = data.Length;
			for (int i = 0; i < n - 1; i++)
			{
				data[i] = Convert.ToInt32(input.ReadLine()) / 100.0;
			}
			data[n - 1] = data[n - 2];
			//input.Close();
		}

		public virtual double getUtilization(double time)
		{
			if (time % SchedulingInterval == 0)
			{
				return data[(int) time / (int) SchedulingInterval];
			}
			int time1 = (int) Math.Floor(time / SchedulingInterval);
			int time2 = (int) Math.Ceiling(time / SchedulingInterval);
			double utilization1 = data[time1];
			double utilization2 = data[time2];
			double delta = (utilization2 - utilization1) / ((time2 - time1) * SchedulingInterval);
			double utilization = utilization1 + delta * (time - time1 * SchedulingInterval);
			return utilization;

		}

		/// <summary>
		/// Sets the scheduling interval.
		/// </summary>
		/// <param name="schedulingInterval"> the new scheduling interval </param>
		public virtual double SchedulingInterval
		{
			set
			{
				this.schedulingInterval = value;
			}
			get
			{
				return schedulingInterval;
			}
		}


		public virtual double[] Data
		{
			get
			{
				return data;
			}
		}
	}

}