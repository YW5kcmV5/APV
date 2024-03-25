using System.Net;

namespace APV.Pottle.Toolkit.Navigation.Entities
{
    public class IPLocation
    {
        public IPAddress IP { get; set; }

        public string CountryCode { get; set; }

        public string CountryName { get; set; }

        public string City { get; set; }

        public GeoLocation Location { get; set; }
    }
}