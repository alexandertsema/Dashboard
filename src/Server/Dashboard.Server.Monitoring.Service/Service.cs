using System;

namespace Dashboard.Server.Monitoring.Service
{
    class Service
    {
        static void Main(string[] args)
        {
            var monitor = new Monitor.Concrete.Monitor();
            var infoModel = monitor.GetInfoModel();
            foreach (var prop1 in infoModel.GetType().GetProperties())
            {
                foreach (var prop2 in prop1.GetType().GetProperties())
                {
                    Console.WriteLine($"prop1: {prop1.Name} \nprop2: {prop2.Name} - {prop2.GetValue(infoModel)}");
                }
            }
            Console.ReadLine();
        }
    }
}
