using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.EntityFramework.DataLayer;
using APV.Pottle.Common;
using APV.Pottle.Common.Application;
using APV.Pottle.Common.Extensions;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;
using APV.Pottle.Toolkit.Linguistics.Interfaces;

namespace APV.Pottle.Core.BusinessLogic
{
    public class WordManagement : BaseManagement<WordEntity, WordCollection, WordDataLayerManager>
    {
        [AnonymousAccess]
        public string[] ParseSentence(string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                return new string[0];
            }

            HashSet<char> chars = LanguageManagement.WordCharsHashset;
            var words = new List<string>();
            int length = sentence.Length;
            var wordBuilder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                char symbol = sentence[i];
                if ((symbol != ' ') && (chars.Contains(symbol)))
                {
                    wordBuilder.Append(symbol);
                }
                else if (wordBuilder.Length > 0)
                {
                    string word = wordBuilder.ToString().ToLowerInvariant();
                    words.Add(word);
                    wordBuilder.Clear();
                }
            }
            if (wordBuilder.Length > 0)
            {
                string word = wordBuilder.ToString().ToLowerInvariant();
                words.Add(word);
            }
            return words.ToArray();
        }

        [AdminAccess]
        public void Clear()
        {
            DatabaseManager.Clear();
        }

        [AdminAccess]
        public override void Delete(WordEntity wordEntity)
        {
            if (wordEntity == null)
                throw new ArgumentNullException("wordEntity");

            //1) delete parent references
            WordReferenceCollection parentReferences = WordReferenceManagement.Instance.GetParentReferences(wordEntity.WordId);
            parentReferences.Delete();

            //2) delete children words
            WordReferenceCollection childrenReferences = WordReferenceManagement.Instance.GetChildReferences(wordEntity.WordId);
            foreach (WordReferenceEntity reference in childrenReferences)
            {
                reference.Delete();
                if (reference.WordId != reference.ReferenceWordId)
                {
                    WordEntity child = reference.Word;
                    if (!child.Lemma)
                    {
                        WordReferenceCollection childParents = WordReferenceManagement.Instance.GetParentReferences(child.WordId);
                        int childParentsCount = childParents.Cast<WordReferenceEntity>().Count(x => (x.ReferenceType != WordReferenceType.Original));
                        if (childParentsCount == 0)
                        {
                            Delete(child);
                        }
                    }
                }
            }

            //3) delete word
            DatabaseManager.Delete(wordEntity);
        }

        [AdminAccess]
        public void Delete(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            name = name.Trim().ToLowerInvariant();

            WordEntity wordEntity = DatabaseManager.FindByName(name);
            if (wordEntity != null)
            {
                Delete(wordEntity);
            }
        }

        [AdminAccess]
        public WordEntity CreateWord(string name, bool lemma, PartsOfSpeech partsOfSpeech)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            name = name.Trim().ToLowerInvariant();

            WordEntity word = DatabaseManager.FindByName(name);
            if (word == null)
            {
                LanguageEntity language = LanguageManagement.Instance.Define(name);
                word = new WordEntity
                    {
                        Language = language,
                        PartsOfSpeech = PartsOfSpeech.Unknown,
                        Lemma = lemma,
                        Name = name,
                    };
                word.Save();
            }
            else
            {
                if ((lemma) && (!word.Lemma))
                {
                    word.Lemma = true;
                }
                UpdatePartsOfSpeech(word);
            }

            if (word.PartsOfSpeech != partsOfSpeech)
            {
                partsOfSpeech |= word.PartsOfSpeech;
                SetPartsOfSpeech(word, partsOfSpeech);
            }

            return word;
        }

        [AdminAccess]
        public WordReferenceEntity CreateReference(string word, WordEntity referenceWord, PartOfSpeech partOfSpeech, WordReferenceType referenceType)
        {
            WordEntity wordEntity = CreateWord(word, false, (PartsOfSpeech)partOfSpeech);
            return CreateReference(wordEntity, referenceWord, partOfSpeech, referenceType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wordEntity">"ели"</param>
        /// <param name="referenceWordEntity">"ель"</param>
        /// <param name="partOfSpeech">"существильное"</param>
        /// <param name="referenceType">"parent"</param>
        /// <returns></returns>
        [AdminAccess]
        public WordReferenceEntity CreateReference(WordEntity wordEntity, WordEntity referenceWordEntity, PartOfSpeech partOfSpeech, WordReferenceType referenceType)
        {
            if (wordEntity == null)
                throw new ArgumentNullException("wordEntity");
            if (referenceWordEntity == null)
                throw new ArgumentNullException("referenceWordEntity");
            if (!referenceWordEntity.PartsOfSpeech.Has(partOfSpeech))
                throw new ArgumentOutOfRangeException("partOfSpeech", string.Format("Reference word \"{0}\" does not have specified part of speech \"{1}\".", referenceWordEntity.Name, partOfSpeech));
            if ((referenceType == WordReferenceType.Original) && (wordEntity.WordId != referenceWordEntity.WordId))
                throw new ArgumentOutOfRangeException("referenceWordEntity", string.Format("\"Original\" reference reference can be created only if word (\"{0}\") and word reference (\"{1}\") are the same.", wordEntity.Name, referenceWordEntity.Name));

            WordReferenceCollection references = WordReferenceManagement.Instance.GetParentReferences(wordEntity.WordId);
            WordReferenceEntity reference = references.Find(referenceWordEntity.WordId, partOfSpeech, referenceType);
            if (reference == null)
            {
                reference = new WordReferenceEntity
                    {
                        Word = wordEntity,
                        ReferenceWord = referenceWordEntity,
                        ReferenceName = referenceWordEntity.Name,
                        LanguageCode = referenceWordEntity.Language.Code,
                        PartOfSpeech = partOfSpeech,
                        ReferenceType = referenceType,
                    };
                reference.Save();

                PartsOfSpeech partsOfSpeech = wordEntity.PartsOfSpeech | (PartsOfSpeech) partOfSpeech;
                SetPartsOfSpeech(wordEntity, partsOfSpeech);
            }
            return reference;
        }

        [AdminAccess]
        public string[] CreateName(string name)
        {
            string[] alternativeNames;
            CreateName(name, out alternativeNames);
            return alternativeNames;
        }

        [AdminAccess]
        public WordEntity CreateName(string name, out string[] alternativeNames)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            name = name.Trim().ToLowerInvariant();
            alternativeNames = ParseName(name);
            if (alternativeNames.Length == 0)
                throw new ArgumentOutOfRangeException("name", string.Format("Invalid format name \"{0}\".", name));

            const PartOfSpeech partOfSpeech = PartOfSpeech.ProperNoun;
            const WordReferenceType referenceType = WordReferenceType.ParentName;

            WordEntity wordEntity = CreateWord(name, true, (PartsOfSpeech)partOfSpeech);
            foreach (string alternativeName in alternativeNames)
            {
                if (name != alternativeName)
                {
                    CreateReference(alternativeName, wordEntity, partOfSpeech, referenceType);
                }
            }
            return wordEntity;
        }

        [AdminAccess]
        public void SetPartsOfSpeech(WordEntity wordEntity, PartsOfSpeech partsOfSpeech)
        {
            if (wordEntity == null)
                throw new ArgumentNullException("wordEntity");

            if (wordEntity.PartsOfSpeech == partsOfSpeech)
            {
                return;
            }

            List<PartOfSpeech> partOfSpeech = partsOfSpeech.ToList();
            WordReferenceCollection references = WordReferenceManagement.Instance.GetOriginalReferences(wordEntity.WordId);

            List<WordReferenceEntity> referencesToDelete = references.Cast<WordReferenceEntity>().Where(item => !partOfSpeech.Contains(item.PartOfSpeech)).ToList();
            List<PartOfSpeech> referencesToCreate = partOfSpeech.Where(item => !references.Contains(item)).ToList();

            foreach (WordReferenceEntity referenceToDelete in referencesToDelete)
            {
                referenceToDelete.Delete();
            }

            foreach (PartOfSpeech referencePartOfSpeech in referencesToCreate)
            {
                var reference = new WordReferenceEntity
                    {
                        Word = wordEntity,
                        ReferenceWord = wordEntity,
                        ReferenceName = wordEntity.Name,
                        LanguageCode = wordEntity.Language.Code,
                        PartOfSpeech = referencePartOfSpeech,
                        ReferenceType = WordReferenceType.Original,
                    };
                reference.Save();
            }

            wordEntity.PartsOfSpeech = partsOfSpeech;
            wordEntity.Save();
        }

        [AdminAccess]
        public void UpdatePartsOfSpeech(WordEntity wordEntity)
        {
            if (wordEntity == null)
                throw new ArgumentNullException("wordEntity");

            WordReferenceCollection references = WordReferenceManagement.Instance.GetParentReferences(wordEntity.WordId);
            PartsOfSpeech partsOfSpeech = references.GetPartsOfSpeech();
            if ((partsOfSpeech != PartsOfSpeech.Unknown) && (wordEntity.PartsOfSpeech != partsOfSpeech))
            {
                wordEntity.PartsOfSpeech = partsOfSpeech;
                wordEntity.Save();
            }
        }

        [AdminAccess]
        public void SaveName(BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            string name = entity.GetName();
            string[] alternativeNames = entity.GetAlternativeNames();

            if (!string.IsNullOrWhiteSpace(name))
            {
                //Create names
                CreateName(name);
            }
            if ((alternativeNames != null) && (alternativeNames.Length > 0))
            {
                //Create alternative names
                foreach (string alternativeName in alternativeNames)
                {
                    CreateName(alternativeName);
                }
            }
            KeywordManagement.Instance.GenerateKeywords(entity);
        }

        [ClientAccess]
        public string[] ParseName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            ITranslitManager translitManager = ApplicationManager.GetTranslitManager();

            var result = new List<string>();
            string[] words = ParseSentence(name);

            if (words.Length == 0)
            {
                return new string[0];
            }

            if (words.Length > 1)
            {
                string longName = string.Join(string.Empty, words);
                result.Add(longName);
                LanguageEntity language = LanguageManagement.Instance.Define(name);
                string[] languagesToTranslit = translitManager.GetTranslits(language.Code);
                foreach (string languageCodeTo in languagesToTranslit)
                {
                    string[] translits = translitManager.Translit(language.Code, languageCodeTo, longName, Constants.TranslitLimit);
                    result.AddRange(translits);
                }
            }

            foreach (string word in words)
            {
                result.Add(word);
                LanguageEntity language = LanguageManagement.Instance.Define(word);
                string[] languagesToTranslit = translitManager.GetTranslits(language.Code);
                foreach (string languageCodeTo in languagesToTranslit)
                {
                    string[] translits = translitManager.Translit(language.Code, languageCodeTo, word, Constants.TranslitLimit);
                    result.AddRange(translits);
                }
            }

            return result.Distinct().ToArray();
        }

        public static readonly WordManagement Instance = (WordManagement)EntityFrameworkManager.GetManagement<WordEntity>();
    }
}