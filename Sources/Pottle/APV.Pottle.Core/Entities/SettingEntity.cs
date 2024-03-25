using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable(CacheLimit = 0)]
    public sealed class SettingEntity : BaseEntity, IName
    {
        #region Constructors

        public SettingEntity()
        {
        }

        public SettingEntity(IEntityCollection container)
            : base(container)
        {
        }

        public SettingEntity(long id)
            : base(id)
        {
        }

        public SettingEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long SettingId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField]
        public SettingType SettingType { get; set; }

        [DbField]
        public string Value { get; set; }

        #endregion
    }
}