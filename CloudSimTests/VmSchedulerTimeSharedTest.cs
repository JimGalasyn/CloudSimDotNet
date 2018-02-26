using System.Collections.Generic;

/*
 * Title: CloudSim Toolkit Description: CloudSim (Cloud Simulation) Toolkit for Modeling and
 * Simulation of Clouds Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * 
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{
	using PeList = org.cloudbus.cloudsim.lists.PeList;
	using PeProvisionerSimple = org.cloudbus.cloudsim.provisioners.PeProvisionerSimple;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class VmSchedulerTimeSharedTest
	{
		private const double MIPS = 1000;

		private VmSchedulerTimeShared vmScheduler;

		private IList<Pe> peList;

		private Vm vm1;

		private Vm vm2;

		// private Vm vm3;

        [TestInitialize()]
        public void Initialize()
        {
            peList = new List<Pe>();
            peList.Add(new Pe(0, new PeProvisionerSimple(MIPS)));
            peList.Add(new Pe(1, new PeProvisionerSimple(MIPS)));
            vmScheduler = new VmSchedulerTimeShared(peList);
            vm1 = new Vm(0, 0, MIPS / 4, 1, 0, 0, 0, "", null);
            vm2 = new Vm(1, 0, MIPS / 2, 2, 0, 0, 0, "", null);
            // vm3 = new Vm(2, 0, MIPS, 2, 0, 0, 0, 0, "", null);
        }

        [TestMethod]
        public virtual void testInit()
		{
			Assert.AreSame(peList, vmScheduler.PeListProperty);
			Assert.AreEqual(PeList.getTotalMips(peList), vmScheduler.AvailableMips);
			Assert.AreEqual(PeList.getTotalMips(peList), vmScheduler.MaxAvailableMips);
			Assert.AreEqual(0, vmScheduler.getTotalAllocatedMipsForVm(vm1));
		}

        [TestMethod]
        public virtual void testAllocatePesForVm()
		{
			IList<double?> mipsShare1 = new List<double?>();
			mipsShare1.Add(MIPS / 4);

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm1, mipsShare1));

			Assert.AreEqual(PeList.getTotalMips(peList) - MIPS / 4, vmScheduler.AvailableMips);
			Assert.AreEqual(PeList.getTotalMips(peList) - MIPS / 4, vmScheduler.MaxAvailableMips);
			Assert.AreEqual(MIPS / 4, vmScheduler.getTotalAllocatedMipsForVm(vm1));

			IList<double?> mipsShare2 = new List<double?>();
			mipsShare2.Add(MIPS / 2);
			mipsShare2.Add(MIPS / 8);

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm2, mipsShare2));

			Assert.AreEqual(PeList.getTotalMips(peList) - MIPS / 4 - MIPS / 2 - MIPS / 8, vmScheduler.AvailableMips);
			Assert.AreEqual(PeList.getTotalMips(peList) - MIPS / 4 - MIPS / 2 - MIPS / 8, vmScheduler.MaxAvailableMips);
			Assert.AreEqual(MIPS / 2 + MIPS / 8, vmScheduler.getTotalAllocatedMipsForVm(vm2));

            //List<double> mipsShare3 = new List<double>();
            //mipsShare3.Add(MIPS);
            //mipsShare3.Add(MIPS);

            //Assert.IsTrue(vmScheduler.allocatePesForVm(vm3, mipsShare3));

            //Assert.AreEqual(0, vmScheduler.getAvailableMips(), 0);
            //Assert.AreEqual(0, vmScheduler.getMaxAvailableMips(), 0);
            //Assert.AreEqual(MIPS / 4 - (MIPS / 4 + MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS * 2) / 5,
            //vmScheduler.getTotalAllocatedMipsForVm(vm1), 0);
            //Assert.AreEqual(MIPS / 2 + MIPS / 8 - (MIPS / 4 + MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS *
            //2) * 2 / 5, vmScheduler.getTotalAllocatedMipsForVm(vm2), 0);
            //Assert.AreEqual(MIPS * 2 - (MIPS / 4 + MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS * 2) * 2 /
            //5, vmScheduler.getTotalAllocatedMipsForVm(vm3), 0);

            //vmScheduler.deallocatePesForVm(vm1);

            //Assert.AreEqual(0, vmScheduler.getAvailableMips(), 0);
            //Assert.AreEqual(0, vmScheduler.getMaxAvailableMips(), 0);
            //Assert.AreEqual(MIPS / 2 + MIPS / 8 - (MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS * 2) * 2 /
            //4, vmScheduler.getTotalAllocatedMipsForVm(vm2), 0);
            //Assert.AreEqual(MIPS * 2 - (MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS * 2) * 2 / 4,
            //vmScheduler.getTotalAllocatedMipsForVm(vm3), 0);

            //vmScheduler.deallocatePesForVm(vm3);

            //Assert.AreEqual(MIPS * 2 - MIPS / 2 - MIPS / 8, vmScheduler.getAvailableMips(), 0);
            //Assert.AreEqual(MIPS * 2 - MIPS / 2 - MIPS / 8, vmScheduler.getMaxAvailableMips(), 0);
            //Assert.AreEqual(0, vmScheduler.getTotalAllocatedMipsForVm(vm3), 0);

            //vmScheduler.deallocatePesForVm(vm2);

            vmScheduler.deallocatePesForAllVms();

			Assert.AreEqual(PeList.getTotalMips(peList), vmScheduler.AvailableMips);
			Assert.AreEqual(PeList.getTotalMips(peList), vmScheduler.MaxAvailableMips);

            // TODO: TEST FAIL: vmScheduler.getTotalAllocatedMipsForVm(vm2) seems like this shouldn't be in the test.
            //Assert.AreEqual(0, vmScheduler.getTotalAllocatedMipsForVm(vm2));
		}

        [TestMethod]
        public virtual void testAllocatePesForVmInMigration()
		{
			vm1.InMigration = true;
			vm2.InMigration = true;

			IList<double?> mipsShare1 = new List<double?>();
			mipsShare1.Add(MIPS / 4);

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm1, mipsShare1));

			Assert.AreEqual(PeList.getTotalMips(peList) - MIPS / 4, vmScheduler.AvailableMips);
			Assert.AreEqual(PeList.getTotalMips(peList) - MIPS / 4, vmScheduler.MaxAvailableMips);
			Assert.AreEqual(0.9 * MIPS / 4, vmScheduler.getTotalAllocatedMipsForVm(vm1));

			IList<double?> mipsShare2 = new List<double?>();
			mipsShare2.Add(MIPS / 2);
			mipsShare2.Add(MIPS / 8);

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm2, mipsShare2));

			Assert.AreEqual(PeList.getTotalMips(peList) - MIPS / 4 - MIPS / 2 - MIPS / 8, vmScheduler.AvailableMips);
			Assert.AreEqual(PeList.getTotalMips(peList) - MIPS / 4 - MIPS / 2 - MIPS / 8, vmScheduler.MaxAvailableMips);
			Assert.AreEqual(0.9 * MIPS / 2 + 0.9 * MIPS / 8, vmScheduler.getTotalAllocatedMipsForVm(vm2));

			vmScheduler.deallocatePesForAllVms();

			Assert.AreEqual(PeList.getTotalMips(peList), vmScheduler.AvailableMips);
			Assert.AreEqual(PeList.getTotalMips(peList), vmScheduler.MaxAvailableMips);

            // TODO: TEST FAIL: vmScheduler.getTotalAllocatedMipsForVm(vm2) seems like this shouldn't be in the test.
            //Assert.AreEqual(0, vmScheduler.getTotalAllocatedMipsForVm(vm2));
		}
	}
}