using System;
using System.Xml;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable(Operations = DbOperations.Create | DbOperations.GetById)]
    public class TechnicalAuditEntity : BaseEntity
    {
        #region Constructors

        public TechnicalAuditEntity()
        {
        }

        public TechnicalAuditEntity(IEntityCollection container)
            : base(container)
        {
        }

        public TechnicalAuditEntity(long id)
            : base(id)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long TechnicalAuditId { get; internal set; }

        [DbField]
        public int ObjectType { get; internal set; }

        [DbField]
        public long ObjectId { get; internal set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnCreate)]
        public DateTime Timestamp { get; internal set; }

        [DbField]
        public DbOperation Operation { get; internal set; }

        [DbField(Generation = DbFieldAttribute.GenerationMode.OnCreate, SpecialField = DbSpecialField.Deleted)]
        public long UserId { get; internal set; }

        [DbField]
        public long InstanceId { get; internal set; }

        [DbField(Nullable = true)]
        public XmlDocument Modification { get; internal set; }

        #endregion
    }
}