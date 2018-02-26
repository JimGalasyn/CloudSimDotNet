using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.lists
{
    using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class PeListTest
	{
		private const double MIPS = 1000;

		private IList<Pe> peList;

        [TestInitialize()]
        public void Initialize()
        {
            peList = new List<Pe>();
            peList.Add(new Pe(0, new PeProvisionerSimple(MIPS)));
            peList.Add(new Pe(1, new PeProvisionerSimple(MIPS)));
        }

        [TestMethod]
        public virtual void testGetMips()
		{
            Assert.AreEqual(MIPS, PeList.getMips(peList, 0));
			Assert.AreEqual(MIPS, PeList.getMips(peList, 1));
            Assert.AreEqual(-1, PeList.getMips(peList, 2));
		}

        [TestMethod]
        public virtual void testGetTotalMips()
		{
            Assert.AreEqual(MIPS * peList.Count, PeList.getTotalMips(peList));
		}

        [TestMethod]
        public virtual void testSetPeStatus()
		{
			Assert.AreEqual(2, PeList.getNumberOfFreePes(peList));
			Assert.AreEqual(0, PeList.getNumberOfBusyPes(peList));
			Assert.IsTrue(PeList.setPeStatus(peList, 0, Pe.BUSY));
			Assert.AreEqual(Pe.BUSY, PeList.getById(peList, 0).Status);
			Assert.AreEqual(1, PeList.getNumberOfFreePes(peList));
			Assert.AreEqual(1, PeList.getNumberOfBusyPes(peList));
			Assert.IsTrue(PeList.setPeStatus(peList, 1, Pe.BUSY));
			Assert.AreEqual(Pe.BUSY, PeList.getById(peList, 1).Status);
			Assert.AreEqual(0, PeList.getNumberOfFreePes(peList));
			Assert.AreEqual(2, PeList.getNumberOfBusyPes(peList));
			Assert.IsFalse(PeList.setPeStatus(peList, 2, Pe.BUSY));
			Assert.AreEqual(0, PeList.getNumberOfFreePes(peList));
			Assert.AreEqual(2, PeList.getNumberOfBusyPes(peList));
		}

        [TestMethod]
        public virtual void testSetStatusFailed()
		{
			Assert.AreEqual(Pe.FREE, PeList.getById(peList, 0).Status);
			Assert.AreEqual(Pe.FREE, PeList.getById(peList, 1).Status);
			PeList.setStatusFailed(peList, true);
			Assert.AreEqual(Pe.FAILED, PeList.getById(peList, 0).Status);
			Assert.AreEqual(Pe.FAILED, PeList.getById(peList, 1).Status);
			PeList.setStatusFailed(peList, false);
			Assert.AreEqual(Pe.FREE, PeList.getById(peList, 0).Status);
			Assert.AreEqual(Pe.FREE, PeList.getById(peList, 1).Status);

			PeList.setStatusFailed(peList, "test", 0, true);
			Assert.AreEqual(Pe.FAILED, PeList.getById(peList, 0).Status);
			Assert.AreEqual(Pe.FAILED, PeList.getById(peList, 1).Status);
			PeList.setStatusFailed(peList, "test", 0, false);
			Assert.AreEqual(Pe.FREE, PeList.getById(peList, 0).Status);
			Assert.AreEqual(Pe.FREE, PeList.getById(peList, 1).Status);
		}

        [TestMethod]
        public virtual void testFreePe()
		{
			Assert.AreSame(peList[0], PeList.getFreePe(peList));
			PeList.setPeStatus(peList, 0, Pe.BUSY);
			Assert.AreSame(peList[1], PeList.getFreePe(peList));
			PeList.setPeStatus(peList, 1, Pe.BUSY);
			Assert.IsNull(PeList.getFreePe(peList));
		}

        [TestMethod]
        public virtual void testGetMaxUtilization()
		{
			Vm vm0 = new Vm(0, 0, MIPS / 2, 1, 0, 0, 0, "", null);
			Vm vm1 = new Vm(1, 0, MIPS / 2, 1, 0, 0, 0, "", null);

			Assert.IsTrue(peList[0].PeProvisioner.allocateMipsForVm(vm0, MIPS / 3));
			Assert.IsTrue(peList[1].PeProvisioner.allocateMipsForVm(vm1, MIPS / 5));

            //Assert.AreEqual((MIPS / 3) / MIPS, PeList.getMaxUtilization(peList), 0.001);
            Assert.IsTrue(Math.Abs((MIPS / 3) / MIPS - PeList.getMaxUtilization(peList)) <= 0.001);
        }

        [TestMethod]
        public virtual void testGetMaxUtilizationAmongVmsPes()
		{
			Vm vm0 = new Vm(0, 0, MIPS / 2, 1, 0, 0, 0, "", null);
			Vm vm1 = new Vm(1, 0, MIPS / 2, 1, 0, 0, 0, "", null);

			Assert.IsTrue(peList[0].PeProvisioner.allocateMipsForVm(vm0, MIPS / 3));
			Assert.IsTrue(peList[1].PeProvisioner.allocateMipsForVm(vm1, MIPS / 5));

            //Assert.AreEqual((MIPS / 3) / MIPS, PeList.getMaxUtilizationAmongVmsPes(peList, vm0), 0.001);
            //Assert.AreEqual((MIPS / 5) / MIPS, PeList.getMaxUtilizationAmongVmsPes(peList, vm1), 0.001);
            Assert.IsTrue(Math.Abs((MIPS / 3) / MIPS - PeList.getMaxUtilizationAmongVmsPes(peList, vm0)) <= 0.001);
            Assert.IsTrue(Math.Abs((MIPS / 5) / MIPS - PeList.getMaxUtilizationAmongVmsPes(peList, vm1)) <= 0.001);
        }
	}
}