using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Web;

using iLabs.Proxies.PAgent;

namespace iLabStatusService
{
    public partial class iLabStatusService : ServiceBase
    {
        private bool go = false;
        //private int waitMilliseconds = 480000; //every eight minutes
        private int waitMilliseconds = 30000; //every 30 Seconds
        private string connectionStr = "http://ludi.mit.edu/BEElab/pingMe.aspx";

        public iLabStatusService()
        {
            InitializeComponent();
        }

        public iLabStatusService(string connection)
        {
            InitializeComponent();
            connectionStr = connection;
        }

        public iLabStatusService(string connection, string waitSeconds)
        {
            InitializeComponent();
            connectionStr = connection;
            waitMilliseconds = 1000 * Int32.Parse(waitSeconds);
        }

        protected override void OnStart(string[] args)
        {
            if (args.Length == 2)
            {
                connectionStr = args[1];
            }
            if(args.Length > 2){
                 connectionStr = args[1];
                waitMilliseconds = 1000 * Int32.Parse(args[2]);
            }
            go = true;
            while(go){
                Thread.Sleep(waitMilliseconds);
                CheckServices();
            }
            
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        private void CheckServices(){
             try
            {
                WebClient http = new WebClient();
                string result = http.DownloadString( "http://ludi.mit.edu/BEElab/pingMe.aspx");
                //ToDo: remove after debugging
                Console.WriteLine(DateTime.Now + " PingServer: OK " + result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(DateTime.Now + " PingServer: " + ex.Message);
            }
        }
    }
}
