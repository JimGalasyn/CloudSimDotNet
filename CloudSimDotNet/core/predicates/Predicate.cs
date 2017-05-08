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
	/// Predicates are used to select events from the deferred queue, according to 
	/// required criteria. 
	/// They are used internally the by <seealso cref="org.cloudbus.cloudsim.core.CloudSim"/> class
	/// and aren't intended to be used directly by the user.
	/// 
	/// This class is abstract and must be
	/// extended when writing a new predicate. Each subclass define
	/// the criteria to select received events.
	/// 
	/// Some standard predicates are provided.<br/>
	/// The idea of simulation predicates was copied from SimJava 2.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0 </summary>
	/// <seealso cref= PredicateType </seealso>
	/// <seealso cref= PredicateFrom </seealso>
	/// <seealso cref= PredicateAny </seealso>
	/// <seealso cref= PredicateNone </seealso>
	/// <seealso cref= Simulation
	/// @todo It would be an interface, since it doesn't have any attributes, just 
	/// abstract methods.
	/// @todo There already is a native java <seealso cref="java.util.function.Predicate"/> interface.
	/// Maybe it was introduced with Java 8 (due to Stream and Lambda functions).
	///  </seealso>
	public abstract class Predicate
	{

		/// <summary>
		/// Verifies if a given event matches the required criteria.
		/// The method is called for each event in the deferred queue when a method such as
		/// <seealso cref="org.cloudbus.cloudsim.core.CloudSim#select(int, org.cloudbus.cloudsim.core.predicates.Predicate) "/> 
		/// is called.
		/// </summary>
		/// <param name="event"> The event to test for a match. </param>
		/// <returns> <code>true</code> if the event matches and should be
		///         selected, or <code>false</code> if it does not match the predicate. </returns>
		public abstract bool match(SimEvent @event);

	}

}