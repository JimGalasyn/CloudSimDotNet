using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.vmSelectionPolicies
{


	using ContainerVm = org.cloudbus.cloudsim.container.core.ContainerVm;
	using PowerContainerHost = org.cloudbus.cloudsim.container.core.PowerContainerHost;
	using PowerContainerHostUtilizationHistory = org.cloudbus.cloudsim.container.core.PowerContainerHostUtilizationHistory;
	using PowerContainerVm = org.cloudbus.cloudsim.container.core.PowerContainerVm;
	using Correlation = org.cloudbus.cloudsim.container.utils.Correlation;

	/// <summary>
	/// Created by sareh on 16/11/15.
	/// </summary>
	public class PowerContainerVMSelectionPolicyCor : PowerContainerVmSelectionPolicy
	{


		/// <summary>
		/// The fallback policy.
		/// </summary>
		private PowerContainerVmSelectionPolicy fallbackPolicy;

		/// <summary>
		/// Instantiates a new power vm selection policy maximum correlation.
		/// </summary>
		/// <param name="fallbackPolicy"> the fallback policy </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public PowerContainerVMSelectionPolicyCor(final PowerContainerVmSelectionPolicy fallbackPolicy)
		public PowerContainerVMSelectionPolicyCor(PowerContainerVmSelectionPolicy fallbackPolicy) : base()
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
//ORIGINAL LINE: @Override public org.cloudbus.cloudsim.container.core.ContainerVm getVmToMigrate(final org.cloudbus.cloudsim.container.core.PowerContainerHost host)
		public override ContainerVm getVmToMigrate(PowerContainerHost host)
		{
			IList<PowerContainerVm> migratableVMs = getMigratableVms(host);
			if (migratableVMs.Count == 0)
			{
				return null;
			}
			ContainerVm vm = getContainerVM(migratableVMs, host);
			migratableVMs.Clear();
			if (vm != null)
			{
	//            Log.printConcatLine("We have to migrate the container with ID", container.getId());
				return vm;
			}
			else
			{
				return FallbackPolicy.getVmToMigrate(host);
			}
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



		public virtual ContainerVm getContainerVM(IList<PowerContainerVm> migratableContainerVMs, PowerContainerHost host)
		{

			double[] corResult = new double[migratableContainerVMs.Count];
			Correlation correlation = new Correlation();
			int i = 0;
			double maxValue = -2;
			int id = -1;
			if (host is PowerContainerHostUtilizationHistory)
			{

				double[] hostUtilization = ((PowerContainerHostUtilizationHistory) host).UtilizationHistory;
				foreach (ContainerVm vm in migratableContainerVMs)
				{
					double[] containerUtilization = ((PowerContainerVm) vm).UtilizationHistoryList;

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

			return migratableContainerVMs[id];

		}


	}














}