using System;
using System.Security.Principal;
using APV.EntityFramework;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Common.Attributes.Db;
using APV.Common.Extensions;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable]
    public sealed class UserEntity : BaseEntity, IUser, IName
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

        [DbField(SpecialField = DbSpecialField.AlternativeName)]
        public string Email { get; set; }

        [DbField(FieldName = "Password")]
        public byte[] PasswordHash { get; internal set; }

        [DbField]
        public UserRole UserRole { get; internal set; }

        [DbField]
        public long CountryId { get; internal set; }

        [DbField]
        public long LanguageId { get; internal set; }

        [DbField]
        public long? AddressId { get; internal set; }

        [DbField(SpecialField = DbSpecialField.Deleted)]
        public bool Deleted { get; internal set; }

        #endregion

        #region IName

        public string Name
        {
            get { return Username; }
        }

        #endregion

        #region Foreign Keys

        public AddressEntity Address
        {
            get { return GetKeyValue<AddressEntity>(() => AddressId); }
            set { SetKeyValue(() => AddressId, value); }
        }

        public LanguageEntity Language
        {
            get { return GetKeyValue<LanguageEntity>(() => LanguageId); }
            set { SetKeyValue(() => LanguageId, value); }
        }

        public CountryEntity Country
        {
            get { return GetKeyValue<CountryEntity>(() => CountryId); }
            set { SetKeyValue(() => CountryId, value); }
        }

        #endregion

        #region IIdentity

        string IIdentity.Name
        {
            get { return Username; }
        }

        bool IIdentity.IsAuthenticated
        {
            get
            {
                IContextManager manager = EntityFrameworkManager.GetContextManager();
                IContext context = manager.GetContext();
                return ((context.Authorized) && (context.UserId == UserId));
            }
        }

        string IIdentity.AuthenticationType
        {
            get { throw new NotSupportedException(); }
        }

        #endregion

        public string Password
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                PasswordHash = value.Hash256();
            }
        }
    }
}