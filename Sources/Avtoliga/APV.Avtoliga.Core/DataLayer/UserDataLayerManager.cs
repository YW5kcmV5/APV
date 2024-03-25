using System;
using System.Collections.Generic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.DataLayer
{
    public class UserDataLayerManager : BaseDataLayerManager<UserEntity, UserCollection>
    {
        public UserEntity Login(string username, byte[] passwordHash)
        {
            if (username == null)
                throw new ArgumentNullException("username");
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentOutOfRangeException("username", "Username is empty or whitespace.");
            if (passwordHash == null)
                throw new ArgumentNullException("passwordHash");

            const string whereSql = @"WHERE ((UPPER([User].Username) = @Username) AND ([User].PasswordHash = @PasswordHash))";
            var @params = new Dictionary<string, object> { { "@Username", username.ToUpperInvariant() }, { "@PasswordHash", passwordHash } };
            return Get(whereSql, @params);
        }
    }
}