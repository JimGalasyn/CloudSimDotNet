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
    public class UtilizationModelStochasticTest
	{
		private UtilizationModelStochastic utilizationModel;

        [TestInitialize()]
        public void Initialize()
        {
            utilizationModel = new UtilizationModelStochastic();
        }

        /// <summary>
        /// Test method for <seealso cref="cloudsim.UtilizationModelStochastic.getUtilization(double)"/>.
        /// </summary>
        [TestMethod]
        public virtual void testGetUtilization()
		{
			double utilization0 = utilizationModel.getUtilization(0);
			double utilization1 = utilizationModel.getUtilization(1);
			Assert.IsNotNull(utilization0);
			Assert.IsNotNull(utilization1);
			Assert.AreNotSame(utilization0, utilization1);
			Assert.AreEqual(utilization0, utilizationModel.getUtilization(0));
            Assert.AreEqual(utilization1, utilizationModel.getUtilization(1));
		}
	}
}