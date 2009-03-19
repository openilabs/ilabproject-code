using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

using iLabs.Ticketing;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Proxies.ESS;
using iLabs.Proxies.ISB;
using iLabs.UtilLib;


namespace iLabs.LabServer.Interactive
{
	/// <summary>
	/// Tread that processes short in memory handles for current tasks, needs to be loaded 
    /// from database and to update database records when status changes.
	/// </summary>
	public class TaskProcessor
	{
        private int waitTime = 10000;
        private int count = 0;
        private bool go = true;
        private List<LabTask> tasks;
        
		public TaskProcessor()
		{
            Utilities.WriteLog("TaskProcessor created");
            tasks = new List<LabTask>();
           
			//
			// TODO: Add constructor logic here
			
		}

        public TaskProcessor(int delay)
        : this() {
            waitTime = delay;
        }

        public void Run()
        {
            while (go)
            {
                ProcessTasks();
                Thread.Sleep(waitTime);
            }
        }
        public void Start()
        {
            lock (this)
            {
                go = true;
            } 
            Run();
        }

        public void Stop()
        {
            lock (this)
            {
                go = false;
            }
        }

        public void Add(LabTask task)
        {
            lock (tasks)
            {
                tasks.Add(task);
            }
        }
        public void Remove(LabTask task)
        {
            lock (tasks)
            {
                tasks.Remove(task);
            }
        }
        public LabTask GetTask(long experimentID)
        {
            LabTask task = null;
            lock (tasks)
            {
                foreach (LabTask t in tasks)
                {
                    if (t.experimentID == experimentID)
                    {
                        task = t;
                        break;
                    }
                }
            }
            return task;
        }



        public void ProcessTasks()
        {
            List<LabTask> toBeRemoved = null;
            count++;
            if (count >= 10000)
            {
                Utilities.WriteLog("ProcessTasks");
                count = 0;
            }

            long ticksNow = DateTime.UtcNow.Ticks;
            lock (tasks)
            {
                foreach (LabTask task in tasks)
                {

                    if (ticksNow > task.endTime.Ticks )
                    { // task has expired
                        try
                        {
                            Utilities.WriteLog("Found expired task: " + task.taskID);
                            
                            if (toBeRemoved == null)
                                toBeRemoved = new List<LabTask>();
                            toBeRemoved.Add(task);

                        }

                        catch (Exception e1)
                        {
                            Utilities.WriteLog("ProcessTasks Expired: exception:" + e1.Message + e1.StackTrace);
                        }

                    }
                    else
                    {
                        try
                        {
                            if (task.Status == LabTask.eStatus.Running)
                            {
                                task.HeartBeat();
                            }
                        }
                        catch (Exception e2)
                        {
                            Utilities.WriteLog("ProcessTasks Status: " + e2.Message);
                        }
                    }
                }
                if (toBeRemoved != null)
                {
                    foreach (LabTask t in toBeRemoved)
                    {
                        t.Expire();
                        tasks.Remove(t);
                    }
                }
            }// End tasks lock
        }
	}
}
