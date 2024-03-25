using System;
using APV.Common;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable]
    public sealed class KeywordReferenceEntity : BaseEntity
    {
        #region Constructors

        public KeywordReferenceEntity()
        {
        }

        public KeywordReferenceEntity(IEntityCollection container)
            : base(container)
        {
        }

        public KeywordReferenceEntity(long id)
            : base(id)
        {
        }

        public KeywordReferenceEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long KeywordReferenceId { get; internal set; }

        [DbField]
        public long KeywordId { get; internal set; }

        [DbField]
        public long LanguageId { get; internal set; }

        [DbField(MaxLength = SystemConstants.MaxNameLength)]
        public string Word { get; internal set; }

        [DbField]
        public long Points { get; internal set; }

        #endregion

        #region Foreign Keys

        public KeywordEntity Keyword
        {
            get { return GetKeyValue<KeywordEntity>(() => KeywordId); }
            set { SetKeyValue(() => KeywordId, value); }
        }

        #endregion
    }
}