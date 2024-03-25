using System;
using APV.Common;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [DbTable(CacheLimit = 0)]
    public sealed class NewsEntity : BaseEntity
    {
        #region Constructors

        public NewsEntity()
        {
        }

        public NewsEntity(IEntityCollection container)
            : base(container)
        {
        }

        public NewsEntity(long id)
            : base(id)
        {
        }

        public NewsEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long NewsId { get; internal set; }

        [DbField(Nullable = false, MaxLength = SystemConstants.MaxNameLength)]
        public string Caption { get; set; }

        [DbField(Nullable = false, MaxLength = SystemConstants.MaxDescriptionLength)]
        public string Text { get; set; }

        [DbField]
        public long? LogoImageId { get; set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnCreate)]
        public DateTime CreatedAt { get; set; }

        [DbField]
        public int Likes { get; set; }

        #endregion

        #region Foreign Keys

        public ImageEntity LogoImage
        {
            get { return GetKeyValue<ImageEntity>(() => LogoImageId); }
            set { SetKeyValue(() => LogoImageId, value); }
        }

        #endregion
    }
}