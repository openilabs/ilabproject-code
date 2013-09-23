using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Threading;
using System.Web;

using iLabs.UtilLib;

namespace TestPinger
{
    class TestPinger
    {
        string pingUrl = "http://beehpz.mit.edu/BEElab/pingMe.aspx";
        string logName = "Application";
        string source ="Pinger";
        string eventOK = "PingEvent OK";

        static void Main(string[] args)
        {
            TestPinger pinger = null;
            if (args.Length == 0)
                pinger = new TestPinger();
            else
                pinger = new TestPinger(args[0]);
            pinger.Ping();

        }

        public TestPinger()
        {
           
        }

        public TestPinger(string url)
        {
            pingUrl = url;
        }

        public void Ping(){
            if (!EventLog.SourceExists(source))
	            EventLog.CreateEventSource(source,logName);
             try
            {
                WebClient http = new WebClient();
                string result = http.DownloadString( pingUrl);
                //ToDo: remove after debugging
                EventLog.WriteEntry(source,eventOK + " " + pingUrl);
            }
            catch (Exception ex)
            {
               EventLog.WriteEntry(source," PingServer: " + ex.Message,EventLogEntryType.Warning, 500);
            }
        }
    }
}
