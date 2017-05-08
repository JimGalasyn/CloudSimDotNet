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
	/// A Cloud Information Service (CIS) is an entity that provides cloud resource registration,
	/// indexing and discovery services. The Cloud hostList tell their readiness to process Cloudlets by
	/// registering themselves with this entity. Other entities such as the resource broker can contact
	/// this class for resource discovery service, which returns a list of registered resource IDs. In
	/// summary, it acts like a yellow page service. This class will be created by CloudSim upon
	/// initialisation of the simulation. Hence, do not need to worry about creating an object of this
	/// class.
	/// 
	/// @author Manzur Murshed
	/// @author Rajkumar Buyya
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class CloudInformationService : SimEntity
	{

		/// <summary>
		/// A list containing the id of all entities that are registered at the 
		/// Cloud Information Service (CIS). 
		/// @todo It is not clear if this list is a list of host id's or datacenter id's.
		/// The previous attribute documentation just said "For all types of hostList".
		/// It can be seen at the method <seealso cref="#processEvent(org.cloudbus.cloudsim.core.SimEvent)"/>
		/// that the list is updated when a CloudSimTags.REGISTER_RESOURCE event
		/// is received. However, only the Datacenter class sends and event
		/// of this type, including its id as parameter.
		/// 
		/// </summary>
		private readonly IList<int?> resList;

		/// <summary>
		/// A list containing only the id of entities with Advanced Reservation feature
		/// that are registered at the CIS. 
		/// </summary>
		private readonly IList<int?> arList;

		/// <summary>
		/// List of all regional CIS. </summary>
		private readonly IList<int?> gisList;

		/// <summary>
		/// Instantiates a new CloudInformationService object.
		/// </summary>
		/// <param name="name"> the name to be associated with this entity (as required by <seealso cref="SimEntity"/> class) </param>
		/// <exception cref="Exception"> when creating this entity before initialising CloudSim package
		///             or this entity name is <tt>null</tt> or empty
		/// @pre name != null
		/// @post $none
		/// 
		/// @todo The use of Exception is not recommended. Specific exceptions
		/// would be thrown (such as <seealso cref="IllegalArgumentException"/>)
		/// or <seealso cref="RuntimeException"/> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CloudInformationService(String name) throws Exception
		public CloudInformationService(string name) : base(name)
		{
			//resList = new List<int?>();
			//arList = new List<int?>();
			//gisList = new List<int?>();

            resList = new List<int?>();
            arList = new List<int?>();
            gisList = new List<int?>();

        }

        /// <summary>
        /// The method has no effect at the current class.
        /// </summary>
        public override void startEntity()
		{
		}

		public override void processEvent(SimEvent ev)
		{
			int id = -1; // requester id
			switch (ev.Tag)
			{
			// storing regional CIS id
				case CloudSimTags.REGISTER_REGIONAL_GIS:
					gisList.Add((int?) ev.Data);
					break;

				// request for all regional CIS list
				case CloudSimTags.REQUEST_REGIONAL_GIS:

					// Get ID of an entity that send this event
					id = ((int?) ev.Data).Value;

					// Send the regional GIS list back to sender
					base.send(id, 0L, ev.Tag, gisList);
					break;

				// A resource is requesting to register.
				case CloudSimTags.REGISTER_RESOURCE:
					resList.Add((int?) ev.Data);
					break;

				// A resource that can support Advance Reservation
				case CloudSimTags.REGISTER_RESOURCE_AR:
					resList.Add((int?) ev.Data);
					arList.Add((int?) ev.Data);
					break;

				// A Broker is requesting for a list of all hostList.
				case CloudSimTags.RESOURCE_LIST:

					// Get ID of an entity that send this event
					id = ((int?) ev.Data).Value;

					// Send the resource list back to the sender
					base.send(id, 0L, ev.Tag, resList);
					break;

				// A Broker is requesting for a list of all hostList.
				case CloudSimTags.RESOURCE_AR_LIST:

					// Get ID of an entity that send this event
					id = ((int?) ev.Data).Value;

					// Send the resource AR list back to the sender
					base.send(id, 0L, ev.Tag, arList);
					break;

				default:
					processOtherEvent(ev);
					break;
			}
		}

		public override void shutdownEntity()
		{
			notifyAllEntity();
		}

		/// <summary>
		/// Gets the list of all CloudResource IDs, including hostList that support Advance Reservation.
		/// </summary>
		/// <returns> list containing resource IDs. Each ID is represented by an Integer object.
		/// @pre $none
		/// @post $none </returns>
		public virtual IList<int?> List
		{
			get
			{
				return resList;
			}
		}

		/// <summary>
		/// Gets the list of CloudResource IDs that <b>only</b> support Advanced Reservation.
		/// </summary>
		/// <returns> list containing resource IDs. Each ID is represented by an Integer object.
		/// @pre $none
		/// @post $none </returns>
		public virtual IList<int?> AdvReservList
		{
			get
			{
				return arList;
			}
		}

		/// <summary>
		/// Checks whether a given resource ID supports Advanced Reservations or not.
		/// </summary>
		/// <param name="id"> a resource ID </param>
		/// <returns> <tt>true</tt> if this resource supports Advanced Reservations, <tt>false</tt>
		///         otherwise
		/// @pre id != null
		/// @post $none </returns>
		public virtual bool resourceSupportAR(int? id)
		{
			if (id == null)
			{
				return false;
			}

			return resourceSupportAR(id.Value);
		}

		/// <summary>
		/// Checks whether a given resource ID supports Advanced Reservations or not.
		/// </summary>
		/// <param name="id"> a resource ID </param>
		/// <returns> <tt>true</tt> if this resource supports Advanced Reservations, <tt>false</tt>
		///         otherwise
		/// @pre id >= 0
		/// @post $none </returns>
		public virtual bool resourceSupportAR(int id)
		{
			bool flag = false;
			if (id < 0)
			{
				flag = false;
			}
			else
			{
				flag = checkResource(arList, id);
			}

			return flag;
		}

		/// <summary>
		/// Checks whether the given CloudResource ID exists or not.
		/// </summary>
		/// <param name="id"> a CloudResource id </param>
		/// <returns> <tt>true</tt> if the given ID exists, <tt>false</tt> otherwise
		/// @pre id >= 0
		/// @post $none </returns>
		public virtual bool resourceExist(int id)
		{
			bool flag = false;
			if (id < 0)
			{
				flag = false;
			}
			else
			{
				flag = checkResource(resList, id);
			}

			return flag;
		}

		/// <summary>
		/// Checks whether the given CloudResource ID exists or not.
		/// </summary>
		/// <param name="id"> a CloudResource id </param>
		/// <returns> <tt>true</tt> if the given ID exists, <tt>false</tt> otherwise
		/// @pre id != null
		/// @post $none </returns>
		public virtual bool resourceExist(int? id)
		{
			if (id == null)
			{
				return false;
			}
			return resourceExist(id.Value);
		}

		// //////////////////////// PROTECTED METHODS ////////////////////////////

		/// <summary>
		/// Process non-default received events that aren't processed by
		/// the <seealso cref="#processEvent(org.cloudbus.cloudsim.core.SimEvent)"/> method.
		/// This method should be overridden by subclasses in other to process
		/// new defined events.
		/// </summary>
		/// <param name="ev"> a SimEvent object
		/// @pre ev != null
		/// @post $none </param>
		protected internal virtual void processOtherEvent(SimEvent ev)
		{
			if (ev == null)
			{
				Log.printConcatLine("CloudInformationService.processOtherEvent(): ", "Unable to handle a request since the event is null.");
				return;
			}

			Log.printLine("CloudInformationSevice.processOtherEvent(): " + "Unable to handle a request from " + CloudSim.getEntityName(ev.Source) + " with event tag = " + ev.Tag);
		}

		/// <summary>
		/// Notifies the registered entities about the end of simulation. This method should be
		/// overridden by child classes.
		/// </summary>
		protected internal virtual void processEndSimulation()
		{
			// this should be overridden by the child class
		}

		// ////////////////// End of PROTECTED METHODS ///////////////////////////

		/// <summary>
		/// Checks whether a list contains a particular resource id.
		/// </summary>
		/// <param name="list"> list of resource id </param>
		/// <param name="id"> a resource ID to find </param>
		/// <returns> true if a resource is in the list, otherwise false
		/// @pre list != null
		/// @pre id > 0
		/// @post $none </returns>
		private bool checkResource(ICollection<int?> list, int id)
		{
			bool flag = false;
			if (list == null || id < 0)
			{
				return flag;
			}

			int? obj = null;
			IEnumerator<int?> it = list.GetEnumerator();

			// a loop to find the match the resource id in a list
			while (it.MoveNext())
			{
				obj = it.Current;
				if (obj.Value == id)
				{
					flag = true;
					break;
				}
			}

			return flag;
		}

		/// <summary>
		/// Tells all registered entities about the end of simulation.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		private void notifyAllEntity()
		{
			Log.printConcatLine(base.Name, ": Notify all CloudSim entities for shutting down.");

			signalShutdown(resList);
			signalShutdown(gisList);

			// reset the values
			resList.Clear();
			gisList.Clear();
		}

		/// <summary>
		/// Sends a <seealso cref="CloudSimTags#END_OF_SIMULATION"/> signal to all entity IDs 
		/// mentioned in the given list.
		/// </summary>
		/// <param name="list"> List storing entity IDs
		/// @pre list != null
		/// @post $none </param>
		protected internal virtual void signalShutdown(ICollection<int?> list)
		{
			// checks whether a list is empty or not
			if (list == null)
			{
				return;
			}

			IEnumerator<int?> it = list.GetEnumerator();
			int? obj = null;
			int id = 0; // entity ID

			// Send END_OF_SIMULATION event to all entities in the list
			while (it.MoveNext())
			{
				obj = it.Current;
				id = obj.Value;
				base.send(id, 0L, CloudSimTags.END_OF_SIMULATION);
			}
		}

	}

}