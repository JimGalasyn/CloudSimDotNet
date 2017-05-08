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
	/// A predicate which selects events coming from specific entities.<br/>
	/// The idea of simulation predicates was copied from SimJava 2.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0 </summary>
	/// <seealso cref= PredicateNotFrom </seealso>
	/// <seealso cref= Predicate </seealso>
	public class PredicateFrom : Predicate
	{

		/// <summary>
		/// The IDs of source entities to check the reception of events from. </summary>
		private readonly int[] ids;

		/// <summary>
		/// Constructor used to select events that were sent by a specific entity.
		/// </summary>
		/// <param name="sourceId"> the id number of the source entity </param>
		public PredicateFrom(int sourceId)
		{
			ids = new int[] {sourceId};
		}

		/// <summary>
		/// Constructor used to select events that were sent by any entity from a given set.
		/// </summary>
		/// <param name="sourceIds"> the set of id numbers of the source entities </param>
		public PredicateFrom(int[] sourceIds)
		{
            //ids = sourceIds.Clone();
            sourceIds.CopyTo(ids, 0);
        }

		/// <summary>
		/// Matches any event received from the registered sources.
		/// </summary>
		/// <param name="ev"> {@inheritDoc} </param>
		/// <returns> {@inheritDoc} </returns>
		/// <seealso cref= #ids </seealso>
		public override bool match(SimEvent ev)
		{
			int src = ev.Source;
					/*
					@todo Instead of using an array where each position stores
					the id of an entity (that requires a loop over the array, it would be 
					used a HashSet (Set interface) to reduce the time
					to match the event. 
					This should be applied to the other implementations
					of the super class.
					*/
			foreach (int id in ids)
			{
				if (src == id)
				{
					return true;
				}
			}
			return false;
		}

	}

}