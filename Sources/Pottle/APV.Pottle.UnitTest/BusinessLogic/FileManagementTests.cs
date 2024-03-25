using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.BusinessLogic.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class FileManagementTests : BaseManagementTests
    {
        [TestMethod]
        public void CreateTest()
        {
            FileManagement management = FileManagement.Instance;

            var newFile = new DataFileEntity
                {
                    Data = new byte[12],
                    DataStorage = DataStorage.Database,
                    Path = null,
                };

            management.Save(newFile);

            var loadedFile = new DataFileEntity(newFile.FileId);
            bool equals = newFile.Equals(loadedFile);
            Assert.IsTrue(equals);

            UserEntity user = newFile.CreatedBy;
            Assert.IsNotNull(user);
            Assert.AreEqual(TestManager.TestUserId, user.UserId);
        }
    }
}