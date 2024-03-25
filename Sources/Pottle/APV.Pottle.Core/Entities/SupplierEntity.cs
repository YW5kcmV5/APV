using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [Keyword]
    [DbTable]
    public sealed class SupplierEntity : BaseEntity, IName
    {
        #region Constructors

        public SupplierEntity()
        {
        }

        public SupplierEntity(IEntityCollection container)
            : base(container)
        {
        }

        public SupplierEntity(long id)
            : base(id)
        {
        }

        public SupplierEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long SupplierId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName, Nullable = true)]
        public string ShortName { get; set; }

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

        #region Collections

        public SupplierProductCollection Products
        {
            get { return GetCollection<SupplierProductCollection, SupplierProductEntity>(() => Products); }
        }

        #endregion
    }
}