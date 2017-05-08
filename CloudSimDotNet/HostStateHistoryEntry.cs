/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2011, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{

	/// <summary>
	/// Stores historic data about a host.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.1.2
	/// </summary>
	public class HostStateHistoryEntry
	{

		/// <summary>
		/// The time. </summary>
		private double time;

		/// <summary>
		/// The allocated mips. </summary>
		private double allocatedMips;

		/// <summary>
		/// The requested mips. </summary>
		private double requestedMips;

		/// <summary>
		/// Indicates if the host was active in the indicated time. </summary>
		/// <seealso cref= #time </seealso>
		private bool isActive;

		/// <summary>
		/// Instantiates a new host state history entry.
		/// </summary>
		/// <param name="time"> the time </param>
		/// <param name="allocatedMips"> the allocated mips </param>
		/// <param name="requestedMips"> the requested mips </param>
		/// <param name="isActive"> the is active </param>
		public HostStateHistoryEntry(double time, double allocatedMips, double requestedMips, bool isActive)
		{
			Time = time;
			AllocatedMips = allocatedMips;
			RequestedMips = requestedMips;
			Active = isActive;
		}

		/// <summary>
		/// Sets the time.
		/// </summary>
		/// <param name="time"> the new time </param>
		public virtual double Time
		{
			set
			{
				this.time = value;
			}
			get
			{
				return time;
			}
		}


		/// <summary>
		/// Sets the allocated mips.
		/// </summary>
		/// <param name="allocatedMips"> the new allocated mips </param>
		public virtual double AllocatedMips
		{
			set
			{
				this.allocatedMips = value;
			}
			get
			{
				return allocatedMips;
			}
		}


		/// <summary>
		/// Sets the requested mips.
		/// </summary>
		/// <param name="requestedMips"> the new requested mips </param>
		public virtual double RequestedMips
		{
			set
			{
				this.requestedMips = value;
			}
			get
			{
				return requestedMips;
			}
		}


		/// <summary>
		/// Sets the active.
		/// </summary>
		/// <param name="isActive"> the new active </param>
		public virtual bool Active
		{
			set
			{
				this.isActive = value;
			}
			get
			{
				return isActive;
			}
		}


	}

}