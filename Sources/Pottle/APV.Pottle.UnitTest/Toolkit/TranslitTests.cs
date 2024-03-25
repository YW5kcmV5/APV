using System.Linq;
using APV.Pottle.Common;
using APV.Pottle.Toolkit.Linguistics.TranslitManagers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.Toolkit
{
    [TestClass]
    public class TranslitTests
    {
        [TestMethod]
        public void UnknownAllSymbolsTest()
        {
            const string word = "@@@@";

            var manager = new TranslitManager();
            string[] enTransilts = manager.Translit(TranslitManager.LanguageCodeRussian, TranslitManager.LanguageCodeEnglish, word, Constants.TranslitLimit);

            Assert.IsNotNull(enTransilts);
            Assert.AreEqual(0, enTransilts.Length);

            string[] ruTransilts = manager.Translit(TranslitManager.LanguageCodeEnglish, TranslitManager.LanguageCodeRussian, word, Constants.TranslitLimit);

            Assert.IsNotNull(ruTransilts);
            Assert.AreEqual(0, ruTransilts.Length);
        }

        [TestMethod]
        public void UnknownRuSymbolsTest()
        {
            const string word = "Ан@др@ей";

            const string modelWord = "Андрей";

            var manager = new TranslitManager();
            string[] transilts = manager.Translit(TranslitManager.LanguageCodeRussian, TranslitManager.LanguageCodeEnglish, word, Constants.TranslitLimit);

            Assert.IsNotNull(transilts);
            Assert.IsTrue(transilts.Length > 0);
            Assert.IsTrue(transilts.Any(x => x == "Andrey"));
            Assert.IsTrue(transilts.Any(x => x == "Andrei"));

            string[] modelTransilts = manager.Translit(TranslitManager.LanguageCodeRussian, TranslitManager.LanguageCodeEnglish, modelWord, Constants.TranslitLimit);

            Assert.IsNotNull(modelTransilts);
            Assert.AreEqual(modelTransilts.Length, transilts.Length);
            foreach (string modelTranslit in modelTransilts)
            {
                Assert.IsTrue(transilts.Any(x => x == modelTranslit));
            }
        }

        [TestMethod]
        public void UnknownEnSymbolsTest()
        {
            const string word = "An@dr@ey";

            const string modelWord = "Andrey";

            var manager = new TranslitManager();
            string[] transilts = manager.Translit(TranslitManager.LanguageCodeEnglish, TranslitManager.LanguageCodeRussian, word, Constants.TranslitLimit);

            Assert.IsNotNull(transilts);
            Assert.IsTrue(transilts.Length > 0);
            Assert.IsTrue(transilts.Any(x => x == "Андрей"));

            string[] modelTransilts = manager.Translit(TranslitManager.LanguageCodeEnglish, TranslitManager.LanguageCodeRussian, modelWord, Constants.TranslitLimit);

            Assert.IsNotNull(modelTransilts);
            Assert.AreEqual(modelTransilts.Length, transilts.Length);
            foreach (string modelTranslit in modelTransilts)
            {
                Assert.IsTrue(transilts.Any(x => x == modelTranslit));
            }
        }

        [TestMethod]
        public void UnknownRuLastSymbolsTest()
        {
            const string word = "Андре@";

            const string modelWord = "Андре";

            var manager = new TranslitManager();
            string[] transilts = manager.Translit(TranslitManager.LanguageCodeRussian, TranslitManager.LanguageCodeEnglish, word, Constants.TranslitLimit);
            
            Assert.IsNotNull(transilts);
            Assert.IsTrue(transilts.Length > 0);
            Assert.IsTrue(transilts.Any(x => x == "Andre"));

            string[] modelTransilts = manager.Translit(TranslitManager.LanguageCodeRussian, TranslitManager.LanguageCodeEnglish, modelWord, Constants.TranslitLimit);

            Assert.IsNotNull(modelTransilts);
            Assert.AreEqual(modelTransilts.Length, transilts.Length);
            foreach (string modelTranslit in modelTransilts)
            {
                Assert.IsTrue(transilts.Any(x => x == modelTranslit));
            }
        }

        [TestMethod]
        public void UnknownEnLastSymbolsTest()
        {
            const string word = "Andre@";

            const string modelWord = "Andre";

            var manager = new TranslitManager();
            string[] transilts = manager.Translit(TranslitManager.LanguageCodeEnglish, TranslitManager.LanguageCodeRussian, word, Constants.TranslitLimit);

            Assert.IsNotNull(transilts);
            Assert.IsTrue(transilts.Length > 0);
            Assert.IsTrue(transilts.Any(x => x == "Андре"));

            string[] modelTransilts = manager.Translit(TranslitManager.LanguageCodeEnglish, TranslitManager.LanguageCodeRussian, modelWord, Constants.TranslitLimit);

            Assert.IsNotNull(modelTransilts);
            Assert.AreEqual(modelTransilts.Length, transilts.Length);
            foreach (string modelTranslit in modelTransilts)
            {
                Assert.IsTrue(transilts.Any(x => x == modelTranslit));
            }
        }

        [TestMethod]
        public void RuToEnTest()
        {
            var words = new[] { "Андрей", "Маргот", "Биз", "пещера", "Ёрик" };

            var manager = new TranslitManager();
            foreach (string word in words)
            {
                string[] transilts = manager.Translit(TranslitManager.LanguageCodeRussian, TranslitManager.LanguageCodeEnglish, word, Constants.TranslitLimit);
                Assert.IsNotNull(transilts);
                Assert.IsTrue(transilts.Length > 0);
            }
        }

        [TestMethod]
        public void AndreyEnToRuTest()
        {
            var manager = new TranslitManager();

            string[] words = manager.Translit(TranslitManager.LanguageCodeEnglish, TranslitManager.LanguageCodeRussian, "Andrey", Constants.TranslitLimit);
            Assert.IsNotNull(words);
            Assert.IsTrue(words.Length > 0);
            Assert.AreEqual(words.Length, words.Distinct().Count());
            Assert.IsTrue(words.Any(x => x == "Андрей"));

            words = manager.Translit(TranslitManager.LanguageCodeEnglish, TranslitManager.LanguageCodeRussian, "Andrei", Constants.TranslitLimit);
            Assert.IsNotNull(words);
            Assert.IsTrue(words.Length > 0);
            Assert.AreEqual(words.Length, words.Distinct().Count());
            Assert.IsTrue(words.Any(x => x == "Андрей"));
        }

        [TestMethod]
        public void AndreyRuToEnTest()
        {
            var manager = new TranslitManager();

            string[] words = manager.Translit(TranslitManager.LanguageCodeRussian, TranslitManager.LanguageCodeEnglish, "Андрей", Constants.TranslitLimit);
            Assert.IsNotNull(words);
            Assert.IsTrue(words.Length > 0);
            Assert.AreEqual(words.Length, words.Distinct().Count());
            Assert.IsTrue(words.Any(x => x == "Andrey"));
            Assert.IsTrue(words.Any(x => x == "Andrei"));
        }

        [TestMethod]
        public void SpecialRuSymbolsTest()
        {
            var manager = new TranslitManager();

            string[] words = manager.Translit(TranslitManager.LanguageCodeRussian, TranslitManager.LanguageCodeEnglish, "подъездной", 255);
            Assert.IsNotNull(words);
            Assert.IsTrue(words.Length > 0);
            Assert.AreEqual(words.Length, words.Distinct().Count());
            Assert.IsTrue(words.Any(x => x == "pod''ezdnoj"));
            Assert.IsTrue(words.Any(x => x == "pod``ezdnoj"));
            Assert.IsTrue(words.Any(x => x == "podezdnoj"));
        }
    }
}