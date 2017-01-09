using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Dashboard.Server.Configuration.Managers;
using Dashboard.Server.Configuration.Models;
using Dashboard.Server.Monitoring.Monitor.Helpers;

namespace Dashboard.Server.Monitoring.Service
{
    class Service
    {
        private static readonly ConfigManager configManager = new ConfigManager("config.json");
        private static ConfigModel config;

        static void Main(string[] args)
        {
            config = configManager.GetSettings();

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
            var server = new TcpListener(IPAddress.Parse(config.MonitoringServiceConfig.Ip), config.MonitoringServiceConfig.Port);

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

            Console.WriteLine($"infoModel sent to Server.Service");

            while (true)
            {
                Console.WriteLine($"Waiting for signal...");
                //todo: waiting for signal
                while (!stream.DataAvailable) // wait for signal to start broadcasting todo: very excpensive! up to 16% CPU
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
                    while (true)
                    {
                        if (stream.DataAvailable) // wait for signal to stop broadcasting
                        {
                            rawMessage = new Byte[client.Available];
                            stream.Read(rawMessage, 0, rawMessage.Length);
                            signal = Encoding.UTF8.GetString(rawMessage);
                            if (Convert.ToInt16(signal) == 3)
                            {
                                Console.WriteLine($"signal with OpCode {signal} recieved, stoping broadcasting...");
                                break;
                            }
                        }
                        //todo: until at least 1 client connected to Server.Service get perfomanceModel, send perfomanceModel
                        var perfomanceModel = monitor.GetPerfomanceStatistics();
                        response = Encoding.UTF8.GetBytes(SerializationHelper.Serialize(perfomanceModel));
                        stream.Write(response, 0, response.Length);

                        //Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
