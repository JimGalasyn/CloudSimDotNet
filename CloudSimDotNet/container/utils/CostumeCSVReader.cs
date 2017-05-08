using System;
using System.Collections.Generic;

namespace org.cloudbus.cloudsim.container.utils
{
    using System.Diagnostics;
    using System.IO;
    //using CSVReader = com.opencsv.CSVReader;

    /// <summary>
    /// Run.
    /// 
    /// 
    /// </summary>
    public class CostumeCSVReader
	{
		private static IList<string[]> fileData;

        //TODO: public CostumeCSVReader(File inputFile)
        public CostumeCSVReader()
        {
			// TODO: Find a CVS reader/writer.
			//CSVReader reader = null;
			try
			{
	//			Log.printLine(inputFile);
				//Get the CSVReader instance with specifying the delimiter to be used
				//reader = new CSVReader(new System.IO.StreamReader(inputFile));
				//fileData = reader.readAll();

			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
                Debug.WriteLine(e.StackTrace);
			}
			finally
			{
				try
				{
					//reader.close();
				}
				catch (IOException e)
				{
                    Debug.WriteLine(e.ToString());
                    Debug.WriteLine(e.StackTrace);
				}

			}
		}



		public static IList<string[]> FileData
		{
			get
			{
				return fileData;
			}
			set
			{
				CostumeCSVReader.fileData = value;
			}
		}
	}
}