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
	/// A predicate to select events with specific tags.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0 </summary>
	/// <seealso cref= PredicateNotType </seealso>
	/// <seealso cref= Predicate </seealso>
	public class PredicateType : Predicate
	{

		/// <summary>
		/// Array of tags to verify if the tag of received events correspond to. </summary>
		private readonly int[] tags;

		/// <summary>
		/// Constructor used to select events with the given tag value.
		/// </summary>
		/// <param name="t1"> an event tag value </param>
		public PredicateType(int t1)
		{
			tags = new int[] {t1};
		}

		/// <summary>
		/// Constructor used to select events with a tag value equal to any of the specified tags.
		/// </summary>
		/// <param name="tags"> the list of tags </param>
		public PredicateType(int[] sourceTags)
		{
			//this.tags = tags.Clone();
            sourceTags.CopyTo(tags, 0);
        }

		/// <summary>
		/// Matches any event that has one of the specified <seealso cref="#tags"/>.
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
					return true;
				}
			}
			return false;
		}

	}

}