
namespace APV.Common.Extensions
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Hash SHA1
        /// </summary>
        public static byte[] Hash1(this byte[] data)
        {
            return Utility.Hash1(data);
        }

        /// <summary>
        /// Hash SHA256
        /// </summary>
        public static byte[] Hash256(this byte[] data)
        {
            return Utility.Hash256(data);
        }

        public static string ToHexString(this byte[] data)
        {
            return Utility.ToHexString(data);
        }

        public static string GetChecksum(this byte[] value)
        {
            return Utility.GetChecksum(value);
        }

        public static byte[] Copy(this byte[] from)
        {
            return Utility.Copy(from);
        }
    }
}
