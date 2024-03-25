using APV.Avtoliga.Core.DataLayer;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public class ArticleGroupManagement : BaseManagement<ArticleGroupEntity, ArticleGroupCollection, ArticleGroupDataLayerManager>
    {
        public static readonly ArticleGroupManagement Instance = (ArticleGroupManagement)EntityFrameworkManager.GetManagement<ArticleGroupEntity>();
    }
}