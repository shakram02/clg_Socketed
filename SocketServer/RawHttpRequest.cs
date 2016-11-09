using System;
using System.Text;

namespace SocketServer
{
    public enum HttpRequestType
    {
        Get,
        Post,
        Test
    }
    /// <summary>
    /// Incoming http request
    /// </summary>
    public class RawHttpRequest
    {
        public RawHttpRequest(byte[] content)
        {
            Content = content;
            string firstRequestLine = Encoding.ASCII.GetString(content.Split(Environment.NewLine.GetBytes(), 1)[0]);
            string requestType = firstRequestLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].ToLower();

            if (requestType == "get")
            {
                Type = HttpRequestType.Get;
            }
            else if (requestType == "post")
            {
                Type = HttpRequestType.Post;
            }
            else
            {
                Type = HttpRequestType.Test;
            }
        }

        public byte[] Content { get; }
        public HttpRequestType Type { get; }
    }
}