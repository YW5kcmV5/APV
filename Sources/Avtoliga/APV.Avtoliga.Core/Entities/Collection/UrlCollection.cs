using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class UrlCollection : BaseEntityCollection<UrlEntity>
    {
        #region Constructors

        public UrlCollection()
        {
        }

        public UrlCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}