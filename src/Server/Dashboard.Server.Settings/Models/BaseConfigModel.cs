using System;
using System.IO;
using Newtonsoft.Json;

namespace Dashboard.Server.Settings.Models
{
    public class BaseConfigModel
    {
        [JsonProperty(PropertyName = "ip")]
        public String Ip { get; set; }
        [JsonProperty(PropertyName = "port")]
        public Int16 Port { get; set; }
    }
}