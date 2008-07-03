
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

namespace iLabs.Scheduling.LabSide
{
	/// <summary>
	///		Summary description for NavBar.
	/// </summary>
	public partial class NavBar : System.Web.UI.UserControl
	{
        protected string currentPage;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            // Get the current page name w/o path name or slashes
            currentPage = Request.Url.Segments[Request.Url.Segments.Length - 1];
			// Put user code to initialize the page here
            switch (currentPage)
            {
                case "Administer.aspx":
                    HLRegisterUSS.ForeColor = Color.White;
                    break;
                case "Manage.aspx":
                    HLexpInfoManagement.ForeColor = Color.White;
                    break;
                case "RegisterGroup.aspx":
                    HLRegisterGroup.ForeColor = Color.White;
                    break;
                case "ReservationInfo.aspx":
                    HLReservationInfo.ForeColor = Color.White;
                    break;
                case "RevokeReservation.aspx":
                    HLRevokeReservation.ForeColor = Color.White;
                    break;
                case "TimeBlockManagement.aspx":
                    HLTimeBlockManage.ForeColor = Color.White;
                    break;
                
            }
            if(Session["sbUrl"] != null)
                HLBackToSB.NavigateUrl = Session["sbUrl"].ToString();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
