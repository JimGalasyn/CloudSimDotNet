/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

using System.Threading.Tasks;

namespace org.cloudbus.cloudsim.network
{

	/// <summary>
	/// An interface to abstract a reader for different graph file formats.
	/// 
	/// @author Thomas Hohnstein
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public interface GraphReaderIF
	{

		/// <summary>
		/// Reads a file and creates an <seealso cref="TopologicalGraph"/> object.
		/// </summary>
		/// <param name="filename"> Name of the file to read </param>
		/// <returns> The created TopologicalGraph </returns>
		/// <exception cref="IOException"> when the file cannot be accessed </exception>
		Task<TopologicalGraph> readGraphFile(string filename);
	}
}