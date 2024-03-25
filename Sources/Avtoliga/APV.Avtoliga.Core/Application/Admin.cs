using APV.EntityFramework;
using APV.EntityFramework.Interfaces;

namespace APV.Avtoliga.Core.Application
{
    public sealed class Admin : IUser
    {
        public long Id
        {
            get { return 0; }
        }

        public long UserId
        {
            get { return Id; }
        }

        public string Name
        {
            get { return "Администратор"; }
        }

        public string Username
        {
            get { return "Admin"; }
        }

        public UserRole UserRole
        {
            get { return UserRole.Administrator; }
        }

        public string AuthenticationType
        {
            get { return string.Empty; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }
    }
}
