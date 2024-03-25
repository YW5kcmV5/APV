using System;
using System.Security.Principal;
using APV.Common.Attributes.Db;
using APV.EntityFramework;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [DbTable]
    public sealed class UserEntity : BaseEntity, IName, IUser
    {
        #region Constructors

        public UserEntity()
        {
        }

        public UserEntity(IEntityCollection container)
            : base(container)
        {
        }

        public UserEntity(long id)
            : base(id)
        {
        }

        public UserEntity(byte[] dump)
            : base(dump)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long UserId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Name)]
        public string Username { get; set; }

        [DbField(Nullable = false, MaxLength = 32)]
        public byte[] PasswordHash { get; set; }

        #endregion

        #region IName

        string IName.Name { get { return Username; } }

        #endregion

        #region IUser

        string IIdentity.Name { get { return Username; } }

        string IIdentity.AuthenticationType { get { return string.Empty; } }

        bool IIdentity.IsAuthenticated { get { return true; } }

        UserRole IUser.UserRole { get { return UserRole.Administrator; } }

        #endregion
    }
}