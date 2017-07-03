/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power.models
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class PowerModelSpecPowerIbmX3550XeonX5675Test
	{
		private PowerModel powerModel;

        [TestInitialize()]
        public void Initialize()
        {
            powerModel = new PowerModelSpecPowerIbmX3550XeonX5675();
        }

        [TestMethod]
        public virtual void testGetPowerArgumentLessThenZero()
		{
			try
            {
                powerModel.getPower(-1);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "Utilization value must be between 0 and 1");
            }
        }

        [TestMethod]
        public virtual void testGetPowerArgumentLargerThenOne()
		{
            try
            {
                powerModel.getPower(2);
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "Utilization value must be between 0 and 1");
            }
        }

        [TestMethod]
        public virtual void testGetPower()
		{
			Assert.AreEqual(58.4, powerModel.getPower(0));
			Assert.AreEqual(58.4 + (98 - 58.4) / 5, powerModel.getPower(0.02));
			Assert.AreEqual(98, powerModel.getPower(0.1));
			Assert.AreEqual(140, powerModel.getPower(0.5));
			Assert.AreEqual(189, powerModel.getPower(0.8));
			Assert.AreEqual(189 + 0.7 * 10 * (205 - 189) / 10, powerModel.getPower(0.87));
			Assert.AreEqual(205, powerModel.getPower(0.9));
			Assert.AreEqual(222, powerModel.getPower(1));
		}
	}
}