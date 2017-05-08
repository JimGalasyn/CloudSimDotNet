using System.Collections.Generic;
using System.Text;

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
	/// This class represents a graph containing vertices (nodes) and edges (links), 
	/// used for input with a network-layer.
	/// Graphical-Output Restricions! <br/>
	/// <ul>
	///   <li>EdgeColors: GraphicalProperties.getColorEdge 
	///   <li>NodeColors: GraphicalProperties.getColorNode
	/// </ul>
	/// 
	/// @author Thomas Hohnstein
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class TopologicalGraph
	{
			/// <summary>
			/// The list of links of the network graph.
			/// </summary>
		private IList<TopologicalLink> linkList = null;

			/// <summary>
			/// The list of nodes of the network graph.
			/// </summary>
		private IList<TopologicalNode> nodeList = null;

		/// <summary>
		/// Creates an empty graph-object.
		/// </summary>
		public TopologicalGraph()
		{
			linkList = new List<TopologicalLink>();
			nodeList = new List<TopologicalNode>();
		}

		/// <summary>
		/// Adds an link between two topological nodes.
		/// </summary>
		/// <param name="edge"> the topological link </param>
		public virtual void addLink(TopologicalLink edge)
		{
			linkList.Add(edge);
		}

		/// <summary>
		/// Adds an Topological Node to this graph.
		/// </summary>
		/// <param name="node"> the topological node to add </param>
		public virtual void addNode(TopologicalNode node)
		{
			nodeList.Add(node);
		}

		/// <summary>
		/// Gets the number of nodes contained inside the topological-graph.
		/// </summary>
		/// <returns> number of nodes </returns>
		public virtual int NumberOfNodes
		{
			get
			{
				return nodeList.Count;
			}
		}

		/// <summary>
		/// Gets the number of links contained inside the topological-graph.
		/// </summary>
		/// <returns> number of links </returns>
		public virtual int NumberOfLinks
		{
			get
			{
				return linkList.Count;
			}
		}

		/// <summary>
		/// Gets an iterator through all network-graph links.
		/// </summary>
		/// <returns> the iterator throug all links </returns>
		public virtual IEnumerator<TopologicalLink> LinkIterator
		{
			get
			{
				return linkList.GetEnumerator();
			}
		}

		/// <summary>
		/// Gets an iterator through all network-graph nodes.
		/// </summary>
		/// <returns> the iterator through all nodes </returns>
		public virtual IEnumerator<TopologicalNode> NodeIterator
		{
			get
			{
				return nodeList.GetEnumerator();
			}
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append("topological-node-information: \n");

			foreach (TopologicalNode node in nodeList)
			{
				buffer.Append(node.NodeID + " | x is: " + node.CoordinateX + " y is: " + node.CoordinateY + "\n");
			}

			buffer.Append("\n\n node-link-information:\n");

			foreach (TopologicalLink link in linkList)
			{
				buffer.Append("from: " + link.SrcNodeID + " to: " + link.DestNodeID + " delay: " + link.LinkDelay + "\n");
			}
			return buffer.ToString();
		}

	}

}