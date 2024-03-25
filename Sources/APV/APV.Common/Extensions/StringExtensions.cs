using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace APV.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Hash SHA1
        /// </summary>
        public static byte[] Hash1(this string data, Encoding encoding = null)
        {
            return Utility.Hash1(data, encoding);
        }

        /// <summary>
        /// Hash SHA256
        /// </summary>
        public static byte[] Hash256(this string data, Encoding encoding = null)
        {
            return Utility.Hash256(data, encoding);
        }

        public static string GetChecksum(this string value)
        {
            return Utility.GetChecksum(value);
        }

        public static byte[] ToByteArray(this string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            if (data.StartsWith("0x"))
            {
                data = data.Substring(2);
            }

            int numberChars = data.Length;
            var bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(data.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static XmlDocument ToXmlDocument(this string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new ArgumentNullException(nameof(xml));

            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
            }
            catch (XmlException)
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    ms.Flush();
                    ms.Position = 0;
                    doc.Load(ms);
                }
            }
            return doc;
        }

        public static string ToPascalCase(this string value, bool join = false)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            if (!join)
            {
                return value.Substring(0, 1).ToUpperInvariant() + value.Substring(1).ToLowerInvariant();
            }
            string[] items = value.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(string.Empty, items.Select(x => x.ToPascalCase()));
        }

        public static int GetWordsCount(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }
            string[] items = value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            return items.Length;
        }

        public static long ToLong(this string value, long defaultValue)
        {
            long longValue;
            if ((!string.IsNullOrWhiteSpace(value)) && (long.TryParse(value, out longValue)))
            {
                return longValue;
            }
            return defaultValue;
        }

        public static bool IsValidEmail(this string email)
        {
            return Utility.IsValidEmail(email);
        }
    }
}