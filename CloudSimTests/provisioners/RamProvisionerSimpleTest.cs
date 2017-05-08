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
    public class RamProvisionerSimpleTest
	{

		private const int RAM = 1000;

		private RamProvisionerSimple ramProvisioner;

        //public virtual void setUp()
        //{
        //	ramProvisioner = new RamProvisionerSimple(RAM);
        //}

        [TestInitialize()]
        public void Initialize()
        {
            ramProvisioner = new RamProvisionerSimple(RAM);
        }

        [TestMethod]
        public virtual void testGetRam()
		{
			Assert.AreEqual(RAM, ramProvisioner.Ram);
		}

        [TestMethod]
        public virtual void testGetAvailableRam()
		{
			Assert.AreEqual(RAM, ramProvisioner.AvailableRam);
		}

        [TestMethod]
        public virtual void testAllocateRamForVm()
		{
			Vm vm1 = new Vm(0, 0, 0, 0, RAM / 2, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, 0, 0, RAM, 0, 0, "", null);

			Assert.IsTrue(ramProvisioner.isSuitableForVm(vm1, RAM / 2));
			Assert.IsTrue(ramProvisioner.allocateRamForVm(vm1, RAM / 2));
			Assert.AreEqual(RAM / 2, ramProvisioner.AvailableRam);

			Assert.IsFalse(ramProvisioner.isSuitableForVm(vm2, RAM));
			Assert.IsFalse(ramProvisioner.allocateRamForVm(vm2, RAM));
			Assert.AreEqual(RAM / 2, ramProvisioner.AvailableRam);

			Assert.IsTrue(ramProvisioner.isSuitableForVm(vm2, RAM / 4));
			Assert.IsTrue(ramProvisioner.allocateRamForVm(vm2, RAM / 4));
			Assert.AreEqual(RAM * 1 / 4, ramProvisioner.AvailableRam);

			Assert.IsTrue(ramProvisioner.isSuitableForVm(vm2, RAM / 2));
			Assert.IsTrue(ramProvisioner.allocateRamForVm(vm2, RAM / 2));
			Assert.AreEqual(0, ramProvisioner.AvailableRam);
		}

        [TestMethod]
        public virtual void testGetAllocatedRamForVm()
		{
			Vm vm1 = new Vm(0, 0, 0, 0, RAM / 2, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, 0, 0, RAM, 0, 0, "", null);

			Assert.IsTrue(ramProvisioner.isSuitableForVm(vm1, RAM / 2));
			Assert.IsTrue(ramProvisioner.allocateRamForVm(vm1, RAM / 2));
			Assert.AreEqual(RAM / 2, ramProvisioner.getAllocatedRamForVm(vm1));

			Assert.IsFalse(ramProvisioner.isSuitableForVm(vm2, RAM));
			Assert.IsFalse(ramProvisioner.allocateRamForVm(vm2, RAM));
			Assert.AreEqual(0, ramProvisioner.getAllocatedRamForVm(vm2));

			Assert.IsTrue(ramProvisioner.isSuitableForVm(vm2, RAM / 4));
			Assert.IsTrue(ramProvisioner.allocateRamForVm(vm2, RAM / 4));
			Assert.AreEqual(RAM / 4, ramProvisioner.getAllocatedRamForVm(vm2));

			Assert.IsTrue(ramProvisioner.isSuitableForVm(vm2, RAM / 2));
			Assert.IsTrue(ramProvisioner.allocateRamForVm(vm2, RAM / 2));
			Assert.AreEqual(RAM / 2, ramProvisioner.getAllocatedRamForVm(vm2));
		}

        [TestMethod]
        public virtual void testDeallocateBwForVm()
		{
			Vm vm1 = new Vm(0, 0, 0, 0, RAM / 2, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, 0, 0, RAM / 2, 0, 0, "", null);

			Assert.AreEqual(0, vm1.CurrentAllocatedRam);
			Assert.AreEqual(0, vm2.CurrentAllocatedRam);

			Assert.IsTrue(ramProvisioner.isSuitableForVm(vm1, RAM / 2));
			Assert.IsTrue(ramProvisioner.allocateRamForVm(vm1, RAM / 2));
			Assert.AreEqual(RAM / 2, ramProvisioner.AvailableRam);

			ramProvisioner.deallocateRamForVm(vm1);
			Assert.AreEqual(RAM, ramProvisioner.AvailableRam);

			Assert.IsTrue(ramProvisioner.isSuitableForVm(vm1, RAM / 2));
			Assert.IsTrue(ramProvisioner.allocateRamForVm(vm1, RAM / 2));
			Assert.IsTrue(ramProvisioner.isSuitableForVm(vm2, RAM / 2));
			Assert.IsTrue(ramProvisioner.allocateRamForVm(vm2, RAM / 2));
			Assert.AreEqual(0, ramProvisioner.AvailableRam);

			ramProvisioner.deallocateRamForVm(vm1);
			ramProvisioner.deallocateRamForVm(vm2);
			Assert.AreEqual(RAM, ramProvisioner.AvailableRam);
			Assert.AreEqual(0, vm1.CurrentAllocatedRam);
			Assert.AreEqual(0, vm2.CurrentAllocatedRam);
		}
	}
}