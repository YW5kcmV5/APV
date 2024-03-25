using System;
using System.IO;
using System.IO.Compression;

namespace APV.TransControl.FuelViewer
{
    public static class Utility
    {
        public static string ExceptionToString(Exception ex)
        {
            return ExceptionToString(ex, true);
        }

        public static string ExceptionToString(Exception ex, bool addNewLine)
        {
            var cr = (addNewLine) ? Environment.NewLine : string.Empty;
            if (ex == null)
            {
                return "Empty exception!";
            }
            var message = string.Format("Message: '{0}' Type: '{1}'{2}", ex.Message, ex.GetType(), cr);
            message += string.Format(" StackTrace: '{0}'{1}", ex.StackTrace, cr);
            if (ex.InnerException != null)
            {
                message += string.Format(" InnerExceptionMessage:{0}{1}", cr, ExceptionToString(ex.InnerException));
            }
            message += cr;
            return message;
        }

        public static byte[] ToByte(Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream) stream).ToArray();
            }

            var resultStream = new MemoryStream();
            var buffer = new byte[4096];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                resultStream.Write(buffer, 0, read);
            }
            return resultStream.ToArray();
        }

        #region Compress
        
        public static byte[] CompressBytes(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Compress))
                {
                    deflateStream.Write(data, 0, data.Length);
                }
                return compressedStream.ToArray();
            }
        }
        
        public static byte[] DecompressBytes(byte[] data)
        {
            var compressedStream = new MemoryStream(data);
            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                var resultStream = new MemoryStream();
                var buffer = new byte[4096];
                int read;
                while ((read = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    resultStream.Write(buffer, 0, read);
                }
                return resultStream.ToArray();
            }
        }

        #endregion
    }
}