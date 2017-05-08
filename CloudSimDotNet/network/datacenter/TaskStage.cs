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
	/// TaskStage represents various stages a <seealso cref="NetworkCloudlet"/> can have during execution. 
	/// Four stage types which are possible: <seealso cref="NetworkConstants#EXECUTION"/>, 
	/// <seealso cref="NetworkConstants#WAIT_SEND"/>, <seealso cref="NetworkConstants#WAIT_RECV"/>, 
	/// <seealso cref="NetworkConstants#FINISH"/>.
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
	/// @todo Attributes should be defined as private.
	/// </summary>
	public class TaskStage
	{
		internal int vpeer;

			/// <summary>
			/// The task type, either <seealso cref="NetworkConstants#EXECUTION"/>, 
			/// <seealso cref="NetworkConstants#WAIT_SEND"/> or <seealso cref="NetworkConstants#WAIT_RECV"/>.
			/// @todo It would be used enum instead of int constants.
			/// </summary>
		internal int type;

			/// <summary>
			/// The data length generated for the task (in bytes).
			/// </summary>
		internal double data;

			/// <summary>
			/// Execution time for this stage. </summary>
		internal double time;

			/// <summary>
			/// Stage (task) id. </summary>
		internal double stageid;

			/// <summary>
			/// Memory used by the task. </summary>
		internal long memory;

			/// <summary>
			/// From whom data needed to be received or sent. </summary>
		internal int peer;

		public TaskStage(int type, double data, double time, double stageid, long memory, int peer, int vpeer) : base()
		{
			this.type = type;
			this.data = data;
			this.time = time;
			this.stageid = stageid;
			this.memory = memory;
			this.peer = peer;
			this.vpeer = vpeer;
		}
	}

}