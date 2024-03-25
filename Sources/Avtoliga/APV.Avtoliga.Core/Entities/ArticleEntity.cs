using System;
using APV.Common;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [DbTable(CacheLimit = 0)]
    public sealed class ArticleEntity : BaseEntity
    {
        #region Constructors

        public ArticleEntity()
        {
        }

        public ArticleEntity(IEntityCollection container)
            : base(container)
        {
        }

        public ArticleEntity(long id)
            : base(id)
        {
        }

        public ArticleEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long ArticleId { get; internal set; }

        [DbField]
        public long? ArticleGroupId { get; internal set; }

        [DbField(Nullable = false, MaxLength = SystemConstants.MaxNameLength)]
        public string Name { get; set; }

        [DbField(SpecialField = DbSpecialField.Description)]
        public string Description { get; set; }

        [DbField(Nullable = false)]
        public string Html { get; set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnCreate)]
        public DateTime CreatedAt { get; set; }

        #endregion

        #region Foreign Keys

        public ArticleGroupEntity ArticleGroup
        {
            get { return GetKeyValue<ArticleGroupEntity>(() => ArticleGroupId); }
            set { SetKeyValue(() => ArticleGroupId, value); }
        }

        #endregion

        public bool Alone
        {
            get { return (ArticleGroupId == null); }
        }
    }
}