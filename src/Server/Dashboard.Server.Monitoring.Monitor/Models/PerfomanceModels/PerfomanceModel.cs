namespace Dashboard.Server.Monitoring.Monitor.Models.PerfomanceModels
{
    public class PerfomanceModel
    {
        public PerfomanceModel()
        {
            CpuPerfomance = new CpuPerfomanceModel();
            RamPerfomance = new RamPerfomanceModel();
            DiskPerfomance = new DiskPerfomanceModel();
        }

        public CpuPerfomanceModel CpuPerfomance { get; set; }
        public RamPerfomanceModel RamPerfomance{ get; set; }
        public DiskPerfomanceModel DiskPerfomance{ get; set; }
    }
}