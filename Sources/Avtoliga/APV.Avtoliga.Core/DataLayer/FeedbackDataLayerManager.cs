using System.Collections.Generic;
using APV.Avtoliga.Common;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.DataLayer
{
    public class FeedbackDataLayerManager : BaseDataLayerManager<FeedbackEntity, FeedbackCollection>
    {
        public FeedbackCollection ListLatest(FeedbackType feedbackType, int top)
        {
            const string whereSql = @"WHERE [Feedback].[Type] = @FeedbackType ORDER BY CreatedAt DESC";
            var @params = new Dictionary<string, object> { { "@FeedbackType", (int)feedbackType } };
            return GetList(whereSql, @params, top);
        }

        public FeedbackCollection ListAll(FeedbackType feedbackType)
        {
            const string whereSql = @"WHERE [Feedback].[Type] = @FeedbackType ORDER BY CreatedAt DESC";
            var @params = new Dictionary<string, object> { { "@FeedbackType", (int)feedbackType } };
            return GetList(whereSql, @params);
        }
    }
}