namespace Dashboard.Server.Monitoring.Monitor.Models.Perfomance
{
    public class PerfomanceModel
    {
        public PerfomanceModel()
        {
            CpuPerfomance = new CpuPerfomanceModel();
        }

        public CpuPerfomanceModel CpuPerfomance { get; set; }
    }
}