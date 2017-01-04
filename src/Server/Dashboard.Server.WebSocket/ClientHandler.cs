using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Server.WebSocket
{
    public class ClientHandler : WebSocket//, IDisposable
    {
        private readonly Guid clientId;
        private readonly TcpClient client;
        private readonly String infoModelString;
        private readonly Boolean isFirstClient;
        private readonly WebSocketClient monitoringClient;
        private readonly WebSocketServer server;
        bool disposed;
        private static ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();

        public ClientHandler()
        {
        }

        public ClientHandler(Guid clientId, TcpClient client)
        {
            this.clientId = clientId;
            this.client = client;

            InitializeClient();
        }

        public ClientHandler(Guid clientId, TcpClient client, string infoModelString, bool isFirstClient, WebSocketClient monitoringClient, WebSocketServer server)
        {
            this.clientId = clientId;
            this.client = client;
            this.infoModelString = infoModelString;
            this.isFirstClient = isFirstClient;
            this.monitoringClient = monitoringClient;
            this.server = server;

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

        private void InitializeClient()
        {
            
            var tokenSource = new CancellationTokenSource();
            var mainTask = Task.Factory.StartNew(() => RunMainTaskAsync(tokenSource, tasks), tokenSource.Token,
                TaskCreationOptions.None, TaskScheduler.Default);
            tasks.Add(mainTask);
            try
            {
                //mainTask.Wait(tokenSource.Token);
            }
            catch (AggregateException)
            {
                Console.WriteLine($"Main Task {mainTask.Id} ends");
            }
        }

        private void RunMainTaskAsync(CancellationTokenSource tokenSource, ConcurrentBag<Task> tasks)
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
                        if (tokenSource.Token.IsCancellationRequested)
                        {
                            Console.WriteLine($"Main Task #{Thread.CurrentThread.ManagedThreadId} for {clientId} ends");
                            tokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }
                    
                    rawMessage = new Byte[client.Available];
                    //try
                    //{
                        stream.Read(rawMessage, 0, rawMessage.Length);
                    //}
                    //catch (IOException exception)
                    //{
                    //    //tokenSource.Cancel(); //todo: kill the thread if client disconnected
                    //    if (tokenSource.Token.IsCancellationRequested)
                    //    {
                    //        Console.WriteLine($"Main Task #{Thread.CurrentThread.ManagedThreadId} for {clientId} ends");
                    //        tokenSource.Token.ThrowIfCancellationRequested();
                    //    }
                    //}

                    var opCode = Recieve(rawMessage);
                    Console.WriteLine($"Client {clientId} received {opCode} OpCode");
                    if (opCode.Equals(OpCodes.RequestInfoModel.ToString()))
                    {
                        Send(infoModelString, stream); // send infoModel to client
                        Console.WriteLine($"Client {clientId} starts broadcasting because he was the first client in the session: {isFirstClient}");

                        var task = Task.Factory.StartNew(() =>
                                Broadcasting(tokenSource, stream), tokenSource.Token, TaskCreationOptions.AttachedToParent,
                            TaskScheduler.Default);
                        tasks.Add(task);
                    }
                }
            }
        }

        private void Broadcasting(CancellationTokenSource tokenSource, NetworkStream stream)
        {
            while (true) // broadcast perfomanceModel to client
            {
                if (!client.Connected) // cancel the task if client is disconnected
                {
                    tokenSource.Cancel();

                    if (tokenSource.Token.IsCancellationRequested)
                    {
                        Console.WriteLine($"Client {clientId} requested to disconnect #{Thread.CurrentThread.ManagedThreadId}");
                        server.Clients--;
                        if (server.Clients <= 0)
                        {
                            Console.WriteLine($"There are no more clients left connected to Server.Service, signalling Monitoring.Service to disconnect...");
                            monitoringClient.Signal(OpCodes.StopBroadcasting);
                        }
                        Console.WriteLine($"Broadcasting #{Thread.CurrentThread.ManagedThreadId} for {clientId} ends");
                        tokenSource.Token.ThrowIfCancellationRequested();
                    }
                }

                //while (!monitoringClient.Stream.DataAvailable) // waiting to recieve PerfomanceModel
                //{
                //}

                if (!monitoringClient.Stream.DataAvailable)
                    continue;

                var perfomanceModel = monitoringClient.Recieve();
                //Console.WriteLine($"Client {clientId} received perfomanceModel: {perfomanceModel} from Monitoring.Service");
                
                Send(perfomanceModel, stream); // send PerfomanceModel to client
            }
        }

        // Public implementation of Dispose pattern callable by consumers.
        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //// Protected implementation of Dispose pattern.
        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposed)
        //        return;

        //    if (disposing)
        //    {
        //        // Free any other managed objects here.
        //        //
        //    }

        //    // Free any unmanaged objects here.
        //    //
        //    disposed = true;
        //}

        //~ClientHandler()
        //{
        //    Dispose(false);
        //}
    }

}