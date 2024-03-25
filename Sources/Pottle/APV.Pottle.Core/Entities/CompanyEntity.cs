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
    public sealed class CompanyEntity : BaseEntity, IName
    {
        #region Constructors

        public CompanyEntity()
        {
        }

        public CompanyEntity(IEntityCollection container)
            : base(container)
        {
        }

        public CompanyEntity(long id)
            : base(id)
        {
        }

        public CompanyEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long CompanyId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName)]
        public string LegalName { get; set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName, Nullable = true)]
        public string CompanyName { get; set; }

        [DbField(SpecialField = DbSpecialField.Description)]
        public string Description { get; set; }

        [DbField]
        public long? LogoImageId { get; internal set; }

        [DbField]
        public long? IconImageId { get; internal set; }

        [DbField]
        public long CountryId { get; internal set; }

        [DbField]
        public long? BusinessTypeId { get; internal set; }

        [DbField]
        public long? RequisitesXmlId { get; internal set; }

        [DbField]
        public long? AddressId { get; set; }

        [DbField]
        public long? LegalAddressId { get; set; }

        [DbField(SpecialField = DbSpecialField.Deleted)]
        public bool Deleted { get; internal set; }

        #endregion

        #region Foreign Keys

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

        public CountryEntity Country
        {
            get { return GetKeyValue<CountryEntity>(() => CountryId); }
            set { SetKeyValue(() => CountryId, value); }
        }

        public BusinessTypeEntity BusinessType
        {
            get { return GetKeyValue<BusinessTypeEntity>(() => BusinessTypeId); }
            set { SetKeyValue(() => BusinessTypeId, value); }
        }

        public DataXmlEntity RequisitesXml
        {
            get { return GetKeyValue<DataXmlEntity>(() => RequisitesXmlId); }
            set { SetKeyValue(() => RequisitesXmlId, value); }
        }

        public AddressEntity Address
        {
            get { return GetKeyValue<AddressEntity>(() => AddressId); }
            set { SetKeyValue(() => AddressId, value); }
        }

        public AddressEntity LegalAddress
        {
            get { return GetKeyValue<AddressEntity>(() => LegalAddressId); }
            set { SetKeyValue(() => LegalAddressId, value); }
        }

        #endregion
    }
}