using APV.Avtoliga.Core.DataLayer;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public class NewsManagement : BaseManagement<NewsEntity, NewsCollection, NewsDataLayerManager>
    {
        [AnonymousAccess]
        public NewsEntity GetLatest()
        {
            return DatabaseManager.ListLatest(1).FirstOrDefault();
        }

        [AnonymousAccess]
        public NewsCollection ListLatest()
        {
            return DatabaseManager.ListLatest(10);
        }

        [AnonymousAccess]
        public NewsCollection ListArchive()
        {
            return DatabaseManager.ListAll();
        }

        [AnonymousAccess]
        public int Like(long newsId)
        {
            NewsEntity entity = Find(newsId);
            if (entity != null)
            {
                entity.Likes++;
                entity.Save();
                return entity.Likes;
            }
            return -1;
        }

        [AnonymousAccess]
        public int Unlike(long newsId)
        {
            NewsEntity entity = Find(newsId);
            if ((entity != null) && (entity.Likes > 0))
            {
                entity.Likes--;
                entity.Save();
                return entity.Likes;
            }
            return -1;
        }

        [AnonymousAccess]
        public int Set(long newsId, bool liked)
        {
            return (liked)
                       ? Like(newsId)
                       : Unlike(newsId);
        }

        public static readonly NewsManagement Instance = (NewsManagement)EntityFrameworkManager.GetManagement<NewsEntity>();
    }
}