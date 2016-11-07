﻿using Sharp.Functional;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SocketServer
{
    public interface IHttpParser
    {
        HttpResponse ParseHttpRequest();
    }

    /// <summary>
    /// Responsible for handling and parsing get requests
    /// </summary>
    public class GetParser : IHttpParser
    {
        private readonly byte[] _request;
        private readonly string httpRootDirectory;

        public GetParser(byte[] request)
        {
            _request = request;

            string solutionDirectory = Path.GetDirectoryName(Path.GetDirectoryName(Environment.CurrentDirectory));
            if (solutionDirectory == null) throw new InvalidOperationException("Root folder can't be found");
            httpRootDirectory = Path.Combine(solutionDirectory, "www");
        }
        /// <summary>
        /// Parses the request then returns its result
        /// </summary>
        /// <returns></returns>
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

            var requrestedFilesResults = GetPageFullPathAndStatus(requestedDirectory.Value);
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

        /// <summary>
        /// Finds the target page / file in the incoming GET request
        /// </summary>
        /// <returns></returns>
        private Maybe<string> GetRequestTargetPage()
        {
            // Get the requested file name from the first line
            var match = Regex.Match(Encoding.ASCII.GetString(_request), "(?<=GET\\s+).+(?=\\s+HTTP)").Value;

            // Nothing requested, bad request format
            if (String.IsNullOrEmpty(match)) return new Maybe<string>();

            var pageName = match;
            return new Maybe<string>(pageName);
        }

        /// <summary>
        /// Finds the full path and the avilability of the requested file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private HttpResult GetPageFullPathAndStatus(string path)
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
        /// <summary>
        /// Creates a body that will form the response later in the process
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
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