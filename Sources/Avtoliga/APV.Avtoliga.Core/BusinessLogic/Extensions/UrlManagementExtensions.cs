using System;
using APV.Avtoliga.Core.Entities;

namespace APV.Avtoliga.Core.BusinessLogic.Extensions
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