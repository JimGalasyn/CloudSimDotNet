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
	/// CloudimShutdown waits for termination of all CloudSim user entities to determine the end of
	/// simulation. This class will be created by CloudSim upon initialisation of the simulation, i.e.
	/// done via <tt>CloudSim.init()</tt> method. Hence, do not need to worry about creating an object of
	/// this class. This object signals the end of simulation to CloudInformationService (CIS) entity.
	/// 
	/// @author Manzur Murshed
	/// @author Rajkumar Buyya
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class CloudSimShutdown : SimEntity
	{

		/// <summary>
		/// The total number of cloud users. </summary>
		private int numUser;

		/// <summary>
		/// Instantiates a new CloudSimShutdown object.
		/// <p/>
		/// The total number of cloud user entities plays an important role to determine whether all
		/// hostList' should be shut down or not. If one or more users are still not finished, then the
		/// hostList's will not be shut down. Therefore, it is important to give a correct number of total
		/// cloud user entities. Otherwise, CloudSim program will hang or encounter a weird behaviour.
		/// </summary>
		/// <param name="name"> the name to be associated with this entity (as required by <seealso cref="SimEntity"/> class) </param>
		/// <param name="numUser"> total number of cloud user entities </param>
		/// <exception cref="Exception"> when creating this entity before initialising CloudSim package
		///             or this entity name is <tt>null</tt> or empty </exception>
		/// <seealso cref= CloudSim#init(int, java.util.Calendar, boolean) 
		/// @pre name != null
		/// @pre numUser >= 0
		/// @post $none
		/// 
		/// @todo The use of Exception is not recommended. Specific exceptions
		/// would be thrown (such as <seealso cref="IllegalArgumentException"/>)
		/// or <seealso cref="RuntimeException"/> </seealso>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CloudSimShutdown(String name, int numUser) throws Exception
		public CloudSimShutdown(string name, int numUser) : base(name)
		{
			// NOTE: This entity doesn't use any I/O port.
			// super(name, CloudSimTags.DEFAULT_BAUD_RATE);
			this.numUser = numUser;
		}

		/// <summary>
		/// The main method that shuts down hostList's and Cloud Information Service (CIS). In addition,
		/// this method writes down a report at the end of a simulation based on
		/// <tt>reportWriterName</tt> defined in the Constructor. <br/>
		/// <b>NOTE:</b> This method shuts down cloud hostList's and CIS entities either <tt>AFTER</tt> all
		/// cloud users have been shut down or an entity requires an abrupt end of the whole simulation.
		/// In the first case, the number of cloud users given in the Constructor <tt>must</tt> be
		/// correct. Otherwise, CloudSim package hangs forever or it does not terminate properly.
		/// </summary>
		/// <param name="ev"> the ev
		/// @pre $none
		/// @post $none </param>
		public override void processEvent(SimEvent ev)
		{
			numUser--;
			if (numUser == 0 || ev.Tag == CloudSimTags.ABRUPT_END_OF_SIMULATION)
			{
				CloudSim.abruptallyTerminate();
			}
		}

			/// <summary>
			/// The method has no effect at the current class.
			/// </summary>
		public override void startEntity()
		{
			// do nothing
		}

			/// <summary>
			/// The method has no effect at the current class.
			/// </summary>
		public override void shutdownEntity()
		{
			// do nothing
		}
	}

}