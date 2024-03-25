using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class LocationCollection : BaseEntityCollection<LocationEntity>
    {
        #region Constructors

        public LocationCollection()
        {
        }

        public LocationCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}