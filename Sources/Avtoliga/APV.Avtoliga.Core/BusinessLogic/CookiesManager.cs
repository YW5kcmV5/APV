using System;
using System.Globalization;
using System.Web;
using APV.Common;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public static class CookiesManager
    {
        public static long GetId(string name)
        {
            string value = GetString(name);
            long id;
            if ((!string.IsNullOrEmpty(value)) && (long.TryParse(value, out id)))
            {
                return id;
            }
            return SystemConstants.UnknownId;
        }

        public static void SetId(string name, long id)
        {
            string value = id.ToString(CultureInfo.InvariantCulture);
            SetString(name, value);
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
                DateTime expires = DateTime.Now.AddYears(1).Date;

                bool set = true;
                HttpCookie cookie = HttpContext.Current.Request.Cookies[name];
                if (cookie == null)
                {
                    set = false;
                    cookie = new HttpCookie(name);
                }

                cookie.Expires = expires;
                cookie.HttpOnly = false;
                cookie.Secure = false;
                cookie.Value = value;

                if (set)
                {
                    HttpContext.Current.Response.Cookies.Set(cookie);
                }
                else
                {
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
            }
        }

        public static readonly bool SupportsCookies = ((HttpContext.Current != null) && (HttpContext.Current.Request != null) && (HttpContext.Current.Response != null));
    }
}