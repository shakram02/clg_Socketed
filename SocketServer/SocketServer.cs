using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServer
{
    public static class SocketServer
    {
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private const int BufferSize = 1024;
        private const int MaxConnectionsInQueue = 10;
        private const int port = 11000;

        public static void AcceptRequestCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject { WorkSocket = handler };
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadRequestCallback, state);
        }

        public static void StartListening()
        {
            byte[] bytes = new byte[BufferSize];

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList.First(entry => !entry.IsIPv6LinkLocal);

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Console.WriteLine($"Listening to:{localEndPoint}");
            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(MaxConnectionsInQueue);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    Console.WriteLine("Waiting for a connection...");

                    listener.BeginAccept(AcceptRequestCallback, listener);
                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void GenerateReplyToRequest(Socket handler, string content)
        {
            // Convert the string data to byte data using ASCII encoding.
            //byte[] replyData = Encoding.ASCII.GetBytes(content);

            var parser = new GetParser(content);


            var response = parser.ParseHttpGet();

            // Begin sending the data to the remote device.
            //var replyString = String.Join(" - ", strings).GetBytes();
            




            handler.BeginSend(response, 0, response.Length, 0, CompleteSend, handler);
        }

        // Invoked when reading request is complete
        private static void ReadRequestCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket from the asynchronous state object.
            StateObject stateObject = (StateObject)ar.AsyncState;
            Socket handler = stateObject.WorkSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead <= 0) return;

            stateObject.sb.Append(Encoding.ASCII.GetString(stateObject.Buffer, 0, bytesRead));

            // Check for end-of-file tag. If it is not there, read more data.
            content = stateObject.sb.ToString();

            if (content.IndexOf($"{Environment.NewLine}", StringComparison.Ordinal) > -1)
            {
                // All the data has been read from the client. Display it on the console.
                Console.WriteLine($"Read {content.Length} bytes from socket. Data :{Environment.NewLine} {content}");
                // Echo the data back to the client.
                GenerateReplyToRequest(handler, content);
            }
            else
            {
                // Not all data received. Get more.
                handler.BeginReceive(stateObject.Buffer, 0, StateObject.BufferSize, 0, ReadRequestCallback, stateObject);
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

    public static class StringExtensions
    {
        public static byte[] GetBytes(this string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        public static byte[] GetBytes(this string[] strings)
        {
            IEnumerable<byte> result = new byte[0];
            foreach (var s1 in strings)
            {
                result = result.Concat(s1.GetBytes());
            }
            return result.ToArray();
        }
    }
}