using System;

/*
 * ** Network and Service Differentiation Extensions to CloudSim 3.0 **
 *
 * Gokul Poduval & Chen-Khong Tham
 * Computer Communication Networks (CCN) Lab
 * Dept of Electrical & Computer Engineering
 * National University of Singapore
 * October 2004
 *
 * Licence: GPL - http://www.gnu.org/copyleft/gpl.html
 * Copyright (c) 2004, The University of Melbourne, Australia and
 * National University of Singapore
 * ParameterException.java - Thrown for illegal parameters
 *
 */

namespace org.cloudbus.cloudsim
{

	/// <summary>
	/// This exception is to report bad or invalid parameters given during constructor.
	/// 
	/// @author Gokul Poduval
	/// @author Chen-Khong Tham, National University of Singapore
	/// @since CloudSim Toolkit 1.0
	/// @todo It would be used the native class InvalidArgumentException instead of this new one.
	/// </summary>
	public class ParameterException : Exception
	{

		/// <summary>
		/// The Constant serialVersionUID. </summary>
		private const long serialVersionUID = 1L;

		/// <summary>
		/// The message. </summary>
		private readonly string message;

		/// <summary>
		/// Constructs a new exception with <tt>null</tt> as its detail message.
		/// 
		/// @pre $none
		/// @post $none
		/// </summary>
		public ParameterException() : base()
		{
			message = null;
		}

		/// <summary>
		/// Creates a new ParameterException object.
		/// </summary>
		/// <param name="message"> an error message
		/// @pre $none
		/// @post $none </param>
		public ParameterException(string message) : base()
		{
			this.message = message;
		}

		public override string ToString()
		{
			return message;
		}

	}

}