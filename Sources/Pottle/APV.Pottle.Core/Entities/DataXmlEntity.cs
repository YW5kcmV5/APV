using System;
using APV.Common;
using APV.Common.Attributes.Db;
using APV.EntityFramework.Interfaces;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable(TableName = "Xml")]
    public class DataXmlEntity : DataFileEntity
    {
        #region Constructors

        public DataXmlEntity()
        {
        }

        public DataXmlEntity(IEntityCollection container)
            : base(container)
        {
        }

        public DataXmlEntity(long id)
            : base(id)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long XmlId { get; internal set; }

        [DbField]
        public Serializer.Type SerializerType { get; set; }

        [DbField]
        public string TypeName { get; set; }

        #endregion
    }
}