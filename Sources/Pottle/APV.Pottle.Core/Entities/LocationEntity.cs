using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable]
    public sealed class LocationEntity : BaseEntity, IName
    {
        #region Constructors

        public LocationEntity()
        {
        }

        public LocationEntity(IEntityCollection container)
            : base(container)
        {
        }

        public LocationEntity(long id)
            : base(id)
        {
        }

        public LocationEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long LocationId { get; internal set; }

        [DbField]
        public float LAT { get; set; }

        [DbField]
        public float LON { get; set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Address { get; set; }

        #endregion

        #region IName

        public string Name
        {
            get { return Address; }
        }

        #endregion
    }
}