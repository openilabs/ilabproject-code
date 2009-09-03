using System;
using System.Xml;
using Library.Lab;
using Library.LabClient;

namespace LabClientHtml
{
    public partial class LabClientMaster : System.Web.UI.MasterPage
    {
        #region Class Constants

        private const string STRLOG_ClassName = "LabClientMaster";

        //
        // String constants
        //
        public const string STR_Initialising = " Initialising...";
        public const string STR_CouponID = "couponID";
        public const string STR_CouponPasskey = "passkey";
        public const string STR_Version = "Version ";
        public const string STR_MailTo = "mailto:";

        //
        // String constants for logfile messages
        //
        public const string STRLOG_GettingLabStatus = " Getting Lab Status...";
        public const string STRLOG_Online = " Online: ";
        public const string STRLOG_LabStatusMessage = " LabStatus Message: ";
        public const string STRLOG_GettingLabConfiguration = " Getting Lab Configuration XML string...";
        public const string STRLOG_LoadingLabConfiguration = " Loading Lab Configuration XML string...";
        public const string STRLOG_ParsingLabConfiguration = " Parsing Lab Configuration...";
        public const string STRLOG_Title = " Title: ";
        public const string STRLOG_Version = " Version: ";
        public const string STRLOG_PhotoUrl = " Photo Url: ";
        public const string STRLOG_LabInfoText = " LabInfo Text: ";
        public const string STRLOG_LabInfoUrl = " LabInfo Url: ";

        //
        // Local variables
        //
        private static bool initialised = false;
        private static string navmenuPhotoUrl;
        private static string mailtoUrl;

        #endregion

        #region Properties

        private static string bannerTitle;
        private static string statusVersion;
        private static string labinfoText;
        private static string labinfoUrl;
        private static bool multipleSubmit;
        private static XmlNode xmlNodeLabConfiguration;
        private static XmlNode xmlNodeConfiguration;
        private static XmlNode xmlNodeValidation;
        private static XmlNode xmlNodeSpecification;
        private static LabClientToSbAPI labClientToSbAPI;

        public string Title
        {
            get { return bannerTitle; }
        }

        public string Version
        {
            get { return statusVersion; }
        }

        public string LabinfoText
        {
            get { return labinfoText; }
        }

        public string LabinfoUrl
        {
            get { return labinfoUrl; }
        }

        public string PageTitle
        {
            get { return bannerTitle + " - "; }
        }

        public string HeaderTitle
        {
            get { return Header.Title; }
            set { Header.Title = value; }
        }

        public bool MultipleSubmit
        {
            get { return multipleSubmit; }
        }

        public XmlNode XmlNodeLabConfiguration
        {
            get { return xmlNodeLabConfiguration; }
        }

        public XmlNode XmlNodeConfiguration
        {
            get { return xmlNodeConfiguration; }
        }

        public XmlNode XmlNodeValidation
        {
            get { return xmlNodeValidation; }
        }

        public XmlNode XmlNodeSpecification
        {
            get { return xmlNodeSpecification; }
        }

        public LabClientToSbAPI ServiceBroker
        {
            get { return labClientToSbAPI; }
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        protected void Page_Init(object sender, EventArgs e)
        {
            const string STRLOG_MethodName = "Page_Init";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            //
            // This method gets called before any other Page_Init() or Page_Load() methods.
            //

            //
            // Check if initialisation has already been carried out
            //
            if (initialised == false)
            {
                //
                // Carry out one-time initialisation for all LabClient instances
                //
                Logfile.Write(STR_Initialising);

                //
                // Create authorisation header instance and fill in
                //
                long couponID = 0;
                string couponPasskey = null;
                try
                {
                    // Get coupon identification number
                    couponID = Convert.ToInt64(Request.QueryString[STR_CouponID]);

                    // Get passkey argument
                    couponPasskey = Request.QueryString[STR_CouponPasskey];
                }
                catch (Exception ex)
                {
                    // couponID or passkey argument was not found
                    Logfile.WriteError(ex.Message);
                }

                //
                // Create ServiceBroker interface with authorisation information
                //
                labClientToSbAPI = new LabClientToSbAPI(couponID, couponPasskey);

                //
                // Get the lab status and lab configuration
                //
                try
                {
                    //
                    // Get the lab status
                    //
                    Logfile.Write(STRLOG_GettingLabStatus);

                    LabStatus labStatus = ServiceBroker.GetLabStatus();

                    Logfile.Write(STRLOG_Online + labStatus.online.ToString());
                    Logfile.Write(STRLOG_LabStatusMessage + labStatus.labStatusMessage);

                    if (labStatus.online == true)
                    {
                        //
                        // Get the XML lab configuration string
                        //
                        Logfile.Write(STRLOG_GettingLabConfiguration);

                        string xmlLabConfiguration = ServiceBroker.GetLabConfiguration();
                        if (xmlLabConfiguration != null)
                        {
                            // Log a portion of the lab configuration string
                            int len = xmlLabConfiguration.Length;
                            Logfile.Write(" " + xmlLabConfiguration.Substring(0, (len > 64) ? 64 : len));

                            // Save information from the lab configuration string
                            ParseLabConfiguration(xmlLabConfiguration);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        Logfile.Write(ex.InnerException.Message);
                    }
                    else
                    {
                        Logfile.WriteError(ex.Message);
                    }
                }

                //
                // Get feedback email URL
                //
                mailtoUrl = STR_MailTo + Utilities.GetAppSetting(Consts.StrCfg_FeedbackEmail);

                //
                // Determine if multiple submission is enabled
                //
                try
                {
                    multipleSubmit = Convert.ToBoolean(Utilities.GetAppSetting(Consts.StrCfg_MultipleSubmit));
                }
                catch
                {
                    multipleSubmit = false;
                }

                initialised = true;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }

        //---------------------------------------------------------------------------------------//

        protected void Page_Load(object sender, EventArgs e)
        {
            Banner.Title = bannerTitle;
            Status.Version = STR_Version + statusVersion;
            Navmenu.PhotoUrl = navmenuPhotoUrl;
            Feedback.MailtoUrl = mailtoUrl;
        }

        //---------------------------------------------------------------------------------------//

        private void ParseLabConfiguration(string xmlLabConfiguration)
        {
            const string STRLOG_MethodName = "ParseLabConfiguration";

            Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

            try
            {
                Logfile.Write(STRLOG_LoadingLabConfiguration);

                //
                // Load the lab configuration from an XML string
                //
                XmlDocument xmlDocument = XmlUtilities.GetXmlDocument(xmlLabConfiguration);

                //
                // Save a copy of the lab configuration XML node
                //
                XmlNode xmlNode = XmlUtilities.GetXmlRootNode(xmlDocument, Consts.STRXML_labConfiguration);
                xmlNodeLabConfiguration = xmlNode.Clone();

                Logfile.Write(STRLOG_ParsingLabConfiguration);

                //
                // Get information from the lab configuration node
                //
                bannerTitle = XmlUtilities.GetXmlValue(xmlNodeLabConfiguration, Consts.STRXMLPARAM_title, false);
                statusVersion = XmlUtilities.GetXmlValue(xmlNodeLabConfiguration, Consts.STRXMLPARAM_version, false);
                navmenuPhotoUrl = XmlUtilities.GetXmlValue(xmlNodeLabConfiguration, Consts.STRXML_navmenuPhoto_image, false);
                labinfoText = XmlUtilities.GetXmlValue(xmlNodeLabConfiguration, Consts.STRXML_labInfo_text, true);
                labinfoUrl = XmlUtilities.GetXmlValue(xmlNodeLabConfiguration, Consts.STRXML_labInfo_url, true);

                Logfile.Write(STRLOG_Title + bannerTitle);
                Logfile.Write(STRLOG_Version + statusVersion);
                Logfile.Write(STRLOG_PhotoUrl + navmenuPhotoUrl);
                Logfile.Write(STRLOG_LabInfoText + labinfoText);
                Logfile.Write(STRLOG_LabInfoUrl + labinfoUrl);

                //
                // These are mandatory
                //
                xmlNode = XmlUtilities.GetXmlNode(xmlNodeLabConfiguration, Consts.STRXML_configuration, false);
                xmlNodeConfiguration = xmlNode.Clone();
                xmlNode = XmlUtilities.GetXmlNode(xmlNodeLabConfiguration, Consts.STRXML_experimentSpecification, false);
                xmlNodeSpecification = xmlNode.Clone();

                //
                // These are optional and depend on the LabServer implementation
                //
                xmlNode = XmlUtilities.GetXmlNode(xmlNodeLabConfiguration, Consts.STRXML_validation, true);
                if (xmlNode != null)
                {
                    xmlNodeValidation = xmlNode.Clone();
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
                throw;
            }

            Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
        }
    }
}