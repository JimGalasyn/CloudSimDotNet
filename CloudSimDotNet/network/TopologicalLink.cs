/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.network
{

	/// <summary>
	/// This class represents an link (edge) from a network graph.
	/// 
	/// @author Thomas Hohnstein
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class TopologicalLink
	{

		/// <summary>
		/// The BRITE id of the source node of the link.
		/// </summary>
		private int srcNodeID = 0;

		/// <summary>
		/// The BRITE id of the destination node of the link.
		/// </summary>
		private int destNodeID = 0;

		/// <summary>
		/// The link delay of the connection.
		/// </summary>
		private float linkDelay = 0;

		/// <summary>
		/// The link bandwidth (bw).
		/// </summary>
		private float linkBw = 0;

		/// <summary>
		/// Creates a new Topological Link.
		/// </summary>
		public TopologicalLink(int srcNode, int destNode, float delay, float bw)
		{
			// lets initialize all internal attributes
			linkDelay = delay;
			srcNodeID = srcNode;
			destNodeID = destNode;
			linkBw = bw;
		}

		/// <summary>
		/// Gets the BRITE id of the source node of the link.
		/// </summary>
		/// <returns> nodeID </returns>
		public virtual int SrcNodeID
		{
			get
			{
				return srcNodeID;
			}
		}

		/// <summary>
		/// Gets the BRITE id of the destination node of the link.
		/// </summary>
		/// <returns> nodeID </returns>
		public virtual int DestNodeID
		{
			get
			{
				return destNodeID;
			}
		}

		/// <summary>
		/// Gets the delay of the link.
		/// </summary>
		/// <returns> the link delay </returns>
		public virtual float LinkDelay
		{
			get
			{
				return linkDelay;
			}
		}

		/// <summary>
		/// Gets the bandwidth of the link.
		/// </summary>
		/// <returns> the bw </returns>
		public virtual float LinkBw
		{
			get
			{
				return linkBw;
			}
		}

	}

}