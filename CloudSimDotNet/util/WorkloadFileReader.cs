using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

/*
 * Title:        CloudSim Toolkit
 * Description:  CloudSim (Cloud Simulation) Toolkit for Modeling and Simulation of Clouds
 * Licence:      GPL - http://www.gnu.org/copyleft/gpl.html
 *
 * Copyright (c) 2009-2012, The University of Melbourne, Australia
 */

namespace org.cloudbus.cloudsim.util
{



    /// <summary>
    /// This class is responsible for reading resource traces from a file and creating a list of jobs
    /// (<seealso cref="Cloudlet Cloudlets"/>).
    /// <p/>
    /// <b>NOTE:</b>
    /// <ul>
    /// <li>This class can only take <tt>one</tt> trace file of the following format: <i>ASCII text, zip,
    /// gz.</i>
    /// <li>If you need to load multiple trace files, then you need to create multiple instances of this
    /// class <tt>each with a unique entity name</tt>.
    /// <li>If size of the trace file is huge or contains lots of traces, please increase the JVM heap
    /// size accordingly by using <tt>java -Xmx</tt> option when running the simulation.
    /// <li>The default job file size for sending to and receiving from a resource is
    /// <seealso cref="gridsim.net.Link#DEFAULT_MTU"/>. However, you can specify the file size by using
    /// <seealso cref="#setCloudletFileSize(int)"/>.
    /// <li>A job run time is only for 1 PE <tt>not</tt> the total number of allocated PEs. Therefore, a
    /// Cloudlet length is also calculated for 1 PE.<br>
    /// For example, job #1 in the trace has a run time of 100 seconds for 2 processors. This means each
    /// processor runs job #1 for 100 seconds, if the processors have the same specification.
    /// </ul>
    /// 
    /// @todo The last item in the list above is not true. The cloudlet length is not
    /// divided by the number of PEs. If there is more than 1 PE, all PEs run the same
    /// number of MI as specified in the <seealso cref="Cloudlet.CloudletLength"/> attribute.
    /// See <seealso cref="Cloudlet.NumberOfPes"/> method documentation.
    /// 
    /// <p/>
    /// By default, this class follows the standard workload format as specified in
    /// <a href="http://www.cs.huji.ac.il/labs/parallel/workload/">
    /// http://www.cs.huji.ac.il/labs/parallel/workload/</a> <br/>
    /// However, you can use other format by calling the methods below before running the simulation:
    /// <ul>
    ///   <li> <seealso cref="setComment(string)"/>
    ///   <li> <seealso cref="setField(int, int, int, int, int)"/>
    /// </ul>
    /// 
    /// @author Anthony Sulistio
    /// @author Marcos Dias de Assuncao
    /// @since 5.0
    /// </summary>
    /// <seealso cref= Workload </seealso>
    public class WorkloadFileReader : WorkloadModel
    {
        /// <summary>
        /// Trace file name.
        /// </summary>
        // TEST: (fixed) Never assigned
        private readonly File file= null;

        /// <summary>
        /// The Cloudlet's PE rating (in MIPS), considering that all PEs of a Cloudlet
        /// have the same rate.
        /// </summary>
        private readonly int rating;

        /// <summary>
        /// List of Cloudlets created from the trace <seealso cref="file"/>.
        /// </summary>
        private List<Cloudlet> jobs = null;


        /* Index of fields from the Standard Workload Format. */

        /// <summary>
        /// Field index of job number.
        /// </summary>
        private int JOB_NUM = 1 - 1;

        /// <summary>
        /// Field index of submit time of a job.
        /// </summary>
        private int SUBMIT_TIME = 2 - 1;

        /// <summary>
        /// Field index of running time of a job.
        /// </summary>
        private readonly int RUN_TIME = 4 - 1;

        /// <summary>
        /// Field index of number of processors needed for a job.
        /// </summary>
        private readonly int NUM_PROC = 5 - 1;

        /// <summary>
        /// Field index of required number of processors.
        /// </summary>
        private int REQ_NUM_PROC = 8 - 1;

        /// <summary>
        /// Field index of required running time.
        /// </summary>
        private int REQ_RUN_TIME = 9 - 1;

        /// <summary>
        /// Field index of user who submitted the job.
        /// </summary>
        private readonly int USER_ID = 12 - 1;

        /// <summary>
        /// Field index of group of the user who submitted the job.
        /// </summary>
        private readonly int GROUP_ID = 13 - 1;

        /// <summary>
        /// Max number of fields in the trace file.
        /// </summary>
        private int MAX_FIELD = 18;

        /// <summary>
        /// A string that denotes the start of a comment.
        /// </summary>
        private string COMMENT = ";";

        /// <summary>
        /// If the field index of the job number (<seealso cref="JOB_NUM"/>) is equals
        /// to this constant, it means the number of the job doesn't have to be
        /// gotten from the trace file, but has to be generated by this workload generator
        /// class.
        /// </summary>
        private const int IRRELEVANT = -1;

        /// <summary>
        /// A temp array storing all the fields read from a line of the trace file.
        /// </summary>
        private string[] fieldArray = null;

        /// <summary>
        /// Create a new WorkloadFileReader object.
        /// </summary>
        /// <param name="fileName"> the workload trace filename in one of the following formats: 
        ///                 <i>ASCII text, zip, gz.</i> </param>
        /// <param name="rating"> the cloudlet's PE rating (in MIPS), considering that all PEs 
        /// of a cloudlet have the same rate </param>
        /// <exception cref="FileNotFoundException"> </exception>
        /// <exception cref="IllegalArgumentException"> This happens for the following conditions:
        ///         <ul>
        ///           <li>the workload trace file name is null or empty
        ///           <li>the resource PE rating <= 0
        ///         </ul>
        /// @pre fileName != null
        /// @pre rating > 0
        /// @post $none </exception>
        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public WorkloadFileReader(final String fileName, final int rating) throws java.io.FileNotFoundException
        //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
        public WorkloadFileReader(string fileName, int rating)
        {
            if (string.ReferenceEquals(fileName, null) || fileName.Length == 0)
            {
                throw new System.ArgumentException("Invalid trace file name.");
            }
            else if (rating <= 0)
            {
                throw new System.ArgumentException("Resource PE rating must be > 0.");
            }

            // TODO: I think this means physical file system File, i.e., like in System.IO.
            //file = new File(fileName);
            //if (!file.exists())
            //{
            //    throw new FileNotFoundException("Workload trace " + fileName + " does not exist");
            //}

            this.rating = rating;
        }

        /// <summary>
        /// Reads job information from a trace file and generates the respective cloudlets.
        /// </summary>
        /// <returns> the list of cloudlets read from the file; <code>null</code> in case of failure. </returns>
        /// <seealso cref= #file </seealso>
        public virtual List<Cloudlet> generateWorkload()
        {
            if (jobs == null)
            {
                jobs = new List<Cloudlet>();

                // create a temp array
                fieldArray = new string[MAX_FIELD];

                try
                {
                    /*@todo It would be implemented
                    using specific classes to avoid using ifs.
                    If a new format is included, the code has to be
                    changed to include another if*/
                    if (file.Name.EndsWith(".gz"))
                    {
                        readGZIPFile(file);
                    }
                    else if (file.Name.EndsWith(".zip"))
                    {
                        readZipFile(file);
                    }
                    else
                    {
                        readFile(file);
                    }
                }
                //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
                //ORIGINAL LINE: catch (final java.io.FileNotFoundException e)
                catch (FileNotFoundException e)
                {
                    Debug.WriteLine(e.ToString());
                    throw e;
                }
                //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
                //ORIGINAL LINE: catch (final java.io.IOException e)
                catch (IOException e)
                {
                    Debug.WriteLine(e.ToString());
                    throw e;
                }
            }
            return jobs;
        }

    /// <summary>
    /// Sets the string that identifies the start of a comment line.
    /// </summary>
    /// <param name="cmt"> a character that denotes the start of a comment, e.g. ";" or "#" </param>
    /// <returns> <code>true</code> if it is successful, <code>false</code> otherwise
    /// @pre comment != null
    /// @post $none </returns>
    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    //ORIGINAL LINE: public boolean setComment(final String cmt)
    public virtual bool setComment(string cmt)
    {
        bool success = false;
        if (!string.ReferenceEquals(cmt, null) && cmt.Length > 0)
        {
            COMMENT = cmt;
            success = true;
        }
        return success;
    }

    /// <summary>
    /// Tells this class what to look in the trace file. This method should be called before the
    /// start of the simulation.
    /// <p/>
    /// By default, this class follows the standard workload format as specified in <a
    /// href="http://www.cs.huji.ac.il/labs/parallel/workload/">
    /// http://www.cs.huji.ac.il/labs/parallel/workload/</a> <br>
    /// However, you can use other format by calling this method.
    /// <p/>
    /// The parameters must be a positive integer number starting from 1. A special case is where
    /// <tt>jobNum == <seealso cref="#IRRELEVANT"/></tt>, meaning the job or cloudlet ID will be generate
    /// by the Workload class, instead of reading from the trace file.
    /// </summary>
    /// <param name="maxField"> max. number of field/column in one row </param>
    /// <param name="jobNum"> field/column number for locating the job ID </param>
    /// <param name="submitTime"> field/column number for locating the job submit time </param>
    /// <param name="runTime"> field/column number for locating the job run time </param>
    /// <param name="numProc"> field/column number for locating the number of PEs required to run a job </param>
    /// <returns> <code>true</code> if successful, <code>false</code> otherwise </returns>
    /// <exception cref="IllegalArgumentException"> if any of the arguments are not within the acceptable ranges
    /// @pre maxField > 0
    /// @pre submitTime > 0
    /// @pre runTime > 0
    /// @pre numProc > 0
    /// @post $none </exception>
    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    //ORIGINAL LINE: public boolean setField(final int maxField, final int jobNum, final int submitTime, final int runTime, final int numProc)
    public virtual bool setField(int maxField, int jobNum, int submitTime, int runTime, int numProc)
    {
        // need to subtract by 1 since array starts at 0.
        if (jobNum > 0)
        {
            JOB_NUM = jobNum - 1;
        }
        else if (jobNum == 0)
        {
            throw new System.ArgumentException("Invalid job number field.");
        }
        else
        {
            JOB_NUM = -1;
        }

        // get the max. number of field
        if (maxField > 0)
        {
            MAX_FIELD = maxField;
        }
        else
        {
            throw new System.ArgumentException("Invalid max. number of field.");
        }

        // get the submit time field
        if (submitTime > 0)
        {
            SUBMIT_TIME = submitTime - 1;
        }
        else
        {
            throw new System.ArgumentException("Invalid submit time field.");
        }

        // get the run time field
        if (runTime > 0)
        {
            REQ_RUN_TIME = runTime - 1;
        }
        else
        {
            throw new System.ArgumentException("Invalid run time field.");
        }

        // get the number of processors field
        if (numProc > 0)
        {
            REQ_NUM_PROC = numProc - 1;
        }
        else
        {
            throw new System.ArgumentException("Invalid number of processors field.");
        }

        return true;
    }

    // ------------------- PRIVATE METHODS -------------------

    /// <summary>
    /// Creates a Cloudlet with the given information and adds to the list of <seealso cref="#jobs"/>.
    /// </summary>
    /// <param name="id"> a Cloudlet ID </param>
    /// <param name="submitTime"> Cloudlet's submit time </param>
    /// <param name="runTime"> The number of seconds the Cloudlet has to run. Considering that 
    /// and the <seealso cref="#rating"/>, the <seealso cref="Cloudlet#cloudletLength"/> is computed. </param>
    /// <param name="numProc"> number of Cloudlet's PEs </param>
    /// <param name="reqRunTime"> user estimated run time 
    /// (@todo the parameter is not being used and it is not clear what it is) </param>
    /// <param name="userID"> user id </param>
    /// <param name="groupID"> user's group id
    /// @pre id >= 0
    /// @pre submitTime >= 0
    /// @pre runTime >= 0
    /// @pre numProc > 0
    /// @post $none </param>
    /// <seealso cref= #rating </seealso>
    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    //ORIGINAL LINE: private void createJob(final int id, final long submitTime, final int runTime, final int numProc, final int reqRunTime, final int userID, final int groupID)
    private void createJob(int id, long submitTime, int runTime, int numProc, int reqRunTime, int userID, int groupID)
    {
        // create the cloudlet
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final int len = runTime * rating;
        int len = runTime * rating;
        UtilizationModel utilizationModel = new UtilizationModelFull();
        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final org.cloudbus.cloudsim.Cloudlet wgl = new org.cloudbus.cloudsim.Cloudlet(id, len, numProc, 0, 0, utilizationModel, utilizationModel, utilizationModel);
        Cloudlet wgl = new Cloudlet(id, len, numProc, 0, 0, utilizationModel, utilizationModel, utilizationModel);
        jobs.Add(wgl);
    }

    /// <summary>
    /// Extracts relevant information from a given array of fields,
    /// representing a line from the trace file, and create a cloudlet 
    /// using this information.
    /// </summary>
    /// <param name="array"> the array of fields generated from a line of the trace file. </param>
    /// <param name="line"> the line number
    /// @pre array != null
    /// @pre line > 0
    /// @todo The name of the method doesn't describe what it in fact does. </param>
    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    //ORIGINAL LINE: private void extractField(final String[] array, final int line)
    private void extractField(string[] array, int line)
    {
        try
        {
            int? obj = null;

            // get the job number
            int id = 0;
            if (JOB_NUM == IRRELEVANT)
            {
                id = jobs.Count + 1;
            }
            else
            {
                obj = Convert.ToInt32(array[JOB_NUM].Trim());
                id = obj.Value;
            }

            // get the submit time
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final Nullable<long> l = new Long(array[SUBMIT_TIME].trim());
            long? l = Convert.ToInt64(array[SUBMIT_TIME].Trim());
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final long submitTime = l.intValue();
            long submitTime = l.Value;

            // get the user estimated run time
            obj = Convert.ToInt32(array[REQ_RUN_TIME].Trim());
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int reqRunTime = obj.intValue();
            int reqRunTime = obj.Value;

            // if the required run time field is ignored, then use
            // the actual run time
            obj = Convert.ToInt32(array[RUN_TIME].Trim());
            int runTime = obj.Value;

            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int userID = new Integer(array[USER_ID].trim()).intValue();
            int userID = (Convert.ToInt32(array[USER_ID].Trim()));
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ORIGINAL LINE: final int groupID = new Integer(array[GROUP_ID].trim()).intValue();
            int groupID = (Convert.ToInt32(array[GROUP_ID].Trim()));

            // according to the SWF manual, runtime of 0 is possible due
            // to rounding down. E.g. runtime is 0.4 seconds -> runtime = 0
            if (runTime <= 0)
            {
                runTime = 1; // change to 1 second
            }

            // get the number of allocated processors
            obj = Convert.ToInt32(array[REQ_NUM_PROC].Trim());
            int numProc = obj.Value;

            // if the required num of allocated processors field is ignored
            // or zero, then use the actual field
            if (numProc == IRRELEVANT || numProc == 0)
            {
                obj = Convert.ToInt32(array[NUM_PROC].Trim());
                numProc = obj.Value;
            }

            // finally, check if the num of PEs required is valid or not
            if (numProc <= 0)
            {
                numProc = 1;
            }
            createJob(id, submitTime, runTime, numProc, reqRunTime, userID, groupID);
        }
        //JAVA TO C# CONVERTER WARNING: 'final' catch parameters are not available in C#:
        //ORIGINAL LINE: catch (final Exception e)
        catch (Exception e)
        {
            Debug.WriteLine(e.ToString());
        }
    }

    /// <summary>
    /// Breaks a line from the trace file into many fields into the
    /// <seealso cref="#fieldArray"/>.
    /// </summary>
    /// <param name="line"> a line from the trace file </param>
    /// <param name="lineNum"> the line number
    /// @pre line != null
    /// @pre lineNum > 0
    /// @post $none </param>
    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    //ORIGINAL LINE: private void parseValue(final String line, final int lineNum)
    private void parseValue(string line, int lineNum)
    {
        // skip a comment line
        if (line.StartsWith(COMMENT, StringComparison.Ordinal))
        {
            return;
        }

        //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
        //ORIGINAL LINE: final String[] sp = line.split("\\s+");
        //string[] sp = line.Split("\\s+", true); // split the fields based on a space
        // TODO: Correct delimiter?
        string[] sp = line.Split(' ');
        int len = 0; // length of a string
        int index = 0; // the index of an array

        // check for each field in the array
        foreach (String elem in sp)
        {
            len = elem.Length; // get the length of a string

            // if it is empty then ignore
            if (len == 0)
            {
                continue;
            }
            fieldArray[index] = elem;
            index++;
        }

        if (index == MAX_FIELD)
        {
            extractField(fieldArray, lineNum);
        }
    }

    /// <summary>
    /// Reads traces from a text file, one line at a time.
    /// </summary>
    /// <param name="fl"> a file name </param>
    /// <returns> <code>true</code> if successful, <code>false</code> otherwise. </returns>
    /// <exception cref="IOException"> if the there was any error reading the file </exception>
    /// <exception cref="FileNotFoundException"> if the file was not found </exception>
    //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
    //ORIGINAL LINE: private boolean readFile(final java.io.File fl) throws java.io.IOException, java.io.FileNotFoundException
    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    private bool readFile(File fl)
    {
        bool success = false;
        System.IO.StreamReader reader = null;
        try
        {
                // TODO: Fix this IO stream stuff.
                reader = null; //new System.IO.StreamReader(new System.IO.FileStream(fl, System.IO.FileMode.Open, System.IO.FileAccess.Read));

            // read one line at the time
            int line = 1;
            string readLine = null;
                // TODO: Find ready() equivalent
                //while (reader.ready() && !string.ReferenceEquals((readLine = reader.ReadLine()), null))
                while (!string.ReferenceEquals((readLine = reader.ReadLine()), null))
                {
                parseValue(readLine, line);
                line++;
            }

                // TODO: using statement
            //reader.Close();
            success = true;
        }
        finally
        {
            if (reader != null)
            {
                //reader.Close();
            }
        }

        return success;
    }

    /// <summary>
    /// Reads traces from a gzip file, one line at a time.
    /// </summary>
    /// <param name="fl"> a gzip file name </param>
    /// <returns> <code>true</code> if successful; <code>false</code> otherwise. </returns>
    /// <exception cref="IOException"> if the there was any error reading the file </exception>
    /// <exception cref="FileNotFoundException"> if the file was not found </exception>
    //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
    //ORIGINAL LINE: private boolean readGZIPFile(final java.io.File fl) throws java.io.IOException, java.io.FileNotFoundException
    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    private bool readGZIPFile(File fl)
    {
        bool success = false;
        System.IO.StreamReader reader = null;
        try
        {
                // TODO: Fix this IO stream stuff.
                reader = null; // new System.IO.StreamReader(new GZIPInputStream(new System.IO.FileStream(fl, System.IO.FileMode.Open, System.IO.FileAccess.Read)));

            // read one line at the time
            int line = 1;
            string readLine = null;
                // TODO: Find ready() equivalent
                //while (reader.ready() && !string.ReferenceEquals((readLine = reader.ReadLine()), null))
                while (!string.ReferenceEquals((readLine = reader.ReadLine()), null))
                {
                parseValue(readLine, line);
                line++;
            }

            //reader.Close();
            success = true;
        }
        finally
        {
            if (reader != null)
            {
                //reader.Close();
            }
        }

        return success;
    }

    /// <summary>
    /// Reads traces from a Zip file, one line at a time.
    /// </summary>
    /// <param name="fl"> a zip file name </param>
    /// <returns> <code>true</code> if reading a file is successful; <code>false</code> otherwise. </returns>
    /// <exception cref="IOException"> if the there was any error reading the file </exception>
    //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
    //ORIGINAL LINE: private boolean readZipFile(final java.io.File fl) throws java.io.IOException
    //JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
    private bool readZipFile(File fl)
    {
        bool success = false;
        ZipFile zipFile = null;
        try
        {
            System.IO.StreamReader reader = null;

            // ZipFile offers an Enumeration of all the files in the file
            zipFile = new ZipFile(fl);
            //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
            //ready()ORIGINAL LINE: final java.util.Iterator<? extends java.util.zip.ZipEntry> e = zipFile.entries();
            //JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
            IEnumerator<ZipEntry> e = zipFile.entries();
            while (e.MoveNext())
            {
                success = false; // reset the value again
                                 //JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
                                 //ORIGINAL LINE: final java.util.zip.ZipEntry zipEntry = e.Current;
                ZipEntry zipEntry = e.Current;

                reader = new System.IO.StreamReader(zipFile.getInputStream(zipEntry));

                // read one line at the time
                int line = 1;
                string readLine = null;
                    // TODO: ready() equivalent
                    //while (reader.ready() && !string.ReferenceEquals((readLine = reader.ReadLine()), null))
                    while (!string.ReferenceEquals((readLine = reader.ReadLine()), null))
                    {
                    parseValue(readLine, line);
                    line++;
                }

                //reader.Close();
                success = true;
            }
        }
        finally
        {
            if (zipFile != null)
            {
                //zipFile.close();
            }
        }

        return success;
    }
}

    // TODO: Replace this with real code.
    class ZipEntry
    {

    }

    // TODO: Replace this with real code.
    class ZipFile
    {
        private File fl;

        public ZipFile(File fl)
        {
            this.fl = fl;
        }

        internal IEnumerator<ZipEntry> entries()
        {
            throw new NotImplementedException();
        }

        internal Stream getInputStream(ZipEntry zipEntry)
        {
            throw new NotImplementedException();
        }
    }
}