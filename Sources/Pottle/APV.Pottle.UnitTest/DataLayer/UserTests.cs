using System;
using APV.EntityFramework;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.DataLayer.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;
using APV.Common.Extensions;

namespace APV.Pottle.UnitTest.DataLayer
{
    [TestClass]
    public class UserTests : BaseTests<UserEntity>
    {
        protected override UserEntity CreateEntity()
        {
            return new UserEntity
                {
                    Username = "Username" + Guid.NewGuid(),
                    Password = "Password",
                    Email = "Email" + Guid.NewGuid(),
                    Language = LanguageManagement.Russian,
                    Country = CountryManagement.Russia,
                };
        }

        protected override void Modify(UserEntity entity)
        {
            Assert.IsNotNull(entity);
            entity.Email = "Email" + Guid.NewGuid();
        }

        [TestMethod]
        public void LoginTest()
        {
            var manager = (UserDataLayerManager)EntityFrameworkManager.GetManager<UserEntity>();
            Assert.IsNotNull(manager);

            UserEntity user = manager.Find(TestManager.AdminUsername, TestManager.AdminPasswod.Hash256());
            Assert.IsNotNull(user);

            long userId = user.UserId;
            Assert.IsTrue(userId != SystemConstants.UnknownId);
            Assert.AreEqual(TestManager.AdminUserId, userId);

            UserEntity loadedEntity = manager.Get(userId);
            Assert.IsNotNull(loadedEntity);

            bool equals = manager.Equals(user, loadedEntity);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void FindByEmailTest()
        {
            var manager = (UserDataLayerManager)EntityFrameworkManager.GetManager<UserEntity>();
            Assert.IsNotNull(manager);

            UserEntity user = manager.FindByEmail(TestManager.AdminEmail);
            Assert.IsNotNull(user);

            long userId = user.UserId;
            Assert.IsTrue(userId != SystemConstants.UnknownId);
            Assert.AreEqual(TestManager.AdminUserId, userId);

            UserEntity loadedEntity = manager.Get(userId);
            Assert.IsNotNull(loadedEntity);

            bool equals = manager.Equals(user, loadedEntity);
            Assert.IsTrue(equals);
        }
    }
}