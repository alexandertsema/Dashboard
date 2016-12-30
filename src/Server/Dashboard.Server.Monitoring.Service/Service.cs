using System;

namespace Dashboard.Server.Monitoring.Service
{
    class Service
    {
        static void Main(string[] args)
        {
            var monitor = new Monitor.Concrete.Monitor();
            var infoModel = monitor.GetInfoModel();
            Console.ReadLine();
        }
    }
}
