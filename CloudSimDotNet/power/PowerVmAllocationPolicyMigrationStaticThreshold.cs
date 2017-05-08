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
	/// A VM allocation policy that uses a Static CPU utilization Threshold (THR) to detect host over
	/// utilization.
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
	public class PowerVmAllocationPolicyMigrationStaticThreshold : PowerVmAllocationPolicyMigrationAbstract
	{

		/// <summary>
		/// The static host CPU utilization threshold to detect over utilization.
		/// It is a percentage value from 0 to 1
		/// that can be changed when creating an instance of the class. 
		/// </summary>
		private double utilizationThreshold = 0.9;

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicyMigrationStaticThreshold.
		/// </summary>
		/// <param name="hostList"> the host list </param>
		/// <param name="vmSelectionPolicy"> the vm selection policy </param>
		/// <param name="utilizationThreshold"> the utilization threshold </param>
		public PowerVmAllocationPolicyMigrationStaticThreshold(IList<Host> hostList, PowerVmSelectionPolicy vmSelectionPolicy, double utilizationThreshold) : base(hostList, vmSelectionPolicy)
		{
			UtilizationThreshold = utilizationThreshold;
		}

		/// <summary>
		/// Checks if a host is over utilized, based on CPU usage.
		/// </summary>
		/// <param name="host"> the host </param>
		/// <returns> true, if the host is over utilized; false otherwise </returns>
		protected internal override bool isHostOverUtilized(PowerHost host)
		{
			addHistoryEntry(host, UtilizationThreshold);
			double totalRequestedMips = 0;
			foreach (Vm vm in host.VmListProperty)
			{
				totalRequestedMips += vm.CurrentRequestedTotalMips;
			}
			double utilization = totalRequestedMips / host.TotalMips;
			return utilization > UtilizationThreshold;
		}

		/// <summary>
		/// Sets the utilization threshold.
		/// </summary>
		/// <param name="utilizationThreshold"> the new utilization threshold </param>
		protected internal virtual double UtilizationThreshold
		{
			set
			{
				this.utilizationThreshold = value;
			}
			get
			{
				return utilizationThreshold;
			}
		}


	}

}