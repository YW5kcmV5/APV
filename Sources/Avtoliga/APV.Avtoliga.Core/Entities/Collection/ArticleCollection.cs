using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class ArticleCollection : BaseEntityCollection<ArticleEntity>
    {
        #region Constructors

        public ArticleCollection()
        {
        }

        public ArticleCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}