using System.Collections;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.Administration;
using System;
using System.Configuration;
public  static class userGroup
{

    static int  userID;
    static string effectiveGroup;

    public static string getUserGroups(string username)
    {
        //retrieve the userID then move on to get e groups
        //userID = Convert.ToInt32(usergroup.userAuthenticatation(username));
        ArrayList nonRequestGroups = new ArrayList();
        ArrayList requestGroups = new ArrayList();
        AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();

        
        int[] groupIDs = wrapper.ListGroupsForAgentWrapper(userID);
        Group[] gps = AdministrativeAPI.GetGroups(groupIDs);
        
        string groupChoosen = "Experiment_Group";   //this has to be changed later <temporary>
        
        //Specifying the group with the lab...
        for (int i = 0; i < gps.Length; i++)
        {
           
            if (String.Equals(gps[i].GroupName, groupChoosen))
            {
                effectiveGroup = gps[i].GroupName;
            }
        }
      

        return effectiveGroup;
    }

    //this method returns the userId of the user who is authenticating into the system
    public static int userAuthenticatation(string username)
    {
        AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
        //string supportMailAddress = ConfigurationSettings.AppSettings["supportMailAddress"];

        userID = -1;
        userID = wrapper.GetUserIDWrapper(username);
        return userID;
    }

   
}
