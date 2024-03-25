using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable]
    public class AddressEntity : BaseEntity, IName
    {
        #region Constructors

        public AddressEntity()
        {
        }

        public AddressEntity(IEntityCollection container)
            : base(container)
        {
        }

        public AddressEntity(long id)
            : base(id)
        {
        }

        public AddressEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long AddressId { get; internal set; }

        [DbField]
        public long LocationId { get; internal set; }

        /// <summary>
        /// Номер квартиры, офиса или строения ("null", если уточнения не требуется)
        /// </summary>
        [DbField(MaxLength = Constants.MaxAddressPositionLength, Nullable = true)]
        public string Position { get; set; }

        /// <summary>
        /// Уточнение типа позиции - номер квартиры, офиса или строения
        /// </summary>
        [DbField]
        public AddressPositionType PositionType { get; set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Address { get; set; }

        [DbField(SpecialField = DbSpecialField.Description)]
        public string Description { get; set; }

        /// <summary>
        /// Этаж
        /// </summary>
        [DbField]
        public int? Floor { get; set; }

        /// <summary>
        /// Подъезд, парадная
        /// </summary>
        [DbField]
        public int? Porch { get; set; }

        #endregion

        #region IName

        public string Name
        {
            get { return Address; }
        }

        #endregion

        #region Foreign Keys

        public LocationEntity Location
        {
            get { return GetKeyValue<LocationEntity>(() => LocationId); }
            set { SetKeyValue(() => LocationId, value); }
        }

        #endregion
    }
}