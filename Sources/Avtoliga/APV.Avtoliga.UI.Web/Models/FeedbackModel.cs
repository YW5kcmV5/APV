using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities.Collection;
using APV.Avtoliga.UI.Web.Controllers.API;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Models
{
    public sealed class FeedbackModel : BaseModel
    {
        public FeedbackInfo[] Feedbacks { get; private set; }

        public bool Archive { get; private set; }

        public FeedbackModel(bool archive)
        {
            Archive = archive;

            FeedbackCollection feedbacks = (archive)
                                               ? FeedbackManagement.Instance.ListArchiveFeedbacks()
                                               : FeedbackManagement.Instance.ListLatestFeedbacks();

            Feedbacks = feedbacks.Transform();
        }
    }
}