using System;
using System.Collections.Generic;
using System.Linq;
using APV.Common.Html;
using APV.Pottle.Common;
using APV.Pottle.WebParsers.InfoEntities;
using APV.Pottle.WebParsers.InfoEntities.Collection;

namespace APV.Pottle.WebParsers.Votonia
{
    public static class VotoniaProductOptionParser
    {
        #region Private Types

        private enum OptionType
        {
            Complectation,
            Instruction,
            Warning,
            Age,
            Structure,
            Size,
            PackingSize,
            Color,
            Weight,
            Mode,
        }

        private class ParseInfo
        {
            public string Name;

            public ProductOptionType Type;

            public BaseProductOptionInfo Create(string value)
            {
                if (Type == ProductOptionType.Characteristic)
                {
                    return new ProductCharacteristicInfo(Name) { Value = value };
                }
                return new ProductOptionInfo(Type) { Value = value };
            }

            public bool IsColor
            {
                get { return (Name == ProductCharacteristicInfo.ColorName); }
            }

            public bool IsSize
            {
                get { return (Name == ProductCharacteristicInfo.SizeName); }
            }
        }

        #endregion

        #region Constants

        private static readonly SortedList<OptionType, ParseInfo> OptionsParseInfo = new SortedList<OptionType, ParseInfo>
            {
                {OptionType.Complectation, new ParseInfo { Type = ProductOptionType.Complectation }},
                {OptionType.Instruction, new ParseInfo { Type = ProductOptionType.Instruction }},
                {OptionType.Warning, new ParseInfo { Type = ProductOptionType.Warning }},
                {OptionType.Age, new ParseInfo { Type = ProductOptionType.Characteristic, Name = ProductCharacteristicInfo.AgeName }},
                {OptionType.Structure, new ParseInfo { Type = ProductOptionType.Characteristic, Name = ProductCharacteristicInfo.StructureName }},
                {OptionType.Size, new ParseInfo { Type = ProductOptionType.Characteristic, Name = ProductCharacteristicInfo.SizeName }},
                {OptionType.PackingSize, new ParseInfo { Type = ProductOptionType.Characteristic, Name = ProductCharacteristicInfo.PackingSizeName }},
                {OptionType.Color, new ParseInfo { Type = ProductOptionType.Characteristic, Name = ProductCharacteristicInfo.ColorName }},
                {OptionType.Weight, new ParseInfo { Type = ProductOptionType.Characteristic, Name = ProductCharacteristicInfo.WeightName }},
                {OptionType.Mode, new ParseInfo { Type = ProductOptionType.Characteristic, Name = ProductCharacteristicInfo.ModeName }},
            };

        private static readonly SortedList<string, ParseInfo> Options = new SortedList<string, ParseInfo>
            {
                {"в комплекте", OptionsParseInfo[OptionType.Complectation]},
                {"комплект", OptionsParseInfo[OptionType.Complectation]},
                {"инструкция", OptionsParseInfo[OptionType.Instruction]},
                {"внимание!", OptionsParseInfo[OptionType.Warning]},
                {"возраст", OptionsParseInfo[OptionType.Age]},
                {"рекомендуемый возраст", OptionsParseInfo[OptionType.Age]},
                {"рекомендованный возраст", OptionsParseInfo[OptionType.Age]},
                {"материал", OptionsParseInfo[OptionType.Structure]},
                {"состав", OptionsParseInfo[OptionType.Structure]},
                {"размер", OptionsParseInfo[OptionType.Size]},
                {"размер фигурки", OptionsParseInfo[OptionType.Size]},
                {"размер упаковки", OptionsParseInfo[OptionType.PackingSize]},
                {"цвет", OptionsParseInfo[OptionType.Color]},
                {"вес", OptionsParseInfo[OptionType.Weight]},
                {"вес устройства", OptionsParseInfo[OptionType.Weight]},
                {"возможны три режима", OptionsParseInfo[OptionType.Mode]},
            };

        private const int MaxAdditionalDescriptionLength = 300;
        private const string DescriptionPattern = @"div.description[0] div[1] p[0]";
        private const string AdditionalInfoPattern = @"div.description[0] div[1] p[1]";
        private const string LiPattern = @"div.description[0] div[1] ul li";
        private const string StrongPattern = @"div.description[0] div[1] p strong";

        #endregion

        private static BaseProductOptionInfo DefineColor(string value, out bool hasAdditionalInfo)
        {
            hasAdditionalInfo = true;
            bool onlyColor;
            string color = ColorParser.Parse(value, out onlyColor);

            if (color != null)
            {
                if (onlyColor)
                {
                    hasAdditionalInfo = false;
                }

                ParseInfo colorParseInfo = OptionsParseInfo[OptionType.Color];
                BaseProductOptionInfo colorInfo = colorParseInfo.Create(color);
                return colorInfo;
            }

            return null;
        }

        private static BaseProductOptionInfo Define(string value, out bool hasAdditionalInfo)
        {
            hasAdditionalInfo = false;
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            int index = value.IndexOf(":");
            if (index != -1)
            {
                string name = value.Substring(0, index).Trim().ToLowerInvariant();
                value = value.Substring(index + 1).Trim();
                if ((!string.IsNullOrEmpty(name)) && (!string.IsNullOrEmpty(value)))
                {
                    index = Options.IndexOfKey(name);
                    if (index != -1)
                    {
                        ParseInfo parseInfo = Options.Values[index];
                        BaseProductOptionInfo info = (parseInfo.IsColor) ? DefineColor(value, out hasAdditionalInfo) : parseInfo.Create(value);
                        return info;
                    }
                    hasAdditionalInfo = true;
                    return null;
                }
            }

            foreach (string name in Options.Keys)
            {
                if (value.StartsWith(name + " ", StringComparison.InvariantCultureIgnoreCase))
                {
                    value = value.Substring(name.Length + 1).Trim();
                    ParseInfo parseInfo = Options[name];
                    BaseProductOptionInfo info = (parseInfo.IsColor) ? DefineColor(value, out hasAdditionalInfo) : parseInfo.Create(value);
                    return info;
                }
            }

            hasAdditionalInfo = true;
            return null;
        }

        private static bool CanBeAdditionalInfo(string value)
        {
            return ((!string.IsNullOrWhiteSpace(value)) && (value.Length <= MaxAdditionalDescriptionLength));
        }

        private static string FormatDescription(string description)
        {
            if (description != null)
            {
                if ((description.StartsWith("\"")) && (description.EndsWith("\"")))
                {
                    description = description.Substring(1, description.Length - 2);
                }
                return description.Trim();
            }
            return null;
        }

        private static ProductOptionCollection Parse(string description, string[] items)
        {
            items = items ?? new string[0];

            string additionalInfo = string.Empty;
            var options = new ProductOptionCollection();

            foreach (string value in items)
            {
                bool hasAdditionalInfo;
                BaseProductOptionInfo info = Define(value, out hasAdditionalInfo);
                if (info != null)
                {
                    options.Set(info);
                }

                if ((hasAdditionalInfo) && (CanBeAdditionalInfo(value)))
                {
                    additionalInfo += "\r\n" + FormatDescription(value);
                }
            }

            description = FormatDescription(description);
            additionalInfo = FormatDescription(additionalInfo);

            options.Description = (!string.IsNullOrEmpty(description)) ? description : null;
            options.AdditionalInfo = (!string.IsNullOrEmpty(additionalInfo)) ? additionalInfo : null;

            return options;
        }

        public static ProductOptionCollection Parse(HtmlDocument doc)
        {
            List<HtmlTag> tags = doc.Find(DescriptionPattern);
            List<HtmlTag> liTags = doc.Find(LiPattern);
            List<HtmlTag> strongTags = doc.Find(StrongPattern);
            List<HtmlTag> additionalInfoTags = doc.Find(AdditionalInfoPattern);

            if (tags.Count > 1)
                throw new InvalidOperationException(string.Format("Product description can not be parsed for pattern \"{0}\", more then one tag is found.", DescriptionPattern));

            string description = (tags.Count == 1) ? tags[0].Text : null;
            string[] lis = liTags.Select(tag => tag.Text).Where(text => !string.IsNullOrWhiteSpace(text)).ToArray();
            string[] strongs = strongTags.Select(tag => string.Format("{0} {1}", tag.Text, tag.Parent.Text)).Where(text => !string.IsNullOrWhiteSpace(text)).ToArray();
            string[] addiotnals = additionalInfoTags.Where(tag => (tag.Children.Count == 0) && (!string.IsNullOrWhiteSpace(tag.Text))).Select(tag => tag.Text).ToArray();
            string[] items = lis.Concat(strongs).Concat(addiotnals).ToArray();

            return Parse(description, items);
        }
    }
}