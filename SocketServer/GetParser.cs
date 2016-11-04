using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SocketServer
{
    public class GetParser
    {
        private readonly string _request;
        public GetParser(string request)
        {
            _request = request;
        }

        public IEnumerable<string> GetRequestTargetPath()
        {
            var match = Regex.Match(_request, "(?<=GET).+(?=HTTP)");
            foreach (var group in match.Groups)
            {
                yield return group.ToString();
            }
        }

        public byte[] GetFileBytes(string path)
        {
            throw new NotImplementedException();
        }

        public byte[] CreateResponseBody()
        {
            // Check if the requested files are found, add the reposne state 200/404/...etc and the body, find
            // any links to other files recursively
            throw new NotImplementedException();
        }

    }
}
