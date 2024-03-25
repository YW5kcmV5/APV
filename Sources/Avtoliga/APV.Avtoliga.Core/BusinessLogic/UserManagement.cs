using APV.Avtoliga.Common;
using APV.Avtoliga.Core.DataLayer;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.Common;
using APV.Common.Extensions;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public class UserManagement : BaseManagement<UserEntity, UserCollection, UserDataLayerManager>
    {
        [AnonymousAccess]
        public bool Login(string username, string password)
        {
            if ((!string.IsNullOrWhiteSpace(username)) && 
                (!string.IsNullOrWhiteSpace(password)) &&
                (username.Length >= Constants.MinUsernameLength) && 
                (password.Length >= Constants.MinPasswordLength))
            {
                byte[] passwordHash = password.Hash256();
                UserEntity user = DatabaseManager.Login(username, passwordHash);
                if (user != null)
                {
                    Context.Login(user);
                    return true;
                }
            }
            return false;
        }

        [AdminAccess]
        public override void Save(UserEntity entity)
        {
            base.Save(entity);
        }

        public static readonly UserManagement Instance = (UserManagement)EntityFrameworkManager.GetManagement<UserEntity>();
    }
}