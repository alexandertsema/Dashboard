using System;
using Dashboard.Server.Monitoring.Monitor.Models.BaseModels;
using Dashboard.Server.Monitoring.Monitor.Models.Enums;

namespace Dashboard.Server.Monitoring.Monitor.Models.InfoModels
{
    public class RamInfoModel : BaseModel
    {
        public RamInfoModel() : base(WmiNamespacesEnum.Win32_OperatingSystem)
        {
        }

        public Int32 TotalVisibleMemorySize { get; set; }
        public Int32 FreePhysicalMemory { get; set; }
    }
}