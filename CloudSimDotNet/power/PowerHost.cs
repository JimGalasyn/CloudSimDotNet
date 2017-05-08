using System;
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

    using PowerModel = org.cloudbus.cloudsim.power.models.PowerModel;
    using BwProvisioner = org.cloudbus.cloudsim.provisioners.BwProvisioner;
    using RamProvisioner = org.cloudbus.cloudsim.provisioners.RamProvisioner;
    using System.Diagnostics;

    /// <summary>
    /// PowerHost class enables simulation of power-aware hosts.
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
    /// @since CloudSim Toolkit 2.0
    /// </summary>
    public class PowerHost : HostDynamicWorkload
    {

        /// <summary>
        /// The power model used by the host. </summary>
        private PowerModel powerModel;

        /// <summary>
        /// Instantiates a new PowerHost.
        /// </summary>
        /// <param name="id"> the id of the host </param>
        /// <param name="ramProvisioner"> the ram provisioner </param>
        /// <param name="bwProvisioner"> the bw provisioner </param>
        /// <param name="storage"> the storage capacity </param>
        /// <param name="peList"> the host's PEs list </param>
        /// <param name="vmScheduler"> the VM scheduler </param>
        //public PowerHost<T1>(int id, RamProvisioner ramProvisioner, BwProvisioner bwProvisioner, long storage, IList<T1> peList, VmScheduler vmScheduler, PowerModel powerModel) where T1 : org.cloudbus.cloudsim.Pe : base(id, ramProvisioner, bwProvisioner, storage, peList, vmScheduler)
        public PowerHost(int id, RamProvisioner ramProvisioner, BwProvisioner bwProvisioner, long storage, IList<Pe> peList, VmScheduler vmScheduler, PowerModel powerModel) : base(id, ramProvisioner, bwProvisioner, storage, peList, vmScheduler)
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
        /// Gets the current power consumption of the host. For this moment only consumed by all PEs.
        /// </summary>
        /// <param name="utilization"> the utilization percentage (between [0 and 1]) of a resource that
        /// is critical for power consumption </param>
        /// <returns> the power consumption </returns>
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
                throw e;
                //Environment.Exit(0);
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
                    throw e;
                    //Environment.Exit(0);
                }
                return power;
            }
        }

        /// <summary>
        /// Gets the energy consumption using linear interpolation of the utilization change.
        /// </summary>
        /// <param name="fromUtilization"> the initial utilization percentage </param>
        /// <param name="toUtilization"> the final utilization percentage </param>
        /// <param name="time"> the time </param>
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
        public virtual PowerModel PowerModel
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
