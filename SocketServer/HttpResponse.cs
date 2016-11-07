using System;
using System.Net;

namespace SocketServer
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string RequestedFile { get; set; }

        public string ResponseHeader { get; }

        public HttpResponse(HttpStatusCode statusCode, string requestedFile = null)
        {
            StatusCode = statusCode;
            RequestedFile = requestedFile ?? string.Empty;
            ResponseHeader = $"HTTP/1.1 {(int)statusCode} {statusCode} {Environment.NewLine} DATE:{DateTime.Now}{Environment.NewLine}";

            // Add the blank line in case of sending data
            if (RequestedFile != String.Empty) ResponseHeader += Environment.NewLine;
        }
    }
}