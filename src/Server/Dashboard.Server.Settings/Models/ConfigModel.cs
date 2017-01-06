using System;
using Newtonsoft.Json;

namespace Dashboard.Server.Settings.Models
{
    public class ConfigModel
    {
        public ConfigModel()
        {
            MonitoringServiceConfig = new MonitoringServiceConfigModel();
            ServiceConfig = new ServiceConfigModel();
        }
        [JsonProperty(PropertyName = "monitoringService")]
        public MonitoringServiceConfigModel MonitoringServiceConfig { get; set; }
        [JsonProperty(PropertyName = "mainService")]
        public ServiceConfigModel ServiceConfig { get; set; }
    }
}