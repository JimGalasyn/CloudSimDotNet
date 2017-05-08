using System;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.power
{
    using System.Diagnostics;
    using CloudSim = org.cloudbus.cloudsim.core.CloudSim;
    using CloudSimTags = org.cloudbus.cloudsim.core.CloudSimTags;
    using SimEvent = org.cloudbus.cloudsim.core.SimEvent;

    /// <summary>
    /// A power-aware <seealso cref="DatacenterBroker"/>.
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
    public class PowerDatacenterBroker : DatacenterBroker
	{

		/// <summary>
		/// Instantiates a new PowerDatacenterBroker.
		/// </summary>
		/// <param name="name"> the name of the broker </param>
		/// <exception cref="Exception"> the exception </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public PowerDatacenterBroker(String name) throws Exception
		public PowerDatacenterBroker(string name) : base(name)
		{
		}

		protected internal override void processVmCreate(SimEvent ev)
		{
			int[] data = (int[]) ev.Data;
			int result = data[2];

			if (result != CloudSimTags.TRUE)
			{
				int datacenterId = data[0];
				int vmId = data[1];
				Debug.WriteLine(CloudSim.clock() + ": " + Name + ": Creation of VM #" + vmId + " failed in Datacenter #" + datacenterId);
                //Environment.Exit(0);
                throw new InvalidOperationException("Creation of VM failed");
			}
			base.processVmCreate(ev);
		}

	}

}