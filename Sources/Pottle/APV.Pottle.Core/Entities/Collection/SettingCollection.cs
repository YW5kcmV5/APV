using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class SettingCollection : BaseEntityCollection<SettingEntity>
    {
        #region Constructors

        public SettingCollection()
        {
        }

        public SettingCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}