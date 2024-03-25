using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class ArticleGroupCollection : BaseEntityCollection<ArticleGroupEntity>
    {
        #region Constructors

        public ArticleGroupCollection()
        {
        }

        public ArticleGroupCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}