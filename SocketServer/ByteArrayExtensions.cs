using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public static class ByteArrayExtensions
    {
        static byte[] SeparateAndGetLast(byte[] source, byte[] separator)
        {
            for (var i = 0; i < source.Length; ++i)
            {
                if (Equals(source, separator, i))
                {
                    var index = i + separator.Length;
                    var part = new byte[source.Length - index];
                    Array.Copy(source, index, part, 0, part.Length);
                    return part;
                }
            }
            throw new Exception("not found");
        }

        public static byte[][] Split(this byte[] source, byte[] separator, int partCount = 0)
        {
            var parts = new List<byte[]>();
            var index = 0;
            byte[] part;
            for (var I = 0; I < source.Length; ++I)
            {
                if (Equals(source, separator, I))
                {
                    part = new byte[I - index];
                    Array.Copy(source, index, part, 0, part.Length);
                    parts.Add(part);

                    // Termination condition
                    if (partCount != 0 && partCount <= parts.Count) break;

                    index = I + separator.Length;
                    I += separator.Length - 1;
                }
            }
            //Copy the remaining elements
            part = new byte[source.Length - index];
            Array.Copy(source, index, part, 0, part.Length);
            parts.Add(part);
            return parts.ToArray();
        }



        static bool Equals(byte[] source, byte[] separator, int index)
        {
            for (int i = 0; i < separator.Length; ++i)
                if (index + i >= source.Length || source[index + i] != separator[i])
                    return false;
            return true;
        }
    }
}
