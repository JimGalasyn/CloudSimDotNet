namespace org.cloudbus.cloudsim.container.utils
{
    using System.IO;
    //using CSVWriter = com.opencsv.CSVWriter;


    /// <summary>
    /// Created by sareh on 30/07/15.
    /// </summary>
    public class CostumeCSVWriter
	{
        // TODO: Find a CVS reader/writer
        //internal CSVWriter writer;
        internal string fileAddress;
		//internal Writer fileWriter;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CostumeCSVWriter(String fileAddress) throws java.io.IOException
		public CostumeCSVWriter(string fileAddress)
		{
			//File f = new File(fileAddress);
			//File parent3 = f.ParentFile;
			//if (!parent3.exists() && !parent3.mkdirs())
			//{
			//	throw new System.InvalidOperationException("Couldn't create dir: " + parent3);
			//}
			//if (!f.exists())
			//{
			//	f.createNewFile();
			//}
			//FileAddress = fileAddress;


		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void writeTofile(String[] entries) throws java.io.IOException
		public virtual void writeTofile(string[] entries)
		{
			// feed in your array (or convert your data to an array)
			try
			{
				//writer = new CSVWriter(new System.IO.StreamWriter(fileAddress, true), ',',CSVWriter.NO_QUOTE_CHARACTER);

			}
			catch (IOException)
			{
				Log.printConcatLine("Couldn't find the file to write to: ", fileAddress);


			}
			//writer.writeNext(entries);
			//writer.flush();
			//writer.close();
		}

		public virtual string FileAddress
		{
			get
			{
				return fileAddress;
			}
			set
			{
				this.fileAddress = value;
			}
		}
	}
}