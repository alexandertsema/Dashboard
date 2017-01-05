using System;
using Dashboard.Server.Monitoring.Monitor.Models.BaseModels;
using Dashboard.Server.Monitoring.Monitor.Models.Enums;

namespace Dashboard.Server.Monitoring.Monitor.Models.InfoModels
{
    public class CpuInfoModel : BaseModel
    {
        public CpuInfoModel(): base (WmiNamespacesEnum.Win32_Processor)
        {
        }

        public String Name { get; set; }
        public Int16 NumberOfCores { get; set; }
        public Int16 NumberOfLogicalProcessors { get; set; }
    }
}