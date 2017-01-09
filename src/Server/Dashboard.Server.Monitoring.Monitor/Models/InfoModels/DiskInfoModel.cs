using System;
using Dashboard.Server.Monitoring.Monitor.Models.BaseModels;
using Dashboard.Server.Monitoring.Monitor.Models.Enums;

namespace Dashboard.Server.Monitoring.Monitor.Models.InfoModels
{
    public class DiskInfoModel : BaseModel
    {
        public DiskInfoModel() : base(WmiNamespacesEnum.Win32_LogicalDisk)
        {
        }

        public UInt64 FreeSpace { get; set; }
        public UInt64 Size { get; set; }
        public String Name { get; set; }
        public String VolumeName { get; set; }
    }
}