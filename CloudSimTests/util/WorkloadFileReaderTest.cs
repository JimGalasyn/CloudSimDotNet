using System.Collections.Generic;

namespace org.cloudbus.cloudsim.util
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;

    [TestClass]
    public class WorkloadFileReaderTest
	{

		public static void setUpBeforeClass()
		{
		}

		public static void tearDownAfterClass()
		{
		}

		public virtual void setUp()
		{
		}

		public virtual void tearDown()
		{
		}

        [TestMethod]
        public virtual void WorkloadFileReaderRead()
        {
            WorkloadModel r = new WorkloadFileReader(@".\resources\LCG.swf", 1);
            IList<Cloudlet> cloudletlist = r.generateWorkload();
            Assert.AreEqual(188041, cloudletlist.Count);

            foreach (Cloudlet cloudlet in cloudletlist)
            {
                Assert.IsTrue(cloudlet.CloudletLength > 0);
            }
        }
	}
}