using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [Keyword]
    [DbTable(CacheLimit = 0)]
    public sealed class CountryEntity : BaseEntity, IName
    {
        #region Constructors

        public CountryEntity()
        {
        }

        public CountryEntity(IEntityCollection container)
            : base(container)
        {
        }

        public CountryEntity(long id)
            : base(id)
        {
        }

        public CountryEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long CountryId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName)]
        public string LegalName { get; set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName, Nullable = true)]
        public string AlternativeName { get; set; }

        [DbField]
        public string Code { get; set; }

        [DbField]
        public long LanguageId { get; internal set; }

        [DbField]
        public long? LogoImageId { get; internal set; }

        [DbField]
        public long? IconImageId { get; internal set; }

        #endregion

        #region Foreign Keys

        public LanguageEntity Language
        {
            get { return GetKeyValue<LanguageEntity>(() => LanguageId); }
            set { SetKeyValue(() => LanguageId, value); }
        }

        public DataImageEntity LogoImage
        {
            get { return GetKeyValue<DataImageEntity>(() => LogoImageId); }
            set { SetKeyValue(() => LogoImageId, value); }
        }

        public DataImageEntity IconImage
        {
            get { return GetKeyValue<DataImageEntity>(() => IconImageId); }
            set { SetKeyValue(() => IconImageId, value); }
        }

        #endregion
    }
}