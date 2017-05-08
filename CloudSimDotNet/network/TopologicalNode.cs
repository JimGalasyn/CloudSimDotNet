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
	/// Represents an topological network node that retrieves its information from a
	/// topological-generated file (eg. topology-generator)
	/// 
	/// @author Thomas Hohnstein
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class TopologicalNode
	{

		/// <summary>
		/// The BRITE id of the node inside the network.
		/// </summary>
		private int nodeID = 0;

		/// <summary>
		/// The name of the node inside the network.
		/// </summary>
		private string nodeName = null;

		/// <summary>
		/// Represents the x world-coordinate.
		/// </summary>
		private int worldX = 0;

		/// <summary>
		/// Represents the y world-coordinate.
		/// </summary>
		private int worldY = 0;

		/// <summary>
		/// Constructs an new node. </summary>
		/// <param name="nodeID"> The BRITE id of the node inside the network </param>
		public TopologicalNode(int nodeID)
		{
				this.nodeID = nodeID;
				nodeName = nodeID.ToString();
		}

		/// <summary>
		/// Constructs an new node including world-coordinates. </summary>
		/// <param name="nodeID"> The BRITE id of the node inside the network </param>
		/// <param name="x"> x world-coordinate </param>
		/// <param name="y"> y world-coordinate </param>
		public TopologicalNode(int nodeID, int x, int y)
		{
				this.nodeID = nodeID;
				nodeName = nodeID.ToString();
				worldX = x;
				worldY = y;
		}

		/// <summary>
		/// Constructs an new node including world-coordinates and the nodeName. </summary>
		/// <param name="nodeID"> The BRITE id of the node inside the network </param>
		/// <param name="nodeName"> The name of the node inside the network </param>
		/// <param name="x"> x world-coordinate </param>
		/// <param name="y"> y world-coordinate </param>
		public TopologicalNode(int nodeID, string nodeName, int x, int y)
		{
				this.nodeID = nodeID;
				this.nodeName = nodeName;
				worldX = x;
				worldY = y;
		}

		/// <summary>
		/// Gets the node BRITE id.
		/// </summary>
		/// <returns> the nodeID </returns>
		public virtual int NodeID
		{
			get
			{
					return nodeID;
			}
		}

		/// <summary>
		/// Gets the name of the node
		/// </summary>
		/// <returns> name of the node </returns>
		public virtual string NodeLabel
		{
			get
			{
					return nodeName;
			}
		}

		/// <summary>
		/// Gets the x world coordinate of this network-node.
		/// </summary>
		/// <returns> the x world coordinate </returns>
		public virtual int CoordinateX
		{
			get
			{
					return worldX;
			}
		}

		/// <summary>
		/// Gets the y world coordinate of this network-node
		/// </summary>
		/// <returns> the y world coordinate </returns>
		public virtual int CoordinateY
		{
			get
			{
					return worldY;
			}
		}

	}

}