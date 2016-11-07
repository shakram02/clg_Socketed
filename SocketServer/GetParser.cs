using Sharp.Functional;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace SocketServer
{
    public interface IHttpParser
    {
        HttpResponse ParseHttpRequest();
    }

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

        public HttpResponse ParseHttpRequest()
        {
            var requestedDirectory = GetRequestTargetPage();
            HttpResponse response;

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
                response = CreateResponseBody(requrestedFilesResults.StatusCode, requrestedFilesResults.FileName);
            }
            return response;
        }

        private Maybe<string> GetRequestTargetPage()
        {
            // Get the requested file name from the first line
            var match = Regex.Match(_request, "(?<=GET\\s+).+(?=\\s+HTTP)").Value;

            // Nothing requested, bad request format
            if (String.IsNullOrEmpty(match)) return new Maybe<string>();

            var pageName = match;
            return new Maybe<string>(pageName);
        }

        private HttpResult GetFileBytes(string path)
        {
            if (path == "/") path = "index.html";
            else if (path.StartsWith("/")) path = path.Replace("/", String.Empty);

            if (httpRootDirectory == null) return new HttpResult { FileName = path, StatusCode = HttpStatusCode.NotFound };

            string fileAbsPath = Path.Combine(httpRootDirectory, path);
            if (!File.Exists(fileAbsPath)) return new HttpResult { FileName = path, StatusCode = HttpStatusCode.NotFound };

            return new HttpResult
            {
                FileName = fileAbsPath,
                StatusCode = HttpStatusCode.OK
            };
        }

        private HttpResponse CreateResponseBody(HttpStatusCode statusCode, string filePath = null)
        {
            if (statusCode != HttpStatusCode.OK || filePath == null)
            {
                return new HttpResponse(statusCode);
            }
            return new HttpResponse(statusCode, filePath);
        }
    }
}