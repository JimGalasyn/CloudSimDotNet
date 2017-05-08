using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.containerSelectionPolicies
{

	using Container = org.cloudbus.cloudsim.container.core.Container;
	using PowerContainer = org.cloudbus.cloudsim.container.core.PowerContainer;
	using PowerContainerHost = org.cloudbus.cloudsim.container.core.PowerContainerHost;
	using PowerContainerHostUtilizationHistory = org.cloudbus.cloudsim.container.core.PowerContainerHostUtilizationHistory;
	using Correlation = org.cloudbus.cloudsim.container.utils.Correlation;

	/// <summary>
	/// Created by sareh on 7/08/15.
	/// </summary>
	public class PowerContainerSelectionPolicyCor : PowerContainerSelectionPolicy
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
//ORIGINAL LINE: public PowerContainerSelectionPolicyCor(final org.cloudbus.cloudsim.container.containerSelectionPolicies.PowerContainerSelectionPolicy fallbackPolicy)
		public PowerContainerSelectionPolicyCor(PowerContainerSelectionPolicy fallbackPolicy) : base()
		{
			FallbackPolicy = fallbackPolicy;
		}

		/*
		* (non-Javadoc)
		*
		* @see PowerContainerSelectionPolicy#getContainerToMigrate
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
			Container container = getContainer(migratableContainers, host);
			migratableContainers.Clear();
			if (container != null)
			{
	//            Log.printConcatLine("We have to migrate the container with ID", container.getId());
				return container;
			}
			else
			{
				return FallbackPolicy.getContainerToMigrate(host);
			}
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



		public virtual Container getContainer(IList<PowerContainer> migratableContainers, PowerContainerHost host)
		{

			double[] corResult = new double[migratableContainers.Count];
			Correlation correlation = new Correlation();
			int i = 0;
			double maxValue = -2.0;
			int id = -1;
			if (host is PowerContainerHostUtilizationHistory)
			{

				double[] hostUtilization = ((PowerContainerHostUtilizationHistory) host).UtilizationHistory;
				foreach (Container container in migratableContainers)
				{
					double[] containerUtilization = ((PowerContainer) container).UtilizationHistoryList;

					double cor = correlation.getCor(hostUtilization, containerUtilization);
					if (double.IsNaN(cor))
					{
						cor = -3;
					}
					corResult[i] = cor;

					if (corResult[i] > maxValue)
					{
						maxValue = corResult[i];
						id = i;
					}

					i++;
				}

			}

			if (id == -1)
			{
				Log.printConcatLine("Problem with correlation list.");
			}

			return migratableContainers[id];

		}


	}

}