using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.core
{
    using CloudSimLib.util;

	/// <summary>
	/// This class implements the future event queue used by <seealso cref="CloudSim"/>. 
	/// The event queue uses a <seealso cref="TreeSet"/> in order to store the events.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0 </summary>
	/// <seealso cref= Simulation </seealso>
	/// <seealso cref= java.util.TreeSet
	/// 
	/// @todo It would be used a common interface for queues
	/// such as this one and <seealso cref="DeferredQueue"/> </seealso>
	public class FutureQueue //: List<SimEvent>
	{

		/// <summary>
		/// The sorted set of events. </summary>
		private readonly SortedSet<SimEvent> sortedSet = new SortedSet<SimEvent>();

		/// <summary>
		/// A incremental number used for <seealso cref="SimEvent#serial"/> event attribute.
		/// </summary>
		private long serial = 0;

		/// <summary>
		/// Adds a new event to the queue. Adding a new event to the queue preserves the temporal order of
		/// the events in the queue.
		/// </summary>
		/// <param name="newEvent"> The event to be put in the queue. </param>
		public virtual void addEvent(SimEvent newEvent)
		{
			newEvent.Serial = serial++;
			sortedSet.Add(newEvent);
		}

		/// <summary>
		/// Adds a new event to the head of the queue.
		/// </summary>
		/// <param name="newEvent"> The event to be put in the queue. </param>
		public virtual void addEventFirst(SimEvent newEvent)
		{
			newEvent.Serial = 0;
			sortedSet.Add(newEvent);
		}

		/// <summary>
		/// Returns an iterator to the queue.
		/// </summary>
		/// <returns> the iterator </returns>
		public virtual IEnumerator<SimEvent> iterator()
		{
			return sortedSet.GetEnumerator();
		}

		/// <summary>
		/// Returns the size of this event queue.
		/// </summary>
		/// <returns> the size </returns>
		public virtual int size()
		{
			return sortedSet.Count;
		}

        public virtual int Count
        {
            get
            {
                return sortedSet.Count;
            }
        }


        /// <summary>
        /// Removes the event from the queue.
        /// </summary>
        /// <param name="event"> the event </param>
        /// <returns> true, if successful </returns>
        public virtual bool remove(SimEvent @event)
		{
			return sortedSet.Remove(@event);
		}

		/// <summary>
		/// Removes all the events from the queue.
		/// </summary>
		/// <param name="events"> the events </param>
		/// <returns> true, if successful </returns>
		public virtual bool removeAll(ICollection<SimEvent> events)
		{
            //return sortedSet.removeAll(events);
            foreach(var e in events)
            {
                sortedSet.Remove(e);
            }
            return true;
		}

		/// <summary>
		/// Clears the queue.
		/// </summary>
		public virtual void clear()
		{
			sortedSet.Clear();
		}
	}
}