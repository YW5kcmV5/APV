using APV.EntityFramework;
using APV.EntityFramework.Interfaces;
using APV.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class UserContextTest
    {
        [TestMethod]
        public void AnonymousTest()
        {
            IContextManager contextManager = EntityFrameworkManager.GetContextManager();
            IContext context = contextManager.GetContext();

            Assert.IsNotNull(context);

            context.Logout();

            Assert.IsFalse(context.Authorized);

            Assert.IsNotNull(context.User);
            Assert.AreEqual(SystemConstants.DefaultCountryCode, context.CountryCode);
            Assert.AreEqual(SystemConstants.DefaultLanguageCode, context.LanguageCode);

            TestManager.Login();

            Assert.AreEqual(TestManager.TestUserId, context.UserId);
        }
    }
}