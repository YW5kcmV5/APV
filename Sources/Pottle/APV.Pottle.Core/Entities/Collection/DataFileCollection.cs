using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class DataFileCollection : BaseEntityCollection<DataFileEntity>
    {
        #region Constructors

        public DataFileCollection()
        {
        }

        public DataFileCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}