using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable(CacheLimit = 0)]
    public sealed class IPAddressEntity : BaseEntity, IName
    {
        #region Constructors

        public IPAddressEntity()
        {
        }

        public IPAddressEntity(IEntityCollection container)
            : base(container)
        {
        }

        public IPAddressEntity(long id)
            : base(id)
        {
        }

        public IPAddressEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long IPAddressId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Value { get; set; }

        [DbField]
        public long LocationId { get; internal set; }

        [DbField]
        public long CountryId { get; internal set; }

        [DbField]
        public bool Restricted { get; internal set; }

        #endregion

        #region IName

        public string Name
        {
            get { return Value; }
        }

        #endregion

        #region Foreign Keys

        public LocationEntity Location
        {
            get { return GetKeyValue<LocationEntity>(() => LocationId); }
            set { SetKeyValue(() => LocationId, value); }
        }

        public CountryEntity Country
        {
            get { return GetKeyValue<CountryEntity>(() => CountryId); }
            set { SetKeyValue(() => CountryId, value); }
        }

        #endregion
    }
}