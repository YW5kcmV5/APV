using System.Security.Principal;

namespace APV.EntityFramework.Interfaces
{
    public interface IUser : IIdentifier, IIdentity
    {
        string Username { get; }

        long UserId { get; }

        UserRole UserRole { get; }
    }
}