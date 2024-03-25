using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.BusinessLogic.Statistics;
using APV.Pottle.Core.Entities;
using APV.Pottle.UnitTest.BusinessLogic.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class StatisticsControllerTests : BaseManagementTests
    {
        [TestMethod]
        public void GenerateKeywordsTest()
        {
            lock (Lock)
            {
                const string search = "Россия";

                WordEntity searchWord = WordManagement.Instance.FindByName(search);

                Assert.IsNotNull(searchWord);

                long originalSearchCount = searchWord.SearchCount;

                KeywordManagement.Instance.SearchIdentifiers(search);

                StatisticsSearchController.Update();

                searchWord = WordManagement.Instance.FindByName(search);

                Assert.IsNotNull(searchWord);
                Assert.IsTrue(searchWord.SearchCount >= originalSearchCount + 1);
            }
        }

        [TestMethod]
        public void NewWordTest()
        {
            lock (Lock)
            {
                const string newWordName = "тестовоеслово";

                WordEntity searchWord = WordManagement.Instance.FindByName(newWordName);

                if (searchWord != null)
                {
                    searchWord.Delete();
                    searchWord = WordManagement.Instance.FindByName(newWordName);
                }

                Assert.IsNull(searchWord);

                KeywordManagement.Instance.SearchIdentifiers(newWordName);

                StatisticsSearchController.Update();

                searchWord = WordManagement.Instance.FindByName(newWordName);

                Assert.IsNotNull(searchWord);
                Assert.IsTrue(searchWord.Unknown);
                Assert.AreEqual(PartsOfSpeech.Unknown, searchWord.PartsOfSpeech);
                Assert.AreEqual(LanguageManagement.Russian.LanguageId, searchWord.LanguageId);
                Assert.AreEqual(1, searchWord.SearchCount);
            }
        }
    }
}