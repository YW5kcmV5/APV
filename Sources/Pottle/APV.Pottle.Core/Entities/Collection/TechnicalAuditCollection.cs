using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class TechnicalAuditCollection : BaseEntityCollection<TechnicalAuditEntity>
    {
        #region Constructors

        public TechnicalAuditCollection()
        {
        }

        public TechnicalAuditCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}