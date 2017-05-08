using System;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{

	using CloudSim = org.cloudbus.cloudsim.core.CloudSim;

	/// <summary>
	/// Stores related information regarding to a <seealso cref="gridsim.datagrid.File"/> entity.
	/// 
	/// @author Uros Cibej
	/// @author Anthony Sulistio
	/// @since CloudSim Toolkit 1.0
	/// 
	/// @todo Some attributes of this class may be duplicated from the <seealso cref="File"/> class,
	/// such as name (logical file name), that is clearly related to the file.
	/// There would be a relation between File and FileAttribute. There is a lot of duplicated
	/// methods to, such as <seealso cref="#setMasterCopy(boolean)"/> or <seealso cref="#isReadOnly()"/>
	/// </summary>
	public class FileAttribute
	{

			/// <summary>
			/// Logical file name. </summary>
		private string name;
			/// <summary>
			/// Owner name of this file. </summary>
		private string ownerName;
			/// <summary>
			/// File ID given by a Replica Catalogue. </summary>
		private int id;
			/// <summary>
			/// File type, for instance raw, reconstructed, etc. </summary>
		private int type;
			/// <summary>
			/// File size in byte. </summary>
		private int size;
			/// <summary>
			/// Check sum. </summary>
		private int checksum;
			/// <summary>
			/// Last updated time (sec) - relative. </summary>
		private double lastUpdateTime;
			/// <summary>
			/// Creation time (ms) - abosulte/relative. </summary>
		private long creationTime;
			/// <summary>
			/// Price of the file. </summary>
		private double cost;
			/// <summary>
			/// Indicates if the file is a master copy or not. 
			/// If the attribute is false, it means the file is a replica. 
			/// </summary>
		private bool masterCopy;
			/// <summary>
			/// Indicates if the file is read-only or not. </summary>
		private bool readOnly;
			/// <summary>
			/// Resource ID storing this file. </summary>
		private int resourceId;

		/// <summary>
		/// Creates a new FileAttribute object.
		/// </summary>
		/// <param name="fileName"> file name </param>
		/// <param name="fileSize"> size of this file (in bytes) </param>
		/// <exception cref="ParameterException"> This happens when one of the following scenarios occur:
		///             <ul>
		///             <li>the file name is empty or <tt>null</tt>
		///             <li>the file size is zero or negative numbers
		///             </ul> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public FileAttribute(String fileName, int fileSize) throws ParameterException
		public FileAttribute(string fileName, int fileSize)
		{
			// check for errors in the input
			if (string.ReferenceEquals(fileName, null) || fileName.Length == 0)
			{
				throw new ParameterException("FileAttribute(): Error - invalid file name.");
			}

			if (fileSize <= 0)
			{
				throw new ParameterException("FileAttribute(): Error - size <= 0.");
			}

			size = fileSize;
			name = fileName;

            // set the file creation time. This is absolute time
            //DateTime date = CloudSim.SimulationCalendar.Time;
            // TODO: Figure out the intended timestamp.
            DateTime date = CloudSim.SimulationCalendar;
            if (date == null)
			{
				creationTime = 0;
			}
			else
			{
				creationTime = date.Ticks;
			}

			ownerName = null;
			id = File.NOT_REGISTERED;
			checksum = 0;
			type = File.TYPE_UNKOWN;
			lastUpdateTime = 0;
			cost = 0;
			resourceId = -1;
			masterCopy = true;
			readOnly = false;
		}

		/// <summary>
		/// Copy the values of the object into a given FileAttribute instance.
		/// </summary>
		/// <param name="attr"> the destination FileAttribute object to copy the current object to </param>
		/// <returns> <tt>true</tt> if the copy operation is successful, <tt>false</tt> otherwise </returns>
		public virtual bool copyValue(FileAttribute attr)
		{
			if (attr == null)
			{
				return false;
			}

			attr.FileSize = size;
			attr.ResourceID = resourceId;
			attr.OwnerName = ownerName;
			attr.LastUpdateTime = lastUpdateTime;
			attr.RegistrationId = id;
			attr.Type = type;
			attr.Checksum = checksum;
			attr.Cost = cost;
			attr.MasterCopy = masterCopy;
			attr.ReadOnly = readOnly;
			attr.Name = name;
			attr.CreationTime = creationTime;

			return true;
		}

		/// <summary>
		/// Sets the file creation time (in millisecond).
		/// </summary>
		/// <param name="creationTime"> the file creation time (in millisecond) </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setCreationTime(long creationTime)
		{
			if (creationTime <= 0)
			{
				return false;
			}

			this.creationTime = creationTime;
			return true;
		}

		/// <summary>
		/// Gets the file creation time (in millisecond).
		/// </summary>
		/// <returns> the file creation time (in millisecond) </returns>
		public virtual long CreationTime
		{
			get
			{
				return creationTime;
			}
            private set
            {
                creationTime = value;
            }
		}

		/// <summary>
		/// Sets the resource ID that stores the file.
		/// </summary>
		/// <param name="resourceID"> a resource ID </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setResourceID(int resourceID)
		{
			if (resourceID == -1)
			{
				return false;
			}

			resourceId = resourceID;
			return true;
		}

		/// <summary>
		/// Gets the resource ID that stores the file.
		/// </summary>
		/// <returns> the resource ID </returns>
		public virtual int ResourceID
		{
			get
			{
				return resourceId;
			}

            private set
            {
                resourceId = value;
            }
		}

		/// <summary>
		/// Sets the owner name of the file.
		/// </summary>
		/// <param name="name"> the owner name </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setOwnerName(string name)
		{
			if (string.ReferenceEquals(name, null) || name.Length == 0)
			{
				return false;
			}

			ownerName = name;
			return true;
		}

		/// <summary>
		/// Gets the owner name of the file.
		/// </summary>
		/// <returns> the owner name or <tt>null</tt> if empty </returns>
		public virtual string OwnerName
		{
			get
			{
				return ownerName;
			}
            private set
            {
                ownerName = value;
            }

        }

        /// <summary>
        /// Gets the size of the object (in byte). <br/>
        /// NOTE: This object size is NOT the actual file size. Moreover, this size is used for
        /// transferring this object over a network.
        /// </summary>
        /// <returns> the object size (in byte) </returns>
        public virtual int AttributeSize
		{
			get
			{
				int length = DataCloudTags.PKT_SIZE;
				if (!string.ReferenceEquals(ownerName, null))
				{
					length += ownerName.Length;
				}
    
				if (!string.ReferenceEquals(name, null))
				{
					length += name.Length;
				}
    
				return length;
			}
		}

		/// <summary>
		/// Sets the file size (in MBytes).
		/// </summary>
		/// <param name="fileSize"> the file size (in MBytes) </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setFileSize(int fileSize)
		{
			if (fileSize < 0)
			{
				return false;
			}

			size = fileSize;
			return true;
		}

		/// <summary>
		/// Gets the file size (in MBytes).
		/// </summary>
		/// <returns> the file size (in MBytes) </returns>
		public virtual int FileSize
		{
			get
			{
				return size;
			}
            private set
            {
                size = value;
            }

		}

		/// <summary>
		/// Gets the file size (in bytes).
		/// </summary>
		/// <returns> the file size (in bytes) </returns>
		public virtual int FileSizeInByte
		{
			get
			{
				return size * Consts.MILLION; // 1e6
				// return size * 1048576; // 1e6 - more accurate
			}
		}

		/// <summary>
		/// Sets the last update time of the file (in seconds). <br/>
		/// NOTE: This time is relative to the start time. Preferably use
		/// <seealso cref="gridsim.CloudSim#clock()"/> method.
		/// </summary>
		/// <param name="time"> the last update time (in seconds) </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setUpdateTime(double time)
		{
			if (time <= 0 || time < lastUpdateTime)
			{
				return false;
			}

			lastUpdateTime = time;
			return true;
		}

		/// <summary>
		/// Gets the last update time (in seconds).
		/// </summary>
		/// <returns> the last update time (in seconds) </returns>
		public virtual double LastUpdateTime
		{
			get
			{
				return lastUpdateTime;
			}
            private set
            {
                setUpdateTime(value);
            }
		}

		/// <summary>
		/// Sets the file registration ID (published by a Replica Catalogue entity).
		/// </summary>
		/// <param name="id"> registration ID </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setRegistrationId(int id)
		{
			if (id < 0)
			{
				return false;
			}

			this.id = id;
			return true;
		}

		/// <summary>
		/// Gets the file registration ID.
		/// </summary>
		/// <returns> registration ID </returns>
		public virtual int RegistrationId
		{
			get
			{
				return id;
			}
            private set
            {
                setRegistrationId(value);
            }
		}

		/// <summary>
		/// Sets the file type (for instance raw, tag, etc).
		/// </summary>
		/// <param name="type"> a file type </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setType(int type)
		{
			if (type < 0)
			{
				return false;
			}

			this.type = type;
			return true;
		}

		/// <summary>
		/// Gets the file type.
		/// </summary>
		/// <returns> file type </returns>
		public virtual int Type
		{
			get
			{
				return type;
			}
            private set
            {
                type = value;
            }
		}

		/// <summary>
		/// Sets the checksum of the file.
		/// </summary>
		/// <param name="checksum"> the checksum of this file </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setChecksum(int checksum)
		{
			if (checksum < 0)
			{
				return false;
			}

			this.checksum = checksum;
			return true;
		}

		/// <summary>
		/// Gets the file checksum.
		/// </summary>
		/// <returns> file checksum </returns>
		public virtual int Checksum
		{
			get
			{
				return checksum;
			}
            private set
            {
                checksum = value;
            }

        }

        /// <summary>
        /// Sets the cost associated with the file.
        /// </summary>
        /// <param name="cost"> cost of this file </param>
        /// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
        public virtual bool setCost(double cost)
		{
			if (cost < 0)
			{
				return false;
			}

			this.cost = cost;
			return true;
		}

		/// <summary>
		/// Gets the cost associated with the file.
		/// </summary>
		/// <returns> the cost of this file </returns>
		public virtual double Cost
		{
			get
			{
				return cost;
			}
            private set
            {
                cost = value;
            }
		}

		/// <summary>
		/// Checks if the file is already registered to a Replica Catalogue.
		/// </summary>
		/// <returns> <tt>true</tt> if it is registered, <tt>false</tt> otherwise </returns>
		public virtual bool Registered
		{
			get
			{
				bool result = true;
				if (id == File.NOT_REGISTERED)
				{
					result = false;
				}
    
				return result;
			}
		}

		/// <summary>
		/// Marks the file as a master copy or replica.
		/// </summary>
		/// <param name="masterCopy"> a flag denotes <tt>true</tt> for master copy or <tt>false</tt> for a
		///            replica </param>
		public virtual bool MasterCopy
		{
			set
			{
				this.masterCopy = value;
			}
			get
			{
				return masterCopy;
			}
		}


		/// <summary>
		/// Marks this file as a read only or not.
		/// </summary>
		/// <param name="readOnly"> a flag denotes <tt>true</tt> for read only or <tt>false</tt> for re-writeable </param>
		public virtual bool ReadOnly
		{
			set
			{
				this.readOnly = value;
			}
			get
			{
				return readOnly;
			}
		}


		/// <summary>
		/// Sets the file name.
		/// </summary>
		/// <param name="name"> the file name </param>
		public virtual string Name
		{
			set
			{
				this.name = value;
			}
			get
			{
				return name;
			}
		}


	}

}