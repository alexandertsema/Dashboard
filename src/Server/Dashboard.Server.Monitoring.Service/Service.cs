using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dashboard.Server.Monitoring.Monitor.Helpers;

namespace Dashboard.Server.Monitoring.Service
{
    class Service
    {
        static void Main(string[] args)
        {
            ServerStart();
            Console.ReadLine();
        }

        private static async void ServerStart()
        {
            var monitor = new Monitor.Concrete.Monitor();
            
            var infoModel = monitor.GetInfoModel();
            Console.WriteLine($"infoModel retrieved");
            //todo: !!! port dll to .Net Standart !!!
            
            //todo: init server
            var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            
            //todo: start waiting for client
            server.Start();
            Console.WriteLine($"Monitoring.Service started at 127.0.0.1:8888, waiting for Server.Service to connect...");

            //todo: client accepted
            var client = await server.AcceptTcpClientAsync();

            Console.WriteLine($"Server.Service connected to Monitoring.Service at 127.0.0.1:8888");
            //todo: send infoModel
            var stream = client.GetStream();
            var response = Encoding.UTF8.GetBytes(SerializationHelper.Serialize(infoModel));
            stream.Write(response, 0, response.Length);

            Console.WriteLine($"infoModel sent to Server.Service, waiting for signal...");

            //todo: waiting for signal
            while (!client.GetStream().DataAvailable) // wait for infoModel
            {
            }

            //todo: signal received
            var rawMessage = new Byte[client.Available];
            stream.Read(rawMessage, 0, rawMessage.Length);
            var signal = Encoding.UTF8.GetString(rawMessage);
            Console.WriteLine($"signal with OpCode {signal} recieved");
            //todo: broadcasting forever
            if (Convert.ToInt16(signal) == 2)
            {
                Console.WriteLine("Start broadcasting");
                int i = 0;
                while (true)
                {
                    //todo: until at least 1 client connected to Server.Service get perfomanceModel, send perfomanceModel
                    response = Encoding.UTF8.GetBytes($"This is perfomanceModel {DateTime.Now} ");
                    stream.Write(response, 0, response.Length);

                    Thread.Sleep(100);
                }
            } 
        }
    }
}
