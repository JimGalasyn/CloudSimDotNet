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
	/// Stores historic data about a VM.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.1.2
	/// </summary>
	public class VmStateHistoryEntry
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
		/// The is in migration. </summary>
		private bool isInMigration;

		/// <summary>
		/// Instantiates a new VmStateHistoryEntry
		/// </summary>
		/// <param name="time"> the time </param>
		/// <param name="allocatedMips"> the allocated mips </param>
		/// <param name="requestedMips"> the requested mips </param>
		/// <param name="isInMigration"> the is in migration </param>
		public VmStateHistoryEntry(double time, double allocatedMips, double requestedMips, bool isInMigration)
		{
			Time = time;
			AllocatedMips = allocatedMips;
			RequestedMips = requestedMips;
			InMigration = isInMigration;
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
		/// Sets the in migration.
		/// </summary>
		/// <param name="isInMigration"> the new in migration </param>
		public virtual bool InMigration
		{
			set
			{
				this.isInMigration = value;
			}
			get
			{
				return isInMigration;
			}
		}


	}

}