using System;
using System.Collections.Generic;
using System.Reflection;
using APV.EntityFramework;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Pottle.Common.Application;
using APV.Pottle.Core.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common.Attributes.Db;

namespace APV.Pottle.UnitTest.DataLayer.Base
{
    [TestClass]
    public abstract class BaseTests<TEntity> where TEntity : BaseEntity
    {
        private readonly List<long> _identifiersToDelete = new List<long>();

        static BaseTests()
        {
            Assembly dataLayerAssembly = typeof(ContextManager).Assembly;
            ApplicationManager.Register(dataLayerAssembly);

            TestManager.Login();
        }

        private TEntity Create()
        {
            TEntity entity = CreateEntity();
            Assert.IsNotNull(entity);

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            long id = dataLayerManager.CreateOrUpdate(entity);
            Assert.IsFalse(id == SystemConstants.UnknownId);
            Assert.AreEqual(id, entity.Id);

            _identifiersToDelete.Add(id);
            return entity;
        }

        protected abstract TEntity CreateEntity();

        protected abstract void Modify(TEntity entity);

        [TestCleanup]
        public void ClassCleanup()
        {
            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            foreach (long id in _identifiersToDelete)
            {
                dataLayerManager.Delete(id);
            }
        }

        [TestMethod]
        public void CreateTest()
        {
            TEntity entity = Create();
            Assert.IsNotNull(entity);
        }

        [TestMethod]
        public void SerializeDeserializeTest()
        {
            TEntity entity = Create();
            Assert.IsNotNull(entity);

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            byte[] dump = dataLayerManager.Serialize(entity);
            Assert.IsNotNull(dump);
            Assert.IsTrue(dump.Length > 0);

            TEntity restoredEntity = dataLayerManager.Deserialize(dump);
            Assert.IsNotNull(restoredEntity);

            bool equals = dataLayerManager.Equals(entity, restoredEntity);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void CloneTest()
        {
            TEntity entity = Create();
            Assert.IsNotNull(entity);

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            TEntity copy = dataLayerManager.Clone(entity);
            Assert.IsNotNull(copy);

            bool equals = dataLayerManager.Equals(entity, copy);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void ConstructorTest()
        {
            TEntity entity = Create();
            Assert.IsNotNull(entity);

            long id = entity.Id;

            var loadedEntity = (TEntity)Activator.CreateInstance(typeof(TEntity), id);
            Assert.IsNotNull(loadedEntity);

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            bool equals = dataLayerManager.Equals(entity, loadedEntity);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void UpdateTest()
        {
            TEntity entity = Create();
            Assert.IsNotNull(entity);

            Modify(entity);

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            dataLayerManager.CreateOrUpdate(entity);

            TEntity loadedEntity = dataLayerManager.Get(entity.Id);
            Assert.IsNotNull(loadedEntity);

            bool equals = dataLayerManager.Equals(entity, loadedEntity);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void FindTest()
        {
            TEntity entity = Create();

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            TEntity loadedEntity = dataLayerManager.Find(entity.Id);
            Assert.IsNotNull(loadedEntity);

            bool equals = dataLayerManager.Equals(entity, loadedEntity);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void FindNotExistsTest()
        {
            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            TEntity loadedEntity = dataLayerManager.Find(TestManager.NotExistsId);
            Assert.IsNull(loadedEntity);
        }

        [TestMethod]
        public void FindByNameTest()
        {
            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            DbTableAttribute attribute = DbTableAttribute.GetAttribute(typeof(TEntity));
            Assert.IsNotNull(attribute);

            if (attribute.Support(DbOperation.GetByName))
            {
                Assert.IsNotNull(attribute.NameField);

                TEntity entity = Create();

                TEntity loadedEntity = dataLayerManager.FindByName(entity.GetName());
                Assert.IsNotNull(loadedEntity);

                bool equals = dataLayerManager.Equals(entity, loadedEntity);
                Assert.IsTrue(equals);
            }
        }

        [TestMethod]
        public void FindByNameNotExistsTest()
        {
            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            DbTableAttribute attribute = DbTableAttribute.GetAttribute(typeof(TEntity));
            Assert.IsNotNull(attribute);

            if (attribute.Support(DbOperation.GetByName))
            {
                TEntity loadedEntity = dataLayerManager.FindByName(TestManager.NotExistsName);
                Assert.IsNull(loadedEntity);
            }
        }

        [TestMethod]
        public void GetTest()
        {
            TEntity entity = Create();

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            TEntity loadedEntity = dataLayerManager.Get(entity.Id);
            Assert.IsNotNull(loadedEntity);

            bool equals = dataLayerManager.Equals(entity, loadedEntity);
            Assert.IsTrue(equals);
        }

        [TestMethod]
        public void ExistsTest()
        {
            TEntity entity = Create();

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            bool exists = dataLayerManager.Exists(entity.Id);
            Assert.IsTrue(exists);

            exists = dataLayerManager.Exists(TestManager.NotExistsId);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void ExistsByNameTest()
        {
            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            DbTableAttribute attribute = DbTableAttribute.GetAttribute(typeof(TEntity));
            Assert.IsNotNull(attribute);

            if (attribute.Support(DbOperation.GetByName))
            {
                TEntity entity = Create();

                bool exists = dataLayerManager.Exists(entity.GetName());
                Assert.IsTrue(exists);

                exists = dataLayerManager.Exists(TestManager.NotExistsId);
                Assert.IsFalse(exists);
            }
        }

        [TestMethod]
        public void ExistsByNameNotExistsTest()
        {
            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            DbTableAttribute attribute = DbTableAttribute.GetAttribute(typeof(TEntity));
            Assert.IsNotNull(attribute);

            if (attribute.Support(DbOperation.GetByName))
            {
                bool exists = dataLayerManager.Exists(TestManager.NotExistsName);
                Assert.IsFalse(exists);
            }
        }

        [TestMethod]
        public void DeleteByIdTest()
        {
            TEntity entity = Create();
            long id = entity.Id;

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            bool exists = dataLayerManager.Exists(id);
            Assert.IsTrue(exists);

            dataLayerManager.Delete(entity.Id);

            exists = dataLayerManager.Exists(id);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void DeleteTest()
        {
            TEntity entity = Create();
            long id = entity.Id;

            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            bool exists = dataLayerManager.Exists(id);
            Assert.IsTrue(exists);

            dataLayerManager.Delete(entity);

            exists = dataLayerManager.Exists(id);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void MarkAsDeletedByIdTest()
        {
            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            DbTableAttribute attribute = DbTableAttribute.GetAttribute(typeof(TEntity));
            Assert.IsNotNull(attribute);

            if (attribute.Support(DbOperation.MarkAsDeleted))
            {
                TEntity entity = Create();
                long id = entity.Id;

                dataLayerManager.MarkAsDeleted(id);

                TEntity loadedEntity = dataLayerManager.Find(entity.Id);
                Assert.IsNotNull(loadedEntity);

                var deleted = (bool)attribute.DeletedField.GetValue(loadedEntity);
                Assert.IsTrue(deleted);
            }
            else
            {
                try
                {
                    dataLayerManager.MarkAsDeleted(SystemConstants.UnknownId);

                    Assert.Fail("\"NotSupportedException\" is expcted. Entity \"{0}\" does not support \"MarkAsDeleted\" operation.", typeof(TEntity).Name);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(typeof(NotSupportedException), ex.GetType());
                }
            }
        }

        [TestMethod]
        public void MarkAsDeletedTest()
        {
            IDataLayerManager<TEntity> dataLayerManager = EntityFrameworkManager.GetManager<TEntity>();
            Assert.IsNotNull(dataLayerManager);

            DbTableAttribute attribute = DbTableAttribute.GetAttribute(typeof(TEntity));
            Assert.IsNotNull(attribute);

            if (attribute.Support(DbOperation.MarkAsDeleted))
            {
                TEntity entity = Create();
                dataLayerManager.MarkAsDeleted(entity);

                TEntity loadedEntity = dataLayerManager.Find(entity.Id);
                Assert.IsNotNull(loadedEntity);

                var deleted = (bool)attribute.DeletedField.GetValue(loadedEntity);
                Assert.IsTrue(deleted);
            }
            else
            {
                try
                {
                    dataLayerManager.MarkAsDeleted(SystemConstants.UnknownId);

                    Assert.Fail("\"NotSupportedException\" is expcted. Entity \"{0}\" does not support \"MarkAsDeleted\" operation.", typeof(TEntity).Name);
                }
                catch (Exception ex)
                {
                    Assert.AreEqual(typeof(NotSupportedException), ex.GetType());
                }
            }
        }
    }
}