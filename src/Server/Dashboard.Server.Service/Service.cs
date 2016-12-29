using Dashboard.Server.WebSocket;
using System;
using System.Net.Sockets;

//using TestSt6andart.Test;

class Service
{
    static void Main(string[] args)
    {
        System.Net.Sockets.TcpClient clientSocket = new System.Net.Sockets.TcpClient();
        NetworkStream serverStream;
        clientSocket.ConnectAsync("127.0.0.1", 8888);
        serverStream = clientSocket.GetStream();
        serverStream.DataAvailable
        WebSocket server = new WebSocket("127.0.0.1", 2222);
        try
        {
            server.Start();
            Console.ReadLine();
        }
        catch (Exception exception)
        {
            throw exception;
        }
        
    }
}