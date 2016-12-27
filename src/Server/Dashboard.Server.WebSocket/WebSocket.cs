using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Server.WebSocket
{
    public class WebSocket
    {

        #region contructors

        public WebSocket()
        {

        }
        public WebSocket(string ip, short port)
        {
            this.ip = IPAddress.Parse(ip);
            this.port = port;
        }

        #endregion constructors

        #region private fields

        private IPAddress ip;
        private Int16 port;
        private Int16 clients = 0;

        #endregion private fields

        #region public properties

        public Int16 Clients { get { return clients; } private set { value = clients; } }

        #endregion public properties

        #region private methods

        private void HandShake(string data, NetworkStream stream)
        {
            Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                        + "Connection: Upgrade" + Environment.NewLine
                        + "Upgrade: websocket" + Environment.NewLine
                        + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                            SHA1.Create().ComputeHash(
                                Encoding.UTF8.GetBytes(
                                    new Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                )
                            )
                        ) + Environment.NewLine
                        + Environment.NewLine);

            stream.Write(response, 0, response.Length);
        }

        private string Recieve(Byte[] bytes)
        {
            var type = bytes[0];
            var secondByte = bytes[1];
            var lngth = secondByte & 127;

            var indexKeyStarts = 2;
            if (lngth == 126)
            {
                indexKeyStarts = 4;
            }
            else if (lngth == 127)
            {
                indexKeyStarts = 10;
            }
            Byte[] key = new byte[4];

            int k = 0;
            for (int i = indexKeyStarts; i < indexKeyStarts + 4; i++)
            {
                key[k] = bytes[i];
                k++;
            }

            var lengh = bytes.Length - indexKeyStarts - 4;
            Byte[] decoded = new Byte[lengh];

            for (int i = 0; i < lengh; i++)
            {
                decoded[i] = (Byte)(bytes[i + key.Length + indexKeyStarts] ^ key[i % 4]);
            }

            return Encoding.UTF8.GetString(decoded);
        }

        private void Send(string data, NetworkStream stream)
        {
            var rawData = Encoding.UTF8.GetBytes(data);
            Byte[] response = new Byte[rawData.Length + 20];

            response[0] = 129;
            int indexStartRawData = -1;

            if (rawData.Length <= 125)
            {
                response[1] = (byte)rawData.Length;
                indexStartRawData = 2;
            }
            else if (rawData.Length >= 126 && rawData.Length <= 65535)
            {
                response[1] = 126;
                response[2] = (byte)((rawData.Length >> 8) & 255);
                response[3] = (byte)(rawData.Length & 255);

                indexStartRawData = 4;
            }

            else
            {
                response[1] = 127;
                response[2] = (byte)((rawData.Length >> 56) & 255);
                response[3] = (byte)((rawData.Length >> 48) & 255);
                response[4] = (byte)((rawData.Length >> 40) & 255);
                response[5] = (byte)((rawData.Length >> 32) & 255);
                response[6] = (byte)((rawData.Length >> 24) & 255);
                response[7] = (byte)((rawData.Length >> 16) & 255);
                response[8] = (byte)((rawData.Length >> 8) & 255);
                response[9] = (byte)((rawData.Length) & 255);

                indexStartRawData = 10;
            }

            int j = 0;
            for (int i = indexStartRawData; i < rawData.Length + indexStartRawData; i++)
            {
                response[i] = rawData[j];
                j++;
            }

            stream.Write(response, 0, response.Length);
        }

        private void HandleClient(TcpClient client)
        {
            ++clients;

            Task.Run(() =>
            {
                using (NetworkStream stream = client.GetStream())
                {
                    while (true)
                    {
                        while (!stream.DataAvailable)
                        {
                        }

                        Byte[] bytes = new Byte[client.Available];
                        stream.Read(bytes, 0, bytes.Length);

                        var data = Encoding.UTF8.GetString(bytes);

                        if (new Regex("^GET").IsMatch(data))
                        {
                            HandShake(data, stream);
                            Console.WriteLine(data);
                            Console.WriteLine($"Client #{clients} connected");
                        }
                        else
                        {
                            data = Recieve(bytes);
                            Console.WriteLine(data);

                            for (int i = 0; i < 5; i++)
                            {
                                Send($"you said for the {i} time: {data}", stream);
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
            });
        }

        #endregion private methods

        #region public methods

        public async void Start()
        {
            TcpListener server = new TcpListener(ip, port);
            server.Start();

            // waiting for new connections
            while (true)
            {
                var client = await server.AcceptTcpClientAsync();
                
                Console.WriteLine($"Thread: {Thread.CurrentThread.ManagedThreadId}");

                HandleClient(client);
            }
        }

        #endregion public methods

    }
}
