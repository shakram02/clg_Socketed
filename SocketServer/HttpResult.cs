using System.Net;

namespace SocketServer
{
    internal class HttpResult
    {
        public byte[] FileBytes { get; set; }
        public string FileName { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}