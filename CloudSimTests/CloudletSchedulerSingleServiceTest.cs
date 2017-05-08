using System.Collections.Generic;
using System.Linq;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class CloudletSchedulerSingleServiceTest
    {
        private const long CLOUDLET_LENGTH = 1000;
        private const long CLOUDLET_FILE_SIZE = 300;
        private const long CLOUDLET_OUTPUT_SIZE = 300;

        private const double MIPS = 1000;
        private const int PES_NUMBER = 2;

        private CloudletSchedulerDynamicWorkload vmScheduler;

        //public virtual void setUp()
        //{
        //    vmScheduler = new CloudletSchedulerDynamicWorkload(MIPS, PES_NUMBER);
        //}

        [TestInitialize()]
        public void Initialize()
        {
            vmScheduler = new CloudletSchedulerDynamicWorkload(MIPS, PES_NUMBER);
        }

        [TestMethod]
        public virtual void testGetNumberOfPes()
        {
            Assert.AreEqual(PES_NUMBER, vmScheduler.NumberOfPes);
        }

        [TestMethod]
        public virtual void testGetMips()
        {
            Assert.AreEqual(MIPS, vmScheduler.Mips);
        }

        [TestMethod]
        public virtual void testGetUnderAllocatedMips()
        {
            UtilizationModelStochastic utilizationModel = new UtilizationModelStochastic();
            Cloudlet cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModel, utilizationModel, utilizationModel);
            ResCloudlet rcl = new ResCloudlet(cloudlet);

            IDictionary<string, double?> underAllocatedMips = new Dictionary<string, double?>();
            Assert.IsTrue(underAllocatedMips.SequenceEqual(vmScheduler.UnderAllocatedMips));

            underAllocatedMips[rcl.Uid] = MIPS / 2;
            vmScheduler.updateUnderAllocatedMipsForCloudlet(rcl, MIPS / 2);
            Assert.IsTrue(underAllocatedMips.SequenceEqual(vmScheduler.UnderAllocatedMips));

            underAllocatedMips[rcl.Uid] = MIPS;
            vmScheduler.updateUnderAllocatedMipsForCloudlet(rcl, MIPS / 2);
            Assert.IsTrue(underAllocatedMips.SequenceEqual(vmScheduler.UnderAllocatedMips));
        }

        [TestMethod]
        public virtual void testGetCurrentRequestedMips()
        {
            UtilizationModelStochastic utilizationModel = new UtilizationModelStochastic();
            Cloudlet cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModel, utilizationModel, utilizationModel);
            cloudlet.setResourceParameter(0, 0, 0);

            IList<double?> mipsShare = new List<double?>();
            mipsShare.Add(MIPS);
            mipsShare.Add(MIPS);
            vmScheduler.CurrentMipsShare = mipsShare;

            Assert.AreEqual(mipsShare.Count, vmScheduler.CurrentMipsShare.Count, 0);
            Assert.AreEqual(mipsShare[0], vmScheduler.CurrentMipsShare[0]);
            Assert.AreEqual(mipsShare[1], vmScheduler.CurrentMipsShare[1]);

            double utilization = utilizationModel.getUtilization(0);

            vmScheduler.cloudletSubmit(cloudlet);

            IList<double?> requestedMips = new List<double?>();
            requestedMips.Add(MIPS * utilization);
            requestedMips.Add(MIPS * utilization);

            Assert.IsTrue(requestedMips.SequenceEqual(vmScheduler.CurrentRequestedMips));
        }

        [TestMethod]
        public virtual void testGetTotalUtilization()
        {
            UtilizationModelStochastic utilizationModel = new UtilizationModelStochastic();
            Cloudlet cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModel, utilizationModel, utilizationModel);
            cloudlet.setResourceParameter(0, 0, 0);

            IList<double?> mipsShare = new List<double?>();
            mipsShare.Add(MIPS);
            mipsShare.Add(MIPS);
            vmScheduler.CurrentMipsShare = mipsShare;

            Assert.AreEqual(mipsShare.Count, vmScheduler.CurrentMipsShare.Count, 0);
            Assert.AreEqual(mipsShare[0], vmScheduler.CurrentMipsShare[0]);
            Assert.AreEqual(mipsShare[1], vmScheduler.CurrentMipsShare[1]);

            double utilization = utilizationModel.getUtilization(0);

            vmScheduler.cloudletSubmit(cloudlet, 0);

            Assert.AreEqual(utilization, vmScheduler.getTotalUtilizationOfCpu(0));
        }

        [TestMethod]
        public virtual void testCloudletFinish()
        {
            UtilizationModelStochastic utilizationModel = new UtilizationModelStochastic();
            Cloudlet cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModel, utilizationModel, utilizationModel);
            cloudlet.setResourceParameter(0, 0, 0);

            IList<double?> mipsShare = new List<double?>();
            mipsShare.Add(MIPS);
            mipsShare.Add(MIPS);
            vmScheduler.CurrentMipsShare = mipsShare;

            vmScheduler.cloudletSubmit(cloudlet, 0);
            vmScheduler.cloudletFinish(new ResCloudlet(cloudlet));

            Assert.AreEqual(Cloudlet.SUCCESS, vmScheduler.getCloudletStatus(0));
            Assert.IsTrue(vmScheduler.FinishedCloudlets);
            Assert.AreSame(cloudlet, vmScheduler.NextFinishedCloudlet);
        }

        [TestMethod]
        public virtual void testGetTotalCurrentMips()
        {
            IList<double?> mipsShare = new List<double?>();
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);
            vmScheduler.CurrentMipsShare = mipsShare;

            Assert.AreEqual(MIPS / 2, vmScheduler.TotalCurrentMips);
        }

        [TestMethod]
        public virtual void testGetTotalCurrentMipsForCloudlet()
        {
            UtilizationModelStochastic utilizationModel = new UtilizationModelStochastic();
            Cloudlet cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModel, utilizationModel, utilizationModel);
            cloudlet.setResourceParameter(0, 0, 0);
            ResCloudlet rgl = new ResCloudlet(cloudlet);

            IList<double?> mipsShare = new List<double?>();
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);

            Assert.AreEqual(MIPS / 4.0 * PES_NUMBER, vmScheduler.getTotalCurrentAvailableMipsForCloudlet(rgl, mipsShare));
        }

        [TestMethod]
        public virtual void testGetEstimatedFinishTimeLowUtilization()
        {
            UtilizationModel utilizationModel = new UtilizationModelLow();
            testGetEstimatedFinishTime(utilizationModel);

            //UtilizationModel utilizationModel = createMock(typeof(UtilizationModel));
            //expect(utilizationModel.getUtilization(0)).andReturn(0.11).anyTimes();
            //replay(utilizationModel);
            //testGetEstimatedFinishTime(utilizationModel);
            //verify(utilizationModel);
        }

        [TestMethod]
        public virtual void testGetEstimatedFinishTimeHighUtilization()
        {
            UtilizationModel utilizationModel = new UtilizationModelHigh();
            testGetEstimatedFinishTime(utilizationModel);

            //UtilizationModel utilizationModel = createMock(typeof(UtilizationModel));
            //expect(utilizationModel.getUtilization(0)).andReturn(0.91).anyTimes();
            //replay(utilizationModel);
            //testGetEstimatedFinishTime(utilizationModel);
            //verify(utilizationModel);
        }

        [TestMethod]
        public virtual void testGetEstimatedFinishTime(UtilizationModel utilizationModel)
        {
            Cloudlet cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModel, utilizationModel, utilizationModel);
            cloudlet.setResourceParameter(0, 0, 0);
            ResCloudlet rgl = new ResCloudlet(cloudlet);

            IList<double?> mipsShare = new List<double?>();
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);

            vmScheduler.CurrentMipsShare = mipsShare;

            double utilization = utilizationModel.getUtilization(0);
            double totalCurrentMipsForCloudlet = MIPS / 4 * PES_NUMBER;
            double requestedMips = (int)(utilization * PES_NUMBER * MIPS);
            if (requestedMips > totalCurrentMipsForCloudlet)
            {
                requestedMips = totalCurrentMipsForCloudlet;
            }

            double expectedFinishTime = (double)CLOUDLET_LENGTH * PES_NUMBER / requestedMips;
            double actualFinishTime = vmScheduler.getEstimatedFinishTime(rgl, 0);

            Assert.AreEqual(expectedFinishTime, actualFinishTime);
        }

        [TestMethod]
        public virtual void testCloudletSubmitLowUtilization()
        {
            UtilizationModel utilizationModel = new UtilizationModelLow();
            testCloudletSubmit(utilizationModel);

            //UtilizationModel utilizationModel = createMock(typeof(UtilizationModel));
            //expect(utilizationModel.getUtilization(0)).andReturn(0.11).anyTimes();
            //replay(utilizationModel);
            //testCloudletSubmit(utilizationModel);
            //verify(utilizationModel);
        }

        [TestMethod]
        public virtual void testCloudletSubmitHighUtilization()
        {
            UtilizationModel utilizationModel = new UtilizationModelHigh();
            testCloudletSubmit(utilizationModel);

            //UtilizationModel utilizationModel = createMock(typeof(UtilizationModel));
            //expect(utilizationModel.getUtilization(0)).andReturn(0.91).anyTimes();
            //replay(utilizationModel);
            //testCloudletSubmit(utilizationModel);
            //verify(utilizationModel);
        }

        [TestMethod]
        public virtual void testCloudletSubmit(UtilizationModel utilizationModel)
        {
            Cloudlet cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModel, utilizationModel, utilizationModel);
            cloudlet.setResourceParameter(0, 0, 0);

            IList<double?> mipsShare = new List<double?>();
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);

            vmScheduler.CurrentMipsShare = mipsShare;

            double utilization = utilizationModel.getUtilization(0);
            double totalCurrentMipsForCloudlet = MIPS / 4 * PES_NUMBER;
            double requestedMips = (int)(utilization * PES_NUMBER * MIPS);
            if (requestedMips > totalCurrentMipsForCloudlet)
            {
                requestedMips = totalCurrentMipsForCloudlet;
            }

            double expectedFinishTime = (double)CLOUDLET_LENGTH * PES_NUMBER / requestedMips;
            double actualFinishTime = vmScheduler.cloudletSubmit(cloudlet);

            Assert.AreEqual(expectedFinishTime, actualFinishTime);
        }

        [TestMethod]
        public virtual void testUpdateVmProcessingLowUtilization()
        {
            UtilizationModel utilizationModel = new UtilizationModelLow();
            testUpdateVmProcessing(utilizationModel);
            
            //UtilizationModel utilizationModel = createMock(typeof(UtilizationModel));
            //expect(utilizationModel.getUtilization(0)).andReturn(0.11).anyTimes();
            //expect(utilizationModel.getUtilization(1.0)).andReturn(0.11).anyTimes();
            //replay(utilizationModel);
            //testUpdateVmProcessing(utilizationModel);
            //verify(utilizationModel);
        }

        [TestMethod]
        public virtual void testUpdateVmProcessingHighUtilization()
        {
            UtilizationModel utilizationModel = new UtilizationModelHigh();
            testUpdateVmProcessing(utilizationModel);

            //UtilizationModel utilizationModel = createMock(typeof(UtilizationModel));
            //expect(utilizationModel.getUtilization(0)).andReturn(0.91).anyTimes();
            //expect(utilizationModel.getUtilization(1.0)).andReturn(0.91).anyTimes();
            //replay(utilizationModel);
            //testUpdateVmProcessing(utilizationModel);
            //verify(utilizationModel);
        }

        [TestMethod]
        public virtual void testUpdateVmProcessingLowAndHighUtilization()
        {
            UtilizationModel utilizationModel = new UtilizationModelLowAndHigh();
            testUpdateVmProcessing(utilizationModel);

            //UtilizationModel utilizationModel = createMock(typeof(UtilizationModel));
            //expect(utilizationModel.getUtilization(0)).andReturn(0.11).anyTimes();
            //expect(utilizationModel.getUtilization(1.0)).andReturn(0.91).anyTimes();
            //replay(utilizationModel);
            //testUpdateVmProcessing(utilizationModel);
            //verify(utilizationModel);
        }

        [TestMethod]
        public virtual void testUpdateVmProcessing(UtilizationModel utilizationModel)
        {
            Cloudlet cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModel, utilizationModel, utilizationModel);
            cloudlet.setResourceParameter(0, 0, 0);

            IList<double?> mipsShare = new List<double?>();
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);
            mipsShare.Add(MIPS / 4);

            vmScheduler.CurrentMipsShare = mipsShare;

            vmScheduler.cloudletSubmit(cloudlet);

            double totalCurrentMipsForCloudlet = MIPS / 4 * PES_NUMBER;

            double utilization1 = utilizationModel.getUtilization(0);
            double requestedMips1 = (int)(utilization1 * PES_NUMBER * MIPS);
            if (requestedMips1 > totalCurrentMipsForCloudlet)
            {
                requestedMips1 = totalCurrentMipsForCloudlet;
            }

            double expectedCompletiontime1 = ((double)CLOUDLET_LENGTH * PES_NUMBER) / requestedMips1;
            double actualCompletionTime1 = vmScheduler.updateVmProcessing(0, mipsShare);
            Assert.AreEqual(expectedCompletiontime1, actualCompletionTime1);

            double utilization2 = utilizationModel.getUtilization(1);
            double requestedMips2 = (int)(utilization2 * PES_NUMBER * MIPS);
            if (requestedMips2 > totalCurrentMipsForCloudlet)
            {
                requestedMips2 = totalCurrentMipsForCloudlet;
            }

            double expectedCompletiontime2 = 1 + ((CLOUDLET_LENGTH * PES_NUMBER - requestedMips1 * 1)) / requestedMips2;
            double actualCompletionTime2 = vmScheduler.updateVmProcessing(1, mipsShare);
            Assert.AreEqual(expectedCompletiontime2, actualCompletionTime2);

            Assert.IsFalse(vmScheduler.FinishedCloudlets);

            Assert.AreEqual(0, vmScheduler.updateVmProcessing(CLOUDLET_LENGTH, mipsShare));

            Assert.IsTrue(vmScheduler.FinishedCloudlets);
        }
    }

    /// <summary>
    /// Stands in for the original Java EasyMock objects.
    /// </summary>
    public class UtilizationModelHigh : UtilizationModel
    {
        /// <summary>
        /// Gets the utilization percentage of a given resource
        /// in relation to the total capacity of that resource allocated
        /// to the cloudlet. </summary>
        /// <param name="time"> the time to get the resource usage, that isn't considered
        /// for this UtilizationModel. </param>
        /// <returns> Always return .91 (91% of utilization), independent of the time. </returns>
        public virtual double getUtilization(double time)
        {
            return highUtilizationPercentage;
        }

        private const double highUtilizationPercentage = 0.91;
    }

    /// <summary>
    /// Stands in for the original Java EasyMock objects.
    /// </summary>
    public class UtilizationModelLow : UtilizationModel
    {
        /// <summary>
        /// Gets the utilization percentage of a given resource
        /// in relation to the total capacity of that resource allocated
        /// to the cloudlet. </summary>
        /// <param name="time"> the time to get the resource usage, that isn't considered
        /// for this UtilizationModel. </param>
        /// <returns> Always return .11 (11% of utilization), independent of the time. </returns>
        public virtual double getUtilization(double time)
        {
            return lowUtilizationPercentage;
        }

        private const double lowUtilizationPercentage = 0.11;
    }

    /// <summary>
    /// Stands in for the original Java EasyMock objects.
    /// </summary>
    public class UtilizationModelLowAndHigh : UtilizationModel
    {
        /// <summary>
        /// Gets the utilization percentage of a given resource
        /// in relation to the total capacity of that resource allocated
        /// to the cloudlet. </summary>
        /// <param name="time"> the time to get the resource usage, that isn't considered
        /// for this UtilizationModel. </param>
        /// <returns> Always return .11 (11% of utilization), independent of the time. </returns>
        public virtual double getUtilization(double time)
        {
            if (time == 0)
            {
                return lowUtilizationPercentage;
            }
            else
            {
                return highUtilizationPercentage;
            }
        }

        private const double lowUtilizationPercentage = 0.11;
        private const double highUtilizationPercentage = 0.91;
    }
}