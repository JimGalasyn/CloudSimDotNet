/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace org.cloudbus.cloudsim
{

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	//using Before = org.junit.Before;
	//using Test = org.junit.Test;

	/// <summary>
	/// @author		Anton Beloglazov
	/// @since		CloudSim Toolkit 2.0
	/// </summary>
	public class LogTest
	{

		private static readonly System.IO.MemoryStream OUTPUT = new System.IO.MemoryStream();
        //private static readonly string LINE_SEPARATOR = System.getProperty("line.separator");
        private static readonly string LINE_SEPARATOR = System.Environment.NewLine;
        //	private static readonly DecimalFormatSymbols dfs = DecimalFormatSymbols.getInstance(Locale.getDefault(Locale.Category.FORMAT));

        //JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
        //ORIGINAL LINE: @Before public void setUp() throws Exception
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        public virtual void setUp()
		{
			Log.Output = OUTPUT;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPrint() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testPrint()
		{
			Log.print("test test");
            //assertEquals("test test", OUTPUT.ToString());
            Assert.AreEqual("test test", OUTPUT.ToString());
            //OUTPUT.reset();

            Log.print(123);
            //assertEquals("123", OUTPUT.ToString());
            Assert.AreEqual("123", OUTPUT.ToString());
            //OUTPUT.reset();

            Log.print(123L);
            //assertEquals("123", OUTPUT.ToString());
            Assert.AreEqual("123", OUTPUT.ToString());
            //OUTPUT.reset();

            Log.print(123.0);
            //assertEquals("123.0", OUTPUT.ToString());
            Assert.AreEqual("123.0", OUTPUT.ToString());
            //OUTPUT.reset();
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPrintLine() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testPrintLine()
		{
			Log.printLine("test test");
            //assertEquals("test test" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("test test" + LINE_SEPARATOR, OUTPUT.ToString());
			//OUTPUT.reset();

			Log.printLine(123);
            //assertEquals("123" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("123" + LINE_SEPARATOR, OUTPUT.ToString());
            //OUTPUT.reset();

			Log.printLine(123L);
            //assertEquals("123" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("123" + LINE_SEPARATOR, OUTPUT.ToString());
            //OUTPUT.reset();

            Log.printLine(123.0);
            //assertEquals("123.0" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("123.0" + LINE_SEPARATOR, OUTPUT.ToString());
            //OUTPUT.reset();
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFormat() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testFormat()
		{
			Log.format("test %s test", "test");
            //assertEquals("test test test", OUTPUT.ToString());
            Assert.AreEqual("test test test", OUTPUT.ToString());
			//OUTPUT.reset();

			Log.format("%d", 123);
            //assertEquals("123", OUTPUT.ToString());
            Assert.AreEqual("123", OUTPUT.ToString());
			//OUTPUT.reset();

			Log.format("%d", 123L);
            //assertEquals("123", OUTPUT.ToString());
            Assert.AreEqual("123", OUTPUT.ToString());
			//OUTPUT.reset();

			//Log.format("%.2f", 123.01);
            //assertEquals("123" + dfs.DecimalSeparator + "01", OUTPUT.ToString());
            //Assert.AreEqual
            //OUTPUT.reset();
        }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFormatLine() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testFormatLine()
		{
		    //OUTPUT.reset();
			Log.formatLine("test %s test", "test");
			//assertEquals("test test test" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("test test test" + LINE_SEPARATOR, OUTPUT.ToString());
			//OUTPUT.reset();

			Log.formatLine("%d", 123);
            //assertEquals("123" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("123" + LINE_SEPARATOR, OUTPUT.ToString());
			//OUTPUT.reset();

			Log.formatLine("%d", 123L);
            //assertEquals("123" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("123" + LINE_SEPARATOR, OUTPUT.ToString());
			//OUTPUT.reset();

			//Log.formatLine("%.2f", 123.01);
			//assertEquals("123" + dfs.DecimalSeparator + "01" + LINE_SEPARATOR, OUTPUT.ToString());
			//OUTPUT.reset();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDisable() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testDisable()
		{
            //OUTPUT.reset();
            //assertFalse(Log.Disabled);
            Assert.IsFalse(Log.Disabled);

			Log.print("test test");
            //assertEquals("test test", OUTPUT.ToString());
            Assert.AreEqual("test test", OUTPUT.ToString());
			//OUTPUT.reset();

			Log.printLine("test test");
            //assertEquals("test test" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("test test" + LINE_SEPARATOR, OUTPUT.ToString());
			//OUTPUT.reset();

			Log.format("test %s test", "test");
            //assertEquals("test test test", OUTPUT.ToString());
            Assert.AreEqual("test test test", OUTPUT.ToString());
			//OUTPUT.reset();

			Log.formatLine("test %s test", "test");
            //assertEquals("test test test" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("test test test" + LINE_SEPARATOR, OUTPUT.ToString());
			//OUTPUT.reset();

			Log.disable();

            //assertTrue(Log.Disabled);
            Assert.IsTrue(Log.Disabled);

			Log.print("test test");
			//assertEquals("", OUTPUT.ToString());
            Assert.AreEqual("", OUTPUT.ToString());
			//OUTPUT.reset();

			Log.printLine("test test");
            //assertEquals("", OUTPUT.ToString());
            Assert.AreEqual("", OUTPUT.ToString());
			//OUTPUT.reset();

			Log.format("test %s test", "test");
            //assertEquals("", OUTPUT.ToString());
            Assert.AreEqual("", OUTPUT.ToString());
			//OUTPUT.reset();

			Log.formatLine("test %s test", "test");
            //assertEquals("", OUTPUT.ToString());
            Assert.AreEqual("", OUTPUT.ToString());
            //OUTPUT.reset();

            Log.enable();

            //assertFalse(Log.Disabled);
            Assert.IsFalse(Log.Disabled);

			Log.print("test test");
            //assertEquals("test test", OUTPUT.ToString());
            Assert.AreEqual("test test", OUTPUT.ToString());
			//OUTPUT.reset();

			Log.printLine("test test");
            //assertEquals("test test" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("test test" + LINE_SEPARATOR, OUTPUT.ToString());
			//OUTPUT.reset();

			Log.format("test %s test", "test");
            //assertEquals("test test test", OUTPUT.ToString());
            Assert.AreEqual("test test test", OUTPUT.ToString());
			//OUTPUT.reset();

			Log.formatLine("test %s test", "test");
            //assertEquals("test test test" + LINE_SEPARATOR, OUTPUT.ToString());
            Assert.AreEqual("test test test" + LINE_SEPARATOR, OUTPUT.ToString());
			//OUTPUT.reset();
		}
	}
}