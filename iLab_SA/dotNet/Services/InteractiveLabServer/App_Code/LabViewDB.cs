using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using iLabs.LabServer;
using iLabs.LabServer.Interactive;
/// <summary>
/// Summary description for LabViewDB
/// </summary>

namespace iLabs.LabServer.LabView
{
    public class LabViewDB : LabDB
    {
        public LabViewDB()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public virtual LabTask GetTask(long task_id)
        {
            LabTask task = base.GetTask(task_id);
            LabViewTaskFactory tf = new LabViewTaskFactory();
            return tf.CreateLabTask(task);
        }
        public virtual LabTask GetTask(long experiment_id, string sbGUID)
        {
            LabTask task = base.GetTask(experiment_id, sbGUID);
            LabViewTaskFactory tf = new LabViewTaskFactory();
            return tf.CreateLabTask(task);
        }
        public virtual LabTask[] GetActiveTasks()
        {
             LabViewTaskFactory tf = new LabViewTaskFactory();
            List<LabTask> tasks = new List<LabTask>();
            LabTask[] at = base.GetActiveTasks();
            foreach(LabTask t in at){
                tasks.Add(tf.CreateLabTask(t));
            }
            return tasks.ToArray();
        }
        public virtual LabTask[] GetExpiredTasks(DateTime targetTime)
        {
            LabViewTaskFactory tf = new LabViewTaskFactory();
            List<LabTask> tasks = new List<LabTask>();
            LabTask[] at = base.GetExpiredTasks(targetTime);
            foreach (LabTask t in at)
            {
                tasks.Add(tf.CreateLabTask(t));
            }
            return tasks.ToArray();
        }
    }
}
