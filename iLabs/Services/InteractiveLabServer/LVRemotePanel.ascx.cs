namespace iLabs.LabServer.LabView
{
	using System;
	using System.Data;
	using System.Drawing;
    using System.Globalization;
    using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

    using iLabs.UtilLib;

	/// <summary>
	///		Creates an OBJECT block which contains parameters for the LabVIEW RemotePanel runtime control.
	/// </summary>
	public partial class LVRemotePanel : System.Web.UI.UserControl
	{
		public string viName = null;
		public string serverURL = null;
		public string hasControl = "true";
		public string scroll = "true";
        protected string codebase = null;
        public int border = 1;
		public int width = 50;
		public int height = 50;

        // 8.2 & 8.2.1 values
        public string version = "8.2";
        protected string classId = "CLSID:A40B0AD4-B50E-4E58-8A1D-8544233807AE";
        


        protected void Page_Load(object sender, System.EventArgs e)
        {

            // Put user code to initialize the page here
            //May want to add support for multiple versions of LabVIEW
            StringBuilder buf = new StringBuilder();
            CultureInfo culture = DateUtil.ParseCulture(Request.Headers["Accept-Language"]);

            buf.Append(@"ftp://ftp.ni.com/support/labview/runtime/windows/");
            buf.Append(version);
            switch (culture.ThreeLetterISOLanguageName)
            {
                case "fra":
                case "fre":
                    buf.Append(@"/French");
                    break;
                case "ger":
                case "deu":
                    buf.Append(@"/German");
                    break;
                case "jpn":
                    buf.Append(@"/Japanese");
                    break;
                case "chi":
                case "zho":
                    buf.Append(@"/Chinese");
                    break;
                case "kor":
                    buf.Append(@"/Korean");
                    break;
                default:
                    break;
            }
            buf.Append(@"/LVRunTimeEng.exe");
            codebase = buf.ToString();

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
