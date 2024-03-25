using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace APV.EntityFramework
{
    /// <summary>
    /// Роль пользователя
    /// </summary>
    [Serializable]
    [DataContract]
    public enum UserRole
    {
        /// <summary>
        /// Покупатель
        /// </summary>
        [EnumMember]
        [Description("Покупатель")]
        Client,

        /// <summary>
        /// Администратор
        /// </summary>
        [EnumMember]
        [Description("Администратор")]
        Administrator,

        /// <summary>
        /// Аноним
        /// </summary>
        [EnumMember]
        [Description("Аноним")]
        Anonymous,
    }
}