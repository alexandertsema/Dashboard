using System;
using Dashboard.Server.Monitoring.Monitor.Helpers;

namespace Dashboard.Server.Monitoring.Service
{
    class Service
    {
        static void Main(string[] args)
        {
            var monitor = new Monitor.Concrete.Monitor();
            while (true)
            {
                var infoModel = monitor.GetInfoModel();
                Console.WriteLine(SerializationHelper.Serialize(infoModel));
            }
            
            Console.ReadLine();
        }
    }
}
