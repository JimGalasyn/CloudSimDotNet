using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.network.datacenter
{



	/// <summary>
	/// NetworkCloudlet class extends Cloudlet to support simulation of complex applications. Each such
	/// a network Cloudlet represents a task of the application. Each task consists of several stages.
	/// 
	/// <br/>Please refer to following publication for more details:<br/>
	/// <ul>
	/// <li><a href="http://dx.doi.org/10.1109/UCC.2011.24">Saurabh Kumar Garg and Rajkumar Buyya, NetworkCloudSim: Modelling Parallel Applications in Cloud
	/// Simulations, Proceedings of the 4th IEEE/ACM International Conference on Utility and Cloud
	/// Computing (UCC 2011, IEEE CS Press, USA), Melbourne, Australia, December 5-7, 2011.</a>
	/// </ul>
	/// 
	/// @author Saurabh Kumar Garg
	/// @since CloudSim Toolkit 1.0
	/// @todo Attributes should be private
	/// @todo The different cloudlet classes should have a class hierarchy, by means
	/// of a super class and/or interface.
	/// </summary>
	public class NetworkCloudlet : Cloudlet, IComparable<object>
	{
			/// <summary>
			/// Time when cloudlet will be submitted. </summary>
		public double submittime;

			/// <summary>
			/// Time when cloudlet finishes execution. </summary>
		public double finishtime;

			/// <summary>
			/// Execution time for cloudlet. </summary>
		public double exetime;

			/// <summary>
			/// Number of cloudlet's stages . </summary>
		public double numStage;

			/// <summary>
			/// Current stage of cloudlet execution. </summary>
		public int currStagenum;

			/// <summary>
			/// Star time of the current stage. 
			/// </summary>
		public double timetostartStage;

			/// <summary>
			/// Time spent in the current stage. 
			/// </summary>
		public double timespentInStage;

			/// <summary>
			/// @todo It doesn't appear to be used. 
			/// </summary>
		public IDictionary<double?, HostPacket> timeCommunicate;

			/// <summary>
			/// All stages which cloudlet execution. </summary>
		public List<TaskStage> stages;

			/// <summary>
			/// Cloudlet's memory.
			/// @todo Required, allocated, used memory?
			/// It doesn't appear to be used.
			/// </summary>
		internal long memory;

			/// <summary>
			/// Cloudlet's start time.
			/// </summary>
		public double starttime;

		public NetworkCloudlet(int cloudletId, long cloudletLength, int pesNumber, long cloudletFileSize, long cloudletOutputSize, long memory, UtilizationModel utilizationModelCpu, UtilizationModel utilizationModelRam, UtilizationModel utilizationModelBw) : base(cloudletId, cloudletLength, pesNumber, cloudletFileSize, cloudletOutputSize, utilizationModelCpu, utilizationModelRam, utilizationModelBw)
		{

			currStagenum = -1;
			this.memory = memory;
			stages = new List<TaskStage>();
		}

		public virtual int CompareTo(object arg0)
		{
			return 0;
		}

		public virtual double Submittime
		{
			get
			{
				return submittime;
			}
		}

	}

}