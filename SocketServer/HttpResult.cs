using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    internal class HttpResult
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

}
