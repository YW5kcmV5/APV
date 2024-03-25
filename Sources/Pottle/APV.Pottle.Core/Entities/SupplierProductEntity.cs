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
    public sealed class SupplierProductEntity : BaseEntity
    {
        #region Constructors

        public SupplierProductEntity()
        {
        }

        public SupplierProductEntity(IEntityCollection container)
            : base(container)
        {
        }

        public SupplierProductEntity(long id)
            : base(id)
        {
        }

        public SupplierProductEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long SupplierProductId { get; internal set; }

        [DbField]
        public long SupplierId { get; set; }

        [DbField]
        public long ProductId { get; set; }

        [DbField]
        public long? UrlId { get; set; }

        [DbField]
        public float? Price { get; set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName, Nullable = true)]
        public string Article { get; set; }

        [DbField(SpecialField = DbSpecialField.AlternativeName, Nullable = true)]
        public string ISBN { get; set; }

        [DbField(SpecialField = DbSpecialField.Deleted)]
        public bool Deleted { get; internal set; }

        #endregion

        #region Foreign Keys

        public SupplierEntity Supplier
        {
            get { return GetKeyValue<SupplierEntity>(() => SupplierId); }
            set { SetKeyValue(() => SupplierId, value); }
        }

        public ProductEntity Product
        {
            get { return GetKeyValue<ProductEntity>(() => ProductId); }
            set { SetKeyValue(() => ProductId, value); }
        }

        public UrlEntity Url
        {
            get { return GetKeyValue<UrlEntity>(() => UrlId); }
            set { SetKeyValue(() => UrlId, value); }
        }

        #endregion
    }
}