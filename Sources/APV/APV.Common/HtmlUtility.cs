using System;
using System.IO;
using System.Net;
using APV.Common.Extensions;

namespace APV.Common
{
    public static class HtmlUtility
    {
        private static WebRequest CreateHttpRequest(Uri url, int timeoutInMlsec = 5000)
        {
            WebRequest request = WebRequest.CreateHttp(url);
            request.Headers.Add("Accept-Language", "ru,en");
            request.Headers.Add("Accept-Charset", "utf-8");
            request.Timeout = timeoutInMlsec;
            return request;
        }

        public static string GetHtml(string url, bool nullIfNotFound = false, int attempts = 1)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            return GetHtml(new Uri(url), nullIfNotFound, attempts);
        }

        public static string GetHtml(this AbsoluteUri url, bool nullIfNotFound = false, int attempts = 1)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            return GetHtml(url.GetUri(), nullIfNotFound, attempts);
        }

        public static string GetHtml(this Uri url, bool nullIfNotFound = false, int attempts = 1)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            Func<string> action = () =>
                {
                    WebRequest request = CreateHttpRequest(url);
                    try
                    {
                        using (WebResponse response = request.GetResponse())
                        {
                            using (Stream responseStream = response.GetResponseStream())
                            {
                                if (responseStream == null)
                                {
                                    return null;
                                }
                                using (var reader = new StreamReader(responseStream))
                                {
                                    return reader.ReadToEnd();
                                }
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        if ((nullIfNotFound) && 
                            (ex.Response is HttpWebResponse httpResponse) && 
                            (httpResponse.StatusCode == HttpStatusCode.NotFound))
                        {
                            return null;
                        }
                        throw;
                    }
                };

            return Utility.Invoke(action, attempts);
        }

        public static byte[] GetData(this Uri url, int attempts = 1)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            Func<byte[]> action = () =>
                {
                    WebRequest request = CreateHttpRequest(url);
                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            return responseStream?.ToByteArray();
                        }
                    }
                };

            return Utility.Invoke(action, attempts);
        }
    }
}