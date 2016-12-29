using Dashboard.Server.Monitoring.Monitor.Models.Info;
using Dashboard.Server.Monitoring.Monitor.Models.Perfomance;

namespace Dashboard.Server.Monitoring.Monitor.Abstract
{
    public interface IMonitor
    {
        PerfomanceModel GetPerfomanceStatistics();
        InfoModel GetInfoModel();
    }
}