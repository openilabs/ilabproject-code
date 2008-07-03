using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;

using iLabs.DataTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.UtilLib;

namespace iLabs.Scheduling.LabSide
{
    public partial class NewTimeBlockPopUp : System.Web.UI.Page
    {
        // string labServerID;
        int[] credentialSetIDs;
        LssCredentialSet[] credentialSets;
        CultureInfo culture = null;
        int userTZ = 0;
        TimeSpan tzOffset = TimeSpan.MinValue;
        StringBuilder buf = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            culture = DateUtil.ParseCulture(Request.Headers["Accept-Language"]);
            userTZ = (int)Session["userTZ"];
            tzOffset = TimeSpan.FromMinutes(-userTZ);

            txtStartDate.Attributes.Add("OnKeyPress", "return false;");
            txtEndDate.Attributes.Add("OnKeyPress", "return false;");

            if (ddlRecurrence.SelectedIndex != 3)
            {
                lblRecur.Visible = false;
                cbxRecurWeekly.Visible = false;
            }
            else
            {
                lblRecur.Visible = true;
                cbxRecurWeekly.Visible = true;
            }
            if (!IsPostBack)
            {
                ddlStartHour.SelectedIndex = 0;
                ddlEndAM.SelectedIndex = 0;
                ddlEndHour.SelectedIndex = 0;
                ddlStartAM.SelectedIndex = 0;

                credentialSetIDs = LSSSchedulingAPI.ListCredentialSetIDs();
                credentialSets = LSSSchedulingAPI.GetCredentialSets(credentialSetIDs);
                BuildServerDropDownListBox();

            }
        }

        /* 
             * Builds the Select Group drop down box. 
             * By default, the box gets filled with all the groups in the database
             */
        private void BuildServerDropDownListBox()
        {
            ddlLabServers.Items.Clear();
            try
            {
                IntTag[] lsTags = null;
                if(Session["labServerGuid"] != null)
                    lsTags = LSSSchedulingAPI.GetLSResourceTags(Session["labServerGuid"].ToString());
                else
                    lsTags = LSSSchedulingAPI.GetLSResourceTags();
                    ddlLabServers.Items.Add(new ListItem(" ---------- select Lab Server Resource ---------- "));
                foreach (IntTag tag in lsTags)
                {
                    ddlLabServers.Items.Add(new ListItem(tag.tag,tag.id.ToString()));
                }
                
            }
            catch (Exception ex)
            {
                string msg = "Exception: Cannot list LabServer Resources " + ex.Message + ". " + ex.GetBaseException();
                lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatErrorMessage(msg);
                lblErrorMessage.Visible = true;
            }

        }

        /// <summary>
        /// Parse the user time displayed values and convert to UTC 'user reperesentations', add user offset before
        /// inserting into the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.MinValue;
            TimeSpan startTime = TimeSpan.MinValue;
            int startHours = -1;
            int startMinutes = -1;
            
            DateTime endDate = DateTime.MinValue;
            TimeSpan endTime = TimeSpan.MinValue;
            int endHours = -1;
            int endMinutes = -1;

            // input error check
            try
            {
                if (ddlLabServers.SelectedIndex <= 0)
                {
                    lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatWarningMessage("You must select a Lab Server Resource.");
                    lblErrorMessage.Visible = true;
                    return;
                }
                if (ddlRecurrence.SelectedIndex <= 0)
                {
                    lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatWarningMessage("You must select a recurrence type.");
                    lblErrorMessage.Visible = true;
                    return;

                }
                // Local System date forced to UTC type
                if (txtStartDate.Text.Length == 0 || txtStartDate.Text.CompareTo(culture.DateTimeFormat.ShortDatePattern) == 0)
                {
                    lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatWarningMessage("You must enter the start date of the recurring time block.");
                    lblErrorMessage.Visible = true;
                    return;
                }
               startDate = DateTime.SpecifyKind(DateTime.Parse(txtStartDate.Text, culture), DateTimeKind.Utc);

               // Local System date forced to UTC type
                if (txtEndDate.Text.Length == 0 || txtEndDate.Text.CompareTo(culture.DateTimeFormat.ShortDatePattern) == 0)
                {
                    lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatWarningMessage("You must enter the end date of the recurring time block.");
                    lblErrorMessage.Visible = true;
                    return;
                }
                endDate = DateTime.SpecifyKind(DateTime.Parse(txtEndDate.Text, culture), DateTimeKind.Utc);

                if (endDate < startDate)
                {
                    lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatWarningMessage("The start date must be less than or equal to the end date.");
                    lblErrorMessage.Visible = true;
                    return;
                }
                // Add a day to end date  force to midnight
                endDate = endDate.AddDays(1.0);

                // Time of day in local time, used as an offset from startDate

                startHours = ddlStartHour.SelectedIndex;
                if (ddlStartAM.Text.CompareTo("PM") == 0)
                {
                    startHours += 12;
                } 
                if (txtStartMin.Text.Length > 0)
                    startMinutes = int.Parse(txtStartMin.Text);
                if (startMinutes >= 60 || startMinutes < 0)
                {
                    string msg = "Please input minutes ( 0 - 59 ) in the start time ";
                    lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatWarningMessage(msg);
                    lblErrorMessage.Visible = true;
                }

                endHours = ddlEndHour.SelectedIndex;
                if (ddlEndAM.Text.CompareTo("PM") == 0)
                {
                    endHours += 12;
                }

                
                if (txtEndMin.Text.Length > 0)
                    endMinutes = int.Parse(txtEndMin.Text);
                if (endMinutes >= 60 || endMinutes < 0)
                {
                    string msg = "Please input minutes ( 0 - 59 ) in the end time ";
                    lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatWarningMessage(msg);
                    lblErrorMessage.Visible = true;
                }

                startTime = new TimeSpan(startHours, startMinutes, 0);
                endTime = new TimeSpan(endHours, endMinutes, 0);


                if (startTime >= endTime)
                {
                    // If confirm see JavaScript
                    if(endDate.Subtract(startDate) > TimeSpan.FromDays(1.0)){
                        endTime.Add(TimeSpan.FromDays(1.0));
                    }
                    if (startTime >= endTime)
                    {
                        lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatWarningMessage("the start time should be earlier than the end time.");
                        lblErrorMessage.Visible = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                lblErrorMessage.Text = Utilities.FormatErrorMessage(msg);
                lblErrorMessage.Visible = true;
            }

            //If all the input error checks are cleared
            //if adding a new Recurrence and time blocks accordingly
            try
            {

                //Create UTC copies of the the startTime and EndTime, to check Recurrence
                DateTime uStartTime = startDate.Add(startTime).AddMinutes(-userTZ);
                DateTime uEndTime = endDate.Add(endTime).AddMinutes(-userTZ);

                DateTime uStartDate = startDate.AddMinutes(-userTZ);
                DateTime uEndDate = endDate.AddMinutes(-userTZ);

                // the database will also throw an exception if the combination of start time,
                // end time,lab server id, resource_id exists, or there are some 
                // Recurrences are included in the recurrence to be added.
                // since combination of them must be unique  and the time slots should 
                // be be overlapped
                // this is just another check to throw a meaningful exception

                Recurrence[] recurs = LSSSchedulingAPI.GetRecurrence(LSSSchedulingAPI.ListRecurrenceIDsByResourceID(uStartTime, uEndTime, int.Parse(ddlLabServers.SelectedValue)));
                if (recurs != null && recurs.Length > 0)
                {
                    Recurrence testRecur = new Recurrence();
                    testRecur.recurrenceStartDate = uStartDate;
                    testRecur.recurrenceEndDate = uEndDate;
                    testRecur.recurrenceStartTime = startTime;
                    testRecur.recurrenceEndTime = endTime;
                    testRecur.recurrenceType = ddlRecurrence.SelectedItem.Text;
                    testRecur.resourceId = int.Parse(ddlLabServers.SelectedValue);
                    // should only get & check recurrences on this resource
                    //int resourceTest = int.Parse(ddlLabServers.SelectedValue);
                    // tell whether the recurrence recur is overlapping with the input recurrence
                    // bool lap = (DateTime.Parse(DateUtil.ToUserTime(recur.recurrenceEndDate, culture, userTZ)) >= startDate) && (DateTime.Parse(DateUtil.ToUserTime(recur.recurrenceStartDate, culture, userTZ)) <= endDate);
                    //if ((recur.labServerGuid == Session["labServerGuid"].ToString()) && (recur.credentialSetId.ToString() == ddlGroup.SelectedValue) && lap)


                    buf = new StringBuilder();
                    bool conflict = false;
                    foreach (Recurrence r in recurs){
                        if(testRecur.HasConflict(r))
                        {
                            conflict = true;
                            report(r);
                        }
                    }

                    //switch (ddlRecurrence.SelectedIndex)
                    //{
                    //    case 1: // Single block always overlaps

                    //        foreach (Recurrence r in recurs)
                    //        { 
                    //                conflict = true;
                    //                report(r);
                    //        }
                    //        break;
                    //    case 2: //Daily block
                    //        TimeSpan start = uStartDate.TimeOfDay;
                    //        start.Add(startTime);
                    //        TimeSpan end = uEndDate.TimeOfDay;
                    //        end.Add(endTime);
                    //        foreach (Recurrence r in recurs)
                    //        {
                                
                    //                if (r.recurrenceType.CompareTo(Recurrence.NoRecurrence) == 0)
                    //                {
                    //                    report(r);
                    //                    conflict = true;
                    //                }

                    //                else if (r.recurrenceType.CompareTo(Recurrence.Daily) == 0)
                    //                {
                    //                    TimeSpan rStart = r.recurrenceStartDate.TimeOfDay;
                    //                    rStart.Add(r.recurrenceStartTime);
                    //                    TimeSpan rEnd = r.recurrenceEndDate.TimeOfDay;
                    //                    rEnd.Add(r.recurrenceEndTime);
                    //                    if (start < rEnd && end > rStart)
                    //                    {
                    //                        conflict = true;
                    //                        report(r);
                    //                    }
                    //                }
                    //                else if (r.recurrenceType.CompareTo(Recurrence.Weekly) == 0)
                    //                {
                    //                    // Not enough information to check
                    //                }
                                
                    //        }
                    //        break;
                    //    case 3: // Weekly block, currently not supported

                    //        foreach (Recurrence r in recurs)
                    //        {
                                
                    //                if (r.recurrenceType.CompareTo(Recurrence.NoRecurrence) == 0)
                    //                {
                    //                    conflict = true;
                    //                    report(r);
                    //                }
                    //                else if (r.recurrenceType.CompareTo(Recurrence.Daily) == 0)
                    //                {
                    //                }
                    //                else if (r.recurrenceType.CompareTo(Recurrence.Weekly) == 0)
                    //                {
                    //                    // Not enough information to check
                    //                }
                                
                    //        }
                    //        break;
                    //    default:
                    //        break;
                    //}



                    if (conflict)
                    {
                        lblErrorMessage.Text = Utilities.FormatErrorMessage(buf.ToString());
                        lblErrorMessage.Visible = true;
                        return;
                    }
                }
                //Add recurrence and time blocks accordingly
                // if no recurrence is selected
                if (ddlRecurrence.SelectedIndex == 1)
                {


                    int recurID = LSSSchedulingAPI.AddRecurrence(uStartDate, uEndDate, ddlRecurrence.SelectedItem.Text, startTime, endTime, Session["labServerGuid"].ToString(), Int32.Parse(ddlLabServers.SelectedValue), 0);
                    Session["newOccurrenceID"] = recurID;
                    int timeBlockID = LSSSchedulingAPI.AddTimeBlock(Session["labServerGuid"].ToString(), Int32.Parse(ddlLabServers.SelectedValue), uStartTime, uEndTime, recurID);

                }
                // if recurrence pattern is daily
                if (ddlRecurrence.SelectedIndex == 2)
                {

                    int recurID = LSSSchedulingAPI.AddRecurrence(uStartDate, uEndDate, Recurrence.Daily, startTime, endTime, Session["labServerGuid"].ToString(), Int32.Parse(ddlLabServers.SelectedValue), 0);
                    Session["newOccurrenceID"] = recurID;
                    DateTime dt = uStartDate;
                    while (dt <= uEndDate)
                    {
                        //the start date time for each time block in UTC
                        DateTime startTimeForTB = dt.Add(startTime);
                        DateTime endTimeForTB = dt.Add(endTime);
                        LSSSchedulingAPI.AddTimeBlock(Session["labServerGuid"].ToString(), Int32.Parse(ddlLabServers.SelectedValue), startTimeForTB, endTimeForTB, recurID);
                        dt = dt.AddDays(1);

                    }
                }
                // if recurrence pattern is weekly
                if (ddlRecurrence.SelectedIndex == 3)
                {
                    byte days = DateUtil.NoDays;
                    ArrayList repeatDays = new ArrayList();
                    if (cbxRecurWeekly.SelectedIndex == -1)
                    {
                        string msg = "Please check the days to repeat weekly.";
                        lblErrorMessage.Text = Utilities.FormatWarningMessage(msg);
                        lblErrorMessage.Visible = true;
                        return;

                    }
                    foreach (ListItem it in cbxRecurWeekly.Items)
                    {
                        if (it.Selected)
                        {
                            switch (it.Text)
                            {
                                case "Sunday":
                                    days |= DateUtil.SunBit;
                                    break;
                                case "Monday":
                                    days |= DateUtil.MonBit;
                                    break;
                                case "Tuesday":
                                    days |= DateUtil.TuesBit;
                                    break;
                                case "Wendesday":
                                    days |= DateUtil.WedBit;
                                    break;
                                case "Thursday":
                                    days |= DateUtil.ThursBit;
                                    break;
                                case "Friday":
                                    days |= DateUtil.FriBit;
                                    break;
                                case "Saturday":
                                    days |= DateUtil.SatBit;
                                    break;
                                default:
                                    break;
                            }
                            repeatDays.Add(it.Text);
                        }
                    }
                    int recurID = LSSSchedulingAPI.AddRecurrence(uStartDate, uEndDate, Recurrence.Weekly, startTime, endTime, Session["labServerGuid"].ToString(), Int32.Parse(ddlLabServers.SelectedValue), days);
                    Session["newOccurrenceID"] = recurID;
                    DateTime dt = uStartDate;



                    while (dt <= uEndDate)
                    {
                        if (repeatDays.Contains(dt.DayOfWeek.ToString()))
                        {

                            DateTime startTimeForTB = dt.Add(startTime);
                            DateTime endTimeForTB = dt.Add(endTime);
                            LSSSchedulingAPI.AddTimeBlock(Session["labServerGuid"].ToString(), Int32.Parse(ddlLabServers.SelectedValue), startTimeForTB, endTimeForTB, recurID);
                        }
                        dt = dt.AddDays(1);

                    }
                }


                string jScript;
                jScript = "<script language=javascript> window.opener.Form1.hiddenPopupOnNewTB.value='1';";
                jScript += "window.close();</script>";
                Page.RegisterClientScriptBlock("postbackScript", jScript);
                return;
            }
            catch (Exception ex)
            {
                string msg = "Exception: Cannot add the time block '" + ddlLabServers.SelectedItem.Text + " " + Session["labServerName"].ToString() + " " + txtStartDate.Text + " " + txtEndDate.Text + "'. " + ex.Message + ". " + ex.GetBaseException() + ".";
                lblErrorMessage.Text = iLabs.UtilLib.Utilities.FormatErrorMessage(msg);
                lblErrorMessage.Visible = true;
            }

        }

        private void report(Recurrence recur)
        {
            // tell whether the recurrence recur is overlapping with the input recurrence
            buf.Append("Conflict with recurrence ");
            buf.Append(recur.recurrenceId + ": " + recur.recurrenceType + " ");
            buf.Append(DateUtil.ToUserDate(recur.recurrenceStartDate, culture, userTZ) + "->");
            buf.Append(DateUtil.ToUserDate(recur.recurrenceEndDate, culture, userTZ) + " ");
            buf.Append(recur.recurrenceStartTime.Add(tzOffset) + " -- " + recur.recurrenceEndTime.Add(tzOffset));
            buf.AppendLine("<br />");
        }



    }

}