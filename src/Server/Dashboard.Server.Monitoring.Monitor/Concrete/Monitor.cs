using System;
using System.Management;
using Dashboard.Server.Monitoring.Monitor.Abstract;
using Dashboard.Server.Monitoring.Monitor.Helpers;
using Dashboard.Server.Monitoring.Monitor.Models.InfoModels;
using Dashboard.Server.Monitoring.Monitor.Models.PerfomanceModels;

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

        public PerfomanceModel GetPerfomanceStatistics() => GetModel(new PerfomanceModel());

        public InfoModel GetInfoModel() => GetModel(new InfoModel());

        #endregion public methods

        #region private methods

        private T GetModel<T>(T model)
        {
            var properties = model.GetType().GetProperties();
            foreach (var property in properties)
            {
                var query = WmiHelper.BuildQuery(property.GetValue(model));
                var searcher = new ManagementObjectSearcher(scope, query);

                foreach (var metrics in searcher.Get())
                {
                    foreach (var metric in metrics.Properties)
                    {
                        var value = property.GetValue(model);
                        var prop = value.GetType().GetProperty(metric.Name);

                        if (metric.Value != null)
                            prop.SetValue(value, Convert.ChangeType(metric.Value, prop.PropertyType));
                    }
                }
            }
            return model;
        }

        #endregion private methods

    }
}