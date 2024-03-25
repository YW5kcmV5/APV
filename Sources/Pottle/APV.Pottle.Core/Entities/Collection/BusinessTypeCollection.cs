using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class BusinessTypeCollection : BaseEntityCollection<BusinessTypeEntity>
    {
        #region Constructors

        public BusinessTypeCollection()
        {
        }

        public BusinessTypeCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}