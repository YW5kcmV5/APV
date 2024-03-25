using System;
using APV.Avtoliga.Core.Entities.Collection;
using APV.Common;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [DbTable]
    public sealed class UrlEntity : BaseEntity, IName
    {
        #region Constructors

        public UrlEntity()
        {
        }

        public UrlEntity(IEntityCollection container)
            : base(container)
        {
        }

        public UrlEntity(long id)
            : base(id)
        {
        }

        public UrlEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long UrlId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name, MaxLength = SystemConstants.MaxUrlLength)]
        public string Url { get; internal set; }

        [DbField]
        public byte[] HashCode { get; internal set; }

        [DbField]
        public bool? Alive { get; internal set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnCreate)]
        public DateTime VerifiedAt { get; internal set; }

        [DbField]
        public long? HostUrlId { get; internal set; }

        #endregion

        #region IName

        public string Name
        {
            get { return Url; }
        }

        #endregion

        #region Foreign Keys

        public UrlEntity HostUrl
        {
            get { return GetKeyValue<UrlEntity>(() => HostUrlId); }
            set { SetKeyValue(() => HostUrlId, value); }
        }

        #endregion

        #region Collections

        public UrlCollection Children
        {
            get { return GetCollection<UrlCollection, UrlEntity>(() => Children); }
        }

        #endregion
    }
}