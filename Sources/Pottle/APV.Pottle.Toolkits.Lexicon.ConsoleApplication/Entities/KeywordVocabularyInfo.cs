using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace APV.Pottle.Toolkits.Lexicon.ConsoleApplication.Entities
{
    [DataContract(Name = "KeywordVocabulary", Namespace = Constants.DictionaryNamespace)]
    public sealed class KeywordVocabularyInfo
    {
        private SortedList<string, WordInfo> _names;

        private void Init()
        {
            Words = Words ?? new WordInfo[0];
            if ((_names == null) || (_names.Count != Words.Length))
            {
                _names = new SortedList<string, WordInfo>();

                int length = Words.Length;
                for (int i = 0; i < length; i++)
                {
                    WordInfo word = Words[i];
                    _names.Add(word.Name, word);
                }
            }
        }

        [DataContract(Name = "Keyword", Namespace = Constants.DictionaryNamespace)]
        public sealed class KeywordInfo
        {
            [DataMember(IsRequired = true)]
            public string Name { get; set; }

            [DataMember(IsRequired = true)]
            public string PartOfSpeech { get; set; }

            [DataMember(IsRequired = true)]
            public KeywordType KeywordType { get; set; }
        }

        [DataContract(Name = "Word", Namespace = Constants.DictionaryNamespace)]
        public sealed class WordInfo
        {
            private SortedList<string, KeywordInfo> _keywords;

            private void Init()
            {
                Keywords = Keywords ?? new KeywordInfo[0];
                if ((_keywords == null) || (_keywords.Count != Keywords.Length))
                {
                    _keywords = new SortedList<string, KeywordInfo>();

                    int length = Keywords.Length;
                    for (int i = 0; i < length; i++)
                    {
                        KeywordInfo keyword = Keywords[i];
                        _keywords.Add(keyword.Name, keyword);
                    }
                }
            }

            [DataMember(IsRequired = true)]
            public string Name { get; set; }

            [DataMember(IsRequired = true)]
            public string PartOfSpeech { get; set; }

            public KeywordInfo[] Keywords { get; set; }

            public void AddKeyword(KeywordInfo keyword)
            {
                if (keyword == null)
                    throw new ArgumentNullException("keyword");

                Init();
                if (!_keywords.ContainsKey(keyword.Name))
                {
                    _keywords.Add(keyword.Name, keyword);

                    List<KeywordInfo> keywords = Keywords.ToList();
                    keywords.Add(keyword);
                    Keywords = keywords.ToArray();
                }
            }

            public KeywordInfo FindWord(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");

                Init();
                int index = _keywords.IndexOfKey(name);
                return (index != -1) ? _keywords.Values[index] : null;
            }

            public bool Contains(string name)
            {
                return (FindWord(name) != null);
            }
        }

        [DataMember(IsRequired = true)]
        public WordInfo[] Words { get; set; }

        public void AddWord(WordInfo word)
        {
            if (word == null)
                throw new ArgumentNullException("word");

            Init();
            _names.Add(word.Name, word);

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

        public bool Contains(string name)
        {
            return (FindWord(name) != null);
        }
    }
}