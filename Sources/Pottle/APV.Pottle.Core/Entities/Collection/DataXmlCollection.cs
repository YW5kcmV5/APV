using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class DataXmlCollection : BaseEntityCollection<DataXmlEntity>
    {
        #region Constructors

        public DataXmlCollection()
        {
        }

        public DataXmlCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}