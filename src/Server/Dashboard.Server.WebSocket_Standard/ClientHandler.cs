using System;
using System.Collections.Concurrent;
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
        private readonly WebSocketClient monitoringClient;
        private readonly WebSocketServer server;
        bool disposed;
        private static ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();

        public ClientHandler()
        {
        }

        public ClientHandler(Guid clientId, TcpClient client, string infoModelString, WebSocketClient monitoringClient, WebSocketServer server)
        {
            this.clientId = clientId;
            this.client = client;
            this.infoModelString = infoModelString;
            this.monitoringClient = monitoringClient;
            this.server = server;

            InitializeClient();
        }

        public Guid ClientId => clientId;

        public TcpClient Client => client;

        private async void InitializeClient()
        {
            var tokenSource = new CancellationTokenSource();

            try
            {
                var t = Task.Factory.StartNew(() => 
                            RunMainTaskAsync(tokenSource), 
                                tokenSource.Token,
                                TaskCreationOptions.None, 
                                TaskScheduler.Default);
                tasks.Add(t);
                await t;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task RunMainTaskAsync(CancellationTokenSource tokenSource)
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

                    await WaitForMessageAsync(stream, tokenSource);
                    
                    //while (!stream.DataAvailable)
                    //{
                    //    if (tokenSource.Token.IsCancellationRequested)
                    //    {
                    //        Console.WriteLine($"Main Task #{Thread.CurrentThread.ManagedThreadId} for {clientId} ends");
                    //        tokenSource.Token.ThrowIfCancellationRequested();
                    //    }
                    //}

                    rawMessage = new Byte[client.Available];

                    try
                    {
                        stream.Read(rawMessage, 0, rawMessage.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    var opCode = Recieve(rawMessage);

                    Console.WriteLine($"Client {clientId} received {opCode} OpCode");
                    if (opCode.Equals(OpCodes.RequestInfoModel.ToString()))
                    {
                        Send(infoModelString, stream); // send infoModel to client
                        Console.WriteLine($"Client {clientId} starts broadcasting");

                        try
                        {
                            var t =  Task.Factory.StartNew(() =>
                                        Broadcasting(stream, tokenSource), tokenSource.Token,
                                            TaskCreationOptions.AttachedToParent,
                                            TaskScheduler.Default);
                            tasks.Add(t);
                            await t;
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"Client {clientId} ends it thread {Thread.CurrentThread.ManagedThreadId}");
                            break;
                        }
                    }
                }
            }
        }

        private void Broadcasting(NetworkStream stream, CancellationTokenSource tokenSource)
        {
            while (true) // broadcast perfomanceModel to client
            {
                if (!client.Connected) // cancel the task if client is disconnected
                {
                    tokenSource.Cancel();

                    Console.WriteLine($"Client {clientId} requested to disconnect #{Thread.CurrentThread.ManagedThreadId}");
                    server.Clients--;
                    if (server.Clients <= 0)
                    {
                        Console.WriteLine($"There are no more clients left connected to Server.Service, signalling Monitoring.Service to disconnect...");
                        monitoringClient.Signal(OpCodes.StopBroadcasting);
                    }
                    else
                    {
                        Console.WriteLine($"Now Client(s) {server.Clients}");
                    }
                    Console.WriteLine($"Broadcasting #{Thread.CurrentThread.ManagedThreadId} for {clientId} ends");

                    tokenSource.Token.ThrowIfCancellationRequested();
                }

                //if (!monitoringClient.Stream.DataAvailable) // consumes reasonable CPU, but not really need it on server side
                //    continue;

                //await WaitForMessageAsync(monitoringClient.Stream, tokenSource); //consumes to much CPU

                var perfomanceModel = monitoringClient.Recieve();

                Send(perfomanceModel, stream); // send PerfomanceModel to client
            }
        }

        private async Task WaitForMessageAsync(NetworkStream stream, CancellationTokenSource tokenSource)
        {
            await Task.Factory.StartNew(() =>
            {
                while (!stream.DataAvailable)
                {
                    if (!tokenSource.Token.IsCancellationRequested) continue;
                    Console.WriteLine($"Main Task #{Thread.CurrentThread.ManagedThreadId} for {clientId} ends");
                    tokenSource.Token.ThrowIfCancellationRequested();
                }
            }, tokenSource.Token);
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