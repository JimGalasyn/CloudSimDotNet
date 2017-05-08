using System.Collections.Generic;

/*
 * Title: CloudSim Toolkit Description: CloudSim (Cloud Simulation) Toolkit for Modeling and
 * Simulation of Clouds Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * 
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	/// <summary>
	/// Represents a Virtual Machine (VM) that runs inside a Host, sharing a hostList with other VMs. It processes
	/// cloudlets. This processing happens according to a policy, defined by the CloudletScheduler. Each
	/// VM has a owner, which can submit cloudlets to the VM to execute them.
	/// 
	/// @author Rodrigo N. Calheiros
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class Vm
	{

		/// <summary>
		/// The VM unique id. </summary>
		private int id;

		/// <summary>
		/// The user id. </summary>
		private int userId;

		/// <summary>
		/// A Unique Identifier (UID) for the VM, that is compounded by the user id and VM id. </summary>
		private string uid;

		/// <summary>
		/// The size the VM image size (the amount of storage it will use, at least initially). </summary>
		private long size;

		/// <summary>
		/// The MIPS capacity of each VM's PE. </summary>
		private double mips;

		/// <summary>
		/// The number of PEs required by the VM. </summary>
		private int numberOfPes;

		/// <summary>
		/// The required ram. </summary>
		private int ram;

		/// <summary>
		/// The required bw. </summary>
		private long bw;

		/// <summary>
		/// The Virtual Machine Monitor (VMM) that manages the VM. </summary>
		private string vmm;

		/// <summary>
		/// The Cloudlet scheduler the VM uses to schedule cloudlets execution. </summary>
		private CloudletScheduler cloudletScheduler;

		/// <summary>
		/// The PM that hosts the VM. </summary>
		private Host host;

		/// <summary>
		/// Indicates if the VM is in migration process. </summary>
		private bool inMigration;

		/// <summary>
		/// The current allocated storage size. </summary>
		private long currentAllocatedSize;

		/// <summary>
		/// The current allocated ram. </summary>
		private int currentAllocatedRam;

		/// <summary>
		/// The current allocated bw. </summary>
		private long currentAllocatedBw;

		/// <summary>
		/// The current allocated mips for each VM's PE. </summary>
		private IList<double?> currentAllocatedMips;

		/// <summary>
		/// Indicates if the VM is being instantiated. </summary>
		private bool beingInstantiated;

		/// <summary>
		/// The mips allocation history. 
		/// @todo Instead of using a list, this attribute would be 
		/// a map, where the key can be the history time
		/// and the value the history itself. 
		/// By this way, if one wants to get the history for a given
		/// time, he/she doesn't have to iterate over the entire list
		/// to find the desired entry.
		/// </summary>
		private readonly IList<VmStateHistoryEntry> stateHistory = new List<VmStateHistoryEntry>();

		/// <summary>
		/// Creates a new Vm object.
		/// </summary>
		/// <param name="id"> unique ID of the VM </param>
		/// <param name="userId"> ID of the VM's owner </param>
		/// <param name="mips"> the mips </param>
		/// <param name="numberOfPes"> amount of CPUs </param>
		/// <param name="ram"> amount of ram </param>
		/// <param name="bw"> amount of bandwidth </param>
		/// <param name="size"> The size the VM image size (the amount of storage it will use, at least initially). </param>
		/// <param name="vmm"> virtual machine monitor </param>
		/// <param name="cloudletScheduler"> cloudletScheduler policy for cloudlets scheduling
		/// 
		/// @pre id >= 0
		/// @pre userId >= 0
		/// @pre size > 0
		/// @pre ram > 0
		/// @pre bw > 0
		/// @pre cpus > 0
		/// @pre priority >= 0
		/// @pre cloudletScheduler != null
		/// @post $none </param>
		public Vm(int id, int userId, double mips, int numberOfPes, int ram, long bw, long size, string vmm, CloudletScheduler cloudletScheduler)
		{
			Id = id;
			UserId = userId;
			Uid = getUid(userId, id);
			Mips = mips;
			NumberOfPes = numberOfPes;
			Ram = ram;
			Bw = bw;
			Size = size;
			Vmm = vmm;
			CloudletScheduler = cloudletScheduler;

			InMigration = false;
			BeingInstantiated = true;

			CurrentAllocatedBw = 0;
			//CurrentAllocatedMips = null;
			CurrentAllocatedRam = 0;
			CurrentAllocatedSize = 0;
		}

		/// <summary>
		/// Updates the processing of cloudlets running on this VM.
		/// </summary>
		/// <param name="currentTime"> current simulation time </param>
		/// <param name="mipsShare"> list with MIPS share of each Pe available to the scheduler </param>
		/// <returns> time predicted completion time of the earliest finishing cloudlet, or 0 if there is no
		///         next events
		/// @pre currentTime >= 0
		/// @post $none </returns>
		public virtual double updateVmProcessing(double currentTime, IList<double?> mipsShare)
		{
			if (mipsShare != null)
			{
				return CloudletScheduler.updateVmProcessing(currentTime, mipsShare);
			}
			return 0.0;
		}

		/// <summary>
		/// Gets the current requested mips.
		/// </summary>
		/// <returns> the current requested mips </returns>
		public virtual IList<double?> CurrentRequestedMips
		{
			get
			{
				IList<double?> currentRequestedMips = CloudletScheduler.CurrentRequestedMips;
				if (BeingInstantiated)
				{
					currentRequestedMips = new List<double?>();
					for (int i = 0; i < NumberOfPes; i++)
					{
						currentRequestedMips.Add(Mips);
					}
				}
				return currentRequestedMips;
			}
		}

		/// <summary>
		/// Gets the current requested total mips.
		/// </summary>
		/// <returns> the current requested total mips </returns>
		public virtual double CurrentRequestedTotalMips
		{
			get
			{
				double totalRequestedMips = 0;
				foreach (double mips in CurrentRequestedMips)
				{
					totalRequestedMips += mips;
				}
				return totalRequestedMips;
			}
		}

		/// <summary>
		/// Gets the current requested max mips among all virtual PEs.
		/// </summary>
		/// <returns> the current requested max mips </returns>
		public virtual double CurrentRequestedMaxMips
		{
			get
			{
				double maxMips = 0;
				foreach (double mips in CurrentRequestedMips)
				{
					if (mips > maxMips)
					{
						maxMips = mips;
					}
				}
				return maxMips;
			}
		}

		/// <summary>
		/// Gets the current requested bw.
		/// </summary>
		/// <returns> the current requested bw </returns>
		public virtual long CurrentRequestedBw
		{
			get
			{
				if (BeingInstantiated)
				{
					return Bw;
				}
				return (long)(CloudletScheduler.CurrentRequestedUtilizationOfBw * Bw);
			}
		}

		/// <summary>
		/// Gets the current requested ram.
		/// </summary>
		/// <returns> the current requested ram </returns>
		public virtual int CurrentRequestedRam
		{
			get
			{
				if (BeingInstantiated)
				{
					return Ram;
				}
				return (int)(CloudletScheduler.CurrentRequestedUtilizationOfRam * Ram);
			}
		}

		/// <summary>
		/// Gets total CPU utilization percentage of all clouddlets running on this VM at the given time
		/// </summary>
		/// <param name="time"> the time </param>
		/// <returns> total utilization percentage </returns>
		public virtual double getTotalUtilizationOfCpu(double time)
		{
			return CloudletScheduler.getTotalUtilizationOfCpu(time);
		}

		/// <summary>
		/// Get total CPU utilization of all cloudlets running on this VM at the given time (in MIPS).
		/// </summary>
		/// <param name="time"> the time </param>
		/// <returns> total cpu utilization in MIPS </returns>
		/// <seealso cref= #getTotalUtilizationOfCpu(double)  </seealso>
		public virtual double getTotalUtilizationOfCpuMips(double time)
		{
			return getTotalUtilizationOfCpu(time) * Mips;
		}

		/// <summary>
		/// Sets the uid.
		/// </summary>
		/// <param name="uid"> the new uid </param>
		public virtual string Uid
		{
			set
			{
				this.uid = value;
			}
			get
			{
				return uid;
			}
		}


		/// <summary>
		/// Generate unique string identifier of the VM.
		/// </summary>
		/// <param name="userId"> the user id </param>
		/// <param name="vmId"> the vm id </param>
		/// <returns> string uid </returns>
		public static string getUid(int userId, int vmId)
		{
			return userId + "-" + vmId;
		}

		/// <summary>
		/// Gets the VM id.
		/// </summary>
		/// <returns> the id </returns>
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


		/// <summary>
		/// Sets the user id.
		/// </summary>
		/// <param name="userId"> the new user id </param>
		protected internal virtual int UserId
		{
			set
			{
				this.userId = value;
			}
			get
			{
				return userId;
			}
		}


		/// <summary>
		/// Gets the mips.
		/// </summary>
		/// <returns> the mips </returns>
		public virtual double Mips
		{
			get
			{
				return mips;
			}
			set
			{
				this.mips = value;
			}
		}


		/// <summary>
		/// Gets the number of pes.
		/// </summary>
		/// <returns> the number of pes </returns>
		public virtual int NumberOfPes
		{
			get
			{
				return numberOfPes;
			}
			set
			{
				this.numberOfPes = value;
			}
		}


		/// <summary>
		/// Gets the amount of ram.
		/// </summary>
		/// <returns> amount of ram
		/// @pre $none
		/// @post $none </returns>
		public virtual int Ram
		{
			get
			{
				return ram;
			}
			set
			{
				this.ram = value;
			}
		}


		/// <summary>
		/// Gets the amount of bandwidth.
		/// </summary>
		/// <returns> amount of bandwidth
		/// @pre $none
		/// @post $none </returns>
		public virtual long Bw
		{
			get
			{
				return bw;
			}
			set
			{
				this.bw = value;
			}
		}


		/// <summary>
		/// Gets the amount of storage.
		/// </summary>
		/// <returns> amount of storage
		/// @pre $none
		/// @post $none </returns>
		public virtual long Size
		{
			get
			{
				return size;
			}
			set
			{
				this.size = value;
			}
		}


		/// <summary>
		/// Gets the VMM.
		/// </summary>
		/// <returns> VMM
		/// @pre $none
		/// @post $none </returns>
		public virtual string Vmm
		{
			get
			{
				return vmm;
			}
			set
			{
				this.vmm = value;
			}
		}


		/// <summary>
		/// Sets the host that runs this VM.
		/// </summary>
		/// <param name="host"> Host running the VM
		/// @pre host != $null
		/// @post $none </param>
		public virtual Host Host
		{
			set
			{
				this.host = value;
			}
			get
			{
				return host;
			}
		}


		/// <summary>
		/// Gets the vm scheduler.
		/// </summary>
		/// <returns> the vm scheduler </returns>
		public virtual CloudletScheduler CloudletScheduler
		{
			get
			{
				return cloudletScheduler;
			}
			set
			{
				this.cloudletScheduler = value;
			}
		}


		/// <summary>
		/// Checks if is in migration.
		/// </summary>
		/// <returns> true, if is in migration </returns>
		public virtual bool InMigration
		{
			get
			{
				return inMigration;
			}
			set
			{
				this.inMigration = value;
			}
		}


		/// <summary>
		/// Gets the current allocated size.
		/// </summary>
		/// <returns> the current allocated size </returns>
		public virtual long CurrentAllocatedSize
		{
			get
			{
				return currentAllocatedSize;
			}
			set
			{
				this.currentAllocatedSize = value;
			}
		}


		/// <summary>
		/// Gets the current allocated ram.
		/// </summary>
		/// <returns> the current allocated ram </returns>
		public virtual int CurrentAllocatedRam
		{
			get
			{
				return currentAllocatedRam;
			}
			set
			{
				this.currentAllocatedRam = value;
			}
		}


		/// <summary>
		/// Gets the current allocated bw.
		/// </summary>
		/// <returns> the current allocated bw </returns>
		public virtual long CurrentAllocatedBw
		{
			get
			{
				return currentAllocatedBw;
			}
			set
			{
				this.currentAllocatedBw = value;
			}
		}


		/// <summary>
		/// Gets the current allocated mips.
		/// </summary>
		/// <returns> the current allocated mips
		/// @TODO replace returning the field by a call to getCloudletScheduler().getCurrentMipsShare() </returns>
		public virtual IList<double?> CurrentAllocatedMips
		{
			get
			{
                //return currentAllocatedMips;
                // TODO: TEST comment says this should be CloudletScheduler.CurrentMipsShare, so...
                return CloudletScheduler.CurrentMipsShare;
			}
			//set
			//{
			//	this.currentAllocatedMips = value;
			//}
		}


		/// <summary>
		/// Checks if is being instantiated.
		/// </summary>
		/// <returns> true, if is being instantiated </returns>
		public virtual bool BeingInstantiated
		{
			get
			{
				return beingInstantiated;
			}
			set
			{
				this.beingInstantiated = value;
			}
		}


		/// <summary>
		/// Gets the state history.
		/// </summary>
		/// <returns> the state history </returns>
		public virtual IList<VmStateHistoryEntry> StateHistory
		{
			get
			{
				return stateHistory;
			}
		}

		/// <summary>
		/// Adds a VM state history entry.
		/// </summary>
		/// <param name="time"> the time </param>
		/// <param name="allocatedMips"> the allocated mips </param>
		/// <param name="requestedMips"> the requested mips </param>
		/// <param name="isInMigration"> the is in migration </param>
		public virtual void addStateHistoryEntry(double time, double allocatedMips, double requestedMips, bool isInMigration)
		{
			VmStateHistoryEntry newState = new VmStateHistoryEntry(time, allocatedMips, requestedMips, isInMigration);
			if (StateHistory.Count > 0)
			{
				VmStateHistoryEntry previousState = StateHistory[StateHistory.Count - 1];
				if (previousState.Time == time)
				{
					StateHistory[StateHistory.Count - 1] = newState;
					return;
				}
			}
			StateHistory.Add(newState);
		}

	}

}