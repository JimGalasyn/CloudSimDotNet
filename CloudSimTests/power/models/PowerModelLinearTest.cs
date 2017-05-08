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
    public class PowerModelLinearTest
	{
		private const double MAX_POWER = 250;
		private const double STATIC_POWER_PERCENT = 0.7;

		private PowerModelLinear powerModel;

		//public virtual void setUp()
		//{
		//	powerModel = new PowerModelLinear(MAX_POWER, STATIC_POWER_PERCENT);
		//}

        [TestInitialize()]
        public void Initialize()
        {
            powerModel = new PowerModelLinear(MAX_POWER, STATIC_POWER_PERCENT);
        }

        [TestMethod]
        public virtual void testGetMaxPower()
		{
			Assert.AreEqual(MAX_POWER, powerModel.MaxPower);
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
			Assert.AreEqual(0, powerModel.getPower(0.0));
			Assert.AreEqual(MAX_POWER, powerModel.getPower(1.0));
			Assert.AreEqual(MAX_POWER * STATIC_POWER_PERCENT + ((MAX_POWER - MAX_POWER * STATIC_POWER_PERCENT) / 100) * 0.5 * 100, powerModel.getPower(0.5));
		}

        [TestMethod]
        public virtual void testPrintPower()
		{
			for (int i = 0; i <= 100; i++)
			{
				Log.print(string.Format("{0:D};{1:F2}\n", i, powerModel.getPower((double) i / 100)));
			}
		}
	}
}