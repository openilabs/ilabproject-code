using System;

namespace LabClientHtml.Controls
{
    public partial class Status : System.Web.UI.UserControl
    {
        public string Version
        {
            get
            {
                return lblVersion.Text;
            }
            set
            {
                lblVersion.Text = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}