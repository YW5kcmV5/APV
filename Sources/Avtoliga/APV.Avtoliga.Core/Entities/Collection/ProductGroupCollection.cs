using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class ProductGroupCollection : BaseEntityCollection<ProductGroupEntity>
    {
        #region Constructors

        public ProductGroupCollection()
        {
        }

        public ProductGroupCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}