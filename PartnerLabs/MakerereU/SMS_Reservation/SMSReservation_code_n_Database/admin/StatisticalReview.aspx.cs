using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;

public partial class StatisticalReview : System.Web.UI.Page
{
   // string messageKey = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        

        if (!IsPostBack)
        {
            DataClassesDataContext db = new DataClassesDataContext();
            var incoming = db.InComingMessages.Select(imsg => imsg.username).ToList();
            //List<string> outgoingMessageuserNames = db.OutGoingMessages.Select(omsg => omsg.username).ToList();
            //var outgoing = db.OutGoingMessages.Select(omsg => omsg.MessageKey).ToList();
            List<string> usernames = new List<string>();

            usernames.AddRange(incoming);
            //List<string> MessageKey = new List<string>();
            //MessageKey.AddRange(outgoing);
            lstUsers.DataSource = usernames.Distinct();
            //lstkeys.DataSource = MessageKey.Distinct();

            lstUsers.DataBind();
        }

    }


    protected void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataClassesDataContext db = new DataClassesDataContext();

        InComingMessage currentMessage  = db.InComingMessages.Single(u => u.username.Contains( lstUsers.SelectedItem.Text));
        txtUserName.Text = currentMessage.username;
        txtLabName.Text = currentMessage.LabConfiguration.LabName;
        txtDate.Text = currentMessage.TimeReceived.ToString();
        txtStartTime.Text = currentMessage.StartTimeRange.ToString();
        txtEndTime.Text = currentMessage.EndTimeRange.ToString();
        txtMsgkey.Text = currentMessage.MessageKey.ToString();


        //messageKey = currentMessage.MessageKey;

        OutGoingMessage outgoingMessage = db.OutGoingMessages.Single(u => u.MessageKey.Equals(currentMessage.MessageKey));
        txtGivenStartTime.Text = outgoingMessage.GivenStartTime.ToString();
        txtGiveEndTime.Text = outgoingMessage.GivenEndTime.ToString();
        txtTimeAndDate.Text = outgoingMessage.TimeAndDateSent.ToString();
        //txtEndTime.Text = currentMessage.EndTimeRange;
        //txtMsgkey.Text = outgoingMessage.MessageKey;
    }
}
