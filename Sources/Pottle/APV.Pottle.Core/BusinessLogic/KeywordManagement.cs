using System;
using System.Collections.Generic;
using System.Linq;
using APV.EntityFramework;
using APV.EntityFramework.DataLayer;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Common;
using APV.Pottle.Common.Application;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.BusinessLogic.Statistics;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;
using APV.Pottle.Toolkit.Linguistics.Entities;
using APV.Pottle.Toolkit.Linguistics.Interfaces;

namespace APV.Pottle.Core.BusinessLogic
{
    public class KeywordManagement : BaseManagement<KeywordEntity, KeywordCollection, KeywordDataLayerManager>, IKeywordManager
    {
        #region Constants

        private static readonly SortedList<KeywordType, double> KeywordTypeFaktors = new SortedList<KeywordType, double>
            {
                { KeywordType.Name, 1.0 },
                { KeywordType.AlternativeName, 0.9 },
                { KeywordType.Characteristic, 0.8 },
                { KeywordType.Description, 0.7 },
            };

        private static readonly SortedList<WordReferenceType, double> WordReferenceTypeFaktors = new SortedList<WordReferenceType, double>
            {
                {WordReferenceType.Original, 1.0},
                {WordReferenceType.ParentName, 0.9},
                {WordReferenceType.Parent, 0.8},
                {WordReferenceType.Synonym, 0.7},
                {WordReferenceType.ParentSynonym, 0.6},
            };

        private static readonly SortedList<PartOfSpeech, double> PartOfSpeechFaktors = new SortedList<PartOfSpeech, double>
            {
                { PartOfSpeech.Noun, 50.0 },
                { PartOfSpeech.ProperNoun, 40.0 },
                { PartOfSpeech.Adjective, 5.0 },
                { PartOfSpeech.Adverb, 30.0 },
            };

        #endregion

        private static List<KeywordReferenceEntity> Distinct(List<KeywordReferenceEntity> from)
        {
            var result = new SortedList<string, KeywordReferenceEntity>();
            int length = from.Count;
            for (int i = 0; i < length; i++)
            {
                KeywordReferenceEntity reference = from[i];
                string hash = string.Format("{0}:{1}:{2}", reference.KeywordId, reference.Word, reference.Points);
                if (!result.ContainsKey(hash))
                {
                    result.Add(hash, reference);
                }
            }
            return result.Values.ToList();
        }

        private static KeywordReferenceEntity Create(long keywordId, KeywordType keywordType, KeywordReferenceInfo reference)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");

            var points = (long)Math.Round((reference.Points) * KeywordTypeFaktors[keywordType]);
            LanguageEntity language = LanguageManagement.Instance.GetByCode(reference.LanguageCode);
            var to = new KeywordReferenceEntity
                {
                    LanguageId = language.LanguageId,
                    Word = reference.Word,
                    KeywordId = keywordId,
                    Points = points,
                };
            return to;
        }

        #region IKeywordManager

        [AnonymousAccess]
        KeywordReferenceInfo[] IKeywordManager.GenerateKeywords(string sentence)
        {
            string[] words = WordManagement.Instance.ParseSentence(sentence);
            if (words.Length == 0)
            {
                return new KeywordReferenceInfo[0];
            }

            WordReferenceCollection references = WordReferenceManagement.Instance.GetReferences(words);
            var result = new List<KeywordReferenceInfo>();
            foreach (WordReferenceEntity keywordReference in references)
            {
                if (PartOfSpeechFaktors.ContainsKey(keywordReference.PartOfSpeech))
                {
                    double points = PartOfSpeechFaktors[keywordReference.PartOfSpeech];
                    points *= WordReferenceTypeFaktors[keywordReference.ReferenceType];
                    var keywordReferenceInfo = new KeywordReferenceInfo
                        {
                            WordId = keywordReference.WordId,
                            LanguageCode = keywordReference.LanguageCode,
                            Word = keywordReference.ReferenceName,
                            Points = (long) Math.Round(points),
                        };
                    result.Add(keywordReferenceInfo);
                }
            }
            return result.ToArray();
        }

        #endregion

        static KeywordManagement()
        {
            ApplicationManager.Register(Instance);
        }

        [ClientAccess]
        public void GenerateKeywords(IEnumerable<BaseEntity> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (BaseEntity entity in collection)
            {
                GenerateKeywords(entity);
            }
        }

        [ClientAccess]
        public KeywordEntity GenerateKeywords(BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            long entityTypeId = entity.TypeId;
            long entityId = entity.Id;

            IKeywordManager keywordManager = ApplicationManager.GetKeywordManager();

            string name = entity.GetName();
            string[] alternativeNames = entity.GetAlternativeNames();
            string description = entity.GetDescription();

            var nameKeywords = new List<KeywordReferenceInfo>();
            var alternativeNamesKeywords = new List<KeywordReferenceInfo>();
            var descriptionKeywords = new List<KeywordReferenceInfo>();

            if (!string.IsNullOrWhiteSpace(name))
            {
                //Create names
                string[] translitedNames = WordManagement.Instance.ParseName(name);
                string sentence = string.Join(" ", translitedNames);
                KeywordReferenceInfo[] keywords = keywordManager.GenerateKeywords(sentence);
                nameKeywords.AddRange(keywords);
            }
            if ((alternativeNames != null) && (alternativeNames.Length > 0))
            {
                string sentence = "";
                foreach (string alternativeName in alternativeNames)
                {
                    string[] translitedNames = WordManagement.Instance.ParseName(alternativeName);
                    sentence += string.Join(" ", translitedNames);
                }
                KeywordReferenceInfo[] keywords = keywordManager.GenerateKeywords(sentence);
                alternativeNamesKeywords.AddRange(keywords);
            }
            if (!string.IsNullOrWhiteSpace(description))
            {
                KeywordReferenceInfo[] keywords = keywordManager.GenerateKeywords(description);
                descriptionKeywords.AddRange(keywords);
            }

            using (var transaction = new TransactionScope())
            {
                DatabaseManager.Delete(entityTypeId, entityId);
                var keyword = new KeywordEntity
                    {
                        EntityId = entityId,
                        EntityTypeId = entityTypeId,
                    };
                keyword.Save();
                long keywordId = keyword.KeywordId;

                var result = new List<KeywordReferenceEntity>();
                result.AddRange(nameKeywords.Select(reference => Create(keywordId, KeywordType.Name, reference)));
                result.AddRange(alternativeNamesKeywords.Select(reference => Create(keywordId, KeywordType.AlternativeName, reference)));
                result.AddRange(descriptionKeywords.Select(reference => Create(keywordId, KeywordType.Description, reference)));
                result = Distinct(result);

                var resultCollection = new KeywordReferenceCollection(result);
                resultCollection.Save();

                keyword = DatabaseManager.Get(entityTypeId, entityId);
                transaction.Commit();

                return keyword;
            }
        }

        [AnonymousAccess]
        public KeywordCollection SearchIdentifiers(string search, long? entityTypeId = null)
        {
            if (string.IsNullOrEmpty(search))
            {
                return new KeywordCollection();
            }

            string[] words = WordManagement.Instance.ParseSentence(search);
            StatisticsSearchController.Search(words);
            KeywordCollection keywords = (entityTypeId != null)
                                             ? DatabaseManager.Search(words, entityTypeId.Value)
                                             : DatabaseManager.Search(words);
            return keywords;
        }

        [ClientAccess]
        public void Delete(BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            long entityTypeId;
            long entityId;

            var keywordEntity = entity as KeywordEntity;
            if (keywordEntity != null)
            {
                entityTypeId = keywordEntity.EntityTypeId;
                entityId = keywordEntity.EntityId;
            }
            else
            {
                entityTypeId = entity.TypeId;
                entityId = entity.Id;
            }

            DatabaseManager.Delete(entityTypeId, entityId);
        }

        [ClientAccess]
        public void Delete(IEnumerable<BaseEntity> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (BaseEntity entity in collection)
            {
                Delete(entity);
            }
        }

        [ClientAccess]
        public KeywordEntity Find(BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            long entityTypeId = entity.TypeId;
            long entityId = entity.Id;

            return DatabaseManager.Find(entityTypeId, entityId);
        }

        [ClientAccess]
        public KeywordEntity Get(BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            long entityTypeId = entity.TypeId;
            long entityId = entity.Id;

            return DatabaseManager.Get(entityTypeId, entityId);
        }

        public static readonly KeywordManagement Instance = (KeywordManagement)EntityFrameworkManager.GetManagement<KeywordEntity>();
    }
}