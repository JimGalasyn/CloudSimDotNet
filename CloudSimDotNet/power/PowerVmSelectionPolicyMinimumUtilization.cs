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

	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

	/// <summary>
	/// A VM selection policy that selects for migration the VM with Minimum Utilization (MU)
	/// of CPU.
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
	public class PowerVmSelectionPolicyMinimumUtilization : PowerVmSelectionPolicy
	{
		public override Vm getVmToMigrate(PowerHost host)
		{
			IList<PowerVm> migratableVms = getMigratableVms(host);
			if (migratableVms.Count == 0)
			{
				return null;
			}
			Vm vmToMigrate = null;
			double minMetric = double.MaxValue;
			foreach (Vm vm in migratableVms)
			{
				if (vm.InMigration)
				{
					continue;
				}
				double metric = vm.getTotalUtilizationOfCpuMips(CloudSim.clock()) / vm.Mips;
				if (metric < minMetric)
				{
					minMetric = metric;
					vmToMigrate = vm;
				}
			}
			return vmToMigrate;
		}

	}

}