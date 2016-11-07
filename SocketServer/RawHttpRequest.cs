using System;
using System.Linq;

namespace SocketServer
{
    public enum HttpRequestType
    {
        Get,
        Post,
        Test
    }

    public class RawHttpRequest
    {
        public RawHttpRequest(string content)
        {
            Content = content;
            string requestType = String.Concat(content.TrimStart(' ').TakeWhile(ch => ch != ' ')).ToLower();

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

        public string Content { get; }
        public HttpRequestType Type { get; }
    }
}