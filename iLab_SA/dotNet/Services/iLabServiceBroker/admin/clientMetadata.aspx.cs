using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.Internal;
using iLabs.ServiceBroker.iLabSB;


using iLabs.UtilLib;
using iLabs.Ticketing;

namespace iLabs.ServiceBroker.admin 
{

    public partial class ClientMetadata : System.Web.UI.Page
    {
        private Coupon authCoupon = null;
        protected BrokerDB brokerDB = new BrokerDB();
        AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
        int labClientID;
        string clientGuid;
        int groupID;
        int userTZ;
        CultureInfo culture;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["UserID"] == null)
                Response.Redirect("../login.aspx");
            //only superusers can view this page
            if (!Session["GroupName"].ToString().Equals(Group.SUPERUSER))
                Response.Redirect("../home.aspx");

            userTZ = Convert.ToInt32(Session["UserTZ"]);
            culture = DateUtil.ParseCulture(Request.Headers["Accept-Language"]);

            if (!Page.IsPostBack)
            {
                hdnMetaId.Value = "";
                initialDropDownList();
                ddlGroups.Items.Clear();
            }
        }
        protected void loadGroupsDDL(int [] groups)
        {
            ddlGroups.Items.Clear();
            ddlGroups.Items.Add( new ListItem(" -- Client Groups -- ","0"));
            foreach(int i in groups){
                string name = AdministrativeAPI.GetGroupName(i);
                ListItem item = new ListItem(name,i.ToString());
                 ddlGroups.Items.Add(item);
            }
        }
        protected void initialDropDownList()
        {
           
            ddlClient.Items.Clear();
            IntTag [] clientTags = brokerDB.GetIntTags("Client_RetrieveTags", null);

            ddlClient.Items.Add(new ListItem(" ------------- select Client ------------ ", "0"));
            if (clientTags != null)
            {
                foreach (IntTag tag in clientTags)
                {
                   ddlClient.Items.Add(new ListItem(tag.tag, tag.id.ToString()));
                }
            }
        }

        protected void ddlGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRegister.Enabled = true;
        }
      
        protected void ddlClient_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblResponse.Visible = false;
            clearScreen();
            long authCouponID = 0;
            authCoupon = null;
            labClientID = Convert.ToInt32(ddlClient.SelectedValue);
            if (labClientID > 0)
            {
                clientGuid = AdministrativeAPI.GetLabClientGUID(labClientID);
                int [] groups = AdministrativeUtilities.GetLabClientGroups(labClientID,true);
                loadGroupsDDL(groups);
                DbConnection connection = FactoryDB.GetConnection();
                DbCommand cmd = FactoryDB.CreateCommand("ClientMetadata_Retrieve", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                // populate parameters
                cmd.Parameters.Add(FactoryDB.CreateParameter("@clientID", labClientID, DbType.Int32));

                try
                {
                    connection.Open();
                    DbDataReader reader = null;
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                            hdnMetaId.Value = reader.GetInt32(0).ToString();
                        
                        //Skip Client_ID
                        if (!reader.IsDBNull(2))
                            groupID = reader.GetInt32(2);
                        else
                            groupID = 0;
                        if (!reader.IsDBNull(3))
                        {
                            authCouponID = reader.GetInt64(3);
                            
                        }
                        if (!reader.IsDBNull(4))
                            txtModTime.Text = DateUtil.ToUserTime(reader.GetDateTime(4),culture,userTZ);

                        if (!reader.IsDBNull(5))
                        {
                            txtPasscode.Text = reader.GetString(5);
                            if (txtPasscode.Text.Length > 0 && authCouponID > 0)
                            {
                                txtCouponID.Text = authCouponID.ToString();
                                txtIssuer.Text = ProcessAgentDB.ServiceGuid;
                            }
                        }
                        else
                        {
                            txtPasscode.Text = "";
                        }

                       
                        if (!reader.IsDBNull(6))
                            txtMetadata.Text = reader.GetString(6);
                       
                        if (!reader.IsDBNull(7))
                            txtScorm.Text = reader.GetString(7);
                        
                        if (!reader.IsDBNull(8))
                            txtFormat.Text = reader.GetString(8);
                       
                        if (txtCouponID.Text != null && txtCouponID.Text.Length > 0
                            && txtPasscode.Text != null && txtPasscode.Text.Length > 0)
                        {
                            authCoupon = new Coupon(ProcessAgentDB.ServiceGuid, Convert.ToInt64(txtCouponID.Text), txtPasscode.Text);
                        }
                        if(groupID > 0){
                            ddlGroups.SelectedValue = groupID.ToString();
                    
                        }
                        else{
                            ddlGroups.ClearSelection();
                        }
                    }
                    btnRegister.Enabled = groupID <= 0;
                }
                catch (DbException ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        void clearScreen()
        {
            authCoupon = null;
            ddlGroups.Items.Clear();
            hdnMetaId.Value = "";
            txtModTime.Text = "";
            txtIssuer.Text = "";
            txtCouponID.Text = "";
            txtPasscode.Text = "";
            txtMetadata.Text = "";
            txtScorm.Text = "";
            txtFormat.Text = "";
            btnRegister.Enabled = false;
        }

        protected void btnNew_Click(object sender, System.EventArgs e)
        {
            clearScreen();
            btnRegister.Enabled = true;
        }

        protected void btnRegister_Click(object sender, System.EventArgs e)
        {
            bool newCoupon = false;
            lblResponse.Visible = false;
            bool error = false;
            StringBuilder message = new StringBuilder();
            labClientID = Convert.ToInt32(ddlClient.SelectedValue);
            if (labClientID > 0)
            {
                if (ddlGroups.SelectedIndex > 0)
                {
                    clientGuid = AdministrativeAPI.GetLabClientGUID(labClientID);

                    if (authCoupon == null)
                    {
                        if (txtPasscode.Text == null || txtPasscode.Text.Length == 0)
                        {
                            authCoupon = brokerDB.CreateCoupon();
                            newCoupon = true;
                        }
                        else
                        {
                            authCoupon = brokerDB.CreateCoupon(txtPasscode.Text);
                            newCoupon = true;
                        }
                    }
                    TicketLoadFactory tlf = TicketLoadFactory.Instance();

                    if (newCoupon)
                    {
                        string payload = tlf.createAuthorizeClientPayload(clientGuid, null, ddlGroups.SelectedItem.Text, null, -1);
                        brokerDB.AddTicket(authCoupon, TicketTypes.AUTHORIZE_CLIENT, ProcessAgentDB.ServiceGuid,
                            ProcessAgentDB.ServiceGuid, -1L, payload);

                        txtCouponID.Text = authCoupon.couponId.ToString();
                        txtIssuer.Text = authCoupon.issuerGuid;
                        txtPasscode.Text = authCoupon.passkey;
                    }

                    Dictionary<string, object> keyValueDictionary = new Dictionary<string, object>();
                    keyValueDictionary.Add("authCouponId", authCoupon.couponId);
                    keyValueDictionary.Add("authIssuer", authCoupon.issuerGuid);
                    keyValueDictionary.Add("authPasskey", authCoupon.passkey);

                    int pos = ProcessAgentDB.ServiceAgent.webServiceUrl.IndexOf('/');
                    pos = ProcessAgentDB.ServiceAgent.webServiceUrl.IndexOf('/', pos);
                    pos = ProcessAgentDB.ServiceAgent.webServiceUrl.IndexOf('/', pos + 1);
                    pos = ProcessAgentDB.ServiceAgent.webServiceUrl.IndexOf('/', pos + 1);
                    string host = ProcessAgentDB.ServiceAgent.webServiceUrl.Substring(0, pos);
                    string serviceUrl = ProcessAgentDB.ServiceAgent.webServiceUrl.Substring(pos);

                    keyValueDictionary.Add("host", host);
                    keyValueDictionary.Add("webService", serviceUrl);
                    keyValueDictionary.Add("webMethod", "LanuchLabClient");
                    keyValueDictionary.Add("clientName", ddlClient.SelectedItem.Text);
                    keyValueDictionary.Add("clientGuid", clientGuid);
                    keyValueDictionary.Add("groupName", ddlGroups.SelectedItem.Text);

                    txtMetadata.Text = writeXML("iLabClientMetadata", keyValueDictionary);


                    DbConnection connection = FactoryDB.GetConnection();
                    DbCommand cmd = null;
                    if (hdnMetaId.Value.Length == 0)
                    {
                        cmd = FactoryDB.CreateCommand("ClientMetadata_Insert", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                    }
                    else
                    {
                        cmd = FactoryDB.CreateCommand("ClientMetadata_Update", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(FactoryDB.CreateParameter("@clientMetaID", Convert.ToInt32(hdnMetaId.Value), DbType.Int32));

                    }

                    // populate parameters
                    cmd.Parameters.Add(FactoryDB.CreateParameter("@clientID", labClientID, DbType.Int32));
                    int gid = 0;
                    if (ddlGroups.SelectedIndex > 0)
                        gid = Convert.ToInt32(ddlGroups.SelectedValue);
                    cmd.Parameters.Add(FactoryDB.CreateParameter("@groupID", gid, DbType.Int32));
                    cmd.Parameters.Add(FactoryDB.CreateParameter("@authCouponID", authCoupon.couponId, DbType.Int64));
                    cmd.Parameters.Add(FactoryDB.CreateParameter("@scoGuid", authCoupon.passkey, DbType.AnsiString, 50));
                    cmd.Parameters.Add(FactoryDB.CreateParameter("@metadata", txtMetadata.Text, DbType.String));
                    cmd.Parameters.Add(FactoryDB.CreateParameter("@sco", txtScorm.Text, DbType.String));
                    cmd.Parameters.Add(FactoryDB.CreateParameter("@metadataFormat", txtFormat.Text, DbType.String, 256));
                    try
                    {
                        connection.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            int value = Convert.ToInt32(result);
                            if (hdnMetaId.Value.Length > 0)
                            { //was update
                                message.Append("Update: " + value + " records");
                            }
                            else
                            {
                                message.Append("Inserted new metadata: ID = " + value);
                                hdnMetaId.Value = value.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Error in metadata register: " + ex.Message);
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                    if (error)
                    {
                        lblResponse.Text = Utilities.FormatErrorMessage(message.ToString());
                    }
                    else
                    {
                        lblResponse.Text = Utilities.FormatConfirmationMessage(message.ToString());
                    }
                    lblResponse.Visible = true;
                }
                else
                {
                    lblResponse.Text = Utilities.FormatWarningMessage("You must select a group.");
                    lblResponse.Visible = true;
                }
            }
            else
            {
                lblResponse.Text = Utilities.FormatWarningMessage("You must select a client.");
                lblResponse.Visible = true;
            }
        }

        protected void btnGuid_Click(object sender, System.EventArgs e)
        {
            Guid guid = System.Guid.NewGuid();
            txtPasscode.Text = Utilities.MakeGuid("N");
            txtCouponID.Text = "";
            btnRegister.Enabled = true;
        }

        protected void checkGuid(object sender, ServerValidateEventArgs args)
        {
            if (args.Value.Length > 0 && args.Value.Length <= 50)
                args.IsValid = true;
            else
                args.IsValid = false;
        }

        public string writeXML(string rootElement, Dictionary<string, object> keyValueDictionary)
        {
            try
            {
                StringWriter stringWriter = new StringWriter();
                XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
                //xmlWriter.Formatting = Formatting.Indented;
                //xmlWriter.Indentation = indentation;

                // write root element
                xmlWriter.WriteStartElement(rootElement);
                xmlWriter.WriteAttributeString("xmlns", "ns", null, "http://ilab.mit.edu/iLab");

               
                foreach (string s in keyValueDictionary.Keys)
                {
                    xmlWriter.WriteStartElement(s);
                    object value = new object();
                    keyValueDictionary.TryGetValue(s, out value);
                    xmlWriter.WriteString(value.ToString());
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                return stringWriter.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return null;
        }

    }
    

}
