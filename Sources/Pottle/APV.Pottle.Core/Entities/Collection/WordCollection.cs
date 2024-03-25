using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class WordCollection : BaseEntityCollection<WordEntity>
    {
        #region Constructors

        public WordCollection()
        {
        }

        public WordCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}