using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DebuggerDisplay("{Name} (Lemma:{IsLemma})")]
    [DataContract(Name = "Word", Namespace = Constants.DictionaryNamespace)]
    public sealed class WordInfo
    {
        private SortedList<long, WordReference> _synonyms;
        private SortedList<long, EntryInfo> _parents;

        private void Init()
        {
            Synonyms = Synonyms ?? new WordReference[0];
            int length = Synonyms.Length;
            if ((_synonyms == null) || (_synonyms.Count != length))
            {
                _synonyms = new SortedList<long, WordReference>();
                for (int i = 0; i < length; i++)
                {
                    WordReference synonym = Synonyms[i];
                    _synonyms.Add(synonym.ReferenceId, synonym);
                }
            }

            Parents = Parents ?? new EntryInfo[0];
            length = Parents.Length;
            if ((_parents == null) || (_parents.Count != length))
            {
                _parents = new SortedList<long, EntryInfo>();
                for (int i = 0; i < length; i++)
                {
                    EntryInfo parent = Parents[i];
                    _parents.Add(parent.Lemma.ReferenceId, parent);
                }
            }
        }

        [DataMember(IsRequired = true, Order = 0)]
        public long WordId { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Показывает, является ли данной слово отдельной словарной статьей (имя словарной статьи совпадает со словом)
        /// </summary>
        [DataMember(IsRequired = true, Order = 2)]
        public bool IsLemma { get; set; }

        /// <summary>
        /// Если слово является леммой (т.е. от него существуют образованные слова), то содержит список образованныз слов и описание
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 4)]
        public LemmaInfo Lemma { get; set; }

        /// <summary>
        /// Список слов, от которых образовано данное слово (лемм)
        /// </summary>
        [DataMember(IsRequired = true, Order = 5)]
        public EntryInfo[] Parents { get; set; }

        /// <summary>
        /// Синонимы
        /// </summary>
        [DataMember(IsRequired = true, Order = 6)]
        public WordReference[] Synonyms { get; internal set; }

        [DataMember(IsRequired = true, Order = 7)]
        public FrequencyValueInfo[] Frequency { get; set; }

        public WordReference FindSynonym(long wordId)
        {
            Init();
            int index = _synonyms.IndexOfKey(wordId);
            return (index != -1) ? _synonyms.Values[index] : null;
        }

        public EntryInfo FindParent(long wordId)
        {
            Init();
            int index = _parents.IndexOfKey(wordId);
            return (index != -1) ? _parents.Values[index] : null;
        }

        public bool ContainsSynonym(long wordId)
        {
            return (FindSynonym(wordId) != null);
        }

        public bool HasParent(long wordId)
        {
            return (FindParent(wordId) != null);
        }

        public bool AddSynonym(WordInfo word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            long wordId = word.WordId;
            if ((WordId != wordId) && (IsLemma) && (word.IsLemma) && (!ContainsSynonym(wordId)) && (!word.HasParent(wordId)))
            {
                List<WordReference> items = Synonyms.ToList();
                var reference = new WordReference
                    {
                        Name = word.Name,
                        ReferenceId = wordId,
                        PartOfSpeech = word.Lemma.PartOfSpeech
                    };
                items.Add(reference);
                Synonyms = items.ToArray();

                if (!word.ContainsSynonym(WordId))
                {
                    items = word.Synonyms.ToList();
                    reference = new WordReference
                        {
                            Name = Name,
                            ReferenceId = WordId,
                            PartOfSpeech = Lemma.PartOfSpeech
                        };
                    items.Add(reference);
                    word.Synonyms = items.ToArray();
                }

                return true;
            }
            return false;
        }
    }
}