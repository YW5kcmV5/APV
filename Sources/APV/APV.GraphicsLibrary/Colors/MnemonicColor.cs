using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using APV.Common;
using APV.GraphicsLibrary.Colors.Palettes;

namespace APV.GraphicsLibrary.Colors
{
    [DebuggerDisplay("{MnemonicName} - {Code} ({RGB.R}, {RGB.G}, {RGB.B})")]
    [Serializable]
    [DataContract(Namespace = SystemConstants.NamespaceData)]
    public class MnemonicColor
    {
        private const string MenmonicNameSymbols = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz_01234567890";
        private static readonly HashSet<char> MenmonicNameSymbolsHashSet = new HashSet<char>(MenmonicNameSymbols);

        private string _mnemonicName;
        private RGB _rgb;
        private ColorNames _colorNames;

        private bool IsValidMnemonicName(string mnemonicName)
        {
            bool correct = mnemonicName.All(symbol => MenmonicNameSymbolsHashSet.Contains(symbol));
            correct &= (!char.IsNumber(mnemonicName[0]));
            return correct;
        }

        /// <summary>
        /// "Unknown"
        /// </summary>
        public const string UnknownColorName = "Unknown";

        public MnemonicColor(string mnemonicName, byte r, byte g, byte b, ColorNames colorNames = null)
        {
            if (!IsValidMnemonicName(mnemonicName))
                throw new ArgumentOutOfRangeException("mnemonicName", "Only latin symbols, underscore and numbers is acceptable (first symbol can not be a number).");

            _mnemonicName = mnemonicName;

            _rgb = new RGB(r, g, b);
            _colorNames = colorNames ?? new ColorNames();
        }

        public MnemonicColor(string mnemonicName, string code, ColorNames colorNames = null)
        {
            if (code == null)
                throw new ArgumentNullException("code");
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentOutOfRangeException("code", "Color mnemonic name is empty or white space.");
            if (mnemonicName == null)
                throw new ArgumentNullException("mnemonicName");
            if (string.IsNullOrWhiteSpace(mnemonicName))
                throw new ArgumentOutOfRangeException("mnemonicName", "Color name is empty or white space.");
            if (!IsValidMnemonicName(mnemonicName))
                throw new ArgumentOutOfRangeException("mnemonicName", "Only latin symbols, underscore and numbers is acceptable (first symbol can not be a number).");

            _mnemonicName = mnemonicName;

            _rgb = new RGB(code);
            _colorNames = colorNames ?? new ColorNames();
        }

        public bool IsCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("code");

            code = code.Trim();
            return (string.Compare(_rgb.Code, code, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public bool Is(string nameOrCode)
        {
            if (string.IsNullOrEmpty(nameOrCode))
                throw new ArgumentNullException("nameOrCode");

            return (string.Compare(nameOrCode, _mnemonicName, StringComparison.InvariantCultureIgnoreCase) == 0) ||
                   (_colorNames.ContainsByName(nameOrCode)) ||
                   IsCode(nameOrCode);
        }

        public bool Is(ColorName colorName)
        {
            return (_colorNames.Contains(colorName));
        }

        public string[] FindAlternativeNames(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
                throw new ArgumentNullException("languageCode");

            ColorName name = _colorNames.FindByLanguageCode(languageCode);
            return (name != null) ? name.AlternativeNames : null;
        }

        [DataMember(IsRequired = true, Order = 0)]
        public string MnemonicName
        {
            get { return _mnemonicName; }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException("value", "Color mnemonic name is empty or white space.");
                if (!IsValidMnemonicName(value))
                    throw new ArgumentOutOfRangeException("value", "Only latin symbols, underscore and numbers is acceptable (first symbol can not be a number).");

                _mnemonicName = value;
            }
        }

        [DataMember(IsRequired = true, Order = 1)]
        public string Code
        {
            get { return _rgb.Code; }
            private set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentOutOfRangeException("value", "Color mnemonic name is empty or white space.");

                _rgb = new RGB(value);
            }
        }

        [DataMember(IsRequired = true, Order = 2)]
        public ColorNames Names
        {
            get { return _colorNames; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _colorNames = value;
            }
        }

        [IgnoreDataMember]
        public RGB RGB
        {
            get { return _rgb; }
        }

        [IgnoreDataMember]
        public bool Unknown
        {
            get { return (_colorNames.Count == 0); }
        }

        [IgnoreDataMember]
        public string Name
        {
            get { return _colorNames.Name ?? Code; }
        }

        [IgnoreDataMember]
        public string NameEn
        {
            get { return Names[SystemConstants.LanguageCodeEnglish]; }
        }

        [IgnoreDataMember]
        public string NameRu
        {
            get { return Names[SystemConstants.LanguageCodeRussian]; }
        }

        [IgnoreDataMember]
        public int MaxWords
        {
            get { return _colorNames.WordsCount; }
        }

        #region Global

        #region Colors

        public static MnemonicColor Red
        {
            get { return GlobalPalette.Red; }
        }

        public static MnemonicColor Orange
        {
            get { return GlobalPalette.Orange; }
        }

        public static MnemonicColor Yellow
        {
            get { return GlobalPalette.Yellow; }
        }

        public static MnemonicColor Green
        {
            get { return GlobalPalette.Green; }
        }

        public static MnemonicColor GreenHtml
        {
            get { return GlobalPalette.GreenHtml; }
        }

        public static MnemonicColor Cyan
        {
            get { return GlobalPalette.Cyan; }
        }

        public static MnemonicColor Blue
        {
            get { return GlobalPalette.Blue; }
        }

        public static MnemonicColor Violet
        {
            get { return GlobalPalette.Violet; }
        }

        public static MnemonicColor SkyBlue
        {
            get { return GlobalPalette.SkyBlue; }
        }

        public static MnemonicColor DarkBlue
        {
            get { return GlobalPalette.DarkBlue; }
        }

        public static MnemonicColor Navy
        {
            get { return GlobalPalette.Navy; }
        }

        public static MnemonicColor AirForceBlue
        {
            get { return GlobalPalette.AirForceBlue; }
        }

        public static MnemonicColor AliceBlue
        {
            get { return GlobalPalette.AliceBlue; }
        }

        public static MnemonicColor Azure
        {
            get { return GlobalPalette.Azure; }
        }

        public static MnemonicColor Ultramarine
        {
            get { return GlobalPalette.Ultramarine; }
        }

        public static MnemonicColor DarkSeaGreen
        {
            get { return GlobalPalette.DarkSeaGreen; }
        }

        public static MnemonicColor DarkViolet
        {
            get { return GlobalPalette.DarkViolet; }
        }

        public static MnemonicColor DeepViolet
        {
            get { return GlobalPalette.DeepViolet; }
        }

        public static MnemonicColor Chestnut
        {
            get { return GlobalPalette.Chestnut; }
        }

        public static MnemonicColor Aubergine
        {
            get { return GlobalPalette.Aubergine; }
        }

        public static MnemonicColor Eggplant
        {
            get { return GlobalPalette.Eggplant; }
        }

        public static MnemonicColor Khaki
        {
            get { return GlobalPalette.Khaki; }
        }

        public static MnemonicColor LightKhaki
        {
            get { return GlobalPalette.LightKhaki; }
        }

        public static MnemonicColor DarkKhaki
        {
            get { return GlobalPalette.DarkKhaki; }
        }

        public static MnemonicColor GrayKhaki
        {
            get { return GlobalPalette.GrayKhaki; }
        }

        public static MnemonicColor LemonCream
        {
            get { return GlobalPalette.LemonCream; }
        }

        public static MnemonicColor Lime
        {
            get { return GlobalPalette.Lime; }
        }

        public static MnemonicColor LemonLime
        {
            get { return GlobalPalette.LemonLime; }
        }

        public static MnemonicColor ArcticLime
        {
            get { return GlobalPalette.ArcticLime; }
        }

        public static MnemonicColor Volt
        {
            get { return GlobalPalette.Volt; }
        }

        public static MnemonicColor ElectricLime
        {
            get { return GlobalPalette.ElectricLime; }
        }

        public static MnemonicColor LimeGreen
        {
            get { return GlobalPalette.LimeGreen; }
        }

        public static MnemonicColor RedViolet
        {
            get { return GlobalPalette.RedViolet; }
        }

        public static MnemonicColor PaleRedViolet
        {
            get { return GlobalPalette.PaleRedViolet; }
        }

        public static MnemonicColor MidnightBlue
        {
            get { return GlobalPalette.MidnightBlue; }
        }

        public static MnemonicColor PaleBlue
        {
            get { return GlobalPalette.PaleBlue; }
        }

        public static MnemonicColor DarkPeach
        {
            get { return GlobalPalette.DarkPeach; }
        }

        public static MnemonicColor Plum
        {
            get { return GlobalPalette.Plum; }
        }

        public static MnemonicColor LightPlum
        {
            get { return GlobalPalette.LightPlum; }
        }

        public static MnemonicColor CrayolaPlum
        {
            get { return GlobalPalette.CrayolaPlum; }
        }

        public static MnemonicColor PowderBlue
        {
            get { return GlobalPalette.PowderBlue; }
        }

        public static MnemonicColor Purple
        {
            get { return GlobalPalette.Purple; }
        }

        public static MnemonicColor RebeccaPurple
        {
            get { return GlobalPalette.RebeccaPurple; }
        }

        public static MnemonicColor Turquoise
        {
            get { return GlobalPalette.Turquoise; }
        }

        public static MnemonicColor BrightTurquoise
        {
            get { return GlobalPalette.BrightTurquoise; }
        }

        public static MnemonicColor Fuchsia
        {
            get { return GlobalPalette.Fuchsia; }
        }

        public static MnemonicColor FashionFuchsia
        {
            get { return GlobalPalette.FashionFuchsia; }
        }

        public static MnemonicColor FuchsiaRose
        {
            get { return GlobalPalette.FuchsiaRose; }
        }

        public static MnemonicColor FuchsiaCrayola
        {
            get { return GlobalPalette.FuchsiaCrayola; }
        }

        public static MnemonicColor Fandango
        {
            get { return GlobalPalette.Fandango; }
        }

        public static MnemonicColor AntiqueFuchsia
        {
            get { return GlobalPalette.AntiqueFuchsia; }
        }

        public static MnemonicColor Redberry
        {
            get { return GlobalPalette.Redberry; }
        }

        public static MnemonicColor UnitedNationsBlue
        {
            get { return GlobalPalette.UnitedNationsBlue; }
        }

        public static MnemonicColor Cerulean
        {
            get { return GlobalPalette.Cerulean; }
        }

        public static MnemonicColor DarkCerulean
        {
            get { return GlobalPalette.DarkCerulean; }
        }

        public static MnemonicColor CeruleanBlue
        {
            get { return GlobalPalette.CeruleanBlue; }
        }

        public static MnemonicColor CeruleanCrayola
        {
            get { return GlobalPalette.CeruleanCrayola; }
        }

        public static MnemonicColor CeruleanPantone
        {
            get { return GlobalPalette.CeruleanPantone; }
        }

        public static MnemonicColor CeruleanFrost
        {
            get { return GlobalPalette.CeruleanFrost; }
        }

        public static MnemonicColor Black
        {
            get { return GlobalPalette.Black; }
        }

        public static MnemonicColor MediumBlue
        {
            get { return GlobalPalette.MediumBlue; }
        }

        public static MnemonicColor Zaffre
        {
            get { return GlobalPalette.Zaffre; }
        }

        public static MnemonicColor PrussianBlue
        {
            get { return GlobalPalette.PrussianBlue; }
        }

        public static MnemonicColor Cobalt
        {
            get { return GlobalPalette.Cobalt; }
        }

        public static MnemonicColor DarkGreen
        {
            get { return GlobalPalette.DarkGreen; }
        }

        public static MnemonicColor GradusBlue
        {
            get { return GlobalPalette.GradusBlue; }
        }

        public static MnemonicColor Teal
        {
            get { return GlobalPalette.Teal; }
        }

        public static MnemonicColor DarkCyan
        {
            get { return GlobalPalette.DarkCyan; }
        }

        public static MnemonicColor BondiBlue
        {
            get { return GlobalPalette.BondiBlue; }
        }

        public static MnemonicColor Jade
        {
            get { return GlobalPalette.Jade; }
        }

        public static MnemonicColor DeepSkyBlue
        {
            get { return GlobalPalette.DeepSkyBlue; }
        }

        public static MnemonicColor RobinEggBlue
        {
            get { return GlobalPalette.RobinEggBlue; }
        }

        public static MnemonicColor DarkTurquoise
        {
            get { return GlobalPalette.DarkTurquoise; }
        }

        public static MnemonicColor MediumSpringGreen
        {
            get { return GlobalPalette.MediumSpringGreen; }
        }

        public static MnemonicColor SpringGreen
        {
            get { return GlobalPalette.SpringGreen; }
        }

        public static MnemonicColor PineGreen
        {
            get { return GlobalPalette.PineGreen; }
        }

        public static MnemonicColor DarkPastelGreen
        {
            get { return GlobalPalette.DarkPastelGreen; }
        }

        public static MnemonicColor Sapphire
        {
            get { return GlobalPalette.Sapphire; }
        }

        public static MnemonicColor ActiveCaption
        {
            get { return GlobalPalette.ActiveCaption; }
        }

        public static MnemonicColor Malachite
        {
            get { return GlobalPalette.Malachite; }
        }

        public static MnemonicColor Denim
        {
            get { return GlobalPalette.Denim; }
        }

        public static MnemonicColor DarkSpringGreen
        {
            get { return GlobalPalette.DarkSpringGreen; }
        }

        public static MnemonicColor DodgerBlue
        {
            get { return GlobalPalette.DodgerBlue; }
        }

        public static MnemonicColor LightSeaGreen
        {
            get { return GlobalPalette.LightSeaGreen; }
        }

        public static MnemonicColor ForestGreen
        {
            get { return GlobalPalette.ForestGreen; }
        }

        public static MnemonicColor SeaGreen
        {
            get { return GlobalPalette.SeaGreen; }
        }

        public static MnemonicColor DarkSlateGray
        {
            get { return GlobalPalette.DarkSlateGray; }
        }

        public static MnemonicColor DarkIndigo
        {
            get { return GlobalPalette.DarkIndigo; }
        }

        public static MnemonicColor Desktop
        {
            get { return GlobalPalette.Desktop; }
        }

        public static MnemonicColor KleinBlue
        {
            get { return GlobalPalette.KleinBlue; }
        }

        public static MnemonicColor Arsenic
        {
            get { return GlobalPalette.Arsenic; }
        }

        public static MnemonicColor MediumSeaGreen
        {
            get { return GlobalPalette.MediumSeaGreen; }
        }

        public static MnemonicColor Bistre
        {
            get { return GlobalPalette.Bistre; }
        }

        public static MnemonicColor ControlDarkDark
        {
            get { return GlobalPalette.ControlDarkDark; }
        }

        public static MnemonicColor Viridian
        {
            get { return GlobalPalette.Viridian; }
        }

        public static MnemonicColor RoyalBlue
        {
            get { return GlobalPalette.RoyalBlue; }
        }

        public static MnemonicColor GrayAsparagus
        {
            get { return GlobalPalette.GrayAsparagus; }
        }

        public static MnemonicColor SteelBlue
        {
            get { return GlobalPalette.SteelBlue; }
        }

        public static MnemonicColor DarkSlateBlue
        {
            get { return GlobalPalette.DarkSlateBlue; }
        }

        public static MnemonicColor MediumTurquoise
        {
            get { return GlobalPalette.MediumTurquoise; }
        }

        public static MnemonicColor Indigo
        {
            get { return GlobalPalette.Indigo; }
        }

        public static MnemonicColor ArmyGreen
        {
            get { return GlobalPalette.ArmyGreen; }
        }

        public static MnemonicColor FernGreen
        {
            get { return GlobalPalette.FernGreen; }
        }

        public static MnemonicColor Emerald
        {
            get { return GlobalPalette.Emerald; }
        }

        public static MnemonicColor DarkOlive
        {
            get { return GlobalPalette.DarkOlive; }
        }

        public static MnemonicColor DarkOliveGreen
        {
            get { return GlobalPalette.DarkOliveGreen; }
        }

        public static MnemonicColor DarkScarlet
        {
            get { return GlobalPalette.DarkScarlet; }
        }

        public static MnemonicColor CadetBlue
        {
            get { return GlobalPalette.CadetBlue; }
        }

        public static MnemonicColor CornflowerBlue
        {
            get { return GlobalPalette.CornflowerBlue; }
        }

        public static MnemonicColor DarkBrown
        {
            get { return GlobalPalette.DarkBrown; }
        }

        public static MnemonicColor PersianBlue
        {
            get { return GlobalPalette.PersianBlue; }
        }

        public static MnemonicColor MediumAquamarine
        {
            get { return GlobalPalette.MediumAquamarine; }
        }

        public static MnemonicColor BrightGreen
        {
            get { return GlobalPalette.BrightGreen; }
        }

        public static MnemonicColor DimGray
        {
            get { return GlobalPalette.DimGray; }
        }

        public static MnemonicColor SlateBlue
        {
            get { return GlobalPalette.SlateBlue; }
        }

        public static MnemonicColor OliveDrab
        {
            get { return GlobalPalette.OliveDrab; }
        }

        public static MnemonicColor Sepia
        {
            get { return GlobalPalette.Sepia; }
        }

        public static MnemonicColor SlateGray
        {
            get { return GlobalPalette.SlateGray; }
        }

        public static MnemonicColor Wine
        {
            get { return GlobalPalette.Wine; }
        }

        public static MnemonicColor RawUmber
        {
            get { return GlobalPalette.RawUmber; }
        }

        public static MnemonicColor Xanadu
        {
            get { return GlobalPalette.Xanadu; }
        }

        public static MnemonicColor Russet
        {
            get { return GlobalPalette.Russet; }
        }

        public static MnemonicColor LightSlateGray
        {
            get { return GlobalPalette.LightSlateGray; }
        }

        public static MnemonicColor PastelGreen
        {
            get { return GlobalPalette.PastelGreen; }
        }

        public static MnemonicColor Cinnamon
        {
            get { return GlobalPalette.Cinnamon; }
        }

        public static MnemonicColor MediumSlateBlue
        {
            get { return GlobalPalette.MediumSlateBlue; }
        }

        public static MnemonicColor Asparagus
        {
            get { return GlobalPalette.Asparagus; }
        }

        public static MnemonicColor LawnGreen
        {
            get { return GlobalPalette.LawnGreen; }
        }

        public static MnemonicColor Chartreuse
        {
            get { return GlobalPalette.Chartreuse; }
        }

        public static MnemonicColor Aquamarine
        {
            get { return GlobalPalette.Aquamarine; }
        }

        public static MnemonicColor Maroon
        {
            get { return GlobalPalette.Maroon; }
        }

        public static MnemonicColor Olive
        {
            get { return GlobalPalette.Olive; }
        }

        public static MnemonicColor Gray
        {
            get { return GlobalPalette.Gray; }
        }

        public static MnemonicColor BattleshipGrey
        {
            get { return GlobalPalette.BattleshipGrey; }
        }

        public static MnemonicColor LightSkyBlue
        {
            get { return GlobalPalette.LightSkyBlue; }
        }

        public static MnemonicColor BlueViolet
        {
            get { return GlobalPalette.BlueViolet; }
        }

        public static MnemonicColor BurntUmber
        {
            get { return GlobalPalette.BurntUmber; }
        }

        public static MnemonicColor DarkRed
        {
            get { return GlobalPalette.DarkRed; }
        }

        public static MnemonicColor DarkMagenta
        {
            get { return GlobalPalette.DarkMagenta; }
        }

        public static MnemonicColor SaddleBrown
        {
            get { return GlobalPalette.SaddleBrown; }
        }

        public static MnemonicColor AppleGreen
        {
            get { return GlobalPalette.AppleGreen; }
        }

        public static MnemonicColor Burgundy
        {
            get { return GlobalPalette.Burgundy; }
        }

        public static MnemonicColor LightGreen
        {
            get { return GlobalPalette.LightGreen; }
        }

        public static MnemonicColor DarkTan
        {
            get { return GlobalPalette.DarkTan; }
        }

        public static MnemonicColor Sangria
        {
            get { return GlobalPalette.Sangria; }
        }

        public static MnemonicColor MediumPurple
        {
            get { return GlobalPalette.MediumPurple; }
        }

        public static MnemonicColor Carmine
        {
            get { return GlobalPalette.Carmine; }
        }

        public static MnemonicColor Brown
        {
            get { return GlobalPalette.Brown; }
        }

        public static MnemonicColor DarkChestnut
        {
            get { return GlobalPalette.DarkChestnut; }
        }

        public static MnemonicColor PaleBrown
        {
            get { return GlobalPalette.PaleBrown; }
        }

        public static MnemonicColor PaleGreen
        {
            get { return GlobalPalette.PaleGreen; }
        }

        public static MnemonicColor MintGreen
        {
            get { return GlobalPalette.MintGreen; }
        }

        public static MnemonicColor VioletEggplant
        {
            get { return GlobalPalette.VioletEggplant; }
        }

        public static MnemonicColor DarkOrchid
        {
            get { return GlobalPalette.DarkOrchid; }
        }

        public static MnemonicColor Mauve
        {
            get { return GlobalPalette.Mauve; }
        }

        public static MnemonicColor PaleMauve
        {
            get { return GlobalPalette.PaleMauve; }
        }

        public static MnemonicColor Amethyst
        {
            get { return GlobalPalette.Amethyst; }
        }

        public static MnemonicColor MountbattenPink
        {
            get { return GlobalPalette.MountbattenPink; }
        }

        public static MnemonicColor YellowGreen
        {
            get { return GlobalPalette.YellowGreen; }
        }

        public static MnemonicColor Sienna
        {
            get { return GlobalPalette.Sienna; }
        }

        public static MnemonicColor GradientActiveCaption
        {
            get { return GlobalPalette.GradientActiveCaption; }
        }

        public static MnemonicColor DarkGray
        {
            get { return GlobalPalette.DarkGray; }
        }

        public static MnemonicColor PaleCornflowerBlue
        {
            get { return GlobalPalette.PaleCornflowerBlue; }
        }

        public static MnemonicColor SwampGreen
        {
            get { return GlobalPalette.SwampGreen; }
        }

        public static MnemonicColor Celadon
        {
            get { return GlobalPalette.Celadon; }
        }

        public static MnemonicColor LightBlue
        {
            get { return GlobalPalette.LightBlue; }
        }

        public static MnemonicColor MossGreen
        {
            get { return GlobalPalette.MossGreen; }
        }

        public static MnemonicColor GreenYellow
        {
            get { return GlobalPalette.GreenYellow; }
        }

        public static MnemonicColor PaleCarmine
        {
            get { return GlobalPalette.PaleCarmine; }
        }

        public static MnemonicColor LightSteelBlue
        {
            get { return GlobalPalette.LightSteelBlue; }
        }

        public static MnemonicColor FireBrick
        {
            get { return GlobalPalette.FireBrick; }
        }

        public static MnemonicColor Brass
        {
            get { return GlobalPalette.Brass; }
        }

        public static MnemonicColor Rust
        {
            get { return GlobalPalette.Rust; }
        }

        public static MnemonicColor Copper
        {
            get { return GlobalPalette.Copper; }
        }

        public static MnemonicColor DarkGoldenrod
        {
            get { return GlobalPalette.DarkGoldenrod; }
        }

        public static MnemonicColor MediumOrchid
        {
            get { return GlobalPalette.MediumOrchid; }
        }

        public static MnemonicColor DarkTeaGreen
        {
            get { return GlobalPalette.DarkTeaGreen; }
        }

        public static MnemonicColor RosyBrown
        {
            get { return GlobalPalette.RosyBrown; }
        }

        public static MnemonicColor Silver
        {
            get { return GlobalPalette.Silver; }
        }

        public static MnemonicColor Cardinal
        {
            get { return GlobalPalette.Cardinal; }
        }

        public static MnemonicColor Pang
        {
            get { return GlobalPalette.Pang; }
        }

        public static MnemonicColor Lilac
        {
            get { return GlobalPalette.Lilac; }
        }

        public static MnemonicColor Wisteria
        {
            get { return GlobalPalette.Wisteria; }
        }

        public static MnemonicColor GrayTeaGreen
        {
            get { return GlobalPalette.GrayTeaGreen; }
        }

        public static MnemonicColor BostonUniversityRed
        {
            get { return GlobalPalette.BostonUniversityRed; }
        }

        public static MnemonicColor TransportRed
        {
            get { return GlobalPalette.TransportRed; }
        }

        public static MnemonicColor BurntOrange
        {
            get { return GlobalPalette.BurntOrange; }
        }

        public static MnemonicColor Ochre
        {
            get { return GlobalPalette.Ochre; }
        }

        public static MnemonicColor Puce
        {
            get { return GlobalPalette.Puce; }
        }

        public static MnemonicColor Periwinkle
        {
            get { return GlobalPalette.Periwinkle; }
        }

        public static MnemonicColor BrightViolet
        {
            get { return GlobalPalette.BrightViolet; }
        }

        public static MnemonicColor DarkCoral
        {
            get { return GlobalPalette.DarkCoral; }
        }

        public static MnemonicColor Bronze
        {
            get { return GlobalPalette.Bronze; }
        }

        public static MnemonicColor LightBrown
        {
            get { return GlobalPalette.LightBrown; }
        }

        public static MnemonicColor OldGold
        {
            get { return GlobalPalette.OldGold; }
        }

        public static MnemonicColor TeaGreen
        {
            get { return GlobalPalette.TeaGreen; }
        }

        public static MnemonicColor Pear
        {
            get { return GlobalPalette.Pear; }
        }

        public static MnemonicColor Chocolate
        {
            get { return GlobalPalette.Chocolate; }
        }

        public static MnemonicColor Tan
        {
            get { return GlobalPalette.Tan; }
        }

        public static MnemonicColor LightGray
        {
            get { return GlobalPalette.LightGray; }
        }

        public static MnemonicColor ActiveBorder
        {
            get { return GlobalPalette.ActiveBorder; }
        }

        public static MnemonicColor Titian
        {
            get { return GlobalPalette.Titian; }
        }

        public static MnemonicColor Thistle
        {
            get { return GlobalPalette.Thistle; }
        }

        public static MnemonicColor Orchid
        {
            get { return GlobalPalette.Orchid; }
        }

        public static MnemonicColor Goldenrod
        {
            get { return GlobalPalette.Goldenrod; }
        }

        public static MnemonicColor PaleSandyBrown
        {
            get { return GlobalPalette.PaleSandyBrown; }
        }

        public static MnemonicColor Crimson
        {
            get { return GlobalPalette.Crimson; }
        }

        public static MnemonicColor Gainsboro
        {
            get { return GlobalPalette.Gainsboro; }
        }

        public static MnemonicColor UbuntuOrange
        {
            get { return GlobalPalette.UbuntuOrange; }
        }

        public static MnemonicColor PaleChestnut
        {
            get { return GlobalPalette.PaleChestnut; }
        }

        public static MnemonicColor Cerise
        {
            get { return GlobalPalette.Cerise; }
        }

        public static MnemonicColor BurlyWood
        {
            get { return GlobalPalette.BurlyWood; }
        }

        public static MnemonicColor Heliotrope
        {
            get { return GlobalPalette.Heliotrope; }
        }

        public static MnemonicColor LightCyan
        {
            get { return GlobalPalette.LightCyan; }
        }

        public static MnemonicColor AlizarinCrimson
        {
            get { return GlobalPalette.AlizarinCrimson; }
        }

        public static MnemonicColor Vermilion
        {
            get { return GlobalPalette.Vermilion; }
        }

        public static MnemonicColor Gamboge
        {
            get { return GlobalPalette.Gamboge; }
        }

        public static MnemonicColor Amaranth
        {
            get { return GlobalPalette.Amaranth; }
        }

        public static MnemonicColor Fawn
        {
            get { return GlobalPalette.Fawn; }
        }

        public static MnemonicColor Lavender
        {
            get { return GlobalPalette.Lavender; }
        }

        public static MnemonicColor DarkPink
        {
            get { return GlobalPalette.DarkPink; }
        }

        public static MnemonicColor BurntSienna
        {
            get { return GlobalPalette.BurntSienna; }
        }

        public static MnemonicColor DarkSalmon
        {
            get { return GlobalPalette.DarkSalmon; }
        }

        public static MnemonicColor Zinnwaldite
        {
            get { return GlobalPalette.Zinnwaldite; }
        }

        public static MnemonicColor Carrot
        {
            get { return GlobalPalette.Carrot; }
        }

        public static MnemonicColor Flax
        {
            get { return GlobalPalette.Flax; }
        }

        public static MnemonicColor PaleGoldenrod
        {
            get { return GlobalPalette.PaleGoldenrod; }
        }

        public static MnemonicColor Almond
        {
            get { return GlobalPalette.Almond; }
        }

        public static MnemonicColor LightCoral
        {
            get { return GlobalPalette.LightCoral; }
        }

        public static MnemonicColor Buff
        {
            get { return GlobalPalette.Buff; }
        }

        public static MnemonicColor Honeydew
        {
            get { return GlobalPalette.Honeydew; }
        }

        public static MnemonicColor AntiFlashWhite
        {
            get { return GlobalPalette.AntiFlashWhite; }
        }

        public static MnemonicColor Sana
        {
            get { return GlobalPalette.Sana; }
        }

        public static MnemonicColor Vanilla
        {
            get { return GlobalPalette.Vanilla; }
        }

        public static MnemonicColor SandyBrown
        {
            get { return GlobalPalette.SandyBrown; }
        }

        public static MnemonicColor Saffron
        {
            get { return GlobalPalette.Saffron; }
        }

        public static MnemonicColor Wheat
        {
            get { return GlobalPalette.Wheat; }
        }

        public static MnemonicColor Beige
        {
            get { return GlobalPalette.Beige; }
        }

        public static MnemonicColor WhiteSmoke
        {
            get { return GlobalPalette.WhiteSmoke; }
        }

        public static MnemonicColor MintCream
        {
            get { return GlobalPalette.MintCream; }
        }

        public static MnemonicColor GhostWhite
        {
            get { return GlobalPalette.GhostWhite; }
        }

        public static MnemonicColor PaleMagenta
        {
            get { return GlobalPalette.PaleMagenta; }
        }

        public static MnemonicColor Salmon
        {
            get { return GlobalPalette.Salmon; }
        }

        public static MnemonicColor PalePink
        {
            get { return GlobalPalette.PalePink; }
        }

        public static MnemonicColor PeachYellow
        {
            get { return GlobalPalette.PeachYellow; }
        }

        public static MnemonicColor AntiqueWhite
        {
            get { return GlobalPalette.AntiqueWhite; }
        }

        public static MnemonicColor Blond
        {
            get { return GlobalPalette.Blond; }
        }

        public static MnemonicColor Linen
        {
            get { return GlobalPalette.Linen; }
        }

        public static MnemonicColor LightGoldenrodYellow
        {
            get { return GlobalPalette.LightGoldenrodYellow; }
        }

        public static MnemonicColor Corn
        {
            get { return GlobalPalette.Corn; }
        }

        public static MnemonicColor HotPink
        {
            get { return GlobalPalette.HotPink; }
        }

        public static MnemonicColor Lemon
        {
            get { return GlobalPalette.Lemon; }
        }

        public static MnemonicColor OldLace
        {
            get { return GlobalPalette.OldLace; }
        }

        public static MnemonicColor Bittersweet
        {
            get { return GlobalPalette.Bittersweet; }
        }

        public static MnemonicColor AmericanRose
        {
            get { return GlobalPalette.AmericanRose; }
        }

        public static MnemonicColor DeepPink
        {
            get { return GlobalPalette.DeepPink; }
        }

        public static MnemonicColor Scarlet
        {
            get { return GlobalPalette.Scarlet; }
        }

        public static MnemonicColor OrangeRed
        {
            get { return GlobalPalette.OrangeRed; }
        }

        public static MnemonicColor InternationalOrange
        {
            get { return GlobalPalette.InternationalOrange; }
        }

        public static MnemonicColor Tomato
        {
            get { return GlobalPalette.Tomato; }
        }

        public static MnemonicColor Pumpkin
        {
            get { return GlobalPalette.Pumpkin; }
        }

        public static MnemonicColor Coral
        {
            get { return GlobalPalette.Coral; }
        }

        public static MnemonicColor DarkOrange
        {
            get { return GlobalPalette.DarkOrange; }
        }

        public static MnemonicColor BlazeOrange
        {
            get { return GlobalPalette.BlazeOrange; }
        }

        public static MnemonicColor PinkOrange
        {
            get { return GlobalPalette.PinkOrange; }
        }

        public static MnemonicColor LightSalmon
        {
            get { return GlobalPalette.LightSalmon; }
        }

        public static MnemonicColor DarkTangerine
        {
            get { return GlobalPalette.DarkTangerine; }
        }

        public static MnemonicColor LightPink
        {
            get { return GlobalPalette.LightPink; }
        }

        public static MnemonicColor SelectiveYellow
        {
            get { return GlobalPalette.SelectiveYellow; }
        }

        public static MnemonicColor Amber
        {
            get { return GlobalPalette.Amber; }
        }

        public static MnemonicColor Pink
        {
            get { return GlobalPalette.Pink; }
        }

        public static MnemonicColor Tangerine
        {
            get { return GlobalPalette.Tangerine; }
        }

        public static MnemonicColor PeachOrange
        {
            get { return GlobalPalette.PeachOrange; }
        }

        public static MnemonicColor PastelPink
        {
            get { return GlobalPalette.PastelPink; }
        }

        public static MnemonicColor Gold
        {
            get { return GlobalPalette.Gold; }
        }

        public static MnemonicColor SchoolBusYellow
        {
            get { return GlobalPalette.SchoolBusYellow; }
        }

        public static MnemonicColor Mustard
        {
            get { return GlobalPalette.Mustard; }
        }

        public static MnemonicColor NavajoWhite
        {
            get { return GlobalPalette.NavajoWhite; }
        }

        public static MnemonicColor Moccasin
        {
            get { return GlobalPalette.Moccasin; }
        }

        public static MnemonicColor Bisque
        {
            get { return GlobalPalette.Bisque; }
        }

        public static MnemonicColor MistyRose
        {
            get { return GlobalPalette.MistyRose; }
        }

        public static MnemonicColor Peach
        {
            get { return GlobalPalette.Peach; }
        }

        public static MnemonicColor BlanchedAlmond
        {
            get { return GlobalPalette.BlanchedAlmond; }
        }

        public static MnemonicColor PapayaWhip
        {
            get { return GlobalPalette.PapayaWhip; }
        }

        public static MnemonicColor LavenderBlush
        {
            get { return GlobalPalette.LavenderBlush; }
        }

        public static MnemonicColor Seashell
        {
            get { return GlobalPalette.Seashell; }
        }

        public static MnemonicColor Cornsilk
        {
            get { return GlobalPalette.Cornsilk; }
        }

        public static MnemonicColor FloralWhite
        {
            get { return GlobalPalette.FloralWhite; }
        }

        public static MnemonicColor Snow
        {
            get { return GlobalPalette.Snow; }
        }

        public static MnemonicColor Cream
        {
            get { return GlobalPalette.Cream; }
        }

        public static MnemonicColor LightYellow
        {
            get { return GlobalPalette.LightYellow; }
        }

        public static MnemonicColor Info
        {
            get { return GlobalPalette.Info; }
        }

        public static MnemonicColor Ivory
        {
            get { return GlobalPalette.Ivory; }
        }

        public static MnemonicColor White
        {
            get { return GlobalPalette.White; }
        }
        #endregion

        public static MnemonicColor Find(string nameOrCode)
        {
            return GlobalPalette.Colors.Find(nameOrCode);
        }

        public static MnemonicColor Get(string nameOrCode)
        {
            return GlobalPalette.Colors[nameOrCode];
        }

        public static MnemonicColor Parse(string sentence, out bool onlyColor)
        {
            return GlobalPalette.Colors.Parse(sentence, out onlyColor);
        }

        public static MnemonicColor Parse(string sentence)
        {
            return GlobalPalette.Colors.Parse(sentence);
        }

        #endregion
    }
}