using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SocketClient
{
    internal class PostCreator
    {
        private readonly string _path;
        private IPAddress _currentAddress;
        private string _boundary = "";

        public PostCreator(string path, IPAddress currentAddress)
        {
            _path = path;
            _currentAddress = currentAddress;
            _boundary = "--3103";
        }

        public byte[] CreatePostRequest()
        {
            List<byte> postRequest = new List<byte>(256);
            byte[] content = File.ReadAllBytes(_path);
            byte[] doubleLine = Encoding.ASCII.GetBytes($"{Environment.NewLine}{Environment.NewLine}");
            string contentBoundary = "--" + _boundary;
            string postString =
                $"POST / HTTP/1.1{Environment.NewLine}Host: {_currentAddress}{Environment.NewLine}Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8{Environment.NewLine}Accept-Language: en-US,en;q=0.5{Environment.NewLine}Content-Type: multipart/form-data; boundary={_boundary}{Environment.NewLine}Content-Length: {content.Length}{Environment.NewLine}{Environment.NewLine}{contentBoundary}{Environment.NewLine}Content-Disposition: form-data; filename=\"{Path.GetFileName(_path)}\"{Environment.NewLine}{Environment.NewLine}";
            postRequest.AddRange(Encoding.ASCII.GetBytes(postString));
            postRequest.AddRange(content);
            postRequest.AddRange(doubleLine);

            return postRequest.ToArray();
        }
    }
}