using APV.Pottle.Toolkit.Navigation.Entities;
using APV.Pottle.Toolkit.Navigation.GeoLocators.Google;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.Toolkit
{
    [TestClass]
    public class GoogleGeoLocatorTests
    {
        public const string SuccessAddress = @"ул. Дыбенко, 12к1, Санкт-Петербург, 193168";

        public const string UnknownAddress = @"ул. ewrfewrfewrfewr, 193168";

        [TestMethod]
        public void SuccessAddressTest()
        {
            var locator = new GoogleGeoLocator();
            GeoAddress geoAddress = locator.GetGeoAddress(SuccessAddress);

            Assert.IsNotNull(geoAddress);
            Assert.IsNotNull(geoAddress.Location);
            Assert.IsNotNull(geoAddress.Address);
            Assert.IsNotNull(geoAddress.FormattedAddress);

            GeoAddress restoredAddress = locator.GetGeoAddress(geoAddress.Location);

            Assert.IsNotNull(restoredAddress);
            Assert.IsNotNull(geoAddress.Location);
            Assert.IsNotNull(geoAddress.Address);
            Assert.IsNotNull(geoAddress.FormattedAddress);

            Assert.AreEqual(geoAddress.FormattedAddress, restoredAddress.FormattedAddress);
        }

        [TestMethod]
        public void UnknownAddressTest()
        {
            var locator = new GoogleGeoLocator();
            GeoAddress geoAddress = locator.GetGeoAddress(UnknownAddress);

            Assert.IsNull(geoAddress);
        }
    }
}