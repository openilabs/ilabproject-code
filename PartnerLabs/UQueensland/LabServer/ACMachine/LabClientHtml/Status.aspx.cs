using System;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Library.Lab;
using Library.LabClient;

namespace LabClientHtml
{
    public partial class Status : System.Web.UI.Page
    {
        #region Class Constants and Variables

        //
        // String constants for error messages
        //
        private const string STRERR_InvalidExperimentNumber = "Experiment number is invalid!";
        private const string STRERR_NoExperimentNumber = "Experiment number is not specified!";
        private const string STRERR_GetExperimentStatusFailed = "Failed to get experiment status!\r\n";

        #endregion

        //---------------------------------------------------------------------------------------//

        protected void Page_Init(object sender, EventArgs e)
        {
            //
            // Set webpage title
            //
            Master.HeaderTitle = this.Title;
            this.Title = Master.PageTitle + this.Title;
        }

        //-------------------------------------------------------------------------------------------------//

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //
                // It is not a postback. This page has been navigated to or been refreshed.
                //
                btnRefresh_Click1(sender, e);

                //
                // Set the dropdown list to not visible
                //
                ddlExperimentIDs.Visible = false;

                //
                // Initialise experiment number if experiment has been submitted
                //
                if (Master.MultiSubmit == true)
                {
                    if (Session[Consts.STRSSN_SubmittedIDs] != null)
                    {
                        //
                        // Get the list of submitted experiment IDs
                        //
                        int[] submittedIDs = (int[])Session[Consts.STRSSN_SubmittedIDs];
                        if (submittedIDs.Length > 0)
                        {
                            if (submittedIDs.Length == 1)
                            {
                                // There is only one
                                txbExperimentID.Text = submittedIDs[0].ToString();
                            }
                            else
                            {
                                //
                                // Populate the dropdown list with submitted experiment IDs
                                //
                                ddlExperimentIDs.Items.Clear();
                                for (int i = 0; i < submittedIDs.Length; i++)
                                {
                                    ddlExperimentIDs.Items.Add(submittedIDs[i].ToString());
                                }

                                //
                                // Insert an empty experiment ID at the start of the list so that
                                // the selected index can be changed
                                //
                                ddlExperimentIDs.Items.Insert(0, String.Empty);

                                // Make it visible
                                ddlExperimentIDs.Visible = true;
                            }
                        }
                    }
                }
                else
                {
                    //
                    // Get the submitted experiment ID
                    //
                    if (Session[Consts.STRSSN_SubmittedID] != null)
                    {
                        int submittedID = (int)Session[Consts.STRSSN_SubmittedID];
                        if (submittedID > 0)
                        {
                            txbExperimentID.Text = submittedID.ToString();
                        }
                    }
                }
            }
            else
            {
                //
                // It is a postback. A button on this page has been clicked to post back information.
                //
            }
        }

        //-------------------------------------------------------------------------------------------------//

        private void ShowExperimentMessageNormal(string message)
        {
            lblExperimentStatusMsg.ForeColor = Color.Black;
            lblExperimentStatusMsg.Text = message;
        }

        //-------------------------------------------------------------------------------------------------//

        private void ShowExperimentMessageError(string message)
        {
            lblExperimentStatusMsg.ForeColor = Color.Red;
            lblExperimentStatusMsg.Text = message;
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnRefresh_Click1(object sender, EventArgs e)
        {
            //
            // Get the LabServer status and display
            //
            bool online = false;
            string message = null;
            try
            {
                //
                // Get the lab status
                //
                LabStatus labStatus = Master.ServiceBroker.GetLabStatus();

                online = labStatus.online;
                message = labStatus.labStatusMessage;

                if (online == true)
                {
                    // Get the queue length
                    WaitEstimate waitEstimate = Master.ServiceBroker.GetEffectiveQueueLength();

                    //
                    // Display queue length
                    //
                    int queueLength = waitEstimate.effectiveQueueLength;
                    string plural = (queueLength == 1) ? "" : "s";
                    message += " - There ";
                    message += (queueLength == 1) ? "is " : "are ";
                    message += queueLength.ToString() + " experiment" + plural + " queued.";

                    //
                    // Display queue wait estimate
                    //
                    if (queueLength > 0)
                    {
                        int waitTime = (int)waitEstimate.estWait;
                        int minutes = waitTime / 60;
                        int seconds = waitTime - (minutes * 60);
                        plural = null;
                        message += " Queue wait time is ";
                        if (minutes > 0)
                        {
                            // Display minutes
                            plural = (minutes == 1) ? "" : "s";
                            message += minutes.ToString() + " minute" + plural + " and ";
                        }
                        // Display seconds
                        plural = (seconds == 1) ? "" : "s";
                        message += seconds.ToString() + " second" + plural + ".";
                    }

                    //
                    // Display lab status
                    //
                    lblOnline.ForeColor = Color.Green;
                    lblOnline.Text = "Online";
                }
                else
                {
                    lblOnline.ForeColor = Color.Red;
                    lblOnline.Text = "Offline";
                }

                //
                // Display lab status message
                //
                lblLabServerStatusMsg.Text = message;
            }
            catch (Exception ex)
            {
                // LabServer error
                lblLabServerStatusMsg.Text = ex.Message;
            }
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            //
            // Get the experiment ID
            //
            int experimentID = ParseExperimentNumber(txbExperimentID.Text);
            if (experimentID < 0)
            {
                return;
            }

            LabExperimentStatus labExperimentStatus = null;
            StatusCodes statusCode;

            //
            // Get the experiment status
            //
            try
            {
                //
                // Get the status of the selected experiment
                //
                labExperimentStatus = Master.ServiceBroker.GetExperimentStatus(experimentID);
                statusCode = (StatusCodes)labExperimentStatus.statusReport.statusCode;
            }
            catch (Exception)
            {
                statusCode = StatusCodes.Unknown;
            }

            //
            // Display the status code
            //
            string message = "Experiment #" + experimentID.ToString() + " is " + statusCode.ToString() + ".";
            ShowExperimentMessageNormal(message);

            if (statusCode == StatusCodes.Running)
            {
                //
                // Experiment is currently running, display time remaining
                //
                int timeRemaining = (int)Decimal.Round((decimal)labExperimentStatus.statusReport.estRemainingRuntime);
                int minutes = timeRemaining / 60;
                int seconds = timeRemaining - (minutes * 60);

                //
                // Display experiment status and remaining time
                //
                string plural = null;
                message += " Time remaining is ";
                if (minutes > 0)
                {
                    // Display minutes
                    plural = (minutes == 1) ? "" : "s";
                    message += minutes.ToString() + " minute" + plural + " and ";
                }
                // Display seconds
                plural = (seconds == 1) ? "" : "s";
                message += seconds.ToString() + " second" + plural + ".";
            }

            else if (statusCode == StatusCodes.Waiting)
            {
                //
                // Experiment is waiting to run, get queue position (zero-based)
                //
                int queuePosition = labExperimentStatus.statusReport.wait.effectiveQueueLength;
                int queueWaitTime = (int)Decimal.Round((decimal)labExperimentStatus.statusReport.wait.estWait);
                if (queueWaitTime < 0)
                {
                    queueWaitTime = 0;
                }

                int minutes = queueWaitTime / 60;
                int seconds = queueWaitTime - (minutes * 60);

                //
                // Display experiment status and queue wait time
                //
                string plural = null;
                message += " Queue position is " + queuePosition.ToString() + ". It will run in ";
                if (minutes > 0)
                {
                    // Display minutes
                    plural = (minutes == 1) ? "" : "s";
                    message += minutes.ToString() + " minute" + plural + " and ";
                }
                // Display seconds
                plural = (seconds == 1) ? "" : "s";
                message += seconds.ToString() + " second" + plural + " .";
            }

            else if (statusCode == StatusCodes.Completed || statusCode == StatusCodes.Failed || statusCode == StatusCodes.Cancelled)
            {
                //
                // Experiment status no longer needs to be checked
                //
                if (Master.MultiSubmit == true)
                {
                    if (Session[Consts.STRSSN_SubmittedIDs] != null)
                    {
                        //
                        // Get the list of submitted experiment IDs
                        //
                        int[] submittedIDs = (int[])Session[Consts.STRSSN_SubmittedIDs];

                        //
                        // Find submitted experiment number
                        //
                        for (int i = 0; i < submittedIDs.Length; i++)
                        {
                            if (submittedIDs[i] == experimentID)
                            {
                                //
                                // Add experiment number to the completed list in the session
                                //
                                if (Session[Consts.STRSSN_CompletedIDs] != null)
                                {
                                    // Get the list of completed experiment IDs
                                    int[] completedIDs = (int[])Session[Consts.STRSSN_CompletedIDs];

                                    // Create a bigger array and copy completed experiment IDs
                                    int[] newCompletedIDs = new int[completedIDs.Length + 1];
                                    completedIDs.CopyTo(newCompletedIDs, 0);

                                    // Add the experiment ID to the bigger array
                                    newCompletedIDs[completedIDs.Length] = experimentID;

                                    // Save experiment ID in the session
                                    Session[Consts.STRSSN_CompletedIDs] = newCompletedIDs;
                                }
                                else
                                {
                                    // Create an array and add the experiment ID
                                    int[] completedIDs = new int[1];
                                    completedIDs[0] = experimentID;

                                    // Save experiment ID in the session
                                    Session[Consts.STRSSN_CompletedIDs] = completedIDs;
                                }

                                //
                                // Remove experiment number from the submitted list in the session
                                //
                                if (submittedIDs.Length == 1)
                                {
                                    Session[Consts.STRSSN_SubmittedIDs] = null;

                                    //
                                    // Clear dropdown list and hide
                                    //
                                    ddlExperimentIDs.Items.Clear();
                                    ddlExperimentIDs.Visible = false;
                                }
                                else
                                {
                                    //
                                    // Create a smaller array and copy submitted experiment IDs
                                    //
                                    int[] newSubmittedIDs = new int[submittedIDs.Length - 1];

                                    //
                                    // Copy experiment IDs up to the one being removed
                                    //
                                    for (int j = 0; j < i; j++)
                                    {
                                        newSubmittedIDs[j] = submittedIDs[j];
                                    }

                                    //
                                    // Copy experiment IDs after the one being removed
                                    //
                                    for (int j = i + 1; j < submittedIDs.Length; j++)
                                    {
                                        newSubmittedIDs[j - 1] = submittedIDs[j];
                                    }

                                    // Save experiment IDs in the session
                                    Session[Consts.STRSSN_SubmittedIDs] = newSubmittedIDs;

                                    // Remove experiment ID from dropdown list
                                    ddlExperimentIDs.Items.Remove(experimentID.ToString());
                                }

                                break;
                            }
                        }

                    }
                }
                else
                {
                    if (Session[Consts.STRSSN_SubmittedID] != null)
                    {
                        //
                        // Check experiment ID against submitted experiment ID
                        //
                        int submittedId = (int)Session[Consts.STRSSN_SubmittedID];
                        if (experimentID == submittedId)
                        {
                            Session[Consts.STRSSN_CompletedID] = submittedId;
                            Session[Consts.STRSSN_SubmittedID] = null;
                        }
                    }
                }
            }

            // Display experiment status
            ShowExperimentMessageNormal(message);

            // Clear the LabServer status
            lblLabServerStatusMsg.Text = null;
        }

        //-------------------------------------------------------------------------------------------------//

        protected void ddlExperimentIDs_SelectedIndexChanged(object sender, EventArgs e)
        {
            txbExperimentID.Text = ddlExperimentIDs.SelectedValue;
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //
            // Get the experiment number
            //
            int experimentNo = ParseExperimentNumber(txbExperimentID.Text);
            if (experimentNo < 0)
            {
                return;
            }

            try
            {
                //
                // Attempt to cancel the selected experiment
                //
                bool cancelled = Master.ServiceBroker.Cancel(experimentNo);

                //
                // Display cancel status
                //
                string message = "Experiment #" + experimentNo.ToString();
                if (cancelled == true)
                {
                    ShowExperimentMessageNormal(message + " has been cancelled.");
                }
                else
                {
                    ShowExperimentMessageError(message + " could not be cancelled!");
                }
            }
            catch (Exception ex)
            {
                ShowExperimentMessageError(ex.Message);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        private int ParseExperimentNumber(string strNumber)
        {
            //
            // Get the experiment number
            //
            int experimentNo = -1;
            try
            {
                // Check if experiment number is entered
                if (txbExperimentID.Text.Trim().Length == 0)
                {
                    throw new ArgumentException(STRERR_NoExperimentNumber);
                }

                // Determine the experiment number
                experimentNo = Int32.Parse(strNumber);

                // Check that experiment number is greater than 0
                if (experimentNo <= 0)
                {
                    throw new ArithmeticException(STRERR_InvalidExperimentNumber);
                }

                // Experiment number is valid
                ShowExperimentMessageNormal(null);
            }
            catch (Exception ex)
            {
                ShowExperimentMessageError(ex.Message);
            }

            return experimentNo;
        }

    }
}
