/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id$
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

using iLabs.DataTypes;
using iLabs.ServiceBroker.Internal;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authentication;
using iLabs.ServiceBroker.Authorization;
using iLabs.UtilLib;

namespace iLabs.ServiceBroker.admin
{
	/// <summary>
	/// Summary description for loginRecords.
	/// </summary>
    public partial class sessionHistory : System.Web.UI.Page
    {
        AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
        CultureInfo culture;
        int userTZ;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            culture = DateUtil.ParseCulture(Request.Headers["Accept-Language"]);
            if (Session["UserID"] == null)
                Response.Redirect("../login.aspx");
            if (Session["userTZ"] != null)
            {
                userTZ = Convert.ToInt32(Session["userTZ"]);
            }
            lblTimezone.Text = "Times are GMT " + userTZ / 60.0;
            if (ddlTimeIs.SelectedIndex != 4)
            {
                txtTime2.ReadOnly = true;
                txtTime2.BackColor = Color.Lavender;
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        private void LoadAuthorityList()
        {
            ddlAuthority.Items.Clear();
            //ListItem liHeaderAdminGroup = new ListItem("--- All Authorities ---", "-1");
            //ddlAuthorities.Items.Add(liHeaderAdminGroup);
            BrokerDB brokerDB = new BrokerDB();
            IntTag[] authTags = brokerDB.GetAuthorityTags();
            if (authTags != null && authTags.Length > 0)
            {
                foreach (IntTag t in authTags)
                {
                    ListItem li = new ListItem(t.tag, t.id.ToString());
                    ddlAuthority.Items.Add(li);
                }
            }
        }

        protected void ddlTimeIs_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            txtTime1.Text = null;
            txtTime2.Text = null;
            if (ddlTimeIs.SelectedIndex == 4)
            {
                txtTime2.ReadOnly = false;
                txtTime2.BackColor = Color.White;
            }
        }
        //list the log-in sessions according to the selected criterion
        private void BuildListBox(int userID, int groupID, DateTime time1, DateTime time2, bool cvs)
        {
            string pad = ",\t";
            try
            {
                DataSet sessionData = InternalAdminDB.SelectSessionHistory(userID, groupID, time1, time2);
                DataTable sessionHistory = sessionData.Tables[0];

                if (sessionHistory.Rows.Count == 0)
                {
                    lblResponse.Text = Utilities.FormatErrorMessage("No sessions found.");
                    lblResponse.Visible = true;
                }
                else
                {
                    StringBuilder buf = new StringBuilder();
                    DataTableReader dtr = new DataTableReader(sessionHistory);
                    while(dtr.Read())
                    {
                        buf.Append(dtr.GetInt64(0) + pad);
                        buf.Append(dtr.GetString(1) + pad);
                        if(!dtr.IsDBNull(2))
                            buf.Append(DateUtil.ToUserTime(dtr.GetDateTime(2),culture,userTZ) +pad);
                        else
                            buf.Append("Create Time Not Set" +pad);
                        if (!dtr.IsDBNull(3))
                            buf.Append(DateUtil.ToUserTime(dtr.GetDateTime(3), culture, userTZ) + pad);
                        else
                            buf.Append("End Time Not Set" +pad);
                        if (!dtr.IsDBNull(4))
                            buf.Append(DateUtil.ToUserTime(dtr.GetDateTime(4),culture,userTZ) +pad);
                        else
                            buf.Append("Modify Time Not Set" +pad);
                        buf.Append(dtr.GetString(5) + pad);
                        buf.Append(dtr.GetString(6));
                        
                        buf.AppendLine();
                    }
                    txtLoginDisplay.Text = buf.ToString();
                }
            }
            catch (Exception ex)
            {
                lblResponse.Text = "<div class=errormessage><p>Cannot retrieve UserSessions. " + ex.GetBaseException() + "</p></div>";
                lblResponse.Visible = true;
            }
        }
        protected void btnGo_Click(object sender, System.EventArgs e)
        {
            int userID = -1;
            int groupID = -1;
            int authID = 0;
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MaxValue;
            txtLoginDisplay.Text = null;
            if (!(txtGroupName.Text == "" && txtUserName.Text == "" && txtTime1.Text == "" && txtTime2.Text == ""))
            {
                if (txtUserName.Text != "")
                {
                    authID = Convert.ToInt32(ddlAuthority.SelectedValue);
                    userID = wrapper.GetUserIDWrapper(txtUserName.Text,authID);
                    if (userID == -1)
                    {
                        lblResponse.Text = "<div class=errormessage><p>no user with the username '" + txtUserName.Text + "'found</p></div>";
                        lblResponse.Visible = true;
                        return;
                    }
                }
                if (txtGroupName.Text != "")
                {
                    groupID = wrapper.GetGroupIDWrapper(txtGroupName.Text);
                    if (groupID == -1)
                    {
                        lblResponse.Text = Utilities.FormatErrorMessage("Group '" + txtGroupName.Text + "' not found.");
                        lblResponse.Visible = true;
                        return;
                    }
                }
                if (ddlTimeIs.SelectedIndex > 0)
                {
                    DateTime time1;
                    try
                    {
                        
                        time1 = DateUtil.ParseUserToUtc(txtTime1.Text.Trim(), culture, userTZ);
                    }
                    catch
                    {
                        lblResponse.Text = "<div class=errormessage><p>Please enter a valid time</p></div>";
                        lblResponse.Visible = true;
                        return;
                    }
                    if (ddlTimeIs.SelectedIndex == 1)
                    {
                        startTime = time1;
                        endTime = time1;

                    }
                    else if (ddlTimeIs.SelectedIndex == 2)
                    {
                        endTime = time1;

                    }
                    else if (ddlTimeIs.SelectedIndex == 3)
                    {
                        startTime = time1;

                    }
                    else if (ddlTimeIs.SelectedIndex == 4)
                    {
                        DateTime time2;
                        try
                        {
                            time2 = DateUtil.ParseUserToUtc(txtTime2.Text.Trim(), culture, userTZ);
                        }
                        catch
                        {
                            lblResponse.Text = "<div class=errormessage><p>Please enter a valid time</p></div>";
                            lblResponse.Visible = true;
                            return;
                        }
                        if (time2 < time1)
                        {
                            lblResponse.Text = "<div class=errormessage><p>The second time must be greater than or equal to the start time</p></div>";
                            lblResponse.Visible = true;
                            return;
                        }
                        startTime = time1;
                        endTime = time2;

                    }
                }
            }
            BuildListBox(userID, groupID, startTime, endTime,false);
        }

    }
}

