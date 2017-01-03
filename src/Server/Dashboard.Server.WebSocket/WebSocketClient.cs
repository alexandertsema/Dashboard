using System;
using System.Net.Sockets;
using System.Text;

namespace Dashboard.Server.WebSocket
{
    public class WebSocketClient : WebSocket
    {
        private TcpClient client;
        private NetworkStream stream;

        public TcpClient Client
        {
            get { return client; }
            set { value = client; }
        }

        public NetworkStream Stream
        {
            get { return stream; }
            set { stream = value; }
        }

        public WebSocketClient(string ip, short port) : base(ip, port)
        {
        }

        public async void Connect()
        {
            client = new TcpClient();
            await client.ConnectAsync(ip, port);

            stream = client.GetStream();
        }

        public string Recieve()
        {
            var rawMessage = new Byte[client.Available];
            stream.Read(rawMessage, 0, rawMessage.Length);
            return Encoding.UTF8.GetString(rawMessage);
        }

        public void Signal(short opCode)
        {
            var response = Encoding.UTF8.GetBytes(opCode.ToString());
            stream.Write(response, 0, response.Length);
        }
    }
}