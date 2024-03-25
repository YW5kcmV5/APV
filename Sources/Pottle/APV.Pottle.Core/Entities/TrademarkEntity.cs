using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [Keyword]
    [DbTable]
    public sealed class TrademarkEntity : BaseEntity, IName
    {
        #region Constructors

        public TrademarkEntity()
        {
        }

        public TrademarkEntity(IEntityCollection container)
            : base(container)
        {
        }

        public TrademarkEntity(long id)
            : base(id)
        {
        }

        public TrademarkEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long TrademarkId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField(SpecialField = DbSpecialField.Description)]
        public string Description { get; set; }

        [DbField]
        public long? UrlId { get; set; }

        [DbField]
        public long CountryId { get; set; }

        [DbField]
        public long? CompanyId { get; set; }

        [DbField]
        public long? LogoImageId { get; set; }

        [DbField]
        public long? IconImageId { get; set; }

        [DbField(SpecialField = DbSpecialField.Deleted)]
        public bool Deleted { get; internal set; }

        #endregion

        #region Foreign Keys

        public UrlEntity Url
        {
            get { return GetKeyValue<UrlEntity>(() => UrlId); }
            set { SetKeyValue(() => UrlId, value); }
        }

        public CountryEntity Country
        {
            get { return GetKeyValue<CountryEntity>(() => CountryId); }
            set { SetKeyValue(() => CountryId, value); }
        }

        public CompanyEntity Company
        {
            get { return GetKeyValue<CompanyEntity>(() => CompanyId); }
            set { SetKeyValue(() => CompanyId, value); }
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
