using System;
using APV.Avtoliga.Core.Entities.Collection;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [DbTable(CacheLimit = 0)]
    public sealed class ArticleGroupEntity : BaseEntity, IName
    {
        #region Constructors

        public ArticleGroupEntity()
        {
        }

        public ArticleGroupEntity(IEntityCollection container)
            : base(container)
        {
        }

        public ArticleGroupEntity(long id)
            : base(id)
        {
        }

        public ArticleGroupEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long ArticleGroupId { get; internal set; }

        [DbField]
        public long? ParentArticleGroupId { get; set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        #endregion

        #region Foreign Keys

        public ArticleGroupEntity Parent
        {
            get { return GetKeyValue<ArticleGroupEntity>(() => ParentArticleGroupId); }
            set { SetKeyValue(() => ParentArticleGroupId, value); }
        }

        #endregion

        #region Collections

        public ArticleCollection Articles
        {
            get { return GetCollection<ArticleCollection, ArticleEntity>(() => Articles); }
        }

        public ArticleGroupCollection Children
        {
            get { return GetCollection<ArticleGroupCollection, ArticleGroupEntity>(() => Children); }
        }

        #endregion

        public bool Top
        {
            get { return (ParentArticleGroupId == null); }
        }
    }
}