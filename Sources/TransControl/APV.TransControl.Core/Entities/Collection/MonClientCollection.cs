using APV.EntityFramework.DataLayer;

namespace APV.TransControl.Core.Entities.Collection
{
    public class MonClientCollection : BaseEntityCollection<MonClientEntity>
    {
        #region Constructors

        public MonClientCollection()
        {
        }

        public MonClientCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}