using System;
using System.Linq;
using System.Net;

namespace SocketServer
{
    public class HttpResponse
    {
        private readonly byte[] _requestedFileCntent;
        public HttpStatusCode StatusCode { get; set; }


        private string _responseHeader { get; }

        public HttpResponse(HttpStatusCode statusCode, byte[] requestedFileCntent = null)
        {
            _requestedFileCntent = requestedFileCntent;
            StatusCode = statusCode;

            _responseHeader = $"HTTP/1.1 {(int)statusCode} {statusCode} {Environment.NewLine} DATE:{DateTime.Now}{Environment.NewLine}";

            // Add the blank line in case of sending data
            if (requestedFileCntent != null) _responseHeader += Environment.NewLine;
        }

        public byte[] Construct()
        {
            byte[] reply = _responseHeader.GetBytes();

            if (_requestedFileCntent != null)
            {
                reply = reply.Concat(_requestedFileCntent).ToArray();
            }

            return reply;
        }
    }
}