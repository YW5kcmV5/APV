using System.Web;
using APV.Common;

namespace APV.Avtoliga.Core.Application
{
    public static class SessionManager
    {
        public const string UserIdKey = @"UserId";
        public const string CountryCodeKey = @"CountryCode";
        public const string LanguageCodeKey = @"LanguageCode";
        public const string EditableKey = @"Editable";

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

        public static bool GetBool(string name)
        {
            object id = (SupportsSession) ? HttpContext.Current.Session[name] : null;
            return ((id is bool) && (bool) id);
        }

        public static void SetBool(string name, bool value)
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

        public static bool GetEditable()
        {
            return GetBool(EditableKey);
        }

        public static void SetEditable(bool editable)
        {
            SetBool(EditableKey, editable);
        }

        public static readonly bool SupportsSession = ((HttpContext.Current != null) && (HttpContext.Current.Session != null));
    }
}