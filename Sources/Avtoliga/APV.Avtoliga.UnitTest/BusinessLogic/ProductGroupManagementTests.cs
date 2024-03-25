using System;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.UnitTest.BusinessLogic.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Avtoliga.UnitTest.BusinessLogic
{
    [TestClass]
    public sealed class ProductGroupManagementTests : BaseManagementTests
    {
        [TestMethod]
        public void CreateAndDeletePermanetlyTest()
        {
            string name = string.Format("TEST_GROUP_{0}", Guid.NewGuid());
            var group = new ProductGroupEntity { Name = name, };
            group.Save();

            Assert.IsTrue(group.ProductGroupId > 0);

            group = ProductGroupManagement.Instance.Get(group.ProductGroupId);

            Assert.IsNotNull(group);
            Assert.AreEqual(name, group.Name);

            group.Delete();

            Assert.IsFalse(group.Deleted);
            group = ProductGroupManagement.Instance.Find(group.ProductGroupId);
            Assert.IsNull(group);
        }

        [TestMethod]
        public void DeleteAndRestoreTest()
        {
            ProductGroupEntity group = ProductGroupManagement.Instance.FindByName(TestManager.ProductGroupWithProduct);

            Assert.IsNotNull(group);
            Assert.IsTrue(group.ProductGroupId > 0);
            Assert.AreEqual(TestManager.ProductGroupWithProduct, group.Name);

            group.Delete();

            Assert.IsTrue(group.Deleted);
            group = ProductGroupManagement.Instance.Find(group.ProductGroupId);
            Assert.IsNotNull(group);
            Assert.IsTrue(group.Deleted);

            group.Restore();
            group.Reload();

            Assert.IsFalse(group.Deleted);
        }
    }
}