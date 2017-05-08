using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power
{
	using PowerModelLinear = org.cloudbus.cloudsim.power.models.PowerModelLinear;
	using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class PowerHostTest
	{
		private const double MIPS = 1000;
		private const double MAX_POWER = 200;
		private const double STATIC_POWER_PERCENT = 0.3;
		private const double TIME = 10;

		private PowerHost host;

		//public virtual void setUp()
		//{
		//	IList<Pe> peList = new List<Pe>();
		//	peList.Add(new Pe(0, new PeProvisionerSimple(MIPS)));
		//	host = new PowerHost(0, null, null, 0, peList, null, new PowerModelLinear(MAX_POWER, STATIC_POWER_PERCENT));
		//}

        [TestInitialize()]
        public void Initialize()
        {
            IList<Pe> peList = new List<Pe>();
            peList.Add(new Pe(0, new PeProvisionerSimple(MIPS)));
            host = new PowerHost(0, null, null, 0, peList, null, new PowerModelLinear(MAX_POWER, STATIC_POWER_PERCENT));
        }

        [TestMethod]
        public virtual void testGetMaxPower()
		{
			Assert.AreEqual(MAX_POWER, host.MaxPower);
		}

        [TestMethod]
        public virtual void testGetEnergy()
		{
			Assert.AreEqual(0, host.getEnergyLinearInterpolation(0, 0, TIME));
			double expectedEnergy = 0;
			try
			{
				expectedEnergy = (host.PowerModel.getPower(0.2) + (host.PowerModel.getPower(0.9) - host.PowerModel.getPower(0.2)) / 2) * TIME;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
                Assert.Fail();
			}

            Assert.AreEqual(expectedEnergy, host.getEnergyLinearInterpolation(0.2, 0.9, TIME));
		}
	}
}