﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SocketServer
{
    public class PostParser : IHttpParser
    {
        private readonly byte[] _content;

        public PostParser(byte[] content)
        {
            _content = content;
        }

        /// <summary>Returns the result of the POST request after processing it</summary>
        /// <returns></returns>
        public HttpResponse ParseHttpRequest()
        {
            var files = ExtractFileData();
            var response = new HttpResponse(HttpStatusCode.Created);

            FileManager.WriteFilesToDisk(files);
            return response;
        }

        /// <summary>Parses Raw data from the http request into a more appropriate format</summary>
        /// <returns>List of files in the request</returns>
        private List<HttpFile> ExtractFileData()
        {
            byte[] doubleBlankLine = Encoding.ASCII.GetBytes($"{Environment.NewLine}{Environment.NewLine}");
            // Extract the request header and get the boundary delimeter
            var cont = _content.Split(doubleBlankLine, 1);
            string strContent = Encoding.ASCII.GetString(cont.TakeWhile(arr => arr.Length != 0).SelectMany(ar => ar).ToArray());
            string contentBoundary = "--" + Regex.Match(strContent, "(?<=Content-Type: multipart/form-data; boundary=).*").Value.TrimEnd('\r');
            byte[] contentSeparator = contentBoundary.GetBytes();

            // Split the request using the boundary
            var cont1 = _content.Split(contentSeparator);
            List<HttpFile> infos = new List<HttpFile>();

            // Iterate the request parts skipping the headers
            foreach (var b in cont1.Skip(1))
            {
                // Split the contents using the double blank line
                var dataChunks = b.Split(doubleBlankLine);

                if (dataChunks.Length < 1) continue;
                var header = dataChunks[0].Skip(2).ToArray();

                if (dataChunks.Length < 2) continue;
                var rawFileContent = dataChunks[1];

                if (rawFileContent.Length == 0) continue;

                byte[] filteredRawFileContent = new byte[rawFileContent.Length - 2];
                Array.Copy(rawFileContent, filteredRawFileContent, filteredRawFileContent.Length);
                infos.Add(new HttpFile(header, filteredRawFileContent));
            }

            return new List<HttpFile>(infos);
        }
    }
}