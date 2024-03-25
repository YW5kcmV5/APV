using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class KeywordCollection : BaseEntityCollection<KeywordEntity>
    {
        #region Constructors

        public KeywordCollection()
        {
        }

        public KeywordCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion

        public long[] GetEntityIdentifiers()
        {
            int length = Count;
            var result = new long[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = this[i].EntityId;
            }
            return result;
        }
    }
}