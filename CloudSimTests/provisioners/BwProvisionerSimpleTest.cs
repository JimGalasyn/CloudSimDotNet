/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.provisioners
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class BwProvisionerSimpleTest
	{

		private const long BW = 1000;

		private BwProvisionerSimple bwProvisioner;

        [TestInitialize()]
        public void Initialize()
        {
            bwProvisioner = new BwProvisionerSimple(BW);
        }

        [TestMethod]
        public virtual void testGetBw()
		{
			Assert.AreEqual(BW, bwProvisioner.Bw);
		}

        [TestMethod]
        public virtual void testGetAvailableBw()
		{
			Assert.AreEqual(BW, bwProvisioner.AvailableBw);
		}

        [TestMethod]
        public virtual void testAllocateBwforVm()
		{
			Vm vm1 = new Vm(0, 0, 0, 0, 0, BW / 2, 0, "", null);
			Vm vm2 = new Vm(1, 0, 0, 0, 0, BW, 0, "", null);

			Assert.IsTrue(bwProvisioner.isSuitableForVm(vm1, BW / 2));
			Assert.IsTrue(bwProvisioner.allocateBwForVm(vm1, BW / 2));
			Assert.AreEqual(BW / 2, bwProvisioner.AvailableBw);

			Assert.IsFalse(bwProvisioner.isSuitableForVm(vm2, BW));
			Assert.IsFalse(bwProvisioner.allocateBwForVm(vm2, BW));
			Assert.AreEqual(BW / 2, bwProvisioner.AvailableBw);

			Assert.IsTrue(bwProvisioner.isSuitableForVm(vm2, BW / 4));
			Assert.IsTrue(bwProvisioner.allocateBwForVm(vm2, BW / 4));
			Assert.AreEqual(BW * 1 / 4, bwProvisioner.AvailableBw);

			Assert.IsTrue(bwProvisioner.isSuitableForVm(vm2, BW / 2));
			Assert.IsTrue(bwProvisioner.allocateBwForVm(vm2, BW / 2));
			Assert.AreEqual(0, bwProvisioner.AvailableBw);
		}

        [TestMethod]
        public virtual void testGetAllocatedBwforVm()
		{
			Vm vm1 = new Vm(0, 0, 0, 0, 0, BW / 2, 0, "", null);
			Vm vm2 = new Vm(1, 0, 0, 0, 0, BW, 0, "", null);

			Assert.IsTrue(bwProvisioner.isSuitableForVm(vm1, BW / 2));
			Assert.IsTrue(bwProvisioner.allocateBwForVm(vm1, BW / 2));
			Assert.AreEqual(BW / 2, bwProvisioner.getAllocatedBwForVm(vm1));

			Assert.IsFalse(bwProvisioner.isSuitableForVm(vm2, BW));
			Assert.IsFalse(bwProvisioner.allocateBwForVm(vm2, BW));
			Assert.AreEqual(0, bwProvisioner.getAllocatedBwForVm(vm2));

			Assert.IsTrue(bwProvisioner.isSuitableForVm(vm2, BW / 4));
			Assert.IsTrue(bwProvisioner.allocateBwForVm(vm2, BW / 4));
			Assert.AreEqual(BW / 4, bwProvisioner.getAllocatedBwForVm(vm2));

			Assert.IsTrue(bwProvisioner.isSuitableForVm(vm2, BW / 2));
			Assert.IsTrue(bwProvisioner.allocateBwForVm(vm2, BW / 2));
			Assert.AreEqual(BW / 2, bwProvisioner.getAllocatedBwForVm(vm2));
		}

        [TestMethod]
        public virtual void testDeallocateBwForVm()
		{
			Vm vm1 = new Vm(0, 0, 0, 0, 0, BW / 2, 0, "", null);
			Vm vm2 = new Vm(1, 0, 0, 0, 0, BW / 2, 0, "", null);

			Assert.AreEqual(0, vm1.CurrentAllocatedBw);
			Assert.AreEqual(0, vm2.CurrentAllocatedBw);

			Assert.IsTrue(bwProvisioner.isSuitableForVm(vm1, BW / 2));
			Assert.IsTrue(bwProvisioner.allocateBwForVm(vm1, BW / 2));
			Assert.AreEqual(BW / 2, bwProvisioner.AvailableBw);

			bwProvisioner.deallocateBwForVm(vm1);
			Assert.AreEqual(BW, bwProvisioner.AvailableBw);

			Assert.IsTrue(bwProvisioner.isSuitableForVm(vm1, BW / 2));
			Assert.IsTrue(bwProvisioner.allocateBwForVm(vm1, BW / 2));
			Assert.IsTrue(bwProvisioner.isSuitableForVm(vm2, BW / 2));
			Assert.IsTrue(bwProvisioner.allocateBwForVm(vm2, BW / 2));
			Assert.AreEqual(0, bwProvisioner.AvailableBw);

			bwProvisioner.deallocateBwForVm(vm1);
			bwProvisioner.deallocateBwForVm(vm2);
			Assert.AreEqual(BW, bwProvisioner.AvailableBw);
			Assert.AreEqual(0, vm1.CurrentAllocatedBw);
			Assert.AreEqual(0, vm2.CurrentAllocatedBw);
		}
	}
}