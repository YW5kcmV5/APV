using System.Linq;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.UI.Web.Models;
using APV.Avtoliga.UI.Web.Models.Entities;
using APV.Common.Extensions;

namespace APV.Avtoliga.UI.Web.Controllers.API
{
    public static class ArticleController
    {
        public static ArticlesModel FindArticles(string groupId = null)
        {
            var model = new ArticlesModel();

            ArticleGroupEntity group = null;
            ArticleGroupEntity[] groups;
            ArticleEntity[] articles;

            if (!string.IsNullOrWhiteSpace(groupId))
            {
                long groupIdValue = groupId.ToLong(0);
                group = ArticleGroupManagement.Instance.Find(groupIdValue);
            }

            if (group != null)
            {
                groups = group.Children.ToArray();
                articles = group.Articles.ToArray();
            }
            else
            {
                groups = ArticleGroupManagement
                    .Instance
                    .GetAll()
                    .OfType<ArticleGroupEntity>()
                    .Where(articleGroup => (articleGroup.Top))
                    .OrderBy(articleGroup => articleGroup.Name)
                    .ToArray();

                articles = ArticleManagement
                    .Instance
                    .GetAll()
                    .OfType<ArticleEntity>()
                    .Where(article => (article.Alone))
                    .OrderByDescending(article => article.CreatedAt)
                    .ToArray();
            }

            ArticleGroupInfo articleGroupInfo = group.Transform(false);
            ArticleInfo[] articlesInfo = articles.Transform(false);
            ArticleGroupInfo[] groupsInfo = groups.Transform(false);

            model.ArticleGroup = articleGroupInfo;
            model.Articles = articlesInfo;
            model.Groups = groupsInfo;

            return model;
        }

        public static ArticleInfo FindArticle(string articleId)
        {
            if (!string.IsNullOrWhiteSpace(articleId))
            {
                long articleIdValue = articleId.ToLong(0);
                ArticleEntity article = ArticleManagement.Instance.Find(articleIdValue);
                if (article != null)
                {
                    return article.Transform(true);
                }
            }
            return null;
        }
    }
}