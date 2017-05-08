using System.Collections.Generic;

/*
 * Title: CloudSim Toolkit Description: CloudSim (Cloud Simulation) Toolkit for Modeling and
 * Simulation of Clouds Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * 
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;

    /// <summary>
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class VmTest
	{
		private const int ID = 1;

		private const int USER_ID = 1;

		private const double MIPS = 1000;

		private const int PES_NUMBER = 2;

		private const int RAM = 1024;

		private const int BW = 10000;

		private const long SIZE = 1000;

		private const string VMM = "Xen";

		private CloudletSchedulerDynamicWorkload vmScheduler;

		private Vm vm;

		//public virtual void setUp()
		//{
		//	vmScheduler = new CloudletSchedulerDynamicWorkload(MIPS, PES_NUMBER);
		//	vm = new Vm(ID, USER_ID, MIPS, PES_NUMBER, RAM, BW, SIZE, VMM, vmScheduler);
		//}

        [TestInitialize()]
        public void Initialize()
        {
            vmScheduler = new CloudletSchedulerDynamicWorkload(MIPS, PES_NUMBER);
            vm = new Vm(ID, USER_ID, MIPS, PES_NUMBER, RAM, BW, SIZE, VMM, vmScheduler);
        }

        [TestMethod]
        public virtual void testGetMips()
		{
			Assert.AreEqual(MIPS, vm.Mips);
		}

        [TestMethod]
        public virtual void testSetMips()
		{
			vm.Mips = MIPS / 2;
			Assert.AreEqual(MIPS / 2, vm.Mips);
		}

        [TestMethod]
        public virtual void testGetNumberOfPes()
		{
			Assert.AreEqual(PES_NUMBER, vm.NumberOfPes);
		}

        [TestMethod]
        public virtual void testGetRam()
		{
			Assert.AreEqual(RAM, vm.Ram);
		}

        [TestMethod]
        public virtual void testGetBw()
		{
			Assert.AreEqual(BW, vm.Bw);
		}

        [TestMethod]
        public virtual void testGetSize()
		{
			Assert.AreEqual(SIZE, vm.Size);
		}

        [TestMethod]
        public virtual void testGetVmm()
		{
			Assert.AreEqual(VMM, vm.Vmm);
		}

        [TestMethod]
        public virtual void testGetHost()
		{
			Assert.AreEqual(null, vm.Host);
			Host host = new Host(0, null, null, 0, new List<Pe>(), null);
			vm.Host = host;
			Assert.AreEqual(host, vm.Host);
		}

        [TestMethod]
        public virtual void testIsInMigration()
		{
			Assert.IsFalse(vm.InMigration);
			vm.InMigration = true;
			Assert.IsTrue(vm.InMigration);
		}

        [TestMethod]
        public virtual void testGetTotalUtilization()
		{
			Assert.AreEqual(0, vm.getTotalUtilizationOfCpu(0));
		}

        [TestMethod]
        public virtual void testGetTotalUtilizationMips()
		{
			Assert.AreEqual(0, vm.getTotalUtilizationOfCpuMips(0));
		}

        [TestMethod]
        public virtual void testGetUid()
		{
			Assert.AreEqual(USER_ID + "-" + ID, vm.Uid);
		}

        [TestMethod]
        public virtual void testUpdateVmProcessing()
		{
			Assert.AreEqual(0, vm.updateVmProcessing(0, null));
			List<double?> mipsShare1 = new List<double?>();
			mipsShare1.Add(1.0);
			List<double?> mipsShare2 = new List<double?>();
			mipsShare2.Add(1.0);
			Assert.AreEqual(vmScheduler.updateVmProcessing(0, mipsShare1), vm.updateVmProcessing(0, mipsShare2));
		}

        [TestMethod]
        public virtual void testGetCurrentAllocatedSize()
		{
			Assert.AreEqual(0, vm.CurrentAllocatedSize);
			vm.CurrentAllocatedSize = SIZE;
			Assert.AreEqual(SIZE, vm.CurrentAllocatedSize);
		}

        [TestMethod]
        public virtual void testGetCurrentAllocatedRam()
		{
			Assert.AreEqual(0, vm.CurrentAllocatedRam);
			vm.CurrentAllocatedRam = RAM;
			Assert.AreEqual(RAM, vm.CurrentAllocatedRam);
		}

        [TestMethod]
        public virtual void testGetCurrentAllocatedBw()
		{
			Assert.AreEqual(0, vm.CurrentAllocatedBw);
			vm.CurrentAllocatedBw = BW;
			Assert.AreEqual(BW, vm.CurrentAllocatedBw);
		}

        [TestMethod]
        public virtual void testGetCurrentAllocatedMips()
		{
            // ArrayList<Integer> currentAllocatedMips = new ArrayList<Integer>();
            // Assert.AreEqual(currentAllocatedMips, vm.getCurrentAllocatedMips());
            // TODO: TEST should CurrentAllocatedMips really be null?
            Assert.IsNull(vm.CurrentAllocatedMips);
		}

        [TestMethod]
        public virtual void testIsBeingInstantiated()
		{
			Assert.IsTrue(vm.BeingInstantiated);
			vm.BeingInstantiated = false;
			Assert.IsFalse(vm.BeingInstantiated);
		}

        [TestMethod]
        public virtual void testGetCurrentRequestedMips()
		{
            CloudletSchedulerTimeShared cloudletScheduler = new CloudletSchedulerTimeShared();

            Vm vm = new Vm(ID, USER_ID, MIPS, PES_NUMBER, RAM, BW, SIZE, VMM, cloudletScheduler);
            // TODO: TEST Setting vm.BeingInstantiated = false seems to be in error.
            //vm.BeingInstantiated = false;

            IList<double?> expectedCurrentMips = new List<double?>();
            // TODO: TEST Seems like it should be: expectedCurrentMips.Add(MIPS), not MIPS / 2.
            //expectedCurrentMips.Add(MIPS / 2);
            //expectedCurrentMips.Add(MIPS / 2);
            expectedCurrentMips.Add(MIPS);
            expectedCurrentMips.Add(MIPS);

            //var currentRequestedMips = cloudletScheduler.CurrentRequestedMips;
            Assert.IsTrue(expectedCurrentMips.SequenceEqual(vm.CurrentRequestedMips));

            //CloudletScheduler cloudletScheduler = createMock(typeof(CloudletScheduler));
            //Vm vm = new Vm(ID, USER_ID, MIPS, PES_NUMBER, RAM, BW, SIZE, VMM, cloudletScheduler);
            //vm.BeingInstantiated = false;

            //IList<double?> expectedCurrentMips = new List<double?>();
            //expectedCurrentMips.Add(MIPS / 2);
            //expectedCurrentMips.Add(MIPS / 2);

            //expect(cloudletScheduler.CurrentRequestedMips).andReturn(expectedCurrentMips);

            //replay(cloudletScheduler);

            //Assert.AreEqual(expectedCurrentMips, vm.CurrentRequestedMips);

            //verify(cloudletScheduler);
        }

        [TestMethod]
        public virtual void testGetCurrentRequestedTotalMips()
		{
            CloudletSchedulerTimeShared cloudletScheduler = new CloudletSchedulerTimeShared();
            Vm vm = new Vm(ID, USER_ID, MIPS, PES_NUMBER, RAM, BW, SIZE, VMM, cloudletScheduler);

            List<double?> currentMips = new List<double?>();
            currentMips.Add(MIPS);
            currentMips.Add(MIPS);

            Assert.AreEqual(MIPS * 2, vm.CurrentRequestedTotalMips);
            
            //CloudletScheduler cloudletScheduler = createMock(typeof(CloudletScheduler));
            //Vm vm = new Vm(ID, USER_ID, MIPS, PES_NUMBER, RAM, BW, SIZE, VMM, cloudletScheduler);

            //List<double?> currentMips = new List<double?>();
            //currentMips.Add(MIPS);
            //currentMips.Add(MIPS);

            //expect(cloudletScheduler.CurrentRequestedMips).andReturn(currentMips);

            //replay(cloudletScheduler);

            //Assert.AreEqual(MIPS * 2, vm.CurrentRequestedTotalMips, 0);

            //verify(cloudletScheduler);
        }
    }
}