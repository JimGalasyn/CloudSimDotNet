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
	/// A predicate which selects events that have not been sent by specific entities.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0 </summary>
	/// <seealso cref= PredicateFrom </seealso>
	/// <seealso cref= Predicate </seealso>
	public class PredicateNotFrom : Predicate
	{

		/// <summary>
		/// The IDs of source entities to check if events were not sent from. </summary>
		private readonly int[] ids;

		/// <summary>
		/// Constructor used to select events that were not sent by a specific entity.
		/// </summary>
		/// <param name="sourceId"> the id number of the source entity </param>
		public PredicateNotFrom(int sourceId)
		{
			ids = new int[] {sourceId};
		}

		/// <summary>
		/// Constructor used to select events that were not sent by any entity from a given set.
		/// </summary>
		/// <param name="sourceIds"> the set of id numbers of the source entities </param>
		public PredicateNotFrom(int[] sourceIds)
		{
            //ids = sourceIds.Clone();
            sourceIds.CopyTo(ids, 0);
        }

		/// <summary>
		/// Matches any event <b>not</b> received from the registered sources.
		/// </summary>
		/// <param name="ev"> {@inheritDoc} </param>
		/// <returns> {@inheritDoc} </returns>
		/// <seealso cref= #ids </seealso>
		public override bool match(SimEvent ev)
		{
			int src = ev.Source;
			foreach (int id in ids)
			{
				if (src == id)
				{
					return false;
				}
			}
			return true;
		}

	}

}