using System;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using APV.Avtoliga.Common;
using APV.Avtoliga.Core.Application;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.Avtoliga.UI.Web.Models;
using APV.Avtoliga.UI.Web.Models.Entities;
using APV.Common;
using APV.EntityFramework.Interfaces;

namespace APV.Avtoliga.UI.Web.Controllers.API
{
    public static class MainController
    {
        public static MainModel GetModel(string type)
        {
            HelpType helpType;
            if ((string.IsNullOrWhiteSpace(type)) ||
                (!Enum.TryParse(type.Trim().Replace("_", string.Empty), true, out helpType)))
            {
                helpType = HelpType.None;
            }

            var model = new MainModel {Type = helpType};

            NewsEntity news = NewsManagement.Instance.GetLatest();
            NewsInfo newsInfo = news.Transform();

            FeedbackEntity feedback = FeedbackManagement.Instance.GetLatestFeedback();
            FeedbackInfo feedbackInfo = feedback.Transform();

            ArticleCollection articles = ArticleManagement.Instance.ListLatest();
            ArticleInfo[] articlesInfo = articles.Transform(true);

            model.LastNews = newsInfo;
            model.LastFeedback = feedbackInfo;
            model.LastArticles = articlesInfo;

            return model;
        }

        public static IUser User
        {
            get { return ContextManager.GetUser(); }
        }

        public static bool IsAuthenticated
        {
            get { return User.IsAuthenticated; }
        }

        public static bool Editable
        {
            get { return ((IsAuthenticated) && (SessionManager.GetEditable())); }
            set
            {
                bool editable = (value) && (IsAuthenticated);
                SessionManager.SetEditable(editable);
            }
        }

        public static string GetSiteRoot(UrlHelper helper)
        {
            if (helper == null)
                throw new ArgumentNullException("helper");

            string path = helper.Content("~");
            HttpRequest request = HttpContext.Current.Request;
            string absolute = request.Url.Scheme + "://" + request.Url.Authority + path;

            return absolute;
        }
    }
}