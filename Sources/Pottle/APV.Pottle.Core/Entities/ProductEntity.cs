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
    public sealed class ProductEntity : BaseEntity
    {
        #region Constructors

        public ProductEntity()
        {
        }

        public ProductEntity(IEntityCollection container)
            : base(container)
        {
        }

        public ProductEntity(long id)
            : base(id)
        {
        }

        public ProductEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long ProductId { get; internal set; }

        [DbField]
        public long? TrademarkId { get; set; }

        [DbField]
        public long? ProducerCompanyId { get; set; }

        [DbField]
        public long? ProducerCountryId { get; set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName)]
        public string ProductName { get; set; }

        [DbField(SpecialField = DbSpecialField.Description)]
        public string Description { get; set; }

        [DbField]
        public long? LogoImageId { get; internal set; }

        [DbField]
        public long? IconImageId { get; internal set; }

        [DbField]
        public long TagsXmlId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Deleted)]
        public bool Deleted { get; internal set; }

        #endregion

        #region Foreign Keys

        public TrademarkEntity Trademark
        {
            get { return GetKeyValue<TrademarkEntity>(() => TrademarkId); }
            set { SetKeyValue(() => TrademarkId, value); }
        }

        public CompanyEntity ProducerCompany
        {
            get { return GetKeyValue<CompanyEntity>(() => ProducerCompanyId); }
            set { SetKeyValue(() => ProducerCompanyId, value); }
        }

        public CountryEntity ProducerCountry
        {
            get { return GetKeyValue<CountryEntity>(() => ProducerCountryId); }
            set { SetKeyValue(() => ProducerCountryId, value); }
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

        public DataXmlEntity TagsXml
        {
            get { return GetKeyValue<DataXmlEntity>(() => TagsXmlId); }
            set { SetKeyValue(() => TagsXmlId, value); }
        }

        #endregion
    }
}