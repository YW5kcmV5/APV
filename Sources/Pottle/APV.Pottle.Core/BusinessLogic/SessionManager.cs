using System.Web;
using APV.Common;

namespace APV.Pottle.Core.BusinessLogic
{
    public static class SessionManager
    {
        public const string UserIdKey = @"UserId";
        public const string CountryCodeKey = @"CountryCode";
        public const string LanguageCodeKey = @"LanguageCode";

        public static long GetId(string name)
        {
            object id = (SupportsSession) ? HttpContext.Current.Session[name] : null;
            return (id is long) ? ((long)id) : SystemConstants.UnknownId;
        }

        public static void SetId(string name, long id)
        {
            if (SupportsSession)
            {
                HttpContext.Current.Session[name] = id;
            }
        }

        public static string GetString(string name)
        {
            object id = (SupportsSession) ? HttpContext.Current.Session[name] : null;
            return id as string;
        }

        public static void SetString(string name, string value)
        {
            if (SupportsSession)
            {
                HttpContext.Current.Session[name] = value;
            }
        }

        public static long GetUserId()
        {
            return GetId(UserIdKey);
        }

        public static void SetUserId(long userId)
        {
            SetId(UserIdKey, userId);
        }

        public static readonly bool SupportsSession = ((HttpContext.Current != null) && (HttpContext.Current.Session != null));
    }
}