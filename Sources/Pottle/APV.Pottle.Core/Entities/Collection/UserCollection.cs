using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class UserCollection : BaseEntityCollection<UserEntity>
    {
        #region Constructors

        public UserCollection()
        {
        }

        public UserCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}