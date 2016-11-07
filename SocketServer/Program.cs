using System;
using System.Threading.Tasks;

namespace SocketServer
{
    internal class Program
    {
        const int port = 3000;
        private static void Main(string[] args)
        {

            AsyncMain().Wait();
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadKey();
        }

        static async Task AsyncMain()
        {
            await SocketServer.StartListening(port);
        }
    }
}