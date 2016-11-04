using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    /// <summary>
    /// Class to handle request state and data
    /// </summary>
    public class StateObject
    {
        // Size of receive buffer.
        public const int BufferSize = 1024;

        // Receive buffer.
        public byte[] Buffer { get; } = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb { get; } = new StringBuilder();

        // Client socket.
        public Socket WorkSocket { get; set; }
    }
}