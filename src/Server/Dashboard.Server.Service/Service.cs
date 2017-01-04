using System;
using Dashboard.Server.WebSocket;

//using TestSt6andart.Test;

namespace Dashboard.Server.Service
{
    class Service
    {
        private static readonly string IP = "127.0.0.1";
        private static readonly short PORT_SERVER = 2222;
        private static readonly short PORT_CLIENT = 8888;
        static void Main(string[] args)
        {
            ServerStart();
            Console.ReadLine();
        }

        private static async void ServerStart()
        {
            var isFirstClient = true;
            WebSocketServer server = new WebSocketServer(IP, PORT_SERVER); // init Server.Service
            
            WebSocketClient monitoringClient = new WebSocketClient(IP, PORT_CLIENT); // connect to Monitoring.Service 
            
            monitoringClient.Connect(); // waiting for incoming messages
            
            Console.WriteLine($"Server.Service connected to Monitoring.Service at {IP}:{PORT_SERVER}, waiting for incoming infoModel...");

            while (!monitoringClient.Client.GetStream().DataAvailable) // wait for infoModel
            { 
            }

            var infoModelString = monitoringClient.Recieve(); //receive infoModel and save somewhere
            Console.WriteLine($"InfoModel received: {infoModelString}");

            server.Start(); // Server.Service started
            Console.WriteLine($"Server.Service started at {IP}:{PORT_SERVER}, waiting for clients...");
            
            while (true) // allows multiple clients to connect
            {
                var client = await server.Server.AcceptTcpClientAsync(); // client connected

                var clientId = Guid.NewGuid();
                Console.WriteLine($"Client {clientId} connected to Server.Service");
                if (server.Clients == 0) // if it is the first client during the session
                {
                    monitoringClient.Signal(OpCodes.RequestBroadcasting); // send signal to Monitoring.Service
                    Console.WriteLine($"Monitoring.Service signaled about first client connection");
                }
                server.Clients++;
                Console.WriteLine($"Now {server.Clients} connected");

                var clientHandler = new ClientHandler(clientId, client, infoModelString, isFirstClient, monitoringClient, server); // start new task with new ClientHandler
                
                isFirstClient = false;
            }
        }
    }
}