using System;
using APV.Pottle.Toolkit.Navigation.Entities;
using APV.Pottle.Toolkit.Navigation.GeoLocators.Google;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.DatabaseConsoleTest
{
    public static class GeoLocatorTests
    {
        public const string TestAddress = @"ул. Дыбенко, 12к1, Санкт-Петербург, 193168";

        private static void GoogleGeoLocatorTest()
        {
            var locator = new GoogleGeoLocator();
            GeoAddress geoAddress = locator.GetGeoAddress(TestAddress);
            Assert.IsNotNull(geoAddress);
            Assert.IsNotNull(geoAddress.Location);
            Assert.IsNotNull(geoAddress.Address);
            Assert.IsNotNull(geoAddress.FormattedAddress);
        }

        public static void Execute()
        {
            try
            {
                GoogleGeoLocatorTest();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}