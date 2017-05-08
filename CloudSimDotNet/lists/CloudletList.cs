using System;
using System.Collections.Generic;
using System.Linq;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.lists
{


	/// <summary>
	/// CloudletList is a collection of operations on lists of Cloudlets.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// </summary>
	public class CloudletList
	{

        /// <summary>
        /// Gets a <seealso cref="Cloudlet"/> with a given id.
        /// </summary>
        /// <param name="cloudletList"> the list of existing Cloudlets </param>
        /// <param name="id"> the Cloudlet id </param>
        /// <returns> a Cloudlet with the given ID or $null if not found </returns>
        //public static T getById<T>(IList<T> cloudletList, int id) where T : org.cloudbus.cloudsim.Cloudlet
        public static Cloudlet getById(IList<Cloudlet> cloudletList, int id)
        {
			foreach (var cloudlet in cloudletList)
			{
				if (cloudlet.CloudletId == id)
				{
					return cloudlet;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the position of a cloudlet with a given id.
		/// </summary>
		/// <param name="cloudletList"> the list of existing cloudlets </param>
		/// <param name="id"> the cloudlet id </param>
		/// <returns> the position of the cloudlet with the given id or -1 if not found </returns>
		public static int getPositionById(IList<Cloudlet> cloudletList, int id)
		{
			int i = 0;
				foreach (var cloudlet in cloudletList)
				{
				if (cloudlet.CloudletId == id)
				{
					return i;
				}
				i++;
				}
			return -1;
		}

		/// <summary>
		/// Sorts the Cloudlets in a list based on their lengths.
		/// </summary>
		/// <param name="cloudletList"> the cloudlet list
		/// @pre $none
		/// @post $none </param>
		public static void sort(IList<Cloudlet> cloudletList)
		{   
            //cloudletList.Sort(new ComparatorAnonymousInnerClass());
            // TEST: (fixed) LINQ sort
            var comparer = new ComparatorAnonymousInnerClass();
            var sortedCloudletList = cloudletList.OrderBy(c => c, comparer).ToList();

            // TODO: return parameter
        }

        private class ComparatorAnonymousInnerClass : IComparer<Cloudlet>
		{
			public ComparatorAnonymousInnerClass()
			{
			}


            /// <summary>
            /// Compares two Cloudlets.
            /// </summary>
            /// <param name="a"> the first Cloudlet to be compared </param>
            /// <param name="b"> the second Cloudlet to be compared </param>
            /// <returns> the value 0 if both Cloudlets are numerically equal; a value less than 0 if the
            ///         first Object is numerically less than the second Cloudlet; and a value greater
            ///         than 0 if the first Cloudlet is numerically greater than the second Cloudlet. </returns>
            /// <exception cref="ClassCastException"> <tt>a</tt> and <tt>b</tt> are expected to be of type
            ///             <tt>Cloudlet</tt>
            /// @pre a != null
            /// @pre b != null
            /// @post $none </exception>
            /// <remarks>TODO: Implement a proper IComparator.</remarks>
            //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
            //ORIGINAL LINE: public int compare(T a, T b) throws ClassCastException
            //public virtual int Compare(T a, T b)
            public virtual int Compare(Cloudlet a, Cloudlet b)
            {
                double? cla = Convert.ToDouble(a.CloudletTotalLength);
                double? clb = Convert.ToDouble(b.CloudletTotalLength);
                return cla.Value.CompareTo(clb.Value);
			}
		}

	}

}