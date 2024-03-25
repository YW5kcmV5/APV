using System.Collections.Generic;
using System.Linq;
using APV.Common;
using APV.Pottle.Common;
using APV.Pottle.Common.Extensions;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.BusinessLogic
{
    [TestClass]
    public class WordManagementTests
    {
        public const string Word0Name = "test_word___word0";
        public const string Word1Name = "test_word___word1";
        public const string Word2Name = "test_word___word2";
        public const string Word3Name = "test_word___word3";
        public const string Word4Name = "test_word___word4";
        public const string Word5Name = "test_word___word5";
        public const string Word6Name = "test_word___word6";
        public const string Word7Name = "test_word___word7";
        public const string Name = "Margot Bis";

        private void DeleteTestWords()
        {
            WordEntity word0 = WordManagement.Instance.FindByName(Word0Name);
            WordEntity word1 = WordManagement.Instance.FindByName(Word1Name);
            WordEntity word2 = WordManagement.Instance.FindByName(Word2Name);
            WordEntity word3 = WordManagement.Instance.FindByName(Word3Name);
            WordEntity word4 = WordManagement.Instance.FindByName(Word4Name);
            WordEntity word5 = WordManagement.Instance.FindByName(Word5Name);
            WordEntity word6 = WordManagement.Instance.FindByName(Word6Name);
            WordEntity word7 = WordManagement.Instance.FindByName(Word7Name);

            if (word0 != null)
            {
                word0.Delete();
            }
            if (word1 != null)
            {
                word1.Delete();
            }
            if (word2 != null)
            {
                word2.Delete();
            }
            if (word3 != null)
            {
                word3.Delete();
            }
            if (word4 != null)
            {
                word4.Delete();
            }
            if (word5 != null)
            {
                word5.Delete();
            }
            if (word6 != null)
            {
                word6.Delete();
            }
            if (word7 != null)
            {
                word7.Delete();
            }

            word0 = WordManagement.Instance.FindByName(Word0Name);
            word1 = WordManagement.Instance.FindByName(Word1Name);
            word2 = WordManagement.Instance.FindByName(Word2Name);
            word3 = WordManagement.Instance.FindByName(Word3Name);
            word4 = WordManagement.Instance.FindByName(Word4Name);
            word5 = WordManagement.Instance.FindByName(Word5Name);
            word6 = WordManagement.Instance.FindByName(Word6Name);
            word7 = WordManagement.Instance.FindByName(Word7Name);

            Assert.IsNull(word0);
            Assert.IsNull(word1);
            Assert.IsNull(word2);
            Assert.IsNull(word3);
            Assert.IsNull(word4);
            Assert.IsNull(word5);
            Assert.IsNull(word6);
            Assert.IsNull(word7);
        }

        private void CreateTestWords()
        {
            const PartOfSpeech noun = PartOfSpeech.Noun;
            const PartOfSpeech verb = PartOfSpeech.Verb;
            const PartOfSpeech numeral = PartOfSpeech.Numeral;
            const PartOfSpeech adverb = PartOfSpeech.Adverb;

            WordEntity word0 = WordManagement.Instance.CreateWord(Word0Name, true, (PartsOfSpeech)noun);
            WordEntity word1 = WordManagement.Instance.CreateWord(Word1Name, false, (PartsOfSpeech)noun);
            WordEntity word2 = WordManagement.Instance.CreateWord(Word2Name, false, (PartsOfSpeech)noun);
            WordEntity word3 = WordManagement.Instance.CreateWord(Word3Name, true, (PartsOfSpeech)numeral);
            WordEntity word4 = WordManagement.Instance.CreateWord(Word4Name, false, (PartsOfSpeech)verb);
            WordEntity word5 = WordManagement.Instance.CreateWord(Word5Name, false, (PartsOfSpeech)verb);
            WordEntity word6 = WordManagement.Instance.CreateWord(Word6Name, false, (PartsOfSpeech)adverb);
            WordEntity word7 = WordManagement.Instance.CreateWord(Word7Name, false, (PartsOfSpeech)numeral);

            Assert.IsNotNull(word0);
            Assert.IsNotNull(word1);
            Assert.IsNotNull(word2);
            Assert.IsNotNull(word3);
            Assert.IsNotNull(word4);
            Assert.IsNotNull(word5);
            Assert.IsNotNull(word6);
            Assert.IsNotNull(word7);

            Assert.AreEqual((PartsOfSpeech)noun, word0.PartsOfSpeech);
            Assert.AreEqual((PartsOfSpeech)noun, word1.PartsOfSpeech);
            Assert.AreEqual((PartsOfSpeech)noun, word2.PartsOfSpeech);
            Assert.IsTrue(word3.PartsOfSpeech.Has(numeral));
            Assert.IsTrue(word4.PartsOfSpeech.Has(verb));
            Assert.IsTrue(word5.PartsOfSpeech.Has(verb));
            Assert.AreEqual((PartsOfSpeech)adverb, word6.PartsOfSpeech);
            Assert.AreEqual((PartsOfSpeech)numeral, word7.PartsOfSpeech);

            WordManagement.Instance.CreateReference(word1, word0, noun, WordReferenceType.Parent);
            WordManagement.Instance.CreateReference(word2, word1, noun, WordReferenceType.Parent);
            WordManagement.Instance.CreateReference(word3, word1, noun, WordReferenceType.Parent);
            WordManagement.Instance.CreateReference(word3, word5, verb, WordReferenceType.Parent);
            WordManagement.Instance.CreateReference(word4, word3, verb, WordReferenceType.Parent);
            WordManagement.Instance.CreateReference(word4, word6, adverb, WordReferenceType.Parent);
            WordManagement.Instance.CreateReference(word7, word3, numeral, WordReferenceType.Parent);

            //circular reference
            WordManagement.Instance.CreateReference(word5, word7, numeral, WordReferenceType.Parent);

            Assert.AreEqual((PartsOfSpeech)numeral | (PartsOfSpeech)verb | (PartsOfSpeech)noun, word3.PartsOfSpeech);
            Assert.AreEqual((PartsOfSpeech)verb | (PartsOfSpeech)adverb, word4.PartsOfSpeech);
            Assert.AreEqual((PartsOfSpeech)verb | (PartsOfSpeech)numeral, word5.PartsOfSpeech);

            //Check children

            WordReferenceCollection children = WordReferenceManagement.Instance.GetChildReferences(word0.WordId);

            Assert.IsNotNull(children);
            Assert.AreEqual(2, children.Count);
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word1.WordId)));

            children = WordReferenceManagement.Instance.GetChildReferences(word1.WordId);

            Assert.IsNotNull(children);
            Assert.AreEqual(3, children.Count);
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word2.WordId)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word3.WordId)));

            children = WordReferenceManagement.Instance.GetChildReferences(word2.WordId);

            Assert.IsNotNull(children);
            Assert.AreEqual(1, children.Count);
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));

            children = WordReferenceManagement.Instance.GetChildReferences(word3.WordId);

            Assert.IsNotNull(children);
            Assert.AreEqual(5, children.Count);
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word4.WordId)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word7.WordId)));

            children = WordReferenceManagement.Instance.GetChildReferences(word4.WordId);

            Assert.IsNotNull(children);
            Assert.AreEqual(2, children.Count);
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));

            children = WordReferenceManagement.Instance.GetChildReferences(word5.WordId);

            Assert.IsNotNull(children);
            Assert.AreEqual(3, children.Count);
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word3.WordId)));

            children = WordReferenceManagement.Instance.GetChildReferences(word6.WordId);

            Assert.IsNotNull(children);
            Assert.AreEqual(2, children.Count);
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word4.WordId)));

            children = WordReferenceManagement.Instance.GetChildReferences(word7.WordId);

            Assert.IsNotNull(children);
            Assert.AreEqual(2, children.Count);
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word5.WordId)));

            //Check parents

            WordReferenceCollection parents = WordReferenceManagement.Instance.GetParentReferences(word0.WordId);

            Assert.IsNotNull(parents);
            Assert.AreEqual(1, parents.Count);
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));

            parents = WordReferenceManagement.Instance.GetParentReferences(word1.WordId);

            Assert.IsNotNull(parents);
            Assert.AreEqual(2, parents.Count);
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word0.WordId)));

            parents = WordReferenceManagement.Instance.GetParentReferences(word2.WordId);

            Assert.IsNotNull(parents);
            Assert.AreEqual(2, parents.Count);
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word1.WordId)));

            parents = WordReferenceManagement.Instance.GetParentReferences(word3.WordId);

            Assert.IsNotNull(parents);
            Assert.AreEqual(5, parents.Count);
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word1.WordId)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word5.WordId)));

            parents = WordReferenceManagement.Instance.GetParentReferences(word4.WordId);

            Assert.IsNotNull(parents);
            Assert.AreEqual(4, parents.Count);
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word3.WordId)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word6.WordId)));

            parents = WordReferenceManagement.Instance.GetParentReferences(word5.WordId);

            Assert.IsNotNull(parents);
            Assert.AreEqual(3, parents.Count);
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word7.WordId)));

            parents = WordReferenceManagement.Instance.GetParentReferences(word6.WordId);

            Assert.IsNotNull(parents);
            Assert.AreEqual(1, parents.Count);
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));

            parents = WordReferenceManagement.Instance.GetParentReferences(word7.WordId);

            Assert.IsNotNull(parents);
            Assert.AreEqual(2, parents.Count);
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Original)));
            Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word3.WordId)));
        }

        private void CreateTestName()
        {
            string[] names;
            WordEntity entity = WordManagement.Instance.CreateName(Name, out names);

            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.WordId > 0);
            Assert.AreEqual(SystemConstants.LanguageCodeEnglish, entity.Language.Code);

            WordReferenceCollection references = WordReferenceManagement.Instance.GetChildReferences(entity.WordId);

            Assert.IsNotNull(references);
            Assert.IsTrue(references.Count > 0);

            Assert.IsTrue(references.Count >= names.Length + 1);

            foreach (WordReferenceEntity reference in references)
            {
                Assert.IsTrue((reference.ReferenceType == WordReferenceType.Original) || (reference.ReferenceType == WordReferenceType.ParentName));
            }

            foreach (string alternativeName in names)
            {
                WordReferenceCollection referenceCollection = references.Where(alternativeName);
                Assert.IsNotNull(referenceCollection);
                Assert.IsTrue(referenceCollection.Count > 0);
            }

            List<string> alternativeNames = references.Cast<WordReferenceEntity>().Where(x => x.ReferenceType == WordReferenceType.ParentName).Select(x => x.Word.Name).ToList();
            foreach (string alternativeName in alternativeNames)
            {
                WordEntity alternativeNameEntity = WordManagement.Instance.FindByName(alternativeName);
                Assert.IsNotNull(alternativeNameEntity);
                Assert.IsTrue(alternativeNameEntity.PartsOfSpeech.Has(PartOfSpeech.ProperNoun));
                Assert.IsFalse(alternativeNameEntity.Lemma);
                Assert.IsTrue(names.Any(x => x == alternativeNameEntity.Name));
                Assert.IsTrue(names.Any(x => x == alternativeNameEntity.Name));
            }
        }

        private void DeteleTestName()
        {
            WordEntity entity = WordManagement.Instance.FindByName(Name);

            Assert.IsNotNull(entity);

            WordReferenceCollection references = WordReferenceManagement.Instance.GetChildReferences(entity.WordId);
            List<string> alternativeNames = references.Cast<WordReferenceEntity>().Select(x => x.Word.Name).ToList();

            entity.Delete();

            entity = WordManagement.Instance.FindByName(Name);
            Assert.IsNull(entity);

            foreach (string alternativeName in alternativeNames)
            {
                WordEntity alternativeNameEntity = WordManagement.Instance.FindByName(alternativeName);
                Assert.IsNull(alternativeNameEntity);
            }
        }

        [TestMethod]
        public void TestReferences()
        {
            WordEntity word = WordManagement.Instance.FindByName("ели");

            Assert.IsNotNull(word);

            WordReferenceCollection parentReferences = WordReferenceManagement.Instance.GetParentReferences(word.WordId);

            Assert.IsNotNull(parentReferences);
            Assert.IsTrue(parentReferences.Count > 0);

            PartsOfSpeech partsOfSpeech = parentReferences.GetPartsOfSpeech();
            Assert.AreEqual(word.PartsOfSpeech, partsOfSpeech);
            Assert.AreEqual(PartsOfSpeech.Noun | PartsOfSpeech.Verb, partsOfSpeech);

            WordEntity[] referenceWords = parentReferences.Cast<WordReferenceEntity>().Select(x => x.ReferenceWord).ToArray();
            
            Assert.IsNotNull(referenceWords);
            Assert.IsTrue(referenceWords.Length > 0);

            Assert.IsTrue(referenceWords.Any(x => x.Name == "ель"));
            Assert.IsTrue(referenceWords.Any(x => x.Name == "есть"));

            WordReferenceCollection childReferences = WordReferenceManagement.Instance.GetChildReferences(word.WordId);

            Assert.IsNotNull(childReferences);
            Assert.IsTrue(childReferences.Count > 0);
            Assert.IsTrue(childReferences.Cast<WordReferenceEntity>().All(x => x.ReferenceType == WordReferenceType.Original));

            referenceWords = childReferences.Cast<WordReferenceEntity>().Select(x => x.ReferenceWord).ToArray();

            Assert.IsNotNull(referenceWords);
            Assert.IsTrue(referenceWords.Length > 0);

            Assert.IsTrue(referenceWords.All(x => x.Name == "ели"));
        }

        [TestMethod]
        public void ParseEnNameTest()
        {
            const string name = "Margot Bis";

            string[] names = WordManagement.Instance.ParseName(name);

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Length > 0);
            Assert.IsTrue(names.Any(x => x == "маргот"));
            Assert.IsTrue(names.Any(x => x == "марготбис"));
            Assert.IsTrue(names.Any(x => x == "марготбиз"));
            Assert.IsTrue(names.Any(x => x == "margot"));
            Assert.IsTrue(names.Any(x => x == "margotbis"));

            Assert.IsFalse(names.Any(x => x == name));
            Assert.IsFalse(names.Any(x => x == "margot bis"));
            Assert.IsFalse(names.Any(x => x == "маргот бис"));
            Assert.IsFalse(names.Any(x => x == "маргот биз"));
        }

        [TestMethod]
        public void ParseLongRusNameTest()
        {
            const string name = "Российская Федерация";

            string[] names = WordManagement.Instance.ParseName(name);

            Assert.IsNotNull(names);
            Assert.IsTrue(names.Length > 0);
            Assert.IsFalse(names.Any(x => x == name));
            Assert.IsTrue(names.Any(x => x == "rossijskayafederacziya"));
            Assert.IsTrue(names.Any(x => x == "rossijskaya"));
            Assert.IsTrue(names.Any(x => x == "federacziya"));
        }

        [TestMethod]
        public void CreateNameTest()
        {
            try
            {
                CreateTestName();
            }
            finally
            {
                DeteleTestName();
            }
        }

        [TestMethod]
        public void SetPartsOfSpeechTest()
        {
            const string word = "test_word_SetPartsOfSpeechTest";

            const PartsOfSpeech pos1 = PartsOfSpeech.Noun | PartsOfSpeech.Verb | PartsOfSpeech.Postposition;
            const PartsOfSpeech pos2 = PartsOfSpeech.Noun | PartsOfSpeech.ProperNoun | PartsOfSpeech.Participle | PartsOfSpeech.Numeral;

            WordEntity wordEntity = WordManagement.Instance.CreateWord(word, true, pos1);

            Assert.IsNotNull(wordEntity);
            Assert.AreEqual(pos1, wordEntity.PartsOfSpeech);

            WordReferenceCollection references = WordReferenceManagement.Instance.GetOriginalReferences(wordEntity.WordId);

            Assert.IsNotNull(references);
            Assert.AreEqual(3, references.Count);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Noun, references[0].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Verb, references[1].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Postposition, references[2].PartOfSpeech);

            WordManagement.Instance.SetPartsOfSpeech(wordEntity, pos2);

            Assert.AreEqual(pos2, wordEntity.PartsOfSpeech);

            references = WordReferenceManagement.Instance.GetOriginalReferences(wordEntity.WordId);

            Assert.IsNotNull(references);
            Assert.AreEqual(4, references.Count);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Noun, references[0].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.ProperNoun, references[1].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Numeral, references[2].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Participle, references[3].PartOfSpeech);

            WordManagement.Instance.Delete(wordEntity);

            wordEntity = WordManagement.Instance.FindByName(word);
            Assert.IsNull(wordEntity);
        }

        [TestMethod]
        public void UpdatePartsOfSpeechTest()
        {
            const string word = "test_word_UpdatePartsOfSpeech";

            const PartsOfSpeech pos = PartsOfSpeech.Noun | PartsOfSpeech.ProperNoun | PartsOfSpeech.Participle | PartsOfSpeech.Numeral;

            WordEntity wordEntity = WordManagement.Instance.CreateWord(word, true, pos);

            Assert.IsNotNull(wordEntity);
            Assert.AreEqual(pos, wordEntity.PartsOfSpeech);

            WordReferenceCollection references = WordReferenceManagement.Instance.GetOriginalReferences(wordEntity.WordId);

            Assert.IsNotNull(references);
            Assert.AreEqual(4, references.Count);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Noun, references[0].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.ProperNoun, references[1].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Numeral, references[2].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Participle, references[3].PartOfSpeech);

            references[1].Delete();
            references[2].Delete();

            references = WordReferenceManagement.Instance.GetOriginalReferences(wordEntity.WordId);

            Assert.IsNotNull(references);
            Assert.AreEqual(2, references.Count);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Noun, references[0].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Participle, references[1].PartOfSpeech);

            WordManagement.Instance.UpdatePartsOfSpeech(wordEntity);

            Assert.AreEqual(PartsOfSpeech.Noun | PartsOfSpeech.Participle, wordEntity.PartsOfSpeech);

            references = WordReferenceManagement.Instance.GetOriginalReferences(wordEntity.WordId);

            Assert.IsNotNull(references);
            Assert.AreEqual(2, references.Count);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Noun, references[0].PartOfSpeech);
            Assert.AreEqual((PartOfSpeech)PartsOfSpeech.Participle, references[1].PartOfSpeech);

            WordManagement.Instance.Delete(wordEntity);

            wordEntity = WordManagement.Instance.FindByName(word);
            Assert.IsNull(wordEntity);
        }

        [TestMethod]
        public void CreateWordTest()
        {
            try
            {
                DeleteTestWords();
                CreateTestWords();
                CreateTestWords();
            }
            finally
            {
                DeleteTestWords();
            }
        }

        [TestMethod]
        public void DeleteWordTest()
        {
            try
            {
                const PartOfSpeech noun = PartOfSpeech.Noun;
                const PartOfSpeech verb = PartOfSpeech.Verb;
                const PartOfSpeech adverb = PartOfSpeech.Adverb;

                CreateTestWords();

                WordEntity word3 = WordManagement.Instance.FindByName(Word3Name);

                Assert.IsNotNull(word3);

                word3.Delete();

                word3 = WordManagement.Instance.FindByName(Word3Name);
                WordEntity word0 = WordManagement.Instance.FindByName(Word0Name);
                WordEntity word1 = WordManagement.Instance.FindByName(Word1Name);
                WordEntity word2 = WordManagement.Instance.FindByName(Word2Name);
                WordEntity word4 = WordManagement.Instance.FindByName(Word4Name);
                WordEntity word5 = WordManagement.Instance.FindByName(Word5Name);
                WordEntity word6 = WordManagement.Instance.FindByName(Word6Name);
                WordEntity word7 = WordManagement.Instance.FindByName(Word7Name);

                Assert.IsNull(word3);
                Assert.IsNull(word7);
                Assert.IsNull(word5);
                Assert.IsNotNull(word0);
                Assert.IsNotNull(word1);
                Assert.IsNotNull(word2);
                Assert.IsNotNull(word4);
                Assert.IsNotNull(word6);

                Assert.AreEqual(PartsOfSpeech.Noun, word0.PartsOfSpeech);
                Assert.AreEqual(PartsOfSpeech.Noun, word1.PartsOfSpeech);
                Assert.AreEqual(PartsOfSpeech.Noun, word2.PartsOfSpeech);
                Assert.AreEqual(PartsOfSpeech.Adverb, word6.PartsOfSpeech);

                Assert.AreEqual(PartsOfSpeech.Verb | PartsOfSpeech.Adverb, word4.PartsOfSpeech);

                //Check children

                WordReferenceCollection children = WordReferenceManagement.Instance.GetChildReferences(word0.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(2, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word1.WordId)));

                children = WordReferenceManagement.Instance.GetChildReferences(word1.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(2, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word2.WordId)));

                children = WordReferenceManagement.Instance.GetChildReferences(word2.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(1, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));

                children = WordReferenceManagement.Instance.GetChildReferences(word4.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(2, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));

                children = WordReferenceManagement.Instance.GetChildReferences(word6.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(2, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word4.WordId)));

                //Check parents

                WordReferenceCollection parents = WordReferenceManagement.Instance.GetParentReferences(word0.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(1, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));

                parents = WordReferenceManagement.Instance.GetParentReferences(word1.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(2, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word0.WordId)));

                parents = WordReferenceManagement.Instance.GetParentReferences(word2.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(2, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word1.WordId)));

                parents = WordReferenceManagement.Instance.GetParentReferences(word4.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(3, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word6.WordId)));

                parents = WordReferenceManagement.Instance.GetParentReferences(word6.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(1, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));
            }
            finally
            {
                DeleteTestWords();
            }
        }

        [TestMethod]
        public void DeleteWordWithLemmaTest()
        {
            try
            {
                const PartOfSpeech noun = PartOfSpeech.Noun;
                const PartOfSpeech verb = PartOfSpeech.Verb;
                const PartOfSpeech numeral = PartOfSpeech.Numeral;
                const PartOfSpeech adverb = PartOfSpeech.Adverb;

                CreateTestWords();

                //mark wod5 as lemma
                WordEntity word5 = WordManagement.Instance.CreateWord(Word5Name, true, (PartsOfSpeech)verb);
                WordEntity word3 = WordManagement.Instance.FindByName(Word3Name);

                Assert.IsNotNull(word3);
                Assert.IsNotNull(word5);
                Assert.IsTrue(word5.Lemma);

                word3.Delete();

                word3 = WordManagement.Instance.FindByName(Word3Name);
                word5 = WordManagement.Instance.FindByName(Word5Name);
                WordEntity word0 = WordManagement.Instance.FindByName(Word0Name);
                WordEntity word1 = WordManagement.Instance.FindByName(Word1Name);
                WordEntity word2 = WordManagement.Instance.FindByName(Word2Name);
                WordEntity word4 = WordManagement.Instance.FindByName(Word4Name);
                WordEntity word6 = WordManagement.Instance.FindByName(Word6Name);
                WordEntity word7 = WordManagement.Instance.FindByName(Word7Name);

                Assert.IsNull(word3);
                Assert.IsNull(word7);
                Assert.IsNotNull(word0);
                Assert.IsNotNull(word1);
                Assert.IsNotNull(word2);
                Assert.IsNotNull(word4);
                Assert.IsNotNull(word5);
                Assert.IsNotNull(word6);

                Assert.AreEqual(PartsOfSpeech.Noun, word0.PartsOfSpeech);
                Assert.AreEqual(PartsOfSpeech.Noun, word1.PartsOfSpeech);
                Assert.AreEqual(PartsOfSpeech.Noun, word2.PartsOfSpeech);
                Assert.AreEqual(PartsOfSpeech.Verb | PartsOfSpeech.Adverb, word4.PartsOfSpeech);
                Assert.AreEqual(PartsOfSpeech.Verb | PartsOfSpeech.Numeral, word5.PartsOfSpeech);
                Assert.AreEqual(PartsOfSpeech.Adverb, word6.PartsOfSpeech);

                //Check children

                WordReferenceCollection children = WordReferenceManagement.Instance.GetChildReferences(word0.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(2, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word1.WordId)));

                children = WordReferenceManagement.Instance.GetChildReferences(word1.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(2, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word2.WordId)));

                children = WordReferenceManagement.Instance.GetChildReferences(word2.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(1, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));

                children = WordReferenceManagement.Instance.GetChildReferences(word4.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(2, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));

                children = WordReferenceManagement.Instance.GetChildReferences(word5.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(2, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Original)));

                children = WordReferenceManagement.Instance.GetChildReferences(word6.WordId);

                Assert.IsNotNull(children);
                Assert.AreEqual(2, children.Count);
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(children.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Parent) && (x.WordId == word4.WordId)));

                //Check parents

                WordReferenceCollection parents = WordReferenceManagement.Instance.GetParentReferences(word0.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(1, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));

                parents = WordReferenceManagement.Instance.GetParentReferences(word1.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(2, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word0.WordId)));

                parents = WordReferenceManagement.Instance.GetParentReferences(word2.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(2, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == noun) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word1.WordId)));

                parents = WordReferenceManagement.Instance.GetParentReferences(word4.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(3, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Parent) && (x.ReferenceWordId == word6.WordId)));

                parents = WordReferenceManagement.Instance.GetParentReferences(word5.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(2, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == verb) && (x.ReferenceType == WordReferenceType.Original)));
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == numeral) && (x.ReferenceType == WordReferenceType.Original)));

                parents = WordReferenceManagement.Instance.GetParentReferences(word6.WordId);

                Assert.IsNotNull(parents);
                Assert.AreEqual(1, parents.Count);
                Assert.IsTrue(parents.Cast<WordReferenceEntity>().Any(x => (x.PartOfSpeech == adverb) && (x.ReferenceType == WordReferenceType.Original)));
            }
            finally
            {
                DeleteTestWords();
            }
        }

        [TestMethod]
        public void SaveCountriesKeywordsTest()
        {
            CountryCollection countries = CountryManagement.Instance.GetAll();
            foreach (CountryEntity country in countries)
            {
                //Update country and keywords
                WordManagement.Instance.SaveName(country);

                KeywordEntity keyword = KeywordManagement.Instance.Find(country);

                Assert.IsNotNull(keyword);
                Assert.IsNotNull(keyword.KeywordReferences);
                Assert.IsTrue(keyword.KeywordReferences.Count > 0);

                KeywordCollection keywords = KeywordManagement.Instance.SearchIdentifiers(country.Name, country.TypeId);
                KeywordEntity foundedKeyword = keywords.Cast<KeywordEntity>().SingleOrDefault(x => (x.EntityId == country.CountryId) && (x.EntityTypeId == country.TypeId));

                Assert.IsNotNull(foundedKeyword);
            }

            CountryEntity russia = CountryManagement.Instance.FindByCode("RU");
            Assert.IsNotNull(russia);

            var russianKeywords = new[] {"Россия", "РФ", "RF", "Rossia", "Федерация", "Российская"};
            foreach (string russianKeyword in russianKeywords)
            {
                KeywordCollection keywords = KeywordManagement.Instance.SearchIdentifiers(russianKeyword);
                Assert.IsNotNull(keywords);
                Assert.IsTrue(keywords.Count > 0);
                Assert.IsTrue(keywords.Cast<KeywordEntity>().Any(x => (x.EntityId == russia.CountryId) && (x.EntityTypeId == russia.TypeId)));
            }
        }
    }
}