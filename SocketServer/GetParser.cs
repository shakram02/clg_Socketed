using Sharp.Functional;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace SocketServer
{
    public class GetParser : IHttpParser
    {
        private readonly string _request;

        private readonly string httpRootDirectory;

        public GetParser(string request)
        {
            _request = request;

            string solutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Environment.CurrentDirectory));
            if (solutionDirectory == null) throw new InvalidOperationException("Root folder can't be found");
            httpRootDirectory = Path.Combine(solutionDirectory, "www");
        }

        public byte[] ParseHttpRequest()
        {
            var requestedDirectory = GetRequestTargetPage();
            byte[] response;

            // Bad request recieved, the request is on an invalid format
            if (requestedDirectory.HasNoValue)
            {
                response = CreateResponseBody(HttpStatusCode.BadRequest);
                return response;
            }

            var requrestedFilesResults = GetFileBytes(requestedDirectory.Value);
            if (requrestedFilesResults.StatusCode != HttpStatusCode.OK)
            {
                response = CreateResponseBody(requrestedFilesResults.StatusCode);
            }
            else
            {
                response = CreateResponseBody(requrestedFilesResults.StatusCode, requrestedFilesResults.FileBytes);
            }
            return response;
        }

        private byte[] CreateResponseBody(HttpStatusCode statusCode, byte[] fileContent = null)
        {
            string body = $"HTTP/1.1 {(int)statusCode} {statusCode} {Environment.NewLine} DATE:{DateTime.Now}{Environment.NewLine}";

            if (statusCode != HttpStatusCode.OK || fileContent == null)
            {
                return body.GetBytes();
            }

            body += Environment.NewLine;    // A blank line must be added between the data and headers
            List<byte> responseBytes = body.GetBytes().ToList();
            responseBytes.AddRange(fileContent);

            return responseBytes.ToArray();
        }

        private HttpResult GetFileBytes(string path)
        {
            if (path == "/") path = "index.html";
            else if (path.StartsWith("/")) path = path.Replace("/", String.Empty);

            if (httpRootDirectory == null) return new HttpResult { FileName = path, StatusCode = HttpStatusCode.NotFound };

            string fileAbsPath = Path.Combine(httpRootDirectory, path);
            if (!File.Exists(fileAbsPath)) return new HttpResult { FileName = path, StatusCode = HttpStatusCode.NotFound };

            return new HttpResult { FileName = path, FileBytes = File.ReadAllBytes(fileAbsPath), StatusCode = HttpStatusCode.OK };
        }

        private Maybe<string> GetRequestTargetPage()
        {
            var match = Regex.Match(_request, "(?<=GET\\s+).+(?=\\s+HTTP)");
            if (match.Groups.Count == 0) return new Maybe<string>();

            var pageName = match.Groups[0].ToString();
            // Nothing requested, bad request format
            return new Maybe<string>(pageName);
        }
    }

    public interface IHttpParser
    {
        byte[] ParseHttpRequest();
    }
}