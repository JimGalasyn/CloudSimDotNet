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
	/// This class contains additional tags for the DataCloud functionalities, such as file information
	/// retrieval, file transfers, and storage info.
	/// 
	/// @author Uros Cibej
	/// @author Anthony Sulistio
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public sealed class DataCloudTags
	{

		// to prevent a conflict with the existing CloudSimTags values
			/// <summary>
			/// Base value used for other general tags.
			/// </summary>
		private const int BASE = 400;

			/// <summary>
			/// Base value used for Replica Manager tags.
			/// </summary>
		private const int RM_BASE = 500;

			/// <summary>
			/// Base value for catalogue tags.
			/// </summary>
		private const int CTLG_BASE = 600;

		// ////////// GENERAL TAGS

		/// <summary>
		/// Default Maximum Transmission Unit (MTU) of a link in bytes. </summary>
		public const int DEFAULT_MTU = 1500;

		/// <summary>
		/// The default packet size (in byte) for sending events to other entity. </summary>
		public static readonly int PKT_SIZE = DEFAULT_MTU * 100; // in bytes

		/// <summary>
		/// The default storage size (10 GB in byte). </summary>
		public const int DEFAULT_STORAGE_SIZE = 10000000;

		/// <summary>
		/// Registers a Replica Catalogue (RC) entity to a Data GIS. </summary>
		public static readonly int REGISTER_REPLICA_CTLG = BASE + 1;

		/// <summary>
		/// Denotes a list of all Replica Catalogue (RC) entities that are listed in this regional Data
		/// GIS entity. This tag should be called from a user to Data GIS.
		/// </summary>
		public static readonly int INQUIRY_LOCAL_RC_LIST = BASE + 2;

		/// <summary>
		/// Denotes a list of Replica Catalogue (RC) entities that are listed in other regional Data GIS
		/// entities. This tag should be called from a user to Data GIS.
		/// </summary>
		public static readonly int INQUIRY_GLOBAL_RC_LIST = BASE + 3;

		/// <summary>
		/// Denotes a list of Replica Catalogue IDs. This tag should be called from a Regional Data GIS
		/// to another
		/// </summary>
		public static readonly int INQUIRY_RC_LIST = BASE + 4;

		/// <summary>
		/// Denotes a result regarding to a list of Replica Catalogue IDs. This tag should be called from
		/// a Regional Data GIS to a sender Regional Data GIS.
		/// </summary>
		public static readonly int INQUIRY_RC_RESULT = BASE + 5;

		/// <summary>
		/// Denotes the submission of a DataCloudlet. This tag is normally used between user and
		/// DataCloudResource entity.
		/// </summary>
		public static readonly int DATAcloudlet_SUBMIT = BASE + 6;

		// ////////// REPLICA MANAGER TAGS

		// ***********************User <--> RM******************************//

		/// <summary>
		/// Requests for a file that is stored on the local storage(s).
		/// <br/>The format of this request is Object[2] = {String lfn, Integer senderID}.<br/>
		/// The reply tag name is <seealso cref="#FILE_DELIVERY"/>.
		/// </summary>
		public static readonly int FILE_REQUEST = RM_BASE + 1;

		/// <summary>
		/// Sends the file to the requester. The format of the reply is File or null if error happens
		/// </summary>
		public static readonly int FILE_DELIVERY = RM_BASE + 2;

		// /////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Requests for a master file to be added to the local storage(s).
		/// <br/>The format of this request is Object[2] = {File obj, Integer senderID}.<br/>
		/// The reply tag name is <seealso cref="#FILE_ADD_MASTER_RESULT"/>.
		/// </summary>
		public static readonly int FILE_ADD_MASTER = RM_BASE + 10;

		/// <summary>
		/// Sends the result of adding a master file back to sender.
		/// <br/>The format of the reply is Object[3] = {String lfn, Integer uniqueID, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of FILE_ADD_XXXX where XXXX means the error/success
		/// message.
		/// </summary>
		public static readonly int FILE_ADD_MASTER_RESULT = RM_BASE + 11;

		/// <summary>
		/// Requests for a replica file to be added from the local storage(s).
		/// <br/>The format of this request is Object[2] = {File obj, Integer senderID}.
		/// <br/>The reply tag name is <seealso cref="#FILE_ADD_REPLICA_RESULT"/>.
		/// </summary>
		public static readonly int FILE_ADD_REPLICA = RM_BASE + 12;

		/// <summary>
		/// Sends the result of adding a replica file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of FILE_ADD_XXXX where XXXX means the error/success
		/// message
		/// </summary>
		public static readonly int FILE_ADD_REPLICA_RESULT = RM_BASE + 13;

		/// <summary>
		/// Denotes that file addition is successful. </summary>
		public static readonly int FILE_ADD_SUCCESSFUL = RM_BASE + 20;

		/// <summary>
		/// Denotes that file addition is failed because the storage is full. </summary>
		public static readonly int FILE_ADD_ERROR_STORAGE_FULL = RM_BASE + 21;

		/// <summary>
		/// Denotes that file addition is failed because the given file is empty. </summary>
		public static readonly int FILE_ADD_ERROR_EMPTY = RM_BASE + 22;

		/// <summary>
		/// Denotes that file addition is failed because the file already exists in the catalogue and it
		/// is read-only file.
		/// </summary>
		public static readonly int FILE_ADD_ERROR_EXIST_READ_ONLY = RM_BASE + 23;

		/// <summary>
		/// Denotes that file addition is failed due to an unknown error. </summary>
		public static readonly int FILE_ADD_ERROR = RM_BASE + 24;

		/// <summary>
		/// Denotes that file addition is failed because access/permission denied or not authorized.
		/// </summary>
		public static readonly int FILE_ADD_ERROR_ACCESS_DENIED = RM_BASE + 25;

		// /////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Requests for a master file to be deleted from the local storage(s).
		/// <br/>The format of this request is Object[2] = {String lfn, Integer senderID}.
		/// <br/>The reply tag name is <seealso cref="#FILE_DELETE_MASTER_RESULT"/>.
		/// </summary>
		public static readonly int FILE_DELETE_MASTER = RM_BASE + 30;

		/// <summary>
		/// Sends the result of deleting a master file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of FILE_DELETE_XXXX where XXXX means the error/success
		/// message
		/// </summary>
		public static readonly int FILE_DELETE_MASTER_RESULT = RM_BASE + 31;

		/// <summary>
		/// Requests for a replica file to be deleted from the local storage(s).
		/// <br/>The format of this request is Object[2] = {String lfn, Integer senderID}.
		/// <br/>The reply tag name is <seealso cref="#FILE_DELETE_REPLICA_RESULT"/>.
		/// </summary>
		public static readonly int FILE_DELETE_REPLICA = RM_BASE + 32;

		/// <summary>
		/// Sends the result of deleting a replica file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of FILE_DELETE_XXXX where XXXX means the error/success
		/// message
		/// </summary>
		public static readonly int FILE_DELETE_REPLICA_RESULT = RM_BASE + 33;

		/// <summary>
		/// Denotes that file deletion is successful. </summary>
		public static readonly int FILE_DELETE_SUCCESSFUL = RM_BASE + 40;

		/// <summary>
		/// Denotes that file deletion is failed due to an unknown error. </summary>
		public static readonly int FILE_DELETE_ERROR = RM_BASE + 41;

		/// <summary>
		/// Denotes that file deletion is failed because it is a read-only file. </summary>
		public static readonly int FILE_DELETE_ERROR_READ_ONLY = RM_BASE + 42;

		/// <summary>
		/// Denotes that file deletion is failed because the file does not exist in the storage nor
		/// catalogue.
		/// </summary>
		public static readonly int FILE_DELETE_ERROR_DOESNT_EXIST = RM_BASE + 43;

		/// <summary>
		/// Denotes that file deletion is failed because it is currently used by others.
		/// </summary>
		public static readonly int FILE_DELETE_ERROR_IN_USE = RM_BASE + 44;

		/// <summary>
		/// Denotes that file deletion is failed because access/permission denied or not authorized.
		/// </summary>
		public static readonly int FILE_DELETE_ERROR_ACCESS_DENIED = RM_BASE + 45;

		// /////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Requests for a file to be modified from the local storage(s).
		/// <br/>The format of this request is Object[2] = {File obj, Integer senderID}.
		/// <br/>The reply tag name is <seealso cref="#FILE_MODIFY_RESULT"/>.
		/// </summary>
		public static readonly int FILE_MODIFY = RM_BASE + 50;

		/// <summary>
		/// Sends the result of deleting a file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of FILE_MODIFY_XXXX where XXXX means the error/success
		/// message
		/// </summary>
		public static readonly int FILE_MODIFY_RESULT = RM_BASE + 51;

		/// <summary>
		/// Denotes that file modification is successful. </summary>
		public static readonly int FILE_MODIFY_SUCCESSFUL = RM_BASE + 60;

		/// <summary>
		/// Denotes that file modification is failed due to an unknown error. </summary>
		public static readonly int FILE_MODIFY_ERROR = RM_BASE + 61;

		/// <summary>
		/// Denotes that file modification is failed because it is a read-only file.
		/// </summary>
		public static readonly int FILE_MODIFY_ERROR_READ_ONLY = RM_BASE + 62;

		/// <summary>
		/// Denotes that file modification is failed because the file does not exist.
		/// </summary>
		public static readonly int FILE_MODIFY_ERROR_DOESNT_EXIST = RM_BASE + 63;

		/// <summary>
		/// Denotes that file modification is failed because the file is currently used by others.
		/// </summary>
		public static readonly int FILE_MODIFY_ERROR_IN_USE = RM_BASE + 64;

		/// <summary>
		/// Denotes that file modification is failed because access/permission denied or not authorized.
		/// </summary>
		public static readonly int FILE_MODIFY_ERROR_ACCESS_DENIED = RM_BASE + 65;

		// ////////// REPLICA CATALOGUE TAGS

		// ***********************User<-->RC******************************//

		/// <summary>
		/// Denotes the request for a location of a replica file.
		/// <br/>The format of this request is Object[2] = {String lfn, Integer senderID}.
		/// <br/>The reply tag name is <seealso cref="#CTLG_REPLICA_DELIVERY"/>.
		/// <br/>NOTE: This request only ask for one location only not all.
		/// </summary>
		public static readonly int CTLG_GET_REPLICA = CTLG_BASE + 1;

		/// <summary>
		/// Sends the result for a location of a replica file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, Integer resourceID}.
		/// <br/>NOTE: The resourceID could be <tt>-1</tt> if not found.
		/// </summary>
		public static readonly int CTLG_REPLICA_DELIVERY = CTLG_BASE + 2;

		/// <summary>
		/// Denotes the request for all locations of a replica file.
		/// <br/>The format of this request is Object[2] = {String lfn, Integer senderID}.
		/// <br/>The reply tag name is <seealso cref="#CTLG_REPLICA_LIST_DELIVERY"/>.
		/// </summary>
		public static readonly int CTLG_GET_REPLICA_LIST = CTLG_BASE + 3;

		/// <summary>
		/// Sends the result for all locations of a replica file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, List locationList}.
		/// <br/>NOTE: The locationList could be <tt>null</tt> if not found.
		/// </summary>
		public static readonly int CTLG_REPLICA_LIST_DELIVERY = CTLG_BASE + 4;

		/// <summary>
		/// Denotes the request to get the attribute of a file.
		/// <br/>The format of this request is Object[2] = {String lfn, Integer senderID}.
		/// <br/>The reply tag name is <seealso cref="#CTLG_FILE_ATTR_DELIVERY"/>.
		/// </summary>
		public static readonly int CTLG_GET_FILE_ATTR = CTLG_BASE + 5;

		/// <summary>
		/// Sends the result for a file attribute back to sender.
		/// <br/>The format of the reply is {FileAttribute fileAttr}
		/// <br/>NOTE: The fileAttr could be <tt>null</tt> if not found.
		/// </summary>
		public static readonly int CTLG_FILE_ATTR_DELIVERY = CTLG_BASE + 6;

		/// <summary>
		/// Denotes the request to get a list of file attributes based on the given filter.
		/// <br/>The format of this request is Object[2] = {Filter filter, Integer senderID}
		/// <br/>The reply tag name is <seealso cref="#CTLG_FILTER_DELIVERY"/>.
		/// </summary>
		public static readonly int CTLG_FILTER = CTLG_BASE + 7;

		/// <summary>
		/// Sends the result for a list of file attributes back to sender.
		/// <br/>The format of the reply is {List attrList}.
		/// <br/>NOTE: The attrList could be <tt>null</tt> if not found.
		/// </summary>
		public static readonly int CTLG_FILTER_DELIVERY = CTLG_BASE + 8;

		// ***********************RM<-->RC******************************//

		/// <summary>
		/// Denotes the request to register / add a master file to the Replica Catalogue.
		/// <br/>The format of this request is Object[3] = {String filename, FileAttribute attr, Integer
		/// resID}.
		/// <br/>The reply tag name is <seealso cref="#CTLG_ADD_MASTER_RESULT"/>.
		/// </summary>
		public static readonly int CTLG_ADD_MASTER = CTLG_BASE + 10;

		/// <summary>
		/// Sends the result of registering a master file back to sender.
		/// <br/>The format of the reply is Object[3] = {String filename, Integer uniqueID, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of CTLG_ADD_MASTER_XXXX where XXXX means the error/success
		/// message
		/// </summary>
		public static readonly int CTLG_ADD_MASTER_RESULT = CTLG_BASE + 11;

		/// <summary>
		/// Denotes that master file addition is successful. </summary>
		public static readonly int CTLG_ADD_MASTER_SUCCESSFUL = CTLG_BASE + 12;

		/// <summary>
		/// Denotes that master file addition is failed due to an unknown error. </summary>
		public static readonly int CTLG_ADD_MASTER_ERROR = CTLG_BASE + 13;

		/// <summary>
		/// Denotes that master file addition is failed due to the catalogue is full.
		/// </summary>
		public static readonly int CTLG_ADD_MASTER_ERROR_FULL = CTLG_BASE + 14;

		// /////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Denotes the request to de-register / delete a master file from the Replica Catalogue.
		/// <br/>The format of this request is Object[2] = {String lfn, Integer resourceID}.
		/// <br/>The reply tag name is <seealso cref="#CTLG_DELETE_MASTER_RESULT"/>.
		/// </summary>
		public static readonly int CTLG_DELETE_MASTER = CTLG_BASE + 20;

		/// <summary>
		/// Sends the result of de-registering a master file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of CTLG_DELETE_MASTER_XXXX where XXXX means the
		/// error/success message
		/// </summary>
		public static readonly int CTLG_DELETE_MASTER_RESULT = CTLG_BASE + 21;

		/// <summary>
		/// Denotes that master file deletion is successful. </summary>
		public static readonly int CTLG_DELETE_MASTER_SUCCESSFUL = CTLG_BASE + 22;

		/// <summary>
		/// Denotes that master file deletion is failed due to an unknown error. </summary>
		public static readonly int CTLG_DELETE_MASTER_ERROR = CTLG_BASE + 23;

		/// <summary>
		/// Denotes that master file deletion is failed because the file does not exist in the catalogue.
		/// </summary>
		public static readonly int CTLG_DELETE_MASTER_DOESNT_EXIST = CTLG_BASE + 24;

		/// <summary>
		/// Denotes that master file deletion is failed because replica files are still in the catalogue.
		/// All replicas need to be deleted first.
		/// </summary>
		public static readonly int CTLG_DELETE_MASTER_REPLICAS_EXIST = CTLG_BASE + 25;

		// /////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Denotes the request to register / add a replica file to the Replica Catalogue.
		/// <br/>The format of this request is Object[2] = {String lfn, Integer resourceID}.
		/// <br/>The reply tag name is <seealso cref="#CTLG_ADD_REPLICA_RESULT"/>.
		/// </summary>
		public static readonly int CTLG_ADD_REPLICA = CTLG_BASE + 30;

		/// <summary>
		/// Sends the result of registering a replica file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of CTLG_ADD_REPLICA_XXXX where XXXX means the
		/// error/success message
		/// </summary>
		public static readonly int CTLG_ADD_REPLICA_RESULT = CTLG_BASE + 31;

		/// <summary>
		/// Denotes that replica file addition is successful. </summary>
		public static readonly int CTLG_ADD_REPLICA_SUCCESSFUL = CTLG_BASE + 32;

		/// <summary>
		/// Denotes that replica file addition is failed due to an unknown error. </summary>
		public static readonly int CTLG_ADD_REPLICA_ERROR = CTLG_BASE + 33;

		/// <summary>
		/// Denotes that replica file addition is failed because the given file name does not exist in
		/// the catalogue.
		/// </summary>
		public static readonly int CTLG_ADD_REPLICA_ERROR_DOESNT_EXIST = CTLG_BASE + 34;

		/// <summary>
		/// Denotes that replica file addition is failed due to the catalogue is full.
		/// </summary>
		public static readonly int CTLG_ADD_REPLICA_ERROR_FULL = CTLG_BASE + 35;

		// /////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Denotes the request to de-register / delete a replica file from the Replica Catalogue.<br/>
		/// The format of this request is Object[2] = {String lfn, Integer resourceID}.<br/>
		/// The reply tag name is <seealso cref="#CTLG_DELETE_REPLICA_RESULT"/>.
		/// </summary>
		public static readonly int CTLG_DELETE_REPLICA = CTLG_BASE + 40;

		/// <summary>
		/// Sends the result of de-registering a replica file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of CTLG_DELETE_REPLICA_XXXX where XXXX means the
		/// error/success message
		/// </summary>
		public static readonly int CTLG_DELETE_REPLICA_RESULT = CTLG_BASE + 41;

		/// <summary>
		/// Denotes that replica file deletion is successful. </summary>
		public static readonly int CTLG_DELETE_REPLICA_SUCCESSFUL = CTLG_BASE + 42;

		/// <summary>
		/// Denotes that replica file deletion is failed due to an unknown error. </summary>
		public static readonly int CTLG_DELETE_REPLICA_ERROR = CTLG_BASE + 43;

		/// <summary>
		/// Denotes that replica file deletion is failed because the file does not exist in the catalogue.
		/// </summary>
		public static readonly int CTLG_DELETE_REPLICA_ERROR_DOESNT_EXIST = CTLG_BASE + 44;

		// /////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Denotes the request to modify an existing master file information stored in the Replica
		/// Catalogue.
		/// <br/>The format of this request is Object[3] = {String filename, FileAttribute attr, Integer
		/// resID}.
		/// <br/>The reply tag name is <seealso cref="#CTLG_MODIFY_MASTER_RESULT"/>.
		/// </summary>
		public static readonly int CTLG_MODIFY_MASTER = CTLG_BASE + 50;

		/// <summary>
		/// Sends the result of modifying a master file back to sender.
		/// <br/>The format of the reply is Object[2] = {String lfn, Integer resultID}.
		/// <br/>NOTE: The result id is in the form of CTLG_MODIFY_MASTER_XXXX where XXXX means the
		/// error/success message
		/// </summary>
		public static readonly int CTLG_MODIFY_MASTER_RESULT = CTLG_BASE + 51;

		/// <summary>
		/// Denotes that master file deletion is successful. </summary>
		public static readonly int CTLG_MODIFY_MASTER_SUCCESSFUL = CTLG_BASE + 52;

		/// <summary>
		/// Denotes that master file modification is failed due to an unknown error.
		/// </summary>
		public static readonly int CTLG_MODIFY_MASTER_ERROR = CTLG_BASE + 53;

		/// <summary>
		/// Denotes that master file modification is failed because the file does not exist in the
		/// catalogue.
		/// </summary>
		public static readonly int CTLG_MODIFY_MASTER_ERROR_DOESNT_EXIST = CTLG_BASE + 54;

		/// <summary>
		/// Denotes that master file modification is failed because the file attribute is set to a
		/// read-only.
		/// </summary>
		public static readonly int CTLG_MODIFY_MASTER_ERROR_READ_ONLY = CTLG_BASE + 55;

		// /////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Private Constructor. </summary>
		private DataCloudTags()
		{
			throw new System.NotSupportedException("DataCloudTags cannot be instantiated");
		}

	}

}