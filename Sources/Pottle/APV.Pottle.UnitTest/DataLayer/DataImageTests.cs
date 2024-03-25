using System;
using APV.Pottle.Common;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.DataLayer.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common.Extensions;
using APV.GraphicsLibrary;

namespace APV.Pottle.UnitTest.DataLayer
{
    [TestClass]
    public class DataImageTests : BaseTests<DataImageEntity>
    {
        protected override DataImageEntity CreateEntity()
        {
            byte[] data = Guid.NewGuid().ToByteArray();
            byte[] hashCode = data.Hash256();
            return new DataImageEntity
                {
                    Data = data,
                    HashCode = hashCode,
                    DataStorage = DataStorage.Database,
                    Path = null,
                    Height = 16,
                    Width = 16,
                    ImageFormat = ImageFormat.Png,
                };
        }

        protected override void Modify(DataImageEntity entity)
        {
            Assert.IsNotNull(entity);
            entity.Data = new byte[13];
            entity.Width = 17;
            entity.Height = 17;
        }
    }
}