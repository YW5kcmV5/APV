using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.UnitTest.BusinessLogic.Base;
using APV.Common.Periods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Avtoliga.UnitTest.BusinessLogic
{
    [TestClass]
    public sealed class TrademarkManagementTests : BaseManagementTests
    {
        [TestMethod]
        public void CreateProduct()
        {
            const string name = @"Фара противотуманная лев=прав";
            const string trademarkName = @"Citroen";
            const string modelName = @"C4 / Picasso";
            const string modelPeriodValue = @"2010- / 2010-";
            const string producerName = @"Depo";
            const string groupName = @"Фара противотуманная";
            const string periodValue = @"2010-";

            ProductGroupEntity group = ProductGroupManagement.Instance.FindByName(groupName);

            Assert.IsNotNull(group);

            TrademarkEntity trademark = TrademarkManagement.Instance.FindByName(trademarkName);

            Assert.IsNotNull(trademark);

            var modelPeriod = new AnnualPeriodCollection(modelPeriodValue);
            ModelEntity model = ModelManagement.Instance.Find(trademark, modelName, modelPeriod);

            Assert.IsNotNull(model);

            ProducerEntity producer = ProducerManagement.Instance.FindByName(producerName);

            Assert.IsNotNull(producer);

            var period = new AnnualPeriodInfo(periodValue);
            var product = ProductManagement.Instance.Find(model, name, period);
            product = product ?? new ProductEntity();

            product.Name = name;
            product.Article = "6208Q3";
            product.Model = model;
            product.ProductGroup = group;
            product.Producer = producer;
            product.ProducerArticle = "552-2010N-UE";
            product.PeriodInfo = period;
            product.DeliveryTime = 1;

            product.Save();

            Assert.IsTrue(product.ProductId > 0);
        }
    }
}