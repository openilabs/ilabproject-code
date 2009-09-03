using System;
using System.Xml;
using Library.Lab;
using Library.LabClient;

namespace LabClientHtml
{
    public partial class LabClient : System.Web.UI.Page
    {
        #region Class Constants and Variables

        private const string STRLOG_ClassName = "LabClient";

        //
        // String constants
        //
        private const string STR_Less = "&#171; Less";
        private const string STR_More = "More &#187;";
        private const string STR_ForMoreInfoSee = "For information specific to this LabClient, see";

        #endregion

        //---------------------------------------------------------------------------------------//

        protected void Page_Load(object sender, EventArgs e)
        {
            const string STRLOG_MethodName = "Page_Load";

            Master.HeaderTitle = this.Title;
            this.Title = Master.PageTitle + this.Title;

            if (!IsPostBack)
            {
                Logfile.WriteCalled(STRLOG_ClassName, STRLOG_MethodName);

                //
                // Update lab info text and Url if specified
                //
                string labinfoText = Master.LabinfoText;
                string labinfoUrl = Master.LabinfoUrl;
                if (labinfoText != null && labinfoText.Length > 0 &&
                    labinfoUrl != null && labinfoUrl.Length > 0)
                {
                    //
                    // Update hyperlink on webpage
                    //
                    lblMoreInfo.Text = STR_ForMoreInfoSee;
                    lnkMoreInfo.Text = labinfoText;
                    lnkMoreInfo.NavigateUrl = labinfoUrl;
                }

                //
                // Don't display the extra inforamtion
                //
                litSetupInfo.Visible = false;
                lnkbtnSetupInfo.Text = STR_More;
                litStatusInfo.Visible = false;
                lnkbtnStatusInfo.Text = STR_More;
                litResultsInfo.Visible = false;
                lnkbtnResultsInfo.Text = STR_More;

                Logfile.WriteCompleted(STRLOG_ClassName, STRLOG_MethodName);
            }
            else
            {
                // Clear labels
                lblMoreInfo.Text = null;
                lnkMoreInfo.Text = null;
            }
        }

        //---------------------------------------------------------------------------------------//

        protected void lnkbtnSetupInfo_Click(object sender, EventArgs e)
        {
            if (litSetupInfo.Visible == false)
            {
                litSetupInfo.Visible = true;
                lnkbtnSetupInfo.Text = STR_Less;
            }
            else
            {
                litSetupInfo.Visible = false;
                lnkbtnSetupInfo.Text = STR_More;
            }
        }

        //---------------------------------------------------------------------------------------//

        protected void lnkbtnStatusInfo_Click(object sender, EventArgs e)
        {
            if (litStatusInfo.Visible == false)
            {
                litStatusInfo.Visible = true;
                lnkbtnStatusInfo.Text = STR_Less;
            }
            else
            {
                litStatusInfo.Visible = false;
                lnkbtnStatusInfo.Text = STR_More;
            }
        }

        //---------------------------------------------------------------------------------------//

        protected void lnkbtnResultsInfo_Click(object sender, EventArgs e)
        {
            if (litResultsInfo.Visible == false)
            {
                litResultsInfo.Visible = true;
                lnkbtnResultsInfo.Text = STR_Less;
            }
            else
            {
                litResultsInfo.Visible = false;
                lnkbtnResultsInfo.Text = STR_More;
            }
        }

    }
}
