using System;
using System.Collections;
using System.Configuration;
//using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

//using iLabs.Architecture.AdministrativeAPI;
//using iLabs.Architecture.AuthenticationAPI;
//using iLabs.Architecture.InternalAPI;

namespace LabServer.Controls
{
    public partial class Login : System.Web.UI.UserControl
    {
        //AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
        //string supportMailAddress = ConfigurationSettings.AppSettings["supportMailAddress"];

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogIn_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Equals("") || txtPassword.Text.Equals(""))
            {
                lblLoginErrorMessage.Text = "<div class=errormessage><p>Missing user ID and/or password field. </p></div>";
                lblLoginErrorMessage.Visible = true;
                return;
            }

            //int userID = -1;
            //userID = wrapper.GetUserIDWrapper(txtUsername.Text);

            //if (userID > 0)
            //{
            //    User user = wrapper.GetUsersWrapper(new int[] { userID })[0];

            //    if (userID != -1 && user.lockAccount == true)
            //    {
            //        lblLoginErrorMessage.Text = "<div class=errormessage><p>Account locked - Email " + supportMailAddress + ". </p></div>";
            //        lblLoginErrorMessage.Visible = true;
            //        return;
            //    }

            //    if (CheckCredentials(userID, txtPassword.Text))
            //    {
            //        FormsAuthentication.SetAuthCookie(txtUsername.Text, false);
            //        Session["UserID"] = userID;
            //        Session["UserName"] = user.userName;
            //        string sessionID = AdministrativeAPI.InsertUserSession(userID, 0, Session.LCID.ToString()).ToString();
            //        Session["SessionID"] = sessionID;

            //        Response.Redirect("MyGroups.aspx");
            //        //Response.Redirect(Global.FormatRegularURL(Request, "MyGroups.aspx"));
            //    }
            //    else
            //    {
            //        lblLoginErrorMessage.Text = "<div class=errormessage><p>Invalid user ID and/or password. </p></div>";
            //        lblLoginErrorMessage.Visible = true;
            //    }
            //}
            //else
            //{
            //    lblLoginErrorMessage.Text = "<div class=errormessage><p>Username does not exist. </p></div>";
            //    lblLoginErrorMessage.Visible = true;
            //}
        }

        /// <summary>
        /// Authenticates a user against information in the database.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool CheckCredentials(int userID, string password)
        {
            //if (AuthenticationAPI.Authenticate(userID, password))
            //    return true;
            //else
            return false;
        }
    }
}