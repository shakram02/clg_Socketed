﻿using System;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the client...");
            Console.ReadKey();

            SocketClient.StartConnect();

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadKey();
        }
    }
}
