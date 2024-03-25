using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class ProducerCollection : BaseEntityCollection<ProducerEntity>
    {
        #region Constructors

        public ProducerCollection()
        {
        }

        public ProducerCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}