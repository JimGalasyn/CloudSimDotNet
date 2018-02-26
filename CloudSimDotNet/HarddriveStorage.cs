using System;
using System.Collections.Generic;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim
{


	using ContinuousDistribution = org.cloudbus.cloudsim.distributions.ContinuousDistribution;

	/// <summary>
	/// An implementation of a storage system. It simulates the behavior of a typical hard drive storage.
	/// The default values for this storage are those of a "Maxtor DiamonMax 10 ATA" hard disk with the
	/// following parameters:
	/// <ul>
	///   <li>latency = 4.17 ms</li>
	///   <li>avg seek time = 9 m/s</li>
	///   <li>max transfer rate = 133 MB/sec</li>
	/// </ul>
	/// 
	/// @author Uros Cibej
	/// @author Anthony Sulistio
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public class HarddriveStorage : Storage
	{

		/// <summary>
		/// A list storing the names of all files on the hard drive. </summary>
		private IList<string> nameList;

		/// <summary>
		/// A list storing all files stored on the hard drive. </summary>
		private IList<File> fileList;

		/// <summary>
		/// The name of the hard drive. </summary>
		private readonly string name;

		/// <summary>
		/// A generator required to randomize the seek time. </summary>
		private ContinuousDistribution gen;

		/// <summary>
		/// The current size of files on the hard drive. </summary>
		private double currentSize;

		/// <summary>
		/// The total capacity of the hard drive in MB. </summary>
		private readonly double capacity;

        /// <summary>
        /// The maximum transfer rate in MB/sec. </summary>
        private double maxTransferRate;
        //private int maxTransferRate;

        /// <summary>
        /// The latency of the hard drive in seconds. </summary>
        private double latency;

		/// <summary>
		/// The average seek time in seconds. </summary>
		private double avgSeekTime;

		/// <summary>
		/// Creates a new hard drive storage with a given name and capacity.
		/// </summary>
		/// <param name="name"> the name of the new hard drive storage </param>
		/// <param name="capacity"> the capacity in MByte </param>
		/// <exception cref="ParameterException"> when the name and the capacity are not valid </exception>
		public HarddriveStorage(string name, double capacity)
		{
			if (string.ReferenceEquals(name, null) || name.Length == 0)
			{
				throw new ParameterException("HarddriveStorage(): Error - invalid storage name.");
			}

			if (capacity <= 0)
			{
				throw new ParameterException("HarddriveStorage(): Error - capacity <= 0.");
			}

			this.name = name;
			this.capacity = capacity;
			init();
		}

		/// <summary>
		/// Creates a new hard drive storage with a given capacity. In this case the name of the storage
		/// is a default name.
		/// </summary>
		/// <param name="capacity"> the capacity in MByte </param>
		/// <exception cref="ParameterException"> when the capacity is not valid </exception>
		public HarddriveStorage(double capacity)
		{
			if (capacity <= 0)
			{
				throw new ParameterException("HarddriveStorage(): Error - capacity <= 0.");
			}
			name = "HarddriveStorage";
			this.capacity = capacity;
			init();
		}

		/// <summary>
		/// The initialization of the hard drive is done in this method. The most common parameters, such
		/// as latency, average seek time and maximum transfer rate are set. The default values are set
		/// to simulate the "Maxtor DiamonMax 10 ATA" hard disk. Furthermore, the necessary lists are
		/// created.
		/// </summary>
		private void init()
		{
			fileList = new List<File>();
			nameList = new List<string>();
			gen = null;
			currentSize = 0;

			latency = 0.00417; // 4.17 ms in seconds
			avgSeekTime = 0.009; // 9 ms
			maxTransferRate = 133; // in MB/sec
		}

		public double AvailableSpace
		{
			get
			{
				return capacity - currentSize;
			}
		}

		public bool Full
		{
			get
			{
				if (Math.Abs(currentSize - capacity) < .0000001)
				{ // currentSize == capacity
					return true;
				}
				return false;
			}
		}

		public int NumStoredFile
		{
			get
			{
				return fileList.Count;
			}
		}

		public bool reserveSpace(int fileSize)
		{
			if (fileSize <= 0)
			{
				return false;
			}

			if (currentSize + fileSize >= capacity)
			{
				return false;
			}

			currentSize += fileSize;
			return true;
		}

		public virtual double addReservedFile(File file)
		{
			if (file == null)
			{
				return 0;
			}

			currentSize -= file.Size;
			double result = addFile(file);

			// if add file fails, then set the current size back to its old value
			if (result == 0.0)
			{
				currentSize += file.Size;
			}

			return result;
		}

		public bool hasPotentialAvailableSpace(int fileSize)
		{
			if (fileSize <= 0)
			{
				return false;
			}

			// check if enough space left
			if (AvailableSpace > fileSize)
			{
				return true;
			}

			IEnumerator<File> it = fileList.GetEnumerator();
			File file = null;
			int deletedFileSize = 0;

			// if not enough space, then if want to clear/delete some files
			// then check whether it still have space or not
			bool result = false;
			while (it.MoveNext())
			{
				file = it.Current;
				if (!file.ReadOnly)
				{
					deletedFileSize += file.Size;
				}

				if (deletedFileSize > fileSize)
				{
					result = true;
					break;
				}
			}

			return result;
		}

		public double Capacity
		{
			get
			{
				return capacity;
			}
		}

		public double CurrentSize
		{
			get
			{
				return currentSize;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		/// <summary>
		/// Sets the latency of this hard drive in seconds.
		/// </summary>
		/// <param name="latency"> the new latency in seconds </param>
		/// <returns> <tt>true</tt> if the setting succeeded, <tt>false</tt> otherwise </returns>
		public virtual bool setLatency(double latency)
		{
			if (latency < 0)
			{
				return false;
			}

			this.latency = latency;
			return true;
		}

		/// <summary>
		/// Gets the latency of this hard drive in seconds.
		/// </summary>
		/// <returns> the latency in seconds </returns>
		public virtual double Latency
		{
			get
			{
				return latency;
			}
		}

        public virtual double MaxTransferRate
        {
            get
            {
                return maxTransferRate;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("must be > 0", "MaxTransferRate");
                }

                maxTransferRate = value;
            }
        }


        public bool setMaxTransferRate(int rate)
		{
			if (rate <= 0)
			{
				return false;
			}

			maxTransferRate = rate;
			return true;
		}

		public double getMaxTransferRate()
		{
			return maxTransferRate;
		}

		/// <summary>
		/// Sets the average seek time of the storage in seconds.
		/// </summary>
		/// <param name="seekTime"> the average seek time in seconds </param>
		/// <returns> <tt>true</tt> if the setting succeeded, <tt>false</tt> otherwise </returns>
		public virtual bool setAvgSeekTime(double seekTime)
		{
			return setAvgSeekTime(seekTime, null);
		}

		/// <summary>
		/// Sets the average seek time and a new generator of seek times in seconds. The generator
		/// determines a randomized seek time.
		/// </summary>
		/// <param name="seekTime"> the average seek time in seconds </param>
		/// <param name="gen"> the ContinuousGenerator which generates seek times </param>
		/// <returns> <tt>true</tt> if the setting succeeded, <tt>false</tt> otherwise </returns>
		public virtual bool setAvgSeekTime(double seekTime, ContinuousDistribution gen)
		{
			if (seekTime <= 0.0)
			{
				return false;
			}

			avgSeekTime = seekTime;
			this.gen = gen;
			return true;
		}

		/// <summary>
		/// Gets the average seek time of the hard drive in seconds.
		/// </summary>
		/// <returns> the average seek time in seconds </returns>
		public virtual double AvgSeekTime
		{
			get
			{
				return avgSeekTime;
			}
		}

		public File getFile(string fileName)
		{
			// check first whether file name is valid or not
			File obj = null;
			if (string.ReferenceEquals(fileName, null) || fileName.Length == 0)
			{
				Log.printConcatLine(name, ".getFile(): Warning - invalid " + "file name.");
				return obj;
			}

			IEnumerator<File> it = fileList.GetEnumerator();
			int size = 0;
			int index = 0;
			bool found = false;
			File tempFile = null;

			// find the file in the disk
			while (it.MoveNext())
			{
				tempFile = it.Current;
				size += tempFile.Size;
				if (tempFile.Name.Equals(fileName))
				{
					found = true;
					obj = tempFile;
					break;
				}

				index++;
			}

			// if the file is found, then determine the time taken to get it
			if (found)
			{
				obj = fileList[index];
				double seekTime = getSeekTime(size);
				double transferTime = getTransferTime(obj.Size);

				// total time for this operation
				obj.TransactionTime = seekTime + transferTime;
			}

			return obj;
		}

		public IList<string> FileNameList
		{
			get
			{
				return nameList;
			}
		}

		/// <summary>
		/// Get the seek time for a file with the defined size. Given a file size in MB, this method
		/// returns a seek time for the file in seconds.
		/// </summary>
		/// <param name="fileSize"> the size of a file in MB </param>
		/// <returns> the seek time in seconds </returns>
		private double getSeekTime(int fileSize)
		{
			double result = 0;

			if (gen != null)
			{
				result += gen.sample();
			}

			if (fileSize > 0 && capacity != 0)
			{
				result += (fileSize / capacity);
			}

			return result;
		}

		/// <summary>
		/// Gets the transfer time of a given file.
		/// </summary>
		/// <param name="fileSize"> the size of the transferred file </param>
		/// <returns> the transfer time in seconds </returns>
		private double getTransferTime(int fileSize)
		{
			double result = 0;
			if (fileSize > 0 && capacity != 0)
			{
				result = (fileSize * maxTransferRate) / capacity;
			}

			return result;
		}

		/// <summary>
		/// Check if the file is valid or not. This method checks whether the given file or the file name
		/// of the file is valid. The method name parameter is used for debugging purposes, to output in
		/// which method an error has occurred.
		/// </summary>
		/// <param name="file"> the file to be checked for validity </param>
		/// <param name="methodName"> the name of the method in which we check for validity of the file </param>
		/// <returns> <tt>true</tt> if the file is valid, <tt>false</tt> otherwise </returns>
		private bool isFileValid(File file, string methodName)
		{

			if (file == null)
			{
				Log.printConcatLine(name, ".", methodName, ": Warning - the given file is null.");
				return false;
			}

			string fileName = file.Name;
			if (string.ReferenceEquals(fileName, null) || fileName.Length == 0)
			{
				Log.printConcatLine(name, "." + methodName, ": Warning - invalid file name.");
				return false;
			}

			return true;
		}

		/// <summary>
		/// {@inheritDoc}
		/// 
		/// <p/>First, the method checks if there is enough space on the storage,
		/// then it checks if the file with the same name is already taken to avoid duplicate filenames. 
		/// </summary>
		/// <param name="file"> {@inheritDoc} </param>
		/// <returns> {@inheritDoc} </returns>
		public virtual double addFile(File file)
		{
			double result = 0.0;
			// check if the file is valid or not
			if (!isFileValid(file, "addFile()"))
			{
				return result;
			}

			// check the capacity
			if (file.Size + currentSize > capacity)
			{
				Log.printConcatLine(name, ".addFile(): Warning - not enough space to store ", file.Name);
				return result;
			}

			// check if the same file name is alredy taken
			if (!contains(file.Name))
			{
				double seekTime = getSeekTime(file.Size);
				double transferTime = getTransferTime(file.Size);

				fileList.Add(file); // add the file into the HD
				nameList.Add(file.Name); // add the name to the name list
				currentSize += file.Size; // increment the current HD size
				result = seekTime + transferTime; // add total time
			}
			file.TransactionTime = result;
			return result;
		}

		public virtual double addFile(IList<File> list)
		{
			double result = 0.0;
			if (list == null || list.Count == 0)
			{
				Log.printConcatLine(name, ".addFile(): Warning - list is empty.");
				return result;
			}

			IEnumerator<File> it = list.GetEnumerator();
			File file = null;
			while (it.MoveNext())
			{
				file = it.Current;
				result += addFile(file); // add each file in the list
			}
			return result;
		}

		public File deleteFile(string fileName)
		{
			if (string.ReferenceEquals(fileName, null) || fileName.Length == 0)
			{
				return null;
			}

			IEnumerator<File> it = fileList.GetEnumerator();
			File file = null;
			while (it.MoveNext())
			{
				file = it.Current;
				string name = file.Name;

				// if a file is found then delete
				if (fileName.Equals(name))
				{
					double result = deleteFile(file);
					file.TransactionTime = result;
					break;
				}
				else
				{
					file = null;
				}
			}
			return file;
		}

		public virtual double deleteFile(string fileName, File file)
		{
			return deleteFile(file);
		}

		public virtual double deleteFile(File file)
		{
			double result = 0.0;
			// check if the file is valid or not
			if (!isFileValid(file, "deleteFile()"))
			{
				return result;
			}
			double seekTime = getSeekTime(file.Size);
			double transferTime = getTransferTime(file.Size);

			// check if the file is in the storage
			if (contains(file))
			{
				fileList.Remove(file); // remove the file HD
				nameList.Remove(file.Name); // remove the name from name list
				currentSize -= file.Size; // decrement the current HD space
				result = seekTime + transferTime; // total time
				file.TransactionTime = result;
			}
			return result;
		}

		public bool contains(string fileName)
		{
			bool result = false;
			if (string.ReferenceEquals(fileName, null) || fileName.Length == 0)
			{
				Log.printConcatLine(name, ".contains(): Warning - invalid file name");
				return result;
			}
			// check each file in the list
			IEnumerator<string> it = nameList.GetEnumerator();
			while (it.MoveNext())
			{
				string name = it.Current;
				if (name.Equals(fileName))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public bool contains(File file)
		{
			bool result = false;
			if (!isFileValid(file, "contains()"))
			{
				return result;
			}

			result = contains(file.Name);
			return result;
		}

		public bool renameFile(File file, string newName)
		{
			// check whether the new filename is conflicting with existing ones
			// or not
			bool result = false;
			if (contains(newName))
			{
				return result;
			}

			// replace the file name in the file (physical) list
			File obj = getFile(file.Name);
			if (obj == null)
			{
				return result;
			}
			else
			{
				obj.Name = newName;
			}

			// replace the file name in the name list
			IEnumerator<string> it = nameList.GetEnumerator();
			while (it.MoveNext())
			{
				string name = it.Current;
				if (name.Equals(file.Name))
				{
					file.TransactionTime = 0;
					nameList.Remove(name);
					nameList.Add(newName);
					result = true;
					break;
				}
			}

			return result;
		}

	}

}