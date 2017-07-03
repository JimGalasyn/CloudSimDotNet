using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{
    using BwProvisionerSimple = org.cloudbus.cloudsim.provisioners.BwProvisionerSimple;
    using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using RamProvisionerSimple = org.cloudbus.cloudsim.provisioners.RamProvisionerSimple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class HostDynamicWorkloadTest
	{
		private const int ID = 0;
		private static readonly long STORAGE = Consts.MILLION;
		private const int RAM = 1024;
		private const int BW = 10000;
		private const double MIPS = 1000;

		private HostDynamicWorkload host;
		private IList<Pe> peList;

        [TestInitialize()]
        public void Initialize()
        {
            peList = new List<Pe>();
            peList.Add(new Pe(0, new PeProvisionerSimple(MIPS)));
            peList.Add(new Pe(1, new PeProvisionerSimple(MIPS)));

            host = new HostDynamicWorkload(
                ID, 
                new RamProvisionerSimple(RAM), 
                new BwProvisionerSimple(BW), 
                STORAGE, 
                peList, 
                new VmSchedulerTimeShared(peList)
           );
        }

        [TestMethod]
        public virtual void testGetUtilizationOfCPU()
		{
			Assert.AreEqual(0, host.UtilizationOfCpu);
		}

        [TestMethod]
        public virtual void testGetUtilizationOfCPUMips()
		{
			Assert.AreEqual(0, host.UtilizationOfCpuMips);
		}

        [TestMethod]
        public virtual void testGetUtilizationOfRam()
		{
			Assert.AreEqual(0, host.UtilizationOfRam);
		}

        [TestMethod]
        public virtual void testGetUtilizationOfBW()
		{
			Assert.AreEqual(0, host.UtilizationOfBw);
		}

        [TestMethod]
        public virtual void testGetMaxUtilization()
		{
			Vm vm0 = new Vm(0, 0, MIPS / 2, 1, 0, 0, 0, "", null);
			Vm vm1 = new Vm(1, 0, MIPS / 2, 1, 0, 0, 0, "", null);

			Assert.IsTrue(peList[0].PeProvisioner.allocateMipsForVm(vm0, MIPS / 3));
			Assert.IsTrue(peList[1].PeProvisioner.allocateMipsForVm(vm1, MIPS / 5));

            //Assert.AreEqual((MIPS / 3) / MIPS, host.MaxUtilization, 0.001);
            Assert.IsTrue(Math.Abs((MIPS / 3) / MIPS - host.MaxUtilization) <= 0.001);
        }

        [TestMethod]
        public virtual void testGetMaxUtilizationAmongVmsPes()
		{
			Vm vm0 = new Vm(0, 0, MIPS / 2, 1, 0, 0, 0, "", null);
			Vm vm1 = new Vm(1, 0, MIPS / 2, 1, 0, 0, 0, "", null);

			Assert.IsTrue(peList[0].PeProvisioner.allocateMipsForVm(vm0, MIPS / 3));
			Assert.IsTrue(peList[1].PeProvisioner.allocateMipsForVm(vm1, MIPS / 5));

            //Assert.AreEqual((MIPS / 3) / MIPS, host.getMaxUtilizationAmongVmsPes(vm0), 0.001);
            //Assert.AreEqual((MIPS / 5) / MIPS, host.getMaxUtilizationAmongVmsPes(vm1), 0.001);
            Assert.IsTrue(Math.Abs((MIPS / 3) / MIPS - host.getMaxUtilizationAmongVmsPes(vm0)) <= 0.001);
            Assert.IsTrue(Math.Abs((MIPS / 5) / MIPS - host.getMaxUtilizationAmongVmsPes(vm1)) <= 0.001);
        }
    }
}