using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketClient
{
    public class SocketClient
    {
        private const int BufferSize = 1024;
        //private readonly Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] data = new byte[BufferSize];
        const int port = 11000;

        public static void StartConnect()
        {
            try
            {
                // Gets the IP entry of the client
                IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = hostInfo.AddressList.First(entry=> !entry.IsIPv6LinkLocal);
                // Specify the connection target as the addres of this pc
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, port);
                Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEndPoint);
                    Console.WriteLine($"Socket connected to {sender.RemoteEndPoint}");

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(data);
                    Console.WriteLine($"Echoed test = {Encoding.ASCII.GetString(data, 0, bytesRec)}");

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"An error occurred while sending the message:{exc}");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"An error occurred while connecting to the server:{exc}");
            }
        }

        private void Terminate()
        {
            // Release the socket.
            //sender.Shutdown(SocketShutdown.Both);
            //sender.Close();
        }
    }
}