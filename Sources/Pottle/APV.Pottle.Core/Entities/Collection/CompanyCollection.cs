using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class CompanyCollection : BaseEntityCollection<CompanyEntity>
    {
        #region Constructors

        public CompanyCollection()
        {
        }

        public CompanyCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}