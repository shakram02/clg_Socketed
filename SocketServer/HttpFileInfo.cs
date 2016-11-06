using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MimeTypes;

namespace SocketServer
{
    internal class HttpFileInfo
    {
        private readonly IList<string> _rawFileData;    // State object

        public HttpFileInfo(IList<string> rawFileData)
        {
            _rawFileData = rawFileData;
            ParseRawFileData();
            FileContent = ParseFileContent();
            BinaryWriter b = new BinaryWriter(File.Open(@"D:\server\eee" + DateTime.Now.ToString("dd-hh-mm-ss") + FileExtension,FileMode.CreateNew));
            b.Write(FileContent);
        }

        public byte[] FileContent { get; }

        public string FileName { private set; get; }

        public string FileExtension { private set; get; }

        private void ParseRawFileData()
        {
            List<string> fileData = new List<string>(8);

            int i = 0;
            string mimeType = String.Empty;
            while (_rawFileData[i] != String.Empty)
            {
                string line = _rawFileData[i];
                fileData.Add(line);
                _rawFileData.RemoveAt(i);

                if (line.StartsWith("Content-Type:"))
                {
                    mimeType = line.Replace("Content-Type:", String.Empty).Trim();
                }
            }
            FileName = fileData[0];
            FileExtension = MimeTypeMap.GetExtension(mimeType == String.Empty ? "text/plain" : mimeType);
        }

        private byte[] ParseFileContent()
        {
            if (_rawFileData.Count == 0) throw new InvalidOperationException("Malformed request");
            if (_rawFileData[0] == String.Empty) _rawFileData.RemoveAt(0);

            string content = string.Concat(_rawFileData);
            return content.GetBytes();
        }
    }
}