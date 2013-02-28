using System;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.IO;
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

                        // DateTime lastWrite = File.GetLastWriteTimeUtc(e.FullPath);
                        FileInfo fInfo = new FileInfo(e.FullPath);
                        
                        long len = fInfo.Length;
                        if (len > 0)
                        {
                            ExperimentStorageProxy essProxy = new ExperimentStorageProxy();
                            essProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                            essProxy.OperationAuthHeaderValue.coupon = opCoupon;
                            essProxy.Url = essUrl;

                            //replace DataSocket code with 'Pusher' interface
                            IPusherProvider provider = new PusherProvider(pusherID, pusherKey, pusherSS, null);

                            string[] records = File.ReadAllLines(e.FullPath);
                            using (FileStream inFile = fInfo.Open(FileMode.Truncate)) { }

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
                            }
                            //Pusher interface Close not needed
                        }
                        if (count > 10)
                        {
                            Global.PingServer();
                            count = 0;
                        }
                        else
                        {
                            count++;
                        }
                        break;
                    case WatcherChangeTypes.Deleted:
                        Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("BeeEventHandler: " + ex.Message);
            }

        }

     
    }
    
}
