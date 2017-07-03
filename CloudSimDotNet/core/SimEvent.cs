using System;

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
	/// This class represents a simulation event which is passed between the entities in the simulation.
	/// 
	/// @author Costas Simatos </summary>
	/// <seealso cref= Simulation </seealso>
	/// <seealso cref= SimEntity </seealso>
	public class SimEvent : ICloneable, IComparable<SimEvent>
	{
        // TEST: (fixed) Deal with ICloneable

        /// <summary>
        /// Internal event type. * </summary>
        private readonly int etype;

		/// <summary>
		/// The time that this event was scheduled, at which it should occur. * </summary>
		private readonly double time;

		/// <summary>
		/// Time that the event was removed from the queue to start service. * </summary>
		private double endWaitingTimeValue;

		/// <summary>
		/// Id of entity who scheduled the event. * </summary>
		private int entSrc;

		/// <summary>
		/// Id of entity that the event will be sent to. * </summary>
		private int entDst;

		/// <summary>
		/// The user defined type of the event. * </summary>
		private readonly int tag;

		/// <summary>
		/// Any data the event is carrying. 
		/// @todo I would be used generics to define the type of the event data.
		/// But this modification would incur several changes in the simulator core
		/// that has to be assessed first.
		///        
		/// </summary>
		private readonly object data;

			/// <summary>
			/// An attribute to help CloudSim to identify the order of received events
			/// when multiple events are generated at the same time.
			/// If two events have the same <seealso cref="#time"/>, to know
			/// what event is greater than other (i.e. that happens after other),
			/// the <seealso cref="#compareTo(org.cloudbus.cloudsim.core.SimEvent)"/>
			/// makes use of this field.
			/// </summary>
		private long serial = -1;

		// Internal event types

		public const int ENULL = 0;

		public const int SEND = 1;

		public const int HOLD_DONE = 2;

		public const int CREATE = 3;

		/// <summary>
		/// Creates a blank event.
		/// </summary>
		public SimEvent()
		{
			etype = ENULL;
			time = -1L;
			endWaitingTimeValue = -1.0;
			entSrc = -1;
			entDst = -1;
			tag = -1;
			data = null;
		}

		// ------------------- PACKAGE LEVEL METHODS --------------------------
		internal SimEvent(int evtype, double time, int src, int dest, int tag, object edata)
		{
			etype = evtype;
			this.time = time;
			entSrc = src;
			entDst = dest;
			this.tag = tag;
			data = edata;
		}

		internal SimEvent(int evtype, double time, int src)
		{
			etype = evtype;
			this.time = time;
			entSrc = src;
			entDst = -1;
			tag = -1;
			data = null;
		}

		protected internal virtual long Serial
		{
			set
			{
				this.serial = value;
			}
		}

		/// <summary>
		/// Gets or sets the time that the event was removed from the queue to start service. 
		/// </summary>
		/// <param name="end_waiting_time"> </param>
		public virtual double EndWaitingTime
		{
            get
            {
                return endWaitingTimeValue;
            }
			internal set
			{
				endWaitingTimeValue = value;
			}
		}

		// ------------------- PUBLIC METHODS --------------------------        

		public override string ToString()
		{
			return "Event tag = " + tag + " source = " + CloudSim.getEntity(entSrc).Name + " destination = " + CloudSim.getEntity(entDst).Name;
		}

		/// <summary>
		/// Gets the internal type
		/// 
		/// @return
		/// </summary>
		public virtual int Type
		{
			get
			{
				return etype;
			}
		}


		public virtual int CompareTo(SimEvent @event)
		{
			if (@event == null)
			{
				return 1;
			}
			else if (time < @event.time)
			{
				return -1;
			}
			else if (time > @event.time)
			{
				return 1;
			}
			else if (serial < @event.serial)
			{
				return -1;
			}
			else if (this == @event)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}

		/// <summary>
		/// Get the unique id number of the entity which received this event.
		/// </summary>
		/// <returns> the id number </returns>
		public virtual int Destination
		{
			get
			{
				return entDst;
			}
			set
			{
				entDst = value;
			}
		}

		/// <summary>
		/// Get the unique id number of the entity which scheduled this event.
		/// </summary>
		/// <returns> the id number </returns>
		public virtual int Source
		{
			get
			{
				return entSrc;
			}
			set
			{
				entSrc = value;
			}
		}

		/// <summary>
		/// Get the simulation time that this event was scheduled.
		/// </summary>
		/// <returns> The simulation time </returns>
		public virtual double eventTime()
		{
			return time;
		}

		/// <summary>
		/// Get the simulation time that this event was removed from the queue for service.
		/// </summary>
		/// <returns> The simulation time </returns>
		//public virtual double endWaitingTime()
		//{
		//	return endWaitingTime_Renamed;
		//}

		/// <summary>
		/// Get the user-defined tag of this event
		/// </summary>
		/// <returns> The tag </returns>
		public virtual int type()
		{
			return tag;
		}

		/// <summary>
		/// Get the unique id number of the entity which scheduled this event.
		/// </summary>
		/// <returns> the id number </returns>
		public virtual int scheduledBy()
		{
			return entSrc;
		}

		/// <summary>
		/// Get the user-defined tag of this event.
		/// </summary>
		/// <returns> The tag </returns>
		public virtual int Tag
		{
			get
			{
				return tag;
			}
		}

		/// <summary>
		/// Get the data passed in this event.
		/// </summary>
		/// <returns> A reference to the data </returns>
		public virtual object Data
		{
			get
			{
				return data;
			}
		}

		//public override object clone()
        public object Clone()
		{
			return new SimEvent(etype, time, entSrc, entDst, tag, data);
		}
	}
}