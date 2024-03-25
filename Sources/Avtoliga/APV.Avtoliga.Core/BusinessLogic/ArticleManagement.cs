using APV.Avtoliga.Core.DataLayer;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public class ArticleManagement : BaseManagement<ArticleEntity, ArticleCollection, ArticleDataLayerManager>
    {
        [AnonymousAccess]
        public ArticleCollection ListLatest()
        {
            return DatabaseManager.ListLatest(5);
        }

        public static readonly ArticleManagement Instance = (ArticleManagement)EntityFrameworkManager.GetManagement<ArticleEntity>();
    }
}