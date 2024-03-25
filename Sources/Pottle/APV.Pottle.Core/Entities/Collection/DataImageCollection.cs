using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class DataImageCollection : BaseEntityCollection<DataImageEntity>
    {
        #region Constructors

        public DataImageCollection()
        {
        }

        public DataImageCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}