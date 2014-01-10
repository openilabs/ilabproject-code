using System;
using System.Collections.Generic;
using System.Text;
using iLabs.DataTypes;
using iLabs.DataTypes.TicketingTypes;

namespace iLabs.LabServer.Interactive
{
    interface I_TaskFactory
    {
         /// <summary>
        /// Parses the appInfo and experiment ticket, inserts the task into the database and
        /// creates a dataManager and dataSources defined in the appInfo.
        /// </summary>
        /// <param name="appInfo"></param>
        /// <param name="expCoupon"></param>
        /// <param name="expTicket"></param>
        /// <returns></returns>
        LabTask CreateLabTask(LabAppInfo appInfo, Coupon expCoupon, Ticket expTicket);
    }
}
