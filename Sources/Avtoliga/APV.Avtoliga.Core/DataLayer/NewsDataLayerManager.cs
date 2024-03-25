using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.DataLayer
{
    public class NewsDataLayerManager : BaseDataLayerManager<NewsEntity, NewsCollection>
    {
        public NewsCollection ListLatest(int top)
        {
            const string whereSql = @"ORDER BY CreatedAt DESC";
            return GetList(whereSql, top);
        }

        public NewsCollection ListAll()
        {
            const string whereSql = @"ORDER BY CreatedAt DESC";
            return GetList(whereSql);
        }
    }
}