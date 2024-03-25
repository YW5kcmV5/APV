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
    public sealed class LanguageEntity : BaseEntity, IName
    {
        #region Constructors

        public LanguageEntity()
        {
        }

        public LanguageEntity(IEntityCollection container)
            : base(container)
        {
        }

        public LanguageEntity(long id)
            : base(id)
        {
        }

        public LanguageEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long LanguageId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; internal set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName)]
        public string EnglishName { get; internal set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName)]
        public string NativeName { get; internal set; }

        [DbField(MaxLength = Constants.LanguageCodeLength)]
        public string Code { get; internal set; }

        [DbField(Nullable = true, MaxLength = SystemConstants.MaxNameLength)]
        public string WordChars { get; internal set; }

        #endregion
    }
}