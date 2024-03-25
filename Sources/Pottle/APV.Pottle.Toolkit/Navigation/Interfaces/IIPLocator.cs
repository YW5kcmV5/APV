using System.Net;
using APV.Pottle.Toolkit.Navigation.Entities;

namespace APV.Pottle.Toolkit.Navigation.Interfaces
{
    public interface IIPLocator
    {
        IPLocation GetIPLocation(IPAddress ip);
    }
}