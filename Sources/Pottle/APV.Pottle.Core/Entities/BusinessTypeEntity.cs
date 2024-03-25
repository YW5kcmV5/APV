using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable(CacheLimit = 0)]
    //[DebuggerDisplay("{\"'\" + Name + \"' \" + \"(\" + GetType().Name + \")\"}")]
    public sealed class BusinessTypeEntity : BaseEntity, IName
    {
        #region Constructors

        public BusinessTypeEntity()
        {
        }

        public BusinessTypeEntity(IEntityCollection container)
            : base(container)
        {
        }

        public BusinessTypeEntity(long id)
            : base(id)
        {
        }

        public BusinessTypeEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long BusinessTypeId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField(SpecialField = DbSpecialField.Description)]
        public string Description { get; internal set; }

        [DbField]
        public long CountryId { get; internal set; }

        #endregion

        #region Foreign Keys

        public CountryEntity Country
        {
            get { return GetKeyValue<CountryEntity>(() => CountryId); }
            set { SetKeyValue(() => CountryId, value); }
        }

        #endregion
    }
}