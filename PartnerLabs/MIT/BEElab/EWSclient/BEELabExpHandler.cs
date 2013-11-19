using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using ClientSoapCom;
using com = CommonTypes.DataExchange;
using System.Net;
using System.ServiceModel.Channels;
using CommonTypes.DataExchange;

namespace SimpleClient
{
    public class BEELabExpHandler
    {
        readonly EWSClientCom _clientCom = new EWSClientCom();
        string connectRef;
        string prefix = "";

        public void startExperiment(string profile, String cycles)
        {

            Console.WriteLine("In startExperiment method 1: " + " profile is " + profile + " and cycle is " + cycles);
            BEELabExpHandler handler = new BEELabExpHandler();
            handler.connectToServer();

      
            string setPointString = handler.createSetPointStringBasedOnIndex(profile, 1, 5);
            Console.WriteLine("After createSetPointStringBasedOnIndex method is : " + setPointString);
            setPointString = handler.createSetPointStringBasedOnCycles(setPointString, int.Parse(cycles));
            Console.WriteLine("Final Set Point String is: " + setPointString);

            string id = "";
            if (profile.Equals("Phoenix"))
                id = ExperimentConstants.PhoenixId;
            if (profile.Equals("Atlanta"))
                id = ExperimentConstants.AtlantaId;

            Console.WriteLine("Set Point ID is: " + id);
            //set the set point valies of selected profile
            handler.SetValue(id, setPointString);
            // set start experiment to true
            handler.SetValue(ExperimentConstants.StartExperiemntId,"True");
            // set chamber occupancy to true
            handler.SetValue(ExperimentConstants.CCocc, "True");
            
        }

        /// <summary>
        /// Connect to StruxureWare Enterprise server 
        /// </summary>
        private void connectToServer()
        {
            try
            {
                // reference server
                string addr = ExperimentConstants.ip_address;
                Console.WriteLine("Connecting to server " + addr);
                
                // Set security to Digest
                com.EwsSecurity security = new com.EwsSecurity()
                {
                    AuthenticationScheme = AuthenticationSchemes.Digest,
                    Password = "admin",
                    Username = "admin"

                };

                // Set default values for connection
                com.EwsBindingConfig config = new com.EwsBindingConfig()
                {
                    MessageVersion = MessageVersion.Soap12,
                    AllowCookies = true,
                    AuthenticationScheme = AuthenticationSchemes.Digest,

                    MaxBufferPoolSize = 1000000,
                    MaxBufferSize = 1000000,
                    MaxReceivedMessageSize = 1000000,

                    OpenTimeout = 30,
                    ReceiveTimeout = 30,
                    SendTimeout = 30,
                    CloseTimeout = 30
                };

                Console.WriteLine("Connect to {0}:", addr);
                // connect to EWS server
                
                int status = _clientCom.ConnectEx(addr, security, config, out connectRef);

            }
            catch (Exception ex)
            {

                ReportException(ex);
            }
        }


        /// <summary>
        /// Reports the exception caught as well as the inner exception when set.
        /// </summary>
        /// <param name="exp">The selected profile.</param>
        /// <param name="exp">Number of cycles e.g. 1 cycle = 7 days houly data</param>
        private string createSetPointStringBasedOnCycles(string profile, int cycles)
        {
            //String profileString = "";

            //if (profile.Equals("Phoenix"))
            //    profileString = ExperimentConstants.PhoenixSetPoint;
            //if (profile.Equals("Atlanta"))
            //    profileString = ExperimentConstants.AtlantaSetPoint;

            String profileString = profile;
            String finalString = "";

            for (int index = 1; index <= cycles; index++)
            {
                if (index == 1)
                    finalString = profileString;
                else
                    finalString = finalString + "|" + profileString;

            }


            return finalString;

        }


        /// <summary>
        /// Set Value
        /// </summary>
        /// <param name="id">Id of the item to change</param>
        /// <param name="value">New value of the item</param>
        void SetValue(string id, string value)
        {
            string error = null;
            try
            {
                List<ValueTypeStateless> writeItemList = new List<ValueTypeStateless>();
                writeItemList.Add(new ValueTypeStateless()
                {
                    Id = id,
                    Value = value
                });
                List<ResultType> resultList;

                // Call server
                int status = _clientCom.SetValue(connectRef, writeItemList, out resultList);

                if (status != 0)
                {
                    error = "Communication error! " + status.ToString();
                }

                if (resultList == null)
                    error = "The Server returned <null>.";
            }
            catch (Exception ex)
            {
                error = string.Format("{0}\n{1}", ex.GetType().Name, ex.Message);
            }

            if (error != null)
            {
                Console.WriteLine(prefix + error);

            }
        }

        /// <summary>
        /// Set Value
        /// </summary>
        /// <param name="id">List of Id and corresponding valuet to change</param>
        void SetValue(List<ValueTypeStateless> writeItemList)
        {
            string error = null;
            try
            {
                List<ResultType> resultList;

                // Call server
                int status = _clientCom.SetValue(connectRef, writeItemList, out resultList);

                if (status != 0)
                {
                    error = "Communication error! " + status.ToString();
                }

                if (resultList == null)
                    error = "The Server returned <null>.";
            }
            catch (Exception ex)
            {
                error = string.Format("{0}\n{1}", ex.GetType().Name, ex.Message);
            }

            if (error != null)
            {
                Console.WriteLine(prefix + error);

            }
        }

        public void stopExperiment()
        {

            Console.WriteLine("In stopExperiment method: " );
            BEELabExpHandler handler = new BEELabExpHandler();
            handler.connectToServer();

            // set start experiment to false
            handler.SetValue(ExperimentConstants.StartExperiemntId, "False");

            // No need to set chamber occupancy to false
            // some other running experiments might need this variable to true

        }


        /// <summary>
        /// Reports the exception caught as well as the inner exception when set.
        /// </summary>
        /// <param name="exp">The selected profile.</param>
        /// <param name="exp">Start Index i.e from which hour</param>
        /// <param name="exp">End Index i.e till which hour</param>
        private string createSetPointStringBasedOnIndex(string profile, int startIndex,int endIndex)
        {
            string[] setPointArray = new string[] { };
            string finalString = "";

            if (profile.Equals("Phoenix"))
                setPointArray = ExperimentConstants.PhoenixSetPointArray;
            if (profile.Equals("Atlanta"))
                setPointArray = ExperimentConstants.AtlantaSetPointArray;

            
            var subArray = setPointArray.Skip(startIndex-1).Take(endIndex-startIndex+1);

            finalString = string.Join("|", subArray);
            //Console.WriteLine(" Final String is " + finalString);

            return finalString;

        } 




        /// <summary>
        /// Reports the exception caught as well as the inner exception when set.
        /// </summary>
        /// <param name="exp">The exception caught.</param>
        private void ReportException(Exception exp)
        {
            Console.WriteLine("!!!" + exp.Message);
            System.Windows.MessageBox.Show(exp.Message, "BEELabExperiment");
            if (exp.InnerException != null)
            {
                Console.WriteLine("!!!" + exp.InnerException.Message);
                System.Windows.MessageBox.Show(exp.InnerException.Message, "BEELabExperiment");
            }
        }
        
    }
}
