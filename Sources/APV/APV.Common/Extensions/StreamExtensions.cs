using System;
using System.IO;

namespace APV.Common.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            if (from is MemoryStream)
            {
                return ((MemoryStream)from).ToArray();
            }
            if (from.CanSeek)
            {
                from.Seek(0, SeekOrigin.Begin);
            }
            using (var copy = new MemoryStream())
            {
                from.CopyTo(copy);
                return copy.ToArray();
            }
        }
    }
}
