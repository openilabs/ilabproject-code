
/*
 * Copyright (c) 2004-2006 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id$ */

using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Services;

using iLabs.DataTypes;

using iLabs.DataTypes.SoapHeaderTypes;
using iLabs.DataTypes.StorageTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.Proxies.ESS;
using iLabs.Proxies.ISB;

namespace iLabs.LabServer.Interactive
{

    /// <summary>
    /// Summary description for DataSourceManager
    /// </summary>
    public class DataSourceManager : IDisposable

    {
        public long taskID = -1L;
        public long experimentID = -1L;
        public ExperimentStorageProxy essProxy;
        private string appKey;
        private ArrayList dataSources;


        public DataSourceManager()
        {
            dataSources = new ArrayList();
        }

        public DataSourceManager(LabTask task)
            : this()
        {
            taskID = task.taskID;
            experimentID = task.experimentID;
            if (task.storage != null && task.storage.Length > 0)
            {
                LabDB labDB = new LabDB();
                Coupon expCoupon = labDB.GetCoupon(task.couponID, task.issuerGUID);
                if (expCoupon != null)
                {
                    essProxy = new ExperimentStorageProxy();
                    essProxy.Url = task.storage;
                    essProxy.OperationAuthHeaderValue = new OperationAuthHeader();
                    essProxy.OperationAuthHeaderValue.coupon = expCoupon;
                }
                else
                    throw new Exception("ExpCoupon not found");
            }
            else
             throw new Exception("ESS is not specified");
        }

        public long ExperimentID
        {
            get
            {
                return experimentID;
            }
            set
            {
                experimentID = value;
            }
        }
        public void AddDataSource(LabDataSource ds)
        {
            ds.DataManager = this;
            dataSources.Add(ds);
        }

        public void CloseDataSources()
        {
            foreach(LabDataSource ds in dataSources)
            {
                ds.Disconnect();
                ds.Dispose();

            }
        }

        public long TaskID
        {
            get
            {
                return taskID;
            }
            set
            {
                taskID = value;
            }
        }

        public  string AppKey
        {
            get
            {
                return appKey;
            }
            set
            {
                appKey = value;
            }
        }

        public void Dispose()
        {
            CloseDataSources();
        }

    }
}

