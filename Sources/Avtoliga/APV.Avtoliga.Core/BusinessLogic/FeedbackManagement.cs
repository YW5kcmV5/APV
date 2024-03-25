using APV.Avtoliga.Common;
using APV.Avtoliga.Core.DataLayer;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public class FeedbackManagement : BaseManagement<FeedbackEntity, FeedbackCollection, FeedbackDataLayerManager>
    {
        [AnonymousAccess]
        public FeedbackEntity GetLatestFeedback()
        {
            return DatabaseManager.ListLatest(FeedbackType.Feedback, 1).FirstOrDefault();
        }

        [AnonymousAccess]
        public FeedbackCollection ListLatestFeedbacks()
        {
            return DatabaseManager.ListLatest(FeedbackType.Feedback, 10);
        }

        [AnonymousAccess]
        public FeedbackCollection ListArchiveFeedbacks()
        {
            return DatabaseManager.ListAll(FeedbackType.Feedback);
        }

        [AnonymousAccess]
        public int Like(long newsId)
        {
            FeedbackEntity entity = Find(newsId);
            if (entity != null)
            {
                int likes = entity.Likes ?? 0;
                likes++;
                entity.Likes = likes;
                entity.Save();
                return likes;
            }
            return -1;
        }

        [AnonymousAccess]
        public int Unlike(long newsId)
        {
            FeedbackEntity entity = Find(newsId);
            if ((entity != null) && (entity.Likes > 0))
            {
                int likes = entity.Likes ?? 0;
                likes--;
                entity.Likes = likes;
                entity.Save();
                return likes;
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

        public static readonly FeedbackManagement Instance = (FeedbackManagement)EntityFrameworkManager.GetManagement<FeedbackEntity>();
    }
}