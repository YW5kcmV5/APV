using System;
using APV.Pottle.Common;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.DataLayer.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.DataLayer
{
    [TestClass]
    public class DataFileTests : BaseTests<DataFileEntity>
    {
        protected override DataFileEntity CreateEntity()
        {
            byte[] data = Guid.NewGuid().ToByteArray();
            return new DataFileEntity
                {
                    Data = data,
                    DataStorage = DataStorage.Database,
                    Path = null,
                };
        }

        protected override void Modify(DataFileEntity entity)
        {
            Assert.IsNotNull(entity);
            entity.Data = new byte[13];
        }
    }
}