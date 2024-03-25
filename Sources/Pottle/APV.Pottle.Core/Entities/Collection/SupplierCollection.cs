using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class SupplierCollection : BaseEntityCollection<SupplierEntity>
    {
        #region Constructors

        public SupplierCollection()
        {
        }

        public SupplierCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}