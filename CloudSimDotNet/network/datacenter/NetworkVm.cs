using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.network.datacenter
{


	/// <summary>
	/// NetworkVm class extends <seealso cref="Vm"/> to support simulation of networked datacenters. 
	/// It executes actions related to management of packets (sent and received).
	/// 
	/// <br/>Please refer to following publication for more details:<br/>
	/// <ul>
	/// <li><a href="http://dx.doi.org/10.1109/UCC.2011.24">Saurabh Kumar Garg and Rajkumar Buyya, NetworkCloudSim: Modelling Parallel Applications in Cloud
	/// Simulations, Proceedings of the 4th IEEE/ACM International Conference on Utility and Cloud
	/// Computing (UCC 2011, IEEE CS Press, USA), Melbourne, Australia, December 5-7, 2011.</a>
	/// </ul>
	/// 
	/// @author Saurabh Kumar Garg
	/// @since CloudSim Toolkit 3.0
	/// @todo Attributes should be private
	/// </summary>
	public class NetworkVm : Vm, IComparable<object>
	{
			/// <summary>
			/// List of <seealso cref="NetworkCloudlet"/> of the VM.
			/// </summary>
		public List<NetworkCloudlet> cloudletlist;

        /// <summary>
        /// @todo It doesn't appear to be used.
        /// </summary>
        // TEST: (fixed) Never used
        //internal int type;

        /// <summary>
        /// List of packets received by the VM.
        /// </summary>
        public List<HostPacket> recvPktlist;

			/// <summary>
			/// @todo It doesn't appear to be used.
			/// </summary>
		public double memory;

			/// <summary>
			/// @todo It doesn't appear to be used.
			/// </summary>
		public bool flagfree;

			/// <summary>
			/// The time when the VM finished to process its cloudlets.
			/// </summary>
		public double finishtime;

		public NetworkVm(int id, int userId, double mips, int pesNumber, int ram, long bw, long size, string vmm, CloudletScheduler cloudletScheduler) : base(id, userId, mips, pesNumber, ram, bw, size, vmm, cloudletScheduler)
		{

			cloudletlist = new List<NetworkCloudlet>();
		}

		public virtual bool Free
		{
			get
			{
				return flagfree;
			}
		}

		public virtual int CompareTo(object arg0)
		{
			NetworkVm hs = (NetworkVm) arg0;
			if (hs.finishtime > finishtime)
			{
				return -1;
			}
			if (hs.finishtime < finishtime)
			{
				return 1;
			}
			return 0;
		}
	}

}