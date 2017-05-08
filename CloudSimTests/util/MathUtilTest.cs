using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.util
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
    public class MathUtilTest
	{
		public static readonly double[] DATA1 = new double[] {105, 109, 107, 112, 102, 118, 115, 104, 110, 116, 108};

		public const double IQR1 = 10;

		public const double SUM1 = 1206;

		public static readonly double[] DATA2 = new double[] {2, 4, 7, -20, 22, -1, 0, -1, 7, 15, 8, 4, -4, 11, 11, 12, 3, 12, 18, 1};

		public const double IQR2 = 12;

		public static readonly double[] DATA3 = new double[] {1, 1, 2, 2, 4, 6, 9};

		public const double MAD = 1;

		public static readonly double[] DATA4 = new double[] {1, 1, 2, 2, 4, 6, 9, 0, 10, 0, 0, 0, 0, 0};

		public const int NON_ZERO = 9;

		public static readonly double[] NON_ZERO_TAIL = new double[] {1, 1, 2, 2, 4, 6, 9, 0, 10};

        [TestMethod]
        public virtual void testMad()
		{
            Assert.AreEqual(MAD, MathUtil.mad(DATA3));
        }

        [TestMethod]
        public virtual void testIqr()
		{
            Assert.AreEqual(IQR1, MathUtil.iqr(DATA1));
            Assert.AreEqual(IQR2, MathUtil.iqr(DATA2));
        }

        [TestMethod]
        public virtual void testCountNonZeroBeginning()
		{
            Assert.AreEqual(NON_ZERO, MathUtil.countNonZeroBeginning(DATA4));
        }

        [TestMethod]
        public virtual void testTrimZeroTail()
		{
            Assert.IsTrue(NON_ZERO_TAIL.SequenceEqual(MathUtil.trimZeroTail(DATA4)));
        }

        [TestMethod]
        public virtual void testSum()
		{
			IList<double> data1 = new List<double>();
			foreach (double number in DATA1)
			{
				data1.Add(number);
			}

            Assert.AreEqual(SUM1, MathUtil.sum(data1));

            IList <double> data2 = new List<double>();
			foreach (double number in DATA1)
			{
				data2.Add(number / 10);
			}

            Assert.AreEqual(SUM1 / 10, MathUtil.sum(data2));
        }
	}
}