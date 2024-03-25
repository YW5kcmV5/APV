using System;
using APV.Avtoliga.Common;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [Keyword]
    [DbTable(CacheLimit = 0)]
    public sealed class ModelEntity : BaseEntity, IName
    {
        #region Constructors

        public ModelEntity()
        {
        }

        public ModelEntity(IEntityCollection container)
            : base(container)
        {
        }

        public ModelEntity(long id)
            : base(id)
        {
        }

        public ModelEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long ModelId { get; internal set; }

        [DbField]
        public long TrademarkId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField(Nullable = true, MaxLength = Constants.MaxNamePeriod)]
        public string Period { get; set; }

        [DbField(SpecialField = DbSpecialField.Deleted)]
        public bool Deleted { get; internal set; }

        #endregion

        #region Foreign Keys

        public TrademarkEntity Trademark
        {
            get { return GetKeyValue<TrademarkEntity>(() => TrademarkId); }
            set { SetKeyValue(() => TrademarkId, value); }
        }

        #endregion
    }
}