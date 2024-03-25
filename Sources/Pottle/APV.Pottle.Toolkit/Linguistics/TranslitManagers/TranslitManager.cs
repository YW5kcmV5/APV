using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APV.Pottle.Toolkit.Linguistics.Interfaces;

namespace APV.Pottle.Toolkit.Linguistics.TranslitManagers
{
    public class TranslitManager : ITranslitManager
    {
        /// <summary>
        /// "RU"
        /// </summary>
        public const string LanguageCodeRussian = "RU";

        /// <summary>
        /// "EN"
        /// </summary>
        public const string LanguageCodeEnglish = "EN";

        #region EnToRu
        public static readonly SortedList<string, string[]> EnToRu =
            new SortedList<string, string[]>
                {
                    {"a", new[] {"а", "я"}},
                    {"b", new[] {"б"}},
                    {"c", new[] {"ц", "к", "с"}},
                    {"v", new[] {"в"}},
                    {"g", new[] {"г", "ж"}},
                    {"d", new[] {"д"}},
                    {"e", new[] {"е", "ё", "э"}},
                    {"z", new[] {"з", "ж"}},
                    {"i", new[] {"и", "й"}},
                    {"j", new[] {"й", "я"}},
                    {"k", new[] {"к"}},
                    {"l", new[] {"л"}},
                    {"m", new[] {"м"}},
                    {"h", new[] {"х"}},
                    {"n", new[] {"н"}},
                    {"o", new[] {"о"}},
                    {"p", new[] {"п"}},
                    {"r", new[] {"р"}},
                    {"s", new[] {"с", "з", "ш"}},
                    {"t", new[] {"т"}},
                    {"u", new[] {"у", "ю"}},
                    {"f", new[] {"ф"}},
                    {"x", new[] {"х"}},
                    {"y", new[] {"ы", "й", "у"}},
                    {"yo", new[] {"ё"}},
                    {"zh", new[] {"ж"}},
                    {"cz", new[] {"ц"}},
                    {"ch", new[] {"ч", "х"}},
                    {"sh", new[] {"ш", "щ"}},
                    {"shh", new[] {"щ"}},
                    {"sc", new[] {"щ"}},
                    {"shch", new[] {"щ"}},
                    {"yu", new[] {"ю"}},
                    {"ya", new[] {"я"}},
                    {"kh", new[] {"х"}},
                    {"ts", new[] {"ц"}},
                    {"tc", new[] {"ц"}},
                    {"je", new[] {"е", "ё"}},
                    {"ye", new[] {"е", "ё"}},
                    {"yi", new[] {"и"}},
                    {"jj", new[] {"й"}},
                    {"ie", new[] {"ъ"}},
                    {"ui", new[] {"ы"}},
                    {"eh", new[] {"э"}},
                    {"ju", new[] {"ю"}},
                    {"iu", new[] {"ю"}},
                    {"ja", new[] {"я"}},
                    {"''", new[] {"ъ"}},
                    {"``", new[] {"ъ"}},
                    {"'", new[] {"ь"}},
                    {"`", new[] {"ь"}},
                };
        #endregion

        #region RuToEn
        public static readonly SortedList<string, string[]> RuToEn =
            new SortedList<string, string[]>
                {
                    { "а", new[]{"a"} },
                    { "б", new[]{"b"} },
                    { "в", new[]{"v"} },
                    { "г", new[]{"g"} },
                    { "д", new[]{"d"} },
                    { "е", new[]{"e", "je", "ye"} },
                    { "ё", new[]{"yo", "jo", "e", "je", "ye"} },
                    { "ж", new[]{"zh", "z", "j"} },
                    { "з", new[]{"z"} },
                    { "и", new[]{"i", "yi"} },
                    { "й", new[]{"j", "jj", "y", "i", "yi"} },
                    { "к", new[]{"k"} },
                    { "л", new[]{"l"} },
                    { "м", new[]{"m"} },
                    { "н", new[]{"n"} },
                    { "о", new[]{"o"} },
                    { "п", new[]{"p"} },
                    { "р", new[]{"r"} },
                    { "с", new[]{"s"} },
                    { "т", new[]{"t"} },
                    { "у", new[]{"u"} },
                    { "ф", new[]{"f"} },
                    { "х", new[]{"x", "ch", "h", "kh"} },
                    { "ц", new[]{"cz", "c", "ts"} },
                    { "ч", new[]{"ch", "c"} },
                    { "ш", new[]{"sh", "s"} },
                    { "щ", new[]{"shh", "s", "sc", "shch"} },
                    { "ъ", new[]{"", "''", "``"} },
                    { "ы", new[]{"y", "ui"} },
                    { "ь", new[]{"", "'", "`"} },
                    { "э", new[]{"e", "eh"} },
                    { "ю", new[]{"yu", "ju", "u", "iu"} },
                    { "я", new[]{"ya", "a", "ja", "ia"} }
                };
        #endregion

        public static readonly string[] RuImpossibleCombinations = { "бйс", "бйш", "бйз" };

        public static readonly string[] EnImpossibleCombinations = { };

        private string[] GetNextSymbols(string word, int firstIndex, int maxLength, out bool uppercase)
        {
            var lastIndex = firstIndex + maxLength - 1;
            if (lastIndex >= word.Length)
            {
                lastIndex = word.Length - 1;
            }
            uppercase = char.IsUpper(word[firstIndex]);
            int length = (lastIndex - firstIndex + 1);
            var result = new string[length];
            var sb = new StringBuilder();
            for (int index = 0; index < length; index++)
            {
                char symbol = word[firstIndex + index];
                symbol = char.ToLowerInvariant(symbol);
                sb.Append(symbol);
                result[index] = sb.ToString();
            }
            return result;
        }

        private string ToPascalCase(string value)
        {
            return (value.Length > 1)
                       ? value.Substring(0, 1).ToUpperInvariant() + value.Substring(1)
                       : value.ToUpperInvariant();
        }

        private void FillVariants(string word, int firstIndex, int limit, int maxLength, SortedList<string, string[]> variants, string lastResult, List<string> results)
        {
            if (results.Count >= limit)
            {
                return;
            }

            bool uppercase;
            string[] nextSymbols = GetNextSymbols(word, firstIndex, maxLength, out uppercase);
            int foundVariants = 0;
            foreach (string nextSymbol in nextSymbols)
            {
                int index = variants.IndexOfKey(nextSymbol);
                if (index != -1)
                {
                    int symbolLength = nextSymbol.Length;
                    foundVariants++;
                    string[] symbolVariants = variants.Values[index];
                    foreach (var symbolVariant in symbolVariants)
                    {
                        var caseSymbolVariant = (uppercase) ? ToPascalCase(symbolVariant) : symbolVariant;
                        var wordVariant = lastResult + caseSymbolVariant;
                        int length = firstIndex + symbolLength;
                        if (length == word.Length)
                        {
                            //End
                            if (!results.Contains(wordVariant))
                            {
                                if (results.Count >= limit)
                                {
                                    return;
                                }
                                results.Add(wordVariant);
                            }
                        }
                        else
                        {
                            int newFirstIndex = firstIndex + symbolLength;
                            string newLastResult = wordVariant;
                            FillVariants(word, newFirstIndex, limit, maxLength, variants, newLastResult, results);
                        }
                    }
                }
            }
            if (foundVariants == 0)
            {
                int newFirstIndex = firstIndex + 1;
                if (newFirstIndex < word.Length)
                {
                    FillVariants(word, newFirstIndex, limit, maxLength, variants, lastResult, results);
                }
                else
                {
                    //End
                    if ((lastResult.Length > 0) && (!results.Contains(lastResult)))
                    {
                        if (results.Count >= limit)
                        {
                            return;
                        }
                        results.Add(lastResult);
                    }
                }
            }
        }

        private string[] Translit(string word, SortedList<string, string[]> variants, string[] impossibleCombinations, int limit, int maxLength = 4)
        {
            var results = new List<string>();
            FillVariants(word, 0, limit, maxLength, variants, string.Empty, results);
            results = results.Where(item => !impossibleCombinations.Any(item.Contains)).ToList();
            return results.ToArray();
        }

        public string[] Translit(string languageCodeFrom, string languageCodeTo, string word, int limit)
        {
            if (string.IsNullOrEmpty(languageCodeFrom))
                throw new ArgumentNullException("languageCodeFrom");
            if (string.IsNullOrEmpty(languageCodeTo))
                throw new ArgumentNullException("languageCodeTo");
            if (string.IsNullOrEmpty(word))
                throw new ArgumentNullException("word");

            if (languageCodeFrom == languageCodeTo)
            {
                return new[] { word };
            }
            if ((languageCodeFrom == LanguageCodeRussian) && (languageCodeTo == LanguageCodeEnglish))
            {
                return Translit(word, RuToEn, EnImpossibleCombinations, limit);
            }
            if ((languageCodeFrom == LanguageCodeEnglish) && (languageCodeTo == LanguageCodeRussian))
            {
                return Translit(word, EnToRu, RuImpossibleCombinations, limit);
            }

            throw new NotSupportedException(string.Format("Specified transliteration \"{0}\"->\"{1}\" is not supported.", languageCodeFrom, languageCodeTo));
        }

        public string[] GetTranslits(string languageCodeFrom)
        {
            if (string.IsNullOrEmpty(languageCodeFrom))
                throw new ArgumentNullException("languageCodeFrom");

            if (languageCodeFrom == LanguageCodeRussian)
            {
                return new[] { LanguageCodeEnglish };
            }
            if (languageCodeFrom == LanguageCodeEnglish)
            {
                return new[] { LanguageCodeRussian };
            }
            return new string[0];
        }
    }
}