using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Common;
using APV.Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Avtoliga.UnitTest.BusinessLogic
{
    [TestClass]
    public class UserManagementTests
    {
        [TestMethod]
        public void CreateAdminTest()
        {
            var user = new UserEntity
                {
                    Username = "Admin",
                    PasswordHash = "Avtoliga.xml".Hash256(),
                };
            UserManagement.Instance.Save(user);
        }
    }
}
