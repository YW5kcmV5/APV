using System;
using APV.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.BusinessLogic.Extensions;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.BusinessLogic.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class UserManagementTests : BaseManagementTests
    {
        private static UserEntity CreateUser()
        {
            return new UserEntity
                {
                    Username = "Username" + Guid.NewGuid(),
                    Password = "Password",
                    Email = "Email" + Guid.NewGuid(),
                    Language = LanguageManagement.Default,
                    Country = CountryManagement.Default,
                };
        }

        [TestMethod]
        public void CreateTest()
        {
            UserManagement management = UserManagement.Instance;

            UserEntity newUser = CreateUser();

            management.Save(newUser);

            var loadedUser = new UserEntity(newUser.UserId);
            bool equals = newUser.Equals(loadedUser);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void DeleteTest()
        {
            UserManagement management = UserManagement.Instance;

            UserEntity newUser = CreateUser();

            management.Save(newUser);

            bool exists = management.Exists(newUser.UserId);
            Assert.IsTrue(exists);

            management.Delete(newUser);

            exists = management.Exists(newUser.UserId);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void SaveTest()
        {
            UserEntity newUser = CreateUser();

            newUser.Save();

            bool exists = UserManagement.Instance.Exists(newUser.UserId);
            Assert.IsTrue(exists);

            newUser.Delete();

            exists = UserManagement.Instance.Exists(newUser.UserId);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void SetLocationTest()
        {
            UserEntity newUser = CreateUser();
            newUser.SetLocation(@"ул. Дыбенко, 12к1, Санкт-Петербург, 193168");

            Assert.AreNotEqual(SystemConstants.UnknownId, newUser.UserId);

            var loadedUser = new UserEntity(newUser.UserId);
            Assert.IsNotNull(loadedUser);

            Assert.AreEqual(newUser.UserId, loadedUser.UserId);
            Assert.IsNotNull(loadedUser.AddressId);
            Assert.IsNotNull(loadedUser.Address);
            Assert.IsNotNull(newUser.AddressId);
            Assert.AreEqual(loadedUser.Address.AddressId, loadedUser.Address.AddressId);
            Assert.AreEqual(loadedUser.Address.LocationId, loadedUser.Address.LocationId);
        }
    }
}