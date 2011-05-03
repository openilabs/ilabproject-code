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
using System.Data.Sql;
using System.Data.SqlClient;

public partial class PopupLabSetup : System.Web.UI.Page
{
    protected int LabConfID;        //probably will not need to refer
    protected string LabNm = null;
    protected string LabDescpt = null;
    protected string ExptGrpN = null;
    protected string sbGuid = null;
    protected string InPasKeyNm = null;
    protected int InPasKyID;
    protected int OutPasKyID;
    protected string OutPasKyNm = null;
    protected string sbURL = null;
    protected string ClientNm = null;
    protected string ClientGUID = null;
    protected string LsName = null;
    protected string LsGuid = null;
    protected string IMGuid = null;
    protected int MinDuratn;
    protected int MaxDuratn;
    

    
    //string Lab_Name = LabNameInput.Text;
    //string Lab_Description = LabDescriptionInput.Text;
    //string Group_Name = ExperimentGroupInPut.Text;
    
    protected void CreateBtn_Click(object sender, EventArgs e)
    {
        if (TxBLabName.Text == "" && TxBLabDescription.Text == "" && TxBExptGroup.Text == "" &&
             TxBSbGuid.Text == "" && TxBInPasskeyName.Text == "" && TxBInPasskey.Text == "" &&
             TxBOutPasskey.Text == "" && TxBOutPasskeyName.Text == "" && TxBSbUrl.Text == "" &&
             TxBClientName.Text == "" && TxBClientGuid.Text == "" && TxBminLabDurn.Text == "" &&
             TxBmaxLabDurn.Text == "" && TxBMessageGuid.Text == "")
        {
            Confirmation.Text = "No value has been inserted";
        }

        else if (TxBLabName.Text == "" || TxBLabDescription.Text == "" || TxBExptGroup.Text == "" ||
             TxBSbGuid.Text == "" || TxBInPasskeyName.Text == "" || TxBInPasskey.Text == "" ||
             TxBOutPasskey.Text == "" || TxBOutPasskeyName.Text == "" || TxBSbUrl.Text == "" ||
             TxBClientName.Text == "" || TxBClientGuid.Text == "" || TxBminLabDurn.Text == "" ||
             TxBmaxLabDurn.Text == "" || TxBMessageGuid.Text == "")
        {
            Confirmation.Text = "some missing values";
        }

        else
        {
            if (IsInteger(TxBInPasskey.Text) == false || IsInteger(TxBOutPasskey.Text) == false ||
                IsInteger(TxBminLabDurn.Text) == false || IsInteger(TxBmaxLabDurn.Text) == false) 
            {
                Confirmation.Text = "Please check if the following are integers:";
                Confirmation.Text += "<br>In Passkey ID";
                Confirmation.Text += "<br>Out passkey ID";
                Confirmation.Text += "<br>Minimum Lab Duration";
                Confirmation.Text += "<br>Maximum Lab duration";
            }
            
            else
            {
        
        LabNm = TxBLabName.Text;
        LabDescpt = TxBLabDescription.Text;
        ExptGrpN = TxBExptGroup.Text;
        sbGuid = TxBSbGuid.Text;
        InPasKeyNm = TxBInPasskeyName.Text;
        InPasKyID = Convert.ToInt32(TxBInPasskey.Text);
        OutPasKyID = Convert.ToInt32(TxBOutPasskey.Text);
        OutPasKyNm = TxBOutPasskeyName.Text;
        sbURL = TxBSbUrl.Text;
        ClientNm = TxBClientName.Text;
        ClientGUID = TxBClientGuid.Text;
        LsName = TxBLsName.Text;
        LsGuid = TxBLsGuid.Text;
        MinDuratn = Convert.ToInt32(TxBminLabDurn.Text);
        MaxDuratn = Convert.ToInt32(TxBmaxLabDurn.Text);
        IMGuid = TxBMessageGuid.Text;

        //insert into database
        Confirmation.Text = AddToDB();
        //add some error checks
            }

       }

    }

    
    protected void ClosePopupButton_Click(object sender, EventArgs e)
    {
        // string clientScript="window.close(); return false;";


    }

    
   

    protected string AddToDB()
    {
        try
        {
            DataClassesDataContext db = new DataClassesDataContext();
            //LabConfiguration v = dx.LabConfigurations.Where(s => s.LabConfigurationID > 34).First();
            //v.LabDescription = "wefwedf";
            //dx.SubmitChanges();

            LabConfiguration lc = new LabConfiguration();
            lc.LabName = TxBLabName.Text;
            lc.LabDescription = LabDescpt;
            lc.ExperimentGroupName = ExptGrpN;
            lc.ServiceBrokerGUID = sbGuid;
            lc.InstantMessageInPasskey = InPasKeyNm;
            lc.InstantMessageOutPassKey = OutPasKyNm;
            lc.InstantMessageInID = InPasKyID;
            lc.InstantMessageOutID = OutPasKyID;
            lc.ServiceBrokerURL = sbURL;
            lc.ClientName = ClientNm;
            lc.ClientGuid = ClientGUID;
            lc.InstantMessageGUID = IMGuid;
            lc.MaximumLabDuration = MaxDuratn;
            lc.MinimumLabDuration = MinDuratn;
            lc.LabServerName = LsName;
            lc.LabServerGuid = LsGuid;

            db.LabConfigurations.InsertOnSubmit(lc);
            db.SubmitChanges();

            return "Instant Scheduling configuration for Client " + ClientName.Text + "has been succesfully saved.";
        }
        catch
        {
            return "Failed to connect to database";
        }
    }

    
   protected bool IsInteger(string theValue)
    {
        try
        {
            Convert.ToInt32(theValue);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
