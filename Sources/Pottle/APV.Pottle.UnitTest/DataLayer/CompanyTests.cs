using System;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.DataLayer.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.DataLayer
{
    [TestClass]
    public class CompanyTests : BaseTests<CompanyEntity>
    {
        protected override CompanyEntity CreateEntity()
        {
            string guid = Guid.NewGuid().ToString();
            return new CompanyEntity
                {
                    Name = "CompanyName " + guid,
                    LegalName = "OOO CompanyName " + guid,
                    CompanyName = null,
                    Description = "Description",
                    Country = CountryManagement.Russia,
                    BusinessType = BusinessTypeManagement.OOO,
                };
        }

        protected override void Modify(CompanyEntity entity)
        {
            Assert.IsNotNull(entity);
            entity.Description = "Description" + Guid.NewGuid();
        }
    }
}