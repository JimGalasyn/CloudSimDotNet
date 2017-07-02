using System;
using System.Collections.Generic;
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
    using System.Diagnostics;
    using Predicate = org.cloudbus.cloudsim.core.predicates.Predicate;
    using PredicateAny = org.cloudbus.cloudsim.core.predicates.PredicateAny;
    using PredicateNone = org.cloudbus.cloudsim.core.predicates.PredicateNone;

    /// <summary>
    /// This class extends the CloudSimCore to enable network simulation in CloudSim. Also, it disables
    /// all the network models from CloudSim, to provide a simpler simulation of networking. In the
    /// network model used by CloudSim, a topology file written in BRITE format is used to describe the
    /// network. Later, nodes in such file are mapped to CloudSim entities. Delay calculated from the
    /// BRITE model are added to the messages send through CloudSim. Messages using the old model are
    /// converted to the apropriate methods with the correct parameters.
    /// 
    /// @author Rodrigo N. Calheiros
    /// @author Anton Beloglazov
    /// @since CloudSim Toolkit 1.0
    /// </summary>
    public class CloudSim
	{

		/// <summary>
		/// The Constant CLOUDSIM_VERSION_STRING. </summary>
		private const string CLOUDSIM_VERSION_STRING = "3.0";

		/// <summary>
		/// The id of CIS entity. </summary>
		private static int cisId = -1;

		/// <summary>
		/// The id of CloudSimShutdown entity. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private static int shutdownId = -1;
		private static int shutdownId = -1;

		/// <summary>
		/// The CIS object. </summary>
		private static CloudInformationService cis = null;

		/// <summary>
		/// The Constant NOT_FOUND. </summary>
		private const int NOT_FOUND = -1;

		/// <summary>
		/// The trace flag. </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unused") private static boolean traceFlag = false;
		private static bool traceFlag = false;

		/// <summary>
		/// The calendar. </summary>
		private static DateTime calendar = default(DateTime);

		/// <summary>
		/// The termination time. </summary>
		private static double terminateAt = -1;

		/// <summary>
		/// The minimal time between events. Events within shorter periods after the last simEvent are discarded. </summary>
		private static double minTimeBetweenEvents = 0.1;

		/// <summary>
		/// Initialises all the common attributes.
		/// </summary>
		/// <param name="_calendar"> the _calendar </param>
		/// <param name="_traceFlag"> the _trace flag </param>
		/// <param name="numUser"> number of users </param>
		/// <exception cref="Exception"> This happens when creating this entity before initialising CloudSim package
		///             or this entity name is <tt>null</tt> or empty
		/// @pre $none
		/// @post $none </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void initCommonVariable(java.util.Calendar _calendar, boolean _traceFlag, int numUser) throws Exception
		private static void initCommonVariable(DateTime _calendar, bool _traceFlag, int numUser)
		{
			initialize();
			// NOTE: the order for the below 3 lines are important
			traceFlag = _traceFlag;

			// Set the current Wall clock time as the starting time of
			// simulation
			if (_calendar == null)
			{
				calendar = new DateTime();
			}
			else
			{
				calendar = _calendar;
			}

			// creates a CloudSimShutdown object
			CloudSimShutdown shutdown = new CloudSimShutdown("CloudSimShutdown", numUser);
			shutdownId = shutdown.Id;
		}

		/// <summary>
		/// Initialises CloudSim parameters. This method should be called before creating any entities.
		/// <para>
		/// Inside this method, it will create the following CloudSim entities:
		/// <ul>
		/// <li>CloudInformationService.
		/// <li>CloudSimShutdown
		/// </ul>
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="numUser"> the number of User Entities created. This parameters indicates that
		///            <seealso cref="gridsim.CloudSimShutdown"/> first waits for all user entities's
		///            END_OF_SIMULATION signal before issuing terminate signal to other entities </param>
		/// <param name="cal"> starting time for this simulation. If it is <tt>null</tt>, then the time will be
		///            taken from <tt>Calendar.getInstance()</tt> </param>
		/// <param name="traceFlag"> <tt>true</tt> if CloudSim trace need to be written </param>
		/// <seealso cref= gridsim.CloudSimShutdown </seealso>
		/// <seealso cref= CloudInformationService.CloudInformationService
		/// @pre numUser >= 0
		/// @post $none </seealso>
		public static void init(int numUser, DateTime cal, bool traceFlag)
		{
			try
			{
				initCommonVariable(cal, traceFlag, numUser);

				// create a GIS object
				cis = new CloudInformationService("CloudInformationService");

				// set all the above entity IDs
				cisId = cis.Id;
			}
			catch (System.ArgumentException s)
			{
				Log.printLine("CloudSim.init(): The simulation has been terminated due to an unexpected error");
				Log.printLine(s.Message);
			}
			catch (Exception e)
			{
				Log.printLine("CloudSim.init(): The simulation has been terminated due to an unexpected error");
				Log.printLine(e.Message);
			}
		}

		/// <summary>
		/// Initialises CloudSim parameters. This method should be called before creating any entities.
		/// <para>
		/// Inside this method, it will create the following CloudSim entities:
		/// <ul>
		/// <li>CloudInformationService.
		/// <li>CloudSimShutdown
		/// </ul>
		/// </para>
		/// <para>
		/// 
		/// </para>
		/// </summary>
		/// <param name="numUser"> the number of User Entities created. This parameters indicates that
		///            <seealso cref="gridsim.CloudSimShutdown"/> first waits for all user entities's
		///            END_OF_SIMULATION signal before issuing terminate signal to other entities </param>
		/// <param name="cal"> starting time for this simulation. If it is <tt>null</tt>, then the time will be
		///            taken from <tt>Calendar.getInstance()</tt> </param>
		/// <param name="traceFlag"> <tt>true</tt> if CloudSim trace need to be written </param>
		/// <param name="periodBetweenEvents"> - the minimal period between events. Events within shorter periods
		/// after the last simEvent are discarded. </param>
		/// <seealso cref= gridsim.CloudSimShutdown </seealso>
		/// <seealso cref= CloudInformationService.CloudInformationService
		/// @pre numUser >= 0
		/// @post $none </seealso>
		public static void init(int numUser, DateTime cal, bool traceFlag, double periodBetweenEvents)
		{
			if (periodBetweenEvents <= 0)
			{
			throw new System.ArgumentException("The minimal time between events should be positive, but is:" + periodBetweenEvents);
			}

			init(numUser, cal, traceFlag);
			minTimeBetweenEvents = periodBetweenEvents;
		}



		/// <summary>
		/// Starts the execution of CloudSim simulation. It waits for complete execution of all entities,
		/// i.e. until all entities threads reach non-RUNNABLE state or there are no more events in the
		/// future simEvent queue.
		/// <para>
		/// <b>Note</b>: This method should be called after all the entities have been setup and added.
		/// 
		/// </para>
		/// </summary>
		/// <returns> the last clock time </returns>
		/// <exception cref="NullPointerException"> This happens when creating this entity before initialising
		///             CloudSim package or this entity name is <tt>null</tt> or empty. </exception>
		/// <seealso cref= gridsim.CloudSim#init(int, Calendar, boolean)
		/// @pre $none
		/// @post $none </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static double startSimulation() throws NullPointerException
		public static double startSimulation()
		{
			Log.printConcatLine("Starting CloudSim version ", CLOUDSIM_VERSION_STRING);
			try
			{
				double clock = run();

				// reset all static variables
				cisId = -1;
				shutdownId = -1;
				cis = null;
				calendar = default(DateTime);
				traceFlag = false;

				return clock;
			}
			catch (System.ArgumentException e)
			{
				Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
				throw new System.NullReferenceException("CloudSim.startCloudSimulation() :" + " Error - you haven't initialized CloudSim.");
			}
		}

		/// <summary>
		/// Stops Cloud Simulation (based on <seealso cref="Simulation#runStop()"/>). This should be only called if
		/// any of the user defined entities <b>explicitly</b> want to terminate simulation during
		/// execution.
		/// </summary>
		/// <exception cref="NullPointerException"> This happens when creating this entity before initialising
		///             CloudSim package or this entity name is <tt>null</tt> or empty </exception>
		/// <seealso cref= gridsim.CloudSim#init(int, Calendar, boolean) </seealso>
		/// <seealso cref= Simulation#runStop()
		/// @pre $none
		/// @post $none </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void stopSimulation() throws NullPointerException
		public static void stopSimulation()
		{
			try
			{
				runStop();
			}
			catch (System.ArgumentException)
			{
				throw new System.NullReferenceException("CloudSim.stopCloudSimulation() : " + "Error - can't stop Cloud Simulation.");
			}
		}

		/// <summary>
		/// This method is called if one wants to terminate the simulation.
		/// </summary>
		/// <returns> true, if successful; false otherwise. </returns>
		public static bool terminateSimulation()
		{
			running_Renamed = false;
			printMessage("Simulation: Reached termination time.");
			return true;
		}

		/// <summary>
		/// This method is called if one wants to terminate the simulation at a given time.
		/// </summary>
		/// <param name="time"> the time at which the simulation has to be terminated </param>
		/// <returns> true, if successful otherwise. </returns>
		public static bool terminateSimulation(double time)
		{
			if (time <= clock_Renamed)
			{
				return false;
			}
			else
			{
				terminateAt = time;
			}
			return true;
		}


		/// <summary>
		/// Returns the minimum time between events. Events within shorter periods after the last simEvent are discarded. </summary>
		/// <returns> the minimum time between events. </returns>
		public static double MinTimeBetweenEvents
		{
			get
			{
				return minTimeBetweenEvents;
			}
		}

		/// <summary>
		/// Gets a new copy of initial simulation Calendar.
		/// </summary>
		/// <returns> a new copy of Calendar object or if CloudSim hasn't been initialized </returns>
		/// <seealso cref= gridsim.CloudSim#init(int, Calendar, boolean, String[], String[], String) </seealso>
		/// <seealso cref= gridsim.CloudSim#init(int, Calendar, boolean)
		/// @pre $none
		/// @post $none </seealso>
		public static DateTime SimulationCalendar
		{
			get
			{
				// make a new copy
				DateTime clone = calendar;
				if (calendar != null)
				{
                    //clone = (DateTime) calendar.clone();
                    // TEST: (fixed) Unnecessary, DateTime is immutable
                    clone = calendar;
                }
    
				return clone;
			}
		}

		/// <summary>
		/// Gets the entity ID of <tt>CloudInformationService</tt>.
		/// </summary>
		/// <returns> the Entity ID or if it is not found
		/// @pre $none
		/// @post $result >= -1 </returns>
		public static int CloudInfoServiceEntityId
		{
			get
			{
				return cisId;
			}
		}

		/// <summary>
		/// Sends a request to Cloud Information Service (CIS) entity to get the list of all Cloud
		/// hostList.
		/// </summary>
		/// <returns> A List containing CloudResource ID (as an Integer object) or if a CIS entity hasn't
		///         been created before
		/// @pre $none
		/// @post $none </returns>
		public static IList<int?> CloudResourceList
		{
			get
			{
				if (cis == null)
				{
					return null;
				}
    
				return cis.List;
			}
		}

		// ======== SIMULATION METHODS ===============//

		/// <summary>
		/// The entities. </summary>
		private static IList<SimEntity> entities;

		/// <summary>
		/// The future simEvent queue. </summary>
		protected internal static FutureQueue future;

		/// <summary>
		/// The deferred simEvent queue. </summary>
		protected internal static DeferredQueue deferred;

		/// <summary>
		/// The current simulation clock.
		/// </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private static double clock_Renamed;

		/// <summary>
		/// Flag for checking if the simulation is running. </summary>
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
		private static bool running_Renamed;

		/// <summary>
		/// The entities by name. </summary>
		private static IDictionary<string, SimEntity> entitiesByName;

		// The predicates used in entity wait methods
		/// <summary>
		/// The wait predicates. </summary>
		private static IDictionary<int?, Predicate> waitPredicates;

		/// <summary>
		/// The paused. </summary>
		private static bool paused = false;

		/// <summary>
		/// The pause at. </summary>
		private static long pauseAt = -1;

		/// <summary>
		/// The abrupt terminate. </summary>
		private static bool abruptTerminate = false;

		/// <summary>
		/// Initialise the simulation for stand alone simulations. This function should be called at the
		/// start of the simulation.
		/// </summary>
		protected internal static void initialize()
		{
			Log.printLine("Initialising...");
			entities = new List<SimEntity>();
            // TEST: (fixed) Dictionary == LinkedHashMap?
            entitiesByName = new Dictionary<string, SimEntity>(); // new LinkedHashMap<string, SimEntity>();
            future = new FutureQueue();
			deferred = new DeferredQueue();
			waitPredicates = new Dictionary<int?, Predicate>();
			clock_Renamed = 0;
			running_Renamed = false;
		}

		// The two standard predicates

		/// <summary>
		/// A standard predicate that matches any simEvent. </summary>
		public static readonly PredicateAny SIM_ANY = new PredicateAny();

		/// <summary>
		/// A standard predicate that does not match any events. </summary>
		public static readonly PredicateNone SIM_NONE = new PredicateNone();

		// Public access methods

		/// <summary>
		/// Get the current simulation time.
		/// </summary>
		/// <returns> the simulation time </returns>
		public static double clock()
		{
			return clock_Renamed;
		}

		/// <summary>
		/// Get the current number of entities in the simulation.
		/// </summary>
		/// <returns> The number of entities </returns>
		public static int NumEntities
		{
			get
			{
				return entities.Count;
			}
		}

		/// <summary>
		/// Get the entity with a given id.
		/// </summary>
		/// <param name="id"> the entity's unique id number </param>
		/// <returns> The entity, or if it could not be found </returns>
		public static SimEntity getEntity(int id)
		{
			return entities[id];
		}

		/// <summary>
		/// Get the entity with a given name.
		/// </summary>
		/// <param name="name"> The entity's name </param>
		/// <returns> The entity </returns>
		public static SimEntity getEntity(string name)
		{
			return entitiesByName[name];
		}

		/// <summary>
		/// Get the id of an entity with a given name.
		/// </summary>
		/// <param name="name"> The entity's name </param>
		/// <returns> The entity's unique id number </returns>
		public static int getEntityId(string name)
		{
            //SimEntity obj = entitiesByName[name];
            SimEntity obj = null;

            if(!String.IsNullOrEmpty(name) && entitiesByName.ContainsKey(name))
            {
                obj = entitiesByName[name];
            }

            if (obj == null)
			{
				return NOT_FOUND;
			}
			else
			{
				return obj.Id;
			}
		}

		/// <summary>
		/// Gets name of the entity given its entity ID.
		/// </summary>
		/// <param name="entityID"> the entity ID </param>
		/// <returns> the Entity name or if this object does not have one
		/// @pre entityID > 0
		/// @post $none </returns>
		public static string getEntityName(int entityID)
		{
			try
			{
				return getEntity(entityID).Name;
			}
			catch (System.ArgumentException)
			{
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets name of the entity given its entity ID.
		/// </summary>
		/// <param name="entityID"> the entity ID </param>
		/// <returns> the Entity name or if this object does not have one
		/// @pre entityID > 0
		/// @post $none </returns>
		public static string getEntityName(int? entityID)
		{
			if (entityID != null)
			{
				return getEntityName(entityID.Value);
			}
			return null;
		}

		/// <summary>
		/// Returns a list of entities created for the simulation.
		/// </summary>
		/// <returns> the entity iterator </returns>
		public static IList<SimEntity> EntityList
		{
			get
			{
				// create a new list to prevent the user from changing
				// the list of entities used by Simulation
				IList<SimEntity> list = new List<SimEntity>();
				((List<SimEntity>)list).AddRange(entities);
				return list;
			}
		}

		// Public update methods

		/// <summary>
		/// Add a new entity to the simulation. This is present for compatibility with existing
		/// simulations since entities are automatically added to the simulation upon instantiation.
		/// </summary>
		/// <param name="e"> The new entity </param>
		public static void addEntity(SimEntity e)
		{
			SimEvent evt;
			if (running_Renamed)
			{
				// Post an simEvent to make this entity
				evt = new SimEvent(SimEvent.CREATE, clock_Renamed, 1, 0, 0, e);
				future.addEvent(evt);
			}
			if (e.Id == -1)
			{ // Only add once!
				int id = entities.Count;
				e.Id = id;
				entities.Add(e);
				entitiesByName[e.Name] = e;
			}
		}

		/// <summary>
		/// Internal method used to add a new entity to the simulation when the simulation is running. It
		/// should <b>not</b> be called from user simulations.
		/// </summary>
		/// <param name="e"> The new entity </param>
		protected internal static void addEntityDynamically(SimEntity e)
		{
			if (e == null)
			{
				throw new System.ArgumentException("Adding null entity.");
			}
			else
			{
				printMessage("Adding: " + e.Name);
			}
			e.startEntity();
		}

		/// <summary>
		/// Internal method used to run one tick of the simulation. This method should <b>not</b> be
		/// called in simulations.
		/// </summary>
		/// <returns> true, if successful otherwise
		/// @todo If the method shouldn't be called by the user,
		/// it should be protected in any way, such as changing
		/// its visibility to package. </returns>
		public static bool runClockTick()
		{
			SimEntity ent;
			bool queue_empty;

			int entities_size = entities.Count;

			for (int i = 0; i < entities_size; i++)
			{
				ent = entities[i];
				if (ent.State == SimEntity.RUNNABLE)
				{
					ent.run();
				}
			}

            // If there are more future events then deal with them
            //if (future.size() > 0)
            if(future.Count > 0)
            {
				IList<SimEvent> toRemove = new List<SimEvent>();
                IEnumerator<SimEvent> fit = future.iterator();
                queue_empty = false;
                //SimEvent first = fit.next();
                fit.MoveNext();
                SimEvent first = fit.Current;
                processEvent(first);
				future.remove(first);

                fit = future.iterator();

                // Check if next events are at same time...
                //bool trymore = fit.hasNext();
                bool trymore = fit.MoveNext();
                while (trymore)
                {
                    SimEvent next = fit.Current; //fit.next();

                    if (next.eventTime() == first.eventTime())
					{
						processEvent(next);
						toRemove.Add(next);
                        //JAVA TO C# CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
                        //trymore = fit.hasNext();
                        trymore = fit.MoveNext();
                    }
					else
					{
						trymore = false;
					}
				}

				future.removeAll(toRemove);
			}
			else
			{
				queue_empty = true;
				running_Renamed = false;
				printMessage("Simulation: No more future events");
			}

			return queue_empty;
		}

		/// <summary>
		/// Internal method used to stop the simulation. This method should <b>not</b> be used directly.
		/// </summary>
		public static void runStop()
		{
			printMessage("Simulation completed.");
		}

		/// <summary>
		/// Used to hold an entity for some time.
		/// </summary>
		/// <param name="src"> the src </param>
		/// <param name="delay"> the delay </param>
		public static void hold(int src, long delay)
		{
			SimEvent e = new SimEvent(SimEvent.HOLD_DONE, clock_Renamed + delay, src);
			future.addEvent(e);
			entities[src].State = SimEntity.HOLDING;
		}

		/// <summary>
		/// Used to pause an entity for some time.
		/// </summary>
		/// <param name="src"> the src </param>
		/// <param name="delay"> the delay </param>
		public static void pause(int src, double delay)
		{
			SimEvent e = new SimEvent(SimEvent.HOLD_DONE, clock_Renamed + delay, src);
			future.addEvent(e);
			entities[src].State = SimEntity.HOLDING;
		}

		/// <summary>
		/// Used to send an simEvent from one entity to another.
		/// </summary>
		/// <param name="src"> the src </param>
		/// <param name="dest"> the dest </param>
		/// <param name="delay"> the delay </param>
		/// <param name="tag"> the tag </param>
		/// <param name="data"> the data </param>
		public static void send(int src, int dest, double delay, int tag, object data)
		{
			if (delay < 0)
			{
				throw new System.ArgumentException("Send delay can't be negative.");
			}

			SimEvent e = new SimEvent(SimEvent.SEND, clock_Renamed + delay, src, dest, tag, data);
			future.addEvent(e);
		}

		/// <summary>
		/// Used to send an simEvent from one entity to another, with priority in the queue.
		/// </summary>
		/// <param name="src"> the src </param>
		/// <param name="dest"> the dest </param>
		/// <param name="delay"> the delay </param>
		/// <param name="tag"> the tag </param>
		/// <param name="data"> the data </param>
		public static void sendFirst(int src, int dest, double delay, int tag, object data)
		{
			if (delay < 0)
			{
				throw new System.ArgumentException("Send delay can't be negative.");
			}

			SimEvent e = new SimEvent(SimEvent.SEND, clock_Renamed + delay, src, dest, tag, data);
			future.addEventFirst(e);
		}

		/// <summary>
		/// Sets an entity's state to be waiting. The predicate used to wait for an simEvent is now passed
		/// to Sim_system. Only events that satisfy the predicate will be passed to the entity. This is
		/// done to avoid unnecessary context switches.
		/// </summary>
		/// <param name="src"> the src </param>
		/// <param name="p"> the p </param>
		public static void wait(int src, Predicate p)
		{
			entities[src].State = SimEntity.WAITING;
			if (p != SIM_ANY)
			{
				// If a predicate has been used store it in order to check it
				waitPredicates[src] = p;
			}
		}

		/// <summary>
		/// Checks if events for a specific entity are present in the deferred simEvent queue.
		/// </summary>
		/// <param name="d"> the d </param>
		/// <param name="p"> the p </param>
		/// <returns> the int </returns>
		public static int waiting(int d, Predicate p)
		{
			int count = 0;
			SimEvent simEvent;
            IEnumerator<SimEvent> iterator = deferred.iterator();
            while (iterator.MoveNext())
			{
				simEvent = iterator.Current;
				if ((simEvent.Destination == d) && (p.match(simEvent)))
				{
					count++;
				}
			}
			return count;
		}

		/// <summary>
		/// Selects an simEvent matching a predicate.
		/// </summary>
		/// <param name="src"> the src </param>
		/// <param name="p"> the p </param>
		/// <returns> the sim simEvent </returns>
		public static SimEvent select(int src, Predicate p)
		{
			SimEvent ev = null;
            IEnumerator<SimEvent> iterator = deferred.iterator();
            while (iterator.MoveNext())
			{
				ev = iterator.Current;
				if (ev.Destination == src && p.match(ev))
				{
                    // TODO: TEST (fixed) Does this iterator.remove() substitute work?
                    //iterator.remove();
                    deferred.removeEvent(iterator.Current);
                    break;
				}
			}
			return ev;
		}

		/// <summary>
		/// Find first deferred simEvent matching a predicate.
		/// </summary>
		/// <param name="src"> the src </param>
		/// <param name="p"> the p </param>
		/// <returns> the sim simEvent </returns>
		public static SimEvent findFirstDeferred(int src, Predicate p)
		{
			SimEvent ev = null;
			IEnumerator<SimEvent> iterator = deferred.iterator();
			while (iterator.MoveNext())
			{
				ev = iterator.Current;
				if (ev.Destination == src && p.match(ev))
				{
					break;
				}
			}
			return ev;
		}

		/// <summary>
		/// Removes an simEvent from the simEvent queue.
		/// </summary>
		/// <param name="src"> the src </param>
		/// <param name="p"> the p </param>
		/// <returns> the sim simEvent </returns>
		public static SimEvent cancel(int src, Predicate p)
		{
			SimEvent ev = null;
			IEnumerator<SimEvent> iter = future.iterator();
			while (iter.MoveNext())
			{
				ev = iter.Current;
				if (ev.Source == src && p.match(ev))
				{
                    //JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
                    // TEST: (fixed) Does this work?
                    //iter.remove();
                    future.remove(iter.Current);
                    break;
				}
			}

			return ev;
		}

		/// <summary>
		/// Removes all events that match a given predicate from the future simEvent queue returns true if
		/// at least one simEvent has been cancelled; false otherwise.
		/// </summary>
		/// <param name="src"> the src </param>
		/// <param name="p"> the p </param>
		/// <returns> true, if successful </returns>
		public static bool cancelAll(int src, Predicate p)
		{
			SimEvent ev = null;
			int previousSize = future.size();
			IEnumerator<SimEvent> iter = future.iterator();
			while (iter.MoveNext())
			{
				ev = iter.Current;
				if (ev.Source == src && p.match(ev))
				{
                    //JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
                    // TEST: (fixed) Does this work?
                    //iter.remove();
                    future.remove(iter.Current);

                }
			}
			return previousSize < future.size();
		}

		//
		// Private internal methods
		//

		/// <summary>
		/// Processes an simEvent.
		/// </summary>
		/// <param name="e"> the e </param>
		private static void processEvent(SimEvent e)
		{
			int dest, src;
			SimEntity dest_ent;
			// Update the system's clock
			if (e.eventTime() < clock_Renamed)
			{
				throw new System.ArgumentException("Past simEvent detected.");
			}
			clock_Renamed = e.eventTime();

			// Ok now process it
			switch (e.Type)
			{
				case SimEvent.ENULL:
					throw new System.ArgumentException("Event has a null type.");

				case SimEvent.CREATE:
					SimEntity newe = (SimEntity) e.Data;
					addEntityDynamically(newe);
					break;

				case SimEvent.SEND:
					// Check for matching wait
					dest = e.Destination;
					if (dest < 0)
					{
						throw new System.ArgumentException("Attempt to send to a null entity detected.");
					}
					else
					{
						int tag = e.Tag;
						dest_ent = entities[dest];
						if (dest_ent.State == SimEntity.WAITING)
						{
							int? destObj = Convert.ToInt32(dest);
							Predicate p = waitPredicates[destObj];
							if ((p == null) || (tag == 9999) || (p.match(e)))
							{
								dest_ent.EventBuffer = (SimEvent) e.Clone();
								dest_ent.State = SimEntity.RUNNABLE;
								waitPredicates.Remove(destObj);
							}
							else
							{
								deferred.addEvent(e);
							}
						}
						else
						{
							deferred.addEvent(e);
						}
					}
					break;

				case SimEvent.HOLD_DONE:
					src = e.Source;
					if (src < 0)
					{
						throw new System.ArgumentException("Null entity holding.");
					}
					else
					{
						entities[src].State = SimEntity.RUNNABLE;
					}
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Internal method used to start the simulation. This method should <b>not</b> be used by user
		/// simulations.
		/// </summary>
		public static void runStart()
		{
			running_Renamed = true;
			// Start all the entities
			foreach (SimEntity ent in entities)
			{
				ent.startEntity();
			}

			printMessage("Entities started.");
		}

		/// <summary>
		/// Check if the simulation is still running. This method should be used by entities to check if
		/// they should continue executing.
		/// </summary>
		/// <returns> if the simulation is still running, otherwise </returns>
		public static bool running()
		{
			return running_Renamed;
		}

		/// <summary>
		/// This method is called if one wants to pause the simulation.
		/// </summary>
		/// <returns> true, if successful otherwise. </returns>
		public static bool pauseSimulation()
		{
			paused = true;
			return paused;
		}

		/// <summary>
		/// This method is called if one wants to pause the simulation at a given time.
		/// </summary>
		/// <param name="time"> the time at which the simulation has to be paused </param>
		/// <returns> true, if successful otherwise. </returns>
		public static bool pauseSimulation(long time)
		{
			if (time <= clock_Renamed)
			{
				return false;
			}
			else
			{
				pauseAt = time;
			}
			return true;
		}

		/// <summary>
		/// This method is called if one wants to resume the simulation that has previously been paused.
		/// </summary>
		/// <returns> if the simulation has been restarted or or otherwise. </returns>
		public static bool resumeSimulation()
		{
			paused = false;

			if (pauseAt <= clock_Renamed)
			{
				pauseAt = -1;
			}

			return !paused;
		}

		/// <summary>
		/// Start the simulation running. This should be called after all the entities have been setup
		/// and added, and their ports linked.
		/// </summary>
		/// <returns> the last clock value </returns>
		public static double run()
		{
			if (!running_Renamed)
			{
				runStart();
			}

            // TEST: (fixed) Does this iterator work?
            var futureEnumerator = future.iterator();

            while (true)
			{
				if (runClockTick() || abruptTerminate)
				{
					break;
				}

				// this block allows termination of simulation at a specific time
				if (terminateAt > 0.0 && clock_Renamed >= terminateAt)
				{
					terminateSimulation();
					clock_Renamed = terminateAt;
					break;
				}

                // TODO: This iterator biz won't work.
                //if (pauseAt != -1 && ((future.size() > 0 && clock_Renamed <= pauseAt && pauseAt <= future.GetEnumerator().next().eventTime()) || future.size() == 0 && pauseAt <= clock_Renamed))                
                if (pauseAt != -1 && ((future.size() > 0 && clock_Renamed <= pauseAt && pauseAt <= futureEnumerator.Current.eventTime()) || future.size() == 0 && pauseAt <= clock_Renamed))
                {
					pauseSimulation();
					clock_Renamed = pauseAt;
				}

				while (paused)
				{
					//try
					//{
                        // TODO: Probably want to handle this thread business differently.
						//Thread.Sleep(100);
					//}
					//catch (InterruptedException e)
					//{
					//Debug.WriteLine(e.ToString());
                    //Debug.WriteLine(e.StackTrace);
					//}
				}
			}

			double clock = CloudSim.clock();

			finishSimulation();
			runStop();

			return clock;
		}

		/// <summary>
		/// Internal method that allows the entities to terminate. This method should <b>not</b> be used
		/// in user simulations.
		/// </summary>
		public static void finishSimulation()
		{
			// Allow all entities to exit their body method
			if (!abruptTerminate)
			{
				foreach (SimEntity ent in entities)
				{
					if (ent.State != SimEntity.FINISHED)
					{
						ent.run();
					}
				}
			}

			foreach (SimEntity ent in entities)
			{
				ent.shutdownEntity();
			}

			// reset all static variables
			// Private data members
			entities = null;
			entitiesByName = null;
			future = null;
			deferred = null;
			clock_Renamed = 0L;
			running_Renamed = false;

			waitPredicates = null;
			paused = false;
			pauseAt = -1;
			abruptTerminate = false;
		}

		/// <summary>
		/// Abruptally terminate.
		/// </summary>
		public static void abruptallyTerminate()
		{
			abruptTerminate = true;
		}

		/// <summary>
		/// Prints a message about the progress of the simulation.
		/// </summary>
		/// <param name="message"> the message </param>
		private static void printMessage(string message)
		{
			Log.printLine(message);
		}

		/// <summary>
		/// Checks if is paused.
		/// </summary>
		/// <returns> true, if is paused </returns>
		public static bool Paused
		{
			get
			{
				return paused;
			}
		}

	}

}