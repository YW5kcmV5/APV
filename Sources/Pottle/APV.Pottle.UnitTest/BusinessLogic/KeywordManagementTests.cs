using System.Linq;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;
using APV.Pottle.UnitTest.BusinessLogic.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class KeywordManagementTests : BaseManagementTests
    {
        [TestMethod]
        public void SaveNameGenerateKeywordsSearchTest()
        {
            KeywordManagement management = KeywordManagement.Instance;

            ProductEntity product = ProductManagement.Instance.Get(TestManager.TestProductId);
            long productId = product.ProductId;
            long entityTypeId = product.TypeId;

            TrademarkEntity trademark = product.Trademark;

            Assert.IsNotNull(trademark);

            WordManagement.Instance.SaveName(trademark);
            
            KeywordEntity keyword = management.GenerateKeywords(product);

            Assert.IsNotNull(keyword);
            Assert.IsNotNull(keyword.KeywordReferences);
            Assert.IsTrue(keyword.KeywordReferences.Count > 0);

            Assert.IsFalse(string.IsNullOrEmpty(SqlProvider.ThreadInstance.Log));

            Assert.IsTrue(keyword.KeywordReferences.Cast<KeywordReferenceEntity>().Any(x => x.Word == "margot"));
            Assert.IsTrue(keyword.KeywordReferences.Cast<KeywordReferenceEntity>().Any(x => x.Word == "bis"));
            Assert.IsTrue(keyword.KeywordReferences.Cast<KeywordReferenceEntity>().Any(x => x.Word == "маргот"));
            Assert.IsTrue(keyword.KeywordReferences.Cast<KeywordReferenceEntity>().Any(x => x.Word == "бис"));

            KeywordCollection keywords = management.SearchIdentifiers(trademark.Name);

            Assert.IsNotNull(keywords);
            Assert.IsNotNull(keywords.Count > 0);

            KeywordEntity entity = keywords.Cast<KeywordEntity>().SingleOrDefault(x => (x.EntityId == productId) && (x.EntityTypeId == entityTypeId));

            Assert.IsNotNull(entity);
        }

        [TestMethod]
        public void GenerateKeywordsTest()
        {
            KeywordManagement management = KeywordManagement.Instance;

            ProductEntity product = ProductManagement.Instance.Get(TestManager.TestProductId);

            KeywordEntity keyword = management.GenerateKeywords(product);

            Assert.IsNotNull(keyword);
            Assert.IsNotNull(keyword.KeywordReferences);
            Assert.IsTrue(keyword.KeywordReferences.Count > 0);

            Assert.IsFalse(string.IsNullOrEmpty(SqlProvider.ThreadInstance.Log));
        }

        [TestMethod]
        public void SearchEntityTest()
        {
            lock (Lock)
            {
                KeywordManagement management = KeywordManagement.Instance;

                ProductEntity product = ProductManagement.Instance.Get(TestManager.TestProductId);
                long productId = product.ProductId;
                long entityTypeId = product.TypeId;

                KeywordCollection keywords = management.SearchIdentifiers(TestManager.TestProductSearch);

                Assert.IsNotNull(keywords);
                Assert.IsNotNull(keywords.Count > 0);

                KeywordEntity entity = keywords.Cast<KeywordEntity>().SingleOrDefault(keyword => (keyword.EntityId == productId) && (keyword.EntityTypeId == entityTypeId));

                Assert.IsNotNull(entity);

                Assert.IsFalse(string.IsNullOrEmpty(SqlProvider.ThreadInstance.Log));
            }
        }

        [TestMethod]
        public void SearchProductTest()
        {
            lock (Lock)
            {
                KeywordManagement management = KeywordManagement.Instance;

                ProductEntity product = ProductManagement.Instance.Get(TestManager.TestProductId);
                long productId = product.ProductId;
                long entityTypeId = product.TypeId;

                KeywordCollection keywords = management.SearchIdentifiers(TestManager.TestProductSearch, entityTypeId);

                Assert.IsNotNull(keywords);
                Assert.IsNotNull(keywords.Count > 0);

                KeywordEntity entity = keywords.Cast<KeywordEntity>().SingleOrDefault(keyword => (keyword.EntityId == productId) && (keyword.EntityTypeId == entityTypeId));

                Assert.IsNotNull(entity);

                Assert.IsFalse(string.IsNullOrEmpty(SqlProvider.ThreadInstance.Log));
            }
        }

        [TestMethod]
        public void GenerateCountriesKeywordsTest()
        {
            lock (Lock)
            {
                CountryCollection countries = CountryManagement.Instance.GetAll();
                foreach (CountryEntity country in countries)
                {
                    KeywordManagement.Instance.Delete(country);

                    KeywordEntity keyword = KeywordManagement.Instance.Find(country);

                    Assert.IsNull(keyword);

                    //Update country and keywords
                    country.Save();

                    keyword = KeywordManagement.Instance.Find(country);

                    Assert.IsNotNull(keyword);
                    Assert.IsNotNull(keyword.KeywordReferences);
                    Assert.IsTrue(keyword.KeywordReferences.Count > 0);

                    KeywordCollection keywords = KeywordManagement.Instance.SearchIdentifiers(country.Name, country.TypeId);
                    KeywordEntity foundedKeyword = keywords.Cast<KeywordEntity>().SingleOrDefault(x => (x.EntityId == country.CountryId) && (x.EntityTypeId == country.TypeId));

                    Assert.IsNotNull(foundedKeyword);
                }
            }
        }

        [TestMethod]
        public void CountrySearchTest()
        {
            lock (Lock)
            {
                CountryEntity russiaCountry = CountryManagement.Russia;
                CountryEntity germanyCountry = CountryManagement.Germany;

                string search = string.Format("{0}, {1}", russiaCountry.Name, germanyCountry.Name);

                CountryCollection countries = CountryManagement.Instance.Search(search);

                Assert.IsNotNull(countries);
                Assert.IsTrue(countries.Count >= 2);

                CountryEntity foundCountry1 = countries.Cast<CountryEntity>().SingleOrDefault(x => (x.CountryId == russiaCountry.CountryId));
                CountryEntity foundCountry2 = countries.Cast<CountryEntity>().SingleOrDefault(x => (x.CountryId == germanyCountry.CountryId));

                Assert.IsNotNull(foundCountry1);
                Assert.IsNotNull(foundCountry2);
            }
        }
    }
}