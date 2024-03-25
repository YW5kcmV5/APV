using System;
using APV.Common;
using APV.GraphicsLibrary.Images;

namespace APV.GraphicsLibrary.Extensions
{
    public static class AbsoluteUriExtensions
    {
        public static ImageContainer GetImage(this AbsoluteUri url, int attempts = 1)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            Uri uri = url.GetUri();
            return uri.GetImage(attempts);
        }
    }
}