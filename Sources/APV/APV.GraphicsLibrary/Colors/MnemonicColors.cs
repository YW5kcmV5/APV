using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using APV.Common;
using APV.GraphicsLibrary.Colors.Palettes;

namespace APV.GraphicsLibrary.Colors
{
    [Serializable]
    [CollectionDataContract(Namespace = SystemConstants.NamespaceData, ItemName = "Color")]
    [DebuggerDisplay("{Count}")]
    public class MnemonicColors : IEnumerable<MnemonicColor>
    {
        #region Private

        [NonSerialized]
        private bool _readonly;
        private readonly List<MnemonicColor> _colors = new List<MnemonicColor>();

        [DataMember(IsRequired = true)]
        private MnemonicColor[] Values
        {
            get { return _colors.ToArray(); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _colors.Clear();
                _colors.AddRange(value);
            }
        }

        #endregion

        public MnemonicColors()
        {
        }

        public MnemonicColors(IEnumerable<MnemonicColor> collection)
        {
            AddRange(collection);
        }

        public void SetReadonly()
        {
            _readonly = true;
        }

        public MnemonicColor FindCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("code");

            return _colors.SingleOrDefault(color => color.IsCode(code));
        }

        public MnemonicColor Find(ColorName colorName)
        {
            if (colorName == null)
                throw new ArgumentNullException("colorName");

            return _colors.SingleOrDefault(color => color.Is(colorName));
        }

        public MnemonicColor Find(string nameOrCode)
        {
            if (nameOrCode == null)
                throw new ArgumentNullException("nameOrCode");

            return _colors.SingleOrDefault(color => color.Is(nameOrCode));
        }

        public MnemonicColor Get(string nameOrCode)
        {
            if (string.IsNullOrEmpty(nameOrCode))
                throw new ArgumentNullException("nameOrCode");

            MnemonicColor color = Find(nameOrCode);

            if (color == null)
                throw new ArgumentOutOfRangeException("nameOrCode", string.Format("Color with name or code \"{0}\" not found.", nameOrCode));

            return color;
        }

        public MnemonicColor Parse(string sentence, out bool onlyColor)
        {
            int maxColorWords = _colors.Max(x => x.MaxWords);

            onlyColor = false;
            if (!string.IsNullOrWhiteSpace(sentence))
            {
                var colorWordsHashSet = new HashSet<string>(new[] { "цвет", "цвета" });
                string lowerSentence = sentence.ToLowerInvariant();
                string[] words = lowerSentence.Split(new[] { " ", "-", "+", ",", ";", ":", "!", "/", "\\"}, StringSplitOptions.RemoveEmptyEntries);
                words = words.Select(word => word.Trim()).Where(word => (!string.IsNullOrEmpty(word))).ToArray();
                int length = words.Length;
                if (length > 0)
                {
                    if (colorWordsHashSet.Contains(words.First()))
                    {
                        words = words.Skip(1).ToArray();
                        length--;
                    }
                    if (length > 0)
                    {
                        if (colorWordsHashSet.Contains(words.Last()))
                        {
                            words = words.Take(words.Length - 1).ToArray();
                            length--;
                        }
                        if (length > 0)
                        {
                            int maxWords = (length >= maxColorWords) ? maxColorWords : length;
                            for (int i = maxWords; i >= 1; i--)
                            {
                                string colorWord = string.Join(" ", words.Take(i));
                                MnemonicColor color = Find(colorWord);
                                if (color != null)
                                {
                                    onlyColor = (i == length);
                                    return color;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public MnemonicColor Parse(string sentence)
        {
            bool onlyColor;
            return Parse(sentence, out onlyColor);
        }

        public bool ContainsCode(string code)
        {
            return (FindCode(code) != null);
        }

        public bool Contains(ColorName colorName)
        {
            return (Find(colorName) != null);
        }

        public bool Contains(string nameOrCode)
        {
            return (Find(nameOrCode) != null);
        }

        public void Add(MnemonicColor color)
        {
            if (color == null)
                throw new ArgumentNullException("color");
            if (_readonly)
                throw new InvalidOperationException("Collection is readonly.");
            if (ContainsCode(color.Code))
                throw new ArgumentOutOfRangeException("color", string.Format("Color with code \"{0}\" (\"{1}\") already exists.", color.Code, color.MnemonicName));
            if (Contains(color.MnemonicName))
                throw new ArgumentOutOfRangeException("color", string.Format("Color with mnemonic name \"{0}\" (\"{1}\") already exists.", color.MnemonicName, color.Code));

            foreach (ColorName colorName in color.Names)
            {
                if (Contains(colorName))
                    throw new ArgumentOutOfRangeException("color", string.Format("Color with name \"{0}\" (\"{1}\", \"{2}\") already exists.", colorName.Value, color.MnemonicName, color.Code));
            }

            _colors.Add(color);
        }

        public void AddRange(IEnumerable<MnemonicColor> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (MnemonicColor mnemonicColor in collection)
            {
                Add(mnemonicColor);
            }
        }

        public IEnumerator<MnemonicColor> GetEnumerator()
        {
            return _colors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _colors.GetEnumerator();
        }

        public MnemonicColor this[string nameOrCode]
        {
            get { return Get(nameOrCode); }
        }

        public int Count
        {
            get { return _colors.Count; }
        }

        public bool Readonly
        {
            get { return _readonly; }
        }

        public static MnemonicColors Deserialize(XmlDocument doc, bool @readonly = false)
        {
            if (doc == null)
                throw new ArgumentNullException("doc");

            var mnemonicColors = Serializer.Deserialize<MnemonicColors>(doc, Serializer.Type.DataContractSerializer);
            if (@readonly)
            {
                mnemonicColors.SetReadonly();
            }
            return mnemonicColors;
        }

        #region Palettes

        public static GlobalPaletteContainer Global
        {
            get { return GlobalPaletteContainer.Instance; }
        }

        public static SpectrumPaletteContainer Spectrum
        {
            get { return SpectrumPaletteContainer.Instance; }
        }

        public static HtmlPaletteContainer Html
        {
            get { return HtmlPaletteContainer.Instance; }
        }

        public static CssPaletteContainer Css
        {
            get { return CssPaletteContainer.Instance; }
        }

        public static SystemColorsPaletteContainer SystemColors
        {
            get { return SystemColorsPaletteContainer.Instance; }
        }

        public static KnownColorsPaletteContainer KnownColors
        {
            get { return KnownColorsPaletteContainer.Instance; }
        }

        #endregion
    }
}