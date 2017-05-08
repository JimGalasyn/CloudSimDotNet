/*
 * Gokul Poduval & Chen-Khong Tham
 * Computer Communication Networks (CCN) Lab
 * Dept of Electrical & Computer Engineering
 * National University of Singapore
 * August 2004
 *
 * Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * Copyright (c) 2004, The University of Melbourne, Australia and National
 * University of Singapore
 * Packet.java - Interface of a Network Packet.
 *
 */

namespace org.cloudbus.cloudsim
{

	/// <summary>
	/// Defines the structure for a network packet.
	/// 
	/// @author Gokul Poduval
	/// @author Chen-Khong Tham, National University of Singapore
	/// @since CloudSim Toolkit 1.0
	/// </summary>
	public interface Packet
	{

		/// <summary>
		/// Returns a string describing this packet in detail.
		/// </summary>
		/// <returns> description of this packet
		/// @pre $none
		/// @post $none </returns>
		string ToString();

		/// <summary>
		/// Returns the size of this packet
		/// </summary>
		/// <returns> size of the packet
		/// @pre $none
		/// @post $none </returns>
		long Size {get;}

		/// <summary>
		/// Sets the size of this packet
		/// </summary>
		/// <param name="size"> size of the packet </param>
		/// <returns> <tt>true</tt> if it is successful, <tt>false</tt> otherwise
		/// @pre size >= 0
		/// @post $none </returns>
		bool setSize(long size);

		/// <summary>
		/// Returns the destination id of this packet.
		/// </summary>
		/// <returns> destination id
		/// @pre $none
		/// @post $none </returns>
		int DestId {get;}

		/// <summary>
		/// Returns the ID of this packet
		/// </summary>
		/// <returns> packet ID
		/// @pre $none
		/// @post $none </returns>
		int Id {get;}

		/// <summary>
		/// Returns the ID of the source of this packet.
		/// </summary>
		/// <returns> source id
		/// @pre $none
		/// @post $none </returns>
		int SrcId {get;}

		/// <summary>
		/// Gets the network service type of this packet
		/// </summary>
		/// <returns> the network service type
		/// @pre $none
		/// @post $none
		/// 
		/// @todo Is it the Type of Service (ToS) of IPv4, like in
		/// the <seealso cref="Cloudlet#netToS"/>? If yes, so the names would
		/// be standardized. </returns>
		int NetServiceType {get;set;}


		/// <summary>
		/// Gets an entity ID from the last hop that this packet has traversed.
		/// </summary>
		/// <returns> an entity ID
		/// @pre $none
		/// @post $none </returns>
		int Last {get;set;}


		/// <summary>
		/// Gets this packet tag
		/// </summary>
		/// <returns> this packet tag
		/// @pre $none
		/// @post $none </returns>
		int Tag { get; set; }

	}

}