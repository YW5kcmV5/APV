using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class AddressCollection : BaseEntityCollection<AddressEntity>
    {
        #region Constructors

        public AddressCollection()
        {
        }

        public AddressCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}