using System;
using System.Diagnostics;
using System.IO;
using System.Text;

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
	/// Logger used for performing logging of the simulation process. It provides the ability to
	/// substitute the output stream by any OutputStream subclass.
	/// 
	/// @author Anton Beloglazov
	/// @since CloudSim Toolkit 2.0
	/// @todo To add a method to print formatted text, such as the 
	/// <seealso cref="String#format(java.lang.String, java.lang.Object...)"/> method.
	/// </summary>
	public class Log
	{

        /// <summary>
        /// The Constant LINE_SEPARATOR. </summary>
        private static readonly string LINE_SEPARATOR = Environment.NewLine; // System.getProperty("line.separator");

		/// <summary>
		/// The stream where the log will the outputted. </summary>
		private static System.IO.Stream output;

		/// <summary>
		/// Indicates if the logger is disabled or not. If set to true,
		///        the call for any print method has no effect. 
		/// </summary>
		private static bool disabled;

		/// <summary>
		/// Buffer to avoid creating new string builder upon every print. </summary>
		private static StringBuilder buffer = new StringBuilder();

		/// <summary>
		/// Prints a message.
		/// </summary>
		/// <param name="message"> the message </param>
		public static void print(string message)
		{
			if (!Disabled)
			{
				try
				{
                    //Output.WriteByte(message.GetBytes());
                    Debug.WriteLine(message);
				}
				catch (IOException e)
				{
					Debug.WriteLine(e.ToString());
                    Debug.WriteLine(e.StackTrace);
				}
			}
		}

		/// <summary>
		/// Prints the message passed as a non-String object.
		/// </summary>
		/// <param name="message"> the message </param>
		public static void print(object message)
		{
			if (!Disabled)
			{
				print(message.ToString());
			}
		}

		/// <summary>
		/// Prints a message and a new line.
		/// </summary>
		/// <param name="message"> the message </param>
		public static void printLine(string message)
		{
			if (!Disabled)
			{
				print(message + LINE_SEPARATOR);
			}
		}

		/// <summary>
		/// Prints an empty line.
		/// </summary>
		public static void printLine()
		{
			if (!Disabled)
			{
				print(LINE_SEPARATOR);
			}
		}


		/// <summary>
		/// Prints the concatenated text representation of the arguments.
		/// </summary>
		/// <param name="messages"> the messages to print </param>
		public static void printConcat(params object[] messages)
		{
			if (!Disabled)
			{
				buffer.Length = 0; // Clear the buffer
				for (int i = 0 ; i < messages.Length ; i++)
				{
					buffer.Append(messages[i].ToString());
				}
				print(buffer);
			}
		}

		/// <summary>
		/// Prints the concatenated text representation of the arguments and a new line.
		/// </summary>
		/// <param name="messages"> the messages to print </param>
		public static void printConcatLine(params object[] messages)
		{
			if (!Disabled)
			{
				buffer.Length = 0; // Clear the buffer
				for (int i = 0 ; i < messages.Length ; i++)
				{
					buffer.Append(messages[i].ToString());
				}
				printLine(buffer);
			}
		}



		/// <summary>
		/// Prints the message passed as a non-String object and a new line.
		/// </summary>
		/// <param name="message"> the message </param>
		public static void printLine(object message)
		{
			if (!Disabled)
			{
			printLine(message.ToString());
			}
		}



		/// <summary>
		/// Prints a string formated as in String.format().
		/// </summary>
		/// <param name="format"> the format </param>
		/// <param name="args"> the args </param>
		public static void format(string format, params object[] args)
		{
			if (!Disabled)
			{
				print(string.Format(format, args));
			}
		}

		/// <summary>
		/// Prints a string formated as in String.format(), followed by a new line.
		/// </summary>
		/// <param name="format"> the format </param>
		/// <param name="args"> the args </param>
		public static void formatLine(string format, params object[] args)
		{
			if (!Disabled)
			{
				printLine(string.Format(format, args));
			}
		}

		/// <summary>
		/// Sets the output stream.
		/// </summary>
		/// <param name="_output"> the new output </param>
		public static System.IO.Stream Output
		{
			set
			{
				output = value;
			}
			get
			{
				if (output == null)
				{
                    // TODO: use the right output stream.
					//Output = System.out;
				}
				return output;
			}
		}


		/// <summary>
		/// Sets the disable output flag.
		/// </summary>
		/// <param name="_disabled"> the new disabled </param>
		public static bool Disabled
		{
			set
			{
				disabled = value;
			}
			get
			{
				return disabled;
			}
		}


		/// <summary>
		/// Disables the output.
		/// </summary>
		public static void disable()
		{
			Disabled = true;
		}

		/// <summary>
		/// Enables the output.
		/// </summary>
		public static void enable()
		{
			Disabled = false;
		}
	}
}