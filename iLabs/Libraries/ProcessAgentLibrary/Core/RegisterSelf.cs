/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: SchedulingControl.cs,v 1.5 2007/02/16 22:50:36 pbailey Exp $
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using iLabs.Core;
using iLabs.DataTypes;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Ticketing;
using iLabs.UtilLib;
using iLabs.Proxies.PAgent;

namespace iLabs.Core
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:RegisterSelf runat=server></{0}:RegisterSelf>")]

    
    public class RegisterSelf : CompositeControl, IPostBackEventHandler
    {
        #region properties

        
        private Color disabled = Color.FromArgb(243, 239, 229);
        private Color enabled = Color.White;
       
        private string agentType = ProcessAgentType.LAB_SERVER;
        public string AgentType
        {
            get
            {
                object o = ViewState["AgentType"];
                if (o == null)
                    return String.Empty;
                return (string)o;
            }
            set { ViewState["AgentType"] = value; }
        }

        public string Title
        {
            get
            {
                object o = ViewState["Title"];
                if (o == null)
                    return String.Empty;
                return (string)o;
            }
            set { ViewState["Title"] = value; }
        }
        public string Description
        {
            get
            {
                object o = ViewState["Description"];
                if (o == null)
                    return String.Empty;
                return (string)o;
            }
            set { ViewState["Description"] = value; }
        }
        public string Response
        {
            get
            {
                object o = ViewState["Response"];
                if (o == null)
                    return String.Empty;
                return (string)o;
            }
            set { ViewState["Response"] = value; }
        }
        public string guidMessage
        {
            get
            {
                object o = ViewState["guidMessage"];
                if (o == null)
                    return String.Empty;
                return (string)o;
            }
            set { ViewState["guidMessage"] = value; }
        }
        public string modifyMessage
        {
            get
            {
                object o = ViewState["modifyMessage"];
                if (o == null)
                    return String.Empty;
                return (string)o;
            }
            set { ViewState["modifyMessage"] = value; }
        }

        public int maxGuidLength
        {
            get
            {
                object o = ViewState["maxGuidLength"];
                if (o == null)
                    return 50;
                return (int)o;
            }
            set { ViewState["maxGuidLength"] = value; }
        }

        
        //maxGuidLength = 50;
        //private string guidMessage = "The Guid must be globally unique and have no more than 50 characters";
        private string nameScript;
        private StringBuilder code;
        //private string modifyMessage = "\\nThis will require changes for all iLab Services!\\nThe changes will not take effect until you click the Modify button";
        private string resetScript;
        private bool secure = false;
        

        ProcessAgentDB dbTicketing = new ProcessAgentDB();

       
        #endregion

        #region UI Controls

        Unit onePX = new Unit(1);
        Unit width = new Unit(620);
        Unit labelWidth = new Unit(150);
        Unit txtBoxHeight = new Unit(20);
        Unit dataWidth = new Unit(496);
        Unit guidWidth = new Unit(380);
        Unit buttonColumnWidth = new Unit(60);
        HtmlGenericControl pageintro;      
        
        HtmlGenericControl pagecontent;
        HtmlGenericControl simpleform;
        HtmlForm frmRegister;
        Table tblMain;
        TableRow trRowPasskey;
        TableRow trDomainSB;
  
        Label lblTitle;
        Label lblResponse;
        Label lblDescription;
        Label lblServiceType;
        Label lblServiceName;
        Label lblCodebaseUrl;
        Label lblServiceUrl;
        Label lblServiceGuid;
        Label lblDomainServer;
        Label lblOutPassKey;
        Label lblContactInfo;
        TextBox txtServiceName;
        TextBox txtCodebaseUrl;
        TextBox txtServiceUrl;
        TextBox txtServiceGuid;
        TextBox txtDomainServer;
        TextBox txtOutPasskey;
        TextBox txtContactInfo;
        Button btnGuid;
        Button btnModifyService;
        Button btnRefresh;
        Button btnClear;
        Button btnRetire;
        Button btnSave;
        HiddenField hdnServiceName;
        HiddenField hdnCodebaseUrl;
        HiddenField hdnServiceUrl;
        HiddenField hdnServiceGuid;
        #endregion

        #region events

        /// <summary>
        /// Event called when the user clicks an availible time block in the calendar. It's only called when DoPostBack is true.
        /// </summary>
        public event ModifySelfDelegate SaveSelf ;//= new ModifySelfDelegate(saveChanges);

        /// <summary>
        /// Event called when the user clicks a reservation in the calendar. It's only called when DoPostBack is true.
        /// </summary>
        public event ModifySelfDelegate ModifySelf;// = new ModifySelfDelegate(modifyService);

        /// <summary>
        /// Event called when the user clicks an availible time block in the calendar. It's only called when DoPostBack is true.
        /// </summary>
        public event RefreshSelfDelegate RetireSelf; // = new RefreshSelfDelegate(btnRefresh_Click);

        /*
        /// <summary>
        /// Event called when the user clicks an availible time block in the calendar. It's only called when DoPostBack is true.
        /// </summary>
        public event RefreshSelfDelegate RefreshSelf =  new RefreshSelfDelegate(this.btnRefresh_Click);

        /// <summary>
        /// Event called when the user clicks an availible time block in the calendar. It's only called when DoPostBack is true.
        /// </summary>
        public event RefreshSelfDelegate ClearSelf;
       */

       

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool handled = false;

            CommandEventArgs ce = args as CommandEventArgs;
            if (ce != null && ce.CommandName != null)
            {
                switch (ce.CommandName)
                {
                    case "Save":
                        ModifySelfEventArgs ss = new ModifySelfEventArgs();
                        ss.codebaseUrl = txtCodebaseUrl.Text;
                        ss.domainGuid = txtDomainServer.Text;
                        ss.originalGuid = hdnServiceGuid.Value;
                        ss.serviceGuid = txtServiceGuid.Text;
                        ss.serviceName = txtServiceName.Text;
                        ss.serviceUrl = txtServiceUrl.Text;
                        if (SaveSelf != null)
                        {
                            SaveSelf(this, ss);
                            handled = true;
                        }
                        break;
                    case "Modify":
                        ModifySelfEventArgs ms = new ModifySelfEventArgs();
                        ms.codebaseUrl = txtCodebaseUrl.Text;
                        ms.domainGuid = txtDomainServer.Text;
                        ms.originalGuid = hdnServiceGuid.Value;
                        ms.serviceGuid = txtServiceGuid.Text;
                        ms.serviceName = txtServiceName.Text;
                        ms.serviceUrl = txtServiceUrl.Text;
                        if (ModifySelf != null)
                        {
                            ModifySelf(this, ms);
                            handled = true;
                        }
                        break;
                    case "Clear":
                        btnNew_Click(this,args);
                        handled = true;
                        
                        break;
                    case "Refresh":
                        btnRefresh_Click(this,args);
                        handled = true;
                        
                        break;
                    case "Retire":
                        RefreshSelfEventArgs ts = new RefreshSelfEventArgs();
                        ts.domainGuid = txtDomainServer.Text;
                        ts.originalGuid = hdnServiceGuid.Value;
                        if (RetireSelf != null)
                        {
                            RetireSelf(this, ts);
                            handled = true;
                        }
                        break;
                    default:
                        break;
                }
               
            }

            return handled;
        }
        
        
   
        #region Events
/*
        private static readonly object ValidateCreditCardEventKey = new object();
        public event ValidateCreditCardEventHandler ValidateCreditCard
        {
            add { Events.AddHandler(ValidateCreditCardEventKey, value); }
            remove { Events.RemoveHandler(ValidateCreditCardEventKey, value); }
        }

        protected virtual void OnValidateCreditCard(ValidateCreditCardEventArgs e)
        {
            ValidateCreditCardEventHandler handler = Events[ValidateCreditCardEventKey] as ValidateCreditCardEventHandler;

            if (handler != null)
                handler(this, e);
        }

        protected virtual PaymentMethod GetPaymentMethod()
        {
            return (paymentMethodList.SelectedValue == "Visa") ? PaymentMethod.Visa : PaymentMethod.MasterCard;
        }

        protected virtual string GetCreditCardNo()
        {
            return creditCardNoTextBox.Text;
        }

        protected virtual string GetCardholderName()
        {
            return cardholderNameTextBox.Text;
        }

        protected virtual DateTime GetExpirationDate()
        {
            return new DateTime(int.Parse(yearList.SelectedValue), int.Parse(monthList.SelectedValue), 25);
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool handled = false;

            CommandEventArgs ce = args as CommandEventArgs;
            if (ce != null && ce.CommandName == "ValidateCreditCard")
            {
                PaymentMethod paymentMethod = GetPaymentMethod();
                string creditCardNo = GetCreditCardNo();
                string cardholderName = GetCardholderName();
                DateTime expirationDate = GetExpirationDate();
                ValidateCreditCardEventArgs ve = new ValidateCreditCardEventArgs(paymentMethod, creditCardNo, cardholderName, expirationDate);
                OnValidateCreditCard(ve);
                handled = true;
            }

            return handled;
        }
        */
        #endregion


        #endregion

        #region pageCode
/*
        protected void Page_Load(object sender, System.EventArgs e)
        {

            string couponId;
            string passkey;
            string issuerGuid;

            if (!IsPostBack)
            {

                // try & test for local access or not configured
                if (secure)
                {
                    // retrieve parameters from URL
                    couponId = Request.QueryString["coupon_id"];
                    passkey = Request.QueryString["passkey"];
                    issuerGuid = Request.QueryString["issuer_guid"];
                    Ticket allowAdminTicket = null;
                    if (couponId != null && passkey != null && issuerGuid != null)
                    {
                        allowAdminTicket = dbTicketing.RetrieveAndVerify(
                            new Coupon(issuerGuid, Int64.Parse(couponId), passkey),
                            TicketTypes.ADMINISTER_LS);
                    }
                    else
                    {
                        Response.Redirect("AccessDenied.aspx", true);
                    }
                }

                string returnUrl = Request.QueryString["returnURL"];
                if (returnUrl != null && returnUrl.Length > 0)
                    Session["returnURL"] = returnUrl;
                txtDomainServer.ReadOnly = true;
                txtDomainServer.BackColor = disabled;

                code = new StringBuilder();
                code.AppendLine("<script>");
                code.AppendLine("function EnableServiceName() {");
                code.AppendLine("var msg= 'Are you sure you want to modify the Web Service Name?" + modifyMessage + "';");
                code.AppendLine("if (confirm(msg)) document.getElementById('selfReg_btnModifyService').disabled = false;");
                code.AppendLine("else{ document.getElementById('selfReg_txtServiceName').value = document.getElementById('selfReg_bakServiceName').value;}}</script>");
                code.AppendLine();
                code.AppendLine("<script>function EnableCodeBase() {");
                code.AppendLine("var msg= 'Are you sure you want to modify the CodeBase for this service?" + modifyMessage + "';");
                code.AppendLine("if (confirm(msg))document.getElementById('selfReg_btnModifyService').disabled = false;");
                code.AppendLine("else{document.getElementById('selfReg_txtCodebaseUrl').value = document.getElementById('selfReg_bakCodebase').value;}}");
                code.AppendLine();
                code.AppendLine("function EnableServiceUrl() {");
                code.AppendLine("var msg= 'Are you sure you want to modify the Web Service URL?" + modifyMessage + "';");
                code.AppendLine("if (confirm(msg))document.getElementById('selfReg_btnModifyService').disabled = false;");
                code.AppendLine("else{document.getElementById('selfReg_txtWebServiceUrl').value = document.getElementById('selfReg_bakServiceUrl').value;}}");
                code.AppendLine();
                code.AppendLine("function ConfirmRetire() {");
                code.AppendLine("var msg= 'Are you sure you want to retire this WebService.\\nIf you proceed all references to this site will be retired';");
                code.AppendLine("var state = confirm(msg);return state;}");
                code.AppendLine("</script>");
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OnNameChange", code.ToString());
                DisplayForm();
            }
            String returnURL = (string)Session["returnURL"];
            if ((returnURL != null) && (returnURL.Length > 0))
            {
                lnkBackSB.NavigateUrl = returnURL;
                lnkBackSB.Visible = true;
            }
            else
            {
                lnkBackSB.Visible = false;
            }
            code = new StringBuilder();
            code.AppendLine("<script>");
            code.AppendLine("function EnableServiceName() {");
            code.AppendLine("var msg= 'Are you sure you want to modify the Web Service Name?\\nThis will require changes for all iLab Services!';");
            code.AppendLine("if (confirm(msg)) document.getElementById('selfReg_btnModifyService').disabled = false;");
            code.AppendLine("else{ document.getElementById('selfReg_txtServiceName').value = document.getElementById('selfReg_bakServiceName').value;}}</script>");
            code.AppendLine();
            code.AppendLine("<script>function EnableCodeBase() {");
            code.AppendLine("var msg= 'Are you sure you want to modify the CodeBase for this service?\\nThis will require changes for all iLab Services!';");
            code.AppendLine("if (confirm(msg))document.getElementById('selfReg_btnModifyService').disabled = false;");
            code.AppendLine("else{document.getElementById('selfReg_txtCodebaseUrl').value = document.getElementById('selfReg_bakCodebase').value;}}");
            code.AppendLine();
            code.AppendLine("function EnableServiceUrl() {");
            code.AppendLine("var msg= 'Are you sure you want to modify the Web Service URL?\\nThis will require changes for all iLab Services!';");
            code.AppendLine("if (confirm(msg))document.getElementById('selfReg_btnModifyService').disabled = false;");
            code.AppendLine("else{document.getElementById('selfReg_txtWebServiceUrl').value = document.getElementById('selfReg_bakServiceUrl').value;}}");
            code.AppendLine();
            code.AppendLine("function ConfirmRetire() {");
            code.AppendLine("var msg= 'Are you sure you want to retire this WebService.\\nIf you proceed all references to this site will be retired';");
            code.AppendLine("var state = confirm(msg);return state;}");
            code.AppendLine("</script>");
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OnNameChange", code.ToString());

            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"OnCodebaseChange",code.ToString());
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType,"OnServiceChange",serviceScript);
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType,"OnRetire",retireScript);

            //btnRetire.Attributes.Add("onClick", "return confirm('Are you sure you want to retire this WebService. If you proceed all references to this site will be retired');");


        }
*/
        #region Data binding

/*
        protected override void PerformSelect()
        {
            // Call OnDataBinding here if bound to a data source using the
            // DataSource property (instead of a DataSourceID), because the
            // databinding statement is evaluated before the call to GetData.       
            if (!IsBoundUsingDataSourceID)
            {
                this.OnDataBinding(EventArgs.Empty);
            }

            // The GetData method retrieves the DataSourceView object from  
            // the IDataSource associated with the data-bound control.            
            GetData().Select(CreateDataSourceSelectArguments(),
                this.OnDataSourceViewSelectCallback);

            // The PerformDataBinding method has completed.
            RequiresDataBinding = false;
            MarkAsDataBound();

            // Raise the DataBound event.
            OnDataBound(EventArgs.Empty);
        }

        private void OnDataSourceViewSelectCallback(IEnumerable retrievedData)
        {
            // Call OnDataBinding only if it has not already been 
            // called in the PerformSelect method.
            if (IsBoundUsingDataSourceID)
            {
                OnDataBinding(EventArgs.Empty);
            }
            // The PerformDataBinding method binds the data in the  
            // retrievedData collection to elements of the data-bound control.
            PerformDataBinding(retrievedData);
        }

        protected override void PerformDataBinding(IEnumerable schedulingData)
        {
            // don't bind in design mode
            if (DesignMode)
            {
                return;
            }

            base.PerformDataBinding(schedulingData);

            DisplayForm();
        }

*/
        #endregion

        protected void DisplayForm()
        {
            lblResponse.Visible = false;
            StringBuilder message = new StringBuilder();
            ProcessAgent pai = ProcessAgentDB.ServiceAgent;
            if (pai != null)
            {
                lblServiceType.Text = pai.type;
                txtServiceName.Text = pai.agentName;
                hdnServiceName.Value = pai.agentName;
                txtServiceGuid.Text = pai.agentGuid;

                txtCodebaseUrl.Text = pai.codeBaseUrl;
                hdnCodebaseUrl.Value = pai.codeBaseUrl;
                txtServiceUrl.Text = pai.webServiceUrl;
                hdnServiceUrl.Value = pai.webServiceUrl;


                if (agentType.Equals(ProcessAgentType.SERVICE_BROKER))
                {
                    trDomainSB.Visible = false;
                    IntTag[] pas = dbTicketing.GetProcessAgentTags();
                    // If additional processAgents are registered the Fields may not be modified
                    SetFormMode(pas != null && pas.Length > 1);
                }
                else
                {
                    // Check to see if a ServiceBroker is registered
                    trDomainSB.Visible = true;
                    ProcessAgentInfo sbInfo = dbTicketing.GetServiceBrokerInfo();
                    if (sbInfo != null)
                    {

                        txtDomainServer.Text = sbInfo.ServiceUrl;

                        // May not modify any fields that define the service since
                        // It is registered with its domainServiceBroker

                        SetFormMode(true);

                    }
                    else
                    {
                        // May modify fields that define the service since
                        // It is not registered registered with a domainServiceBroker
                        message.Append("A domain ServiceBroker has not been registered!");
                        lblResponse.Text = Utilities.FormatWarningMessage(message.ToString());
                        lblResponse.Visible = true;
                        Utilities.WriteLog("administration: DomainServerNotFound");
                        SetFormMode(false);
                    }
                }

            }
            else
            {
                message.Append("The self Registration information has not been saved to the database.");
                message.Append(" Displaying default values from Web.Config. Please modify & save.");
                btnSave.Visible = true;
                btnModifyService.Visible = false;
                // Need to call selfRegister
                //lblServiceType.Text = ConfigurationManager.AppSettings["serviceType"];
                lblServiceType.Text = agentType;
                string serviceGUID = ConfigurationManager.AppSettings["serviceGUID"];
                if (serviceGUID != null)
                    txtServiceGuid.Text = serviceGUID;

                string serviceURL = ConfigurationManager.AppSettings["serviceURL"];
                if (serviceURL != null)
                    txtServiceUrl.Text = serviceURL;

                string serviceName = ConfigurationManager.AppSettings["serviceName"];
                if (serviceName != null)
                    txtServiceName.Text = serviceName;

                string codebaseURL = ConfigurationManager.AppSettings["codebaseURL"];
                if (codebaseURL != null)
                    txtCodebaseUrl.Text = codebaseURL;
                lblResponse.Text = Utilities.FormatWarningMessage(message.ToString());
                lblResponse.Visible = true;
            }
            txtOutPasskey.Text = ConfigurationManager.AppSettings["defaultPasskey"];

        }

        protected void SetFormMode(bool hasDomain)
        {
            lblResponse.Visible = false;
            if (hasDomain)
            {

                txtServiceName.Attributes.Add("onChange", "EnableServiceName();");
                txtCodebaseUrl.Attributes.Add("onChange", "EnableCodeBase();");
                txtServiceUrl.Attributes.Add("onChange", "EnableServiceUrl();");

                btnSave.Visible = false;
                btnModifyService.Visible = true;
                btnModifyService.Enabled = false;
                btnRetire.Visible = true;

            }
            else
            {
                txtServiceName.Attributes.Remove("onChange");
                txtCodebaseUrl.Attributes.Remove("onChange");
                txtServiceUrl.Attributes.Remove("onChange");

                btnSave.Visible = true;
                btnModifyService.Visible = false;
                btnRetire.Visible = false;
            }
            txtServiceGuid.ReadOnly = hasDomain;
            txtServiceGuid.BackColor = hasDomain ? disabled : enabled;
            btnGuid.Visible = !hasDomain;
        }


        protected void btnGuid_Click(object sender, System.EventArgs e)
        {
            Guid guid = System.Guid.NewGuid();
            txtServiceGuid.Text = Utilities.MakeGuid();
            //valGuid.Validate();
        }

        protected void checkGuid(object sender, ServerValidateEventArgs args)
        {
            if (args.Value.Length > 0 && args.Value.Length <= 50)
                args.IsValid = true;
            else
                args.IsValid = false;
        }

        protected void modifyService(object sender, ModifySelfEventArgs e)
        {
            bool error = false;
            StringBuilder message = new StringBuilder();
            try
            {
                if (ProcessAgentDB.ServiceAgent != null)
                {
                    string originalGuid = ProcessAgentDB.ServiceAgent.agentGuid;
                    if (!(txtServiceName.Text != null && txtServiceName.Text.Length > 0))
                    {
                        error = true;
                        message.Append(" You must enter a Service Name<br/>");
                    }

                    if (!(txtCodebaseUrl.Text != null && txtCodebaseUrl.Text.Length > 0))
                    {
                        error = true;
                        message.Append(" You must enter the base URL for the Web Site<br/>");
                    }
                    if (!(txtServiceUrl.Text != null && txtServiceUrl.Text.Length > 0))
                    {
                        error = true;
                        message.Append(" You must enter full URL of the Web Service page<br/>");
                    }
                    if (error)
                    {
                        lblResponse.Text = Utilities.FormatErrorMessage(message.ToString());
                        lblResponse.Visible = true;
                        return;
                    }
                    if (ProcessAgentDB.ServiceAgent.domainGuid != null)
                    {
                        ProcessAgentInfo originalAgent = dbTicketing.GetProcessAgentInfo(originalGuid);
                        ProcessAgentInfo sb = dbTicketing.GetProcessAgentInfo(ProcessAgentDB.ServiceAgent.domainGuid);
                        if ((sb != null) && !sb.retired)
                        {
                            ProcessAgentProxy psProxy = new ProcessAgentProxy();
                            AgentAuthHeader header = new AgentAuthHeader();
                            header.agentGuid = ProcessAgentDB.ServiceAgent.agentGuid;
                            header.coupon = sb.identOut;
                            psProxy.AgentAuthHeaderValue = header;
                            psProxy.Url = sb.webServiceUrl;
                            ProcessAgent pa = new ProcessAgent();
                            pa.agentGuid = txtServiceGuid.Text;
                            pa.agentName = txtServiceName.Text;
                            pa.domainGuid = ProcessAgentDB.ServiceAgent.domainGuid;
                            pa.codeBaseUrl = txtCodebaseUrl.Text;
                            pa.webServiceUrl = txtServiceUrl.Text;
                            pa.type = agentType;
                            //dbTicketing.SelfRegisterProcessAgent(pa.agentGuid, pa.agentName, agentType,
                            //    pa.domainGuid, pa.codeBaseUrl, pa.webServiceUrl);
                            //message.Append("Local information has been saved. ");
                            int returnValue = psProxy.ModifyProcessAgent(originalGuid, pa, null);
                            message.Append("The changes have been sent to the ServiceBroker");
                            if (returnValue > 0)
                            {
                                dbTicketing.SelfRegisterProcessAgent(pa.agentGuid, pa.agentName, agentType,
                                pa.domainGuid, pa.codeBaseUrl, pa.webServiceUrl);

                                message.Append(".<br />Local information has been saved. ");
                                lblResponse.Text = Utilities.FormatConfirmationMessage(message.ToString());
                                lblResponse.Visible = true;

                            }
                            else
                            {
                                message.Append(", but did not process correctly!");
                                message.Append("<br />Local information has not been saved. ");
                                lblResponse.Text = Utilities.FormatErrorMessage(message.ToString());
                                lblResponse.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        dbTicketing.SelfRegisterProcessAgent(ProcessAgentDB.ServiceAgent.agentGuid, txtServiceName.Text, agentType,
                                null, txtCodebaseUrl.Text, txtServiceUrl.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = new Exception("Error in  selfRegistration.modify()", ex);
                Utilities.WriteLog(Utilities.DumpException(ex2));
                throw ex2;
            }
        }


        protected void saveChanges(object sender, ModifySelfEventArgs e)
        {

            bool error = false;
            StringBuilder message = new StringBuilder();
            //Check fields for valid input
            if (!(txtServiceName.Text != null && txtServiceName.Text.Length > 0))
            {
                error = true;
                message.Append(" You must enter a Service Name<br/>");
            }
            if (!(txtServiceGuid.Text != null && txtServiceGuid.Text.Length > 0))
            {
                error = true;
                message.Append(" You must enter a Guid for the service<br/>");
            }
            //if (!valGuid.IsValid)
            //{
            //    error = true;
            //    message.Append(valGuid.Text + "<br />");
            //}
            if (!(txtCodebaseUrl.Text != null && txtCodebaseUrl.Text.Length > 0))
            {
                error = true;
                message.Append(" You must enter the base URL for the Web Site<br/>");
            }
            if (!(txtServiceUrl.Text != null && txtServiceUrl.Text.Length > 0))
            {
                error = true;
                message.Append(" You must enter full URL of the Web Service page<br/>");
            }

            if (error)
            {
                lblResponse.Text = Utilities.FormatErrorMessage(message.ToString());
                lblResponse.Visible = true;
                return;
            }
            else
            {
                // Check if domain is set if so only update mutable Fields
                dbTicketing.SelfRegisterProcessAgent(txtServiceGuid.Text,
                    txtServiceName.Text, lblServiceType.Text, txtServiceGuid.Text,
                    txtCodebaseUrl.Text, txtServiceUrl.Text);

                DisplayForm();
                lblResponse.Text = Utilities.FormatConfirmationMessage("Self registration has completed!");
                lblResponse.Visible = true;
            }
        }

        protected void btnRetire_Click(object sender, RefreshSelfEventArgs e)
        {
            bool error = false;
            StringBuilder message = new StringBuilder();
            try
            {
                if (ProcessAgentDB.ServiceAgent.domainGuid != null)
                {
                    ProcessAgentInfo originalAgent = dbTicketing.GetProcessAgentInfo(ProcessAgentDB.ServiceAgent.agentGuid);
                    ProcessAgentInfo sb = dbTicketing.GetProcessAgentInfo(ProcessAgentDB.ServiceAgent.domainGuid);
                    if ((sb != null) && !sb.retired)
                    {
                        ProcessAgentProxy psProxy = new ProcessAgentProxy();
                        AgentAuthHeader header = new AgentAuthHeader();
                        header.agentGuid = originalAgent.agentGuid;
                        header.coupon = sb.identOut;
                        psProxy.AgentAuthHeaderValue = header;
                        psProxy.Url = sb.webServiceUrl;

                        int returnValue = psProxy.RetireProcessAgent(originalAgent.domainGuid,
                            originalAgent.agentGuid, true);
                        message.Append("The changes have been sent to the ServiceBroker");
                        if (returnValue > 0)
                        {
                            dbTicketing.SetDomainGuid(null);
                            dbTicketing.SetSelfState(originalAgent.agentGuid, false);
                            dbTicketing.SetProcessAgentRetired(originalAgent.agentGuid, true);
                            dbTicketing.DeleteTickets(originalAgent.agentGuid);

                            message.Append(".<br />Local information has been saved. ");
                            lblResponse.Text = Utilities.FormatConfirmationMessage(message.ToString());
                            lblResponse.Visible = true;

                        }
                        else
                        {
                            message.Append(", but did not process correctly!");
                            message.Append("<br />Local information has not been saved. ");
                            lblResponse.Text = Utilities.FormatErrorMessage(message.ToString());
                            lblResponse.Visible = true;
                        }
                    }
                    else
                    {
                        dbTicketing.SelfRegisterProcessAgent(ProcessAgentDB.ServiceAgent.agentGuid, txtServiceName.Text, agentType,
                                null, txtCodebaseUrl.Text, txtServiceUrl.Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = new Exception("Error in  selfRegistration.modify()", ex);
                Utilities.WriteLog(Utilities.DumpException(ex2));
                throw ex2;
            }
            txtServiceName.Text = "";
            txtServiceGuid.Text = "";
            txtCodebaseUrl.Text = "";
            txtServiceUrl.Text = "";
            lblResponse.Text = "";
            lblResponse.Visible = false;
        }


        protected void btnNew_Click(object sender, System.EventArgs e)
        {
            txtServiceName.Text = "";
            txtServiceGuid.Text = "";
            txtCodebaseUrl.Text = "";
            txtServiceUrl.Text = "";
            lblResponse.Text = "";
            lblResponse.Visible = false;
        }

        protected void btnRefresh_Click(object sender, System.EventArgs e)
        {
            txtServiceName.Text = "";
            txtServiceGuid.Text = "";
            txtCodebaseUrl.Text = "";
            txtServiceUrl.Text = "";

            DisplayForm();
        }

        #endregion

        #region render

        protected override void Render(HtmlTextWriter output)
        {
            EnsureChildControls();
            DisplayForm();
            RenderContents(output);
        }


        protected override void CreateChildControls()
        {
            Controls.Clear();
            CreateControlHierarchy();
            ClearChildViewState();
        }

        protected void CreateControlHierarchy()
        {
          code = new StringBuilder();
                code.AppendLine("<script>");
                code.AppendLine("function EnableServiceName() {");
                code.AppendLine("var msg= 'Are you sure you want to modify the Web Service Name?" + modifyMessage +"';");
                code.AppendLine("if (confirm(msg)) document.getElementById('selfReg_btnModifyService').disabled = false;");
                code.AppendLine("else{ document.getElementById('txtServiceName').value = document.getElementById('bakServiceName').value;}}</script>");
                code.AppendLine();
                code.AppendLine("<script>function EnableCodeBase() {");
                code.AppendLine("var msg= 'Are you sure you want to modify the CodeBase for this service?" + modifyMessage + "';");
                code.AppendLine("if (confirm(msg))document.getElementById('btnModifyService').disabled = false;");
                code.AppendLine("else{document.getElementById('txtCodebaseUrl').value = document.getElementById('bakCodebase').value;}}");
                code.AppendLine();
                code.AppendLine("function EnableServiceUrl() {");
                code.AppendLine("var msg= 'Are you sure you want to modify the Web Service URL?" + modifyMessage + "';");
                code.AppendLine("if (confirm(msg))document.getElementById('btnModifyService').disabled = false;");
                code.AppendLine("else{document.getElementById('txtWebServiceUrl').value = document.getElementById('bakServiceUrl').value;}}");
                code.AppendLine();
                code.AppendLine("function ConfirmRetire() {");
                code.AppendLine("var msg= 'Are you sure you want to retire this WebService.\\nIf you proceed all references to this site will be retired';");
                code.AppendLine("var state = confirm(msg);return state;}");
                code.AppendLine("</script>");
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OnNameChange", code.ToString());
            pageintro = new HtmlGenericControl("div");
            pageintro.ID ="pageintro";
            lblTitle = new Label();
            lblTitle.Text = Title;
            lblTitle.ID = "lblTitle";
            lblResponse = new Label();
            lblResponse.Visible = false;
            lblResponse.ID = "lblResponse";
            lblDescription = new Label();
            lblDescription.ID = "lblDescription";
            lblDescription.Text = Description;
            lblDescription.Visible = true;
            lblServiceType = new Label();
            lblServiceType.ID = "lblServiceType";
            HtmlGenericControl h1 = new HtmlGenericControl("h1");
            h1.Controls.Add(lblTitle);
            pageintro.Controls.Add(h1);
            
            HtmlGenericControl p = new HtmlGenericControl("p");
            p.Controls.Add(lblResponse);
            pageintro.Controls.Add(p);
           
            p = new HtmlGenericControl("p");
            p.Controls.Add(lblDescription);
            pageintro.Controls.Add(p);
            Controls.Add(pageintro);

            pagecontent = new HtmlGenericControl("div");
            pagecontent.ID = "pagecontent";

            frmRegister = new HtmlForm();
            frmRegister.Method = "post";
            frmRegister.Target = "";
            frmRegister.Name = "RegisterSelf";
            frmRegister.ID = "frmRegister";
            hdnServiceGuid = new HiddenField();
            hdnServiceGuid.ID = "bakServiceGuid";
            frmRegister.Controls.Add(hdnServiceGuid);
            hdnServiceName = new HiddenField();
            hdnServiceName.ID = "bakServiceName";
            frmRegister.Controls.Add(hdnServiceName);
            hdnServiceUrl = new HiddenField();
            hdnServiceUrl.ID = "bakServiceUrl";
            frmRegister.Controls.Add(hdnServiceUrl);
            hdnCodebaseUrl = new HiddenField();
            hdnCodebaseUrl.ID = "bakCodebaseUrl";
            frmRegister.Controls.Add(hdnCodebaseUrl);
            tblMain = new Table();
            tblMain.CssClass = "simpleform";
            tblMain.Width = width;
            
            Literal spacer = new Literal();
            spacer.Text = "&nbsp;&nbsp;";
            Style s1 = new Style();
            s1.Width = labelWidth;
            s1.Height = onePX;
            s1.ForeColor = ForeColor;
            Style s2 = new Style();
            s2.Width = guidWidth;
            s2.ForeColor = BorderColor;
            Style s3 = new Style();
            s3.Width = buttonColumnWidth;
            s3.ForeColor = ForeColor;
            Style s4 = new Style();
            s4.Width = dataWidth;
            Style txtBoxStyle = new Style();
            txtBoxStyle.Height = txtBoxHeight;
            txtBoxStyle.Width = dataWidth;
            txtBoxStyle.BorderColor = BorderColor;
            

            Style txtGuidStyle = new Style();
            txtGuidStyle.Height = txtBoxHeight;
            txtGuidStyle.Width = guidWidth;
            txtGuidStyle.BorderColor = BorderColor;
            TableRow row;
            TableHeaderCell th;
            TableCell td;
            TableCell td2;
            //Label lbl;
            //Create the first row
            row = new TableRow();
            th = new TableHeaderCell();
            th.ApplyStyle(s1);
            td = new TableCell();
            td.ApplyStyle(s2);
            td2 = new TableCell();
            td2.ApplyStyle(s3);
            row.Cells.Add(th);
            row.Cells.Add(td);
            row.Cells.Add(td2);
            tblMain.Rows.Add(row);

            row = new TableRow();
            th = new TableHeaderCell();
            th.ColumnSpan = 3;
            th.HorizontalAlign = HorizontalAlign.Center;
            th.Text = "Required Credential Information";
            row.Cells.Add(th);
            tblMain.Rows.Add(row);

            row = new TableRow();
            th = new TableHeaderCell();
            
            lblServiceName = new Label();
            lblServiceName.ID = "lblServiceName";
            lblServiceName.Text = "Service Name";
            th.Controls.Add(lblServiceName);
            td = new TableCell();
            td.ColumnSpan = 2;
            txtServiceName = new TextBox();
            txtServiceName.ID = "txtServiceName";
            txtServiceName.ApplyStyle(txtBoxStyle);
            td.Controls.Add(txtServiceName);
            row.Cells.Add(th);
            row.Cells.Add(td);
            tblMain.Rows.Add(row);

            row = new TableRow();
            th = new TableHeaderCell();
            lblServiceGuid = new Label();
            lblServiceGuid.ID = "lblServiceGuid";
            lblServiceGuid.Text = "Service GUID";
            th.Controls.Add(lblServiceGuid);

            td = new TableCell();
            td.ColumnSpan = 1;
            txtServiceGuid = new TextBox();
            txtServiceGuid.ID = "txtServiceGuid";
            txtServiceGuid.ApplyStyle(txtGuidStyle);
            td.Controls.Add(txtServiceGuid);
            td2 = new TableCell();
            td2.ApplyStyle(s3);
            btnGuid = new Button();
            btnGuid.Text = "Create GUID";
            btnGuid.ID = "btnGuid";
            btnGuid.Enabled = false;
            td2.Controls.Add(btnGuid);
            row.Cells.Add(th);
            row.Cells.Add(td);
            row.Cells.Add(td2);
            tblMain.Rows.Add(row);

            row = new TableRow();
            th = new TableHeaderCell();
            lblCodebaseUrl = new Label();
            lblCodebaseUrl.ID = "lblCodebaseUrl";
            lblCodebaseUrl.Text = "Codebase URL";
            th.Controls.Add(lblCodebaseUrl);
         
            td = new TableCell();
            td.ColumnSpan = 2;
            txtCodebaseUrl = new TextBox();
            txtCodebaseUrl.ID = "txtCodebaseUrl";
            txtCodebaseUrl.ApplyStyle(txtBoxStyle);
            td.Controls.Add(txtCodebaseUrl);
            row.Cells.Add(th);
            row.Cells.Add(td);
            tblMain.Rows.Add(row);

            row = new TableRow();
            th = new TableHeaderCell();
            lblServiceUrl = new Label();
            lblServiceUrl.ID = "lblServiceUrl";
            lblServiceUrl.Text = "Web Service URL";
            th.Controls.Add(lblServiceUrl);
            
            td = new TableCell();
            td.ColumnSpan = 2;
            txtServiceUrl = new TextBox();
            txtServiceUrl.ID = "txtServiceUrl";
            txtServiceUrl.ApplyStyle(txtBoxStyle);
            td.Controls.Add(txtServiceUrl);
            row.Cells.Add(th);
            row.Cells.Add(td);
            tblMain.Rows.Add(row);

           

            trDomainSB = new TableRow();
            th = new TableHeaderCell();
            lblDomainServer = new Label();
            lblDomainServer.ID = "lblDomainServer";
            lblDomainServer.Text = "Domain ServiceBroker";
            th.Controls.Add(lblDomainServer);
           
            td = new TableCell();
            td.ColumnSpan = 2;
            txtDomainServer = new TextBox();
            txtDomainServer.ApplyStyle(txtBoxStyle);
            td.Controls.Add(txtDomainServer);
            trDomainSB.Cells.Add(th);
            trDomainSB.Cells.Add(td);
            tblMain.Rows.Add(trDomainSB);

            trRowPasskey = new TableRow();
            th = new TableHeaderCell();
            lblOutPassKey = new Label();
            lblOutPassKey.ID = "lblOutPassKey";
            lblOutPassKey.Text = "Install Credential Passkey";
            th.Controls.Add(lblOutPassKey);
           
            td = new TableCell();
            td.ColumnSpan = 2;
            txtOutPasskey = new TextBox();
            txtOutPasskey.ApplyStyle(txtBoxStyle);
            td.Controls.Add(txtOutPasskey);
            td2 = new TableCell();
            td2.ApplyStyle(s3);
            trRowPasskey.Cells.Add(th);
            trRowPasskey.Cells.Add(td);
            tblMain.Rows.Add(trRowPasskey);

            row = new TableRow();
            th = new TableHeaderCell();
            th.ColumnSpan = 3;
            th.HorizontalAlign = HorizontalAlign.Center;
            th.Text = "Optional Information";
            row.Cells.Add(th);
            tblMain.Rows.Add(row);

            row = new TableRow();
            th = new TableHeaderCell();
            lblContactInfo = new Label();
            lblContactInfo.ID = "lblContactInfo";
            lblContactInfo.Text = "Contact Information";
            th.Controls.Add(lblContactInfo);
           
            td = new TableCell();
            td.ColumnSpan = 2;
            txtContactInfo = new TextBox();
            txtContactInfo.ApplyStyle(txtBoxStyle);
            td.Controls.Add(txtContactInfo);
            row.Cells.Add(th);
            row.Cells.Add(td);
            tblMain.Rows.Add(row);

            row = new TableRow();
            th = new TableHeaderCell();
            th.ColumnSpan = 2;
            btnSave = new Button();
            btnSave.Text = "Save Service";
            btnSave.CommandName = "Save";
            btnSave.Enabled = false;
            th.Controls.Add(btnSave);  
            btnModifyService = new Button();
            btnModifyService.Text = "Modify Service";
            btnModifyService.CommandName = "Modify";
            btnModifyService.Enabled = false;
            th.Controls.Add(btnModifyService);  
            th.Controls.Add(spacer);

            btnRefresh = new Button();
            btnRefresh.Text = "Refresh";
            btnRefresh.CommandName="Refresh";
            btnRefresh.ID= "btnRefresh";
            th.Controls.Add(btnRefresh);
            th.Controls.Add(spacer);

            btnClear = new Button();
            btnClear.Text = "Clear";
             btnClear.CommandName="Clear";
             btnClear.ID="btnClear";
      
            th.Controls.Add(btnClear);
            
            th.Controls.Add(spacer);
            btnRetire = new Button();
            btnRetire.ID = "btnRetire";
            btnRetire.Text = "Retire";
            btnRetire.CommandName="Retire";
            th.Controls.Add(btnRetire);
            td = new TableCell();
            row.Cells.Add(th);
            row.Cells.Add(td);
            tblMain.Rows.Add(row);


            //td = new TableCell();
            //td.ColumnSpan = 2;
            //txtContactInfo = new TextBox();
            //txtContactInfo.ApplyStyle(txtBoxStyle);
            //td.Controls.Add(txtContactInfo);
            //row.Cells.Add(th);
            //row.Cells.Add(td);
            //tblMain.Rows.Add(row);

            frmRegister.Controls.Add(tblMain);
            pagecontent.Controls.Add(frmRegister);
            Controls.Add(pagecontent);

            ChildControlsCreated = true;
          
        }

#endregion

        #region PostBack


        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventArgument"></param>
        public void RaisePostBackEvent(string eventArgument)
        {
            ModifySelf(this, new ModifySelfEventArgs());
        }

        #endregion

    }

    public class ModifySelfEventArgs : EventArgs
    {
        public string serviceName;
        public string originalGuid;
        public string serviceGuid;
        public string domainGuid;
        public string codebaseUrl;
        public string serviceUrl;

        public ModifySelfEventArgs()
        {
        }
    }

    public class RefreshSelfEventArgs : EventArgs
    {
        public string originalGuid;
        public string domainGuid;
        
        public RefreshSelfEventArgs()
        {
        }
    }
    /// <summary>
    /// Delegate for passing an event.
    /// </summary>
    public delegate void ModifySelfDelegate(object sender, ModifySelfEventArgs e);

    /// <summary>
    /// Delegate for passing an event.
    /// </summary>
    public delegate void RefreshSelfDelegate(object sender, RefreshSelfEventArgs e);
}
