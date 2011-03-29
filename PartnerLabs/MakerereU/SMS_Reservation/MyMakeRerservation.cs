using iLabs.Proxies.LSS;
using System;
using iLabs.UtilLib;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.DataTypes.TicketingTypes;
using System.Globalization;
using iLabs.Core;
using iLabs.Proxies.Ticketing;
using iLabs.DataTypes.SoapHeaderTypes;
using System.Collections.Generic;
using iLabs.Scheduling.UserSide;
using System.Xml;
using iLabs.Ticketing;


public class MyMakeReservationClass
{
    public bool useQuirk = false;
    DateTime startTime;
    DateTime endTime;
    string serviceBrokerGuid = null;
    string groupName = "Experiment_group";
    string clientGuid = null;
    string labServerGuid = null;
    string labClientName = null;
    string labClientVersion = null;
    string username = null;
    string lssURL = null;
    TimePeriod[] timeSlots;
    DateTime startReserveTime;
    DateTime endReserveTime;
    string lssGuid = null;
    string ussGuid = null;
    TicketIssuerProxy ticketIssuer;
    Coupon coupon;
    int userTZ;
    CultureInfo culture;
    ProcessAgentDB dbTicketing = new ProcessAgentDB();
    List<TimePeriod> periods = null;
    int defaultRange = 30;
    int quantum = 1; //this has been put there for temporaray purposes
    DateTime endTimePeriod;
    TimeSpan maxAllowTime;
    TimeSpan minRequiredTime;
    TimeSpan minTime;


    public string SMSMakeReservation()
    {

        //some elements need for the make reservation
        myDoSchedule scheduleDetails = new myDoSchedule();
        scheduleDetails.doScheduling();
        groupName = scheduleDetails.effectiveGroupName;
        serviceBrokerGuid = scheduleDetails.issuerGuid;
        clientGuid = scheduleDetails.ClientGuid;
        labServerGuid = scheduleDetails.labServerGuid;
        ussGuid = scheduleDetails.ussGuid;
        username = scheduleDetails.username;

        long couponID = Convert.ToInt64(scheduleDetails.couponId);
            
        string issuerID = scheduleDetails.issuerGuid;
        string passKey = scheduleDetails.passkey;

        coupon = new Coupon(issuerID, couponID, passKey);
        Ticket ticket = dbTicketing.RetrieveAndVerify(coupon, TicketTypes.SCHEDULE_SESSION);

        //XmlDocument payload = new XmlDocument();
        //payload.LoadXml(ticket.payload);


        // ToDo: Add error checking
        string userRequestTime = "4/4/2011 11:00PM";        //this is for testing purposes 
        string durationAdded = "1200";                      //this is for testing purposes but can be assigned by admin
        //int userTZ = 180;
        culture = CultureInfo.CreateSpecificCulture("en-US");

        DateTime startReserveTime = DateUtil.ParseUserToUtc(userRequestTime, culture, userTZ);
        DateTime endReserveTime = startReserveTime.AddSeconds(Double.Parse(durationAdded));
        if ((endReserveTime.Minute % quantum) != 0)
        {
            DateTime dt = endReserveTime.AddMinutes(quantum - (endReserveTime.Minute % quantum));
            if (dt <= endTimePeriod)
            {
                endReserveTime = dt;
            }
            else
            {
                endReserveTime = endTimePeriod;
            }
        }
        string scheduleFeedback = userRequestTime + " " + DateUtil.ToUserTime(endReserveTime, culture, -userTZ);
        //return scheduleFeedback;
        //lblErrorMessage.Visible = true;
        string notification = null;
        LabSchedulingProxy lssProxy = new LabSchedulingProxy();
        //lssProxy.Url = lssURL;
        
        try
        {

            // create "REQUEST RESERVATION" ticket in SB and get the coupon for the ticket.
            iLabs.DataTypes.TicketingTypes.Coupon authCoupon = ticketIssuer.CreateTicket(lssGuid, "REQUEST RESERVATION", 300, "");

            //assign the coupon from ticket to the soap header;
            OperationAuthHeader opHeader = new OperationAuthHeader();
            opHeader.coupon = coupon;
            lssProxy.OperationAuthHeaderValue = opHeader;
            notification = lssProxy.ConfirmReservation(serviceBrokerGuid, groupName, 
                ussGuid, labServerGuid, clientGuid, startReserveTime, endReserveTime);
            if (notification != "The reservation is confirmed successfully")
            {
                scheduleFeedback = Utilities.FormatErrorMessage(notification);
                return scheduleFeedback + "00001";
            }
            else
            {
                try
                {
                    int status = USSSchedulingAPI.AddReservation(username, serviceBrokerGuid, groupName, labServerGuid, clientGuid,
                        startReserveTime, endReserveTime);
                    string confirm = "The reservation from " + DateUtil.ToUserTime(startReserveTime, culture, userTZ)
                        + " to " + DateUtil.ToUserTime(endReserveTime, culture, userTZ) + " is confirmed.";
                    scheduleFeedback = Utilities.FormatConfirmationMessage(confirm);
                    return scheduleFeedback + "00002";
                }
                catch (Exception insertEx)
                {
                    scheduleFeedback = Utilities.FormatErrorMessage(notification);
                    return scheduleFeedback + "00003";
                }
               // getTimePeriods();
            }
           // lblErrorMessage.Visible = true;
           
        }
        catch (Exception ex)
        {
            //string msg = "Exception: reservation can not be confirmed. " + ex.GetBaseException() + ".";
            //scheduleFeedback = Utilities.FormatErrorMessage(msg);
            return "00004";
            //lblErrorMessage.Visible = true;

        }
        try
        {
            if (notification == "The reservation is confirmed successfully")
            {
                int experimentInfoId = USSSchedulingAPI.ListExperimentInfoIDByExperiment(labServerGuid, clientGuid);
                DateTime startTimeUTC = startReserveTime.ToUniversalTime();
                DateTime endTimeUTC = endReserveTime.ToUniversalTime();
                return scheduleFeedback + "00005";
            }
           // return scheduleFeedback;
        }
        catch (Exception ex)
        {
            string msg = "Exception: reservation can not be added successfully. " + ex.GetBaseException() + ".";
            //lblErrorMessage.Text = Utilities.FormatErrorMessage(msg);

            lssProxy.RemoveReservation(serviceBrokerGuid, groupName, ProcessAgentDB.ServiceGuid, labServerGuid, 
                clientGuid, startReserveTime, endReserveTime);
            return msg + "00006";
        }
    }


    //private void getTimePeriods()
    //{
    //    throw new NotImplementedException();
    //}
}