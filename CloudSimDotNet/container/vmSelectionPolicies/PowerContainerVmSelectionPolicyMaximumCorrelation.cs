using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.vmSelectionPolicies
{

	using org.cloudbus.cloudsim.container.core;
	//using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;


	/// <summary>
	/// Created by sareh on 3/08/15.
	/// </summary>
	public class PowerContainerVmSelectionPolicyMaximumCorrelation : PowerContainerVmSelectionPolicy
	{


			/// <summary>
			/// The fallback policy. </summary>
			private PowerContainerVmSelectionPolicy fallbackPolicy;

			/// <summary>
			/// Instantiates a new power vm selection policy maximum correlation.
			/// </summary>
			/// <param name="fallbackPolicy"> the fallback policy </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public PowerContainerVmSelectionPolicyMaximumCorrelation(final PowerContainerVmSelectionPolicy fallbackPolicy)
			public PowerContainerVmSelectionPolicyMaximumCorrelation(PowerContainerVmSelectionPolicy fallbackPolicy) : base()
			{
				FallbackPolicy = fallbackPolicy;
			}

			/*
			 * (non-Javadoc)
			 *
			 * @see org.cloudbus.cloudsim.experiments.power.PowerVmSelectionPolicy#
			 * getVmsToMigrate(org.cloudbus .cloudsim.power.PowerHost)
			 */
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public ContainerVm getVmToMigrate(final PowerContainerHost host)
			public override ContainerVm getVmToMigrate(PowerContainerHost host)
			{
				IList<PowerContainerVm> migratableVms = getMigratableVms(host);
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
			/// Gets the utilization matrix.
			/// </summary>
			/// <param name="vmList"> the host </param>
			/// <returns> the utilization matrix </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected double[][] getUtilizationMatrix(final java.util.List<PowerContainerVm> vmList)
			protected internal virtual double[][] getUtilizationMatrix(IList<PowerContainerVm> vmList)
			{
				int n = vmList.Count;
				int m = getMinUtilizationHistorySize(vmList);
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] utilization = new double[n][m];
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
			/// Gets the min utilization history size.
			/// </summary>
			/// <param name="vmList"> the vm list </param>
			/// <returns> the min utilization history size </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected int getMinUtilizationHistorySize(final java.util.List<PowerContainerVm> vmList)
			protected internal virtual int getMinUtilizationHistorySize(IList<PowerContainerVm> vmList)
			{
				int minSize = int.MaxValue;
				foreach (PowerContainerVm vm in vmList)
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
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected java.util.List<Nullable<double>> getCorrelationCoefficients(final double[][] data)
			protected internal virtual IList<double?> getCorrelationCoefficients(double[][] data)
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
                // TODO: Figure out calculateRSquared
                //correlationCoefficients.Add(MathUtil.createLinearRegression(xT, data[i]).calculateRSquared());
            }
            return correlationCoefficients;
			}

			/// <summary>
			/// Gets the fallback policy.
			/// </summary>
			/// <returns> the fallback policy </returns>
			public virtual PowerContainerVmSelectionPolicy FallbackPolicy
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