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

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        OpenPopUp(NewLabConfiguration, "PopupLabSetup.aspx", "New Lab Configuration", 950, 1200);

    }
    protected void NewLabConfiguration_Click(object sender, EventArgs e)
    {
      //  OpenPopUp(NewLabConfiguration,"PopupLabSetup.aspx", "New Lab Configuration", 950, 1200);
    }

    //opening the instant schedule configuration window
    protected static void OpenPopUp(System.Web.UI.WebControls.WebControl opener, string PagePath, string windowName, int width, int height)
    {
        string clientScript;
        string windowAttribs;
            
        //Building Client side window attributes with width and height.//
        //Also the the window will be positioned to the middle of the screen//
        windowAttribs = "width=" + width + "px," + "height=" + height + "px," + "scrollbars=1," + "left=\'+((screen.width -" + width + ") / 2)+\'," + "top=\'+ (screen.height - " + height + ")/ 2+\'";


        //***Building the client script- window.open, with additional parameters***///
        clientScript = "window.open(\'" + PagePath + "\',\'" + windowName + "\',\'" + windowAttribs + "\');return false;";
        //regiter the script to the clientside click event of the 'opener' control*****///
        opener.Attributes.Add("onClick", clientScript);
    }


    protected void drop1_SelectedIndexChanged(object sender, EventArgs e)
    {

    }


}
