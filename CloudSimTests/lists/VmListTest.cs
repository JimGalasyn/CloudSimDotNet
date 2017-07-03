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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class VmListTest
	{
		private IList<Vm> vmList;

        [TestInitialize()]
        public void Initialize()
        {
            vmList = new List<Vm>();
        }

        [TestMethod]
        public virtual void testGetVMbyID()
		{
			Assert.IsNull(VmList.getById(vmList, 0));
			Assert.IsNull(VmList.getById(vmList, 1));
			Assert.IsNull(VmList.getById(vmList, 2));

			Vm vm1 = new Vm(0, 0, 0, 1, 0, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, 0, 1, 0, 0, 0, "", null);
			Vm vm3 = new Vm(2, 0, 0, 2, 0, 0, 0, "", null);

			vmList.Add(vm1);
			vmList.Add(vm2);
			vmList.Add(vm3);

			Assert.AreSame(vm1, VmList.getById(vmList, 0));
			Assert.AreSame(vm2, VmList.getById(vmList, 1));
			Assert.AreSame(vm3, VmList.getById(vmList, 2));
		}

        [TestMethod]
        public virtual void testGetVMByIdAndUserId()
		{
			Assert.IsNull(VmList.getByIdAndUserId(vmList, 0, 0));
			Assert.IsNull(VmList.getByIdAndUserId(vmList, 1, 0));
			Assert.IsNull(VmList.getByIdAndUserId(vmList, 0, 1));
			Assert.IsNull(VmList.getByIdAndUserId(vmList, 1, 1));

			Vm vm1 = new Vm(0, 0, 0, 1, 0, 0, 0, "", null);
			Vm vm2 = new Vm(1, 0, 0, 1, 0, 0, 0, "", null);
			Vm vm3 = new Vm(0, 1, 0, 2, 0, 0, 0, "", null);
			Vm vm4 = new Vm(1, 1, 0, 2, 0, 0, 0, "", null);

			vmList.Add(vm1);
			vmList.Add(vm2);
			vmList.Add(vm3);
			vmList.Add(vm4);

			Assert.AreSame(vm1, VmList.getByIdAndUserId(vmList, 0, 0));
			Assert.AreSame(vm2, VmList.getByIdAndUserId(vmList, 1, 0));
			Assert.AreSame(vm3, VmList.getByIdAndUserId(vmList, 0, 1));
			Assert.AreSame(vm4, VmList.getByIdAndUserId(vmList, 1, 1));
		}
	}
}