namespace Dashboard.Server.Monitoring.Monitor.Models.Info
{
    public class InfoModel
    {
        public InfoModel()
        {
            this.CpuInfo = new CpuInfoModel();
        }

        public CpuInfoModel CpuInfo { get; set; }
    }
}