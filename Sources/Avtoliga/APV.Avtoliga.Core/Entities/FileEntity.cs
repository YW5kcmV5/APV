using System;
using APV.Avtoliga.Common;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [DbTable(TableName = "File", CacheLimit = 1000)]
    public class FileEntity : BaseEntity
    {
        #region Constructors

        public FileEntity()
        {
        }

        public FileEntity(IEntityCollection container)
            : base(container)
        {
        }

        public FileEntity(long id)
            : base(id)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public long FileId { get; internal set; }

        [DbField(Nullable = true, SpecialField = DbSpecialField.Name)]
        public string Name { get; internal set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnUpdate)]
        public Guid Tag { get; internal set; }

        [DbField]
        public DataStorage DataStorage { get; set; }

        [DbField(Nullable = true)]
        public byte[] Data { get; set; }

        [DbField(Nullable = true)]
        public string Path { get; set; }

        #endregion
    }
}