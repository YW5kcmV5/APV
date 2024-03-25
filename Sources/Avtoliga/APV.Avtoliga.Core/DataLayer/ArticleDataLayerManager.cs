using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.DataLayer
{
    public class ArticleDataLayerManager : BaseDataLayerManager<ArticleEntity, ArticleCollection>
    {
        public ArticleCollection ListLatest(int top)
        {
            const string whereSql = @"ORDER BY CreatedAt DESC";
            return GetList(whereSql, top);
        }
    }
}