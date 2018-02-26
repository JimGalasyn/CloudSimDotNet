namespace org.cloudbus.cloudsim
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class UtilizationModelPlanetLabInMemoryTest
	{
		public const double SCHEDULING_INTERVAL = 300;

		public const string FILE = "146-179_surfsnel_dsl_internl_net_colostate_557.dat";

        private UtilizationModelPlanetLabInMemory utilizationModel;

        [TestInitialize()]
        public void Initialize()
        {
            utilizationModel = new UtilizationModelPlanetLabInMemory(
                @".\resources\146-179_surfsnel_dsl_internl_net_colostate_557.dat", 
                SCHEDULING_INTERVAL);
        }

        [TestMethod]
        public virtual void testGetPowerModel()
		{
            Assert.AreEqual(0.24, utilizationModel.getUtilization(0));
			Assert.AreEqual(0.34, utilizationModel.getUtilization(1 * SCHEDULING_INTERVAL));
            Assert.AreEqual((24 + 0.2 * SCHEDULING_INTERVAL * (34 - 24) / SCHEDULING_INTERVAL) / 100, utilizationModel.getUtilization(0.2 * SCHEDULING_INTERVAL), 0.01);
            var utilization1 = (24 + 0.2 * SCHEDULING_INTERVAL * (34 - 24) / SCHEDULING_INTERVAL) / 100;
            var utilization2 = utilizationModel.getUtilization(0.2 * SCHEDULING_INTERVAL);
            Assert.IsTrue(Math.Abs(utilization1-utilization2) <= 0.01);
            Assert.AreEqual(0.29, utilizationModel.getUtilization(2 * SCHEDULING_INTERVAL));
			Assert.AreEqual(0.18, utilizationModel.getUtilization(136 * SCHEDULING_INTERVAL));
            Assert.AreEqual((18 + 0.7 * SCHEDULING_INTERVAL * (21 - 18) / SCHEDULING_INTERVAL) / 100, utilizationModel.getUtilization(136.7 * SCHEDULING_INTERVAL), 0.01);
            utilization1 = (18 + 0.7 * SCHEDULING_INTERVAL * (21 - 18) / SCHEDULING_INTERVAL) / 100;
            utilization2 = utilizationModel.getUtilization(136.7 * SCHEDULING_INTERVAL);
            Assert.IsTrue(Math.Abs(utilization1 - utilization2) <= 0.01);
            Assert.AreEqual(0.51, utilizationModel.getUtilization(287 * SCHEDULING_INTERVAL));
		}
	}
}