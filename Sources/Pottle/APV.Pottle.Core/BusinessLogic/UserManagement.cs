using System;
using APV.EntityFramework;
using APV.Common;
using APV.Common.Extensions;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class UserManagement : BaseManagement<UserEntity, UserCollection, UserDataLayerManager>
    {
        private UserManagement()
        {
        }

        [AnonymousAccess]
        public bool Login(string username, string password)
        {
            UserEntity user = (!string.IsNullOrEmpty(username)) && (!string.IsNullOrEmpty(password))
                                  ? DatabaseManager.Find(username, password.Hash256())
                                  : null;

            if (user != null)
            {
                Context.Login(user);
                return true;
            }

            return false;
        }

        [AnonymousAccess]
        public UserEntity GetAnonymous()
        {
            return DatabaseManager.GetAnonymous();
        }

        [AdminAccess]
        public override void Delete(UserEntity user)
        {
            base.Delete(user);
        }

        [AdminAccess]
        public override void Save(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            bool hasAccess = (IsAdmin) || ((!Authorized) && (user.IsNew)) || ((Authorized) && (User.UserId == user.UserId));
            if (!hasAccess)
                throw new UnauthorizedAccessException(string.Format("Only administrator can create or update users. The user \"{0}\" (UserId=\"{1}\") can not be saved.", user.Username, user.UserId));

            base.Save(user);
        }

        public void SetLocation(UserEntity user, string address)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");

            AddressEntity addressEntity = AddressManagement.Instance.Create(address);
            user.Address = addressEntity;
            user.Save();
        }

        public static readonly UserManagement Instance = (UserManagement)EntityFrameworkManager.GetManagement<UserEntity>();

        public static readonly UserEntity Anonymous = Instance.GetAnonymous();
    }
}