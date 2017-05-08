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

	/// <summary>
	/// An interface which defines the desired functionality of a storage system in a Data Cloud. The
	/// classes that implement this interface should simulate the characteristics of different storage
	/// systems by setting the capacity of the storage and the maximum transfer rate. The transfer rate
	/// defines the time required to execute some common operations on the storage, e.g. storing a file,
	/// getting a file and deleting a file.
	/// 
	/// @author Uros Cibej
	/// @author Anthony Sulistio
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public interface Storage
	{

		/// <summary>
		/// Gets the name of the storage.
		/// </summary>
		/// <returns> the name of this storage </returns>
		string Name {get;}

		/// <summary>
		/// Gets the total capacity of the storage in MByte.
		/// </summary>
		/// <returns> the capacity of the storage in MB </returns>
		double Capacity {get;}

		/// <summary>
		/// Gets the current size of the storage in MByte.
		/// </summary>
		/// <returns> the current size of the storage in MB </returns>
		double CurrentSize {get;}

		/// <summary>
		/// Gets the maximum transfer rate of the storage in MByte/sec.
		/// </summary>
		/// <returns> the maximum transfer rate in MB/sec </returns>
		double getMaxTransferRate();

        /// <summary>
        /// Sets the maximum transfer rate of this storage system in MByte/sec.
        /// </summary>
        /// <param name="rate"> the maximum transfer rate in MB/sec </param>
        /// <returns> <tt>true</tt> if the setting succeeded, <tt>false</tt> otherwise </returns>
        bool setMaxTransferRate(int rate);

        /// <summary>
        /// Gets or sets the maximum transfer rate of this storage system in MByte/sec.
        /// </summary>
        double MaxTransferRate { get; set; }

        /// <summary>
        /// Gets the available space on this storage in MByte.
        /// </summary>
        /// <returns> the available space in MB </returns>
        double AvailableSpace {get;}

		/// <summary>
		/// Checks if the storage is full or not.
		/// </summary>
		/// <returns> <tt>true</tt> if the storage is full, <tt>false</tt> otherwise </returns>
		bool Full {get;}

		/// <summary>
		/// Gets the number of files stored on this device.
		/// </summary>
		/// <returns> the number of stored files </returns>
		int NumStoredFile {get;}

		/// <summary>
		/// Makes reservation of space on the storage to store a file.
		/// </summary>
		/// <param name="fileSize"> the size to be reserved in MB </param>
		/// <returns> <tt>true</tt> if reservation succeeded, <tt>false</tt> otherwise </returns>
		bool reserveSpace(int fileSize);

		/// <summary>
		/// Adds a file for which the space has already been reserved. The time taken (in seconds) for
		/// adding the specified file can also be found using
		/// <seealso cref="org.cloudbus.cloudsim.File#getTransactionTime()"/>.
		/// </summary>
		/// <param name="file"> the file to be added </param>
		/// <returns> the time (in seconds) required to add the file </returns>
		double addReservedFile(File file);

		/// <summary>
		/// Checks whether there is enough space on the storage for a certain file.
		/// </summary>
		/// <param name="fileSize"> a FileAttribute object to compare to </param>
		/// <returns> <tt>true</tt> if enough space available, <tt>false</tt> otherwise </returns>
		bool hasPotentialAvailableSpace(int fileSize);

		/// <summary>
		/// Gets the file with the specified name. The time taken (in seconds) for getting the specified
		/// file can also be found using <seealso cref="org.cloudbus.cloudsim.File#getTransactionTime()"/>.
		/// </summary>
		/// <param name="fileName"> the name of the needed file </param>
		/// <returns> the file with the specified filename </returns>
		File getFile(string fileName);

		/// <summary>
		/// Gets the list of file names located on this storage.
		/// </summary>
		/// <returns> a List of file names </returns>
		IList<string> FileNameList {get;}

		/// <summary>
		/// Adds a file to the storage. The time taken (in seconds) for adding the specified file can
		/// also be found using <seealso cref="org.cloudbus.cloudsim.File#getTransactionTime()"/>.
		/// </summary>
		/// <param name="file"> the file to be added </param>
		/// <returns> the time taken (in seconds) for adding the specified file </returns>
		double addFile(File file);

		/// <summary>
		/// Adds a set of files to the storage. The time taken (in seconds) for adding each file can also
		/// be found using <seealso cref="gridsim.datagrid.File#getTransactionTime()"/>.
		/// </summary>
		/// <param name="list"> the files to be added </param>
		/// <returns> the time taken (in seconds) for adding the specified files </returns>
		double addFile(IList<File> list);

		/// <summary>
		/// Removes a file from the storage. The time taken (in seconds) for deleting the specified file
		/// can be found using <seealso cref="gridsim.datagrid.File#getTransactionTime()"/>.
		/// </summary>
		/// <param name="fileName"> the name of the file to be removed </param>
		/// <returns> the deleted file. </returns>
		File deleteFile(string fileName);

		/// <summary>
		/// Removes a file from the storage. The time taken (in seconds) for deleting the specified file
		/// can also be found using <seealso cref="gridsim.datagrid.File#getTransactionTime()"/>.
		/// </summary>
		/// <param name="fileName"> the name of the file to be removed </param>
		/// <param name="file"> the file removed from the storage is returned through this parameter </param>
		/// <returns> the time taken (in seconds) for deleting the specified file </returns>
		double deleteFile(string fileName, File file);

		/// <summary>
		/// Removes a file from the storage. The time taken (in seconds) for deleting the specified file
		/// can also be found using <seealso cref="gridsim.datagrid.File#getTransactionTime()"/>.
		/// </summary>
		/// <param name="file"> the file to be removed </param>
		/// <returns> the time taken (in seconds) for deleting the specified file </returns>
		double deleteFile(File file);

		/// <summary>
		/// Checks whether a file exists in the storage or not.
		/// </summary>
		/// <param name="fileName"> the name of the file we are looking for </param>
		/// <returns> <tt>true</tt> if the file is in the storage, <tt>false</tt> otherwise </returns>
		bool contains(string fileName);

		/// <summary>
		/// Checks whether a file is stored in the storage or not.
		/// </summary>
		/// <param name="file"> the file we are looking for </param>
		/// <returns> <tt>true</tt> if the file is in the storage, <tt>false</tt> otherwise </returns>
		bool contains(File file);

		/// <summary>
		/// Renames a file on the storage. The time taken (in seconds) for renaming the specified file
		/// can also be found using <seealso cref="org.cloudbus.cloudsim.File#getTransactionTime()"/>.
		/// </summary>
		/// <param name="file"> the file we would like to rename </param>
		/// <param name="newName"> the new name of the file </param>
		/// <returns> <tt>true</tt> if the renaming succeeded, <tt>false</tt> otherwise </returns>
		bool renameFile(File file, string newName);

	}

}