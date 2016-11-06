using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServer
{
    public static class StringExtensions
    {
        public static byte[] GetBytes(this string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        public static byte[] GetBytes(this string[] strings)
        {
            IEnumerable<byte> result = new byte[0];
            foreach (var s1 in strings)
            {
                result = result.Concat(s1.GetBytes());
            }
            return result.ToArray();
        }
    }
}