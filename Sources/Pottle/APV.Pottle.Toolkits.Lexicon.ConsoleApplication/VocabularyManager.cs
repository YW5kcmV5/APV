using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APV.Common;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.Entities;
using APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication
{
    public static class VocabularyManager
    {
        private static WordReferenceEntity[] GetOriginal(WordInfo wordInfo, WordEntity wordEntity)
        {
            var result = new List<WordReferenceEntity>();
            PartOfSpeech[] partsOfSpeach = wordInfo.GetPartsOfSpeach();
            foreach (PartOfSpeech partOfSpeech in partsOfSpeach)
            {
                if (partOfSpeech != PartOfSpeech.Unknown)
                {
                    var reference = new WordReferenceEntity
                        {
                            Word = wordEntity,
                            ReferenceWord = wordEntity,
                            ReferenceName = wordEntity.Name,
                            PartOfSpeech = partOfSpeech,
                            ReferenceType = WordReferenceType.Original,
                        };
                    result.Add(reference);
                }
            }
            if (result.Count > 0)
            {
                //Synonyms
                if (wordInfo.Synonyms != null)
                {
                    foreach (WordReference wordReference in wordInfo.Synonyms)
                    {
                        if ((wordReference != null) && (!string.IsNullOrWhiteSpace(wordReference.PartOfSpeech)))
                        {
                            WordEntity synonymWordEntity = WordManagement.Instance.GetByName(wordReference.Name);
                            var reference = new WordReferenceEntity
                                {
                                    Word = wordEntity,
                                    ReferenceWord = synonymWordEntity,
                                    ReferenceName = synonymWordEntity.Name,
                                    PartOfSpeech = wordReference.PartOfSpeech.Transform(),
                                    ReferenceType = WordReferenceType.Synonym,
                                };
                            result.Add(reference);
                        }
                    }
                }
            }
            return result.ToArray();
        }

        private static WordReferenceEntity[] GetParents(VocabularyInfo vocabulary, WordInfo wordInfo, WordEntity wordEntity)
        {
            var result = new List<WordReferenceEntity>();
            if (wordInfo.Parents != null)
            {
                foreach (EntryInfo parent in wordInfo.Parents)
                {
                    if ((parent != null) && (parent.Lemma != null) && (!string.IsNullOrWhiteSpace(parent.Lemma.Name)) && (!string.IsNullOrWhiteSpace(parent.PartOfSpeech)))
                    {
                        WordInfo parentWordInfo = vocabulary.FindWord(parent.Lemma.Name);
                        WordEntity parentWordEntity = WordManagement.Instance.GetByName(parentWordInfo.Name);
                        var reference = new WordReferenceEntity
                            {
                                Word = wordEntity,
                                ReferenceWord = parentWordEntity,
                                ReferenceName = parentWordEntity.Name,
                                PartOfSpeech = parent.PartOfSpeech.Transform(),
                                ReferenceType = WordReferenceType.Parent,
                            };
                        result.Add(reference);

                        //Synonyms
                        if (parentWordInfo.Synonyms != null)
                        {
                            foreach (WordReference wordReference in parentWordInfo.Synonyms)
                            {
                                if ((wordReference != null) && (!string.IsNullOrWhiteSpace(wordReference.PartOfSpeech)))
                                {
                                    WordEntity synonymWordEntity = WordManagement.Instance.GetByName(parentWordInfo.Name);
                                    reference = new WordReferenceEntity
                                        {
                                            Word = wordEntity,
                                            ReferenceWord = synonymWordEntity,
                                            ReferenceName = synonymWordEntity.Name,
                                            PartOfSpeech = wordReference.PartOfSpeech.Transform(),
                                            ReferenceType = WordReferenceType.ParentSynonym,
                                        };
                                    result.Add(reference);
                                }
                            }
                        }
                    }
                }
            }
            return result.ToArray();
        }

        private static List<WordReferenceEntity> Distinct(this IEnumerable<WordReferenceEntity> references)
        {
            var list = new SortedList<string, WordReferenceEntity>();
            foreach (WordReferenceEntity reference in references)
            {
                string hash = string.Format("{0}:{1}:{2}", reference.ReferenceWordId, reference.ReferenceType, reference.PartOfSpeech);
                if (!list.ContainsKey(hash))
                {
                    list.Add(hash, reference);
                }
            }
            return list.Values.ToList();
        }

        public static VocabularyInfo LoadVocabulary(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (!File.Exists(path))
                throw new ArgumentOutOfRangeException("path", string.Format("File can not be found \"{0}\".", path));

            var vocabulary = Serializer.DeserializeFromFile<VocabularyInfo>(path, Serializer.Type.DataContractSerializer);
            return vocabulary;
        }

        public static PartOfSpeech Transform(this string partOfSpeech)
        {
            if (string.IsNullOrEmpty(partOfSpeech))
                throw new ArgumentNullException("partOfSpeech");

            switch (partOfSpeech)
            {
                case "СОЮЗ":
                    return PartOfSpeech.Union;
                case "ПРЕДЛОГ":
                    return PartOfSpeech.Excuse;
                case "ЧАСТИЦА":
                    return PartOfSpeech.Particle;
                case "НАРЕЧИЕ":
                    return PartOfSpeech.Adverb;
                case "ПРИЛАГАТЕЛЬНОЕ":
                    return PartOfSpeech.Adjective;
                case "МЕСТОИМ_СУЩ":
                    return PartOfSpeech.PronounNoun;
                case "МЕСТОИМЕНИЕ":
                    return PartOfSpeech.Pronoun;
                case "ПРИТЯЖ_ЧАСТИЦА":
                    return PartOfSpeech.PossessiveParticle;
                case "ГЛАГОЛ":
                    return PartOfSpeech.Verb;
                case "СУЩЕСТВИТЕЛЬНОЕ":
                    return PartOfSpeech.Noun;
                case "ВВОДНОЕ":
                    return PartOfSpeech.Parenthesis;
                case "ИНФИНИТИВ":
                    return PartOfSpeech.Infinitive;
                case "БЕЗЛИЧ_ГЛАГОЛ":
                    return PartOfSpeech.ImpersonalVerb;
                case "ЧИСЛИТЕЛЬНОЕ":
                    return PartOfSpeech.Numeral;
                case "ДЕЕПРИЧАСТИЕ":
                    return PartOfSpeech.Participle;
                case "ПОСЛЕЛОГ":
                case "ПОСЛЕСЛОГ":
                    return PartOfSpeech.Postposition;
                case "ВОСКЛ_ГЛАГОЛ":
                    return PartOfSpeech.ExclamationVerb;
            }

            throw new ArgumentOutOfRangeException("partOfSpeech", string.Format("Unknown part of speech \"{0}\".", partOfSpeech));
        }

        public static PartsOfSpeech Transform(this IEnumerable<PartOfSpeech> partsOfSpeach)
        {
            if (partsOfSpeach == null)
                throw new ArgumentNullException("partsOfSpeach");

            PartsOfSpeech to = partsOfSpeach.Aggregate(PartsOfSpeech.Unknown, (current, partOfSpeech) => current | (PartsOfSpeech)partOfSpeech);
            return to;
        }

        public static PartOfSpeech[] GetPartsOfSpeach(this WordInfo from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            var partsOfSpeech = new List<string>();
            if (from.Lemma != null)
            {
                partsOfSpeech.Add(from.Lemma.PartOfSpeech);
            }
            if (from.Parents != null)
            {
                partsOfSpeech.AddRange(from.Parents.Where(parent => parent.Lemma != null).Select(parent => parent.PartOfSpeech));
            }
            return partsOfSpeech.Where(partOfSpeech => !string.IsNullOrWhiteSpace(partOfSpeech)).Select(partOfSpeech => partOfSpeech.Transform()).ToArray();
        }

        public static WordEntity Transform(this WordInfo from, LanguageEntity language)
        {
            if (from == null)
                throw new ArgumentNullException("from");
            if (language == null)
                throw new ArgumentNullException("language");

            PartsOfSpeech partsOfSpeech = from.GetPartsOfSpeach().Transform();
            var to = new WordEntity
                {
                    Unknown = false,
                    Lemma = from.IsLemma,
                    Name = from.Name,
                    Language = language,
                    SearchCount = 0,
                    PartsOfSpeech = partsOfSpeech,
                };

            return to;
        }

        public static void ExportReferencesToDb(this VocabularyInfo vocabulary, LanguageEntity languageEntity, WordInfo wordInfo)
        {
            if (wordInfo == null)
                throw new ArgumentNullException("wordInfo");

            WordEntity wordEntity = WordManagement.Instance.GetByName(wordInfo.Name);

            var references = new List<WordReferenceEntity>();
            //Original
            references.AddRange(GetOriginal(wordInfo, wordEntity));
            if (references.Count > 0)
            {
                //Parents
                references.AddRange(GetParents(vocabulary, wordInfo, wordEntity));
            }

            references = references.Distinct();

            //Add to Db
            foreach (WordReferenceEntity reference in references)
            {
                reference.LanguageCode = languageEntity.Code;
                reference.Save();
            }
        }

        public static void ExportReferencesToDb(VocabularyInfo vocabulary)
        {
            if (vocabulary == null)
                throw new ArgumentNullException("vocabulary");

            LanguageEntity languageEntity = LanguageManagement.Instance.FindByCode(vocabulary.Language.Code);

            Console.WriteLine("Exporting. Start.");
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            int length = vocabulary.Words.Length;
            for (int i = 0; i < length; i++)
            {
                WordInfo wordInfo = vocabulary.Words[i];
                vocabulary.ExportReferencesToDb(languageEntity, wordInfo);

                double process = 100.0 * i / length;
                Console.SetCursorPosition(x, y);
                Console.Write("{0:00.00}%", process);
            }
            Console.WriteLine("Exporting. End.");
        }

        public static void ExportToDb(VocabularyInfo vocabulary)
        {
            if (vocabulary == null)
                throw new ArgumentNullException("vocabulary");

            LanguageEntity languageEntity = LanguageManagement.Instance.FindByCode(vocabulary.Language.Code);

            Console.WriteLine("Exporting. Start.");
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            int length = vocabulary.Words.Length;
            for (int i = 0; i < length; i++)
            {
                WordInfo wordInfo = vocabulary.Words[i];
                WordEntity wordEntity = wordInfo.Transform(languageEntity);
                WordManagement.Instance.Save(wordEntity);

                double process = 100.0*i/length;
                Console.SetCursorPosition(x, y);
                Console.Write("{0:00.00}%", process);
            }
            Console.WriteLine("Exporting. End.");
        }
    }
}