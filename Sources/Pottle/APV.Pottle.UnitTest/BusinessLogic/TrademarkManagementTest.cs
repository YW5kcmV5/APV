using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.BusinessLogic.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class TrademarkManagementTest : BaseManagementTests
    {
        private TrademarkEntity Create()
        {
            var trademark = new TrademarkEntity
                {
                    Name = TestManager.TestTrademarkName,
                    Description = "Тестовая торговая марка для тестирования бизнесс логики и поиску по ключевым словам",
                    Country = CountryManagement.Russia,
                };
            return trademark;
        }

        [TestMethod]
        public void CreateDeleteTest()
        {
            TrademarkEntity trademark = TrademarkManagement.Instance.FindByName(TestManager.TestTrademarkName);

            if (trademark != null)
            {
                TrademarkManagement.Instance.Delete(trademark);
            }

            trademark = Create();

            trademark.Save();
            long trademarkId = trademark.TrademarkId;

            KeywordEntity keyword = KeywordManagement.Instance.Find(trademark);
            
            Assert.IsNotNull(keyword);
            Assert.IsNotNull(keyword.KeywordReferences);
            Assert.IsTrue(keyword.KeywordReferences.Count > 0);

            trademark.Delete();

            bool exists = KeywordManagement.Instance.Exists(trademarkId);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void MarkAsDeletedTest()
        {
            TrademarkEntity trademark = TrademarkManagement.Instance.FindByName(TestManager.TestTrademarkName);

            if (trademark == null)
            {
                trademark = Create();
                trademark.Save();
            }

            long trademarkId = trademark.TrademarkId;

            trademark.MarkAsDeleted();

            trademark = TrademarkManagement.Instance.Find(trademarkId);

            Assert.IsNotNull(trademark);
            Assert.IsTrue(trademark.Deleted);

            KeywordEntity keyword = KeywordManagement.Instance.Find(trademark);

            Assert.IsNull(keyword);

            trademark.Delete();

            bool exists = KeywordManagement.Instance.Exists(trademarkId);
            Assert.IsFalse(exists);
        }
    }
}