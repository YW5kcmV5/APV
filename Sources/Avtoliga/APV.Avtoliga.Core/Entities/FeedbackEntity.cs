using System;
using APV.Avtoliga.Common;
using APV.Common;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [DbTable(CacheLimit = 0)]
    public sealed class FeedbackEntity : BaseEntity
    {
        #region Constructors

        public FeedbackEntity()
        {
        }

        public FeedbackEntity(IEntityCollection container)
            : base(container)
        {
        }

        public FeedbackEntity(long id)
            : base(id)
        {
        }

        public FeedbackEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long FeedbackId { get; internal set; }

        [DbField(Nullable = false, MaxLength = SystemConstants.MaxNameLength)]
        public string Name { get; set; }

        [DbField(Nullable = false, MaxLength = SystemConstants.MaxNameLength)]
        public string Email { get; set; }

        [DbField(Nullable = true, MaxLength = SystemConstants.MaxNameLength)]
        public string Phone { get; set; }

        [DbField(Nullable = false, MaxLength = SystemConstants.MaxDescriptionLength)]
        public string Text { get; set; }

        [DbField]
        public FeedbackType Type { get; set; }

        [DbField]
        public int? Likes { get; set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnCreate)]
        public DateTime CreatedAt { get; set; }

        #endregion
    }
}
