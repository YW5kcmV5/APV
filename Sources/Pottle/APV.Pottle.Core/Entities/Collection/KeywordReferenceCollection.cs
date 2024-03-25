using System.Collections.Generic;
using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities.Collection
{
    public class KeywordReferenceCollection : BaseEntityCollection<KeywordReferenceEntity>
    {
        #region Constructors

        public KeywordReferenceCollection()
        {
        }

        public KeywordReferenceCollection(BaseEntity owner)
            : base(owner)
        {
        }

        public KeywordReferenceCollection(IEnumerable<KeywordReferenceEntity> collection)
            : base(collection)
        {
        }

        #endregion
    }
}