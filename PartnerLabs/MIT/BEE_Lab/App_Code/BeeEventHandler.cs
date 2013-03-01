using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
//using System.Web.Mvc;

using PusherRESTDotNet;

using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.LabServer.Interactive;
using iLabs.Proxies.ESS;
using iLabs.Proxies.ISB;
using iLabs.UtilLib;

namespace iLabs.LabServer.BEE
{
    /// <summary>
    /// Provide FileWatcher EventHandlers for the BEE Lab. Each instance 
    /// of the class should support one experiment and use a FileDataSource
    /// to interface with the file.
    /// </summary>
    public class BeeEventHandler
    {
        protected Coupon opCoupon;
        protected long experimentID;
        protected string submitter;
        protected string socketUrl;
        protected string essUrl;
        protected string recordType;

        protected string pusherEvent;
        protected string pusherChannel;

        protected string pusherID = "27877";
        protected string pusherKey = "b46f3ea8632aaeb34706";
        protected string pusherSS = "8eb0e8fb6087f48b1163";
        protected char[] delim = ",".ToCharArray();
        protected int count = 0;
        protected int status =0;
        protected int waitTime = 1000;
        //
        // TODO: Add constructor logic here
        //

        public BeeEventHandler()
        {
        }

        public BeeEventHandler(Coupon opCoupon, long experimentId,
            string essUrl, string recordType, string submitter)
        {
            this.opCoupon = opCoupon;
            this.experimentID = experimentId;
            this.essUrl = essUrl;
            this.recordType = recordType;
            this.submitter = submitter;
        }

        public string PusherEvent
        {
            get
            {
                return pusherEvent;
            }
            set
            {
                pusherEvent = value;
            }
        }

        public string PusherChannel
        {
            get
            {
                return pusherChannel;
            }
            set
            {
                pusherChannel = value;
            }
        }
        /*
         ++numTries;
            try
            {
                // Attempt to open the file exclusively.
                using (FileStream fs = new FileStream(fullPath,
                    FileMode.Open, FileAccess.ReadWrite, 
                    FileShare.None, 100))
                {
                    fs.ReadByte();

                    // If we got this far the file is ready
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.LogWarning(
                   "WaitForFile {0} failed to get an exclusive lock: {1}", 
                    fullPath, ex.ToString());

                if (numTries > 10)
                {
                    Log.LogWarning(
                        "WaitForFile {0} giving up after 10 tries", 
                        fullPath);
                    return false;
                }

                // Wait for the lock to be released
                System.Threading.Thread.Sleep(500);
            }
        */
        // Define the event handlers.
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                bool ok = false;
                // Specify what is done when a file is changed, created, or deleted.
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        Logger.WriteLine("BeeEventHabdler File Created: " + e.FullPath + " " + e.ChangeType);
                        break;
                    case WatcherChangeTypes.Changed:
                        try{
                            status = 0;
                            status = ProcessRecords(  e.FullPath);
                        }
                        catch(IOException ioe){
                            if(IsFileLocked(ioe)){
                                Logger.WriteLine("BeeEventHandle waiting for unlock");
                                status = 0;
                                int loopCOunt = 0;
                                while (status == 0 && loopCOunt < 10)
                                {
                                    // Wait for the lock to be released
                                    System.Threading.Thread.Sleep(waitTime);
                                    loopCOunt++;
                                    try
                                    {
                                        status = ProcessRecords(e.FullPath);
                                    }
                                    catch (IOException ioe2)
                                    {
                                        if (!IsFileLocked(ioe2))
                                        {
                                            throw ioe2;
                                        }
                                    }
                                    if (loopCOunt >= 10)
                                    {
                                        Logger.WriteLine("BeeEventHandle waiting for unlock timed out");
                                    }
                                }
                            }
                        }
                        catch(Exception ex){
                             throw ex;
                        }
                        break;
 
                    case WatcherChangeTypes.Deleted:
                        Logger.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("BeeEventHandler: " + ex.Message);
            }
        }

        private int ProcessRecords(string fullPath)
        {
            List<string> records = new List<string>();
            int count = 0;
            try
            {
                try
                {
                    // Attempt to open the file exclusively.
                   
                    using (FileStream fs = File.Open(fullPath, FileMode.Open, FileAccess.ReadWrite,
                             FileShare.None))
                    {
                        string line;
                        StreamReader sr = new StreamReader(fs);
                        while ((line = sr.ReadLine()) != null)
                        {
                            records.Add(line);
                        }
                        
                        fs.SetLength(0L);
                        sr.Close();
                    }
                }
                catch (IOException ioe)
                {
                    throw ioe;
                }

                // If we got this far send the data
                if (records.Count > 0)
                {
                    ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
                    essProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                    essProxy.OperationAuthHeaderValue.coupon = opCoupon;
                    essProxy.Url = essUrl;

                    //replace DataSocket code with 'Pusher' interface
                    IPusherProvider provider = new PusherProvider(pusherID, pusherKey, pusherSS, null);


                    foreach (string rec in records)
                    {
                        string[] vals = rec.Split(delim, 2);
                        DateTime timeStamp = new DateTime(0L, DateTimeKind.Local);
                        bool status = DateTime.TryParseExact(vals[0].Replace("\"", ""), "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out timeStamp);
                        string record = "\"" + timeStamp.ToString("o") + "\"," + vals[1];
                        try
                        {
                            essProxy.AddRecord(experimentID, submitter, recordType, false, record, null);
                        }
                        catch (Exception essEx)
                        {
                            Logger.WriteLine("BeeEventHandler: OnChange ESS: " + essEx.Message);
                        }
                        try
                        {
                            string str = @"{'rawData': [" + record + "]}";
                            ObjectPusherRequest request =
                                new ObjectPusherRequest(pusherChannel, "meassurement-added", str);
                            provider.Trigger(request);
                        }
                        catch (Exception dsEx)
                        {
                            Logger.WriteLine("BeeEventHandler: OnChange DS: " + dsEx.Message);
                        }
                        count++;
                    }
                    //Pusher interface Close not needed
                }
            }
            catch (Exception e)
            {
            }
            return count;
        }
 

    private bool IsFileLocked(IOException exception)
    {
        int errorCode = Marshal.GetHRForException(exception) & ((1 << 16) - 1);
        return errorCode == 32 || errorCode == 33;
    }

    /*                  
           }
       }
       catch (Exception ex)
       {
           Log.LogWarning(
              "WaitForFile {0} failed to get an exclusive lock: {1}", 
               fullPath, ex.ToString());

           if (numTries > 10)
           {
               Log.LogWarning(
                   "WaitForFile {0} giving up after 10 tries", 
                   fullPath);
               return false;
           }

           // Wait for the lock to be released
           System.Threading.Thread.Sleep(500);
       }
                   // DateTime lastWrite = File.GetLastWriteTimeUtc(e.FullPath);
                   FileInfo fInfo = new FileInfo(e.FullPath);
                   fInfo.
                   long len = fInfo.Length;
                 */  
     
    }
    
}
