using System;
using System.Collections;

using iLabs.DataTypes;
using iLabs.DataTypes.SchedulingTypes;
using iLabs.UtilLib;

namespace iLabs.Scheduling.LabSide
{


  

    /// <summary>
    /// a structure which holds time block
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public struct TimeBlockInfo
    {
        /// <summary>
        /// the ID of the time block
        /// </summary>
        public int timeBlockId;
        /// <summary>
        /// the resourceID , the LabServer and resource the time block was assigned to. May not be needed see recurrenceID.
        /// </summary>
        public int resourceId;
        /// <summary>
        /// the start time of the time block, in UTC
        /// </summary>
        public DateTime startTime;
        /// <summary>
        /// the end time of the time block, in UTC
        /// </summary>
        public DateTime endTime;
        /// <summary>
        /// the GUID of the lab server that the time block belongs to. May not be needed see recurrenceID.
        /// </summary>
        public String labServerGuid;
        /// <summary>
        /// the ID of the recurrence that the time block belongs to
        /// </summary>
        public int recurrenceID;
    }

    /// <summary>
    /// a structure which holds recurrence
    /// </summary>
    //[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public class Recurrence
    {
        public const string NoRecurrence = "No recurrence";
        public const string Daily = "Daily";
        public const string Weekly = "Weekly";
        /// <summary>
        /// the ID of the recurrence
        /// </summary>
        public int recurrenceId;
        /// <summary>
        /// the resourceID , the labServer and resource the recurrence is assigned to.
        /// </summary>
        public int resourceId;
        /// <summary>
        /// the GUID of the lab server that the time block belongs to. May not be needed see resourceID.
        /// </summary>
        public String labServerGuid;
        /// <summary>
        /// the recurrenceType which can be "No recurrence", "Daily", "Weekly"
        /// </summary>
        public string recurrenceType;
        /// <summary>
        /// A mask defining the days of the week in the recurrence, see DateUtil DayMask
        /// </summary>
        public byte dayMask;
        /// <summary>
        /// the start date of the recurrence, 00:00 local time as UTC
        /// </summary>
        public DateTime recurrenceStartDate;
        /// <summary>
        /// the end date of the recurrence, 24:00(midnight) local time as UTC
        /// </summary>
        public DateTime recurrenceEndDate;
        /// <summary>
        /// the start time during a day in the recurrence, unit is second. This is system local time used as an offset from the startDate.
        /// </summary>
        public TimeSpan recurrenceStartTime;
        /// <summary>
        /// the send time during a day in the recurrence, unit is second. May be greater than midnight. This is system local time used as an offset from the startDate
        /// </summary>
        public TimeSpan recurrenceEndTime;

        public bool HasConflict(Recurrence recur)
        {
            bool status = false;
            switch (recurrenceType)
            {
                case Recurrence.NoRecurrence: // Single block always overlaps
                    switch (recur.recurrenceType)
                    {
                        case Recurrence.NoRecurrence: // Single block always overlaps
                        case Recurrence.Daily: //Daily block
                        case Recurrence.Weekly: // Weekly block, currently not supported
                            if(SpanIntersect(recur))
                                status = true;
                            break;
                        default:
                            break;
                    }
                    break;
                    case Recurrence.Daily: //Daily block
                    switch (recur.recurrenceType)
                    {
                        case Recurrence.NoRecurrence: // Single block always overlaps
                           if(SpanIntersect(recur))
                                status = true;
                            break;
                        case Recurrence.Daily: //Daily block
                        case Recurrence.Weekly: // Weekly block, currently not supported
                            if (SpanIntersect(recur))
                            {
                                if((recurrenceStartDate.TimeOfDay.Add(recurrenceStartTime) 
                                    < recur.recurrenceStartDate.TimeOfDay.Add(recur.recurrenceEndTime))
                                    && (recurrenceStartDate.TimeOfDay.Add(recurrenceEndTime)
                                        > recur.recurrenceStartDate.TimeOfDay.Add(recur.recurrenceStartTime)))
                                {
                                    status = true;
                                }
                            }  
                            break;
                        default:
                            break;
                    }
                    break;
                    case Recurrence.Weekly: // Weekly block, currently not supported
                    switch (recur.recurrenceType)
                    {
                        case Recurrence.NoRecurrence: // Single block always overlaps
                            if (SpanIntersect(recur))
                            {
                                status = true;
                            }
                            break;
                        case Recurrence.Daily: //Daily block
                            if (SpanIntersect(recur))
                            {
                                if ((recurrenceStartDate.TimeOfDay.Add(recurrenceStartTime)
                                    < recur.recurrenceStartDate.TimeOfDay.Add(recur.recurrenceEndTime))
                                    && (recurrenceStartDate.TimeOfDay.Add(recurrenceEndTime)
                                        > recur.recurrenceStartDate.TimeOfDay.Add(recur.recurrenceStartTime)))
                                {
                                    status = true;
                                }
                            }
                            break;
                        case Recurrence.Weekly: // Weekly block, currently not supported
                            if (SpanIntersect(recur))
                            {
                                if ((recurrenceStartDate.TimeOfDay.Add(recurrenceStartTime)
                                    < recur.recurrenceStartDate.TimeOfDay.Add(recur.recurrenceEndTime))
                                    && (recurrenceStartDate.TimeOfDay.Add(recurrenceEndTime)
                                        > recur.recurrenceStartDate.TimeOfDay.Add(recur.recurrenceStartTime)))
                                {
                                    if((dayMask & recur.dayMask) !=0)
                                        status = true;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return status;
        }

        public bool SpanIntersect(Recurrence recur){
            return ((recurrenceStartDate.Add(recurrenceStartTime) < recur.recurrenceEndDate.AddDays(-1).Add(recur.recurrenceEndTime))
                && (recurrenceEndDate.AddDays(-1).Add(recurrenceEndTime) > recur.recurrenceStartDate.Add(recur.recurrenceStartTime)))? true : false;
        }
    }

    /// <summary>
    /// a structure which holds experiment information
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public struct LssExperimentInfo
    {
        /// <summary>
        /// the ID of the experiment information
        /// </summary>
        public int experimentInfoId;
        /// <summary>
        /// the GUID of the lab Client 
        /// </summary>
        public string labClientGuid;
        /// <summary>
        /// the GUID of the lab server which provide the experiment
        /// </summary>
        public string labServerGuid;
        /// <summary>
        /// the Name of the lab server which provide the experiment
        /// </summary>
        public string labServerName;
        /// <summary>
        /// the name of the lab client through which the experiment can be executed
        /// </summary>
        public string labClientName;
        /// <summary>
        /// the version of the lab client through which the experiment can be executed
        /// </summary>
        public string labClientVersion;
        /// <summary>
        /// the name of the provider of the experiment
        /// </summary>
        public string providerName;
        /// <summary>
        /// the maximum divisor of the experiment's possible exection time
        /// </summary>
        public int quantum;
        /// <summary>
        /// the start up time needed for the execution of the experiment
        /// </summary>
        public int prepareTime;
        /// <summary>
        /// the cool down time needed after the execution of the experiment
        /// </summary>
        public int recoverTime;
        /// <summary>
        /// the experiment's minimum exection time
        /// </summary>
        public int minimumTime;
        /// <summary>
        /// the maxium time the user is allowed to arrive before the excution time of the experiment 
        /// </summary>
        public int earlyArriveTime;
    }
    /// <summary>
    /// a structure which holds Lab server side policy
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public struct LSSPolicy
    {
        /// <summary>
        /// the ID of the lab side scheduling server policy
        /// </summary>
        public int lssPolicyId;
        /// <summary>
        /// the ID of the credential set, the goup with which should obey this policy
        /// </summary>
        public int credentialSetId;
        /// <summary>
        /// the rule
        /// </summary>
        public string rule;
        /// <summary>
        /// the ID of the information of the experiment which the policy is applied to
        /// </summary>
        public int experimentInfoId;
    }

    public class LSResource{
        public int resourceID;
        public string labServerGuid;
        public string labServerName;
        public string description;
    }
    /// <summary>
    /// a structure which holds a permitted experiment for a time block
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public struct PermittedExperiment
    {
        /// <summary>
        /// the ID of the permission
        /// </summary>
        public int permittedExperimentId;
        /// <summary>
        /// the ID of the informaiton of the experiment which is permitted to be executed 
        /// </summary>
        public int experimentInfoId;
        /// <summary>
        /// the ID of the recurrence whose permission is given to the experiment
        /// </summary>
        public int recurrenceId;
    }

    /// <summary>
    /// a structure which holds user side scheduling information
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public struct USSInfo
    {
        /// <summary>
        /// the ID of the user side scheduling server information
        /// </summary>
        public int ussInfoId;
        public long couponId;
        public string domainGuid;
        /// <summary>
        /// the GUID of the user side scheduling server
        /// </summary>
        public string ussGuid;
        /// <summary>
        /// the name of the user side scheduling server
        /// </summary>
        public string ussName;
        /// <summary>
        ///  the URL of the user side scheduling server
        /// </summary>
        public string ussUrl;
        
    }

    /// <summary>
    /// a structure which holds credential set
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public struct LssCredentialSet
    {
        /// <summary>
        /// the ID of the credential set
        /// </summary>
        public int credentialSetId;
        /// <summary>
        /// the GUID of the service broker whose domain the group belongs to
        /// </summary>
        public string serviceBrokerGuid;
        /// <summary>
        /// the Name of the service broker whose domain the group belongs to
        /// </summary>
        public string serviceBrokerName;
        /// <summary>
        /// the Name of the goup with this credential set
        /// </summary>
        public string groupName;
        /// <summary>
        /// the GUID of the user side scheduling server one which the group registered   
        /// </summary>
        public string ussGuid;
    }
    /// <summary>
    /// a structure which holds reservation information
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ilab.mit.edu/iLabs/type")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://ilab.mit.edu/iLabs/type", IsNullable = false)]
    public struct ReservationInfo
    {
        /// <summary>
        /// the ID of the reservation information
        /// </summary>
        public int reservationInfoId;
        /// <summary>
        /// the ID of the credentialSet. the user from the group with this credential set made the reservation
        /// </summary>
        public int credentialSetId;
        /// <summary>
        /// the start time of the reservation
        /// </summary>
        public DateTime startTime;
        /// <summary>
        /// the end time of the reservation
        /// </summary>
        public DateTime endTime;
        /// <summary>
        /// the ID of the information of the experiment which is reserved for execution
        /// </summary>
        public int experimentInfoId;
    }
	/// <summary>
	/// Summary description for LSSSchedulingAPI.
	/// </summary>
	public class LSSSchedulingAPI
	{
		public LSSSchedulingAPI()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR ExperimentInfo
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add information of particular experiment
		/// </summary>
        /// <param name="labClientGuid"></param>
		/// <param name="labServerGuid"></param>
		/// <param name="labServerName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="labClientName"></param>
		/// <param name="providerName"></param>
		/// <param name="quantum"></param>
		/// <param name="prepareTime"></param>
		/// <param name="recoverTime"></param>
		/// <param name="minimumTime"></param>
		/// <param name="earlyArriveTime"></param>
		/// <returns></returns>the unique ID which identifies the experiment information added, >0 was successfully added, ==-1 otherwise
        public static int AddExperimentInfo( string labServerGuid, string labServerName,
            string labClientGuid, string labClientName, string labClientVersion, string providerName, int quantum, int prepareTime, int recoverTime, int minimumTime, int earlyArriveTime)
		{
            int experimentInfoID = DBManager.AddExperimentInfo(labServerGuid, labServerName, labClientGuid, labClientName, labClientVersion, providerName, quantum, prepareTime, recoverTime, minimumTime, earlyArriveTime);
				return experimentInfoID;		
		}
		/// <summary>
		/// delete the experiment information specified by the experimentInfoIDs
		/// </summary>
		/// <param name="experimentInfoIDs"></param>
		/// <returns></returns>an array of ints containing the IDs of all experiment information not successfully removed
		public static int[] RemoveExperimentInfo(int[] experimentInfoIDs)
		{
			int[] uIDs=DBManager.RemoveExperimentInfo(experimentInfoIDs);
			return uIDs;
		}
		/// <summary>
		/// Return an array of the immutable USSInfo objects thta correspond to the supplied USS information IDs
		/// </summary>
		/// <param name="experimentInfoIDs"></param>
		/// <returns></returns>
		public static LssExperimentInfo[] GetExperimentInfos(int[] experimentInfoIDs)
		{
			LssExperimentInfo[] experimentInfos=DBManager.GetExperimentInfos(experimentInfoIDs);
			return experimentInfos;
		}
		/// <summary>
		/// update the data fields for the experiment information specified by the experimentInfoID
		/// </summary>
		/// <param name="experimentInfoID"></param>
		/// <param name="labServerGuid"></param>
		/// <param name="labServerName"></param>
        /// <param name="labClientName"></param>
        /// <param name="labClientVersion"></param>
		/// <param name="providerName"></param>
		/// <param name="quantum"></param>
		/// <param name="prepareTime"></param>
		/// <param name="recoverTime"></param>
		/// <param name="minimumTime"></param>
		/// <param name="earlyArriveTime"></param>
		/// <returns></returns>true if modified successfully, falso otherwise
        public static bool ModifyExperimentInfo(int experimentInfoID, string labServerGuid, string labServerName,
             string labClientGuid, string labClientName, string labClientVersion, string providerName, int quantum, int prepareTime, int recoverTime, int minimumTime, int earlyArriveTime)
		{
            bool i = DBManager.ModifyExperimentInfo(experimentInfoID, labServerGuid, labServerName, labClientGuid, labClientName, labClientVersion, providerName, quantum, prepareTime, recoverTime, minimumTime, earlyArriveTime);
		    return i;
		}

        /// <summary>
        /// get the labserver name according to the labserver ID
        /// </summary>
        /// <param name="labServer/// <param name="labClientName"></param>"></param>
        /// <returns></returns>
        public static string RetrieveLabServerName(string labServerGuid)
        {
            string labServerName = DBManager.RetrieveLabServerName(labServerGuid);
            return labServerName;
        }
		
		/// <summary>
		/// enumerates IDs of the information of all the experiments belonging to certain lab server identified by the labserverID
		/// </summary>
		/// <param name="labServerGuid"></param>
		/// <returns></returns>an array of ints containing the IDs of the information of all the experiments belonging to specified lab server
		public static int[] ListExperimentInfoIDsByLabServer(string labServerGuid)
		{
			int[] eIDs=DBManager.ListExperimentInfoIDsByLabServer(labServerGuid);
			return eIDs;
		}
		/// <summary>
		/// retrieve the ids of all the experiment information
		/// </summary>
		/// <returns></returns>an array of ints containing the IDs of the information of all the experiments 
		public static int[] ListExperimentInfoIDs()
		{
			int[] eIDs=DBManager.ListExperimentInfoIDs();
			return eIDs;
		}
		/// <summary>
		/// enumerates the ID of the information of a particular experiment specified by labClientName and labClientVersion
		/// </summary>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <returns></returns>the ID of the information of a particular experiment, -1 if such a experiment info can not be retrieved
		public static int ListExperimentInfoIDByExperiment(string clientGuid, string labServerGuid)
		{
			int i=DBManager.ListExperimentInfoIDByExperiment(clientGuid, labServerGuid);
			return i;
		}

        /* !------------------------------------------------------------------------------!
			 *							CALLS FOR LS Resource
			 * !------------------------------------------------------------------------------!
			 */

        public static int CheckForLSResource(string guid, string name)
        {
            return DBManager.CheckForLSResource(guid, name);
        }
        public static LSResource GetLSResource(int id)
        {
            return DBManager.GetLSResource(id);
        }

        public static LSResource GetLSResource(string guid)
        {
            return DBManager.GetLSResource(guid);
        }

        public static IntTag[] GetLSResourceTags()
        {
            return DBManager.GetLSResourceTags();
        }

        public static IntTag[] GetLSResourceTags(string guid)
        {
            return DBManager.GetLSResourceTags(guid);
        }


		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR Time Block
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add a time block in which users with a particular credential set are allowed to access a particular lab server
		/// </summary>
		/// <param name="labServerGuid"></param>
		/// <param name="credentialSetID"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>the uniqueID which identifies the time block added, >0 was successfully added; ==-1 otherwise
		public static int AddTimeBlock(string labServerGuid,int resourceID, DateTime startTime, DateTime endTime, int recurrenceID)
		{
			int i=DBManager.AddTimeBlock(labServerGuid, resourceID, startTime, endTime, recurrenceID);
			return i;
		}
		
		/// <summary>
		/// delete the time blocks specified by the timeBlockIDs
		/// </summary>
		/// <param name="timeBlockIDs"></param>
		/// <returns></returns>an array of ints containning the IDs of all time blocks not successfully removed
		public static int[] RemoveTimeBlock(int[] timeBlockIDs)
		{
			int[] tbIDs=DBManager.RemoveTimeBlock(timeBlockIDs);
			return tbIDs;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeBlockID"></param>
		/// <param name="labServerGuid"></param>
		/// <param name="credentialSetID"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns> true if updated sucessfully, fals otherwise
		public static bool ModifyTimeBlock(int timeBlockID, string labServerGuid,int resourceID, DateTime startTime, DateTime endTime)
		{
			bool i=DBManager.ModifyTimeBlock(timeBlockID, labServerGuid, resourceID, startTime,  endTime);
            return i;
		}
		/*
		/// <summary>
		/// enumerates all IDs of the time blocks belonging to a particular lab server identified by the labserverID
		/// </summary>
		/// <param name="labServerGuid"></param>
		/// <returns></returns>an array of ints containing the IDs of all the time blocks of specified lab server
		public static int[] ListTimeBlockIDsByLabServer(string labServerGuid)
		{
			int[] tbIDs=DBManager.ListTimeBlockIDsByLabServer(labServerGuid);
			return tbIDs;
		}
         * */
        /*
		/// <summary>
		/// enumerates all IDs of the time blocks during which the members of a particular group identified by the credentialSetID are allowed to access a particular lab server identified by the labServerID
		/// </summary>
		/// <param name="labServerGuid"></param>
		/// <param name="credentialSetID"></param>
		/// <returns></returns>an array of ints containing the IDs of all the time blocks during which the members of a particular grou[ are allowed to access a particular lab server
		public static int[] ListTimeBlockIDsByGroup(string labServerGuid,int credentialSetID)
		{
			int[] tbIDs=DBManager.ListTimeBlockIDsByGroup(labServerGuid,credentialSetID);
			return tbIDs;
		}
         * */
		/// <summary>
		/// list the IDs of a particular lab server's time blocks which are assigned to particular group in the time chunk defined by the start time and end time
		/// </summary>
		/// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <param name="ussGuid"></param>
		/// <param name="labServerGuid"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>
		public static int[] ListTimeBlockIDsByTimeChunk(string serviceBrokerGuid,string groupName, string ussGuid,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
			int[] tbIDs=DBManager.ListTimeBlockIDsByTimeChunk(serviceBrokerGuid, groupName, ussGuid, clientGuid, labServerGuid, startTime, endTime);
			return tbIDs;
		}
		/// Enumerates the IDs of the information of all the time blocks 
		/// </summary>
		/// <returns></returns>the array  of ints contains the IDs of the information of all the time blocks
		public static int[] ListTimeBlockIDs()
		{
			int[] tbIDs=DBManager.ListTimeBlockIDs();
			return tbIDs;
		}
		/// <summary>
		/// Returns an array of the immutable TimeBlockInfo objects that correspond ot the supplied time block IDs
		/// </summary>
		/// <param name="timeBlockIDs"></param>
		/// <returns></returns>an array of immutable objects describing the specified time blocks
		public static TimeBlock[] GetTimeBlocks(int[] timeBlockIDs)
		{
			TimeBlock[] tbs=DBManager.GetTimeBlocks(timeBlockIDs);
			return tbs;
		}
		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR ReservationInfo
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add reservation information
		/// </summary>
		/// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <param name="ussGuid"></param>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>the unique ID identifying the reservation information added, >0 successfully added, -1 otherwise
		public static int AddReservationInfo(string serviceBrokerGuid, string groupName, string ussGuid,
            string clientGuid,string labServerGuid, DateTime startTime, DateTime endTime)
		{
			return DBManager.AddReservationInfo(serviceBrokerGuid, groupName, ussGuid, clientGuid, labServerGuid, startTime, endTime);
		}
		/// <summary>
		/// add reservationInfo
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <param name="credentialSetID"></param>
		/// <param name="experimentInfoID"></param>
		/// <returns></returns>the unique ID identifying the reservation information added, >0 successfully added, -1 otherwise
		public static int AddReservationInfo(DateTime startTime, DateTime endTime,int credentialSetID, int experimentInfoID)
		{
			return DBManager.AddReservationInfo( startTime,  endTime, credentialSetID, experimentInfoID);
		}
		
		
		/// <summary>
		/// delete the reservation information specified by the reservationInfoIDs
		/// </summary>
		/// <param name="reservationInfoIDs"></param>
		/// <returns></returns>an array of ints containning the IDs of all reservation information not successfully removed
		public static int[] RemoveReservationInfoByIDs(int[] reservationInfoIDs)
		{
			int[] rIDs = DBManager.RemoveReservationInfoByIDs(reservationInfoIDs);
			return rIDs;
		}
        
		/// <summary>
		/// remove the reservation information
		/// </summary>
		/// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <param name="ussGuid"></param>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>true remove successfully, false otherwise
		public static bool RemoveReservationInfo(string serviceBrokerGuid, string groupName, string ussGuid, 
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
			bool removed = DBManager.RemoveReservationInfo(serviceBrokerGuid, groupName,  ussGuid,
                clientGuid, labServerGuid, startTime, endTime);
			return removed;
		}
		
		/// <summary>
		/// enumerates all IDs of the reservations made to a particular experiment identified by the experimentInfoID
		/// </summary>
		/// <param name="experimentInfoID"></param>
        /// <returns>an array of ints containing the IDs of all the reservation information made to the specified experiment</returns>
		public static int[] ListReservationInfoIDsByExperiment(int experimentInfoID)
		{
			int[] rIDs = DBManager.ListReservationInfoIDsByExperiment(experimentInfoID);
			return rIDs;
		}
		/// <summary>
		/// enumerates all IDs of the reservations made to a particular experiment from a particular group 
        /// between the start time and the end time
		/// </summary>
		/// <param name="serviceBrokerGuid"></param>
		/// <param name="groupName"></param>
		/// <param name="ussGuid"></param>
		/// <param name="labClientName"></param>
		/// <param name="labClientVersion"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>
		public static int[] ListReservationInfoIDs(string serviceBrokerGuid,string groupName, string ussGuid,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
            int[] rIDs = DBManager.ListReservationInfoIDs(serviceBrokerGuid, groupName, ussGuid,
                clientGuid, labServerGuid, startTime, endTime);
			return rIDs;
		}
		/// <summary>
		/// retrieve reservation made to a particular labserver during a given time chunk.
		/// </summary>
		/// <param name="labServerGuid"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		/// <returns></returns>
		public static int[] ListReservationInfoIDsByLabServer(string labServerGuid, DateTime startTime, DateTime endTime)
		{
			int[] rIDs = DBManager.ListReservationInfoIDsByLabServer(labServerGuid, startTime, endTime);
			return rIDs;
		}

		/// <summary>
		/// to select reservation Infos accorrding to given criterion
		/// </summary>
		public static ReservationInfo[] SelectReservationInfo(string labServerGuid, int experimentInfoID,
            int credentialSetID, DateTime timeAfter, DateTime timeBefore)
		{
			ReservationInfo[] reservationInfos = DBManager.SelectReservationInfo(labServerGuid, experimentInfoID, 
                credentialSetID, timeAfter, timeBefore);
			return reservationInfos;
		}
		
		/// <summary>
		/// returns an array of the immutable ReservationInfo objects that correspond to the supplied reservationInfoIDs
		/// </summary>
		/// <param name="reservationInfoIDs"></param>
		/// <returns></returns>an array ofimmutable objects describing the specified reservations
 
		public static ReservationInfo[] GetReservationInfos(int[] reservationInfoIDs)
		{
			ReservationInfo[] reservationInfos = DBManager.GetReservationInfos(reservationInfoIDs);
			return reservationInfos;
		}
		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR CredentialSet
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add a credential set of a particular group
		/// </summary>
		/// <param name="serviceBrokerID"></param>
		/// <param name="groupName"></param>
		/// <param name="ussID"></param>
		/// <returns></returns>the unique ID which identifies the credential set added.>0 was successfully added,-1 otherwise
		public static int AddCredentialSet(string serviceBrokerGuid,string serviceBrokerName, string groupName, string ussGuid)
		{
            int[] credentialSetIDs = LSSSchedulingAPI.ListCredentialSetIDs();
            LssCredentialSet[] credentialSets = LSSSchedulingAPI.GetCredentialSets(credentialSetIDs);

            foreach (LssCredentialSet set in credentialSets)
            {
                if (set.serviceBrokerGuid.Equals(serviceBrokerGuid) && set.serviceBrokerName.Equals(serviceBrokerName) &&
                    set.groupName.Equals(groupName) && set.ussGuid.Equals(ussGuid))
                {
                    return -1;
                }
                    
            }

			int cID=DBManager.AddCredentialSet(serviceBrokerGuid,serviceBrokerName, groupName, ussGuid);	
			return cID;
		}
		/// <summary>
		/// Updates the data fields for the credential set specified by the credentialSetID; note credentialSetID may not be changed 
		/// </summary>
		/// <param name="credentialSetID"></param>
		/// <param name="serviceBrokerGuid"></param>
		/// <param name="serviceBrokerName"></param>
		/// <param name="groupName"></param>
		/// <param name="ussGuid"></param>
		/// <returns></returns>if modified successfully, false otherwise
		public static bool ModifyCredentialSet(int credentialSetID, string serviceBrokerGuid, string serviceBrokerName, string groupName, string ussGuid)
		{
			bool i=DBManager.ModifyCredentialSet(credentialSetID, serviceBrokerGuid,serviceBrokerName, groupName,  ussGuid);
			return i;
			   
		}
		/// <summary>
		///  remove a credential set specified by the credentialsetsIDS
		/// </summary>
		/// <param name="credentialSetIDs"></param>
		/// <returns></returns>An array of ints containing the IDs of all credential sets not successfully removed, i.e., those for which the operation failed. 
		public static int[] RemoveCredentialSets(int[] credentialSetIDs)
		{
		     int[] cIDs=DBManager.RemoveCredentialSets(credentialSetIDs);
			return cIDs;
		}
		/// <summary>
		/// Enumerates the IDs of the information of all the credential set 
		/// </summary>
		/// <returns></returns>the array  of ints contains the IDs of all the credential set
		public static int[] ListCredentialSetIDs()
		{
			int[] cIDs=DBManager.ListCredentialSetIDs();
			return cIDs;
			
		}
          /// <summary>
        /// remove a credential set of a particular group
        /// </summary>
        /// <param name="serviceBrokerGuid"></param>
        /// <param name="groupName"></param>
        /// <param name="ussGuid"></param>
        /// <returns></returns>true, the credentialset is removed successfully, false otherwise
        public static bool RemoveCredentialSet(string serviceBrokerGuid, string serviceBrokerName, string groupName, string ussGuid)
        {
            return DBManager.RemoveCredentialSet(serviceBrokerGuid, serviceBrokerName, groupName, ussGuid);
        }
		/// <summary>
		/// Returns an array of the immutable Credential objects that correspond to the supplied credentialSet IDs. 
		/// </summary>
		/// <param name="credentialSetIDs"></param>
		/// <returns></returns>An array of immutable objects describing the specified Credential Set information; if the nth credentialSetID does not correspond to a valid experiment scheduling property, the nth entry in the return array will be null.
		public static LssCredentialSet[] GetCredentialSets(int[] credentialSetIDs)
		{
			LssCredentialSet[] credentialSets=DBManager.GetCredentialSets(credentialSetIDs);
			return credentialSets;
		}
        /// <summary>
        /// Get the credential set ID of a particular group
        /// </summary>
        /// <param name="serviceBrokerID"></param>
        /// <param name="groupName"></param>
        /// <param name="ussID"></param>
        /// <returns></returns>the unique ID which identifies the credential set added.>0 was successfully added,-1 otherwise
        public static int GetCredentialSetID(string serviceBrokerGuid, string groupName, string ussGuid)
        {
            int credentialSetID = DBManager.GetCredentialSetID(serviceBrokerGuid, groupName, ussGuid);
           
            return credentialSetID;
        }
		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR USSInfo
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// Add information of a particular user side scheduling server identified by ussID 
		/// </summary>
		/// <param name="ussGuid"></param>
		/// <param name="ussName"></param>
		/// <param name="ussURL"></param>
        /// <param name="couponID">ID of the RevokeReservation ticket Coupon, coupon should be in the database</param>
        /// <param name="issuerGuid">Issuer of the RevokeReservation ticket Coupon</param>
		/// <returns></returns>The unique ID which identifies the experiment information added. >0 was successfully added; ==-1 otherwise   
		public static int AddUSSInfo(string ussGuid, string ussName, string ussURL,long couponID,string issuerGuid)
		{
            int[] ussInfoIDs = ListUSSInfoIDs();
            USSInfo[] ussInfo = GetUSSInfos(ussInfoIDs);

            foreach (USSInfo info in ussInfo)
            {
                if (ussGuid.Equals(info.ussGuid))
                    return -1;
            }

			int uID=DBManager.AddUSSInfo(ussGuid,ussName,ussURL,couponID,issuerGuid);
			return uID;
		}

		/// <summary>
		/// Updates the data fields for the USS information specified by the ussInfoID; note ussInfoID may not be changed 
		/// </summary>
		/// <param name="ussInfoID"></param>
		/// <param name="ussID"></param>
		/// <param name="ussName"></param>
		/// <param name="ussURL"></param>
		/// <returns></returns>true if modified successfully, false otherwise
        public static bool ModifyUSSInfo(int ussInfoID, string ussGUID, string ussName, string ussURL, long couponID, string issuerGuid)
		{
			bool i=DBManager.ModifyUSSInfo(ussInfoID, ussGUID,  ussName,  ussURL,couponID,issuerGuid);
			return i;
			    
		}
		
		/// <summary>
		/// Delete the uss information specified by the ussInfoIDs. 
		/// </summary>
		/// <param name="ussInfoIDs"></param>
		/// <returns></returns>An array of USS information IDs specifying the USS information to be removed
		public static int[] RemoveUSSInfo(int[] ussInfoIDs)
		{
				int[] uIDs=DBManager.RemoveUSSInfo(ussInfoIDs);
			    return uIDs;
		}

		/// <summary>
		/// Enumerates the IDs of the information of all the USS 
		/// </summary>
		/// <returns></returns>the array  of ints contains the IDs of the information of all the USS
		public static int[] ListUSSInfoIDs()
		{
			int[] uIDs=DBManager.ListUSSInfoIDs();
			return uIDs;
			
		}
		/// <summary>
		/// Enumerates the ID of the information of a particular USS specified by ussID
		/// </summary>
		/// <param name="ussGuid"></param>
		/// <returns></returns>the ID of the information of a particular USS 
		public static int ListUSSInfoID(string ussGuid)
		{
			int i=DBManager.ListUSSInfoID(ussGuid);
			return i;
			
		}
		
		/// <summary>
		/// Returns an array of the immutable USSInfo objects that correspond to the supplied USS information IDs. 
		/// </summary>
		/// <param name="ussInfoIDs"></param>
		/// <returns></returns>An array of immutable objects describing the specified USS information; if the nth ussInfoID does not correspond to a valid experiment scheduling property, the nth entry in the return array will be null
		public static USSInfo[] GetUSSInfos(int[] ussInfoIDs)
		{
			USSInfo[] ussInfos=DBManager.GetUSSInfos(ussInfoIDs);
			return ussInfos;
			
		}

		/* !------------------------------------------------------------------------------!
			 *							CALLS FOR Permited Experiment
			 * !------------------------------------------------------------------------------!
			 */
		/// <summary>
		/// add permission of a particular experiment being executed in a particular time block
		/// </summary>
		/// <param name="experimentInfoID"></param>
		/// <param name="timeBlockID"></param>
		/// <returns></returns>the unique ID which identifies the permission added. >0 was successfully added;==-1 otherwise
		public static int AddPermittedExperiment(int experimentInfoID, int recurrenceID)
		{
			int i=DBManager.AddPermittedExperiment(experimentInfoID, recurrenceID);
			return i;
		}
		
		/// <summary>
		/// delete permission of  a particular experiment being executed in a particular time block
		/// </summary>
		/// <param name="permittedExperimentIDs"></param>
		/// <returns></returns>an array of ints containing the IDs of all permissions not successfully removed
		public static int[] RemovePermittedExperiments(int[] permittedExperimentIDs, int recurrenceID)
		{
            int[] ids = DBManager.RemovePermittedExperiments(permittedExperimentIDs, recurrenceID);
			return ids;
		}
		
		/// <summary>
		/// enumerates the IDs of the information of the permitted experiments for a particular time block identified by the timeBlockID
		/// </summary>
		/// <param name="timeBlockID"></param>
		/// <returns></returns>an array of ints containing the IDs of the information of the permitted experiments for a particular time block identified by the timeBlockID
		public static int[] ListPermittedExperimentInfoIDsByTimeBlock(int timeBlockID)
		{
			int[] ids=DBManager.ListPermittedExperimentInfoIDsByTimeBlock(timeBlockID);
			return ids;
		}
		/// <summary>
		/// returns an array of the immutable PermittedExperiment objects that correspond to the supplied permittedExperimentIDs
		/// </summary>
		/// <param name="permittedExperimentIDs"></param>
		/// <returns></returns>an array of immutable objects describing the specified PermittedExperiments
		public static PermittedExperiment[] GetPermittedExperiments(int[] permittedExperimentIDs)
		{
			PermittedExperiment[] exps=DBManager.GetPermittedExperiments(permittedExperimentIDs);
			return exps;
		}
		/// <summary>
		/// retrieve unique ID of the PerimmttiedExperiment which represents the permission of executing a particular experiment in a particular time block
		/// </summary>
		/// <param name="experimentInfoID"></param>
		/// <param name="timeBlockID"></param>
		/// <returns></returns>-1 if the permission can not be retrieved
		public static int ListPermittedExperimentID(int experimentInfoID, int timeBlockID)
		{
			int i = DBManager.ListPermittedExperimentID(experimentInfoID, timeBlockID);
			return i;
		}
         /// <summary>
        /// retrieve unique ID of the PerimmttiedExperiment which represents the permission of executing a particular experiment in a particular recurrence
        /// </summary>
        /// <param name="experimentInfoID"></param>
        /// <param name="recurrenceID"></param>
        /// <returns></returns>-1 if the permission can not be retrieved
        public static int ListPermittedExperimentIDByRecur(int experimentInfoID, int recurrenceID)
        {
            int i = DBManager.ListPermittedExperimentIDByRecur(experimentInfoID, recurrenceID);
            return i;
        }

		/// <summary>
		/// check whether the permission a particular time block has been given to a particular experiment  
		/// </summary>
		/// <param name="experimentInfoID"></param>
		/// <param name="timeBlockID"></param>
		/// <returns></returns>true the the permission exists. false other wise
		public static bool CheckPermission(int experimentInfoID, int timeBlockID)
		{
			bool i;

			if (DBManager.ListPermittedExperimentID(experimentInfoID, timeBlockID)==-1)
			{
				i=false;
			}
			else
			{
				i=true;
			}
			return i;

		}
          /// <summary>
        /// enumerates the IDs of information of the permitted experiments for a particular recurrence identified by the recurrenceID
        /// </summary>
        /// <param name="recurrenceID"></param>
        /// <returns></returns>an array of ints containing the IDs of the information of the permitted experiments for a particular recurrence identified by the recurrenceID
        public static int[] ListExperimentInfoIDsByRecurrence(int recurrenceID)
        {
            int[] ids = DBManager.ListExperimentInfoIDsByRecurrence(recurrenceID);
            return ids;
        }

        /* !------------------------------------------------------------------------------!
			 *							CALLS FOR Permited CredentialSet
			 * !------------------------------------------------------------------------------!
			 */
        /// <summary>
        /// add permission of a particular experiment being executed in a particular time block
        /// </summary>
        /// <param name="experimentInfoID"></param>
        /// <param name="timeBlockID"></param>
        /// <returns></returns>the unique ID which identifies the permission added. >0 was successfully added;==-1 otherwise
        public static int AddPermittedCredentialSet(int credentialSetID, int recurrenceID)
        {
            int i = DBManager.AddPermittedCredentialSet(credentialSetID, recurrenceID);
            return i;
        }

        /// <summary>
        /// delete permission of  a particular CredentialSet in a particular time block
        /// </summary>
        /// <param name="permittedExperimentIDs"></param>
        /// <returns></returns>an array of ints containing the IDs of all permissions not successfully removed
        public static int[] RemovePermittedCredentialSets(int[] permittedCredentialSetIDs, int recurrenceID)
        {
            int[] ids = DBManager.RemovePermittedCredentialSets(permittedCredentialSetIDs, recurrenceID);
            return ids;
        }

        /// <summary>
        /// enumerates the IDs of the information of the permitted experiments for a particular time block identified by the timeBlockID
        /// </summary>
        /// <param name="timeBlockID"></param>
        /// <returns></returns>an array of ints containing the IDs of the information of the permitted experiments for a particular time block identified by the timeBlockID
        public static int[] ListPermittedCredentialSetIDsByTimeBlock(int timeBlockID)
        {
            int[] ids = DBManager.ListCredentialSetIDsByTimeBlock(timeBlockID);
            return ids;
        }
        /*
        /// <summary>
        /// returns an array of the immutable PermittedExperiment objects that correspond to the supplied permittedExperimentIDs
        /// </summary>
        /// <param name="permittedExperimentIDs"></param>
        /// <returns></returns>an array of immutable objects describing the specified PermittedExperiments
        public static LssCredentialSet[] GetPermittedCredentialSets(int[] permittedCredentialSetIDs)
        {
            LssCredentialSet[] exps = DBManager.GetPermittedCredentialSets(permittedCredentialSetIDs);
            return exps;
        }
        /// <summary>
        /// retrieve unique ID of the PerimmttiedExperiment which represents the permission of executing a particular experiment in a particular time block
        /// </summary>
        /// <param name="experimentInfoID"></param>
        /// <param name="timeBlockID"></param>
        /// <returns></returns>-1 if the permission can not be retrieved
        public static int ListPermittedCredentialSetID(int credentialSetID, int timeBlockID)
        {
            int i = DBManager.ListCredentialSetID(credentialSetID, timeBlockID);
            return i;
        }
        /// <summary>
        /// retrieve unique ID of the PerimmttiedExperiment which represents the permission of executing a particular experiment in a particular recurrence
        /// </summary>
        /// <param name="experimentInfoID"></param>
        /// <param name="recurrenceID"></param>
        /// <returns></returns>-1 if the permission can not be retrieved
        public static int ListPermittedCredentialSetIDByRecur(int credentialSetID, int recurrenceID)
        {
            int i = DBManager.ListPermittedCredentialSetIDByRecur(credentialSetID, recurrenceID);
            return i;
        }
         * */

        /// <summary>
        /// check whether the permission to a particular time block has been given to a CredentialSet 
        /// </summary>
        /// <param name="experimentInfoID"></param>
        /// <param name="timeBlockID"></param>
        /// <returns></returns>true the the permission exists. false other wise
        public static bool CheckGroupPermission(int credentialSetID, int timeBlockID)
        {
            return DBManager.IsPermittedCredentialSet(credentialSetID, timeBlockID);
          
        }
        /// <summary>
        /// enumerates the IDs of information of the permitted experiments for a particular recurrence identified by the recurrenceID
        /// </summary>
        /// <param name="recurrenceID"></param>
        /// <returns></returns>an array of ints containing the IDs of the information of the permitted groups for a particular recurrence identified by the recurrenceID
        public static int[] ListCredentialSetIDsByRecurrence(int recurrenceID)
        {
            int[] ids = DBManager.ListCredentialSetIDsByRecurrence(recurrenceID);
            return ids;
        }


/// <summary>
/// retrieve available time periods(local time of LSS) overlaps with a time chrunk for a particular group and particular experiment, so that we get the a serials available time periods
/// which is the minumum available time periods set covering the time chunk
/// </summary>
/// <param name="serviceBrokeGUID"></param>
/// <param name="groupName"></param>
/// <param name="ussGUID"></param>
/// <param name="labClientName"></param>
/// <param name="labClientVersion"></param>
/// <param name="startTime"></param>the local time of LSS
/// <param name="endTime"></param>the local time of LSS
/// <returns></returns>return an array of time periods (local time), each of the time periods is longer than the experiment's minimum time 
		public static TimePeriod[] RetrieveAvailableTimePeriods(string serviceBrokerGUID,string groupName, string ussGUID,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
			try
			{   
				DateTime startTimeUTC = startTime.ToUniversalTime();
				DateTime endTimeUTC = endTime.ToUniversalTime();
				//get the experiment configuration
                int eID=DBManager.ListExperimentInfoIDByExperiment(clientGuid, labServerGuid);
				if(eID < 0)
				{
                     throw new Exception("exception throw in retrieving available time periods, the experiment can not be found"); 
				}
				LssExperimentInfo exInfo=GetExperimentInfos(new int[] {eID})[0];
				int preTime=exInfo.prepareTime;
				int recTime=exInfo.recoverTime;
				int minimumTime = exInfo.minimumTime;
				string labServerGUID=exInfo.labServerGuid;
				//get the timeblocks which are the minimum time blocks set covering the time chunk for the lab server where the experiment is executed
				int[] tIDs=ListTimeBlockIDsByTimeChunk(serviceBrokerGUID,groupName, ussGUID, clientGuid, labServerGuid, startTimeUTC, endTimeUTC);
				//get the IDs timeblocks in the time chunk which has been given the permission to the experiment for particular group
				ArrayList arrayListTIDs=new ArrayList();
				for(int i=0; i<tIDs.Length;i++)
				{
					if (CheckPermission(eID,tIDs[i]))
					{
						arrayListTIDs.Add(tIDs[i]);
					}
				}
				
				int[] availTIDs=Utilities.ArrayListToIntArray(arrayListTIDs);
				//get the  time blocks which are the minimum time blocks set covering the time chunk, and has been given the permission to the experiment for particular group
				TimeBlock[] availTBs=DBManager.GetTimeBlocks(availTIDs);
				
				//get the reservations  made for the lab server whose experiment time including the warm up and cool down time overlap the time chunk 
                int[] rIDs = DBManager.ListReservationInfoIDsByLabServer(labServerGUID, startTimeUTC.AddMinutes(-preTime), endTimeUTC.AddMinutes(recTime));	
				
                //Check whar happens when no prior reservations
                ReservationInfo[] rInfos=DBManager.GetReservationInfos(rIDs);
				// get the unavailable time periods(UTC) which overlap the time chunk define by the start time and end time
				ArrayList unavailTPs=new ArrayList();
				foreach(ReservationInfo rInfo in rInfos)
				{
					TimePeriod tp=new TimePeriod();
					tp.startTime=rInfo.startTime.AddMinutes(-preTime);
                    tp.endTime=rInfo.endTime.AddMinutes(recTime);
					unavailTPs.Add(tp);
				}
				// get the free time periods(UTC) during the time chunk define by the start time and end time
				ArrayList freeTPs= new ArrayList();
                int len=unavailTPs.Count;
				if (len==0)
				{
					TimePeriod atp=new TimePeriod();
					atp.startTime=startTimeUTC;
					atp.endTime =endTimeUTC;
					freeTPs.Add(atp);
				}
				else{
					if (((TimePeriod)unavailTPs[0]).startTime>startTimeUTC)
					{
						TimePeriod atp=new TimePeriod();
						atp.startTime=startTimeUTC;
						atp.endTime =((TimePeriod)unavailTPs[0]).startTime;
						freeTPs.Add(atp);
					}
					
					if (((TimePeriod)unavailTPs[len-1]).endTime<endTimeUTC)
					{
						TimePeriod atp=new TimePeriod();
						atp.startTime=((TimePeriod)unavailTPs[len-1]).endTime;
						atp.endTime =endTimeUTC;
						freeTPs.Add(atp);
					}
					if(len>1)
					{
						for(int i=0;i<len-1;i++)
						{
							TimePeriod atp=new TimePeriod();
							atp.startTime=((TimePeriod)unavailTPs[i]).endTime;
							atp.endTime =((TimePeriod)unavailTPs[i+1]).startTime;
							freeTPs.Add(atp);
						}
					}
				}
				// get the available time periods(UTC)
				ArrayList availTPs=new ArrayList();
				for(int i=0; i<availTBs.Length;i++)
				{
					//get the free time periods which are overlap with each available time block
					ArrayList arrayListTPs=new ArrayList();
					foreach(TimePeriod tp in freeTPs)
					{
						if (tp.endTime>availTBs[i].startTime && tp.startTime<availTBs[i].endTime)
						{
							arrayListTPs.Add(tp);
						}
					}
					// intersect the time block with the corresponding free time periods,and turn the result to local time.
			     	int lenTP=arrayListTPs.Count;
					if (lenTP!=0)
					{
						foreach(TimePeriod atp in arrayListTPs)
						{
							TimePeriod avTP=new TimePeriod();
							if (atp.startTime<availTBs[i].startTime)
							{
								avTP.startTime=availTBs[i].startTime;
							}
							else
							{
								avTP.startTime=atp.startTime;
							}
							if (atp.endTime>availTBs[i].endTime)
							{
								avTP.endTime=availTBs[i].endTime;
							}
							else
							{
								avTP.endTime=atp.endTime;
							}
                            availTPs.Add(avTP);
						}
					}

				}
				// get the available timeperiod which is longer than the minimum time the experiment needed, 
				ArrayList availLongTPs=new ArrayList();
                int minTime = minimumTime + preTime + recTime;
				foreach (TimePeriod tp in availTPs)
				{
                    
					if ((tp.endTime.Subtract(tp.startTime)).TotalMinutes >= minTime)
					{
                       availLongTPs.Add(tp);
					}
				}
				//sort the timeperiod by time
				ArrayList availSortedLongTPs = TimePeriod.SortByTime(availLongTPs);
				TimePeriod[] tpArray = new TimePeriod[availLongTPs.Count];
				
				for(int i=0; i< availSortedLongTPs.Count;i++)
				{
                  tpArray[i] = (TimePeriod)availSortedLongTPs[i];
				}
               return tpArray;
				}
			
			catch(Exception ex)
			{
				throw new Exception("exception throw in retrieving available time periods"+ex.Message); 
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceBrokerGUID"></param>
        /// <param name="groupName"></param>
        /// <param name="ussGUID"></param>
        /// <param name="labClientName"></param>
        /// <param name="labClientVersion"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static TimeBlock[] RetrieveAvailableTimeBlocks(string serviceBrokerGUID,string groupName, string ussGUID,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
            try
            {
                DateTime startTimeUTC = startTime.ToUniversalTime();
                DateTime endTimeUTC = endTime.ToUniversalTime();
                //get the experiment configuration
                int eID = DBManager.ListExperimentInfoIDByExperiment(clientGuid, labServerGuid);
                if (eID < 0)
                {
                    throw new Exception("exception throw in retrieving available time periods, the experiment can not be found");
                }
                LssExperimentInfo exInfo = GetExperimentInfos(new int[] { eID })[0];
                int preTime = exInfo.prepareTime;
                int recTime = exInfo.recoverTime;
                int minimumTime = exInfo.minimumTime;
                string labServerGUID = exInfo.labServerGuid;
                //get the timeblocks which are the minimum time blocks set covering the time chunk for the lab server where the experiment is executed
                int[] tIDs = ListTimeBlockIDsByTimeChunk(serviceBrokerGUID, groupName, ussGUID, clientGuid, labServerGUID, startTimeUTC, endTimeUTC);
                //get the IDs timeblocks in the time chunk which has been given the permission to the experiment for particular group
                ArrayList arrayListTIDs = new ArrayList();
                for (int i = 0; i < tIDs.Length; i++)
                {
                    
                    if (CheckPermission(eID, tIDs[i]))
                    {
                        arrayListTIDs.Add(tIDs[i]);
                    }
                }

                int[] availTIDs = Utilities.ArrayListToIntArray(arrayListTIDs);
                //get the  time blocks which are the minimum time blocks se-t covering the time chunk, and has been given the permission to the experiment for particular group
                TimeBlock[] availTBs = DBManager.GetTimeBlocks(availTIDs);
                return availTBs;
            }
            catch (Exception ex)
            {

                throw new Exception("exception throw in retrieving available time blocks" + ex.Message); 

            }	

            }
/// <summary>
/// Returns an Boolean indicating whether a particular reservation from a USS is confirmed and added to the database in LSS successfully. If it fails, exception will be throw out indicating
///	the reason for rejection.
/// </summary>
/// <param name="serviceBrokerGuid"></param>
/// <param name="groupName"></param>
/// <param name="ussGuid"></param>
/// <param name="labClientName"></param>
/// <param name="labClientVersion"></param>
/// <param name="startTime"></param>the local time of LSS
/// <param name="endTime"></param>the local time of LSS
/// <returns></returns>the notification whether the reservation is confirmed. If not, notification will give a reason
		public static string ConfirmReservation(string serviceBrokerGuid, string groupName, string ussGuid,
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
			try
			{
				string notification = null ;
				//get the experiment configuration
                
				int eID = DBManager.ListExperimentInfoIDByExperiment(clientGuid, labServerGuid);
				LssExperimentInfo exInfo = GetExperimentInfos(new int[] {eID})[0];
				int preTime = exInfo.prepareTime;
				int recTime = exInfo.recoverTime;
				int minTime = exInfo.minimumTime;
				//check whether the reservation is executable
				if (((TimeSpan)endTime.Subtract(startTime)).TotalMinutes < minTime)
				{
					notification = "The reservation time is less than the minimum time the experiment required";
					return notification;
				}
				//the start time for the experiment equipment
				DateTime expStartTime = startTime.AddMinutes(-preTime);
				//the end time for the experiment equipment
				DateTime expEndTime = endTime.AddMinutes(recTime);
				TimePeriod[] availTPs = RetrieveAvailableTimePeriods(serviceBrokerGuid,groupName, ussGuid, clientGuid, labServerGuid,  expStartTime, expEndTime);
				if ( availTPs.Length != 1)
				{
					notification = "this reservation time is not available now, please reset your reservation time";
					return notification;
				}
				else
				{	
					if(availTPs[0].startTime > startTime || availTPs[0].endTime < endTime)
					{
						notification = "this reservation time is not available now,, please reset your reservation time";
						return notification;
					}

					if ( availTPs[0].startTime > expStartTime)
					{
						notification = "there is not enough preparation time for the experiment, please set your start time later";
						return notification;
					}
					if( availTPs[0].endTime < expEndTime)
					{
						notification = "there is not enough recovery time for the experiment,please set your end time earlier";
						return notification;
					}
			
					//add the reservation to to reservationInfo table
			
					int status = AddReservationInfo(serviceBrokerGuid, groupName, ussGuid, clientGuid, labServerGuid,  startTime.ToUniversalTime(),  endTime.ToUniversalTime());
					if(status > 0)
                        notification = "The reservation is confirmed successfully";
                    else
                        notification = "Error: AddReservation status = " + status;
					return notification;
				}
			}
			catch(Exception ex)
			{
				 
				return ex.Message;

			}	
		}
/// <summary>
/// given a time period defined by the start time and the end tiime, return the time slots defined by the quatum of the experiment during this time period
/// </summary>
/// <param name="labClientName"></param>
/// <param name="labClientVersion"></param>
/// <param name="startTime"></param>
/// <param name="endTime"></param>
/// <returns></returns>
        public static TimePeriod[] RetrieveTimeSlots(string serviceBrokerGuid, string groupName, 
            string clientGuid, string labServerGuid, DateTime startTime, DateTime endTime)
		{
			try
			{
				//get the experiment configuration
				int eID=DBManager.ListExperimentInfoIDByExperiment(clientGuid,labServerGuid);
				if(eID < 0)
				{
					throw new Exception("exception throw in retrieving available time periods, the experiment can not be found"); 
				}
				LssExperimentInfo exInfo=GetExperimentInfos(new int[] {eID})[0];
                //int preTime=exInfo.prepareTime;
                //int recTime=exInfo.recoverTime;
				int quatum = exInfo.quantum;
                if (quatum <= 0)
                {
                    throw new Exception("RetrieveTimeSlots: quantum must be greater than zero");
                }
                //DateTime expStartTime = startTime.AddMinutes(preTime);
                //DateTime expEndTime = endTime.AddMinutes(-recTime);
                DateTime expStartTime = startTime;
                DateTime expEndTime = endTime;
				ArrayList tslots = new ArrayList();
				DateTime t = expStartTime;
				DateTime t1 = expEndTime.AddMinutes(-quatum);
				while(t <= t1)
				{   
					TimePeriod ts = new TimePeriod();
					ts.startTime = t;
					ts.endTime = t.AddMinutes(quatum);
					tslots.Add(ts);
					t = t.AddMinutes(quatum);	
				}
				TimePeriod[] tsArray = new TimePeriod[tslots.Count];
				for(int i=0; i< tslots.Count;i++)
				{
					tsArray[i] = (TimePeriod)tslots[i];
				}
				return tsArray;
			}
			catch
			{
                throw;
			}
		}
        /* !------------------------------------------------------------------------------!
         *							CALLS FOR Recurrence
         * !------------------------------------------------------------------------------!
         */
        /// <summary>
        /// add recurrence
        /// </summary>
        /// <param name="recurrenceStartDate"></param>
        /// <param name="recurrenceEndDate"></param>
        /// <param name="recurrenceType"></param>
        /// <param name="recurrenceStartTime"></param>
        /// <param name="recurrenceEndTime"></param>
        /// <param name="labServerGuid"></param>
        /// <param name="resourceID"></param>
        /// <returns></returns>the uniqueID which identifies the recurrence added, >0 was successfully added; ==-1 otherwise
        public static int AddRecurrence(DateTime recurrenceStartDate, DateTime recurrenceEndDate, 
            string recurrenceType, TimeSpan recurrenceStartTime, TimeSpan recurrenceEndTime,
            string labServerGuid, int resourceID,byte dayMask)
        {
            int recurrenceID = DBManager.AddRecurrence(recurrenceStartDate, recurrenceEndDate, recurrenceType,
                recurrenceStartTime, recurrenceEndTime, labServerGuid, resourceID, dayMask);
            return recurrenceID;
        }

        /// <summary>
        /// delete the recurrence specified by the recurrenceIDs
        /// </summary>
        /// <param name="recurrenceIDs"></param>
        /// <returns></returns>an array of ints containning the IDs of all recurrence not successfully removed
        public static int[] RemoveRecurrence(int[] recurrenceIDs)
        {
            int[] recurs = DBManager.RemoveRecurrence(recurrenceIDs);
            return recurs;
          
        }
        /// <summary>
        /// Returns an array of the immutable Recurrence objects that correspond ot the supplied Recurrence IDs
        /// </summary>
        /// <param name="timeBlockIDs"></param>
        /// <returns></returns>an array of immutable objects describing the specified Recurrence
        public static Recurrence[] GetRecurrence(int[] recurrenceIDs)
        {
            Recurrence[] recur = DBManager.GetRecurrence(recurrenceIDs);
            return recur;
         
        }
        /// Enumerates the IDs of the information of all the Recurrence 
        /// </summary>
        /// <returns></returns>the array  of ints contains the IDs of the information of all the Recurrence
        public static int[] ListRecurrenceIDs()
        {
            int[] recurIDs = DBManager.ListRecurrenceIDs();
            return recurIDs;
        }
          /// <summary>
        /// enumerates all IDs of the recurrences belonging to a particular lab server identified by the labserverID
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <returns></returns>an array of ints containing the IDs of all the time blocks of specified lab server
        public static int[] ListRecurrenceIDsByLabServer(string labServerGuid)
        {
            int[] recurIDs = DBManager.ListRecurrenceIDsByLabServer(labServerGuid);
            return recurIDs;
        }

        /// <summary>
        /// enumerates all IDs of the recurrences belonging to a particular lab server identified by the labserverID
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <returns></returns>an array of ints containing the IDs of all the time blocks of specified lab server
        public static int[] ListRecurrenceIDsByLabServer(DateTime start, DateTime end, string labServerGuid)
        {
            int[] recurIDs = DBManager.ListRecurrenceIDsByLabServer(start, end, labServerGuid);
            return recurIDs;
        }

        /// <summary>
        /// enumerates all IDs of the recurrences belonging to a particular lab server identified by the labserverID
        /// </summary>
        /// <param name="labServerGuid"></param>
        /// <returns></returns>an array of ints containing the IDs of all the time blocks of specified lab server
        public static int[] ListRecurrenceIDsByResourceID(DateTime start, DateTime end, int resourceID)
        {
            int[] recurIDs = DBManager.ListRecurrenceIDsByResourceID(start, end, resourceID);
            return recurIDs;
        }
	
	}

}
