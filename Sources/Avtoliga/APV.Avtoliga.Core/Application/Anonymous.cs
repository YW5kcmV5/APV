using APV.Common;
using APV.EntityFramework;
using APV.EntityFramework.Interfaces;

namespace APV.Avtoliga.Core.Application
{
    public sealed class Anonymous : IUser
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
            get { return "Anonymous"; }
        }

        public string Username
        {
            get { return "Anonymous"; }
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
            get { return false; }
        }
    }
}