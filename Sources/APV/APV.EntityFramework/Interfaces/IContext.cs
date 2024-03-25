using System.Security.Principal;

namespace APV.EntityFramework.Interfaces
{
    public interface IContext : IPrincipal
    {
        IUser User { get; }

        bool Authorized { get; }

        long UserId { get; }

        string CountryCode { get; }

        string LanguageCode { get; }

        void Login(IUser user);

        void Logout();
    }
}