using System;
using System.Security.Principal;
using APV.EntityFramework.Interfaces;
using APV.Common;

namespace APV.TransControl.Core.Application
{
    public sealed class AnonymousUserContext : IContext
    {
        private IUser _user = new AnonymousUser();

        public IUser User
        {
            get { return _user; }
        }

        public bool Authorized
        {
            get { return (_user != null) && (_user.IsAuthenticated); }
        }

        public long UserId
        {
            get { return _user.UserId; }
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
            if (user == null)
                throw new ArgumentNullException("user");

            _user = user;
        }

        public void Logout()
        {
            _user = new AnonymousUser();
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