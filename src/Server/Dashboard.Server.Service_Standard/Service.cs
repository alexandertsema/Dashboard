using System;
using Dashboard.Server.Configuration.Managers;
using Dashboard.Server.Configuration.Models;
using Dashboard.Server.WebSocket;

namespace Dashboard.Server.Service
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
            WebSocketServer server = new WebSocketServer(config.ServiceConfig.Ip, config.ServiceConfig.Port); // init Server.Service
            
            WebSocketClient monitoringClient = new WebSocketClient(config.MonitoringServiceConfig.Ip, config.MonitoringServiceConfig.Port); // connect to Monitoring.Service 
            
            monitoringClient.Connect(); // waiting for incoming messages
            
            Console.WriteLine($"Server.Service connected to Monitoring.Service at {config.MonitoringServiceConfig.Ip}:{config.MonitoringServiceConfig.Port}, waiting for incoming infoModel...");

            while (!monitoringClient.Client.GetStream().DataAvailable) // wait for infoModel
            { 
            }

            var infoModelString = monitoringClient.Recieve(); //receive infoModel and save somewhere
            Console.WriteLine($"InfoModel received: {infoModelString}");

            server.Start(); // Server.Service started
            Console.WriteLine($"Server.Service started at {config.ServiceConfig.Ip}:{config.ServiceConfig.Port}, waiting for clients...");
            
            while (true) // allows multiple clients to connect
            {
                var client = await server.Server.AcceptTcpClientAsync(); // client connected

                var clientId = Guid.NewGuid();
                Console.WriteLine($"Client {clientId} connected to Server.Service");
                if (server.Clients == 0) // if it is the first client during the session
                {
                    monitoringClient.Signal(OpCodes.StartBroadcasting); // send signal to Monitoring.Service
                    Console.WriteLine($"Monitoring.Service signaled about first client connection");
                }
                server.Clients++;
                Console.WriteLine($"Now {server.Clients} connected");

                new ClientHandler(clientId, client, infoModelString, monitoringClient, server);
                
            }
        }
    }
}