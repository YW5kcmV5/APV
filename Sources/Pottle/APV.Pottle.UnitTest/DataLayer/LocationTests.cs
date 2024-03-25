using System;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.DataLayer.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.DataLayer
{
    [TestClass]
    public class LocationTests : BaseTests<LocationEntity>
    {
        protected override LocationEntity CreateEntity()
        {
            return new LocationEntity
                {
                    Address = "Address",
                    LAT = (float) 0.01,
                    LON = (float) 0.01,
                };
        }

        protected override void Modify(LocationEntity entity)
        {
            Assert.IsNotNull(entity);
            entity.Address = "Address" + Guid.NewGuid();
        }
    }
}