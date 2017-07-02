using System.Collections.Generic;

namespace org.cloudbus.cloudsim.util
{

    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.Equals;
    //JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
    //	import static org.junit.Assert.Assert.IsTrue;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;

    [TestClass]
    public class WorkloadFileReaderTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void setUpBeforeClass() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static void setUpBeforeClass()
		{
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void tearDownAfterClass() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public static void tearDownAfterClass()
		{
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void setUp()
		{
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void tearDown()
		{
		}

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Test public void read() throws java.io.FileNotFoundException
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        [TestMethod]
        public virtual void read()
        {

            //WorkloadModel r = new WorkloadFileReader("src" + File.separator + "test" + File.separator + "LCG.swf.gz", 1);
            //WorkloadModel r = new WorkloadFileReader("src" + Path.DirectorySeparatorChar + "test" + Path.DirectorySeparatorChar + "LCG.swf.gz", 1);
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