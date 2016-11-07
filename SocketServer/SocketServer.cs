using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketServer
{
    public static class SocketServer
    {
        public const int ConnectionTimeOut = 3 * 1000;

        /// <summary>
        /// Signals the listening socket to resume whenever a connection was made
        /// </summary>
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private const int BufferSize = 1024;
        private const int MaxConnectionsInQueue = 10;

        /// <summary>
        /// Starts the server to listen for incomming connections
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static async Task StartListening(int port = 3000)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());    // Avoid use GetHostEntry to stay away from IPv6s
            IPAddress ipAddress = ipHostInfo.AddressList.First(entry => !entry.IsIPv6LinkLocal);

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Console.WriteLine($"Listening to:{localEndPoint}");
            // Create a TCP/IP socket.
            TcpListener listener = new TcpListener(localEndPoint);
            listener.Server.ReceiveTimeout = 10 * 1000;
            listener.Server.ReceiveBufferSize = BufferSize * BufferSize;
            try
            {
                listener.Start(MaxConnectionsInQueue);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    Console.WriteLine("[DEBUG]Waiting for a connection...");

                    var clientConnection = listener.AcceptTcpClientAsync();
                    Thread t = new Thread(() => HandleHttpRequest(clientConnection).ConfigureAwait(false));
                    t.Start();
                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Receives a task that resuslts in a TcpClient which will be used to handle requests
        /// </summary>
        /// <param name="task">Async task that completes when a connection is available</param>
        /// <returns></returns>
        private static async Task HandleHttpRequest(Task<TcpClient> task)
        {
            var client = await task;
            allDone.Set();  // Release semaphore for the threads
            client.ReceiveTimeout = ConnectionTimeOut;
            client.SendTimeout = ConnectionTimeOut;

            byte[] tempBuffer = new byte[client.Available];

            var netStream = client.GetStream();
            netStream.Read(tempBuffer, 0, tempBuffer.Length);

            if (tempBuffer.Length == 0)
            {
                client.Close();
                return;
            }

            StreamWriter writer = new StreamWriter(netStream, Encoding.ASCII, BufferSize, true);
            
            // All the data has been read from the client. Display it on the console.
            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
            Console.WriteLine($"[DEBUG]Read {tempBuffer.Length} bytes from socket.");
            Console.WriteLine(Environment.NewLine);

            GenerateReplyToRequest(client.Client, writer, new RawHttpRequest(tempBuffer));
            writer.Dispose();

            client.Close();
        }

        /// <summary>
        /// Generates reply using the socket stream
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="networkStreamWriter"></param>
        /// <param name="request"></param>
        private static void GenerateReplyToRequest(Socket clientSocket, StreamWriter networkStreamWriter, RawHttpRequest request)
        {
            // Echo the data back to the client.
            HttpResponse response;
            IHttpParser parser;
            if (request.Type == HttpRequestType.Get)
            {
                parser = new GetParser(request.Content);
                response = parser.ParseHttpRequest();
            }
            else if (request.Type == HttpRequestType.Post)
            {
                parser = new PostParser(request.Content);
                response = parser.ParseHttpRequest();
            }
            else
            {
                Console.WriteLine("[DEBUG]TEST Message received sucessfully...");
                var testReply = "[REPLY]Echo Complete...".ToCharArray();
                networkStreamWriter.Write(testReply, 0, testReply.Length);
                return;
            }
            if (response.RequestedFile == String.Empty)
            {
                networkStreamWriter.Write(response.ResponseHeader.ToCharArray(), 0, response.ResponseHeader.Length);
            }
            else
            {
                clientSocket.SendFile(response.RequestedFile, response.ResponseHeader.GetBytes(), null, TransmitFileOptions.UseDefaultWorkerThread);
            }
        }
    }
}