using System.Web;
using APV.Avtoliga.Core.BusinessLogic;

namespace APV.Avtoliga.UI.Web.Controllers.API
{
    public static class NewsController
    {
        private static string GetCookieId(long newsId)
        {
            return string.Format("News#{0}", newsId);
        }

        public static bool GetLike(long newsId)
        {
            if (HttpContext.Current != null)
            {
                string cookieId = GetCookieId(newsId);
                string likedValue = CookiesManager.GetString(cookieId);
                return (likedValue == "1");
            }
            return false;
        }

        public static int SetLike(long newsId, bool liked)
        {
            int likes = -1;
            if (HttpContext.Current != null)
            {
                likes = NewsManagement.Instance.Set(newsId, liked);
                string likedValue = (liked) ? "1" : "0";
                string cookieId = GetCookieId(newsId);
                CookiesManager.SetString(cookieId, likedValue);
            }
            return likes;
        }
    }
}