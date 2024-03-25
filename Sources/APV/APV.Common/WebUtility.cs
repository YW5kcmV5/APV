using System;
using System.Net;
using System.Web;

namespace APV.Common
{
    public static class WebUtility
    {
        public static bool IsUrlAvailable(string url, int timeoutInSeconds = 5)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));
            if (!AbsoluteUri.IsWellFormedAbsoluteUriString(url))
                throw new ArgumentOutOfRangeException(nameof(url), $"Url \"{url}\" is not weel formed absolute url.");

            Uri uri = new AbsoluteUri(url).GetUri();
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Timeout = 1000 * timeoutInSeconds;
            try
            {
                WebResponse webResponse = request.GetResponse();
                using (webResponse.GetResponseStream())
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static IPAddress GetRemoteAddress()
        {
            if (HttpContext.Current != null)
            {
                string ips = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                IPAddress ip;
                if (!string.IsNullOrEmpty(ips))
                {
                    string[] items = (ips + ",").Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    if ((items.Length > 0) && (!string.IsNullOrEmpty(items[0])) &&
                        (IPAddress.TryParse(items[0], out ip)))
                    {
                        return ip;
                    }
                }
                string value = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if ((!string.IsNullOrEmpty(value)) && (IPAddress.TryParse(value, out ip)))
                {
                    return ip;
                }
                value = HttpContext.Current.Request.UserHostAddress;
                if ((!string.IsNullOrEmpty(value)) && (IPAddress.TryParse(value, out ip)))
                {
                    return ip;
                }
            }
            return null;
        }
    }
}