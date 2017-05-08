using System;
using System.Collections.Generic;
using System.Linq;
/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.util
{
    using MathNet.Numerics.Statistics;

    /// <summary>
    /// A class containing multiple convenient math functions.
    /// 
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 3.0
    /// @todo Using Java 8 Stream, some methods here can be improved or removed.
    /// </summary>
    public class MathUtil
	{
        /// <summary>
        /// Sums a list of numbers.
        /// </summary>
        /// <param name="list"> the list of numbers </param>
        /// <returns> the double </returns>
        // TEST: (fixed) LINQ Sum implementation.
        public static double sum(IList<double> list)
        {
            double sum = list.Sum();

			//foreach (Number number in list)
			//{
			//	sum += number.doubleValue();
			//}
			return sum;
		}

        /// <summary>
        /// Converts a List to array.
        /// </summary>
        /// <param name="list"> the list of numbers </param>
        /// <returns> the double[]
        /// @todo The method <seealso cref="List#toArray()"/> could be used directly
        /// instead of creating this method. </returns>
        // TEST: (fixed) ToArray implementation.
        public static double[] listToArray(IList<double> list)
        {   
			double[] array = list.ToArray(); //new double[list.Count];
            //for (int i = 0; i < array.Length; i++)
			//{
				//array[i] = list[i].doubleValue();
			//}
			return array;
		}

        /// <summary>
        /// Gets the median from a list of numbers.
        /// </summary>
        /// <param name="list"> the list of numbers </param>
        /// <returns> the median </returns>
        // TEST: (fixed) Statistics.Median implementation
        public static double median(IList<double?> list)
		{
            //return getStatistics(list).getPercentile(50);
            // TODO: Implement getPercentile(50) 
            return Statistics.Median(list);
		}

		/// <summary>
		/// Gets the median from an array of numbers.
		/// </summary>
		/// <param name="list"> the array of numbers
		/// </param>
		/// <returns> the median </returns>
        // TEST: (fixed) Figure out median
		public static double median(double[] list)
		{
            //return getStatistics(list).getPercentile(50);
            // TODO: Implement getPercentile(50) 
            return Statistics.Median(list);
		}

        /// <summary>
        /// Returns an object to compute descriptive statistics for an list of numbers.
        /// </summary>
        /// <param name="list"> the list of numbers. Must not be null. </param>
        /// <returns> descriptive statistics for the list of numbers. </returns>
        // TEST: (fixed) Implement DescriptiveStatistics.addValue.
        public static DescriptiveStatistics getStatistics(IList<double?> list)
		{
			// Get a DescriptiveStatistics instance
			DescriptiveStatistics stats = new DescriptiveStatistics(list);

			// Add the data from the array
			//foreach (double? d in list)
			//{
				//stats.addValue(d);
			//}
			return stats;
		}

        /// <summary>
        /// Returns an object to compute descriptive statistics for an array of numbers.
        /// </summary>
        /// <param name="list"> the array of numbers. Must not be null. </param>
        /// <returns> descriptive statistics for the array of numbers. </returns>
        // TEST: (fixed) Implement DescriptiveStatistics.addValue.
        public static DescriptiveStatistics getStatistics(double[] list)
		{
			// Get a DescriptiveStatistics instance
			DescriptiveStatistics stats = new DescriptiveStatistics(list);

			// Add the data from the array
			//for (int i = 0; i < list.Length; i++)
			//{
				//stats.addValue(list[i]);
			//}
			return stats;
		}

        /// <summary>
        /// Gets the average from a list of numbers.
        /// </summary>
        /// <param name="list"> the list of numbers
        /// </param>
        /// <returns> the average </returns>
        // TEST: Statistics.Mean(list)
        public static double mean(IList<double?> list)
		{
            //double sum = 0;
            //foreach (double? number in list)
            //{
            //	sum += number.Value;
            //}
            //return sum / list.Count;

            return Statistics.Mean(list);
		}

        /// <summary>
        /// Gets the Variance from a list of numbers.
        /// </summary>
        /// <param name="list"> the list of numbers </param>
        /// <returns> the variance </returns>
        // TEST: (fixed) Statistics.Variance(list).
        public static double variance(IList<double?> list)
		{   
			//long n = 0;
            //double mean = Statistics.Mean(list);
			//double s = 0.0;

			//foreach (double x in list)
			//{
			//	n++;
			//	double delta = x - mean;
			//	mean += delta / n;
			//	s += delta * (x - mean);
			//}
			// if you want to calculate std deviation
			// of a sample change this to (s/(n-1))
			//return s / (n - 1);
            return Statistics.Variance(list);
        }

        /// <summary>
        /// Gets the standard deviation from a list of numbers.
        /// </summary>
        /// <param name="list"> the list of numbers </param>
        /// <returns> the standard deviation </returns>
        // TEST: (fixed) Statistics.StandardDeviation
        public static double stDev(IList<double?> list)
		{
            //return Math.Sqrt(variance(list));
            return Statistics.StandardDeviation(list);
		}

        /// <summary>
        /// Gets the Median absolute deviation (MAD) from a array of numbers.
        /// </summary>
        /// <param name="data"> the array of numbers </param>
        /// <returns> the mad </returns>
        // TEST: (fixed) Implement median(data).
        public static double mad(double[] data)
		{
			double mad = 0;
			if (data.Length > 0)
			{
                double median = Statistics.Median(data);

                double[] deviationSum = new double[data.Length];
				for (int i = 0; i < data.Length; i++)
				{
					deviationSum[i] = Math.Abs(median - data[i]);
				}
				mad = Statistics.Median(deviationSum); // median(deviationSum);
            }
			return mad;
		}

        /// <summary>
        /// Gets the Interquartile Range (IQR) from an array of numbers.
        /// </summary>
        /// <param name="data"> the array of numbers </param>
        /// <returns> the IQR </returns>
        public static double iqr(double[] data)
		{
            var sortedData = data.OrderBy(d => d).ToArray();
            int q1 = (int)Math.Round(0.25 * (sortedData.Length + 1)) - 1;
            int q3 = (int)Math.Round(0.75 * (sortedData.Length + 1)) - 1;
            return sortedData[q3] - sortedData[q1];

            // TODO: (low pri) SortedArrayStatistics.InterquartileRange
            //return SortedArrayStatistics.InterquartileRange(data);
        }

        /// <summary>
        /// Counts the number of values different of zero at the beginning of 
        /// an array.
        /// </summary>
        /// <param name="data"> the array of numbers </param>
        /// <returns> the number of values different of zero at the beginning of the array </returns>
        public static int countNonZeroBeginning(double[] data)
		{
			int i = data.Length - 1;
			while (i >= 0)
			{
				if (data[i--] != 0)
				{
					break;
				}
			}
			return i + 2;
		}

		/// <summary>
		/// Gets the length of the shortest row in a given matrix
		/// </summary>
		/// <param name="data"> the data matrix </param>
		/// <returns> the length of the shortest row int he matrix </returns>
		public static int countShortestRow(double[][] data)
		{
			int minLength = 0;
			foreach (double[] row in data)
			{
				if (row.Length < minLength)
				{
					minLength = row.Length;
				}
			}
			return minLength;
		}

        /// <summary>
        /// Trims zeros at the end of an array.
        /// </summary>
        /// <param name="data"> the data array </param>
        /// <returns> the trimmed array </returns>
        public static double[] trimZeroTail(double[] data)
		{
            //return Arrays.copyOfRange(data, 0, countNonZeroBeginning(data))
            var index = countNonZeroBeginning(data);
            double[] trimmedArray = new double[index];
            Array.Copy(data, 0, trimmedArray, 0, index);
            return trimmedArray;
		}

        /// <summary>
        /// Gets the Local Regression (Loess) parameter estimates.
        /// </summary>
        /// <param name="y"> the y array </param>
        /// <returns> the Loess parameter estimates </returns>
        // TODO: Figure out Loess estimate
        public static double[] getLoessParameterEstimates(double[] y)
		{
			int n = y.Length;
			double[] x = new double[n];
			for (int i = 0; i < n; i++)
			{
				x[i] = i + 1;
			}
            //return createWeigthedLinearRegression(x, y, getTricubeWeigts(n)).regress().ParameterEstimates;
            return y;
		}

        // TODO: Figure out SimpleRegression. 
        public static SimpleRegression createLinearRegression(double[] x, double[] y)
		{
			SimpleRegression regression = new SimpleRegression();
			for (int i = 0; i < x.Length; i++)
			{
				//regression.addData(x[i], y[i]);
			}
			return regression;
		}

        // TODO: Figure out OLSMultipleLinearRegression
        public static OLSMultipleLinearRegression createLinearRegression(double[][] x, double[] y)
		{
			OLSMultipleLinearRegression regression = new OLSMultipleLinearRegression();
			//regression.newSampleData(y, x);
			return regression;
		}

		public static SimpleRegression createWeigthedLinearRegression(double[] x, double[] y, double[] weigths)
		{
			double[] xW = new double[x.Length];
			double[] yW = new double[y.Length];

			int numZeroWeigths = 0;
			for (int i = 0; i < weigths.Length; i++)
			{
				if (weigths[i] <= 0)
				{
					numZeroWeigths++;
				}
			}

			for (int i = 0; i < x.Length; i++)
			{
				if (numZeroWeigths >= 0.4 * weigths.Length)
				{
					// See: http://www.ncsu.edu/crsc/events/ugw07/Presentations/Crooks_Qiao/Crooks_Qiao_Alt_Presentation.pdf
					xW[i] = Math.Sqrt(weigths[i]) * x[i];
					yW[i] = Math.Sqrt(weigths[i]) * y[i];
				}
				else
				{
					xW[i] = x[i];
					yW[i] = y[i];
				}
			}

			return createLinearRegression(xW, yW);
		}

		/// <summary>
		/// Gets the robust loess parameter estimates.
		/// </summary>
		/// <param name="y"> the y array </param>
		/// <returns> the robust loess parameter estimates </returns>
        // TODO: Find implementations for predict and regress.
		public static double[] getRobustLoessParameterEstimates(double[] y)
		{
			int n = y.Length;
			double[] x = new double[n];
			for (int i = 0; i < n; i++)
			{
				x[i] = i + 1;
			}
			SimpleRegression tricubeRegression = createWeigthedLinearRegression(x, y, getTricubeWeigts(n));
			double[] residuals = new double[n];
			for (int i = 0; i < n; i++)
			{
				//residuals[i] = y[i] - tricubeRegression.predict(x[i]);
			}
			SimpleRegression tricubeBySquareRegression = createWeigthedLinearRegression(x, y, getTricubeBisquareWeigts(residuals));

            double[] estimates = y; // tricubeBySquareRegression.regress().ParameterEstimates;
			if (double.IsNaN(estimates[0]) || double.IsNaN(estimates[1]))
			{
				//return tricubeRegression.regress().ParameterEstimates;
			}
			return estimates;
		}

		/// <summary>
		/// Gets the tricube weigths.
		/// </summary>
		/// <param name="n"> the number of weights </param>
		/// <returns> an array of tricube weigths with n elements
		/// @todo The word "weight" is misspelled in the method name. </returns>
		public static double[] getTricubeWeigts(int n)
		{
			double[] weights = new double[n];
			double top = n - 1;
			double spread = top;
			for (int i = 2; i < n; i++)
			{
				double k = Math.Pow(1 - Math.Pow((top - i) / spread, 3), 3);
				if (k > 0)
				{
					weights[i] = 1 / k;
				}
				else
				{
					weights[i] = double.MaxValue;
				}
			}
			weights[0] = weights[1] = weights[2];
			return weights;
		}

		/// <summary>
		/// Gets the tricube bisquare weigths.
		/// </summary>
		/// <param name="residuals"> the residuals array </param>
		/// <returns> the tricube bisquare weigths
		/// @todo The word "weight" is misspelled in the method name. </returns>
		public static double[] getTricubeBisquareWeigts(double[] residuals)
		{
			int n = residuals.Length;
			double[] weights = getTricubeWeigts(n);
			double[] weights2 = new double[n];
			double s6 = median(abs(residuals)) * 6;
			for (int i = 2; i < n; i++)
			{
				double k = Math.Pow(1 - Math.Pow(residuals[i] / s6, 2), 2);
				if (k > 0)
				{
					weights2[i] = (1 / k) * weights[i];
				}
				else
				{
					weights2[i] = double.MaxValue;
				}
			}
			weights2[0] = weights2[1] = weights2[2];
			return weights2;
		}

		/// <summary>
		/// Gets the absolute values of an array of values
		/// </summary>
		/// <param name="data"> the array of values </param>
		/// <returns> a new array with the absolute value of each element in the given array. </returns>
		public static double[] abs(double[] data)
		{
			double[] result = new double[data.Length];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = Math.Abs(data[i]);
			}
			return result;
		}
	}

    public class OLSMultipleLinearRegression
    {
    }

    public class SimpleRegression
    {
    }
}