﻿using System.Collections.Generic;

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
	/// Defines what a workload model should provide. A workload model generates a list of
	/// jobs (<seealso cref="Cloudlet"/>) that can be dispatched to a resource by <seealso cref="Workload"/>.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since 5.0
	/// </summary>
	/// <seealso cref="Workload"</seealso>
	/// <seealso cref="WorkloadFileReader"</seealso>
	public interface WorkloadModel
	{

		/// <summary>
		/// Generates a list of jobs (<seealso cref="Cloudlet"/>) to be executed.
		/// </summary>
		/// <returns> a list with the jobs (<seealso cref="Cloudlet"/>) 
		/// generated by the workload or null in case of failure. </returns>
		List<Cloudlet> generateWorkload();

	}

}