using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SocketServer
{
    /// <summary>
    /// Represents information about a file requested in an HTTP request
    /// </summary>
    public class HttpFile
    {
        private readonly byte[] _rawFileHeader;    // State object
        const int ByteBufferSize = 256;
        public HttpFile(byte[] rawFileHeader, byte[] rawFileContent)
        {
            _rawFileHeader = rawFileHeader;
            ParseRawFileData();
            FileContent = rawFileContent;
        }

        /// <summary>
        /// Byte array containing the file content
        /// </summary>
        public byte[] FileContent { get; }
        /// <summary>
        /// Full path of the file
        /// </summary>
        public string FileName { private set; get; }
        /// <summary>
        /// File extension
        /// </summary>
        public string FileExtension { private set; get; }

        /// <summary>
        /// Extracts information about the file from the incoming HTTP POST request
        /// </summary>
        private void ParseRawFileData()
        {
            string fileHeader = Encoding.ASCII.GetString(_rawFileHeader);
            var matches = Regex.Matches(fileHeader, "(?<=\")(.*?)(?=\")");
            var fileName = matches[matches.Count - 1].Value;

            if (!String.IsNullOrEmpty(fileName) && matches.Count > 1)
            {
                FileName = fileName;
                FileExtension = Path.GetExtension(FileName);
            }
            else
            {
                FileExtension = ".txt";
                FileName = !String.IsNullOrEmpty(fileName) ? fileName : DateTime.Now.ToString("yy-MM-dd-hh-mm-ss");
            }
        }
    }
}