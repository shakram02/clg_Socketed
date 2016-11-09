using System;
using System.Collections.Generic;

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
        /// <summary>
        /// Splits the current byte[] into multiple array using a separator
        /// </summary>
        /// <param name="source">Source array</param>
        /// <param name="separator">Separator array for delimiting</param>
        /// <param name="partCount">Number of desired parts, 0 for all available parts</param>
        /// <returns></returns>
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
