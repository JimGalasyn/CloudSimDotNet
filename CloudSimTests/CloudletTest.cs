using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkiimport static org.junit.Assert.Assert.AreEqual;
(c) 2009-2010, The University of Melbourne, Australia
 fubar */

namespace org.cloudbus.cloudsim
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// @author		Anton Beloglazov
    /// @since		CloudSim Toolkit 2.0
    /// </summary>
    /// <remarks>Ported to C# and .NET Core by Jim Galasyn.
    /// </remarks>
    [TestClass]
    public class CloudletTest
	{
		private const long CLOUDLET_LENGTH = 1000;
		private const long CLOUDLET_FILE_SIZE = 300;
		private const long CLOUDLET_OUTPUT_SIZE = 300;

		private const int PES_NUMBER = 2;

		private Cloudlet cloudlet;
		private UtilizationModel utilizationModelCpu;
		private UtilizationModel utilizationModelRam;
		private UtilizationModel utilizationModelBw;

		//public virtual void setUp()
		//{
		//	utilizationModelCpu = new UtilizationModelStochastic();
		//	utilizationModelRam = new UtilizationModelStochastic();
		//	utilizationModelBw = new UtilizationModelStochastic();
		//	cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModelCpu, utilizationModelRam, utilizationModelBw);
		//}

        [TestInitialize()]
        public void Initialize()
        {
            utilizationModelCpu = new UtilizationModelStochastic();
            utilizationModelRam = new UtilizationModelStochastic();
            utilizationModelBw = new UtilizationModelStochastic();
            cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModelCpu, utilizationModelRam, utilizationModelBw);
        }

        [TestMethod]
        public virtual void testCloudlet()
		{
			Assert.AreEqual(CLOUDLET_LENGTH, cloudlet.CloudletLength);
			Assert.AreEqual(CLOUDLET_LENGTH * PES_NUMBER, cloudlet.CloudletTotalLength);
			Assert.AreEqual(CLOUDLET_FILE_SIZE, cloudlet.CloudletFileSize);
			Assert.AreEqual(CLOUDLET_OUTPUT_SIZE, cloudlet.CloudletOutputSize);
			Assert.AreEqual(PES_NUMBER, cloudlet.NumberOfPes);
			Assert.AreSame(utilizationModelCpu, cloudlet.UtilizationModelCpu);
			Assert.AreSame(utilizationModelRam, cloudlet.UtilizationModelRam);
			Assert.AreSame(utilizationModelBw, cloudlet.UtilizationModelBw);
		}

        [TestMethod]
        public virtual void testGetUtilizationOfCpu()
		{
			Assert.AreEqual(utilizationModelCpu.getUtilization(0), cloudlet.getUtilizationOfCpu(0));
		}

        [TestMethod]
        public virtual void testGetUtilizationOfRam()
		{
			Assert.AreEqual(utilizationModelRam.getUtilization(0), cloudlet.getUtilizationOfRam(0));
		}

        [TestMethod]
        public virtual void testGetUtilizationOfBw()
		{
			Assert.AreEqual(utilizationModelBw.getUtilization(0), cloudlet.getUtilizationOfBw(0));
		}

        [TestMethod]
        public virtual void testCloudletAlternativeConstructor1()
		{
			cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModelCpu, utilizationModelRam, utilizationModelBw, true, new List<string>());
			testCloudlet();
			testGetUtilizationOfCpu();
			testGetUtilizationOfRam();
			testGetUtilizationOfBw();
		}

        [TestMethod]
        public virtual void testCloudletAlternativeConstructor2()
		{
			cloudlet = new Cloudlet(0, CLOUDLET_LENGTH, PES_NUMBER, CLOUDLET_FILE_SIZE, CLOUDLET_OUTPUT_SIZE, utilizationModelCpu, utilizationModelRam, utilizationModelBw, new List<string>());
			testCloudlet();
			testGetUtilizationOfCpu();
			testGetUtilizationOfRam();
			testGetUtilizationOfBw();
		}
	}
}