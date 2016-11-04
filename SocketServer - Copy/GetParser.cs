using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sharp.Functional;

namespace SocketServer
{
    public class GetParser
    {
        private readonly string _request;
        private string rootPath = Environment.CurrentDirectory;
        public GetParser(string request)
        {
            _request = request;
        }
        public byte[] ParseHttpGet()
        {
            var requestedDirectory = GetRequestTargetPaths();
            List<HttpResult> requrestedFilesResults = GetRequestTargetPaths().Select(GetFileBytes).ToList();
            byte[] response = CreateResponseBody(requrestedFilesResults);

            return response;
        }

        private IEnumerable<string> GetRequestTargetPaths()
        {
            var match = Regex.Match(_request, "(?<=GET).+(?=HTTP)");
            foreach (var group in match.Groups)
            {
                yield return group.ToString();
            }
        }

        private HttpResult GetFileBytes(string path)
        {
            string absolutePath = Path.Combine(rootPath, path);
            if (!File.Exists(absolutePath)) return new HttpResult { IsFound = false, FileName = path };

            return new HttpResult { IsFound = true, FileName = path, FileBytes = File.ReadAllBytes(absolutePath) };
        }

        private byte[] CreateResponseBody(IEnumerable<HttpResult> requestedBytes = null)
        {

            // Check if the requested files are found, add the reposne state 200/404/...etc and the body, find
            // any links to other files recursively
            throw new NotImplementedException();
        }



    }
}
