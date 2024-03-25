using System;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Core.BusinessLogic.Extensions
{
    public static class UrlManagementExtensions
    {
        public static string GetUrl(this UrlEntity url)
        {
            return UrlManagement.Instance.GetUrl(url);
        }

        public static Uri GetUri(this UrlEntity url)
        {
            return UrlManagement.Instance.GetUri(url);
        }

        public static void Verify(this UrlEntity url)
        {
            UrlManagement.Instance.Verify(url);
        }
    }
}