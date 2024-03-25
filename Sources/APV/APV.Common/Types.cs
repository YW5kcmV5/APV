using System;
using System.Runtime.Serialization;

namespace APV.Common
{
    [Serializable]
    [DataContract]
    public enum XmlObjectState
    {
        [EnumMember]
        Initialized,

        [EnumMember]
        Serializing,

        [EnumMember]
        Deserializing
    }

    [Serializable]
    [DataContract]
    public enum XmlObjectType
    {
        [EnumMember]
        Initialized,

        /// <summary>
        /// Object instance is deserialized from xml
        /// </summary>
        [EnumMember]
        Deserialized,
    }
}