using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class FeedbackCollection : BaseEntityCollection<FeedbackEntity>
    {
        #region Constructors

        public FeedbackCollection()
        {
        }

        public FeedbackCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}