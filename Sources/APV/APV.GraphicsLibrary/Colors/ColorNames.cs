using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using APV.Common;

namespace APV.GraphicsLibrary.Colors
{
    [Serializable]
    [CollectionDataContract(Namespace = SystemConstants.NamespaceData, ItemName = "Name")]
    [DebuggerDisplay("{Count}")]
    public class ColorNames : IEnumerable<ColorName>
    {
        private readonly List<ColorName> _names = new List<ColorName>();
        private int? _wordsCount;

        [DataMember(IsRequired = true, Order = 1)]
        private ColorName[] Values
        {
            get { return _names.ToArray(); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _names.Clear();
                _names.AddRange(value);
            }
        }

        public ColorNames()
        {
        }

        public ColorNames(IEnumerable<ColorName> collection)
        {
            AddRange(collection);
        }

        public void Add(string languageCode, string name, string[] alternativeNames = null)
        {
            var colorName = new ColorName(languageCode, name, alternativeNames);
            Add(colorName);
        }

        public void Add(ColorName colorName)
        {
            if (colorName == null)
                throw new ArgumentNullException("colorName");
            if (ContainsByLanguageCode(colorName.LanguageCode))
                throw new ArgumentOutOfRangeException("colorName", string.Format("Language name for language \"{0}\" already exists.", colorName.LanguageCode));
            if (ContainsByName(colorName.Value))
                throw new ArgumentOutOfRangeException("colorName", string.Format("Language name \"{0}\" already exists.", colorName.Value));

            foreach (string alternativeName in colorName.AlternativeNames)
            {
                if (ContainsByName(alternativeName))
                    throw new ArgumentOutOfRangeException("colorName", string.Format("Language name \"{0}\" already exists.", alternativeName));
            }

            _names.Add(colorName);
            _wordsCount = null;
        }

        public void AddRange(IEnumerable<ColorName> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (ColorName colorName in collection)
            {
                Add(colorName);
            }
        }

        public void AddName(string languageCode, string name)
        {
            if (string.IsNullOrEmpty(languageCode))
                throw new ArgumentNullException("languageCode");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            ColorName colorName = FindByLanguageCode(languageCode);

            if (colorName == null)
            {
                colorName = new ColorName(languageCode, name);
                Add(colorName);
            }
            else
            {
                colorName.AddAlternativeName(name);
                _wordsCount = null;
            }
        }

        public bool ContainsByLanguageCode(string languageCode)
        {
            return (FindByLanguageCode(languageCode) != null);
        }

        public bool ContainsByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException("name", "Color name is empty or white space.");

            return (FindByName(name) != null);
        }

        public bool Contains(ColorName colorName)
        {
            if (_names.Contains(colorName))
            {
                return true;
            }
            if (ContainsByName(colorName.Value))
            {
                return true;
            }
            return colorName.AlternativeNames.Any(ContainsByName);
        }

        public ColorName FindByLanguageCode(string languageCode)
        {
            if (languageCode == null)
                throw new ArgumentNullException("languageCode");
            if (string.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentOutOfRangeException("languageCode", "Language code is empty or white space.");
            if (languageCode.Length != 2)
                throw new ArgumentOutOfRangeException("languageCode", string.Format("Language code length shoul be 2, but \"{0}\".", languageCode.Length));

            return _names.SingleOrDefault(item => item.LanguageCode == languageCode);
        }

        public ColorName FindByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException("name", "Color name is empty or white space.");

            return (_names.SingleOrDefault(item => (item.Contains(name))));
        }

        public ColorName[] ToArray()
        {
            return _names.ToArray();
        }

        public int Count
        {
            get { return _names.Count; }
        }

        public int WordsCount
        {
            get { return (_wordsCount ?? (_wordsCount = _names.Max(name => name.WordsCount))).Value; }
        }

        public string Name
        {
            get
            {
                if (_names.Count > 0)
                {
                    string languageCode = Locallization.LanguageCode;
                    ColorName colorName = FindByLanguageCode(languageCode) ?? _names[0];
                    return colorName.Value;
                }
                return null;
            }
        }

        public string this[string languageCode]
        {
            get
            {
                ColorName colorName = FindByLanguageCode(languageCode);
                return (colorName != null) ? colorName.Value : null;
            }
        }

        #region ICollection

        public bool IsReadOnly
        {
            get { return true; }
        }

        public IEnumerator<ColorName> GetEnumerator()
        {
            return _names.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _names.GetEnumerator();
        }

        #endregion
    }
}