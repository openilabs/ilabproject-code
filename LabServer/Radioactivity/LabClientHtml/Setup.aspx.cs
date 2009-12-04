using System;
using System.Drawing;
using System.IO;
using System.Xml;
using Library.Lab;
using Library.LabClient;

namespace LabClientHtml
{
    public partial class Setup : System.Web.UI.Page
    {
        #region Class Constants and Variables

        //
        // String constants
        //
        private const string STR_ExperimentNo = "Experiment #";
        private const string STR_HasBeenSubmitted = " has been submitted.";
        private const string STR_ExperimentNos = "Experiments #";
        private const string STR_HaveBeenSubmitted = " have been submitted.";

        //
        // String constants for error messages
        //
        private const string STRERR_NullConfiguration = "Configuration is null!";

        #endregion

        #region Properties

        private string Message
        {
            get { return this.lblMessage.Text; }
            set
            {
                this.lblMessage.ForeColor = Color.Black;
                this.lblMessage.Text = value;
            }
        }

        private string MessageError
        {
            get { return this.lblMessage.Text; }
            set
            {
                this.lblMessage.ForeColor = Color.Red;
                this.lblMessage.Text = value;
            }
        }

        private string MessageFailure
        {
            get { return this.lblMessage.Text; }
            set
            {
                this.lblMessage.ForeColor = Color.Blue;
                this.lblMessage.Text = value;
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------//

        protected void Page_Load(object sender, EventArgs e)
        {
            //
            // Set webpage title
            //
            Master.HeaderTitle = this.Title;
            this.Title = Master.PageTitle + this.Title;

            //
            // Update local variables
            //
            labSetup.XmlNodeConfiguration = Master.XmlNodeConfiguration;
            labSetup.XmlNodeValidation = Master.XmlNodeValidation;

            if (!IsPostBack)
            {
                //
                // It is not a postback. This page has been navigated to or been refreshed.
                //
                PopulatePageControls();
            }
            else
            {
                //
                // It is a postback. A button on this page has been clicked to post back information.
                //
                this.Message = null;
            }

            //
            // Check if an experiment has been submitted
            //
            if (Master.MultipleSubmit == true)
            {
                if (Session[Consts.STRSSN_SubmittedIDs] != null)
                {
                    int[] submittedIDs = (int[])Session[Consts.STRSSN_SubmittedIDs];
                    if (submittedIDs.Length == 1 && submittedIDs[0] > 0)
                    {
                        this.Message = STR_ExperimentNo + submittedIDs[0].ToString() + STR_HasBeenSubmitted;
                    }
                    else if (submittedIDs.Length > 1)
                    {
                        string submittedIds = "";
                        for (int i = 0; i < submittedIDs.Length; i++)
                        {
                            submittedIds += submittedIDs[i].ToString() + " ";
                        }
                        this.Message = STR_ExperimentNos + submittedIds + STR_HaveBeenSubmitted;
                    }
                }
            }
            else
            {
                if (Session[Consts.STRSSN_SubmittedID] != null)
                {
                    int submittedID = (int)Session[Consts.STRSSN_SubmittedID];
                    if (submittedID > 0)
                    {
                        //
                        // Experiment has been submitted but not checked
                        //
                        this.Message = STR_ExperimentNo + submittedID.ToString() + STR_HasBeenSubmitted;
                        btnSubmit.Enabled = false;
                    }
                }
            }
        }

        //-------------------------------------------------------------------------------------------------//

        private void PopulatePageControls()
        {
            //
            // Cannot do anything without a configuration
            //
            if (Master.XmlNodeConfiguration == null)
            {
                return;
            }

            //
            // Get all setups and add to the dropdownlist
            //
            XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(Master.XmlNodeConfiguration, Consts.STRXML_setup, true);
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlNode xmlNodeTemp = xmlNodeList.Item(i);

                //
                // Get the setup id and setup name and add to the dropdown list
                //
                string setupId = XmlUtilities.GetXmlValue(xmlNodeTemp, Consts.STRXMLPARAM_id, true);
                string setupName = XmlUtilities.GetXmlValue(xmlNodeTemp, Consts.STRXML_name, true);
                if (setupId.Length > 0 && setupName.Length > 0)
                {
                    ddlExperimentSetupIds.Items.Add(setupId);
                    ddlExperimentSetups.Items.Add(setupName);
                }
            }

            //
            // Set the selected index for the experiment setups and tell LabSetup control
            //
            ddlExperimentSetups.SelectedIndex = 0;
            labSetup.XmlNodeSelectedSetup = xmlNodeList.Item(ddlExperimentSetups.SelectedIndex);

            //
            // Update page controls for the selected index
            //
            UpdatePageControls();
        }

        //-------------------------------------------------------------------------------------------------//

        private void UpdatePageControls()
        {
            //
            // Cannot do anything without a configuration
            //
            if (Master.XmlNodeConfiguration == null)
            {
                return;
            }

            //
            // Update page controls for the selected index
            //
            XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(Master.XmlNodeConfiguration, Consts.STRXML_setup, false);
            XmlNode xmlNodeSetup = xmlNodeList.Item(ddlExperimentSetups.SelectedIndex);

            // Set the description for the setup
            lblSetupDescription.Text = XmlUtilities.GetXmlValue(xmlNodeSetup, Consts.STRXML_description, false);
        }

        //-------------------------------------------------------------------------------------------------//

        protected void ddlExperimentSetups_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
            // Cannot do anything without a configuration
            //
            if (Master.XmlNodeConfiguration == null)
            {
                return;
            }

            // Update page controls for the selected index
            UpdatePageControls();

            //
            // Tell LabSetup control that a different setup has been selected
            //
            XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(Master.XmlNodeConfiguration, Consts.STRXML_setup, false);
            labSetup.XmlNodeSelectedSetup = xmlNodeList.Item(ddlExperimentSetups.SelectedIndex);
            labSetup.ddlExperimentSetups_SelectedIndexChanged(sender, e);
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnValidate_Click1(object sender, EventArgs e)
        {
            try
            {
                // Build the XML specification string
                string xmlSpecification = this.BuildSpecification();

                // Validate the experiment specification
                ValidationReport validationReport = Master.ServiceBroker.Validate(xmlSpecification);

                //
                // Check if specification was accepted
                //
                if (validationReport.accepted)
                {
                    string message = "Specification is valid. ";

                    // Get runtime in minutes and seconds
                    int minutes = (int)validationReport.estRuntime / 60;
                    int seconds = (int)validationReport.estRuntime - (minutes * 60);

                    // Display expected runtime
                    string plural = null;
                    message += "Execution time will be ";
                    if (minutes > 0)
                    {
                        // Display minutes
                        plural = (minutes == 1) ? "" : "s";
                        message += minutes.ToString() + " minute" + plural + " and ";
                    }
                    // Display seconds
                    plural = (seconds == 1) ? "" : "s";
                    message += seconds.ToString() + " second" + plural + ".";

                    // Specification was accepted
                    this.Message = message;
                }
                else
                {
                    // Specification was rejected
                    this.MessageError = validationReport.errorMessage;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    this.MessageFailure = ex.InnerException.Message;
                }
                else
                {
                    this.MessageFailure = ex.Message;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnSubmit_Click1(object sender, EventArgs e)
        {
            //
            // Check if an experiment has already been submitted. The Submit button's
            // enable state gets set when page is loaded.
            //
            if (btnSubmit.Enabled == false)
            {
                return;
            }

            //
            // Submit experiment specification
            //
            try
            {
                // Build the XML specification string
                string xmlSpecification = this.BuildSpecification();

                // Submit the experiment specification
                SubmissionReport submissionReport = Master.ServiceBroker.Submit(xmlSpecification);
                WaitEstimate wait = submissionReport.wait;

                // Check that the specification was accepted ok
                if (submissionReport.vReport.accepted == true)
                {
                    // Get experiment number
                    int experimentID = submissionReport.experimentID;

                    string message = "Submission successful. " +
                        "Experiment is #" + experimentID.ToString() + ". ";

                    // Get runtime in minutes and seconds
                    int minutes = (int)submissionReport.vReport.estRuntime / 60;
                    int seconds = (int)submissionReport.vReport.estRuntime - (minutes * 60);

                    // Display accepted message and runtime
                    string plural = null;
                    message += "Execution time is ";
                    if (minutes > 0)
                    {
                        // Display minutes
                        plural = (minutes == 1) ? "" : "s";
                        message += minutes.ToString() + " minute" + plural + " and ";
                    }
                    // Display seconds
                    plural = (seconds == 1) ? "" : "s";
                    message += seconds.ToString() + " second" + plural + ".";

                    // Submission was accepted
                    this.Message = message;

                    //
                    // Experiment has been submitted successfully
                    //
                    if (Master.MultipleSubmit == true)
                    {
                        // Add experiment ID to the list in the session
                        if (Session[Consts.STRSSN_SubmittedIDs] != null)
                        {
                            // Get the list of submitted experiment IDs
                            int[] submittedIDs = (int[])Session[Consts.STRSSN_SubmittedIDs];

                            // Create a bigger array and copy submitted experiment IDs
                            int[] newSubmittedIDs = new int[submittedIDs.Length + 1];
                            submittedIDs.CopyTo(newSubmittedIDs, 0);

                            // Add the experiment ID to the bigger array
                            newSubmittedIDs[submittedIDs.Length] = experimentID;

                            // Save experiment IDs in the session
                            Session[Consts.STRSSN_SubmittedIDs] = newSubmittedIDs;
                        }
                        else
                        {
                            // Create an array and add the experiment ID
                            int[] submittedIDs = new int[1];
                            submittedIDs[0] = experimentID;

                            // Save experiment IDs in the session
                            Session[Consts.STRSSN_SubmittedIDs] = submittedIDs;
                        }
                    }
                    else
                    {
                        // Save experiment ID in the session
                        Session[Consts.STRSSN_SubmittedID] = experimentID;

                        // Update buttons
                        btnValidate.Enabled = false;
                        btnSubmit.Enabled = false;
                    }
                }
                else
                {
                    // Submission was rejected
                    this.MessageError = submissionReport.vReport.errorMessage;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    this.MessageFailure = ex.InnerException.Message;
                }
                else
                {
                    this.MessageFailure = ex.Message;
                }
            }
        }

        //---------------------------------------------------------------------------------------//

        private string BuildSpecification()
        {
            //
            // Get setup Id
            //
            XmlNodeList xmlNodeList = XmlUtilities.GetXmlNodeList(Master.XmlNodeConfiguration, Consts.STRXML_setup, false);
            XmlNode xmlNodeSetup = xmlNodeList.Item(ddlExperimentSetups.SelectedIndex);
            string setupId = XmlUtilities.GetXmlValue(xmlNodeSetup, Consts.STRXMLPARAM_id, false);

            //
            // Get a copy of the XML specification node and fill in
            //
            XmlNode xmlNodeSpecification = Master.XmlNodeSpecification.Clone();
            XmlUtilities.SetXmlValue(xmlNodeSpecification, Consts.STRXML_setupId, setupId, true);
            xmlNodeSpecification = labSetup.BuildSpecification(xmlNodeSpecification, setupId);

            //
            // Write the Xml specification to a string
            //
            StringWriter xmlSpecification = new StringWriter();
            XmlTextWriter xtw = new XmlTextWriter(xmlSpecification);
            xtw.Formatting = Formatting.Indented;
            xmlNodeSpecification.WriteTo(xtw);
            xtw.Flush();

            return xmlSpecification.ToString();
        }
    }
}
