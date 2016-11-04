using System;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketServer.StartListening();

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadKey();
        }
    }
}
