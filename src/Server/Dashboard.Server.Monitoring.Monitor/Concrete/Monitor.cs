﻿using System;
using System.Management;
using Dashboard.Server.Monitoring.Monitor.Abstract;
using Dashboard.Server.Monitoring.Monitor.Helpers;
using Dashboard.Server.Monitoring.Monitor.Models.Info;
using Dashboard.Server.Monitoring.Monitor.Models.Perfomance;

namespace Dashboard.Server.Monitoring.Monitor.Concrete
{
    public class Monitor : IMonitor
    {
        #region private fields

        private readonly ManagementScope scope;

        #endregion private fields

        #region constructors

        public Monitor()
        {
            this.scope = new ManagementScope(WmiHelper.GetServerName());
        }

        #endregion constructors

        #region public methods

        public PerfomanceModel GetPerfomanceStatistics()
        {
            throw new System.NotImplementedException();
        }

        public InfoModel GetInfoModel()
        {
            var infoModel = new InfoModel();

            var properties = infoModel.GetType().GetProperties();
            foreach (var property in properties)
            {
                var query = WmiHelper.BuildQuery(property.GetValue(infoModel));
                var searcher = new ManagementObjectSearcher(scope, query);

                foreach (var metrics in searcher.Get())
                {
                    foreach (var metric in metrics.Properties)
                    {
                        var value = property.GetValue(infoModel);
                        var prop = value.GetType().GetProperty(metric.Name);

                        prop.SetValue(value, Convert.ChangeType(metric.Value, prop.PropertyType));
                    }
                }
            }

            return infoModel;
        }

        #endregion public methods

    }
}