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


	/// <summary>
	/// An abstract VM selection policy used to select VMs from a list of migratable VMs.
	/// The selection is defined by sub classes.
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
	public abstract class PowerVmSelectionPolicy
	{

		/// <summary>
		/// Gets a VM to migrate from a given host.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the vm to migrate </returns>
		public abstract Vm getVmToMigrate(PowerHost host);

		/// <summary>
		/// Gets the list of migratable VMs from a given host.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> the list of migratable VMs </returns>
		protected internal virtual IList<PowerVm> getMigratableVms(PowerHost host)
		{
			IList<PowerVm> migratableVms = new List<PowerVm>();
			foreach (PowerVm vm in host.VmListProperty)
			{
				if (!vm.InMigration)
				{
					migratableVms.Add(vm);
				}
			}
			return migratableVms;
		}

	}

}