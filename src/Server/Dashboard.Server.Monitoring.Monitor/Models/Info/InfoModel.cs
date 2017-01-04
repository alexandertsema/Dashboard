namespace Dashboard.Server.Monitoring.Monitor.Models.Info
{
    public class InfoModel
    {
        public InfoModel()
        {
            CpuInfo = new CpuInfoModel();
            RamInfo = new RamInfoModel();
        }

        public CpuInfoModel CpuInfo { get; set; }
        public RamInfoModel RamInfo { get; set; }
    }
}