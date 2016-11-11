using System.Text;

namespace SocketServer
{
    public static class StringExtensions
    {
        public static byte[] GetBytes(this string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }
    }
}