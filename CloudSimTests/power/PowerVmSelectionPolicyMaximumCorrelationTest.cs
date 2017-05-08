using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.power
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
 
    [TestClass]
    public class PowerVmSelectionPolicyMaximumCorrelationTest
	{
		public static readonly double[][] DATA = new double[][]
		{
			new double[] {1, 2, 2, 4, 3, 6},
			new double[] {14, 23, 30, 50, 39, 67},
			new double[] {4, 4, 7, 7, 10, 10}
		};

		public static readonly double[] CORRELATION = new double[] {0.9834528493463638, 0.986553560148001, 0.732289527720739};

		private PowerVmSelectionPolicyMaximumCorrelation vmSelectionPolicyMaximumCorrelation;

		//public virtual void setUp()
		//{
		//	vmSelectionPolicyMaximumCorrelation = new PowerVmSelectionPolicyMaximumCorrelation(new PowerVmSelectionPolicyRandomSelection());
		//}

        [TestInitialize()]
        public void Initialize()
        {
            vmSelectionPolicyMaximumCorrelation = new PowerVmSelectionPolicyMaximumCorrelation(new PowerVmSelectionPolicyRandomSelection());
        }

        [TestMethod]
        public virtual void testGetPowerModel()
		{
            // TODO: TEST FAIL: getCorrelationCoefficients(DATA)
            IList<double?> result = vmSelectionPolicyMaximumCorrelation.getCorrelationCoefficients(DATA);
			for (int i = 0; i < result.Count; i++)
			{
                //Assert.Equals(CORRELATION[i], result[i], 0.00001);
                Assert.IsTrue(Math.Abs(CORRELATION[i] - result[i].Value) <= 0.00001);
			}
		}
	}
}