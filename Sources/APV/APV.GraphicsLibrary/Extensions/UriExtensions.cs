using System;
using APV.Common;
using APV.GraphicsLibrary.Images;

namespace APV.GraphicsLibrary.Extensions
{
    public static class UriExtensions
    {
        public static ImageContainer GetImage(this Uri url, int attempts = 1)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            byte[] data = url.GetData(attempts);

            if ((data == null) || (data.Length == 0))
                throw new InvalidOperationException($"Data can not be douwloaded from url \"{url}\", response is empty.");

            return new ImageContainer(data);
        }
    }
}