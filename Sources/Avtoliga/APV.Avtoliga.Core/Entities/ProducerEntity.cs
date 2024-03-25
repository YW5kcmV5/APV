using System;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Common.Attributes;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [Keyword]
    [DbTable(CacheLimit = 0)]
    public sealed class ProducerEntity : BaseEntity, IName
    {
        #region Constructors

        public ProducerEntity()
        {
        }

        public ProducerEntity(IEntityCollection container)
            : base(container)
        {
        }

        public ProducerEntity(long id)
            : base(id)
        {
        }

        public ProducerEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long ProducerId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Name { get; set; }

        [DbField]
        public long? UrlId { get; internal set; }

        [DbField]
        public long? LogoImageId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Description)]
        public string Title { get; set; }

        [DbField(Nullable = true, MaxLength = SystemConstants.MaxStringLength)]
        public string About { get; set; }

        [DbField(SpecialField = DbSpecialField.Deleted)]
        public bool Deleted { get; internal set; }

        #endregion

        #region Foreign Keys

        public UrlEntity Url
        {
            get { return GetKeyValue<UrlEntity>(() => UrlId); }
            set { SetKeyValue(() => UrlId, value); }
        }

        public ImageEntity Logo
        {
            get { return GetKeyValue<ImageEntity>(() => LogoImageId); }
            set { SetKeyValue(() => LogoImageId, value); }
        }

        #endregion
    }
}