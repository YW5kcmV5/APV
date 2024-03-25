using APV.Pottle.Toolkit.Navigation.Entities;

namespace APV.Pottle.Toolkit.Navigation.Interfaces
{
    public interface IGeoLocator
    {
        GeoAddress GetGeoAddress(string address);

        GeoAddress GetGeoAddress(GeoLocation location);
    }
}