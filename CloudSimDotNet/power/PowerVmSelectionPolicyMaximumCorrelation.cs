using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power
{


	//using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;

	/// <summary>
	/// A VM selection policy that selects for migration the VM with the Maximum Correlation Coefficient (MCC) among 
	/// a list of migratable VMs.
	/// 
	/// <br/>If you are using any algorithms, policies or workload included in the power package please cite
	/// the following paper:<br/>
	/// 
	/// <ul>
	/// <li><a href="http://dx.doi.org/10.1002/cpe.1867">Anton Beloglazov, and Rajkumar Buyya, "Optimal Online Deterministic Algorithms and Adaptive
	/// Heuristics for Energy and Performance Efficient Dynamic Consolidation of Virtual Machines in
	/// Cloud Data Centers", Concurrency and Computation: Practice and Experience (CCPE), Volume 24,
	/// Issue 13, Pages: 1397-1420, John Wiley & Sons, Ltd, New York, USA, 2012</a>
	/// </ul>
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 3.0
	/// </summary>
	public class PowerVmSelectionPolicyMaximumCorrelation : PowerVmSelectionPolicy
	{

		/// <summary>
		/// The fallback VM selection policy to be used when
		/// the  Maximum Correlation policy doesn't have data to be computed. 
		/// </summary>
		private PowerVmSelectionPolicy fallbackPolicy;

		/// <summary>
		/// Instantiates a new PowerVmSelectionPolicyMaximumCorrelation.
		/// </summary>
		/// <param name="fallbackPolicy"> the fallback policy </param>
		public PowerVmSelectionPolicyMaximumCorrelation(PowerVmSelectionPolicy fallbackPolicy) : base()
		{
			FallbackPolicy = fallbackPolicy;
		}

		public override Vm getVmToMigrate(PowerHost host)
		{
			IList<PowerVm> migratableVms = getMigratableVms(host);
			if (migratableVms.Count == 0)
			{
				return null;
			}
			IList<double?> metrics = null;
			try
			{
				metrics = getCorrelationCoefficients(getUtilizationMatrix(migratableVms));
			}
			catch (System.ArgumentException)
			{ // the degrees of freedom must be greater than zero
				return FallbackPolicy.getVmToMigrate(host);
			}
			double maxMetric = double.Epsilon;
			int maxIndex = 0;
			for (int i = 0; i < metrics.Count; i++)
			{
				double metric = metrics[i].Value;
				if (metric > maxMetric)
				{
					maxMetric = metric;
					maxIndex = i;
				}
			}
			return migratableVms[maxIndex];
		}

		/// <summary>
		/// Gets the CPU utilization percentage matrix for a given list of VMs.
		/// </summary>
		/// <param name="vmList"> the VM list </param>
		/// <returns> the CPU utilization percentage matrix, where each line i
		/// is a VM and each column j is a CPU utilization percentage history for that VM. </returns>
		protected internal virtual double[][] getUtilizationMatrix(IList<PowerVm> vmList)
		{
			int n = vmList.Count;
					/*@todo It gets the min size of the history among all VMs considering
					that different VMs can have different history sizes.
					However, the j loop is not using the m variable
					but the size of the vm list. If a VM list has 
					a size greater than m, it will thow an exception.
					It as to be included a test case for that.*/
			int m = getMinUtilizationHistorySize(vmList);
			double[][] utilization = RectangularArrays.ReturnRectangularDoubleArray(n, m);
			for (int i = 0; i < n; i++)
			{
				IList<double?> vmUtilization = vmList[i].UtilizationHistory;
				for (int j = 0; j < vmUtilization.Count; j++)
				{
					utilization[i][j] = vmUtilization[j].Value;
				}
			}
			return utilization;
		}

		/// <summary>
		/// Gets the min CPU utilization percentage history size among a list of VMs.
		/// </summary>
		/// <param name="vmList"> the VM list </param>
		/// <returns> the min CPU utilization percentage history size of the VM list </returns>
		protected internal virtual int getMinUtilizationHistorySize(IList<PowerVm> vmList)
		{
			int minSize = int.MaxValue;
			foreach (PowerVm vm in vmList)
			{
				int size = vm.UtilizationHistory.Count;
				if (size < minSize)
				{
					minSize = size;
				}
			}
			return minSize;
		}

		/// <summary>
		/// Gets the correlation coefficients.
		/// </summary>
		/// <param name="data"> the data </param>
		/// <returns> the correlation coefficients </returns>
		public virtual IList<double?> getCorrelationCoefficients(double[][] data)
		{
			int n = data.Length;
			int m = data[0].Length;
			IList<double?> correlationCoefficients = new List<double?>();
			for (int i = 0; i < n; i++)
			{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] x = new double[n - 1][m];
				double[][] x = RectangularArrays.ReturnRectangularDoubleArray(n - 1, m);
				int k = 0;
				for (int j = 0; j < n; j++)
				{
					if (j != i)
					{
						x[k++] = data[j];
					}
				}

                // Transpose the matrix so that it fits the linear model
                //double[][] xT = (new Array2DRowRealMatrix(x)).transpose().Data;
                // TODO: Find Array2DRowRealMatrix replacement.
                double[][] xT = new double[n][];

                // RSquare is the "coefficient of determination"
                // TODO: Implement calculateRSquared()
                //correlationCoefficients.Add(MathUtil.createLinearRegression(xT, data[i]).calculateRSquared());
            }
            return correlationCoefficients;
		}

		/// <summary>
		/// Gets the fallback policy.
		/// </summary>
		/// <returns> the fallback policy </returns>
		public virtual PowerVmSelectionPolicy FallbackPolicy
		{
			get
			{
				return fallbackPolicy;
			}
			set
			{
				this.fallbackPolicy = value;
			}
		}


	}

}