using System.Collections.Generic;

/*
 * Title: CloudSim Toolkit Description: CloudSim (Cloud Simulation) Toolkit for Modeling and
 * Simulation of Clouds Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * 
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{
	using org.cloudbus.cloudsim.lists;
	using org.cloudbus.cloudsim.provisioners;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class VmSchedulerTimeSharedOverSubscriptionTest
	{
		private const double MIPS = 1000;

		private VmSchedulerTimeSharedOverSubscription vmScheduler;

		private IList<Pe> peList;

		private Vm vm1;

		private Vm vm2;

		// private Vm vm3;

		//public virtual void setUp()
		//{
		//	peList = new List<Pe>();
		//	peList.Add(new Pe(0, new PeProvisionerSimple(MIPS)));
		//	peList.Add(new Pe(1, new PeProvisionerSimple(MIPS)));
		//	vmScheduler = new VmSchedulerTimeSharedOverSubscription(peList);
		//	vm1 = new Vm(0, 0, MIPS / 4, 1, 0, 0, 0, "", null);
		//	vm2 = new Vm(1, 0, MIPS / 2, 2, 0, 0, 0, "", null);
		//	// vm3 = new Vm(2, 0, MIPS, 2, 0, 0, 0, 0, "", null);
		//}

        [TestInitialize()]
        public void Initialize()
        {
            peList = new List<Pe>();
            peList.Add(new Pe(0, new PeProvisionerSimple(MIPS)));
            peList.Add(new Pe(1, new PeProvisionerSimple(MIPS)));
            vmScheduler = new VmSchedulerTimeSharedOverSubscription(peList);
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

			// List<Double> mipsShare3 = new ArrayList<Double>();
			// mipsShare3.add(MIPS);
			// mipsShare3.add(MIPS);
			//
			// Assert.IsTrue(vmScheduler.allocatePesForVm(vm3, mipsShare3));
			//
			// Assert.AreEqual(0, vmScheduler.getAvailableMips(), 0);
			// Assert.AreEqual(0, vmScheduler.getMaxAvailableMips(), 0);
			// Assert.AreEqual(MIPS / 4 - (MIPS / 4 + MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS * 2) / 5,
			// vmScheduler.getTotalAllocatedMipsForVm(vm1), 0);
			// Assert.AreEqual(MIPS / 2 + MIPS / 8 - (MIPS / 4 + MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS *
			// 2) * 2 / 5, vmScheduler.getTotalAllocatedMipsForVm(vm2), 0);
			// Assert.AreEqual(MIPS * 2 - (MIPS / 4 + MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS * 2) * 2 /
			// 5, vmScheduler.getTotalAllocatedMipsForVm(vm3), 0);
			//
			// vmScheduler.deallocatePesForVm(vm1);
			//
			// Assert.AreEqual(0, vmScheduler.getAvailableMips(), 0);
			// Assert.AreEqual(0, vmScheduler.getMaxAvailableMips(), 0);
			// Assert.AreEqual(MIPS / 2 + MIPS / 8 - (MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS * 2) * 2 /
			// 4, vmScheduler.getTotalAllocatedMipsForVm(vm2), 0);
			// Assert.AreEqual(MIPS * 2 - (MIPS / 2 + MIPS / 8 + MIPS + MIPS - MIPS * 2) * 2 / 4,
			// vmScheduler.getTotalAllocatedMipsForVm(vm3), 0);
			//
			// vmScheduler.deallocatePesForVm(vm3);
			//
			// Assert.AreEqual(MIPS * 2 - MIPS / 2 - MIPS / 8, vmScheduler.getAvailableMips(), 0);
			// Assert.AreEqual(MIPS * 2 - MIPS / 2 - MIPS / 8, vmScheduler.getMaxAvailableMips(), 0);
			// Assert.AreEqual(0, vmScheduler.getTotalAllocatedMipsForVm(vm3), 0);
			//
			// vmScheduler.deallocatePesForVm(vm2);

			vmScheduler.deallocatePesForAllVms();

			Assert.AreEqual(PeList.getTotalMips(peList), vmScheduler.AvailableMips);
			Assert.AreEqual(PeList.getTotalMips(peList), vmScheduler.MaxAvailableMips);

            Assert.AreEqual(0, vmScheduler.getTotalAllocatedMipsForVm(vm2));
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

            Assert.AreEqual(0, vmScheduler.getTotalAllocatedMipsForVm(vm2));
		}

        [TestMethod]
        public virtual void testAllocatePesForVmShortageEqualsToAllocatedMips()
		{
			IList<Pe> peList = new List<Pe>();
			peList.Add(new Pe(0, new PeProvisionerSimple(3500)));
			VmScheduler vmScheduler = new VmSchedulerTimeSharedOverSubscription(peList);
			Vm vm1 = new Vm(0, 0, 170, 1, 0, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, 2000, 1, 0, 0, 0, "", null);
			Vm vm3 = new Vm(2, 0, 10, 1, 0, 0, 0, "", null);
			Vm vm4 = new Vm(3, 0, 2000, 1, 0, 0, 0, "", null);

			IList<double?> mipsShare1 = new List<double?>();
			mipsShare1.Add(170.0);

			IList<double?> mipsShare2 = new List<double?>();
			mipsShare2.Add(2000.0);

			IList<double?> mipsShare3 = new List<double?>();
			mipsShare3.Add(10.0);

			IList<double?> mipsShare4 = new List<double?>();
			mipsShare4.Add(2000.0);

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm1, mipsShare1));
			Assert.AreEqual(3330, vmScheduler.AvailableMips);
			Assert.AreEqual(170, vmScheduler.getTotalAllocatedMipsForVm(vm1));

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm2, mipsShare2));
			Assert.AreEqual(1330, vmScheduler.AvailableMips);
			Assert.AreEqual(2000, vmScheduler.getTotalAllocatedMipsForVm(vm2));

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm3, mipsShare3));
			Assert.AreEqual(1320, vmScheduler.AvailableMips);
			Assert.AreEqual(10, vmScheduler.getTotalAllocatedMipsForVm(vm3));

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm4, mipsShare4));
			Assert.AreEqual(0, vmScheduler.AvailableMips);
			Assert.AreEqual(1674, vmScheduler.getTotalAllocatedMipsForVm(vm4));

			vmScheduler.deallocatePesForAllVms();

			Assert.AreEqual(3500, vmScheduler.AvailableMips);
			Assert.AreEqual(3500, vmScheduler.MaxAvailableMips);
		}

        [TestMethod]
        public virtual void testAllocatePesForSameSizedVmsOversubscribed()
		{
			IList<Pe> peList = new List<Pe>();
			peList.Add(new Pe(0, new PeProvisionerSimple(1000)));
			VmScheduler vmScheduler = new VmSchedulerTimeSharedOverSubscription(peList);
			Vm vm1 = new Vm(0, 0, 1500, 1, 0, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, 1000, 1, 0, 0, 0, "", null);
			Vm vm3 = new Vm(2, 0, 1000, 1, 0, 0, 0, "", null);

			IList<double?> mipsShare1 = new List<double?>();
			mipsShare1.Add(1500.0);

			IList<double?> mipsShare2 = new List<double?>();
			mipsShare2.Add(1000.0);

			IList<double?> mipsShare3 = new List<double?>();
			mipsShare3.Add(1000.0);

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm1, mipsShare1));
			Assert.AreEqual(0, vmScheduler.AvailableMips);
			Assert.AreEqual(1000, vmScheduler.getTotalAllocatedMipsForVm(vm1));

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm2, mipsShare2));
			Assert.AreEqual(0, vmScheduler.AvailableMips);
			Assert.AreEqual(500, vmScheduler.getTotalAllocatedMipsForVm(vm1));
			Assert.AreEqual(500, vmScheduler.getTotalAllocatedMipsForVm(vm2));

			Assert.IsTrue(vmScheduler.allocatePesForVm(vm3, mipsShare3));
			Assert.AreEqual(0, vmScheduler.AvailableMips);
			Assert.AreEqual(333, vmScheduler.getTotalAllocatedMipsForVm(vm1));
			Assert.AreEqual(333, vmScheduler.getTotalAllocatedMipsForVm(vm2));
			Assert.AreEqual(333, vmScheduler.getTotalAllocatedMipsForVm(vm3));

			vmScheduler.deallocatePesForAllVms();

			Assert.AreEqual(1000, vmScheduler.AvailableMips);
			Assert.AreEqual(1000, vmScheduler.MaxAvailableMips);
		}
	}
}