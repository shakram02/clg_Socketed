using System;

namespace SocketServer
{
    internal class Program
    {
        const int port = 3000;
        private static void Main(string[] args)
        {
            SocketServer.StartListening(port);

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadKey();
        }
    }
}