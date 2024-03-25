using System;
using System.Globalization;
using System.Web;
using APV.Common;

namespace APV.Pottle.Core.BusinessLogic
{
    public static class CookiesManager
    {
        public static long GetId(string name)
        {
            if (SupportsCookies)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
                long id;
                if ((cookie != null) && (!string.IsNullOrEmpty(cookie.Value)) && (long.TryParse(cookie.Value, out id)))
                {
                    return id;
                }
            }
            return SystemConstants.UnknownId;
        }

        public static void SetId(string name, long id)
        {
            if (SupportsCookies)
            {
                var cookie = new HttpCookie(name, id.ToString(CultureInfo.InvariantCulture));
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public static string GetString(string name)
        {
            if (SupportsCookies)
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
                if ((cookie != null) && (!string.IsNullOrEmpty(cookie.Value)))
                {
                    return cookie.Value;
                }
            }
            return null;
        }

        public static void SetString(string name, string value)
        {
            if (SupportsCookies)
            {
                var cookie = new HttpCookie(name, value);
                cookie.Expires = DateTime.MaxValue;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public static readonly bool SupportsCookies = ((HttpContext.Current != null) && (HttpContext.Current.Request != null) && (HttpContext.Current.Response != null));
    }
}