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
	/// <summary>
	/// This class implements the deferred event queue used by <seealso cref="CloudSim"/>. 
	/// The event queue uses a linked list to store the events.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0 </summary>
	/// <seealso cref= CloudSim </seealso>
	/// <seealso cref= SimEvent </seealso>
	public class DeferredQueue //: List<SimEvent>
    {
		/// <summary>
		/// The list of events. </summary>
		private readonly IList<SimEvent> list = new List<SimEvent>();

		/// <summary>
		/// The max time that an added event is scheduled. </summary>
		private double maxTime = -1;

		/// <summary>
		/// Adds a new event to the queue. Adding a new event to the queue preserves the temporal order
		/// of the events.
		/// </summary>
		/// <param name="newEvent"> The event to be added to the queue. </param>
		public virtual void addEvent(SimEvent newEvent)
		{
			// The event has to be inserted as the last of all events
			// with the same event_time(). Yes, this matters.
			double eventTime = newEvent.eventTime();
			if (eventTime >= maxTime)
			{
				list.Add(newEvent);
				maxTime = eventTime;
				return;
			}

            // TODO: Needs to be reimplemented.
			IEnumerator<SimEvent> iterator = list.GetEnumerator();
			SimEvent simEevent;
			while (iterator.MoveNext())
			{
                simEevent = iterator.Current;
				if (simEevent.eventTime() > eventTime)
				{
                    //iterator.previous();
                    //iterator.add(newEvent);
                    list.Add(newEvent);
                    return;
				}
			}

			list.Add(newEvent);
		}

        // TODO: TEST this new DeferedQueue.removeEvent method
        public virtual void removeEvent(SimEvent newEvent)
        {
            list.Remove(newEvent);
        }

        /// <summary>
        /// Returns an iterator to the events in the queue.
        /// </summary>
        /// <returns> the iterator </returns>
        public virtual IEnumerator<SimEvent> iterator()
		{
			return list.GetEnumerator();
		}

		/// <summary>
		/// Returns the size of this event queue.
		/// </summary>
		/// <returns> the number of events in the queue. </returns>
		public virtual int size()
		{
			return list.Count;
		}

		/// <summary>
		/// Clears the queue.
		/// </summary>
		public virtual void clear()
		{
			list.Clear();
		}
	}
}