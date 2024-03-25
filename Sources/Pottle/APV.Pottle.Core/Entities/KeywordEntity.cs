using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable]
    public sealed class KeywordEntity : BaseEntity
    {
        #region Constructors

        public KeywordEntity()
        {
        }

        public KeywordEntity(IEntityCollection container)
            : base(container)
        {
        }

        public KeywordEntity(long id)
            : base(id)
        {
        }

        public KeywordEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long KeywordId { get; internal set; }

        [DbField]
        public long EntityTypeId { get; internal set; }

        [DbField]
        public long EntityId { get; internal set; }

        #endregion

        #region Foreign Keys

        #endregion

        #region Collections

        public KeywordReferenceCollection KeywordReferences
        {
            get { return GetCollection<KeywordReferenceCollection, KeywordReferenceEntity>(() => KeywordReferences); }
        }

        #endregion
    }
}