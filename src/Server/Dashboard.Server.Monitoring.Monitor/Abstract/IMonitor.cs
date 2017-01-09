using Dashboard.Server.Monitoring.Monitor.Models.InfoModels;
using Dashboard.Server.Monitoring.Monitor.Models.PerfomanceModels;

namespace Dashboard.Server.Monitoring.Monitor.Abstract
{
    public interface IMonitor
    {
        PerfomanceModel GetPerfomanceStatistics();
        InfoModel GetInfoModel();
    }
}