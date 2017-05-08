/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{
    using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class PeTest
	{
        // TODO: why double, not int?
		private const double MIPS = 1000;

        [TestMethod]
        public virtual void testGetPeProvisioner()
		{
			PeProvisionerSimple peProvisioner = new PeProvisionerSimple(MIPS);
			Pe pe = new Pe(0, peProvisioner);
            Assert.AreSame(peProvisioner, pe.PeProvisioner);
            Assert.AreEqual(MIPS, pe.PeProvisioner.AvailableMips);
        }

        [TestMethod]
        public virtual void testSetId()
		{
			Pe pe = new Pe(0, null);
            Assert.AreEqual(0, pe.Id);

            pe.Id = 1;
            Assert.AreEqual(1, pe.Id);
        }

        [TestMethod]
        public virtual void testSetMips()
		{
			PeProvisionerSimple peProvisioner = new PeProvisionerSimple(MIPS);
			Pe pe = new Pe(0, peProvisioner);
            Assert.AreEqual(MIPS, pe.Mips);
            pe.Mips = (int)MIPS/2;
            Assert.AreEqual(MIPS/2, pe.Mips);
        }

        [TestMethod]
        public virtual void testSetStatus()
		{
			Pe pe = new Pe(0, null);
            Assert.AreEqual(Pe.FREE, pe.Status);
            pe.setStatusBusy();
            Assert.AreEqual(Pe.BUSY, pe.Status);
            pe.setStatusFailed();
            Assert.AreEqual(Pe.FAILED, pe.Status);
            pe.setStatusFree();
            Assert.AreEqual(Pe.FREE, pe.Status);
        }
	}
}