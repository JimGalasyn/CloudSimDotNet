using System;
using System.IO;
using System.Text;
//using PCLStorage;
using System.Threading.Tasks;

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
	/// A file reader for the special BRITE-format. A BRITE file is structured as
	/// follows:<br/> 
	/// <ul>
	/// <li>Node-section: NodeID, xpos, ypos, indegree, outdegree, ASid, type(router/AS)
	/// <li>Edge-section: EdgeID, fromNode, toNode, euclideanLength, linkDelay, linkBandwith, AS_from, AS_to,
	/// type
	/// </ul>
	/// 
	/// @author Thomas Hohnstein
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class GraphReaderBrite : GraphReaderIF
	{
		private const int PARSE_NOTHING = 0;

		private const int PARSE_NODES = 1;

		private const int PARSE_EDGES = 2;

		private int state = PARSE_NOTHING;

		/// <summary>
		/// The network Topological Graph.
		/// </summary>
		private TopologicalGraph graph = null;
		public virtual async Task<TopologicalGraph> readGraphFile(string filename)
		{
			graph = new TopologicalGraph();

            // let's read the file
            //System.IO.StreamReader fr = new System.IO.StreamReader(filename);
            //var graphFile = await FileSystem.Current.GetFileFromPathAsync(filename);
            var graphFile = System.IO.File.OpenRead(filename);
            if (graphFile == null)
            {
                throw new FileNotFoundException($"{filename} not found");
            }

            // TODO: review
            var fileSize = graphFile.Length;
            var graphBuffer = new byte[fileSize];
            var bytesRead = await graphFile.ReadAsync(graphBuffer, 0, (int)fileSize); //.ReadAllTextAsync();
            //string graphString = Encoding.Unicode.GetString(graphBuffer, 0, bytesRead);
            string graphString = Encoding.UTF8.GetString(graphBuffer, 0, bytesRead);
            using (StringReader sr = new StringReader(graphString))
            {
                string lineSep = Environment.NewLine; // System.getProperty("line.separator");
                string nextLine = null;
                StringBuilder sb = new StringBuilder();

                //while (!string.ReferenceEquals((nextLine = sr.ReadLine()), null))
                while((nextLine = sr.ReadLine()) != null)
                {
                    sb.Append(nextLine);
                    //
                    // note:
                    // BufferedReader strips the EOL character.
                    //
                    sb.Append(lineSep);

                    // functionality to diferentiate between all the parsing-states
                    // state that should just find the start of node-declaration
                    if (state == PARSE_NOTHING)
                    {
                        if (nextLine.Contains("Nodes:"))
                        {
                            Log.printLine("found start of Nodes... switch to parse nodes!");
                            state = PARSE_NODES;
                        }
                    }

                    // the state to retrieve all node-information
                    else if (state == PARSE_NODES)
                    {
                        // perform the parsing of this node-line
                        parseNodeString(nextLine);
                    }

                    // the state to retrieve all edges-information
                    else if (state == PARSE_EDGES)
                    {
                        parseEdgesString(nextLine);
                    }
                }

                Log.printLine(sb.ToString());
            }

			Log.printLine("read file successfully...");
			
			return graph;
		}

		/// <summary>
		/// Parses nodes inside a line from the BRITE file.
		/// </summary>
		/// <param name="nodeLine"> A line read from the file </param>
		private void parseNodeString(string nodeLine)
		{
            // number of node parameters to parse (counts at linestart)
            int parameters = 3;

            // first test to step to the next parsing-state (edges)
            if (nodeLine.Contains("Edges:"))
			{
				// Log.printLine("found start of Edges... switch to parse edges!");
				state = PARSE_EDGES;
				return;
			}

            // test against an empty line
            //if (!tokenizer.hasMoreElements())
            if (String.IsNullOrEmpty(nodeLine))
            {
                Log.printLine("this line contains no tokens...");
                return;
            }

            // parse this string-line to read all node-parameters
            // NodeID, xpos, ypos, indegree, outdegree, ASid, type(router/AS)

            int nodeID = 0;
			string nodeLabel = "";
			int xPos = 0;
			int yPos = 0;

            //for (int actualParam = 0; tokenizer.hasMoreElements() && actualParam < parameters; actualParam++)
            //{
            //	string token = tokenizer.nextToken();
            //	switch (actualParam)
            //	{
            //		case 0: // Log.printLine("nodeID: "+token);
            //				// Log.printLine("nodeLabel: "+token);
            //			nodeID = Convert.ToInt32(token);
            //			nodeLabel = Convert.ToString(nodeID);
            //			break;

            //		case 1: // Log.printLine("x-Pos: "+token);
            //			xPos = Convert.ToInt32(token);
            //			break;

            //		case 2: // Log.printLine("y-Pos: "+token);
            //			yPos = Convert.ToInt32(token);
            //			break;
            //	}
            //}

            var tokens = nodeLine.Split('\t');
            int actualParam = 0;
            foreach (var token in tokens)
            {
                switch (actualParam)
                {
                    case 0: // Log.printLine("nodeID: "+token);
                            // Log.printLine("nodeLabel: "+token);
                        nodeID = Convert.ToInt32(token);
                        nodeLabel = Convert.ToString(nodeID);
                        break;

                    case 1: // Log.printLine("x-Pos: "+token);
                        xPos = Convert.ToInt32(token);
                        break;

                    case 2: // Log.printLine("y-Pos: "+token);
                        yPos = Convert.ToInt32(token);
                        break;

                    default:
                        throw new InvalidOperationException("actualParam must be < parameters");
                        break;
                }

                if(++actualParam >= parameters)
                {
                    break;
                }
            }

            // instantiate a new node-object with previous parsed parameters
            TopologicalNode topoNode = new TopologicalNode(nodeID, nodeLabel, xPos, yPos);
			graph.addNode(topoNode);
		}

		/// <summary>
		/// Parses edges inside a line from the BRITE file.
		/// </summary>
		/// <param name="nodeLine"> A line read from the file </param>
		private void parseEdgesString(string nodeLine)
		{
            //// number of node parameters to parse (counts at linestart)
            int parameters = 6;

            //// test against an empty line
            if (String.IsNullOrEmpty(nodeLine))
            {
                Log.printLine("this line contains no tokens...");
                return;
            }

            //// parse this string-line to read all node-parameters
            //// EdgeID, fromNode, toNode, euclideanLength, linkDelay, linkBandwith, AS_from, AS_to, type

            // int edgeID = 0;
            int fromNode = 0;
            int toNode = 0;
            // float euclideanLength = 0;
            float linkDelay = 0;
            int linkBandwith = 0;

            //for (int actualParam = 0; tokenizer.hasMoreElements() && actualParam < parameters; actualParam++)
            //{
            //	string token = tokenizer.nextToken();
            //	switch (actualParam)
            //	{
            //		case 0: // Log.printLine("edgeID: "+token);
            //				// edgeID = Integer.valueOf(token);
            //			break;

            //		case 1: // Log.printLine("fromNode: "+token);
            //			fromNode = Convert.ToInt32(token);
            //			break;

            //		case 2: // Log.printLine("toNode: "+token);
            //			toNode = Convert.ToInt32(token);
            //			break;

            //		case 3: // Log.printLine("euclideanLength: "+token);
            //				// euclideanLength = Float.valueOf(token);
            //			break;

            //		case 4: // Log.printLine("linkDelay: "+token);
            //			linkDelay = Convert.ToSingle(token);
            //			break;

            //		case 5: // Log.printLine("linkBandwith: "+token);
            //			linkBandwith = Convert.ToSingle(token).intValue();
            //			break;
            //	} // switch-END
            //} // for-END

            var tokens = nodeLine.Split('\t', ' ');
            int actualParam = 0;
            foreach (var token in tokens)
            {
                if(String.IsNullOrEmpty(token))
                {
                    continue;
                }

                switch (actualParam)
                {
                    case 0: // Log.printLine("edgeID: "+token);
                            // edgeID = Integer.valueOf(token);
                        break;

                    case 1: // Log.printLine("fromNode: "+token);
                        fromNode = Convert.ToInt32(token);
                        break;

                    case 2: // Log.printLine("toNode: "+token);
                        toNode = Convert.ToInt32(token);
                        break;

                    case 3: // Log.printLine("euclideanLength: "+token);
                            //euclideanLength = Float.valueOf(token);
                        break;

                    case 4: // Log.printLine("linkDelay: "+token);
                        linkDelay = Convert.ToSingle(token);
                        break;

                    case 5: // Log.printLine("linkBandwith: "+token);
                        linkBandwith = (int)Convert.ToSingle(token); //.intValue();
                        break;
                }

                if (++actualParam >= parameters)
                {
                    break;
                }
            }

            graph.addLink(new TopologicalLink(fromNode, toNode, linkDelay, linkBandwith));
		}
	}
}