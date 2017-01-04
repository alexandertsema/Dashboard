namespace Dashboard.Server.Monitoring.Monitor.Models.BaseModels
{
    public class RamBaseModel
    {
        public WmiNamespacesEnum WmiNamespace => WmiNamespacesEnum.Win32_OperatingSystem;
    }
}