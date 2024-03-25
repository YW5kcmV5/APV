using APV.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.System
{
    [TestClass]
    public class WebUtilityTests
    {
        [TestMethod]
        public void IsUrlAvailableSuccessTest()
        {
            const string url = "www.google.ru";
            bool alive = WebUtility.IsUrlAvailable(url);
            Assert.IsTrue(alive);
        }

        [TestMethod]
        public void IsUrlNotAvailableSuccessTest()
        {
            const string url = "www.wekfjklwqefhqkwehfqkwehfqw.ru";
            bool alive = WebUtility.IsUrlAvailable(url);
            Assert.IsFalse(alive);
        }
    }
}