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
	/// SanStorage represents a Storage Area Network (SAN) composed of a set of harddisks connected in a LAN.
	/// Capacity of individual disks are abstracted, thus only the overall capacity of the SAN is
	/// considered. <tt>WARNING</tt>: This class is not yet fully functional. Effects of network contention are
	/// not considered in the simulation. So, time for file transfer is underestimated in the presence of
	/// high network load.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class SanStorage : HarddriveStorage
	{

		/// <summary>
		/// The bandwidth of SAN network. </summary>
		internal double bandwidth;

		/// <summary>
		/// The SAN's network latency. </summary>
		internal double networkLatency;

		/// <summary>
		/// Creates a new SAN with a given capacity, latency, and bandwidth of the network connection.
		/// </summary>
		/// <param name="capacity"> Storage device capacity </param>
		/// <param name="bandwidth"> Network bandwidth </param>
		/// <param name="networkLatency"> Network latency </param>
		/// <exception cref="ParameterException"> when the name and the capacity are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SanStorage(double capacity, double bandwidth, double networkLatency) throws ParameterException
		public SanStorage(double capacity, double bandwidth, double networkLatency) : base(capacity)
		{
			this.bandwidth = bandwidth;
			this.networkLatency = networkLatency;
		}

		/// <summary>
		/// Creates a new SAN with a given capacity, latency, and bandwidth of the network connection
		/// and with a specific name.
		/// </summary>
		/// <param name="name"> the name of the new storage device </param>
		/// <param name="capacity"> Storage device capacity </param>
		/// <param name="bandwidth"> Network bandwidth </param>
		/// <param name="networkLatency"> Network latency </param>
		/// <exception cref="ParameterException"> when the name and the capacity are not valid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public SanStorage(String name, double capacity, double bandwidth, double networkLatency) throws ParameterException
		public SanStorage(string name, double capacity, double bandwidth, double networkLatency) : base(name, capacity)
		{
			this.bandwidth = bandwidth;
			this.networkLatency = networkLatency;
		}

		public override double addReservedFile(File file)
		{
			double time = base.addReservedFile(file);
			time += networkLatency;
			time += file.Size * bandwidth;

			return time;
		}

		public override double MaxTransferRate
		{
			get
			{
    
				double diskRate = base.MaxTransferRate;
    
				// the max transfer rate is the minimum between
				// the network bandwidth and the disk rate
				if (diskRate < bandwidth)
				{
					return diskRate;
				}
				return bandwidth;
			}
		}

		public override double addFile(File file)
		{
			double time = base.addFile(file);

			time += networkLatency;
			time += file.Size * bandwidth;

			return time;
		}

		public override double addFile(IList<File> list)
		{
			double result = 0.0;
			if (list == null || list.Count == 0)
			{
				Log.printConcatLine(Name, ".addFile(): Warning - list is empty.");
				return result;
			}

			IEnumerator<File> it = list.GetEnumerator();
			File file = null;
			while (it.MoveNext())
			{
				file = it.Current;
				result += this.addFile(file); // add each file in the list
			}
			return result;
		}

		public override double deleteFile(string fileName, File file)
		{
			return this.deleteFile(file);
		}

		public override double deleteFile(File file)
		{
			double time = base.deleteFile(file);

			time += networkLatency;
			time += file.Size * bandwidth;

			return time;
		}

	}

}