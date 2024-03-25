using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable(TableName = "File", CacheLimit = 1000)]
    public class DataFileEntity : BaseEntity
    {
        #region Constructors

        public DataFileEntity()
        {
        }

        public DataFileEntity(IEntityCollection container)
            : base(container)
        {
        }

        public DataFileEntity(long id)
            : base(id)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long FileId { get; internal set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnUpdate)]
        public Guid Tag { get; internal set; }

        [DbField]
        public DataStorage DataStorage { get; set; }

        [DbField(Nullable = true)]
        public byte[] Data { get; set; }

        [DbField(Nullable = true)]
        public string Path { get; set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnCreate, SpecialField = DbSpecialField.UserId)]
        public long CreatedByUserId { get; internal set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnCreate)]
        public DateTime CreatedAt { get; internal set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnUpdate, SpecialField = DbSpecialField.UserId)]
        public long ModifiedByUserId { get; internal set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnUpdate)]
        public DateTime ModifiedAt { get; internal set; }

        #endregion

        #region Foreign Keys

        public UserEntity CreatedBy
        {
            get { return GetKeyValue<UserEntity>(() => CreatedByUserId); }
        }

        public UserEntity ModifiedBy
        {
            get { return GetKeyValue<UserEntity>(() => CreatedByUserId); }
        }

        #endregion
    }
}