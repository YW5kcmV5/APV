using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [Keyword]
    [DbTable(CacheLimit = 0)]
    public sealed class ProductGroupEntity : BaseEntity, IName
    {
        #region Constructors

        public ProductGroupEntity()
        {
        }

        public ProductGroupEntity(IEntityCollection container)
            : base(container)
        {
        }

        public ProductGroupEntity(long id)
            : base(id)
        {
        }

        public ProductGroupEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long ProductGroupId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField(SpecialField = DbSpecialField.Deleted)]
        public bool Deleted { get; internal set; }

        #endregion
    }
}