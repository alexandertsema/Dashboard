namespace Dashboard.Server.Monitoring.Monitor.Models.BaseModels
{
    public class CpuBaseModel
    {
        public WmiNamespacesEnum WmiNamespace => WmiNamespacesEnum.Win32_Processor;
    }
}