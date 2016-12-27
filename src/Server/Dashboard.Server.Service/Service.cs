using Dashboard.Server.WebSocket;
using System;

class Service
{
    static void Main(string[] args)
    {
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