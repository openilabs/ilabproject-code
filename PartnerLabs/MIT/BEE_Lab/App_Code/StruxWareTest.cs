using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using iLabs.Proxies.StruxWare;

namespace iLabs.struxware
{

    /// <summary>
    /// Summary description for StruxWareTest
    /// </summary>
    public class StruxWareTest
    {
        //
        // TODO: Add constructor logic here
        //

        private void testStruxWare()
        {
            DataExchangeService struxProxy = new DataExchangeService();
            GetWebServiceInformationRequest req = null;
            GetWebServiceInformationResponse wsRespon = struxProxy.GetWebServiceInformation(req);
            string[] operations = wsRespon.GetWebServiceInformationSupportedOperations;
        }
    }

}
