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
	using MathUtil = org.cloudbus.cloudsim.util.MathUtil;

	/// <summary>
	/// A host that stores its CPU utilization percentage history. The history is used by VM allocation
	/// and selection policies.
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
	public class PowerHostUtilizationHistory : PowerHost
	{

		/// <summary>
		/// Instantiates a new PowerHostUtilizationHistory.
		/// </summary>
		/// <param name="id"> the host id </param>
		/// <param name="ramProvisioner"> the ram provisioner </param>
		/// <param name="bwProvisioner"> the bw provisioner </param>
		/// <param name="storage"> the storage capacity </param>
		/// <param name="peList"> the host's PEs list </param>
		/// <param name="vmScheduler"> the vm scheduler </param>
		/// <param name="powerModel"> the power consumption model </param>
		public PowerHostUtilizationHistory(int id, RamProvisioner ramProvisioner, BwProvisioner bwProvisioner, long storage, IList<Pe> peList, VmScheduler vmScheduler, PowerModel powerModel) : base(id, ramProvisioner, bwProvisioner, storage, peList, vmScheduler, powerModel)
		{
		}

		/// <summary>
		/// Gets the host CPU utilization percentage history.
		/// </summary>
		/// <returns> the host CPU utilization percentage history </returns>
		protected internal virtual double[] UtilizationHistory
		{
			get
			{
				double[] utilizationHistory = new double[PowerVm.HISTORY_LENGTH];
				double hostMips = TotalMips;
				foreach (PowerVm vm in this.VmListProperty)
				{
					for (int i = 0; i < vm.UtilizationHistory.Count; i++)
					{
						utilizationHistory[i] += vm.UtilizationHistory[i].Value * vm.Mips / hostMips;
					}
				}
				return MathUtil.trimZeroTail(utilizationHistory);
			}
		}
	}
}