using System;
using System.Runtime.Serialization;
using System.Xml;
using APV.Common.Interfaces;

namespace APV.Common
{
    [DataContract]
    public abstract class XmlObject : IXmlObject, ICloneable
    {
        #region Private/Protected

        private XmlObjectState _xmlState;
        private XmlObjectType _xmlObjectType;

        #region Methods

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            _xmlState = XmlObjectState.Serializing;
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            _xmlState = XmlObjectState.Initialized;
        }

        [OnDeserializing()]
        private void OnDeserializing(StreamingContext context)
        {
            _xmlState = XmlObjectState.Deserializing;
            _xmlObjectType = XmlObjectType.Deserialized;
        }

        [OnDeserialized]
        private void OnDeserialize(StreamingContext context)
        {
            _xmlState = XmlObjectState.Initialized;
        }

        #endregion

        #region Properties

        [IgnoreDataMember]
        protected XmlObjectState XmlState
        {
            get { return _xmlState; }
        }

        [IgnoreDataMember]
        protected XmlObjectType XmlType
        {
            get { return _xmlObjectType; }
        }

        #endregion

        #endregion

        #region ICloneable

        public object Clone()
        {
            return Serializer.Deserialize(GetType(), OuterXml, Serializer.Type.DataContractSerializer);
        }

        #endregion

        #region IXmlObject

        public XmlDocument Serialize()
        {
            return Serializer.Serialize(this, Serializer.Type.DataContractSerializer);
        }

        [IgnoreDataMember]
        public string OuterXml
        {
            get { return Serialize().OuterXml; }
        }

        [IgnoreDataMember]
        public string SqlXml
        {
            get
            {
                XmlDocument doc = Serializer.Serialize(this, SystemConstants.SqlXmlEncoding, Serializer.Type.DataContractSerializer);
                return doc.OuterXml;
            }
        }

        #endregion

        #region Static

        public static T Deserialize<T>(string xml) where T : XmlObject
        {
            return Serializer.Deserialize<T>(xml, Serializer.Type.DataContractSerializer);
        }

        #endregion
    }
}