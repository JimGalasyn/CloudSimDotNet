/*
 * @(#)FloydWarshall.java	ver 1.2  6/20/2005
 *
 * Modified by Weishuai Yang (wyang@cs.binghamton.edu).
 * Originally written by Rahul Simha
 *
 */

namespace org.cloudbus.cloudsim.network
{

	/// <summary>
	/// FloydWarshall algorithm to calculate the predecessor matrix 
	/// and the delay between all pairs of nodes.
	/// 
	/// @author Rahul Simha
	/// @author Weishuai Yang
	/// @version 1.2, 6/20/2005
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class FloydWarshall_Float
	{

		/// <summary>
		/// Number of vertices (nodes).
		/// </summary>
		private int numVertices;

		/// <summary>
		/// Matrices used in dynamic programming.
		/// </summary>
		private float[][] Dk, Dk_minus_one;

		/// <summary>
		/// The predecessor matrix. Matrix used by dynamic programming.
		/// </summary>
		private int[][] Pk;

			/// <summary>
			/// Matrix used by dynamic programming.
			/// </summary>
			private int[][] Pk_minus_one;

		/// <summary>
		/// Initialization the matrix.
		/// </summary>
		/// <param name="numVertices"> number of nodes
		/// @todo The class doesn't have a constructor. This should be the constructor. </param>
		public virtual void initialize(int numVertices)
		{
			this.numVertices = numVertices;

			// Initialize Dk matrices.
			Dk = new float[numVertices][];
			Dk_minus_one = new float[numVertices][];
			for (int i = 0; i < numVertices; i++)
			{
				Dk[i] = new float[numVertices];
				Dk_minus_one[i] = new float[numVertices];
			}

			// Initialize Pk matrices.
			Pk = new int[numVertices][];
			Pk_minus_one = new int[numVertices][];
			for (int i = 0; i < numVertices; i++)
			{
				Pk[i] = new int[numVertices];
				Pk_minus_one[i] = new int[numVertices];
			}

		}

		/// <summary>
		/// Calculates the delay between all pairs of nodes.
		/// </summary>
		/// <param name="adjMatrix"> original delay matrix </param>
		/// <returns> the delay matrix </returns>
		public virtual float[][] allPairsShortestPaths(float[][] adjMatrix)
		{
			// Dk_minus_one = weights when k = -1
			for (int i = 0; i < numVertices; i++)
			{
				for (int j = 0; j < numVertices; j++)
				{
					if (adjMatrix[i][j] != 0)
					{
						Dk_minus_one[i][j] = adjMatrix[i][j];
						Pk_minus_one[i][j] = i;
					}
					else
					{
						Dk_minus_one[i][j] = float.MaxValue;
						Pk_minus_one[i][j] = -1;
					}
					// NOTE: we have set the value to infinity and will exploit
					// this to avoid a comparison.
				}
			}

			// Now iterate over k.

			for (int k = 0; k < numVertices; k++)
			{

				// Compute Dk[i][j], for each i,j

				for (int i = 0; i < numVertices; i++)
				{
					for (int j = 0; j < numVertices; j++)
					{
						if (i != j)
						{

							// D_k[i][j] = min ( D_k-1[i][j], D_k-1[i][k] + D_k-1[k][j].
							if (Dk_minus_one[i][j] <= Dk_minus_one[i][k] + Dk_minus_one[k][j])
							{
								Dk[i][j] = Dk_minus_one[i][j];
								Pk[i][j] = Pk_minus_one[i][j];
							}
							else
							{
								Dk[i][j] = Dk_minus_one[i][k] + Dk_minus_one[k][j];
								Pk[i][j] = Pk_minus_one[k][j];
							}
						}
						else
						{
							Pk[i][j] = -1;
						}
					}
				}

				// Now store current Dk into D_k-1
				for (int i = 0; i < numVertices; i++)
				{
					for (int j = 0; j < numVertices; j++)
					{
						Dk_minus_one[i][j] = Dk[i][j];
						Pk_minus_one[i][j] = Pk[i][j];
					}
				}

			} // end-outermost-for

			return Dk;

		}

		/// <summary>
		/// Gets predecessor matrix.
		/// </summary>
		/// <returns> predecessor matrix </returns>
		public virtual int[][] PK
		{
			get
			{
				return Pk;
			}
		}
	}

}