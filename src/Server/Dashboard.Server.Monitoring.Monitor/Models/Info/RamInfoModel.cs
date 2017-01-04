using System;
using Dashboard.Server.Monitoring.Monitor.Models.BaseModels;

namespace Dashboard.Server.Monitoring.Monitor.Models.Info
{
    public class RamInfoModel : RamBaseModel
    {
        public Int32 TotalVisibleMemorySize { get; set; }
        public Int32 FreePhysicalMemory { get; set; }
    }
}