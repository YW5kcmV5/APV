using System.Collections.Generic;
using System.Linq;
using APV.Pottle.Common;
using APV.Pottle.Core.Entities;

namespace APV.Pottle.Core.BusinessLogic.Statistics
{
    public static class StatisticsSearchController
    {
        private class WordStatistics
        {
            public string Word { get; set; }

            public long SearchCount { get; set; }
        }

        private static readonly SortedList<string, WordStatistics> SearchWords = new SortedList<string, WordStatistics>();
        private static readonly object DbLock = new object();

        public static void Search(string[] words)
        {
            if ((words != null) && (words.Length > 0))
            {
                int length = words.Length;
                for (int i = 0; i < length; i++)
                {
                    string word = words[i];
                    if (!string.IsNullOrWhiteSpace(word))
                    {
                        lock (SearchWords)
                        {
                            int index = SearchWords.IndexOfKey(word);
                            WordStatistics statistics;
                            if (index == -1)
                            {
                                statistics = new WordStatistics
                                    {
                                        Word = word,
                                        SearchCount = 0
                                    };
                                SearchWords.Add(word, statistics);
                            }
                            else
                            {
                                statistics = SearchWords.Values[index];
                            }
                            statistics.SearchCount++;
                        }
                    }
                }
            }
        }

        public static void Update()
        {
            WordStatistics[] items;
            lock (SearchWords)
            {
                items = SearchWords.Values.ToArray();
                SearchWords.Clear();
            }
            lock (DbLock)
            {
                int length = items.Length;
                for (int i = 0; i < length; i++)
                {
                    WordStatistics statistics = items[i];
                    string wordName = statistics.Word;
                    WordEntity word = WordManagement.Instance.FindByName(wordName);
                    if (word == null)
                    {
                        LanguageEntity language = LanguageManagement.Instance.Define(wordName);
                        word = new WordEntity
                            {
                                Name = wordName,
                                Unknown = true,
                                PartsOfSpeech = PartsOfSpeech.Unknown,
                                Lemma = false,
                                Language = language,
                            };
                    }
                    word.SearchCount += statistics.SearchCount;
                    word.Save();
                }
            }
        }
    }
}