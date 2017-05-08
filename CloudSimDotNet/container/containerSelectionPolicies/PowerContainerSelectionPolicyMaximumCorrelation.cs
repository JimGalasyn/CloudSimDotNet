using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerSelectionPolicies
{

	using Container = org.cloudbus.cloudsim.container.core.Container;
	using PowerContainer = org.cloudbus.cloudsim.container.core.PowerContainer;
	using PowerContainerHost = org.cloudbus.cloudsim.container.core.PowerContainerHost;
	//using Array2DRowRealMatrix = org.apache.commons.math3.linear.Array2DRowRealMatrix;
	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;


	/// <summary>
	/// Created by sareh on 31/07/15.
	/// </summary>
	public class PowerContainerSelectionPolicyMaximumCorrelation : PowerContainerSelectionPolicy
	{


		/// <summary>
		/// The fallback policy.
		/// </summary>
		private PowerContainerSelectionPolicy fallbackPolicy;

		/// <summary>
		/// Instantiates a new power container selection policy maximum correlation.
		/// </summary>
		/// <param name="fallbackPolicy"> the fallback policy </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public PowerContainerSelectionPolicyMaximumCorrelation(final PowerContainerSelectionPolicy fallbackPolicy)
		public PowerContainerSelectionPolicyMaximumCorrelation(PowerContainerSelectionPolicy fallbackPolicy) : base()
		{
			FallbackPolicy = fallbackPolicy;
		}

		/*
		 * (non-Javadoc)
		 *
		 * @see powerContainerSelectionPolicy#getContainerToMigrate()
		 */
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public org.cloudbus.cloudsim.container.core.Container getContainerToMigrate(final org.cloudbus.cloudsim.container.core.PowerContainerHost host)
		public override Container getContainerToMigrate(PowerContainerHost host)
		{
			IList<PowerContainer> migratableContainers = getMigratableContainers(host);
			if (migratableContainers.Count == 0)
			{
				return null;
			}
			IList<double?> metrics = null;
			try
			{
				metrics = getCorrelationCoefficients(getUtilizationMatrix(migratableContainers));
			}
			catch (System.ArgumentException)
			{ // the degrees of freedom must be greater than zero
				return FallbackPolicy.getContainerToMigrate(host);
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
			return migratableContainers[maxIndex];
		}

		/// <summary>
		/// Gets the utilization matrix.
		/// </summary>
		/// <param name="powerContainers"> the powerContainers </param>
		/// <returns> the utilization matrix </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected double[][] getUtilizationMatrix(final java.util.List<org.cloudbus.cloudsim.container.core.PowerContainer> powerContainers)
		protected internal virtual double[][] getUtilizationMatrix(IList<PowerContainer> powerContainers)
		{
			int n = powerContainers.Count;
			int m = getMinUtilizationHistorySize(powerContainers);
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] utilization = new double[n][m];
			double[][] utilization = RectangularArrays.ReturnRectangularDoubleArray(n, m);
			for (int i = 0; i < n; i++)
			{
				IList<double?> vmUtilization = powerContainers[i].UtilizationHistory;
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
		/// <param name="containerList"> the container list </param>
		/// <returns> the min utilization history size </returns>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected int getMinUtilizationHistorySize(final java.util.List<org.cloudbus.cloudsim.container.core.PowerContainer> containerList)
		protected internal virtual int getMinUtilizationHistorySize(IList<PowerContainer> containerList)
		{
			int minSize = int.MaxValue;
			foreach (PowerContainer container in containerList)
			{
				int size = container.UtilizationHistory.Count;
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
                // TODO: Find a replacement for the Array2DRowRealMatrix class.
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
		public virtual PowerContainerSelectionPolicy FallbackPolicy
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