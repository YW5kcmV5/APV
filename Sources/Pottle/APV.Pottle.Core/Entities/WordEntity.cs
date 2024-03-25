using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable(CacheLimit = 0)]
    public sealed class WordEntity : BaseEntity, IName
    {
        #region Constructors

        public WordEntity()
        {
        }

        public WordEntity(IEntityCollection container)
            : base(container)
        {
        }

        public WordEntity(long id)
            : base(id)
        {
        }

        public WordEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long WordId { get; internal set; }

        [DbField]
        public long LanguageId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField]
        public bool Unknown { get; set; }

        [DbField]
        public PartsOfSpeech PartsOfSpeech { get; set; }

        [DbField]
        public bool Lemma { get; set; }

        [DbField]
        public long SearchCount { get; set; }

        #endregion

        #region Foreign Keys

        public LanguageEntity Language
        {
            get { return GetKeyValue<LanguageEntity>(() => LanguageId); }
            set { SetKeyValue(() => LanguageId, value); }
        }

        #endregion
    }
}