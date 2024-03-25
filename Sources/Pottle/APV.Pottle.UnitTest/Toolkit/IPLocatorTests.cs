using System.Net;
using APV.Pottle.Toolkit.Navigation.Entities;
using APV.Pottle.Toolkit.Navigation.GeoLocators.Google;
using APV.Pottle.Toolkit.Navigation.IPLocators.IpApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.Toolkit
{
    [TestClass]
    public class IPLocatorTests
    {
        public const string SuccessAddress = "212.119.177.5";

        public const string UnknownAddress = @"127.0.0.1";

        [TestMethod]
        public void SuccessAddressTest()
        {
            var locator = new IPLocator();
            IPAddress ip = IPAddress.Parse(SuccessAddress);
            IPLocation ipLocation = locator.GetIPLocation(ip);

            Assert.IsNotNull(ipLocation);
            Assert.IsNotNull(ipLocation.Location);
            Assert.IsNotNull(ipLocation.CountryCode);
            Assert.IsNotNull(ipLocation.CountryName);
            Assert.IsNotNull(ipLocation.City);
            Assert.AreEqual(ip, ipLocation.IP);

            var geoLocator = new GoogleGeoLocator();
            GeoAddress geoAddress = geoLocator.GetGeoAddress(ipLocation.Location);

            Assert.IsNotNull(geoAddress);
            Assert.IsNotNull(geoAddress.Location);
            Assert.IsNotNull(geoAddress.Address);
            Assert.IsNotNull(geoAddress.FormattedAddress);
        }

        [TestMethod]
        public void UnknownAddressTest()
        {
            var locator = new IPLocator();
            IPAddress ip = IPAddress.Parse(UnknownAddress);
            IPLocation ipLocation = locator.GetIPLocation(ip);

            Assert.IsNull(ipLocation);
        }
    }
}
