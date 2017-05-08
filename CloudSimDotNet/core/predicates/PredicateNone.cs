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
	/// A predicate which will <b>not</b> match any event on the deferred event queue. 
	/// See the publicly accessible instance of this predicate in
	/// <seealso cref="org.cloudbus.cloudsim.core.CloudSim#SIM_NONE"/>, so no new instances needs to be created. <br/>
	/// The idea of simulation predicates was copied from SimJava 2.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0 </summary>
	/// <seealso cref= Predicate </seealso>
	/// <seealso cref= Simulation </seealso>
	public class PredicateNone : Predicate
	{

		/// <summary>
		/// Considers that no event received by the predicate matches.
		/// </summary>
		/// <param name="ev"> {@inheritDoc} </param>
		/// <returns> always false to indicate that no event is accepted </returns>
		public override bool match(SimEvent ev)
		{
			return false;
		}
	}

}