using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class CountryCollection : BaseEntityCollection<CountryEntity>
    {
        #region Constructors

        public CountryCollection()
        {
        }

        public CountryCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}