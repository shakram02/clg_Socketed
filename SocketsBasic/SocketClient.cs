using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClient
{
    public class SocketClient
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private const int BufferSize = 1024;
        //private readonly Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int port = 3000;

        public static void StartConnect(string inputIpAddress = null, int portNumber = 3000)
        {
            try
            {
                IPAddress ipAddress;
                if (!string.IsNullOrEmpty(inputIpAddress))
                {
                    bool parseSuccess = IPAddress.TryParse(inputIpAddress, out ipAddress);
                    if (!parseSuccess) throw new InvalidOperationException("IP Address is invalid");
                }
                else
                {
                    // Gets the IP entry of the client
                    IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());
                    ipAddress = hostInfo.AddressList.First(entry => !entry.IsIPv6LinkLocal);
                }

                // Specify the connection target as the addres of this pc
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, port);

                try
                {
                    // Encode the data string into a byte array.
                    while (true)
                    {
                        TcpClient client = new TcpClient
                        {
                            NoDelay = true,
                            SendTimeout = 1024 * 1024
                        };
                        char[] tempBuffer = new char[BufferSize];
                        List<char> from = new List<char>(BufferSize);

                        Console.WriteLine("Input a message to send");
                        string message = Console.ReadLine();
                        if (message != null && message.ToLower() == "exit") break;

                        byte[] to = Encoding.ASCII.GetBytes(message + "\0");

                        if (!client.Connected)
                        {
                            client.Connect(remoteEndPoint);
                            Console.WriteLine($"[DEBUG]Socket connected to {client.Client.LocalEndPoint}");
                        }
                        var netStream = client.GetStream();
                        if (message.StartsWith("POST"))
                        {
                            var filePath = Regex.Match(message, "(?<=POST\\s+).+(?=\\s+HTTP)").Value;
                            var postCreator = new PostCreator(filePath, ipAddress);
                            var requestBytes = postCreator.CreatePostRequest();
                            netStream.Write(requestBytes, 0, requestBytes.Length);

                        }
                        else
                        {
                            netStream.Write(to, 0, to.Length);
                        }


                        if (!client.Connected)
                        {
                            client.Connect(remoteEndPoint);
                            Console.WriteLine($"[DEBUG]Socket connected to {client.Client.LocalEndPoint}");
                        }

                        Console.WriteLine($"[DEBUG]Echoed test => {Encoding.ASCII.GetString(to)}");
                        if (client.Available == 0)
                        {
                            client.Close();
                            continue;
                        }
                        using (StreamReader reader = new StreamReader(netStream, Encoding.ASCII, false, BufferSize, true))
                        {
                            reader.Read(tempBuffer, 0, BufferSize);
                            from.AddRange(tempBuffer);
                            from.TrimExcess();
                            string data = new string(from.ToArray()).TrimEnd();
                            //var data = String.Concat(from).Trim();
                            Console.WriteLine("[DEBUG] Received:" + data);
                        }
                        client.Close();
                    }
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

        public static void SendDataToServer(byte[] content, NetworkStream netStream)
        {
            allDone.Set();
        }

        private static async Task ReadDataFromServer(NetworkStream netStream)
        {
            byte[] tempBuffer = new byte[BufferSize];
            List<byte> from = new List<byte>(BufferSize);

            await netStream.ReadAsync(tempBuffer, 0, from.Count);
            while (netStream.DataAvailable)
            {
                from.AddRange(tempBuffer);
                netStream.Read(tempBuffer, 0, from.Count);
            }
            from.TrimExcess();
            var data = String.Concat(from);
            Console.WriteLine("[DEBUG] Received:" + data);

            allDone.Set();
        }

        private void Terminate()
        {
            // Release the socket.
            //sender.Shutdown(SocketShutdown.Both);
            //sender.Close();
        }
    }
}