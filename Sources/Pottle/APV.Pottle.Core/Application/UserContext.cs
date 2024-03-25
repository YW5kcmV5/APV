using System;
using System.Net;
using System.Security.Principal;
using APV.EntityFramework;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using WebUtility = APV.Common.WebUtility;

namespace APV.Pottle.Core.Application
{
    public sealed class UserContext : IContext
    {
        private UserEntity _user;
        private CountryEntity _country;
        private LanguageEntity _language;

        public UserEntity User
        {
            get
            {
                if (_user == null)
                {
                    long userId = SessionManager.GetUserId();
                    _user = (userId != SystemConstants.UnknownId) ? new UserEntity(userId) : UserManagement.Anonymous;
                }
                return _user;
            }
        }

        public CountryEntity Country
        {
            get
            {
                if (_country == null)
                {
                    string countryCode = SessionManager.GetString(SessionManager.CountryCodeKey);
                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        _country = CountryManagement.Instance.FindByCode(countryCode);
                    }
                    if (_country == null)
                    {
                        countryCode = CookiesManager.GetString(SessionManager.CountryCodeKey);
                        if (!string.IsNullOrEmpty(countryCode))
                        {
                            _country = CountryManagement.Instance.FindByCode(countryCode);
                        }
                        if (_country == null)
                        {
                            IPAddress ipAddress = WebUtility.GetRemoteAddress();
                            if (ipAddress != null)
                            {
                                IPAddressEntity entity = IPAddressManagement.Instance.Find(ipAddress);
                                if (entity != null)
                                {
                                    _country = entity.Country;
                                }
                            }
                            _country = _country ?? CountryManagement.Default;

                            CookiesManager.SetString(SessionManager.CountryCodeKey, _country.Code);
                        }
                        SessionManager.SetString(SessionManager.CountryCodeKey, _country.Code);
                    }
                }
                return _country;
            }
        }

        public LanguageEntity Language
        {
            get
            {
                if (_language == null)
                {
                    string languageCode = SessionManager.GetString(SessionManager.LanguageCodeKey);
                    if (!string.IsNullOrEmpty(languageCode))
                    {
                        _language = LanguageManagement.Instance.FindByCode(languageCode);
                    }
                    if (_language == null)
                    {
                        languageCode = CookiesManager.GetString(SessionManager.LanguageCodeKey);
                        if (!string.IsNullOrEmpty(languageCode))
                        {
                            _language = LanguageManagement.Instance.FindByCode(languageCode);
                        }
                        if (_language == null)
                        {
                            _language = Country.Language;
                            CookiesManager.SetString(SessionManager.LanguageCodeKey, _language.Code);
                        }
                        SessionManager.SetString(SessionManager.LanguageCodeKey, _language.Code);
                    }
                }
                return _language;
            }
        }

        public bool Authorized
        {
            get { return User.UserRole != UserRole.Anonymous; }
        }

        public long UserId
        {
            get { return User.UserId; }
        }

        public string CountryCode
        {
            get { return Country.Code; }
        }

        public string LanguageCode
        {
            get { return Language.Code; }
        }

        public void Login(UserEntity user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (user.UserId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("user", string.Format("Specified entity is new (is not stored in database)."));
            if (user.UserId == UserManagement.Anonymous.UserId)
                throw new ArgumentOutOfRangeException("user", string.Format("Specified entity is anonimus."));

            _user = user;
            _country = user.Country;
            _language = user.Language;
            
            SessionManager.SetUserId(user.UserId);
            SessionManager.SetString(SessionManager.CountryCodeKey, _country.Code);
            SessionManager.SetString(SessionManager.LanguageCodeKey, _language.Code);
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

        IUser IContext.User
        {
            get { return User; }
        }

        public void Login(IUser user)
        {
            Login((UserEntity)user);
        }

        #endregion
    }
}