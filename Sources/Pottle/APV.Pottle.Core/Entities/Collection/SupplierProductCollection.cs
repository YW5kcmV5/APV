using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class SupplierProductCollection : BaseEntityCollection<SupplierProductEntity>
    {
        #region Constructors

        public SupplierProductCollection()
        {
        }

        public SupplierProductCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}