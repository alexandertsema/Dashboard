using System;
using System.Net.Sockets;

namespace Dashboard.Server.WebSocket
{
    public class WebSocketServer : WebSocket
    {
        private Int16 clients = 0;
        private TcpListener server;

        public Int16 Clients
        {
            get { return clients; }
            set { clients = value; }
        }

        public TcpListener Server
        {
            get { return server; }
            set { value = server; }
        }

        public WebSocketServer(string ip, short port) : base(ip, port)
        {
        }

        public void Start()
        {
            server = new TcpListener(ip, port);
            server.Start();
        }
    }
}