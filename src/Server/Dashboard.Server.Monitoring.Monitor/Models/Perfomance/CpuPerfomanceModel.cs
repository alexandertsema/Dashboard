using System;
using Dashboard.Server.Monitoring.Monitor.Models.BaseModels;

namespace Dashboard.Server.Monitoring.Monitor.Models.Perfomance
{
    public class CpuPerfomanceModel : CpuBaseModel
    {
        public Double LoadPercentage { get; set; }
    }
}
