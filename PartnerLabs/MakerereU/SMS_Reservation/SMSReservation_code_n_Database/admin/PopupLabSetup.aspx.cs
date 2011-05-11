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
    protected string sbURL = null;
    protected string ClientNm = null;
    protected string ClientGUID = null;
    protected string LsName = null;
    protected string LsGuid = null;
    protected int MaxDuratn;
   
    

    
    //string Lab_Name = LabNameInput.Text;
    //string Lab_Description = LabDescriptionInput.Text;
    //string Group_Name = ExperimentGroupInPut.Text;
    
    protected void CreateBtn_Click(object sender, EventArgs e)
    {
        if (TxBLabName.Text == "" && TxBLabDescription.Text == "" && appCallName.Text == "" && TxBExptGroup.Text == "" &&
             TxBClientName.Text == "" && TxBClientGuid.Text == "" &&
             TxBmaxLabDurn.Text == "" && appCallNameBx.Text == "")
        {
            Confirmation.Text = "No value has been inserted";
        }

        else if (TxBLabName.Text == "" || TxBLabDescription.Text == "" || appCallName.Text == "" || TxBExptGroup.Text == "" ||
             TxBSbGuid.Text == ""  || TxBClientName.Text == "" || TxBClientGuid.Text == "" || 
             TxBmaxLabDurn.Text == "" || appCallNameBx.Text == "")
        {
            Confirmation.Text = "some missing values";
        }

        else
        {
            if (IsInteger(TxBmaxLabDurn.Text) == false) 
            {
                Confirmation.Text = "Please check the maximum lab duaration is an integer";
                
            }

            
            else
            {
        
                //assigning the incoming values and putting some to lower case
        LabNm = TxBLabName.Text.ToLower();
        LabDescpt = TxBLabDescription.Text.ToLower();
        ExptGrpN = TxBExptGroup.Text.ToLower();
        sbGuid = TxBSbGuid.Text;
        sbURL = TxBSbUrl.Text.ToLower();
        ClientNm = TxBClientName.Text;
        ClientGUID = TxBClientGuid.Text;
        LsName = TxBLsName.Text;
        LsGuid = TxBLsGuid.Text;
        MaxDuratn = Convert.ToInt32(TxBmaxLabDurn.Text.Trim());
        

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
            lc.applicationCallName = appCallNameBx.Text.Trim();
            lc.LabName = TxBLabName.Text.Trim();
            lc.LabDescription = LabDescpt.Trim();
            lc.ExperimentGroupName = ExptGrpN.Trim();
            lc.ServiceBrokerGUID = sbGuid.Trim();


            lc.ServiceBrokerURL = sbURL.Trim();
            lc.ClientName = ClientNm.Trim();
            lc.ClientGuid = ClientGUID.Trim();

            lc.MaximumLabDuration = MaxDuratn;

            lc.LabServerName = LsName.Trim();
            lc.LabServerGuid = LsGuid.Trim();
            lc.DateCreated = DateTime.UtcNow;

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
