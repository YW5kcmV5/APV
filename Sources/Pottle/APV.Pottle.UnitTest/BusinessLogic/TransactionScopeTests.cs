using System;
using System.Threading;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.Application;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.BusinessLogic.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class TransactionScopeTests : BaseManagementTests
    {
        [TestMethod]
        public void DefaultRollbackTest()
        {
            string name1 = string.Format("Test_{0}", Guid.NewGuid());
            string name2;

            using (new TransactionScope())
            {
                using (var scope2 = new TransactionScope())
                {
                    var entity = new BusinessTypeEntity { Country = CountryManagement.Russia, Name = name1 };
                    entity.Save();

                    scope2.Commit();
                }

                long businessTypeId = BusinessTypeManagement.Instance.GetByName(name1).BusinessTypeId;
                name2 = string.Format("{0}_{1}", name1, businessTypeId);

                using (var scope3 = new TransactionScope())
                {
                    var entity = new BusinessTypeEntity { Country = CountryManagement.Russia, Name = name2 };
                    entity.Save();

                    scope3.Commit();
                }

                //No explicit rollaback = "Dispose" should call rollback
            }

            Assert.IsNotNull(name2);

            BusinessTypeEntity entity1 = BusinessTypeManagement.Instance.FindByName(name1);
            BusinessTypeEntity entity2 = BusinessTypeManagement.Instance.FindByName(name2);

            Assert.IsNull(entity1);
            Assert.IsNull(entity2);

            Assert.IsFalse(string.IsNullOrEmpty(SqlProvider.ThreadInstance.Log));
        }

        [TestMethod]
        public void ExtensionRollbackTest()
        {
            string name1 = string.Format("Test_{0}", Guid.NewGuid());
            string name2 = null;

            try
            {
                using (new TransactionScope())
                {
                    using (var scope2 = new TransactionScope())
                    {
                        var entity = new BusinessTypeEntity {Country = CountryManagement.Russia, Name = name1};
                        entity.Save();

                        scope2.Commit();
                    }

                    long businessTypeId = BusinessTypeManagement.Instance.GetByName(name1).BusinessTypeId;
                    name2 = string.Format("{0}_{1}", name1, businessTypeId);

                    using (var scope3 = new TransactionScope())
                    {
                        var entity = new BusinessTypeEntity {Country = CountryManagement.Russia, Name = name2};
                        entity.Save();

                        scope3.Commit();
                    }

                    //No explicit rollaback = Exception calls "Dispose", "Dispose" should call rollback
                    throw new InvalidOperationException("1");
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message != "1")
                    throw;
            }

            Assert.IsNotNull(name2);

            BusinessTypeEntity entity1 = BusinessTypeManagement.Instance.FindByName(name1);
            BusinessTypeEntity entity2 = BusinessTypeManagement.Instance.FindByName(name2);

            Assert.IsNull(entity1);
            Assert.IsNull(entity2);

            Assert.IsFalse(string.IsNullOrEmpty(SqlProvider.ThreadInstance.Log));
        }

        [TestMethod]
        public void InnerScopeExtensionRollbackTest()
        {
            string name1 = string.Format("Test_{0}", Guid.NewGuid());
            string name2 = null;

            try
            {
                //T_0_0
                using (new TransactionScope())
                {
                    //T_0_1
                    using (var scope2 = new TransactionScope())
                    {
                        var entity = new BusinessTypeEntity { Country = CountryManagement.Russia, Name = name1 };
                        entity.Save();

                        scope2.Commit();
                    }

                    long businessTypeId = BusinessTypeManagement.Instance.GetByName(name1).BusinessTypeId;
                    name2 = string.Format("{0}_{1}", name1, businessTypeId);

                    //T_0_2
                    using (new TransactionScope())
                    {
                        var entity = new BusinessTypeEntity { Country = CountryManagement.Russia, Name = name2 };
                        entity.Save();

                        //No explicit rollaback = Exception calls "Dispose", "Dispose" should call rollback
                        throw new InvalidOperationException("1");
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message != "1")
                    throw;
            }

            Assert.IsNotNull(name2);

            BusinessTypeEntity entity1 = BusinessTypeManagement.Instance.FindByName(name1);
            BusinessTypeEntity entity2 = BusinessTypeManagement.Instance.FindByName(name2);

            Assert.IsNull(entity1);
            Assert.IsNull(entity2);

            Assert.IsFalse(string.IsNullOrEmpty(SqlProvider.ThreadInstance.Log));
        }

        [TestMethod]
        public void NoSqlCallCommitTest()
        {
            using (var scope = new TransactionScope())
            {
                scope.Commit();
            }

            Assert.IsTrue(string.IsNullOrEmpty(SqlProvider.ThreadInstance.Log));
        }

        [TestMethod]
        public void LostInnerCommitCallTest()
        {
            try
            {
                string name1 = string.Format("Test_{0}", Guid.NewGuid());

                using (var scope1 = new TransactionScope())
                {
                    using (new TransactionScope())
                    {
                        var entity = new BusinessTypeEntity {Country = CountryManagement.Russia, Name = name1};
                        entity.Save();

                        //No explicit rollaback = "Dispose" should call rollback
                    }

                    scope1.Commit();
                }
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(InvalidOperationException), ex.GetType());
            }
        }

        [TestMethod]
        public void WatchdogTimerTest()
        {
            string name1 = string.Format("Test_{0}", Guid.NewGuid());

            try
            {
                using (var scope1 = new TransactionScope())
                {
                    var entity = new BusinessTypeEntity { Country = CountryManagement.Russia, Name = name1 };
                    entity.Save();

                    Thread.Sleep(1000*ContextManager.ConnectionTimeoutInSec);

                    scope1.Commit();
                }
            }
            catch (Exception ex)
            {
                Assert.AreEqual(typeof(InvalidOperationException), ex.GetType());
            }

            BusinessTypeEntity entity1 = BusinessTypeManagement.Instance.FindByName(name1);

            Assert.IsNull(entity1);
        }
    }
}