﻿/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{

	/// <summary>
	/// The UtilizationModelNull class is a simple model, according to which a Cloudlet always require
	/// zero capacity for a given resource all the time.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public class UtilizationModelNull : UtilizationModel
	{

		public virtual double getUtilization(double time)
		{
			return 0;
		}

	}

}