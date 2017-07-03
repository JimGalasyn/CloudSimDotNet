using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using DelayMatrix_Float = org.cloudbus.cloudsim.network.DelayMatrix_Float;
    using GraphReaderBrite = org.cloudbus.cloudsim.network.GraphReaderBrite;
    using TopologicalGraph = org.cloudbus.cloudsim.network.TopologicalGraph;
    using TopologicalLink = org.cloudbus.cloudsim.network.TopologicalLink;
    using TopologicalNode = org.cloudbus.cloudsim.network.TopologicalNode;

    /// <summary>
    /// Implements the network layer in CloudSim. It reads a file in the <a href="http://www.cs.bu.edu/brite/user_manual/node29.html">BRITE format</a>,
    /// the <a href="http://www.cs.bu.edu/brite/">Boston university Representative Topology gEnerator</a>, and
    /// generates a topological network from it. Information of this network is used to simulate latency
    /// in network traffic of CloudSim.
    /// <p/>
    /// The topology file may contain more nodes than the number of entities in the simulation. It allows
    /// users to increase the scale of the simulation without changing the topology file.
    /// Nevertheless, each CloudSim entity must be mapped to one (and only one) BRITE node to allow
    /// proper work of the network simulation. Each BRITE node can be mapped to only one entity at a
    /// time.
    /// 
    /// @author Rodrigo N. Calheiros
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 1.0
    /// 
    /// @todo The class only have static methods, that indicates it doesn't have to be instantiated.
    /// In fact, it doesn't appear it is being instantiated anywhere.
    /// A private default constructor would be created to avoid instantiation.
    /// </summary>
    public class NetworkTopology
	{
			/// <summary>
			/// The BRITE id to use for the next node to be created in the network.
			/// </summary>
		protected internal static int nextIdx = 0;

		private static bool networkEnabled = false;

			/// <summary>
			/// A matrix containing the delay between every pair of nodes in the network.
			/// </summary>
		protected internal static DelayMatrix_Float delayMatrix = null;

			/// <summary>
			/// A matrix containing the bandwidth between every pair of nodes in the network.
			/// </summary>
		protected internal static double[][] bwMatrix = null;

			/// <summary>
			/// The Topological Graph of the network.
			/// </summary>
		protected internal static TopologicalGraph graph = null;

		/// <summary>
		/// The map between CloudSim entities and BRITE entities.
		/// Each key is a CloudSim entity ID and each value the corresponding
		/// BRITE entity ID.
		/// </summary>
			protected internal static IDictionary<int?, int?> map = null;

		/// <summary>
		/// Creates the network topology if the file exists and can be successfully parsed. File is
		/// written in the BRITE format and contains topological information on simulation entities.
		/// </summary>
		/// <param name="fileName"> name of the BRITE file
		/// @pre fileName != null
		/// @post $none </param>
		public static async Task buildNetworkTopology(string fileName)
		{
			Log.printConcatLine("Topology file: ", fileName);

			// try to find the file
			GraphReaderBrite reader = new GraphReaderBrite();

			try
			{
				graph = await reader.readGraphFile(fileName);
				map = new Dictionary<int?, int?>();
				generateMatrices();
			}
			catch (IOException e)
			{
				// problem with the file. Does not simulate network
				Log.printLine("Problem in processing BRITE file. Network simulation is disabled. Error: " + e.Message);
                throw e;
			}
		}

		/// <summary>
		/// Generates the matrices used internally to set latency and bandwidth between elements.
		/// </summary>
		private static void generateMatrices()
		{
			// creates the delay matrix
			delayMatrix = new DelayMatrix_Float(graph, false);

			// creates the bw matrix
			bwMatrix = createBwMatrix(graph, false);

			networkEnabled = true;
		}

		/// <summary>
		/// Adds a new link in the network topology.
		/// The CloudSim entities that represent the source and destination of the link
		/// will be mapped to BRITE entities.
		/// </summary>
		/// <param name="srcId"> ID of the CloudSim entity that represents the link's source node </param>
		/// <param name="destId"> ID of the CloudSim entity that represents the link's destination node </param>
		/// <param name="bw"> Link's bandwidth </param>
		/// <param name="lat"> link's latency
		/// @pre srcId > 0
		/// @pre destId > 0
		/// @post $none </param>
		public static void addLink(int srcId, int destId, double bw, double lat)
		{
			if (graph == null)
			{
				graph = new TopologicalGraph();
			}

			if (map == null)
			{
				map = new Dictionary<int?, int?>();
			}

			// maybe add the nodes
			if (!map.ContainsKey(srcId))
			{
				graph.addNode(new TopologicalNode(nextIdx));
				map[srcId] = nextIdx;
				nextIdx++;
			}

			if (!map.ContainsKey(destId))
			{
				graph.addNode(new TopologicalNode(nextIdx));
				map[destId] = nextIdx;
				nextIdx++;
			}

			// generate a new link
			graph.addLink(new TopologicalLink(map[srcId].Value, map[destId].Value, (float) lat, (float) bw));

			generateMatrices();
		}

		/// <summary>
		/// Creates the matrix containing the available bandwidth between every pair of nodes.
		/// </summary>
		/// <param name="graph"> topological graph describing the topology </param>
		/// <param name="directed"> true if the graph is directed; false otherwise </param>
		/// <returns> the bandwidth graph </returns>
		private static double[][] createBwMatrix(TopologicalGraph graph, bool directed)
		{
			int nodes = graph.NumberOfNodes;

            // double[][] mtx = new double[nodes][nodes];
            // TODO: TEST Does this RectangularArrays class really work?
            double[][] mtx = RectangularArrays.ReturnRectangularDoubleArray(nodes, nodes);

			// cleanup matrix
			for (int i = 0; i < nodes; i++)
			{
				for (int j = 0; j < nodes; j++)
				{
					mtx[i][j] = 0.0;
				}
			}

			IEnumerator<TopologicalLink> iter = graph.LinkIterator;
			while (iter.MoveNext())
			{
				TopologicalLink edge = iter.Current;

				mtx[edge.SrcNodeID][edge.DestNodeID] = edge.LinkBw;

				if (!directed)
				{
					mtx[edge.DestNodeID][edge.SrcNodeID] = edge.LinkBw;
				}
			}

			return mtx;
		}

		/// <summary>
		/// Maps a CloudSim entity to a BRITE node in the network topology.
		/// </summary>
		/// <param name="cloudSimEntityID"> ID of the entity being mapped </param>
		/// <param name="briteID"> ID of the BRITE node that corresponds to the CloudSim entity
		/// @pre cloudSimEntityID >= 0
		/// @pre briteID >= 0
		/// @post $none </param>
		public static void mapNode(int cloudSimEntityID, int briteID)
		{
			if (networkEnabled)
			{
				try
				{
					// this CloudSim entity was already mapped?
					if (!map.ContainsKey(cloudSimEntityID))
					{
						if (!map.ContainsKey(briteID))
						{ // this BRITE node was already mapped?
							map[cloudSimEntityID] = briteID;
						}
						else
						{
							Log.printConcatLine("Error in network mapping. BRITE node ", briteID, " already in use.");
						}
					}
					else
					{
						Log.printConcatLine("Error in network mapping. CloudSim entity ", cloudSimEntityID, " already mapped.");
					}
				}
				catch (Exception)
				{
					Log.printConcatLine("Error in network mapping. CloudSim node ", cloudSimEntityID, " not mapped to BRITE node ", briteID, ".");
				}
			}
		}

		/// <summary>
		/// Unmaps a previously mapped CloudSim entity to a BRITE node in the network topology.
		/// </summary>
		/// <param name="cloudSimEntityID"> ID of the entity being unmapped
		/// @pre cloudSimEntityID >= 0
		/// @post $none </param>
		public static void unmapNode(int cloudSimEntityID)
		{
			if (networkEnabled)
			{
				try
				{
					map.Remove(cloudSimEntityID);
				}
				catch (Exception)
				{
					Log.printConcatLine("Error in network unmapping. CloudSim node: ", cloudSimEntityID);
				}
			}
		}

		/// <summary>
		/// Calculates the delay between two nodes.
		/// </summary>
		/// <param name="srcID"> ID of the CloudSim entity that represents the link's source node </param>
		/// <param name="destID"> ID of the CloudSim entity that represents the link's destination node </param>
		/// <returns> communication delay between the two nodes
		/// @pre srcID >= 0
		/// @pre destID >= 0
		/// @post $none </returns>
		public static double getDelay(int srcID, int destID)
		{
			if (networkEnabled)
			{
				try
				{
					// add the network latency
					double delay = delayMatrix.getDelay(map[srcID].Value, map[destID].Value);

					return delay;
				}
				catch (Exception ex)
				{
                    // in case of error, just keep running and return 0.0
                    // TODO: This seems to be by design -- convert to using map.ContainsKey.
                    //Debug.WriteLine(ex.ToString());
                }
            }
			return 0.0;
		}

		/// <summary>
		/// Checks if the network simulation is working. If there were some problem during
		/// creation of network (e.g., during parsing of BRITE file) that does not allow a proper
		/// simulation of the network, this method returns false.
		/// </summary>
		/// <returns> $true if network simulation is working, $false otherwise
		/// @pre $none
		/// @post $none </returns>
		public static bool NetworkEnabled
		{
			get
			{
				return networkEnabled;
			}
		}
	}
}