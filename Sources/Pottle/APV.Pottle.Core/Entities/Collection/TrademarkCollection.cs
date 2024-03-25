using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class TrademarkCollection : BaseEntityCollection<TrademarkEntity>
    {
        #region Constructors

        public TrademarkCollection()
        {
        }

        public TrademarkCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}