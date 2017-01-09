using System;
using System.Web.Script.Serialization;

namespace Dashboard.Server.Monitoring.Monitor.Helpers
{
    public class SerializationHelper
    {
        public static string Serialize<T>(T model) => new JavaScriptSerializer().Serialize(model);
    }
}