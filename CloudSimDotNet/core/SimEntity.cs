using System;
using System.Threading;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.core
{

	using Predicate = org.cloudbus.cloudsim.core.predicates.Predicate;

	/// <summary>
	/// This class represents a simulation entity. An entity handles events and can send events to other
	/// entities. When this class is extended, there are a few methods that need to be implemented:
	/// <ul>
	/// <li> <seealso cref="#startEntity()"/> is invoked by the <seealso cref="Simulation"/> class when the simulation is
	/// started. This method should be responsible for starting the entity up.
	/// <li> <seealso cref="#processEvent(SimEvent)"/> is invoked by the <seealso cref="Simulation"/> class whenever there is
	/// an event in the deferred queue, which needs to be processed by the entity.
	/// <li> <seealso cref="#shutdownEntity()"/> is invoked by the <seealso cref="Simulation"/> before the simulation
	/// finishes. If you want to save data in log files this is the method in which the corresponding
	/// code would be placed.
	/// </ul>
	/// 
	/// @todo the list above is redundant once all mentioned methods are abstract.
	/// The documentation duplication may lead to have some of them
	/// out-of-date and future confusion.
	/// 
	/// @author Marcos Dias de Assuncao
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public abstract class SimEntity : ICloneable
	{
        // TEST: (fixed) Deal with ICloneable

        /// <summary>
        /// The entity name. </summary>
        private string name;

		/// <summary>
		/// The entity id. </summary>
		private int id;

		/// <summary>
		/// The buffer for selected incoming events. </summary>
		private SimEvent evbuf;

		/// <summary>
		/// The entity's current state. </summary>
		private int state;

		/// <summary>
		/// Creates a new entity.
		/// </summary>
		/// <param name="name"> the name to be associated with the entity </param>
		public SimEntity(string name)
		{
			if (name.IndexOf(" ", StringComparison.Ordinal) != -1)
			{
				throw new System.ArgumentException("Entity names can't contain spaces.");
			}
			this.name = name;
			id = -1;
			state = RUNNABLE;
			CloudSim.addEntity(this);
		}

		/// <summary>
		/// Gets the name of this entity.
		/// </summary>
		/// <returns> The entity's name </returns>
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		/// <summary>
		/// Gets the unique id number assigned to this entity.
		/// </summary>
		/// <returns> The id number </returns>
		public virtual int Id
		{
			get
			{
				return id;
			}
			set
			{
				this.id = value;
			}
		}

		// The schedule functions

		/// <summary>
		/// Sends an event to another entity by id number, with data. Note that the tag <code>9999</code>
		/// is reserved.
		/// </summary>
		/// <param name="dest"> The unique id number of the destination entity </param>
		/// <param name="delay"> How long from the current simulation time the event should be sent </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		/// <param name="data"> The data to be sent with the event. </param>
		public virtual void schedule(int dest, double delay, int tag, object data)
		{
			if (!CloudSim.running())
			{
				return;
			}
			CloudSim.send(id, dest, delay, tag, data);
		}

		/// <summary>
		/// Sends an event to another entity by id number and with <b>no</b> data. Note that the tag
		/// <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The unique id number of the destination entity </param>
		/// <param name="delay"> How long from the current simulation time the event should be sent </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		public virtual void schedule(int dest, double delay, int tag)
		{
			schedule(dest, delay, tag, null);
		}

		/// <summary>
		/// Sends an event to another entity through a port with a given name, with data. Note that the
		/// tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The name of the port to send the event through </param>
		/// <param name="delay"> How long from the current simulation time the event should be sent </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		/// <param name="data"> The data to be sent with the event. </param>
		public virtual void schedule(string dest, double delay, int tag, object data)
		{
			schedule(CloudSim.getEntityId(dest), delay, tag, data);
		}

		/// <summary>
		/// Sends an event to another entity through a port with a given name, with <b>no</b> data. Note
		/// that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The name of the port to send the event through </param>
		/// <param name="delay"> How long from the current simulation time the event should be sent </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		public virtual void schedule(string dest, double delay, int tag)
		{
			schedule(dest, delay, tag, null);
		}

		/// <summary>
		/// Sends an event to another entity by id number, with data
		/// but no delay. Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The unique id number of the destination entity </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		/// <param name="data"> The data to be sent with the event. </param>
		public virtual void scheduleNow(int dest, int tag, object data)
		{
			schedule(dest, 0, tag, data);
		}

		/// <summary>
		/// Sends an event to another entity by id number and with <b>no</b> data
		/// and no delay. Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The unique id number of the destination entity </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		public virtual void scheduleNow(int dest, int tag)
		{
			schedule(dest, 0, tag, null);
		}

		/// <summary>
		/// Sends an event to another entity through a port with a given name, with data
		/// but no delay. Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The name of the port to send the event through </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		/// <param name="data"> The data to be sent with the event. </param>
		public virtual void scheduleNow(string dest, int tag, object data)
		{
			schedule(CloudSim.getEntityId(dest), 0, tag, data);
		}

		/// <summary>
		/// Send an event to another entity through a port with a given name, with <b>no</b> data
		/// and no delay. 
		/// Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The name of the port to send the event through </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		public virtual void scheduleNow(string dest, int tag)
		{
			schedule(dest, 0, tag, null);
		}

		/// <summary>
		/// Sends a high priority event to another entity by id number, with data. 
		/// Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The unique id number of the destination entity </param>
		/// <param name="delay"> How long from the current simulation time the event should be sent </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		/// <param name="data"> The data to be sent with the event. </param>
		public virtual void scheduleFirst(int dest, double delay, int tag, object data)
		{
			if (!CloudSim.running())
			{
				return;
			}
			CloudSim.sendFirst(id, dest, delay, tag, data);
		}

		/// <summary>
		/// Sends a high priority event to another entity by id number and with <b>no</b> data. 
		/// Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The unique id number of the destination entity </param>
		/// <param name="delay"> How long from the current simulation time the event should be sent </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		public virtual void scheduleFirst(int dest, double delay, int tag)
		{
			scheduleFirst(dest, delay, tag, null);
		}

		/// <summary>
		/// Sends a high priority event to another entity through a port with a given name, with data.
		/// Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The name of the port to send the event through </param>
		/// <param name="delay"> How long from the current simulation time the event should be sent </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		/// <param name="data"> The data to be sent with the event. </param>
		public virtual void scheduleFirst(string dest, double delay, int tag, object data)
		{
			scheduleFirst(CloudSim.getEntityId(dest), delay, tag, data);
		}

		/// <summary>
		/// Sends a high priority event to another entity through a port with a given name, with <b>no</b>
		/// data. Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The name of the port to send the event through </param>
		/// <param name="delay"> How long from the current simulation time the event should be sent </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		public virtual void scheduleFirst(string dest, double delay, int tag)
		{
			scheduleFirst(dest, delay, tag, null);
		}

		/// <summary>
		/// Sends a high priority event to another entity by id number, with data
		/// and no delay. 
		/// Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The unique id number of the destination entity </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		/// <param name="data"> The data to be sent with the event. </param>
		public virtual void scheduleFirstNow(int dest, int tag, object data)
		{
			scheduleFirst(dest, 0, tag, data);
		}

		/// <summary>
		/// Sends a high priority event to another entity by id number and with <b>no</b> data
		/// and no delay. 
		/// Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The unique id number of the destination entity </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		public virtual void scheduleFirstNow(int dest, int tag)
		{
			scheduleFirst(dest, 0, tag, null);
		}

		/// <summary>
		/// Sends a high priority event to another entity through a port with a given name, with data
		/// and no delay.
		/// Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The name of the port to send the event through </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		/// <param name="data"> The data to be sent with the event. </param>
		public virtual void scheduleFirstNow(string dest, int tag, object data)
		{
			scheduleFirst(CloudSim.getEntityId(dest), 0, tag, data);
		}

		/// <summary>
		/// Sends a high priority event to another entity through a port with a given name, with <b>no</b>
		/// data and no delay. Note that the tag <code>9999</code> is reserved.
		/// </summary>
		/// <param name="dest"> The name of the port to send the event through </param>
		/// <param name="tag"> An user-defined number representing the type of event. </param>
		public virtual void scheduleFirstNow(string dest, int tag)
		{
			scheduleFirst(dest, 0, tag, null);
		}

		/// <summary>
		/// Sets the entity to be inactive for a time period.
		/// </summary>
		/// <param name="delay"> the time period for which the entity will be inactive </param>
		public virtual void pause(double delay)
		{
			if (delay < 0)
			{
				throw new System.ArgumentException("Negative delay supplied.");
			}
			if (!CloudSim.running())
			{
				return;
			}
			CloudSim.pause(id, delay);
		}

		/// <summary>
		/// Counts how many events matching a predicate are waiting in the entity's deferred queue.
		/// </summary>
		/// <param name="p"> The event selection predicate </param>
		/// <returns> The count of matching events </returns>
		public virtual int numEventsWaiting(Predicate p)
		{
			return CloudSim.waiting(id, p);
		}

		/// <summary>
		/// Counts how many events are waiting in the entity's deferred queue.
		/// </summary>
		/// <returns> The count of events </returns>
		public virtual int numEventsWaiting()
		{
			return CloudSim.waiting(id, CloudSim.SIM_ANY);
		}

		/// <summary>
		/// Extracts the first event matching a predicate waiting in the entity's deferred queue.
		/// </summary>
		/// <param name="p"> The event selection predicate </param>
		/// <returns> the simulation event </returns>
		public virtual SimEvent selectEvent(Predicate p)
		{
			if (!CloudSim.running())
			{
				return null;
			}

			return CloudSim.select(id, p);
		}

		/// <summary>
		/// Cancels the first event matching a predicate waiting in the entity's future queue.
		/// </summary>
		/// <param name="p"> The event selection predicate </param>
		/// <returns> The number of events cancelled (0 or 1) </returns>
		public virtual SimEvent cancelEvent(Predicate p)
		{
			if (!CloudSim.running())
			{
				return null;
			}

			return CloudSim.cancel(id, p);
		}

		/// <summary>
		/// Gets the first event matching a predicate from the deferred queue, or if none match, wait for
		/// a matching event to arrive.
		/// </summary>
		/// <param name="p"> The predicate to match </param>
		/// <returns> the simulation event </returns>
		public virtual SimEvent getNextEvent(Predicate p)
		{
			if (!CloudSim.running())
			{
				return null;
			}
			if (numEventsWaiting(p) > 0)
			{
				return selectEvent(p);
			}
			return null;
		}

		/// <summary>
		/// Waits for an event matching a specific predicate. This method does not check the entity's
		/// deferred queue.
		/// </summary>
		/// <param name="p"> The predicate to match </param>
		public virtual void waitForEvent(Predicate p)
		{
			if (!CloudSim.running())
			{
				return;
			}

            // TODO: Figure out how to implement this Wait call.
			//Monitor.Wait(CloudSim, TimeSpan.FromMilliseconds(id + p / 1000d));
			state = WAITING;
		}

		/// <summary>
		/// Gets the first event waiting in the entity's deferred queue, or if there are none, wait for an
		/// event to arrive.
		/// </summary>
		/// <returns> the simulation event </returns>
		public virtual SimEvent NextEvent
		{
			get
			{
				return getNextEvent(CloudSim.SIM_ANY);
			}
		}

		/// <summary>
		/// This method is invoked by the <seealso cref="CloudSim"/> class when the simulation is started. 
		/// It should be responsible for starting the entity up.
		/// </summary>
		public abstract void startEntity();

		/// <summary>
		/// Processes events or services that are available for the entity.
		/// This method is invoked by the <seealso cref="CloudSim"/> class whenever there is an event in the
		/// deferred queue, which needs to be processed by the entity.
		/// </summary>
		/// <param name="ev"> information about the event just happened
		/// 
		/// @pre ev != null
		/// @post $none </param>
		public abstract void processEvent(SimEvent ev);

		/// <summary>
		/// Shuts down the entity.
		/// This method is invoked by the <seealso cref="CloudSim"/> before the simulation finishes. If you want
		/// to save data in log files this is the method in which the corresponding code would be placed.
		/// </summary>
		public abstract void shutdownEntity();

        /// <summary>
        /// The run loop to process events fired during the simulation.
        /// The events that will be processed are defined
        /// in the <seealso cref="processEvent(org.cloudbus.cloudsim.core.SimEvent)"/> method.
        /// </summary>
        /// <seealso cref= "processEvent(org.cloudbus.cloudsim.core.SimEvent)"/>
        public virtual void run()
		{
			SimEvent ev = evbuf != null ? evbuf : NextEvent;

			while (ev != null)
			{
				processEvent(ev);
				if (state != RUNNABLE)
				{
					break;
				}

				ev = NextEvent;
			}

			evbuf = null;
		}

        /// <summary>
        /// Gets a clone of the entity. This is used when independent replications have been specified as
        /// an output analysis method. Clones or backups of the entities are made in the beginning of the
        /// simulation in order to reset the entities for each subsequent replication. This method should
        /// not be called by the user.
        /// </summary>
        /// <returns> A clone of the entity </returns>
        /// <exception cref="CloneNotSupportedException"> when the entity doesn't support cloning </exception>
        //protected internal override object clone()
        public object Clone()
        {
            SimEntity copy = (SimEntity)base.MemberwiseClone(); //.clone();
			copy.Name = name;
			copy.EventBuffer = null;
			return copy;
		}

		// Used to set a cloned entity's name

		// --------------- PACKAGE LEVEL METHODS ------------------

		/// <summary>
		/// Gets the entity state.
		/// </summary>
		/// <returns> the state </returns>
		protected internal virtual int State
		{
			get
			{
				return state;
			}
			set
			{
				this.state = value;
			}
		}

		/// <summary>
		/// Gets the event buffer.
		/// </summary>
		/// <returns> the event buffer </returns>
		protected internal virtual SimEvent EventBuffer
		{
			get
			{
				return evbuf;
			}
			set
			{
				evbuf = value;
			}
		}

		// The entity states
			//@todo The states should be an enum.
		/// <summary>
		/// The Constant RUNNABLE. </summary>
		public const int RUNNABLE = 0;

		/// <summary>
		/// The Constant WAITING. </summary>
		public const int WAITING = 1;

		/// <summary>
		/// The Constant HOLDING. </summary>
		public const int HOLDING = 2;

		/// <summary>
		/// The Constant FINISHED. </summary>
		public const int FINISHED = 3;




		// --------------- EVENT / MESSAGE SEND WITH NETWORK DELAY METHODS ------------------

		/// <summary>
		/// Sends an event/message to another entity by <tt>delaying</tt> the simulation time from the
		/// current time, with a tag representing the event type.
		/// </summary>
		/// <param name="entityId"> the id number of the destination entity </param>
		/// <param name="delay"> how long from the current simulation time the event should be sent. If delay is
		///            a negative number, then it will be changed to 0 </param>
		/// <param name="cloudSimTag"> an user-defined number representing the type of an event/message </param>
		/// <param name="data"> A reference to data to be sent with the event
		/// @pre entityID > 0
		/// @pre delay >= 0.0
		/// @pre data != null
		/// @post $none </param>
		protected internal virtual void send(int entityId, double delay, int cloudSimTag, object data)
		{
			if (entityId < 0)
			{
				return;
			}

			// if delay is -ve, then it doesn't make sense. So resets to 0.0
			if (delay < 0)
			{
				delay = 0;
			}

			if (double.IsInfinity(delay))
			{
				throw new System.ArgumentException("The specified delay is infinite value");
			}

			if (entityId < 0)
			{
				Log.printConcatLine(Name, ".send(): Error - " + "invalid entity id ", entityId);
				return;
			}

			int srcId = Id;
			if (entityId != srcId)
			{ // only delay messages between different entities
				delay += getNetworkDelay(srcId, entityId);
			}

			schedule(entityId, delay, cloudSimTag, data);
		}

		/// <summary>
		/// Sends an event/message to another entity by <tt>delaying</tt> the simulation time from the
		/// current time, with a tag representing the event type.
		/// </summary>
		/// <param name="entityId"> the id number of the destination entity </param>
		/// <param name="delay"> how long from the current simulation time the event should be sent. If delay is
		///            a negative number, then it will be changed to 0 </param>
		/// <param name="cloudSimTag"> an user-defined number representing the type of an event/message
		/// @pre entityID > 0
		/// @pre delay >= 0.0
		/// @post $none </param>
		protected internal virtual void send(int entityId, double delay, int cloudSimTag)
		{
			send(entityId, delay, cloudSimTag, null);
		}

		/// <summary>
		/// Sends an event/message to another entity by <tt>delaying</tt> the simulation time from the
		/// current time, with a tag representing the event type.
		/// </summary>
		/// <param name="entityName"> the name of the destination entity </param>
		/// <param name="delay"> how long from the current simulation time the event should be sent. If delay is
		///            a negative number, then it will be changed to 0 </param>
		/// <param name="cloudSimTag"> an user-defined number representing the type of an event/message </param>
		/// <param name="data"> A reference to data to be sent with the event
		/// @pre entityName != null
		/// @pre delay >= 0.0
		/// @pre data != null
		/// @post $none </param>
		protected internal virtual void send(string entityName, double delay, int cloudSimTag, object data)
		{
			send(CloudSim.getEntityId(entityName), delay, cloudSimTag, data);
		}

		/// <summary>
		/// Sends an event/message to another entity by <tt>delaying</tt> the simulation time from the
		/// current time, with a tag representing the event type.
		/// </summary>
		/// <param name="entityName"> the name of the destination entity </param>
		/// <param name="delay"> how long from the current simulation time the event should be sent. If delay is
		///            a negative number, then it will be changed to 0 </param>
		/// <param name="cloudSimTag"> an user-defined number representing the type of an event/message
		/// @pre entityName != null
		/// @pre delay >= 0.0
		/// @post $none </param>
		protected internal virtual void send(string entityName, double delay, int cloudSimTag)
		{
			send(entityName, delay, cloudSimTag, null);
		}

		/// <summary>
		/// Sends an event/message to another entity, with a tag representing the event type.
		/// </summary>
		/// <param name="entityId"> the id number of the destination entity </param>
		/// <param name="cloudSimTag"> an user-defined number representing the type of an event/message </param>
		/// <param name="data"> A reference to data to be sent with the event
		/// @pre entityID > 0
		/// @pre delay >= 0.0
		/// @pre data != null
		/// @post $none </param>
		protected internal virtual void sendNow(int entityId, int cloudSimTag, object data)
		{
			send(entityId, 0, cloudSimTag, data);
		}

		/// <summary>
		/// Sends an event/message to another entity, with a tag representing the event type.
		/// </summary>
		/// <param name="entityId"> the id number of the destination entity </param>
		/// <param name="cloudSimTag"> an user-defined number representing the type of an event/message
		/// @pre entityID > 0
		/// @pre delay >= 0.0
		/// @post $none </param>
		protected internal virtual void sendNow(int entityId, int cloudSimTag)
		{
			send(entityId, 0, cloudSimTag, null);
		}

		/// <summary>
		/// Sends an event/message to another entity, with a tag representing the event type.
		/// </summary>
		/// <param name="entityName"> the name of the destination entity </param>
		/// <param name="cloudSimTag"> an user-defined number representing the type of an event/message </param>
		/// <param name="data"> A reference to data to be sent with the event
		/// @pre entityName != null
		/// @pre delay >= 0.0
		/// @pre data != null
		/// @post $none </param>
		protected internal virtual void sendNow(string entityName, int cloudSimTag, object data)
		{
			send(CloudSim.getEntityId(entityName), 0, cloudSimTag, data);
		}

		/// <summary>
		/// Sends an event/message to another entity, with a tag representing the event type.
		/// </summary>
		/// <param name="entityName"> the name of the destination entity </param>
		/// <param name="cloudSimTag"> an user-defined number representing the type of an event/message
		/// @pre entityName != null
		/// @pre delay >= 0.0
		/// @post $none </param>
		protected internal virtual void sendNow(string entityName, int cloudSimTag)
		{
			send(entityName, 0, cloudSimTag, null);
		}

		/// <summary>
		/// Gets the network delay associated to the sent of a message from a given source to a given
		/// destination.
		/// </summary>
		/// <param name="src"> source of the message </param>
		/// <param name="dst"> destination of the message </param>
		/// <returns> delay to send a message from src to dst
		/// @pre src >= 0
		/// @pre dst >= 0 </returns>
		private double getNetworkDelay(int src, int dst)
		{
			if (NetworkTopology.NetworkEnabled)
			{
				return NetworkTopology.getDelay(src, dst);
			}
			return 0.0;
		}

	}

}