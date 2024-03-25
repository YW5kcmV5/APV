using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using APV.Common;
using APV.Common.Extensions;

namespace APV.GraphicsLibrary.Colors
{
    [Serializable]
    [DataContract(Namespace = SystemConstants.NamespaceData)]
    public sealed class ColorName : IEquatable<ColorName>
    {
        private string _value;
        private string _languageCode;
        private string[] _alternativeNames;
        private int? _hashCode;
        private int? _wordsCount;

        private ColorName()
        {
        }

        private int CalcHashCode()
        {
            return string.Format("{0}:{1}", _languageCode, _value).ToLowerInvariant().GetHashCode();
        }

        public ColorName(string languageCode, string value, string[] alternativeNames = null)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentOutOfRangeException("value", "Value is empty or white space.");
            if (languageCode == null)
                throw new ArgumentNullException("languageCode");
            if (string.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentOutOfRangeException("languageCode", "Language code is empty or white space.");
            if (languageCode.Length != 2)
                throw new ArgumentOutOfRangeException("languageCode", string.Format("Language code length shoul be 2, but \"{0}\".", languageCode.Length));

            _value = value.Trim().ToPascalCase();
            _languageCode = languageCode.ToUpperInvariant();

            _alternativeNames = (alternativeNames ?? new string[0])
                .Where(alternativeName => (!string.IsNullOrWhiteSpace(alternativeName)))
                //.Select(alternativeName => alternativeName.Trim().ToPascalCase())
                .Select(alternativeName => alternativeName.Trim())
                .Where(alternativeName => (string.Compare(alternativeName, _value, StringComparison.InvariantCultureIgnoreCase) != 0))
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .OrderBy(alternativeName => alternativeName)
                .ToArray();
        }

        public bool Contains(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException("name", "Color name is empty or white space.");

            name = name.Trim();

            return
                (string.Compare(_value, name, StringComparison.InvariantCultureIgnoreCase) == 0) ||
                (_alternativeNames.Any(alternativeName => (string.Compare(alternativeName, name, StringComparison.InvariantCultureIgnoreCase) == 0)));
        }

        public void AddAlternativeName(string alternativeName)
        {
            if (alternativeName == null)
                throw new ArgumentNullException("alternativeName");
            if (string.IsNullOrWhiteSpace(alternativeName))
                throw new ArgumentOutOfRangeException("alternativeName", "Color alternative name is empty or white space.");

            if (!Contains(alternativeName))
            {
                List<string> alternativeNames = _alternativeNames.ToList();
                alternativeNames.Add(alternativeName);
                _alternativeNames = alternativeNames.ToArray();
                _wordsCount = null;
            }
        }

        public bool Equals(ColorName other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (_hashCode != other._hashCode)
            {
                return false;
            }
            bool equals =
                (string.Compare(_languageCode, other._languageCode, StringComparison.InvariantCultureIgnoreCase) == 0) &&
                (string.Compare(_value, other._value, StringComparison.InvariantCultureIgnoreCase) == 0);

            return equals;
        }

        public override int GetHashCode()
        {
            return (_hashCode ?? (_hashCode = GetHashCode())).Value;
        }

        [DataMember(IsRequired = true, Order = 0)]
        public string LanguageCode
        {
            get { return _languageCode; }
            private set { _languageCode = value; }
        }

        [DataMember(IsRequired = true, Order = 1)]
        public string Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        [DataMember(IsRequired = true, Order = 2)]
        public string[] AlternativeNames
        {
            get { return _alternativeNames; }
            set { _alternativeNames = value; }
        }

        [IgnoreDataMember]
        public int WordsCount
        {
            get
            {
                if (_wordsCount == null)
                {
                    int valueWordsCount = _value.GetWordsCount();
                    if (_alternativeNames.Length == 0)
                    {
                        return valueWordsCount;
                    }
                    int alternativeNamesWordsCount = _alternativeNames.Max(alternativeName => alternativeName.GetWordsCount());
                    _wordsCount = (valueWordsCount >= alternativeNamesWordsCount) ? valueWordsCount : alternativeNamesWordsCount;
                }
                return _wordsCount.Value;
            }
        }
    }
}