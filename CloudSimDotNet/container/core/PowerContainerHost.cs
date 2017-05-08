using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.core
{

    using ContainerVmScheduler = org.cloudbus.cloudsim.container.schedulers.ContainerVmScheduler;
    using ContainerVmBwProvisioner = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmBwProvisioner;
    using ContainerVmPe = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmPe;
    using ContainerVmRamProvisioner = org.cloudbus.cloudsim.container.containerVmProvisioners.ContainerVmRamProvisioner;
    using PowerModel = org.cloudbus.cloudsim.power.models.PowerModel;
    using System.Diagnostics;

    /// <summary>
    /// Created by sareh on 28/07/15.
    /// </summary>
    public class PowerContainerHost : ContainerHostDynamicWorkload
	{

		/// <summary>
		/// The power model.
		/// </summary>
		private PowerModel powerModel;

		/// <summary>
		/// Instantiates a new host.
		/// </summary>
		/// <param name="id">             the id </param>
		/// <param name="ramProvisioner"> the ram provisioner </param>
		/// <param name="bwProvisioner">  the bw provisioner </param>
		/// <param name="storage">        the storage </param>
		/// <param name="peList">         the pe list </param>
		/// <param name="vmScheduler">    the VM scheduler </param>
		public PowerContainerHost(int id, ContainerVmRamProvisioner ramProvisioner, ContainerVmBwProvisioner bwProvisioner, long storage, IList<ContainerVmPe> peList, ContainerVmScheduler vmScheduler, PowerModel powerModel) : base(id, ramProvisioner, bwProvisioner, storage, peList, vmScheduler)
		{
			PowerModel = powerModel;
		}

		/// <summary>
		/// Gets the power. For this moment only consumed by all PEs.
		/// </summary>
		/// <returns> the power </returns>
		public virtual double Power
		{
			get
			{
				return getPower(UtilizationOfCpu);
			}
		}

		/// <summary>
		/// Gets the power. For this moment only consumed by all PEs.
		/// </summary>
		/// <param name="utilization"> the utilization </param>
		/// <returns> the power </returns>
		protected internal virtual double getPower(double utilization)
		{
			double power = 0;
			try
			{
				power = PowerModel.getPower(utilization);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
                //Environment.Exit(0);
                throw new ArgumentException("PowerModel.getPower failed", "utilization", e);
			}
			return power;
		}

		/// <summary>
		/// Gets the max power that can be consumed by the host.
		/// </summary>
		/// <returns> the max power </returns>
		public virtual double MaxPower
		{
			get
			{
				double power = 0;
				try
				{
					power = PowerModel.getPower(1);
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.ToString());
                    Debug.WriteLine(e.StackTrace);
                    //Environment.Exit(0);
                    //throw new ArgumentException("PowerModel.getPower(1)", "utilization");
                    throw e;
                }
				return power;
			}
		}

		/// <summary>
		/// Gets the energy consumption using linear interpolation of the utilization change.
		/// </summary>
		/// <param name="fromUtilization"> the from utilization </param>
		/// <param name="toUtilization">   the to utilization </param>
		/// <param name="time">            the time </param>
		/// <returns> the energy </returns>
		public virtual double getEnergyLinearInterpolation(double fromUtilization, double toUtilization, double time)
		{
			if (fromUtilization == 0)
			{
				return 0;
			}
			double fromPower = getPower(fromUtilization);
			double toPower = getPower(toUtilization);
			return (fromPower + (toPower - fromPower) / 2) * time;
		}

		/// <summary>
		/// Sets the power model.
		/// </summary>
		/// <param name="powerModel"> the new power model </param>
		protected internal virtual PowerModel PowerModel
		{
			set
			{
				this.powerModel = value;
			}
			get
			{
				return powerModel;
			}
		}


	}


}