using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace SocketServer
{
    public class PostParser : IHttpParser
    {
        private string content;

        public PostParser(string content)
        {
            this.content = content;
        }

        public HttpResponse ParseHttpRequest()
        {
            var files = ExtractFileData();

            return new HttpResponse(HttpStatusCode.Created);
        }

        private List<HttpFileInfo> ExtractFileData()
        {
            var cont = this.content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            List<string> fileData = new List<string>(16);
            List<HttpFileInfo> infos = new List<HttpFileInfo>();

            // Find the boundary of the files
            string contentBoundary = "--" + Regex.Match(this.content, "(?<=Content-Type: multipart/form-data; boundary=).*").Value.TrimEnd('\r');

            for (int i = 0; i < cont.Length; i++)
            {
                if (cont[i] != (contentBoundary))
                {
                    continue;
                }

                while (i < cont.Length)
                {
                    if (cont[i] != contentBoundary)
                        fileData.Add(cont[i]);

                    if (i + 1 >= cont.Length || cont[i + 1] == contentBoundary) break;
                    i++;
                }

                if (fileData.Count > 0)
                {
                    // Extract each data rows into one HttpFileInfo object, then clear the fileData
                    // list to prepare for the next data chunk
                    infos.Add(new HttpFileInfo(fileData));
                    fileData.Clear();
                }
            }
            return new List<HttpFileInfo>(infos);
        }
    }
}