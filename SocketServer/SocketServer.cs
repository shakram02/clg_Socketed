using System;
using System.Collections.Generic;
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
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private const int BufferSize = 1024;
        private const int MaxConnectionsInQueue = 10;

        public static void StartListening(int port = 3000)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());    // Avoid use GetHostEntry to stay away from IPv6s
            IPAddress ipAddress = ipHostInfo.AddressList.First(entry => !entry.IsIPv6LinkLocal);

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Console.WriteLine($"Listening to:{localEndPoint}");
            // Create a TCP/IP socket.
            TcpListener listener = new TcpListener(localEndPoint);

            try
            {
                listener.Start(MaxConnectionsInQueue);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    Console.WriteLine("[DEBUG]Waiting for a connection...");

                    var task = listener.AcceptTcpClientAsync();

                    Thread t = new Thread(async () => await HandleHttpRequest(task));
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

        private static async Task HandleHttpRequest(Task<TcpClient> task)
        {
            var client = await task;
            allDone.Set();  // Release semaphore for the threads
            var netStream = client.GetStream();
            StreamReader reader = new StreamReader(netStream, Encoding.ASCII, false, BufferSize, true);
            StreamWriter writer = new StreamWriter(netStream, Encoding.ASCII, BufferSize, true);
            char[] tempBuffer = new char[BufferSize];
            List<char> buffer = new List<char>(BufferSize);

            while (reader.Peek() > 0)
            {
                reader.Read(tempBuffer, 0, tempBuffer.Length);
                buffer.AddRange(tempBuffer.TakeWhile(b => b != 0));
            }

            string sb = new string(buffer.ToArray());
            string content = sb;

            // All the data has been read from the client. Display it on the console.
            Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
            Console.WriteLine($"[DEBUG]Read {content.Length} bytes from socket. Data :{Environment.NewLine} {content}");

            GenerateReplyToRequest(client.Client, writer, new RawHttpRequest(content));
            reader.Dispose();
            writer.Dispose();
            client.Close();
        }

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
            //else if (request.Type == HttpRequestType.Post)
            //{
            //    parser = new PostParser(request.Content);
            //    response = parser.ParseHttpRequest();
            //}
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

        //Called when the sending ends
        private static void CompleteSend(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine($"Sent {bytesSent} bytes to client.");

                // TODO HTTP 1.1 will allow presistent connections
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}