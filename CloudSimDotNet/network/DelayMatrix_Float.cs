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
	/// This class represents a delay matrix between every pair or nodes
	/// inside a network topology, storing every distance between connected nodes.
	/// 
	/// @author Thomas Hohnstein
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class DelayMatrix_Float
	{

		/// <summary>
		/// Matrix holding delay information between any two nodes.
		/// </summary>
		protected internal float[][] mDelayMatrix = null;

		/// <summary>
		/// Number of nodes in the distance-aware-topology.
		/// </summary>
		protected internal int mTotalNodeNum = 0;

		/// <summary>
		/// Private constructor to ensure that only an correct initialized delay-matrix could be created.
		/// </summary>
		private DelayMatrix_Float()
		{
		}

		/// <summary>
		/// Creates an correctly initialized Float-Delay-Matrix.
		/// </summary>
		/// <param name="graph"> the network topological graph </param>
		/// <param name="directed"> indicates if an directed matrix should be computed (true) or not (false) </param>
		public DelayMatrix_Float(TopologicalGraph graph, bool directed)
		{

			// lets preinitialize the Delay-Matrix
			createDelayMatrix(graph, directed);

			// now its time to calculate all possible connection-delays
			calculateShortestPath();
		}

		/// <summary>
		/// Gets the delay between two nodes.
		/// </summary>
		/// <param name="srcID"> the id of the source node </param>
		/// <param name="destID"> the id of the destination node </param>
		/// <returns> the delay between the given two nodes </returns>
		public virtual float getDelay(int srcID, int destID)
		{
			// check the nodeIDs against internal array-boundarys
			if (srcID > mTotalNodeNum || destID > mTotalNodeNum)
			{
				throw new System.IndexOutOfRangeException("srcID or destID is higher than highest stored node-ID!");
			}

			return mDelayMatrix[srcID][destID];
		}

		/// <summary>
		/// Creates all internal necessary network-distance structures from the given graph. 
		/// For similarity, we assume all communication-distances are symmetrical, 
		/// thus leading to an undirected network.
		/// </summary>
		/// <param name="graph"> the network topological graph </param>
		/// <param name="directed"> indicates if an directed matrix should be computed (true) or not (false) </param>
		private void createDelayMatrix(TopologicalGraph graph, bool directed)
		{

			// number of nodes inside the network
			mTotalNodeNum = graph.NumberOfNodes;

//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: mDelayMatrix = new float[mTotalNodeNum][mTotalNodeNum];
			mDelayMatrix = RectangularArrays.ReturnRectangularFloatArray(mTotalNodeNum, mTotalNodeNum);

			// cleanup the complete distance-matrix with "0"s
			for (int row = 0; row < mTotalNodeNum; ++row)
			{
				for (int col = 0; col < mTotalNodeNum; ++col)
				{
					mDelayMatrix[row][col] = float.MaxValue;
				}
			}

			IEnumerator<TopologicalLink> itr = graph.LinkIterator;

			TopologicalLink edge;
			while (itr.MoveNext())
			{
				edge = itr.Current;

				mDelayMatrix[edge.SrcNodeID][edge.DestNodeID] = edge.LinkDelay;

				if (!directed)
				{
					// according to aproximity of symmetry to all communication-paths
					mDelayMatrix[edge.DestNodeID][edge.SrcNodeID] = edge.LinkDelay;
				}

			}
		}

		/// <summary>
		/// Calculates the shortest path between all pairs of nodes.
		/// </summary>
		private void calculateShortestPath()
		{
			FloydWarshall_Float floyd = new FloydWarshall_Float();

			floyd.initialize(mTotalNodeNum);
			mDelayMatrix = floyd.allPairsShortestPaths(mDelayMatrix);
		}

		public override string ToString()
		{
			StringBuilder buffer = new StringBuilder();

			buffer.Append("just a simple printout of the distance-aware-topology-class\n");
			buffer.Append("delay-matrix is:\n");

			for (int column = 0; column < mTotalNodeNum; ++column)
			{
				buffer.Append("\t" + column);
			}

			for (int row = 0; row < mTotalNodeNum; ++row)
			{
				buffer.Append("\n" + row);

				for (int col = 0; col < mTotalNodeNum; ++col)
				{
					if (mDelayMatrix[row][col] == float.MaxValue)
					{
						buffer.Append("\t" + "-");
					}
					else
					{
						buffer.Append("\t" + mDelayMatrix[row][col]);
					}
				}
			}

			return buffer.ToString();
		}
	}

}