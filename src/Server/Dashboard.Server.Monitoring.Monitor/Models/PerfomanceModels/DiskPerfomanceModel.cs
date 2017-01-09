using System;
using Dashboard.Server.Monitoring.Monitor.Models.BaseModels;
using Dashboard.Server.Monitoring.Monitor.Models.Enums;

namespace Dashboard.Server.Monitoring.Monitor.Models.PerfomanceModels
{
    public class DiskPerfomanceModel : BaseModel
    {
        public DiskPerfomanceModel() : base(WmiNamespacesEnum.Win32_PerfRawData_PerfDisk_PhysicalDisk)
        {
        }

        public UInt64 DiskReadBytesPersec { get; set; }
        public UInt64 DiskWriteBytesPersec { get; set; }
    }
}