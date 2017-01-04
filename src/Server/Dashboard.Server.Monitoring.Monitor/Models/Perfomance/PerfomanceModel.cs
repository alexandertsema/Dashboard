namespace Dashboard.Server.Monitoring.Monitor.Models.Perfomance
{
    public class PerfomanceModel
    {
        public PerfomanceModel()
        {
            CpuPerfomance = new CpuPerfomanceModel();
            RamPerfomance = new RamPerfomanceModel();
        }

        public CpuPerfomanceModel CpuPerfomance { get; set; }
        public RamPerfomanceModel RamPerfomance{ get; set; }
    }
}