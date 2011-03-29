/*-------------------------------MyDoSchedule Class--------------------------------------------------
 * 
 * The class contains a method similar to the DoSchedule() that is available in MyClient.aspx.cs
 * This method however doesnot redirect a user, but returns the variables required for scheduling a user
 * that is, coupon_id, issuerGuid and passKey. 
 * The service broker url is also included but i dont need it in my case, atleast not for now
 * 
 */

using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.Core;
using iLabs.ServiceBroker;
using System.Configuration;
using System;
using iLabs.UtilLib;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.DataTypes.TicketingTypes;


public class myDoSchedule
{
    protected LabClient lc;

    AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();

    BrokerDB issuer = new BrokerDB();

    public string couponId = null;
    public string passkey = null;
    public string issuerGuid = null;
    public string auto = null;
    Coupon opCoupon = null;
    public string ussGuid;
    public string lssGuid;
    public int userTZ;

    public string username = "test";   //this is the actual user of the system
    public string labClientName = "Time Of Day Interactive Client";
    public string labClientVersion = "1.5";
    public string effectiveGroupName;
    public string ClientGuid = null;
    public string labServerGuid = null;

   public string doScheduling()
    {
        string clientID = "1";  //this was manually got and thus needs to be later replaced
        lc = wrapper.GetLabClientsWrapper(new int[] { Convert.ToInt32(clientID) })[0];
            

        effectiveGroupName = "Experiment_Group";

        ProcessAgent labServer = issuer.GetProcessAgent(lc.labServerIDs[0]);
        int ussId = issuer.FindProcessAgentIdForClient(lc.clientID, ProcessAgentType.SCHEDULING_SERVER);
        ClientGuid = lc.clientGuid;
        labServerGuid = labServer.agentGuid;
        //labServer = labServer.agentGuid;
        

            ussGuid = issuer.GetProcessAgent(ussId).agentGuid;
            int lssId = issuer.FindProcessAgentIdForAgent(lc.labServerIDs[0], ProcessAgentType.LAB_SCHEDULING_SERVER);

            lssGuid = issuer.GetProcessAgent(lssId).agentGuid;

          
            //Default duration ????
            long duration = 36000;

            userTZ = 180;

            //now below are the 
            RecipeExecutor recipeExec = RecipeExecutor.Instance();
            string schedulingUrl = recipeExec.ExecuteExerimentSchedulingRecipe(ussGuid, lssGuid, username, effectiveGroupName,
                labServer.agentGuid, lc.clientGuid, labClientName, labClientVersion,
               duration, userTZ);

            

            //displaying the elements in scheduleUrl (this is just temporary)
            string[] scheduleUrlElements = splitScheduleUrl(schedulingUrl) ;
            couponId = scheduleUrlElements[2];
            issuerGuid = scheduleUrlElements[4];
            passkey = scheduleUrlElements[6];



            return "Scheduling Url:" + schedulingUrl + "\n And LabServerGuid is:" + labServerGuid;
    }

    /* This method below splits the schedule url provided by the do schedule function
     * to pick out the coupon_id, issuer_guid, and passkey.
     * These are elements later needed in the scheduling of the user.
     */
   public string[] splitScheduleUrl(string scheduleUrl)
   {
       string[] scheduleUrlElements = scheduleUrl.Split('?','&','=');
       return scheduleUrlElements;
   }
}