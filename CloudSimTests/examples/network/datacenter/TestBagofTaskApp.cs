using System.Collections.Generic;

namespace org.cloudbus.cloudsim.examples.network.datacenter
{

    /// <summary>
    /// BagofTaskApp is an example of AppCloudlet having three noncommunicating tasks. 
    /// 
    /// 
    /// Please refer to following publication for more details:
    /// 
    /// Saurabh Kumar Garg and Rajkumar Buyya, NetworkCloudSim: Modelling Parallel
    /// Applications in Cloud Simulations, Proceedings of the 4th IEEE/ACM
    /// International Conference on Utility and Cloud Computing (UCC 2011, IEEE CS
    /// Press, USA), Melbourne, Australia, December 5-7, 2011.
    /// 
    /// 
    /// @author Saurabh Kumar Garg
    /// @since CloudSim Toolkit 1.0
    /// </summary>

    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using AppCloudlet = org.cloudbus.cloudsim.network.datacenter.AppCloudlet;
    using NetworkCloudlet = org.cloudbus.cloudsim.network.datacenter.NetworkCloudlet;
    using NetworkConstants = org.cloudbus.cloudsim.network.datacenter.NetworkConstants;
    using TaskStage = org.cloudbus.cloudsim.network.datacenter.TaskStage;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;


    public class TestBagofTaskApp : AppCloudlet
    {

        public TestBagofTaskApp(int type, int appID, double deadline, int numbervm, int userId) : base(type, appID, deadline, numbervm, userId)
        {

            this.numbervm = this.getnumvm();
            this.exeTime = ExecTime / this.numbervm;
        }

        public override void createCloudletList(IList<int?> vmIdList)
        {
            //basically, each task runs the simulation and then data is consolidated in one task
            int executionTime = ExecTime;
            long memory = 1000;
            long fileSize = NetworkConstants.FILE_SIZE;
            long outputSize = NetworkConstants.OUTPUT_SIZE;
            int pesNumber = NetworkConstants.PES_NUMBER;
            int stgId = 0;
            int t = NetworkConstants.currentCloudletId;
            for (int i = 0; i < numbervm; i++)
            {
                UtilizationModel utilizationModel = new UtilizationModelFull();
                NetworkCloudlet cl = new NetworkCloudlet(NetworkConstants.currentCloudletId, executionTime / numbervm, pesNumber, fileSize, outputSize, memory, utilizationModel, utilizationModel, utilizationModel);
                NetworkConstants.currentCloudletId++;
                cl.UserId = userId;
                cl.submittime = CloudSim.clock();
                cl.currStagenum = -1;
                cl.VmId = vmIdList[i].Value;
                //compute and send data to node 0
                cl.stages.Add(new TaskStage(NetworkConstants.EXECUTION, NetworkConstants.COMMUNICATION_LENGTH, executionTime / numbervm, stgId++, memory, vmIdList[0].Value, cl.CloudletId));

                //0 has an extra stage of waiting for results; others send
                if (i == 0)
                {
                    for (int j = 1; j < numbervm; j++)
                    {
                        cl.stages.Add(new TaskStage(NetworkConstants.WAIT_RECV, NetworkConstants.COMMUNICATION_LENGTH, 0, stgId++, memory, vmIdList[j].Value, cl.CloudletId + j));
                    }
                }
                else
                {
                    cl.stages.Add(new TaskStage(NetworkConstants.WAIT_SEND, NetworkConstants.COMMUNICATION_LENGTH, 0, stgId++, memory, vmIdList[0].Value, t));
                }

                clist.Add(cl);
            }
        }

        /// <summary>
        /// One can generate number of VMs for each application based on deadline
        /// @return
        /// </summary>
        public virtual int getnumvm()
        {
            double exetime = ExecTime / 2; //for two vms
            if (this.deadline > exetime)
            {
                return 2;
            }
            else if (this.deadline > (exetime / 4))
            {
                return 4;
            }

            return 4;
        }

        private int ExecTime
        {
            get
            {
                //use exec constraints 

                return 100;
            }
        }
    }
}