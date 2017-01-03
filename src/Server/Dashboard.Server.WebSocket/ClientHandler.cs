using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Server.WebSocket
{
    public class ClientHandler : WebSocket
    {
        private readonly Guid clientId;
        private readonly TcpClient client;
        private readonly String infoModelString;
        private readonly Boolean isFirstClient;
        private readonly WebSocketClient monitoringClient;

        public ClientHandler()
        {
        }

        public ClientHandler(Guid clientId, TcpClient client)
        {
            this.clientId = clientId;
            this.client = client;

            InitializeClient();
        }

        public ClientHandler(Guid clientId, TcpClient client, string infoModelString, bool isFirstClient, WebSocketClient monitoringClient)
        {
            this.clientId = clientId;
            this.client = client;
            this.infoModelString = infoModelString;
            this.isFirstClient = isFirstClient;
            this.monitoringClient = monitoringClient;

            InitializeClient();
        }

        public Guid ClientId
        {
            get { return clientId; }
            private set { value = clientId; }
        }
        public TcpClient Client
        {
            get { return client; }
            private set { value = client; }
        }

        public void InitializeClient()
        {
            Task.Run(() =>
            {
                var isHandshaked = false;
                byte[] rawMessage;
               
                using (NetworkStream stream = client.GetStream())
                {
                    while (true)
                    {
                        if (!isHandshaked)
                        {
                            rawMessage = new Byte[client.Available];
                            stream.Read(rawMessage, 0, rawMessage.Length);

                            var data = Encoding.UTF8.GetString(rawMessage);
                            if (new Regex("^GET").IsMatch(data))
                            {
                                HandShake(data, stream);
                                isHandshaked = true;

                                Console.WriteLine($"Client {clientId} succesfully handshaked");
                            }
                        }

                        Console.WriteLine($"Client {clientId} is waiting for incoming messages...");
                        while (!stream.DataAvailable)
                        {
                        }

                        rawMessage = new Byte[client.Available];
                        stream.Read(rawMessage, 0, rawMessage.Length);

                        var opCode = Recieve(rawMessage);
                        Console.WriteLine($"Client {clientId} received {opCode} OpCode");
                        if (opCode.Equals(OpCodes.RequestInfoModel.ToString()))
                        {
                            Send(infoModelString, stream); // send infoModel to client
                        //}
                        //if (opCode.Equals(OpCodes.RequestBroadcasting.ToString())) //|| isFirstClient
                        //{
                            Console.WriteLine($"Client {clientId} starts broadcasting because he was the first client in the session: {isFirstClient}");
                            Task.Run(() =>
                            {
                                while (true) // broadcast perfomanceModel to client
                                {
                                    //todo: waiting to recieve PerfomanceModel
                                    while (!monitoringClient.Stream.DataAvailable)
                                    {
                                    }

                                    var perfomanseModel = monitoringClient.Recieve();
                                    Console.WriteLine($"perfomanceModel {perfomanseModel} received from Monitoring.Service");
                                    
                                    //todo: send PerfomanceModel to client
                                    Send(perfomanseModel, stream);
                                    Thread.Sleep(5000);
                                }
                            });
                        }
                    }
                }
            });
        }
    }
}