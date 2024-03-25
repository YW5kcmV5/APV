using System;
using System.Collections.Generic;
using APV.EntityFramework;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.DataLayer
{
    public class UserDataLayerManager : BaseDataLayerManager<UserEntity, UserCollection>
    {
        public UserEntity Find(string username, byte[] password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");
            if (password == null)
                throw new ArgumentNullException("password");

            const string sql = @"WHERE [User].[Username] = @Username AND [User].[Password] = @Password";
            var @params = new Dictionary<string, object>
                {
                    { "@Username", username },
                    { "@Password", password },
                };

            return Find(sql, @params);
        }

        public UserEntity FindByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");

            const string sql = @"WHERE [User].[Email] = @Email";
            var @params = new Dictionary<string, object> { { "@Email", email } };
            return Find(sql, @params);
        }

        public UserEntity GetAnonymous()
        {
            const string sql = @"WHERE [User].[UserRole] = @UserRole";
            var @params = new Dictionary<string, object>{ { "@UserRole", (int) UserRole.Anonymous } };

            return Get(sql, @params);
        }
    }
}