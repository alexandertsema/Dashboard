namespace Dashboard.Server.Monitoring.Monitor.Models.InfoModels
{
    public class InfoModel
    {
        public InfoModel()
        {
            CpuInfo = new CpuInfoModel();
            RamInfo = new RamInfoModel();
            DiskInfo = new DiskInfoModel();
        }

        public CpuInfoModel CpuInfo { get; set; }
        public RamInfoModel RamInfo { get; set; }
        public DiskInfoModel DiskInfo { get; set; }
    }
}