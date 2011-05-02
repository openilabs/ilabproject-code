using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;



/* This file contains the code that
 * recieves elements from the Format message class and other element from the database to communicate 
 * with the ISA.
 * 
 */

public class ISAinterface
{
    private string LabNm = null;
    private string LabDescpt = null;
    private string ExptGrpN = null;
    private string sbGuid = null;
    private string InPasKeyNm = null;
    private int InPasKyID;
    private int OutPasKyID;
    private string OutPasKyNm = null;
    private string sbURL = null;
    private string ClientNm = null;
    private string ClientGUID = null;
    private string LsName = null;
    private string LsGuid = null;
    private string IMGuid = null;
    private int MinDuratn;
    private int MaxDuratn;
   

    private void pickingRequiredVariables()
    {
        //used to pick Guids and other variables required for scheduling
        //from the database
        //these are the ones declared above

        //DataClassesDataContext db = new DataClassesDataContext();
        //        //LabConfiguration v = dx.LabConfigurations.Where(s => s.LabConfigurationID > 34).First();
        //        //v.LabDescription = "wefwedf";
        //        //dx.SubmitChanges();
       
       
    }

    //Pick the time range from the user and use it to retrieve the TimePeriod
    //With the available time periods, check if there is an available time slot and how long it is
    //if present, and sufficient for the duration requested for, then make schedule
    //if present but with no sufficient time duration then no schedule and reply to user
    //if no available schedule, then inform the user.


    
    
}