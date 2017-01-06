using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Server.WebSocket
{
    public abstract class WebSocket
    {

        #region contructors

        protected WebSocket()
        {

        }

        protected WebSocket(string ip, short port)
        {
            this.ip = IPAddress.Parse(ip);
            this.port = port;
        }

        #endregion constructors

        #region private fields

        protected readonly IPAddress ip;
        protected readonly Int16 port;

        #endregion private fields

        #region protected methods

        protected void HandShake(string data, NetworkStream stream)
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

        protected string Recieve(byte[] encodedMessage) => Decode(encodedMessage);
        
        protected void Send(string message, NetworkStream stream)
        {
            var response = Encode(message);
            try
            {
                stream.Write(response, 0, response.Length);
            }
            catch (IOException exception)
            {
                //todo: kill current task: client disconnected
                stream.FlushAsync();
            }
        }

        #endregion protected methods

        #region private methods

        private byte[] Encode(string message)
        {
            var rawMessage = Encoding.UTF8.GetBytes(message);
            Byte[] response = null;

            int indexStartRawData;

            if (rawMessage.Length <= 125)
            {
                indexStartRawData = 2;

                response = new Byte[rawMessage.Length + indexStartRawData];

                response[1] = (byte)rawMessage.Length;
            }
            else if (rawMessage.Length >= 126 && rawMessage.Length <= 65535)
            {
                indexStartRawData = 4;

                response = new Byte[rawMessage.Length + indexStartRawData];

                response[1] = 126;
                response[2] = (byte)((rawMessage.Length >> 8) & 255);
                response[3] = (byte)(rawMessage.Length & 255);
            }
            else
            {
                indexStartRawData = 10;

                response = new Byte[rawMessage.Length + indexStartRawData];

                response[1] = 127;
                response[2] = (byte)((rawMessage.Length >> 56) & 255);
                response[3] = (byte)((rawMessage.Length >> 48) & 255);
                response[4] = (byte)((rawMessage.Length >> 40) & 255);
                response[5] = (byte)((rawMessage.Length >> 32) & 255);
                response[6] = (byte)((rawMessage.Length >> 24) & 255);
                response[7] = (byte)((rawMessage.Length >> 16) & 255);
                response[8] = (byte)((rawMessage.Length >> 8) & 255);
                response[9] = (byte)((rawMessage.Length) & 255);
            }

            response[0] = 129; // 129 represents text type

            for (int i = indexStartRawData, j = 0; i < rawMessage.Length + indexStartRawData; i++, j++)
            {
                response[i] = rawMessage[j];
            }

            return response;
        }

        private string Decode(byte[] encodedMessage)
        {
            var typeByte = encodedMessage[0];
            var messageLength = encodedMessage[1] & 127;

            var indexKeyStarts = 2;
            if (messageLength == 126)
            {
                indexKeyStarts = 4;
            }
            else if (messageLength == 127)
            {
                indexKeyStarts = 10;
            }

            Byte[] key = new Byte[4];
            Array.Copy(encodedMessage, indexKeyStarts, key, 0, key.Length);

            Byte[] decodedMessage = new Byte[messageLength];

            for (int i = 0; i < messageLength; i++)
            {
                decodedMessage[i] = (Byte)(encodedMessage[i + key.Length + indexKeyStarts] ^ key[i % 4]);
            }

            return Encoding.UTF8.GetString(decodedMessage);
        }

        #endregion private methods
        
    }
}