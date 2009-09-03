using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Web.UI;
using Library.Lab;
using Library.LabClient;
using LabClientHtml.LabControls;

namespace LabClientHtml
{
    public partial class Results : System.Web.UI.Page
    {
        #region Class Constants and Variables

        //
        // String constants
        //
        private const string STR_InvalidExperimentNumber = "Experiment number is invalid!";
        private const string STR_NoExperimentNumber = "Experiment number is not specified!";

        private const string STR_ExperimentInformation = "Experiment Information";
        private const string STR_CsvExperimentInformation = "---Experiment Information---";
        private const string STR_Timestamp = "Timestamp";
        private const string STR_ExperimentID = "Experiment ID";
        private const string STR_UnitID = "Unit ID";
        private const string STR_setupName = "Setup Name";

        private const string STR_ExperimentSetup = "Experiment Setup";
        private const string STR_CsvExperimentSetup = "---Experiment Setup---";

        private const string STR_ExperimentResults = "Experiment Results";
        private const string STR_CsvExperimentResults = "---Experiment Results---";

        private const string STR_ErrorMessage = "Error Message";

        private const string STR_SwTblBegin = "<table cols=\"3\" cellpadding=\"5\">";
        private const string STR_SwTblHdrArgument = "<tr align=\"left\"><th colspan=\"3\"><nobr>{0}</nobr></th></tr>";
        private const string STR_SwTblArgument = "<tr><td><nobr>{0}:</nobr></td><td><nobr>{1}</nobr></td><td width=\"100%\">&nbsp;</td></tr>";
        private const string STR_SwTblBlankRow = "<tr><td colspan=\"3\">&nbsp;</td></tr>";
        private const string STR_SwTblEnd = "</table>";

        private const string STR_SwCsvArgument = "{0},{1}";

        private const string STR_SwAppletArgument = "<param name=\"{0}\" value=\"{1}\">";
        private const string STR_HtmlAppletTag_Args5 =
            "<applet width=\"{0}\" height=\"{1}\" archive=\"{2}\" code=\"{3}\"\r\n" +
            "alt=\"This is ALT text.  Couldn't load the applet.\">\r\n" +
            "{4}\r\n" +
            "<strong>Could not load the applet!</strong>\r\n" +
            "</applet>\r\n";

        #endregion

        //-------------------------------------------------------------------------------------------------//

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.HeaderTitle = this.Title;
            this.Title = Master.PageTitle + this.Title;

            if (!IsPostBack)
            {
                //
                // It is not a postback. This page has been navigated to or been refreshed.
                //
                lblHiddenResults.Visible = false;
                lblHiddenApplet.Visible = false;
                btnSave.Enabled = false;
                btnDisplay.Enabled = false;
                btnDisplay.Visible = false;

                //
                // Set the dropdown list of completed experiment IDs to not visible
                //
                ddlExperimentIDs.Visible = false;

                //
                // Initialise experiment number if experiment has completed
                //
                if (Master.MultipleSubmit == true)
                {
                    if (Session[Consts.StrSsn_CompletedIDs] != null)
                    {
                        //
                        // Get the list of completed experiment IDs
                        //
                        int[] completedIDs = (int[])Session[Consts.StrSsn_CompletedIDs];
                        if (completedIDs.Length > 0)
                        {
                            if (completedIDs.Length == 1)
                            {
                                // There is only one
                                txbExperimentID.Text = completedIDs[0].ToString();
                            }
                            else
                            {
                                //
                                // Populate the dropdown list with completed experiment IDs
                                //
                                ddlExperimentIDs.Items.Clear();
                                for (int i = 0; i < completedIDs.Length; i++)
                                {
                                    ddlExperimentIDs.Items.Add(completedIDs[i].ToString());
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
                    // Get the completed experiment ID
                    //
                    if (Session[Consts.StrSsn_CompletedID] != null)
                    {
                        int completedID = (int)Session[Consts.StrSsn_CompletedID];
                        if (completedID > 0)
                        {
                            txbExperimentID.Text = completedID.ToString();
                        }
                    }
                }
            }
            else
            {
                //
                // It is a postback. A button on this page has been clicked to post back information.
                //

                // Clear message
                ShowMessageNormal(null);
            }
        }

        //-------------------------------------------------------------------------------------------------//

        private void ShowMessageNormal(string message)
        {
            lblResultMessage.ForeColor = Color.Black;
            lblResultMessage.Text = message;
        }

        //-------------------------------------------------------------------------------------------------//

        private void ShowMessageWarning(string message)
        {
            lblResultMessage.ForeColor = Color.Blue;
            lblResultMessage.Text = message;
        }

        //-------------------------------------------------------------------------------------------------//

        private void ShowMessageError(string message)
        {
            lblResultMessage.ForeColor = Color.Red;
            lblResultMessage.Text = message;
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnRetrieve_Click(object sender, EventArgs e)
        {
            //
            // Clear hidden labels and disable buttons
            //
            lblHiddenResults.Text = null;
            lblHiddenApplet.Text = null;
            btnSave.Enabled = false;
            btnDisplay.Enabled = false;
            btnDisplay.Visible = false;

            //
            // Get the experiment number
            //
            int experimentID = 0;
            try
            {
                // Check if experiment number is entered
                if (txbExperimentID.Text.Trim().Length == 0)
                {
                    throw new ArgumentException();
                }

                // Determine the experiment ID
                experimentID = Int32.Parse(txbExperimentID.Text);

                // Check that experiment ID is greater than 0
                if (experimentID <= 0)
                {
                    throw new Exception();
                }

                // Experiment number is valid
                ShowMessageNormal(null);
            }
            catch (ArgumentException)
            {
                ShowMessageError(STR_NoExperimentNumber);
                return;
            }
            catch (Exception)
            {
                ShowMessageError(STR_InvalidExperimentNumber);
                return;
            }

            //
            // Get the experiment results for the selected experiment
            //
            try
            {
                StatusCodes statusCode;
                string errorMessage = null;
                string experimentResults = null;

                //
                // Get ServiceBrokerAPI from session state and retrieve the result
                //
                try
                {
                    ResultReport resultReport = Master.ServiceBroker.RetrieveResult(experimentID);
                    statusCode = (StatusCodes)resultReport.statusCode;
                    errorMessage = resultReport.errorMessage;
                    experimentResults = resultReport.experimentResults;
                }
                catch (Exception)
                {
                    statusCode = StatusCodes.Unknown;
                }

                //
                // Display the status code
                //
                string message = "Experiment #" + experimentID.ToString() + " - " + statusCode.ToString();
                ShowMessageNormal(message);

                if (statusCode != StatusCodes.Unknown && experimentResults != null)
                {
                    //
                    // Get result information
                    //
                    Result result = new Result(experimentResults);
                    ResultInfo resultInfo = result.GetResultInfo();

                    //
                    // Check experiment type
                    //
                    if (resultInfo != null && resultInfo.title != null &&
                        resultInfo.title.Equals(Master.Title, StringComparison.OrdinalIgnoreCase) == false)
                    {
                        //
                        // Wrong experiment type
                        //
                        errorMessage = "Experiment type is not '" + Master.Title + "'";
                        ShowMessageWarning(errorMessage);
                        return;
                    }

                    //
                    // Build table to display results on the webpage
                    //
                    string strResultsTable = BuildTableResult(resultInfo, statusCode, errorMessage);
                    phResultsTable.Controls.Add(new LiteralControl(strResultsTable));

                    if (statusCode == StatusCodes.Completed)
                    {
                        try
                        {
                            //
                            // Build a CSV string from the result report and store in a hidden label
                            //
                            lblHiddenResults.Text = BuildCsvResult(resultInfo);

                            // Enable button
                            btnSave.Enabled = true;

                            //
                            // Create HTML applet tag for insertion into the webpage
                            //
                            string appletParams = BuildAppletParams(resultInfo);
                            string applet = CreateHtmlAppletTag(appletParams);
                            if (applet != null)
                            {
                                // Save applet for displaying in a hidden label
                                lblHiddenApplet.Text = applet;

                                // Enable applet display
                                btnDisplay.Enabled = true;
                                btnDisplay.Visible = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMessageWarning(ex.Message);
                        }
                    }

                    //
                    // Completed experiment is no longer needed
                    //
                    if (Master.MultipleSubmit == true)
                    {
                    }
                    else
                    {
                        Session[Consts.StrSsn_CompletedID] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageWarning(ex.Message);
                return;
            }
        }

        //-------------------------------------------------------------------------------------------------//

        protected void ddlExperimentIDs_SelectedIndexChanged(object sender, EventArgs e)
        {
            txbExperimentID.Text = ddlExperimentIDs.SelectedValue;
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Download the result string as an Excel csv file

            // Set the content type of the file to be downloaded
            Response.ContentType = Consts.StrRsp_ContentTypeCsv;

            // Clear all response headers
            Response.Clear();

            // Add response header
            Response.AddHeader(Consts.StrRsp_Disposition, Consts.StrRsp_AttachmentCsv);

            // Add specification string
            Response.Write(lblHiddenResults.Text);

            // End the http response
            Response.End();
        }

        //-------------------------------------------------------------------------------------------------//

        protected void btnDisplay_Click(object sender, EventArgs e)
        {
            //
            // Display the experiment result information
            //
            if (lblHiddenApplet.Text != null)
            {
                PlaceHolder1.Controls.Add(new LiteralControl(lblHiddenApplet.Text));
                PlaceHolder1.Controls.Add(new LiteralControl("</td></tr><tr><td>"));
            }
        }

        //-------------------------------------------------------------------------------------------------//

        private string BuildTableResult(ResultInfo resultInfo, StatusCodes statusCode, string errorMessage)
        {
            StringWriter sw = new StringWriter();
            try
            {
                sw.WriteLine(STR_SwTblBegin);

                //
                // Experiment information
                //
                sw.WriteLine(STR_SwTblBlankRow);
                sw.WriteLine(STR_SwTblHdrArgument, STR_ExperimentInformation);
                sw.WriteLine(STR_SwTblArgument, STR_Timestamp, resultInfo.timestamp);
                sw.WriteLine(STR_SwTblArgument, STR_ExperimentID, resultInfo.experimentId);
                if (resultInfo.title != null)
                {
                    sw.WriteLine(STR_SwTblArgument, STR_UnitID, resultInfo.unitId);
                }

                //
                // Experiment setup
                //
                sw.WriteLine(STR_SwTblBlankRow);
                sw.WriteLine(STR_SwTblHdrArgument, STR_ExperimentSetup);
                if (resultInfo.setupName != null)
                {
                    sw.WriteLine(STR_SwTblArgument, STR_setupName, resultInfo.setupName);
                }
                string csvSpecification = labResults.CreateSpecificationString(resultInfo, STR_SwTblArgument);
                sw.Write(csvSpecification);

                //
                // Experiment results
                //
                sw.WriteLine(STR_SwTblBlankRow);
                sw.WriteLine(STR_SwTblHdrArgument, STR_ExperimentResults);

                //
                // Check if experiment had completed successfully
                //
                if (statusCode == StatusCodes.Completed)
                {
                    // Include experiment results
                    string tblResults = labResults.CreateResultsString(resultInfo, STR_SwTblArgument);
                    sw.Write(tblResults);
                }
                else
                {
                    sw.WriteLine(STR_SwTblArgument, STR_ErrorMessage, errorMessage);
                }

                sw.WriteLine(STR_SwTblEnd);
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return sw.ToString();
        }

        //-------------------------------------------------------------------------------------------------//

        private string BuildCsvResult(ResultInfo resultInfo)
        {
            StringWriter sw = new StringWriter();
            try
            {
                //
                // Experiment information
                //
                sw.WriteLine();
                sw.WriteLine(STR_CsvExperimentInformation);
                sw.WriteLine(STR_SwCsvArgument, STR_Timestamp, resultInfo.timestamp);
                sw.WriteLine(STR_SwCsvArgument, STR_ExperimentID, resultInfo.experimentId);
                if (resultInfo.title != null)
                {
                    sw.WriteLine(STR_SwCsvArgument, STR_UnitID, resultInfo.unitId);
                }

                //
                // Experiment setup
                //
                sw.WriteLine();
                sw.WriteLine(STR_CsvExperimentSetup);
                if (resultInfo.title != null)
                {
                    sw.WriteLine(STR_SwCsvArgument, STR_setupName, resultInfo.setupName);
                }
                string csvSpecification = labResults.CreateSpecificationString(resultInfo, STR_SwCsvArgument);
                sw.Write(csvSpecification);

                //
                // Experiment results
                //
                sw.WriteLine();
                sw.WriteLine(STR_CsvExperimentResults);

                string csvResults = labResults.CreateResultsString(resultInfo, STR_SwCsvArgument);
                sw.Write(csvResults);
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return sw.ToString();
        }

        //-------------------------------------------------------------------------------------------------//

        private string BuildAppletParams(ResultInfo resultInfo)
        {
            StringWriter sw = new StringWriter();
            try
            {
                // Experiment specification
                sw.Write(labResults.CreateSpecificationString(resultInfo, STR_SwAppletArgument));

                // Experiment results
                sw.Write(labResults.CreateResultsString(resultInfo, STR_SwAppletArgument));
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return sw.ToString();
        }

        //-------------------------------------------------------------------------------------------------//

        private string CreateHtmlAppletTag(string appletParams)
        {
            // Clear applet string
            lblHiddenApplet.Text = null;

            string htmlAppletTag = null;
            try
            {
                int width = 1;
                int height = 1;

                //
                // Get applet archive and code
                //
                XmlNode xmlNodeLabConfiguration = Master.XmlNodeLabConfiguration;
                string appletArchive = XmlUtilities.GetXmlValue(xmlNodeLabConfiguration, Consts.STRXML_resultsApplet_archive, true);
                string appletCode = XmlUtilities.GetXmlValue(xmlNodeLabConfiguration, Consts.STRXML_resultsApplet_code, true);

                //
                // Ensure that the applet archive and code are valid
                //
                if (appletArchive != null && appletArchive.Length > 0 && appletCode != null && appletCode.Length > 0)
                {
                    //
                    // Check to see if the applet archive file exists
                    //
                    string path = Path.GetDirectoryName(appletArchive);
                    path = MapPath(path);
                    string filename = Path.GetFileName(appletArchive);
                    filename = path + "\\" + filename;
                    if (File.Exists(filename) == true)
                    {
                        //
                        // Create the HTML applet tag
                        //
                        StringWriter sw = new StringWriter();
                        sw.WriteLine(STR_HtmlAppletTag_Args5, width, height, appletArchive, appletCode, appletParams);
                        htmlAppletTag = sw.ToString();
                    }
                    else
                    {
                        // Doesn't exist
                        htmlAppletTag = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logfile.WriteError(ex.Message);
            }

            return htmlAppletTag;
        }

    }
}
