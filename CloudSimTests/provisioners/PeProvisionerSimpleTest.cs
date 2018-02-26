using System.Collections.Generic;

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
    using System.Linq;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class PeProvisionerSimpleTest
	{
		private const double MIPS = 1000;

		private PeProvisionerSimple peProvisioner;
        
        [TestInitialize()]
        public void Initialize()
        {
            peProvisioner = new PeProvisionerSimple(MIPS);
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
        }

        [TestMethod]
        public virtual void testGetMips()
		{
			Assert.AreEqual(MIPS, peProvisioner.Mips);
		}

        [TestMethod]
        public virtual void testGetAvailableMips()
		{
			Assert.AreEqual(MIPS, peProvisioner.AvailableMips);
		}

        [TestMethod]
        public virtual void testGetTotalAllocatedMips()
		{
			Assert.AreEqual(0, peProvisioner.TotalAllocatedMips);
		}

        [TestMethod]
        public virtual void testGetUtilization()
		{
			Assert.AreEqual(0, peProvisioner.Utilization);
		}

        [TestMethod]
        public virtual void testAllocateMipsForVm()
		{
			Vm vm1 = new Vm(0, 0, MIPS / 2, 1, 0, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, MIPS / 2, 1, 0, 0, 0, "", null);
			Vm vm3 = new Vm(2, 0, MIPS / 2, 2, 0, 0, 0, "", null);

			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm1, MIPS / 2));
			Assert.AreEqual(MIPS / 2, peProvisioner.AvailableMips);
			Assert.AreEqual(MIPS / 2, peProvisioner.TotalAllocatedMips);
			Assert.AreEqual(0.5, peProvisioner.Utilization);

			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm2, MIPS / 4));
			Assert.AreEqual(MIPS / 4, peProvisioner.AvailableMips);
			Assert.AreEqual(MIPS * 3 / 4, peProvisioner.TotalAllocatedMips);
			Assert.AreEqual(0.75, peProvisioner.Utilization);

			Assert.IsFalse(peProvisioner.allocateMipsForVm(vm3, MIPS / 2));
			Assert.AreEqual(MIPS / 4, peProvisioner.AvailableMips);
			Assert.AreEqual(MIPS * 3 / 4, peProvisioner.TotalAllocatedMips);
			Assert.AreEqual(0.75, peProvisioner.Utilization);

			peProvisioner.deallocateMipsForVm(vm1);
			peProvisioner.deallocateMipsForVm(vm2);

			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm3, MIPS / 4));
			Assert.AreEqual(MIPS * 3 / 4, peProvisioner.AvailableMips);
			Assert.AreEqual(MIPS / 4, peProvisioner.TotalAllocatedMips);
			Assert.AreEqual(0.25, peProvisioner.Utilization);

			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm3, MIPS / 4));
			Assert.AreEqual(MIPS / 2, peProvisioner.AvailableMips);
			Assert.AreEqual(MIPS / 2, peProvisioner.TotalAllocatedMips);
			Assert.AreEqual(0.5, peProvisioner.Utilization);

			List<double?> mipsArray = new List<double?>();
			mipsArray.Add(MIPS / 2.0);
			mipsArray.Add(MIPS / 2.0);

			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm3, mipsArray));
			Assert.AreEqual(0, peProvisioner.AvailableMips);
			Assert.AreEqual(MIPS, peProvisioner.TotalAllocatedMips);
			Assert.AreEqual(1, peProvisioner.Utilization);
		}

        /// <summary>
        /// Tests getAllocatedMipsForVm
        /// </summary>
        /// <remarks> TODO: LINQ SequenceEqual not always same as Java AbstractList.equals
        /// </remarks>
        [TestMethod]
        public virtual void testGetAllocatedMipsForVm()
		{
			Vm vm1 = new Vm(0, 0, MIPS / 2, 1, 0, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, MIPS / 2, 1, 0, 0, 0, "", null);
			Vm vm3 = new Vm(2, 0, MIPS / 2, 2, 0, 0, 0, "", null);

			Assert.IsNull(peProvisioner.getAllocatedMipsForVm(vm1));
			Assert.AreEqual(0, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm1, 0));

			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm1, MIPS / 2));
			List<double?> allocatedMips1 = new List<double?>();
			allocatedMips1.Add(MIPS / 2);
			Assert.IsTrue(allocatedMips1.SequenceEqual(peProvisioner.getAllocatedMipsForVm(vm1)));
			Assert.AreEqual(MIPS / 2, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm1, 0));
			Assert.AreEqual(0, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm1, 1));
			Assert.AreEqual(MIPS / 2, peProvisioner.getTotalAllocatedMipsForVm(vm1));

			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm2, MIPS / 4));
			List<double?> allocatedMips2 = new List<double?>();
			allocatedMips2.Add(MIPS / 4);
			Assert.IsTrue(allocatedMips2.SequenceEqual(peProvisioner.getAllocatedMipsForVm(vm2)));
			Assert.AreEqual(MIPS / 4, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm2, 0));
			Assert.AreEqual(MIPS / 4, peProvisioner.getTotalAllocatedMipsForVm(vm2));

			peProvisioner.deallocateMipsForVm(vm1);
			peProvisioner.deallocateMipsForVm(vm2);

			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm3, MIPS / 4));
			List<double?> allocatedMips3 = new List<double?>();
			allocatedMips3.Add(MIPS / 4);
			Assert.IsTrue(allocatedMips3.SequenceEqual(peProvisioner.getAllocatedMipsForVm(vm3)));
			Assert.AreEqual(MIPS / 4, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm3, 0));
			Assert.AreEqual(MIPS / 4, peProvisioner.getTotalAllocatedMipsForVm(vm3));

			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm3, MIPS / 4));
			allocatedMips3.Add(MIPS / 4);
			Assert.IsTrue(allocatedMips3.SequenceEqual(peProvisioner.getAllocatedMipsForVm(vm3)));
			Assert.AreEqual(MIPS / 4, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm3, 0));
			Assert.AreEqual(MIPS / 4, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm3, 1));
			Assert.AreEqual(MIPS / 2, peProvisioner.getTotalAllocatedMipsForVm(vm3));

			List<double?> allocatedMips4 = new List<double?>();
			allocatedMips4.Add(MIPS / 2.0);
			allocatedMips4.Add(MIPS);
			Assert.IsFalse(peProvisioner.allocateMipsForVm(vm3, allocatedMips4));

			List<double?> allocatedMips5 = new List<double?>();
			allocatedMips5.Add(MIPS / 2.0);
			allocatedMips5.Add(MIPS / 2.0);
			Assert.IsTrue(peProvisioner.allocateMipsForVm(vm3, allocatedMips5));
			Assert.IsTrue(allocatedMips5.SequenceEqual(peProvisioner.getAllocatedMipsForVm(vm3)));
			Assert.AreEqual(MIPS / 2, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm3, 0));
			Assert.AreEqual(MIPS / 2, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm3, 1));
			Assert.AreEqual(MIPS, peProvisioner.getTotalAllocatedMipsForVm(vm3));

			peProvisioner.deallocateMipsForVm(vm1);
			peProvisioner.deallocateMipsForVm(vm2);
			peProvisioner.deallocateMipsForVm(vm3);

			Assert.IsNull(peProvisioner.getAllocatedMipsForVm(vm1));
			Assert.IsNull(peProvisioner.getAllocatedMipsForVm(vm2));
			Assert.IsNull(peProvisioner.getAllocatedMipsForVm(vm3));

			Assert.AreEqual(0, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm1, 0));
			Assert.AreEqual(0, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm2, 0));
			Assert.AreEqual(0, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm3, 0));
			Assert.AreEqual(0, peProvisioner.getAllocatedMipsForVmByVirtualPeId(vm3, 1));

			Assert.AreEqual(0, peProvisioner.getTotalAllocatedMipsForVm(vm1));
			Assert.AreEqual(0, peProvisioner.getTotalAllocatedMipsForVm(vm2));
			Assert.AreEqual(0, peProvisioner.getTotalAllocatedMipsForVm(vm3));

			Assert.AreEqual(MIPS, peProvisioner.AvailableMips);
		}

        [TestMethod]
        public virtual void testDeallocateMipsForVM()
		{
			Vm vm1 = new Vm(0, 0, MIPS / 2, 1, 0, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, MIPS / 2, 1, 0, 0, 0, "", null);

			peProvisioner.allocateMipsForVm(vm1, MIPS / 2);
			peProvisioner.allocateMipsForVm(vm2, MIPS / 4);

			Assert.AreEqual(MIPS / 4, peProvisioner.AvailableMips);

			peProvisioner.deallocateMipsForVm(vm1);

			Assert.AreEqual(MIPS * 3 / 4, peProvisioner.AvailableMips);

			peProvisioner.deallocateMipsForVm(vm2);

			Assert.AreEqual(MIPS, peProvisioner.AvailableMips);
		}
	}
}