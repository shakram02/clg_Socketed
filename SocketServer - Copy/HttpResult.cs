using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    internal class HttpResult
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public string StatusCode { get; set; }

        public bool IsFound { get; set; }
    }

}
