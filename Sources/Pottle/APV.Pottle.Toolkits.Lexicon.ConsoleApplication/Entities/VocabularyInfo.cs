using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "Vocabulary", Namespace = Constants.DictionaryNamespace)]
    public sealed class VocabularyInfo
    {
        private SortedList<string, WordInfo> _names;
        private SortedList<long, WordInfo> _identifiers;

        private void Init()
        {
            Words = Words ?? new WordInfo[0];
            if (_names == null)
            {
                _names = new SortedList<string, WordInfo>();
                _identifiers = new SortedList<long, WordInfo>();

                int length = Words.Length;
                for (int i = 0; i < length; i++)
                {
                    WordInfo word = Words[i];
                    _names.Add(word.Name, word);
                    _identifiers.Add(word.WordId, word);
                }
            }
        }

        [DataMember(IsRequired = true, Order = 0)]
        public long VocabularyId { get; set; }

        [DataMember(IsRequired = true, Order = 1)]
        public LanguageInfo Language { get; set; }

        [DataMember(IsRequired = true, Order = 2)]
        public CoordInfo[] Coords { get; set; }

        [DataMember(IsRequired = true, Order = 3)]
        public string[] PartsOfSpeech { get; set; }

        [DataMember(IsRequired = true, Order = 4)]
        public StatisticsInfo Statistics { get; set; }

        [DataMember(IsRequired = true, Order = 5)]
        public FrequencyInfo[] Frequency { get; set; }

        [DataMember(IsRequired = true, Order = 6)]
        public WordInfo[] Words { get; private set; }

        public void AddWord(WordInfo word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            Init();
            _names.Add(word.Name, word);
            _identifiers.Add(word.WordId, word);

            List<WordInfo> words = Words.ToList();
            words.Add(word);
            Words = words.ToArray();
        }

        public WordInfo FindWord(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Init();
            int index = _names.IndexOfKey(name);
            return (index != -1) ? _names.Values[index] : null;
        }

        public WordInfo FindWord(long wordId)
        {
            Init();
            int index = _identifiers.IndexOfKey(wordId);
            return (index != -1) ? _identifiers.Values[index] : null;
        }

        public bool Contains(string name)
        {
            return (FindWord(name) != null);
        }

        public bool Contains(long wordId)
        {
            return (FindWord(wordId) != null);
        }
    }
}