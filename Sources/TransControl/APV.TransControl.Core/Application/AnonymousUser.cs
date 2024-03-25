using APV.EntityFramework;
using APV.EntityFramework.Interfaces;
using APV.Common;

namespace APV.TransControl.Core.Application
{
    public sealed class AnonymousUser : IUser
    {
        public long Id
        {
            get { return SystemConstants.UnknownId; }
        }

        public long UserId
        {
            get { return Id; }
        }

        public string Name
        {
            get { return UserRole.ToString(); }
        }

        public string Username
        {
            get { return Name; }
        }

        public UserRole UserRole
        {
            get { return UserRole.Anonymous; }
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
