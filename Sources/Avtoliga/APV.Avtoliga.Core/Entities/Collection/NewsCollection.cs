using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class NewsCollection : BaseEntityCollection<NewsEntity>
    {
        #region Constructors

        public NewsCollection()
        {
        }

        public NewsCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}