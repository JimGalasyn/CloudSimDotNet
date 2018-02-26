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

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class HostTest
	{

		private const int ID = 0;
		private static readonly long STORAGE = Consts.MILLION;
		private const int RAM = 1024;
		private const int BW = 10000;
		private const double MIPS = 1000;

		//private static final int PES_NUMBER = 2;

		//private static final double CLOUDLET_LENGTH = 1000;
		//private static final long CLOUDLET_FILE_SIZE = 300;
		//private static final long CLOUDLET_OUTPUT_SIZE = 300;

		private Host host;
		private IList<Pe> peList;

        [TestInitialize()]
        public void Initialize()
        {
            peList = new List<Pe>();
            peList.Add(new Pe(0, new PeProvisionerSimple(MIPS)));
            peList.Add(new Pe(1, new PeProvisionerSimple(MIPS)));

            host = new Host(
                ID, 
                new RamProvisionerSimple(RAM), 
                new BwProvisionerSimple(BW), 
                STORAGE, 
                peList, 
                new VmSchedulerTimeShared(peList)
           );
        }

        [TestMethod]
        public virtual void testIsSuitableForVm()
		{
			Vm vm0 = new Vm(0, 0, MIPS, 2, RAM, BW, 0, "", new CloudletSchedulerDynamicWorkload(MIPS, 2));
			Vm vm1 = new Vm(1, 0, MIPS * 2, 1, RAM * 2, BW * 2, 0, "", new CloudletSchedulerDynamicWorkload(MIPS * 2, 2));

			Assert.IsTrue(host.isSuitableForVm(vm0));
			Assert.IsFalse(host.isSuitableForVm(vm1));
		}

        [TestMethod]
        public virtual void testVmCreate()
		{
			Vm vm0 = new Vm(0, 0, MIPS / 2, 1, RAM / 2, BW / 2, 0, "", new CloudletSchedulerDynamicWorkload(MIPS / 2, 1));
			Vm vm1 = new Vm(1, 0, MIPS, 1, RAM, BW, 0, "", new CloudletSchedulerDynamicWorkload(MIPS, 1));
			Vm vm2 = new Vm(2, 0, MIPS * 2, 1, RAM, BW, 0, "", new CloudletSchedulerDynamicWorkload(MIPS * 2, 1));
			Vm vm3 = new Vm(3, 0, MIPS / 2, 2, RAM / 2, BW / 2, 0, "", new CloudletSchedulerDynamicWorkload(MIPS / 2, 2));

			Assert.IsTrue(host.vmCreate(vm0));
			Assert.IsFalse(host.vmCreate(vm1));
			Assert.IsFalse(host.vmCreate(vm2));
			Assert.IsTrue(host.vmCreate(vm3));
		}

        [TestMethod]
        public virtual void testVmDestroy()
		{
			Vm vm = new Vm(0, 0, MIPS, 1, RAM / 2, BW / 2, 0, "", new CloudletSchedulerDynamicWorkload(MIPS, 1));

			Assert.IsTrue(host.vmCreate(vm));
			Assert.AreSame(vm, host.getVm(0, 0));
			Assert.AreEqual(MIPS, host.VmScheduler.AvailableMips);

			host.vmDestroy(vm);
			Assert.IsNull(host.getVm(0, 0));
			Assert.AreEqual(0, host.VmListProperty.Count);
			Assert.AreEqual(MIPS * 2, host.VmScheduler.AvailableMips);
		}

        [TestMethod]
        public virtual void testVmDestroyAll()
		{
			Vm vm0 = new Vm(0, 0, MIPS, 1, RAM / 2, BW / 2, 0, "", new CloudletSchedulerDynamicWorkload(MIPS, 1));
			Vm vm1 = new Vm(1, 0, MIPS, 1, RAM / 2, BW / 2, 0, "", new CloudletSchedulerDynamicWorkload(MIPS, 1));

			Assert.IsTrue(host.vmCreate(vm0));
			Assert.AreSame(vm0, host.getVm(0, 0));
			Assert.AreEqual(MIPS, host.VmScheduler.AvailableMips);

			Assert.IsTrue(host.vmCreate(vm1));
			Assert.AreSame(vm1, host.getVm(1, 0));
			Assert.AreEqual(0, host.VmScheduler.AvailableMips);

			host.vmDestroyAll();
			Assert.IsNull(host.getVm(0, 0));
            Assert.IsNull(host.getVm(1, 0));
			Assert.AreEqual(0, host.VmListProperty.Count);
			Assert.AreEqual(MIPS * 2, host.VmScheduler.AvailableMips);
		}

        [TestMethod]
        public virtual void testUpdateVmsProcessing()
		{
	//		UtilizationModelStochastic utilizationModel1 = new UtilizationModelStochastic();
	//		UtilizationModelStochastic utilizationModel2 = new UtilizationModelStochastic();
	//
	//		VMGridlet gridlet1 = new VMGridlet(0, 0, GRIDLET_LENGTH, GRIDLET_FILE_SIZE, GRIDLET_OUTPUT_SIZE, PES_NUMBER, utilizationModel1, utilizationModel1, utilizationModel1);
	//		VMGridlet gridlet2 = new VMGridlet(0, 0, GRIDLET_LENGTH, GRIDLET_FILE_SIZE, GRIDLET_OUTPUT_SIZE, PES_NUMBER, utilizationModel2, utilizationModel2, utilizationModel2);
	//
	//		gridlet1.setResourceParameter(0, 0, 0);
	//		gridlet2.setResourceParameter(0, 0, 0);
	//
	//		int[] mipsShare = { (int) (GRIDLET_LENGTH / 2) };
	//		vmScheduler.setPEMips(mipsShare[0]);
	//
	//		double utilization1 = utilizationModel1.getUtilization(0);
	//		double utilization2 = utilizationModel2.getUtilization(0);
	//
	//		vmScheduler.gridletSubmit(gridlet1);
	//		vmScheduler.gridletSubmit(gridlet2);
	//
	//		double actualCompletionTime = vmScheduler.updateVMProcessing(0, mipsShare);
	//
	//		double completionTime1 = GRIDLET_LENGTH / (utilization1 * GRIDLET_LENGTH / 2);
	//		double completionTime2 = GRIDLET_LENGTH / (utilization2 * GRIDLET_LENGTH / 2);
	//
	//		double expectedCompletiontime;
	//		if (completionTime1 < completionTime2) {
	//			expectedCompletiontime = completionTime1;
	//		} else {
	//			expectedCompletiontime = completionTime2;
	//		}
	//
	//		Assert.AreEqual(expectedCompletiontime, actualCompletionTime, 0);
	//
	//		actualCompletionTime = vmScheduler.updateVMProcessing(1, mipsShare);
	//
	//		completionTime1 = 1 + (GRIDLET_LENGTH - utilization1 * GRIDLET_LENGTH / 2 * 1) / (utilizationModel1.getUtilization(1) * GRIDLET_LENGTH / 2);
	//		completionTime2 = 1 + (GRIDLET_LENGTH - utilization2 * GRIDLET_LENGTH / 2 * 1) / (utilizationModel2.getUtilization(1) * GRIDLET_LENGTH / 2);
	//
	//		if (completionTime1 < completionTime2) {
	//			expectedCompletiontime = completionTime1;
	//		} else {
	//			expectedCompletiontime = completionTime2;
	//		}
	//
	//		Assert.AreEqual(expectedCompletiontime, actualCompletionTime, 0);
	//
	//		Assert.IsFalse(vmScheduler.isFinishedGridlets());
	//
	//		Assert.AreEqual(0, vmScheduler.updateVMProcessing(GRIDLET_LENGTH, mipsShare), 0);
	//
	//		Assert.IsTrue(vmScheduler.isFinishedGridlets());

		}

	//	@Test
	//	public void testUpdateVmsProcessing() {
	//		UtilizationModelStochastic utilizationModel1 = new UtilizationModelStochastic();
	//		UtilizationModelStochastic utilizationModel2 = new UtilizationModelStochastic();
	//
	//		Cloudlet cloudlet1 = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE,
	//				utilizationModel1, utilizationModel1, utilizationModel1);
	//
	//		Cloudlet cloudlet2 = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE,
	//				utilizationModel2, utilizationModel2, utilizationModel2);
	//
	//		cloudlet1.setResourceParameter(0, 0, 0);
	//		cloudlet2.setResourceParameter(0, 0, 0);
	//
	//		CloudletSchedulerSingleService vmScheduler = new CloudletSchedulerSingleService(PES_NUMBER, MIPS);
	//
	//		int[] mipsShare = { (int) (CLOUDLET_LENGTH / 2) };
	//		vmScheduler.setCurrentMipsShare(mipsShare);
	//		//vmScheduler.setMips(mipsShare[0]);
	//
	//		double utilization1 = utilizationModel1.getUtilization(0);
	//		double utilization2 = utilizationModel2.getUtilization(0);
	//
	//		vmScheduler.cloudletSubmit(cloudlet1);
	//		vmScheduler.cloudletSubmit(cloudlet2);
	//
	//		double actualCompletionTime = vmScheduler.updateVmProcessing(0, mipsShare);
	//
	//		double completionTime1 = CLOUDLET_LENGTH / (utilization1 * CLOUDLET_LENGTH / 2);
	//		double completionTime2 = CLOUDLET_LENGTH / (utilization2 * CLOUDLET_LENGTH / 2);
	//
	//		double expectedCompletiontime;
	//		if (completionTime1 < completionTime2) {
	//			expectedCompletiontime = completionTime1;
	//		} else {
	//			expectedCompletiontime = completionTime2;
	//		}
	//
	//		Assert.AreEqual(expectedCompletiontime, actualCompletionTime, 0);
	//
	//		actualCompletionTime = vmScheduler.updateVmProcessing(1, mipsShare);
	//
	//		completionTime1 = 1 + (CLOUDLET_LENGTH - utilization1 * CLOUDLET_LENGTH / 2 * 1) / (utilizationModel1.getUtilization(1) * CLOUDLET_LENGTH / 2);
	//		completionTime2 = 1 + (CLOUDLET_LENGTH - utilization2 * CLOUDLET_LENGTH / 2 * 1) / (utilizationModel2.getUtilization(1) * CLOUDLET_LENGTH / 2);
	//
	//		if (completionTime1 < completionTime2) {
	//			expectedCompletiontime = completionTime1;
	//		} else {
	//			expectedCompletiontime = completionTime2;
	//		}
	//
	//		Assert.AreEqual(expectedCompletiontime, actualCompletionTime, 0);
	//
	//		Assert.IsFalse(vmScheduler.isFinishedCloudlets());
	//
	//		Assert.AreEqual(0, vmScheduler.updateVmProcessing(CLOUDLET_LENGTH, mipsShare), 0);
	//
	//		Assert.IsTrue(vmScheduler.isFinishedCloudlets());
	//
	//	}
	}
}