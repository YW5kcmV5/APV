using System;
using APV.Common;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable(CacheLimit = 0)]
    public sealed class WordReferenceEntity : BaseEntity
    {
        #region Constructors

        public WordReferenceEntity()
        {
        }

        public WordReferenceEntity(IEntityCollection container)
            : base(container)
        {
        }

        public WordReferenceEntity(long id)
            : base(id)
        {
        }

        public WordReferenceEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long WordReferenceId { get; internal set; }

        [DbField]
        public long WordId { get; internal set; }

        [DbField(MaxLength = Constants.LanguageCodeLength)]
        public string LanguageCode { get; set; }

        [DbField]
        public long ReferenceWordId { get; internal set; }

        [DbField(MaxLength = SystemConstants.MaxNameLength)]
        public string ReferenceName { get; set; }

        [DbField]
        public WordReferenceType ReferenceType { get; set; }

        [DbField]
        public PartOfSpeech PartOfSpeech { get; set; }

        #endregion

        #region Foreign Keys

        public WordEntity Word
        {
            get { return GetKeyValue<WordEntity>(() => WordId); }
            set { SetKeyValue(() => WordId, value); }
        }

        public LanguageEntity Language
        {
            get { return (Word != null) ? Word.Language : null; }
        }

        public WordEntity ReferenceWord
        {
            get { return GetKeyValue<WordEntity>(() => ReferenceWordId); }
            set { SetKeyValue(() => ReferenceWordId, value); }
        }

        #endregion
    }
}