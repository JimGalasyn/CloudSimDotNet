/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.core.predicates
{

	/// <summary>
	/// A predicate to select events that don't match specific tags.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0 </summary>
	/// <seealso cref= PredicateType </seealso>
	/// <seealso cref= Predicate </seealso>
	public class PredicateNotType : Predicate
	{

		/// <summary>
		/// Array of tags to verify if the tag of received events doesn't correspond to. </summary>
		private readonly int[] tags;

		/// <summary>
		/// Constructor used to select events whose tags do not match a given tag.
		/// </summary>
		/// <param name="tag"> An event tag value </param>
		public PredicateNotType(int tag)
		{
			tags = new int[] {tag};
		}

		/// <summary>
		/// Constructor used to select events whose tag values do not match any of the given tags.
		/// </summary>
		/// <param name="tags"> the list of tags </param>
		public PredicateNotType(int[] sourceTags)
		{
            //this.tags = tags.Clone();
            sourceTags.CopyTo(tags, 0);
        }

		/// <summary>
		/// Matches any event that hasn't one of the specified <seealso cref="#tags"/>.
		/// </summary>
		/// <param name="ev"> {@inheritDoc} </param>
		/// <returns> {@inheritDoc} </returns>
		/// <seealso cref= #tags </seealso>
		public override bool match(SimEvent ev)
		{
			int tag = ev.Tag;
			foreach (int tag2 in tags)
			{
				if (tag == tag2)
				{
					return false;
				}
			}
			return true;
		}

	}
}