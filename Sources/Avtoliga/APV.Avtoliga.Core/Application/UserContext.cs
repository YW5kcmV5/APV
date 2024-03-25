using System;
using System.Security.Principal;
using APV.Avtoliga.Core.Entities;
using APV.EntityFramework.Interfaces;
using APV.Common;

namespace APV.Avtoliga.Core.Application
{
    public sealed class UserContext : IContext
    {
        private IUser _user;

        public IUser User
        {
            get
            {
                if (_user == null)
                {
                    long userId = SessionManager.GetUserId();
                    _user = (userId != SystemConstants.UnknownId) ? (IUser) new UserEntity(userId) : new Anonymous();
                }
                return _user;
            }
        }

        public bool Authorized
        {
            get { return (_user != null) && (_user.IsAuthenticated); }
        }

        public long UserId
        {
            get { return (_user != null) ? _user.UserId : SystemConstants.UnknownId; }
        }

        public string CountryCode
        {
            get { return SystemConstants.DefaultCountryCode; }
        }

        public string LanguageCode
        {
            get { return SystemConstants.DefaultLanguageCode; }
        }

        public void Login(IUser user)
        {
            _user = user;
        }

        public void Login(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (user.UserId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("user", string.Format("Specified entity is new (is not stored in database)."));

            _user = user;
            SessionManager.SetUserId(user.UserId);
        }

        public void Logout()
        {
            _user = null;
            SessionManager.SetUserId(SystemConstants.UnknownId);
        }

        #region IPrincipal

        bool IPrincipal.IsInRole(string role)
        {
            return (User.UserRole.ToString() == role);
        }

        IIdentity IPrincipal.Identity
        {
            get { return User; }
        }

        #endregion
    }
}