using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class IPAddressCollection : BaseEntityCollection<IPAddressEntity>
    {
        #region Constructors

        public IPAddressCollection()
        {
        }

        public IPAddressCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}