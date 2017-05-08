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

	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

	/// <summary>
	/// WorkflowApp is an example of AppCloudlet having three communicating tasks. Task A and B sends the
	/// data (packet) while Task C receives them.
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
	/// </summary>
	public class WorkflowApp : AppCloudlet
	{

		public WorkflowApp(int type, int appID, double deadline, int numbervm, int userId) : base(type, appID, deadline, numbervm, userId)
		{
			exeTime = 100;
			this.numbervm = 3;
		}

			/// <summary>
			/// If the method completely overrides the parent method,
			/// a interface or abstract class should be used.
			/// The method constant values for cloudlet attributes
			/// that have to be defined by the user.
			/// Thus, that class appears to be another example class,
			/// such as the <seealso cref="AppCloudlet"/>. If this is the intention,
			/// it would be inside the examples package.
			/// At the constructor, there are other hard-coded values. </summary>
			/// <param name="vmIdList">  </param>
		public override void createCloudletList(IList<int?> vmIdList)
		{
			long fileSize = NetworkConstants.FILE_SIZE;
			long outputSize = NetworkConstants.OUTPUT_SIZE;
			int memory = 100;
			UtilizationModel utilizationModel = new UtilizationModelFull();
			int i = 0;
			// Task A
			NetworkCloudlet cl = new NetworkCloudlet(NetworkConstants.currentCloudletId, 0, 1, fileSize, outputSize, memory, utilizationModel, utilizationModel, utilizationModel);
			cl.numStage = 2;
			NetworkConstants.currentCloudletId++;
			cl.UserId = userId;
			cl.submittime = CloudSim.clock();
			cl.currStagenum = -1;
			cl.VmId = vmIdList[i].Value;

			// first stage: big computation
			cl.stages.Add(new TaskStage(NetworkConstants.EXECUTION, 0, 1000 * 0.8, 0, memory, vmIdList[0].Value, cl.CloudletId));
			cl.stages.Add(new TaskStage(NetworkConstants.WAIT_SEND, 1000, 0, 1, memory, vmIdList[2].Value, cl.CloudletId + 2));
			clist.Add(cl);
			i++;
			// Task B
			NetworkCloudlet clb = new NetworkCloudlet(NetworkConstants.currentCloudletId, 0, 1, fileSize, outputSize, memory, utilizationModel, utilizationModel, utilizationModel);
			clb.numStage = 2;
			NetworkConstants.currentCloudletId++;
			clb.UserId = userId;
			clb.submittime = CloudSim.clock();
			clb.currStagenum = -1;
			clb.VmId = vmIdList[i].Value;

			// first stage: big computation

			clb.stages.Add(new TaskStage(NetworkConstants.EXECUTION, 0, 1000 * 0.8, 0, memory, vmIdList[1].Value, clb.CloudletId));
			clb.stages.Add(new TaskStage(NetworkConstants.WAIT_SEND, 1000, 0, 1, memory, vmIdList[2].Value, clb.CloudletId + 1));
			clist.Add(clb);
			i++;

			// Task C
			NetworkCloudlet clc = new NetworkCloudlet(NetworkConstants.currentCloudletId, 0, 1, fileSize, outputSize, memory, utilizationModel, utilizationModel, utilizationModel);
			clc.numStage = 2;
			NetworkConstants.currentCloudletId++;
			clc.UserId = userId;
			clc.submittime = CloudSim.clock();
			clc.currStagenum = -1;
			clc.VmId = vmIdList[i].Value;

			// first stage: big computation
			clc.stages.Add(new TaskStage(NetworkConstants.WAIT_RECV, 1000, 0, 0, memory, vmIdList[0].Value, cl.CloudletId));
			clc.stages.Add(new TaskStage(NetworkConstants.WAIT_RECV, 1000, 0, 1, memory, vmIdList[1].Value, cl.CloudletId + 1));
			clc.stages.Add(new TaskStage(NetworkConstants.EXECUTION, 0, 1000 * 0.8, 1, memory, vmIdList[0].Value, clc.CloudletId));

			clist.Add(clc);

		}
	}

}