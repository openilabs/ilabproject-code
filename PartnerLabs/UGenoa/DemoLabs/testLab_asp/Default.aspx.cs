using System;
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
using ReservationService;
using LabProxyService;

public partial class _Default : System.Web.UI.Page
{


    protected void Page_Load(object sender, EventArgs e)
    {
        // call Webservice on labproxz to retrieve reservation information
        LabProxyClient.RetrieveReservation("http://remlab-esng.dibe.unige.it/LPS_MIT/LabProxy.asmx");

        // get URL of service Broker to create link back
        HyperLinkSB.NavigateUrl = HttpContext.Current.Session["sbUrl"].ToString();

        // get reservation details
        ReservationExpirationLbl.Text = LabProxyClient.ReservationExpiration;
        ExperimentIdLbl.Text = LabProxyClient.ExperimentID;
        GroupIdLbl.Text = LabProxyClient.GroupName;

        // check whether the current seesion is still in time or is alreadz expired (return: true/false)
        // this method was designed to check the access to the proper laboratory
        isvalid.Text = LabProxyClient.isResgistrationValid().ToString();

    }
}
