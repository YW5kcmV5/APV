using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class ProductCollection : BaseEntityCollection<ProductEntity>
    {
        #region Constructors

        public ProductCollection()
        {
        }

        public ProductCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}