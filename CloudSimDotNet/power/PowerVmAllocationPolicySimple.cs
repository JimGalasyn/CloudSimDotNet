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
	/// A simple VM allocation policy that does <b>not</b> perform any optimization on VM allocation.
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
	public class PowerVmAllocationPolicySimple : PowerVmAllocationPolicyAbstract
	{

		/// <summary>
		/// Instantiates a new PowerVmAllocationPolicySimple.
		/// </summary>
		/// <param name="list"> the list </param>
		public PowerVmAllocationPolicySimple(IList<Host> list) : base(list)
		{
		}

			/// <summary>
			/// The method doesn't perform any VM allocation optimization
			/// and in fact has no effect. </summary>
			/// <param name="vmList"> </param>
			/// <returns>  </returns>
		public override IList<IDictionary<string, object>> optimizeAllocation(IList<Vm> vmList)
		{
					//@todo It is better to return an empty map in order to avoid NullPointerException or extra null checks
			// This policy does not optimize the VM allocation
			return null;
		}

	}

}