using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace APV.Avtoliga.UI.Web
{
    public static class HtmlHelpers
    {
        public static string GetImageUrl(long? imageId)
        {
            imageId = imageId ?? 1;
            return FormatUrl(string.Format("/image/{0}", imageId));
        }

        public static string ToJsonString(this object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return HttpUtility.JavaScriptStringEncode(new JavaScriptSerializer().Serialize(entity));
        }

        public static T FromJsonString<T>(this string json)
        {
            if (string.IsNullOrEmpty(json))
                throw new ArgumentNullException("json");

            return (T)new JavaScriptSerializer().Deserialize(json, typeof(T));
        }

        public static string GetDomain()
        {
            HttpRequest request = HttpContext.Current.Request;
            RequestContext context = request.RequestContext;
            var urlHelper = new UrlHelper(context);
            var domain = new Uri(request.Url, urlHelper.Content(@"~"));
            return domain.ToString();
        }

        public static string FormatUrl(string relativeUrl)
        {
            relativeUrl = (!string.IsNullOrWhiteSpace(relativeUrl)) ? relativeUrl : "";
            if (relativeUrl.StartsWith(@"~/"))
            {
                relativeUrl = relativeUrl.Substring(2);
            }
            if (relativeUrl.StartsWith("/"))
            {
                relativeUrl = relativeUrl.Substring(1);
            }
            relativeUrl = "~/" + relativeUrl;
            HttpRequest request = HttpContext.Current.Request;
            RequestContext context = request.RequestContext;
            var urlHelper = new UrlHelper(context);
            var domain = new Uri(request.Url, urlHelper.Content(relativeUrl));
            return domain.ToString();
        }
    }
}