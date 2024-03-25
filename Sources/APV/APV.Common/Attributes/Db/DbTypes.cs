using System;
using System.Runtime.Serialization;

namespace APV.Common.Attributes.Db
{
    [Serializable]
    [DataContract]
    public enum DbOperation
    {
        [EnumMember]
        GetById = 1,

        [EnumMember]
        GetByName = 2,

        [EnumMember]
        GetAll = 4,

        [EnumMember]
        Create = 8,

        [EnumMember]
        Update = 16,

        [EnumMember]
        Delete = 32,

        [EnumMember]
        MarkAsDeleted = 64,
    }

    [Flags]
    [Serializable]
    [DataContract]
    public enum DbOperations
    {
        [EnumMember]
        GetById = DbOperation.GetById,

        [EnumMember]
        GetByName = DbOperation.GetByName,

        [EnumMember]
        GetAll = DbOperation.GetAll,

        [EnumMember]
        Create = DbOperation.Create,

        [EnumMember]
        Update = DbOperation.Update,

        [EnumMember]
        Delete = DbOperation.Delete,

        [EnumMember]
        MarkAsDeleted = DbOperation.MarkAsDeleted,

        /// <summary>
        /// GetById | Create | Update | Delete
        /// </summary>
        BaseOperations = GetById | Create | Update | Delete,
    }

    [Serializable]
    [DataContract]
    public enum DbSpecialField
    {
        None,

        /// <summary>
        /// Nullable = False, MaxLength = 255, Unique key,
        /// Only one field in entity
        /// </summary>
        Name,

        /// <summary>
        /// MaxLength = 255
        /// </summary>
        AlternativeName,

        /// <summary>
        /// Nullable = True, MaxLength = 1000,
        /// Only one field in entity
        /// </summary>
        Description,

        UserId,

        InstanceId,

        Deleted
    }
}