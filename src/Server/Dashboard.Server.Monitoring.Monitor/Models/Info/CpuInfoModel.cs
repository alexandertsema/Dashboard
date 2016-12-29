using System;
using Dashboard.Server.Monitoring.Monitor.Models.BaseModels;

namespace Dashboard.Server.Monitoring.Monitor.Models.Info
{
    public class CpuInfoModel : CpuBaseModel
    {
        public String Name { get; set; }
        public Int16 NumberOfCores { get; set; }
        public Int16 NumberOfLogicalProcessors { get; set; }
    }
}