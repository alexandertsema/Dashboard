using Dashboard.Server.Monitoring.Monitor.Models.Enums;

namespace Dashboard.Server.Monitoring.Monitor.Models.BaseModels
{
    public class BaseModel
    {
        public BaseModel()
        {
        }

        public BaseModel(WmiNamespacesEnum wmiNamespace)
        {
            this.WmiNamespace = wmiNamespace;
        }

        public WmiNamespacesEnum WmiNamespace { get; set; }
    }
}