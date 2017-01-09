using System;
using Dashboard.Server.Monitoring.Monitor.Models.BaseModels;
using Dashboard.Server.Monitoring.Monitor.Models.Enums;

namespace Dashboard.Server.Monitoring.Monitor.Models.PerfomanceModels
{
    public class CpuPerfomanceModel : BaseModel
    {
        public CpuPerfomanceModel() : base(WmiNamespacesEnum.Win32_Processor)
        {
        }

        public Int16 LoadPercentage { get; set; }
    }
}
