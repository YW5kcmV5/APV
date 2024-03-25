using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.UnitTest.BusinessLogic.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class CompanyManagementTests : BaseManagementTests
    {
        [TestMethod]
        public void FormatNameTest()
        {
            CompanyManagement management = CompanyManagement.Instance;

            Assert.AreEqual(@"ООО Томь-Сервис", management.FormatName(@"ООО 'Томь-Сервис'"));
            Assert.AreEqual(@"Томь-Сервис", management.FormatName(@" 'Томь-Сервис'"));
            Assert.AreEqual(@"Томь-Сервис", management.FormatName(@" ' Томь-Сервис'"));
        }
    }
}