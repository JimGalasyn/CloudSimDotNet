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

	/// <summary>
	/// A class for representing a physical file in a DataCloud environment
	/// 
	/// @author Uros Cibej
	/// @author Anthony Sulistio
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class File
	{

			/// <summary>
			/// Logical file name.
			/// </summary>
		private string name;

			/// <summary>
			/// A file attribute.
			/// </summary>
		private FileAttribute attribute;

		/// <summary>
		/// A transaction time for adding, deleting or getting the file. </summary>
		/// <seealso cref= #setTransactionTime(double)  </seealso>
		private double transactionTime;

		/// <summary>
		/// Denotes that this file has not been registered to a Replica Catalogue. </summary>
		public const int NOT_REGISTERED = -1;

		/// <summary>
		/// Denotes that the type of this file is unknown. </summary>
		public const int TYPE_UNKOWN = 0;

		/// <summary>
		/// Denotes that the type of this file is a raw data. </summary>
		public const int TYPE_RAW_DATA = 1;

		/// <summary>
		/// Denotes that the type of this file is a reconstructed data. </summary>
		public const int TYPE_RECONSTRUCTED_DATA = 2;

		/// <summary>
		/// Denotes that the type of this file is a tag data. </summary>
		public const int TYPE_TAG_DATA = 3;

		/// <summary>
		/// Creates a new DataCloud file with a given size (in MBytes). <br>
		/// NOTE: By default, a newly-created file is set to a <b>master</b> copy.
		/// </summary>
		/// <param name="fileName"> file name </param>
		/// <param name="fileSize"> file size in MBytes </param>
		/// <exception cref="ParameterException"> This happens when one of the following scenarios occur:
		///             <ul>
		///             <li>the file name is empty or <tt>null</tt>
		///             <li>the file size is zero or negative numbers
		///             </ul> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public File(String fileName, int fileSize) throws ParameterException
		public File(string fileName, int fileSize)
		{
			if (string.ReferenceEquals(fileName, null) || fileName.Length == 0)
			{
				throw new ParameterException("File(): Error - invalid file name.");
			}

			if (fileSize <= 0)
			{
				throw new ParameterException("File(): Error - size <= 0.");
			}

			name = fileName;
			attribute = new FileAttribute(fileName, fileSize);
			transactionTime = 0;
		}

		/// <summary>
		/// Copy constructor that creates a clone from a source file and set the given file
		/// as a <b>replica</b>.
		/// </summary>
		/// <param name="file"> the source file to create a copy and that will be set as a replica </param>
		/// <exception cref="ParameterException"> This happens when the source file is <tt>null</tt> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public File(File file) throws ParameterException
		public File(File file)
		{
			if (file == null)
			{
				throw new ParameterException("File(): Error - file is null.");
			}

			// copy the attributes into the file
			FileAttribute fileAttr = file.FileAttribute;
			attribute.copyValue(fileAttr);
			fileAttr.MasterCopy = false; // set this file to replica
		}

		/// <summary>
		/// Clone the current file and set the cloned one as a <b>replica</b>.
		/// </summary>
		/// <returns> a clone of the current file (as a replica) or <tt>null</tt> if an error occurs </returns>
		public virtual File makeReplica()
		{
			return makeCopy();
		}

		/// <summary>
		/// Clone the current file and make the new file as a <b>master</b> copy as well.
		/// </summary>
		/// <returns> a clone of the current file (as a master copy) or <tt>null</tt> if an error occurs </returns>
		public virtual File makeMasterCopy()
		{
			File file = makeCopy();
			if (file != null)
			{
				file.MasterCopy = true;
			}

			return file;
		}

		/// <summary>
		/// Makes a copy of this file.
		/// </summary>
		/// <returns> a clone of the current file (as a replica) or <tt>null</tt> if an error occurs </returns>
		private File makeCopy()
		{
			File file = null;
			try
			{
				file = new File(name, attribute.FileSize);
				FileAttribute fileAttr = file.FileAttribute;
				attribute.copyValue(fileAttr);
				fileAttr.MasterCopy = false; // set this file to replica
			}
			catch (Exception)
			{
				file = null;
			}

			return file;
		}

		/// <summary>
		/// Gets an attribute of this file.
		/// </summary>
		/// <returns> a file attribute </returns>
		public virtual FileAttribute FileAttribute
		{
			get
			{
				return attribute;
			}
		}

		/// <summary>
		/// Gets the size of this object (in byte). <br/>
		/// NOTE: This object size is NOT the actual file size. Moreover, this size is used for
		/// transferring this object over a network.
		/// </summary>
		/// <returns> the object size (in byte) </returns>
		public virtual int AttributeSize
		{
			get
			{
				return attribute.AttributeSize;
			}
		}

		/// <summary>
		/// Sets the resource ID that stores this file.
		/// </summary>
		/// <param name="resourceID"> a resource ID </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setResourceID(int resourceID)
		{
			return attribute.setResourceID(resourceID);
		}

		/// <summary>
		/// Gets the resource ID that stores this file.
		/// </summary>
		/// <returns> the resource ID </returns>
		public virtual int ResourceID
		{
			get
			{
				return attribute.ResourceID;
			}
		}

		/// <summary>
		/// Gets the file name.
		/// </summary>
		/// <returns> the file name </returns>
		public virtual string Name
		{
			get
			{
				return attribute.Name;
			}
			set
			{
				attribute.Name = value;
			}
		}


		/// <summary>
		/// Sets the owner name of this file.
		/// </summary>
		/// <param name="name"> the owner name </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setOwnerName(string name)
		{
			return attribute.setOwnerName(name);
		}

		/// <summary>
		/// Gets the owner name of this file.
		/// </summary>
		/// <returns> the owner name or <tt>null</tt> if empty </returns>
		public virtual string OwnerName
		{
			get
			{
				return attribute.OwnerName;
			}
		}

		/// <summary>
		/// Gets the file size (in MBytes).
		/// </summary>
		/// <returns> the file size (in MBytes) </returns>
		public virtual int Size
		{
			get
			{
				return attribute.FileSize;
			}
		}

		/// <summary>
		/// Gets the file size (in bytes).
		/// </summary>
		/// <returns> the file size (in bytes) </returns>
		public virtual int SizeInByte
		{
			get
			{
				return attribute.FileSizeInByte;
			}
		}

		/// <summary>
		/// Sets the file size (in MBytes).
		/// </summary>
		/// <param name="fileSize"> the file size (in MBytes) </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setFileSize(int fileSize)
		{
			return attribute.setFileSize(fileSize);
		}

		/// <summary>
		/// Sets the last update time of this file (in seconds). <br/>
		/// NOTE: This time is relative to the start time. Preferably use
		/// <seealso cref="gridsim.CloudSim#clock()"/> method.
		/// </summary>
		/// <param name="time"> the last update time (in seconds) </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setUpdateTime(double time)
		{
			return attribute.setUpdateTime(time);
		}

		/// <summary>
		/// Gets the last update time (in seconds).
		/// </summary>
		/// <returns> the last update time (in seconds) </returns>
		public virtual double LastUpdateTime
		{
			get
			{
				return attribute.LastUpdateTime;
			}
		}

		/// <summary>
		/// Sets the file registration ID (published by a Replica Catalogue entity).
		/// </summary>
		/// <param name="id"> registration ID </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setRegistrationID(int id)
		{
			return attribute.setRegistrationId(id);
		}

		/// <summary>
		/// Gets the file registration ID.
		/// </summary>
		/// <returns> registration ID </returns>
		public virtual int RegistrationID
		{
			get
			{
				return attribute.RegistrationId;
			}
		}

		/// <summary>
		/// Sets the file type (for instance, raw, tag, etc).
		/// </summary>
		/// <param name="type"> a file type </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setType(int type)
		{
			return attribute.setType(type);
		}

		/// <summary>
		/// Gets the file type.
		/// </summary>
		/// <returns> file type </returns>
		public virtual int Type
		{
			get
			{
				return attribute.Type;
			}
		}

		/// <summary>
		/// Sets the checksum of the file.
		/// </summary>
		/// <param name="checksum"> the checksum of this file </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setChecksum(int checksum)
		{
			return attribute.setChecksum(checksum);
		}

		/// <summary>
		/// Gets the file checksum.
		/// </summary>
		/// <returns> file checksum </returns>
		public virtual int Checksum
		{
			get
			{
				return attribute.Checksum;
			}
		}

		/// <summary>
		/// Sets the cost associated with the file.
		/// </summary>
		/// <param name="cost"> cost of this file </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		public virtual bool setCost(double cost)
		{
			return attribute.setCost(cost);
		}

		/// <summary>
		/// Gets the cost associated with the file.
		/// </summary>
		/// <returns> the cost of this file </returns>
		public virtual double Cost
		{
			get
			{
				return attribute.Cost;
			}
		}

		/// <summary>
		/// Gets the file creation time (in millisecond).
		/// </summary>
		/// <returns> the file creation time (in millisecond) </returns>
		public virtual long CreationTime
		{
			get
			{
				return attribute.CreationTime;
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
				return attribute.Registered;
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
				attribute.MasterCopy = value;
			}
			get
			{
				return attribute.MasterCopy;
			}
		}


		/// <summary>
		/// Marks the file as read-only or not.
		/// </summary>
		/// <param name="readOnly"> a flag denotes <tt>true</tt> for read only or <tt>false</tt> for re-writeable </param>
		public virtual bool ReadOnly
		{
			set
			{
				attribute.ReadOnly = value;
			}
			get
			{
				return attribute.ReadOnly;
			}
		}


		/// <summary>
		/// Sets the current transaction time (in second) of this file. This transaction time can be
		/// related to the operation of adding, deleting or getting the file on a resource's storage.
		/// </summary>
		/// <param name="time"> the transaction time (in second) </param>
		/// <returns> <tt>true</tt> if successful, <tt>false</tt> otherwise </returns>
		/// <seealso cref= gridsim.datagrid.storage.Storage#addFile(File) </seealso>
		/// <seealso cref= gridsim.datagrid.storage.Storage#addFile(List) </seealso>
		/// <seealso cref= gridsim.datagrid.storage.Storage#addReservedFile(File) </seealso>
		/// <seealso cref= gridsim.datagrid.storage.Storage#deleteFile(File) </seealso>
		/// <seealso cref= gridsim.datagrid.storage.Storage#deleteFile(String) </seealso>
		/// <seealso cref= gridsim.datagrid.storage.Storage#deleteFile(String, File) </seealso>
		/// <seealso cref= gridsim.datagrid.storage.Storage#getFile(String) </seealso>
		/// <seealso cref= gridsim.datagrid.storage.Storage#renameFile(File, String) </seealso>
		public virtual bool setTransactionTime(double time)
		{
			if (time < 0)
			{
				return false;
			}

			transactionTime = time;
			return true;
		}

		/// <summary>
		/// Gets the last transaction time of the file (in second).
		/// </summary>
		/// <returns> the transaction time (in second) </returns>
		public virtual double TransactionTime
		{
			get
			{
				return transactionTime;
			}

            set
            {
                transactionTime = value;
            }
		}
	}

}