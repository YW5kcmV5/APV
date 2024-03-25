using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using APV.Common;
using APV.Common.Extensions;
using APV.Common.Html;
using APV.GraphicsLibrary.Colors;
using APV.GraphicsLibrary.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.UnitTest.System
{
    [TestClass]
    public class MnemonicColorTests
    {
        private static string[] SplitNames(string name, HashSet<char> symbolsHashSet)
        {
            //separators: ",", "(", "[", "]"
            string[] items = name.Split(new[] { ",", ";", "(", ")", "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
            items = items
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().Replace("   ", " ").Replace("  ", " ").Replace(" цвет", string.Empty).Replace("цвет ", string.Empty).Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .ToArray();
            items = items.Where(x => x.ToList().All(symbolsHashSet.Contains)).ToArray();

            var result = new List<string>();
            foreach (string item in items)
            {
                if (!result.Contains(item, StringComparer.InvariantCultureIgnoreCase))
                {
                    result.Add(item.ToPascalCase());
                    string variant1 = item.Replace("ё", "е").ToPascalCase();
                    string variant2 = item.Replace("-", " ").ToPascalCase();
                    string variant3 = variant1.Replace("-", " ").ToPascalCase();
                    //string variant4 = variant2.ToPascalCase(true);
                    //string variant5 = variant3.ToPascalCase(true);
                    if (!result.Contains(variant1, StringComparer.InvariantCultureIgnoreCase))
                    {
                        result.Add(variant1);
                    }
                    if (!result.Contains(variant2, StringComparer.InvariantCultureIgnoreCase))
                    {
                        result.Add(variant2);
                    }
                    if (!result.Contains(variant3, StringComparer.InvariantCultureIgnoreCase))
                    {
                        result.Add(variant3);
                    }
                    //if (!result.Contains(variant4, StringComparer.InvariantCultureIgnoreCase))
                    //{
                    //    result.Add(variant4);
                    //}
                    //if (!result.Contains(variant5, StringComparer.InvariantCultureIgnoreCase))
                    //{
                    //    result.Add(variant5);
                    //}
                }
            }

            return result.ToArray();
        }

        private static string GetFullText(HtmlTag tag)
        {
            var sb = new StringBuilder();
            foreach (HtmlTag child in tag.Children)
            {
                if ((!string.IsNullOrWhiteSpace(child.Text)) && (string.Compare(child.Text, "ср.", StringComparison.InvariantCultureIgnoreCase) != 0))
                {
                    sb.AppendFormat("{0}, ", child.Text);
                }
            }
            if ((!string.IsNullOrWhiteSpace(tag.Text)) && (string.Compare(tag.Text, "ср.", StringComparison.InvariantCultureIgnoreCase) != 0))
            {
                sb.AppendFormat("{0}, ", tag.Text);
            }
            if (sb.Length > 2)
            {
                sb.Length -= 2;
            }
            return sb.ToString();
        }

        private static void SetColor(SortedList<string, SortedList<string, string[]>> colors, string code, string[] enValues, string[] ruValues)
        {
            enValues = enValues ?? new string[0];
            ruValues = ruValues ?? new string[0];
            if (!colors.ContainsKey(code))
            {
                colors.Add(code, new SortedList<string, string[]> {{"EN", new string[0]}, {"RU", new string[0]}});
            }
            SortedList<string, string[]> names = colors[code];
            if (enValues.Length > 0)
            {
                string[] items = names["EN"];
                items = items.Concat(enValues).Distinct().ToArray();
                names["EN"] = items;
            }
            if (ruValues.Length > 0)
            {
                string[] items = names["RU"];
                items = items.Concat(ruValues).Distinct().ToArray();
                names["RU"] = items;
            }
        }

        private static string RestoreFromPascalCase(string pascalCase)
        {
            var sb = new StringBuilder();
            foreach (char symbol in pascalCase)
            {
                if ((sb.Length > 0) && (char.IsUpper(symbol) && (sb[sb.Length - 1] != ' ')))
                {
                    sb.Append(" ");
                }
                sb.Append(symbol);
            }
            return sb.ToString().Trim();
        }

        [TestMethod]
        public void RedColorTest()
        {
            MnemonicColor red = MnemonicColor.Red;

            Assert.IsNotNull(red);
            Assert.AreEqual("#FF0000", red.Code);
            Assert.AreEqual("Красный", red.Names["RU"]);
            Assert.AreEqual("Red", red.Names["EN"]);

            Assert.AreEqual(red.Code, MnemonicColor.Get(" red").Code);
            Assert.AreEqual(red, MnemonicColor.Find(" red"));
            Assert.AreEqual(red, MnemonicColor.Find(" красный"));
            Assert.AreEqual(red, MnemonicColor.Find(" #FF0000 "));
        }

        [TestMethod]
        public void NavyColorTest()
        {
            MnemonicColor navy = MnemonicColor.Navy;

            Assert.IsNotNull(navy);
            Assert.AreEqual("#000080", navy.Code);
            Assert.AreEqual("Тёмно-синий", navy.Names["RU"]);
            Assert.AreEqual("Navy", navy.Names["EN"]);

            Assert.AreEqual(navy.Code, MnemonicColor.Get("navy").Code);
            Assert.AreEqual(navy, MnemonicColor.Find("navy"));
            Assert.AreEqual(navy, MnemonicColor.Find("тёмно-синий"));
            Assert.AreEqual(navy, MnemonicColor.Find("#000080"));
        }

        [TestMethod]
        public void HtmlPaletteTest()
        {
            Assert.AreEqual(16, MnemonicColors.Html.Count);
            Assert.AreEqual("#000000", MnemonicColors.Html.Black.Code);
            Assert.AreEqual("#C0C0C0", MnemonicColors.Html.Silver.Code);
            Assert.AreEqual("#808080", MnemonicColors.Html.Gray.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.Html.White.Code);
            Assert.AreEqual("#800000", MnemonicColors.Html.Maroon.Code);
            Assert.AreEqual("#FF0000", MnemonicColors.Html.Red.Code);
            Assert.AreEqual("#800080", MnemonicColors.Html.Purple.Code);
            Assert.AreEqual("#FF00FF", MnemonicColors.Html.Fuchsia.Code);
            Assert.AreEqual("#008000", MnemonicColors.Html.Green.Code);
            Assert.AreEqual("#00FF00", MnemonicColors.Html.Lime.Code);
            Assert.AreEqual("#808000", MnemonicColors.Html.Olive.Code);
            Assert.AreEqual("#FFFF00", MnemonicColors.Html.Yellow.Code);
            Assert.AreEqual("#000080", MnemonicColors.Html.Navy.Code);
            Assert.AreEqual("#0000FF", MnemonicColors.Html.Blue.Code);
            Assert.AreEqual("#008080", MnemonicColors.Html.Teal.Code);
            Assert.AreEqual("#00FFFF", MnemonicColors.Html.Aqua.Code);
        }

        [TestMethod]
        public void SpectrumPaletteTest()
        {
            Assert.AreEqual(7, MnemonicColors.Spectrum.Count);
            Assert.AreEqual("#FF0000", MnemonicColors.Spectrum.Red.Code);
            Assert.AreEqual("#FFA500", MnemonicColors.Spectrum.Orange.Code);
            Assert.AreEqual("#FFFF00", MnemonicColors.Spectrum.Yellow.Code);
            Assert.AreEqual("#00FF00", MnemonicColors.Spectrum.Green.Code);
            Assert.AreEqual("#00FFFF", MnemonicColors.Spectrum.Cyan.Code);
            Assert.AreEqual("#0000FF", MnemonicColors.Spectrum.Blue.Code);
            Assert.AreEqual("#8B00FF", MnemonicColors.Spectrum.Violet.Code);
        }

        [TestMethod]
        public void CssPaletteTest()
        {
            Assert.AreEqual("#F0F8FF", MnemonicColors.Css.AliceBlue.Code);
            Assert.AreEqual("#FAEBD7", MnemonicColors.Css.AntiqueWhite.Code);
            Assert.AreEqual("#00FFFF", MnemonicColors.Css.Aqua.Code);
            Assert.AreEqual("#7FFFD4", MnemonicColors.Css.Aquamarine.Code);
            Assert.AreEqual("#F0FFFF", MnemonicColors.Css.Azure.Code);
            Assert.AreEqual("#F5F5DC", MnemonicColors.Css.Beige.Code);
            Assert.AreEqual("#FFE4C4", MnemonicColors.Css.Bisque.Code);
            Assert.AreEqual("#000000", MnemonicColors.Css.Black.Code);
            Assert.AreEqual("#FFEBCD", MnemonicColors.Css.BlanchedAlmond.Code);
            Assert.AreEqual("#0000FF", MnemonicColors.Css.Blue.Code);
            Assert.AreEqual("#8A2BE2", MnemonicColors.Css.BlueViolet.Code);
            Assert.AreEqual("#A52A2A", MnemonicColors.Css.Brown.Code);
            Assert.AreEqual("#DEB887", MnemonicColors.Css.BurlyWood.Code);
            Assert.AreEqual("#5F9EA0", MnemonicColors.Css.CadetBlue.Code);
            Assert.AreEqual("#7FFF00", MnemonicColors.Css.Chartreuse.Code);
            Assert.AreEqual("#D2691E", MnemonicColors.Css.Chocolate.Code);
            Assert.AreEqual("#FF7F50", MnemonicColors.Css.Coral.Code);
            Assert.AreEqual("#6495ED", MnemonicColors.Css.CornflowerBlue.Code);
            Assert.AreEqual("#FFF8DC", MnemonicColors.Css.Cornsilk.Code);
            Assert.AreEqual("#DC143C", MnemonicColors.Css.Crimson.Code);
            Assert.AreEqual("#00FFFF", MnemonicColors.Css.Cyan.Code);
            Assert.AreEqual("#00008B", MnemonicColors.Css.DarkBlue.Code);
            Assert.AreEqual("#008B8B", MnemonicColors.Css.DarkCyan.Code);
            Assert.AreEqual("#B8860B", MnemonicColors.Css.DarkGoldenrod.Code);
            Assert.AreEqual("#A9A9A9", MnemonicColors.Css.DarkGray.Code);
            Assert.AreEqual("#006400", MnemonicColors.Css.DarkGreen.Code);
            Assert.AreEqual("#BDB76B", MnemonicColors.Css.DarkKhaki.Code);
            Assert.AreEqual("#8B008B", MnemonicColors.Css.DarkMagenta.Code);
            Assert.AreEqual("#556B2F", MnemonicColors.Css.DarkOliveGreen.Code);
            Assert.AreEqual("#FF8C00", MnemonicColors.Css.DarkOrange.Code);
            Assert.AreEqual("#9932CC", MnemonicColors.Css.DarkOrchid.Code);
            Assert.AreEqual("#8B0000", MnemonicColors.Css.DarkRed.Code);
            Assert.AreEqual("#E9967A", MnemonicColors.Css.DarkSalmon.Code);
            Assert.AreEqual("#8FBC8F", MnemonicColors.Css.DarkSeaGreen.Code);
            Assert.AreEqual("#483D8B", MnemonicColors.Css.DarkSlateBlue.Code);
            Assert.AreEqual("#2F4F4F", MnemonicColors.Css.DarkSlateGray.Code);
            Assert.AreEqual("#00CED1", MnemonicColors.Css.DarkTurquoise.Code);
            Assert.AreEqual("#9400D3", MnemonicColors.Css.DarkViolet.Code);
            Assert.AreEqual("#FF1493", MnemonicColors.Css.DeepPink.Code);
            Assert.AreEqual("#00BFFF", MnemonicColors.Css.DeepSkyBlue.Code);
            Assert.AreEqual("#696969", MnemonicColors.Css.DimGray.Code);
            Assert.AreEqual("#1E90FF", MnemonicColors.Css.DodgerBlue.Code);
            Assert.AreEqual("#B22222", MnemonicColors.Css.FireBrick.Code);
            Assert.AreEqual("#FFFAF0", MnemonicColors.Css.FloralWhite.Code);
            Assert.AreEqual("#228B22", MnemonicColors.Css.ForestGreen.Code);
            Assert.AreEqual("#FF00FF", MnemonicColors.Css.Fuchsia.Code);
            Assert.AreEqual("#DCDCDC", MnemonicColors.Css.Gainsboro.Code);
            Assert.AreEqual("#F8F8FF", MnemonicColors.Css.GhostWhite.Code);
            Assert.AreEqual("#FFD700", MnemonicColors.Css.Gold.Code);
            Assert.AreEqual("#DAA520", MnemonicColors.Css.Goldenrod.Code);
            Assert.AreEqual("#808080", MnemonicColors.Css.Gray.Code);
            Assert.AreEqual("#008000", MnemonicColors.Css.Green.Code);
            Assert.AreEqual("#ADFF2F", MnemonicColors.Css.GreenYellow.Code);
            Assert.AreEqual("#F0FFF0", MnemonicColors.Css.Honeydew.Code);
            Assert.AreEqual("#FF69B4", MnemonicColors.Css.HotPink.Code);
            Assert.AreEqual("#CD5C5C", MnemonicColors.Css.IndianRed.Code);
            Assert.AreEqual("#4B0082", MnemonicColors.Css.Indigo.Code);
            Assert.AreEqual("#FFFFF0", MnemonicColors.Css.Ivory.Code);
            Assert.AreEqual("#F0E68C", MnemonicColors.Css.Khaki.Code);
            Assert.AreEqual("#E6E6FA", MnemonicColors.Css.Lavender.Code);
            Assert.AreEqual("#FFF0F5", MnemonicColors.Css.LavenderBlush.Code);
            Assert.AreEqual("#7CFC00", MnemonicColors.Css.LawnGreen.Code);
            Assert.AreEqual("#FFFACD", MnemonicColors.Css.LemonChiffon.Code);
            Assert.AreEqual("#ADD8E6", MnemonicColors.Css.LightBlue.Code);
            Assert.AreEqual("#F08080", MnemonicColors.Css.LightCoral.Code);
            Assert.AreEqual("#E0FFFF", MnemonicColors.Css.LightCyan.Code);
            Assert.AreEqual("#FAFAD2", MnemonicColors.Css.LightGoldenrodYellow.Code);
            Assert.AreEqual("#D3D3D3", MnemonicColors.Css.LightGray.Code);
            Assert.AreEqual("#90EE90", MnemonicColors.Css.LightGreen.Code);
            Assert.AreEqual("#FFB6C1", MnemonicColors.Css.LightPink.Code);
            Assert.AreEqual("#FFA07A", MnemonicColors.Css.LightSalmon.Code);
            Assert.AreEqual("#20B2AA", MnemonicColors.Css.LightSeaGreen.Code);
            Assert.AreEqual("#87CEFA", MnemonicColors.Css.LightSkyBlue.Code);
            Assert.AreEqual("#778899", MnemonicColors.Css.LightSlateGray.Code);
            Assert.AreEqual("#B0C4DE", MnemonicColors.Css.LightSteelBlue.Code);
            Assert.AreEqual("#FFFFE0", MnemonicColors.Css.LightYellow.Code);
            Assert.AreEqual("#00FF00", MnemonicColors.Css.Lime.Code);
            Assert.AreEqual("#32CD32", MnemonicColors.Css.LimeGreen.Code);
            Assert.AreEqual("#FAF0E6", MnemonicColors.Css.Linen.Code);
            Assert.AreEqual("#FF00FF", MnemonicColors.Css.Magenta.Code);
            Assert.AreEqual("#800000", MnemonicColors.Css.Maroon.Code);
            Assert.AreEqual("#66CDAA", MnemonicColors.Css.MediumAquamarine.Code);
            Assert.AreEqual("#0000CD", MnemonicColors.Css.MediumBlue.Code);
            Assert.AreEqual("#BA55D3", MnemonicColors.Css.MediumOrchid.Code);
            Assert.AreEqual("#9370DB", MnemonicColors.Css.MediumPurple.Code);
            Assert.AreEqual("#3CB371", MnemonicColors.Css.MediumSeaGreen.Code);
            Assert.AreEqual("#7B68EE", MnemonicColors.Css.MediumSlateBlue.Code);
            Assert.AreEqual("#00FA9A", MnemonicColors.Css.MediumSpringGreen.Code);
            Assert.AreEqual("#48D1CC", MnemonicColors.Css.MediumTurquoise.Code);
            Assert.AreEqual("#C71585", MnemonicColors.Css.MediumVioletRed.Code);
            Assert.AreEqual("#191970", MnemonicColors.Css.MidnightBlue.Code);
            Assert.AreEqual("#F5FFFA", MnemonicColors.Css.MintCream.Code);
            Assert.AreEqual("#FFE4E1", MnemonicColors.Css.MistyRose.Code);
            Assert.AreEqual("#FFE4B5", MnemonicColors.Css.Moccasin.Code);
            Assert.AreEqual("#FFDEAD", MnemonicColors.Css.NavajoWhite.Code);
            Assert.AreEqual("#000080", MnemonicColors.Css.Navy.Code);
            Assert.AreEqual("#FDF5E6", MnemonicColors.Css.OldLace.Code);
            Assert.AreEqual("#808000", MnemonicColors.Css.Olive.Code);
            Assert.AreEqual("#6B8E23", MnemonicColors.Css.OliveDrab.Code);
            Assert.AreEqual("#FFA500", MnemonicColors.Css.Orange.Code);
            Assert.AreEqual("#FF4500", MnemonicColors.Css.OrangeRed.Code);
            Assert.AreEqual("#DA70D6", MnemonicColors.Css.Orchid.Code);
            Assert.AreEqual("#EEE8AA", MnemonicColors.Css.PaleGoldenrod.Code);
            Assert.AreEqual("#98FB98", MnemonicColors.Css.PaleGreen.Code);
            Assert.AreEqual("#AFEEEE", MnemonicColors.Css.PaleTurquoise.Code);
            Assert.AreEqual("#DB7093", MnemonicColors.Css.PaleVioletRed.Code);
            Assert.AreEqual("#FFEFD5", MnemonicColors.Css.PapayaWhip.Code);
            Assert.AreEqual("#FFDAB9", MnemonicColors.Css.PeachPuff.Code);
            Assert.AreEqual("#CD853F", MnemonicColors.Css.Peru.Code);
            Assert.AreEqual("#FFC0CB", MnemonicColors.Css.Pink.Code);
            Assert.AreEqual("#DDA0DD", MnemonicColors.Css.Plum.Code);
            Assert.AreEqual("#B0E0E6", MnemonicColors.Css.PowderBlue.Code);
            Assert.AreEqual("#800080", MnemonicColors.Css.Purple.Code);
            Assert.AreEqual("#663399", MnemonicColors.Css.RebeccaPurple.Code);
            Assert.AreEqual("#FF0000", MnemonicColors.Css.Red.Code);
            Assert.AreEqual("#BC8F8F", MnemonicColors.Css.RosyBrown.Code);
            Assert.AreEqual("#4169E1", MnemonicColors.Css.RoyalBlue.Code);
            Assert.AreEqual("#8B4513", MnemonicColors.Css.SaddleBrown.Code);
            Assert.AreEqual("#FA8072", MnemonicColors.Css.Salmon.Code);
            Assert.AreEqual("#F4A460", MnemonicColors.Css.SandyBrown.Code);
            Assert.AreEqual("#2E8B57", MnemonicColors.Css.SeaGreen.Code);
            Assert.AreEqual("#FFF5EE", MnemonicColors.Css.Seashell.Code);
            Assert.AreEqual("#A0522D", MnemonicColors.Css.Sienna.Code);
            Assert.AreEqual("#C0C0C0", MnemonicColors.Css.Silver.Code);
            Assert.AreEqual("#87CEEB", MnemonicColors.Css.SkyBlue.Code);
            Assert.AreEqual("#6A5ACD", MnemonicColors.Css.SlateBlue.Code);
            Assert.AreEqual("#708090", MnemonicColors.Css.SlateGray.Code);
            Assert.AreEqual("#FFFAFA", MnemonicColors.Css.Snow.Code);
            Assert.AreEqual("#00FF7F", MnemonicColors.Css.SpringGreen.Code);
            Assert.AreEqual("#4682B4", MnemonicColors.Css.SteelBlue.Code);
            Assert.AreEqual("#D2B48C", MnemonicColors.Css.Tan.Code);
            Assert.AreEqual("#008080", MnemonicColors.Css.Teal.Code);
            Assert.AreEqual("#D8BFD8", MnemonicColors.Css.Thistle.Code);
            Assert.AreEqual("#FF6347", MnemonicColors.Css.Tomato.Code);
            Assert.AreEqual("#40E0D0", MnemonicColors.Css.Turquoise.Code);
            Assert.AreEqual("#EE82EE", MnemonicColors.Css.Violet.Code);
            Assert.AreEqual("#F5DEB3", MnemonicColors.Css.Wheat.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.Css.White.Code);
            Assert.AreEqual("#F5F5F5", MnemonicColors.Css.WhiteSmoke.Code);
            Assert.AreEqual("#FFFF00", MnemonicColors.Css.Yellow.Code);
            Assert.AreEqual("#9ACD32", MnemonicColors.Css.YellowGreen.Code);
        }

        [TestMethod]
        public void SystemColorsPaletteTest()
        {
            Assert.AreEqual(11, MnemonicColors.SystemColors.Count);
            Assert.AreEqual("#D4D0C8", MnemonicColors.SystemColors.ActiveBorder.Code);
            Assert.AreEqual("#0A246A", MnemonicColors.SystemColors.ActiveCaption.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.SystemColors.ActiveCaptionText.Code);
            Assert.AreEqual("#808080", MnemonicColors.SystemColors.AppWorkspace.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.SystemColors.ButtonFace.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.SystemColors.ButtonHighlight.Code);
            Assert.AreEqual("#808080", MnemonicColors.SystemColors.ButtonShadow.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.SystemColors.Control.Code);
            Assert.AreEqual("#808080", MnemonicColors.SystemColors.ControlDark.Code);
            Assert.AreEqual("#404040", MnemonicColors.SystemColors.ControlDarkDark.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.SystemColors.ControlLight.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.SystemColors.ControlLightLight.Code);
            Assert.AreEqual("#000000", MnemonicColors.SystemColors.ControlText.Code);
            Assert.AreEqual("#3A6EA5", MnemonicColors.SystemColors.Desktop.Code);
            Assert.AreEqual("#A6CAF0", MnemonicColors.SystemColors.GradientActiveCaption.Code);
            Assert.AreEqual("#C0C0C0", MnemonicColors.SystemColors.GradientInactiveCaption.Code);
            Assert.AreEqual("#808080", MnemonicColors.SystemColors.GrayText.Code);
            Assert.AreEqual("#0A246A", MnemonicColors.SystemColors.Highlight.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.SystemColors.HighlightText.Code);
            Assert.AreEqual("#000080", MnemonicColors.SystemColors.HotTrack.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.SystemColors.InactiveBorder.Code);
            Assert.AreEqual("#808080", MnemonicColors.SystemColors.InactiveCaption.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.SystemColors.InactiveCaptionText.Code);
            Assert.AreEqual("#FFFFE1", MnemonicColors.SystemColors.Info.Code);
            Assert.AreEqual("#000000", MnemonicColors.SystemColors.InfoText.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.SystemColors.Menu.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.SystemColors.MenuBar.Code);
            Assert.AreEqual("#0A246A", MnemonicColors.SystemColors.MenuHighlight.Code);
            Assert.AreEqual("#000000", MnemonicColors.SystemColors.MenuText.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.SystemColors.ScrollBar.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.SystemColors.Window.Code);
            Assert.AreEqual("#000000", MnemonicColors.SystemColors.WindowFrame.Code);
            Assert.AreEqual("#000000", MnemonicColors.SystemColors.WindowText.Code);
        }

        [TestMethod]
        public void KnownColorsPaletteTest()
        {
            Assert.AreEqual(144, MnemonicColors.KnownColors.Count);
            Assert.AreEqual("#D4D0C8", MnemonicColors.KnownColors.ActiveBorder.Code);
            Assert.AreEqual("#0A246A", MnemonicColors.KnownColors.ActiveCaption.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.KnownColors.ActiveCaptionText.Code);
            Assert.AreEqual("#808080", MnemonicColors.KnownColors.AppWorkspace.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.KnownColors.Control.Code);
            Assert.AreEqual("#808080", MnemonicColors.KnownColors.ControlDark.Code);
            Assert.AreEqual("#404040", MnemonicColors.KnownColors.ControlDarkDark.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.KnownColors.ControlLight.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.KnownColors.ControlLightLight.Code);
            Assert.AreEqual("#000000", MnemonicColors.KnownColors.ControlText.Code);
            Assert.AreEqual("#3A6EA5", MnemonicColors.KnownColors.Desktop.Code);
            Assert.AreEqual("#808080", MnemonicColors.KnownColors.GrayText.Code);
            Assert.AreEqual("#0A246A", MnemonicColors.KnownColors.Highlight.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.KnownColors.HighlightText.Code);
            Assert.AreEqual("#000080", MnemonicColors.KnownColors.HotTrack.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.KnownColors.InactiveBorder.Code);
            Assert.AreEqual("#808080", MnemonicColors.KnownColors.InactiveCaption.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.KnownColors.InactiveCaptionText.Code);
            Assert.AreEqual("#FFFFE1", MnemonicColors.KnownColors.Info.Code);
            Assert.AreEqual("#000000", MnemonicColors.KnownColors.InfoText.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.KnownColors.Menu.Code);
            Assert.AreEqual("#000000", MnemonicColors.KnownColors.MenuText.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.KnownColors.ScrollBar.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.KnownColors.Window.Code);
            Assert.AreEqual("#000000", MnemonicColors.KnownColors.WindowFrame.Code);
            Assert.AreEqual("#000000", MnemonicColors.KnownColors.WindowText.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.KnownColors.Transparent.Code);
            Assert.AreEqual("#F0F8FF", MnemonicColors.KnownColors.AliceBlue.Code);
            Assert.AreEqual("#FAEBD7", MnemonicColors.KnownColors.AntiqueWhite.Code);
            Assert.AreEqual("#00FFFF", MnemonicColors.KnownColors.Aqua.Code);
            Assert.AreEqual("#7FFFD4", MnemonicColors.KnownColors.Aquamarine.Code);
            Assert.AreEqual("#F0FFFF", MnemonicColors.KnownColors.Azure.Code);
            Assert.AreEqual("#F5F5DC", MnemonicColors.KnownColors.Beige.Code);
            Assert.AreEqual("#FFE4C4", MnemonicColors.KnownColors.Bisque.Code);
            Assert.AreEqual("#000000", MnemonicColors.KnownColors.Black.Code);
            Assert.AreEqual("#FFEBCD", MnemonicColors.KnownColors.BlanchedAlmond.Code);
            Assert.AreEqual("#0000FF", MnemonicColors.KnownColors.Blue.Code);
            Assert.AreEqual("#8A2BE2", MnemonicColors.KnownColors.BlueViolet.Code);
            Assert.AreEqual("#A52A2A", MnemonicColors.KnownColors.Brown.Code);
            Assert.AreEqual("#DEB887", MnemonicColors.KnownColors.BurlyWood.Code);
            Assert.AreEqual("#5F9EA0", MnemonicColors.KnownColors.CadetBlue.Code);
            Assert.AreEqual("#7FFF00", MnemonicColors.KnownColors.Chartreuse.Code);
            Assert.AreEqual("#D2691E", MnemonicColors.KnownColors.Chocolate.Code);
            Assert.AreEqual("#FF7F50", MnemonicColors.KnownColors.Coral.Code);
            Assert.AreEqual("#6495ED", MnemonicColors.KnownColors.CornflowerBlue.Code);
            Assert.AreEqual("#FFF8DC", MnemonicColors.KnownColors.Cornsilk.Code);
            Assert.AreEqual("#DC143C", MnemonicColors.KnownColors.Crimson.Code);
            Assert.AreEqual("#00FFFF", MnemonicColors.KnownColors.Cyan.Code);
            Assert.AreEqual("#00008B", MnemonicColors.KnownColors.DarkBlue.Code);
            Assert.AreEqual("#008B8B", MnemonicColors.KnownColors.DarkCyan.Code);
            Assert.AreEqual("#B8860B", MnemonicColors.KnownColors.DarkGoldenrod.Code);
            Assert.AreEqual("#A9A9A9", MnemonicColors.KnownColors.DarkGray.Code);
            Assert.AreEqual("#006400", MnemonicColors.KnownColors.DarkGreen.Code);
            Assert.AreEqual("#BDB76B", MnemonicColors.KnownColors.DarkKhaki.Code);
            Assert.AreEqual("#8B008B", MnemonicColors.KnownColors.DarkMagenta.Code);
            Assert.AreEqual("#556B2F", MnemonicColors.KnownColors.DarkOliveGreen.Code);
            Assert.AreEqual("#FF8C00", MnemonicColors.KnownColors.DarkOrange.Code);
            Assert.AreEqual("#9932CC", MnemonicColors.KnownColors.DarkOrchid.Code);
            Assert.AreEqual("#8B0000", MnemonicColors.KnownColors.DarkRed.Code);
            Assert.AreEqual("#E9967A", MnemonicColors.KnownColors.DarkSalmon.Code);
            Assert.AreEqual("#8FBC8B", MnemonicColors.KnownColors.DarkSeaGreen.Code);
            Assert.AreEqual("#483D8B", MnemonicColors.KnownColors.DarkSlateBlue.Code);
            Assert.AreEqual("#2F4F4F", MnemonicColors.KnownColors.DarkSlateGray.Code);
            Assert.AreEqual("#00CED1", MnemonicColors.KnownColors.DarkTurquoise.Code);
            Assert.AreEqual("#9400D3", MnemonicColors.KnownColors.DarkViolet.Code);
            Assert.AreEqual("#FF1493", MnemonicColors.KnownColors.DeepPink.Code);
            Assert.AreEqual("#00BFFF", MnemonicColors.KnownColors.DeepSkyBlue.Code);
            Assert.AreEqual("#696969", MnemonicColors.KnownColors.DimGray.Code);
            Assert.AreEqual("#1E90FF", MnemonicColors.KnownColors.DodgerBlue.Code);
            Assert.AreEqual("#B22222", MnemonicColors.KnownColors.Firebrick.Code);
            Assert.AreEqual("#FFFAF0", MnemonicColors.KnownColors.FloralWhite.Code);
            Assert.AreEqual("#228B22", MnemonicColors.KnownColors.ForestGreen.Code);
            Assert.AreEqual("#FF00FF", MnemonicColors.KnownColors.Fuchsia.Code);
            Assert.AreEqual("#DCDCDC", MnemonicColors.KnownColors.Gainsboro.Code);
            Assert.AreEqual("#F8F8FF", MnemonicColors.KnownColors.GhostWhite.Code);
            Assert.AreEqual("#FFD700", MnemonicColors.KnownColors.Gold.Code);
            Assert.AreEqual("#DAA520", MnemonicColors.KnownColors.Goldenrod.Code);
            Assert.AreEqual("#808080", MnemonicColors.KnownColors.Gray.Code);
            Assert.AreEqual("#008000", MnemonicColors.KnownColors.Green.Code);
            Assert.AreEqual("#ADFF2F", MnemonicColors.KnownColors.GreenYellow.Code);
            Assert.AreEqual("#F0FFF0", MnemonicColors.KnownColors.Honeydew.Code);
            Assert.AreEqual("#FF69B4", MnemonicColors.KnownColors.HotPink.Code);
            Assert.AreEqual("#CD5C5C", MnemonicColors.KnownColors.IndianRed.Code);
            Assert.AreEqual("#4B0082", MnemonicColors.KnownColors.Indigo.Code);
            Assert.AreEqual("#FFFFF0", MnemonicColors.KnownColors.Ivory.Code);
            Assert.AreEqual("#F0E68C", MnemonicColors.KnownColors.Khaki.Code);
            Assert.AreEqual("#E6E6FA", MnemonicColors.KnownColors.Lavender.Code);
            Assert.AreEqual("#FFF0F5", MnemonicColors.KnownColors.LavenderBlush.Code);
            Assert.AreEqual("#7CFC00", MnemonicColors.KnownColors.LawnGreen.Code);
            Assert.AreEqual("#FFFACD", MnemonicColors.KnownColors.LemonChiffon.Code);
            Assert.AreEqual("#ADD8E6", MnemonicColors.KnownColors.LightBlue.Code);
            Assert.AreEqual("#F08080", MnemonicColors.KnownColors.LightCoral.Code);
            Assert.AreEqual("#E0FFFF", MnemonicColors.KnownColors.LightCyan.Code);
            Assert.AreEqual("#FAFAD2", MnemonicColors.KnownColors.LightGoldenrodYellow.Code);
            Assert.AreEqual("#D3D3D3", MnemonicColors.KnownColors.LightGray.Code);
            Assert.AreEqual("#90EE90", MnemonicColors.KnownColors.LightGreen.Code);
            Assert.AreEqual("#FFB6C1", MnemonicColors.KnownColors.LightPink.Code);
            Assert.AreEqual("#FFA07A", MnemonicColors.KnownColors.LightSalmon.Code);
            Assert.AreEqual("#20B2AA", MnemonicColors.KnownColors.LightSeaGreen.Code);
            Assert.AreEqual("#87CEFA", MnemonicColors.KnownColors.LightSkyBlue.Code);
            Assert.AreEqual("#778899", MnemonicColors.KnownColors.LightSlateGray.Code);
            Assert.AreEqual("#B0C4DE", MnemonicColors.KnownColors.LightSteelBlue.Code);
            Assert.AreEqual("#FFFFE0", MnemonicColors.KnownColors.LightYellow.Code);
            Assert.AreEqual("#00FF00", MnemonicColors.KnownColors.Lime.Code);
            Assert.AreEqual("#32CD32", MnemonicColors.KnownColors.LimeGreen.Code);
            Assert.AreEqual("#FAF0E6", MnemonicColors.KnownColors.Linen.Code);
            Assert.AreEqual("#FF00FF", MnemonicColors.KnownColors.Magenta.Code);
            Assert.AreEqual("#800000", MnemonicColors.KnownColors.Maroon.Code);
            Assert.AreEqual("#66CDAA", MnemonicColors.KnownColors.MediumAquamarine.Code);
            Assert.AreEqual("#0000CD", MnemonicColors.KnownColors.MediumBlue.Code);
            Assert.AreEqual("#BA55D3", MnemonicColors.KnownColors.MediumOrchid.Code);
            Assert.AreEqual("#9370DB", MnemonicColors.KnownColors.MediumPurple.Code);
            Assert.AreEqual("#3CB371", MnemonicColors.KnownColors.MediumSeaGreen.Code);
            Assert.AreEqual("#7B68EE", MnemonicColors.KnownColors.MediumSlateBlue.Code);
            Assert.AreEqual("#00FA9A", MnemonicColors.KnownColors.MediumSpringGreen.Code);
            Assert.AreEqual("#48D1CC", MnemonicColors.KnownColors.MediumTurquoise.Code);
            Assert.AreEqual("#C71585", MnemonicColors.KnownColors.MediumVioletRed.Code);
            Assert.AreEqual("#191970", MnemonicColors.KnownColors.MidnightBlue.Code);
            Assert.AreEqual("#F5FFFA", MnemonicColors.KnownColors.MintCream.Code);
            Assert.AreEqual("#FFE4E1", MnemonicColors.KnownColors.MistyRose.Code);
            Assert.AreEqual("#FFE4B5", MnemonicColors.KnownColors.Moccasin.Code);
            Assert.AreEqual("#FFDEAD", MnemonicColors.KnownColors.NavajoWhite.Code);
            Assert.AreEqual("#000080", MnemonicColors.KnownColors.Navy.Code);
            Assert.AreEqual("#FDF5E6", MnemonicColors.KnownColors.OldLace.Code);
            Assert.AreEqual("#808000", MnemonicColors.KnownColors.Olive.Code);
            Assert.AreEqual("#6B8E23", MnemonicColors.KnownColors.OliveDrab.Code);
            Assert.AreEqual("#FFA500", MnemonicColors.KnownColors.Orange.Code);
            Assert.AreEqual("#FF4500", MnemonicColors.KnownColors.OrangeRed.Code);
            Assert.AreEqual("#DA70D6", MnemonicColors.KnownColors.Orchid.Code);
            Assert.AreEqual("#EEE8AA", MnemonicColors.KnownColors.PaleGoldenrod.Code);
            Assert.AreEqual("#98FB98", MnemonicColors.KnownColors.PaleGreen.Code);
            Assert.AreEqual("#AFEEEE", MnemonicColors.KnownColors.PaleTurquoise.Code);
            Assert.AreEqual("#DB7093", MnemonicColors.KnownColors.PaleVioletRed.Code);
            Assert.AreEqual("#FFEFD5", MnemonicColors.KnownColors.PapayaWhip.Code);
            Assert.AreEqual("#FFDAB9", MnemonicColors.KnownColors.PeachPuff.Code);
            Assert.AreEqual("#CD853F", MnemonicColors.KnownColors.Peru.Code);
            Assert.AreEqual("#FFC0CB", MnemonicColors.KnownColors.Pink.Code);
            Assert.AreEqual("#DDA0DD", MnemonicColors.KnownColors.Plum.Code);
            Assert.AreEqual("#B0E0E6", MnemonicColors.KnownColors.PowderBlue.Code);
            Assert.AreEqual("#800080", MnemonicColors.KnownColors.Purple.Code);
            Assert.AreEqual("#FF0000", MnemonicColors.KnownColors.Red.Code);
            Assert.AreEqual("#BC8F8F", MnemonicColors.KnownColors.RosyBrown.Code);
            Assert.AreEqual("#4169E1", MnemonicColors.KnownColors.RoyalBlue.Code);
            Assert.AreEqual("#8B4513", MnemonicColors.KnownColors.SaddleBrown.Code);
            Assert.AreEqual("#FA8072", MnemonicColors.KnownColors.Salmon.Code);
            Assert.AreEqual("#F4A460", MnemonicColors.KnownColors.SandyBrown.Code);
            Assert.AreEqual("#2E8B57", MnemonicColors.KnownColors.SeaGreen.Code);
            Assert.AreEqual("#FFF5EE", MnemonicColors.KnownColors.SeaShell.Code);
            Assert.AreEqual("#A0522D", MnemonicColors.KnownColors.Sienna.Code);
            Assert.AreEqual("#C0C0C0", MnemonicColors.KnownColors.Silver.Code);
            Assert.AreEqual("#87CEEB", MnemonicColors.KnownColors.SkyBlue.Code);
            Assert.AreEqual("#6A5ACD", MnemonicColors.KnownColors.SlateBlue.Code);
            Assert.AreEqual("#708090", MnemonicColors.KnownColors.SlateGray.Code);
            Assert.AreEqual("#FFFAFA", MnemonicColors.KnownColors.Snow.Code);
            Assert.AreEqual("#00FF7F", MnemonicColors.KnownColors.SpringGreen.Code);
            Assert.AreEqual("#4682B4", MnemonicColors.KnownColors.SteelBlue.Code);
            Assert.AreEqual("#D2B48C", MnemonicColors.KnownColors.Tan.Code);
            Assert.AreEqual("#008080", MnemonicColors.KnownColors.Teal.Code);
            Assert.AreEqual("#D8BFD8", MnemonicColors.KnownColors.Thistle.Code);
            Assert.AreEqual("#FF6347", MnemonicColors.KnownColors.Tomato.Code);
            Assert.AreEqual("#40E0D0", MnemonicColors.KnownColors.Turquoise.Code);
            Assert.AreEqual("#EE82EE", MnemonicColors.KnownColors.Violet.Code);
            Assert.AreEqual("#F5DEB3", MnemonicColors.KnownColors.Wheat.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.KnownColors.White.Code);
            Assert.AreEqual("#F5F5F5", MnemonicColors.KnownColors.WhiteSmoke.Code);
            Assert.AreEqual("#FFFF00", MnemonicColors.KnownColors.Yellow.Code);
            Assert.AreEqual("#9ACD32", MnemonicColors.KnownColors.YellowGreen.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.KnownColors.ButtonFace.Code);
            Assert.AreEqual("#FFFFFF", MnemonicColors.KnownColors.ButtonHighlight.Code);
            Assert.AreEqual("#808080", MnemonicColors.KnownColors.ButtonShadow.Code);
            Assert.AreEqual("#A6CAF0", MnemonicColors.KnownColors.GradientActiveCaption.Code);
            Assert.AreEqual("#C0C0C0", MnemonicColors.KnownColors.GradientInactiveCaption.Code);
            Assert.AreEqual("#D4D0C8", MnemonicColors.KnownColors.MenuBar.Code);
            Assert.AreEqual("#0A246A", MnemonicColors.KnownColors.MenuHighlight.Code);
        }

        [TestMethod]
        public void ResourceColorsTest()
        {
            MnemonicColors colors = ResourceManager.MnemonicColors;

            //Common Names  Asserts
            #region Common Names  Asserts
            var names = new[]
                {
                    "aqua", "морская волна", "cyan", "gray", "серый", "navy", "тёмно-синий", "silver", "серебряный",
                    "black", "чёрный", "green", "зелёный", "olive", "оливковый",
                    "teal", "сине-зелёный", "blue", "синий", "lime", "лайм", "lightgreen", "purple", "пурпурный",
                    "white", "белый", "fuchsia", "фуксия", "magenta", "maroon",
                    "тёмно-бордовый", "red", "красный", "yellow", "жёлтый"
                };
            foreach (string name in names)
            {
                MnemonicColor color = MnemonicColor.Find(name);
                Assert.IsNotNull(color, name);
            }
            #endregion

            #region Votona Product Catalog Asserts

            //Assert.IsNotNull(colors["анилиновый"]);
            //Assert.IsNotNull(colors["индийский"]);
            //Assert.IsNotNull(colors["лазурно серый"]);
            //Assert.IsNotNull(colors["лазурно-серый"]);
            //Assert.IsNotNull(colors["тропический лес"]);
            //Assert.IsNotNull(colors["умеренный цвет орхидеи"]);
            Assert.IsNotNull(colors["зеленовато синий"]);
            Assert.IsNotNull(colors["зеленовато-синий"]);
            Assert.IsNotNull(colors["зеленоватый"]);
            Assert.IsNotNull(colors["зеленые джунгли"]);
            Assert.IsNotNull(colors["коричнево желтый цвета увядших листьев"]);
            Assert.IsNotNull(colors["коричнево-желтый цвета увядших листьев"]);
            Assert.IsNotNull(colors["сигнальный оранжевый"]);

            Assert.IsNotNull(colors["синяя элис"]);
            Assert.IsNotNull(colors["пурпур"]);
            Assert.IsNotNull(colors["морской волны"]);
            Assert.IsNotNull(colors["абрикосовый"]);
            Assert.IsNotNull(colors["абрикосовый крайола"]);
            Assert.IsNotNull(colors["агатовый серый"]);
            Assert.IsNotNull(colors["аквамариновый"]);
            Assert.IsNotNull(colors["аквамариновый крайола"]);
            Assert.IsNotNull(colors["ализариновый красный"]);
            Assert.IsNotNull(colors["алый"]);
            Assert.IsNotNull(colors["амарантово глубоко пурпурный"]);
            Assert.IsNotNull(colors["амарантово пурпурный"]);
            Assert.IsNotNull(colors["амарантово розовый"]);
            Assert.IsNotNull(colors["амарантово-глубоко-пурпурный"]);
            Assert.IsNotNull(colors["амарантово-пурпурный"]);
            Assert.IsNotNull(colors["амарантово-розовый"]);
            Assert.IsNotNull(colors["амарантовый"]);
            Assert.IsNotNull(colors["амарантовый маджента"]);
            Assert.IsNotNull(colors["амарантовый светло вишневый"]);
            Assert.IsNotNull(colors["амарантовый светло-вишневый"]);
            Assert.IsNotNull(colors["американский розовый"]);
            Assert.IsNotNull(colors["аметистовый"]);
            Assert.IsNotNull(colors["анилиновый пурпур"]);
            Assert.IsNotNull(colors["античная латунь"]);
            Assert.IsNotNull(colors["антрацитово серый"]);
            Assert.IsNotNull(colors["антрацитово-серый"]);
            Assert.IsNotNull(colors["арлекин"]);
            Assert.IsNotNull(colors["аспидно серый"]);
            Assert.IsNotNull(colors["аспидно синий"]);
            Assert.IsNotNull(colors["аспидно-серый"]);
            Assert.IsNotNull(colors["аспидно-синий"]);
            Assert.IsNotNull(colors["бабушкины яблоки"]);
            Assert.IsNotNull(colors["базальтово серый"]);
            Assert.IsNotNull(colors["базальтово-серый"]);
            Assert.IsNotNull(colors["баклажаннный крайола"]);
            Assert.IsNotNull(colors["баклажановый"]);
            Assert.IsNotNull(colors["баклажановый крайола"]);
            Assert.IsNotNull(colors["банана мания"]);
            Assert.IsNotNull(colors["барвинок"]);
            Assert.IsNotNull(colors["барвинок крайола"]);
            Assert.IsNotNull(colors["бедра испуганной нимфы"]);
            Assert.IsNotNull(colors["бежево коричневый"]);
            Assert.IsNotNull(colors["бежево красный"]);
            Assert.IsNotNull(colors["бежево серый"]);
            Assert.IsNotNull(colors["бежево-коричневый"]);
            Assert.IsNotNull(colors["бежево-красный"]);
            Assert.IsNotNull(colors["бежево-серый"]);
            Assert.IsNotNull(colors["бежевый"]);
            Assert.IsNotNull(colors["бело алюминиевый"]);
            Assert.IsNotNull(colors["бело зеленый"]);
            Assert.IsNotNull(colors["бело-алюминиевый"]);
            Assert.IsNotNull(colors["бело-зеленый"]);
            Assert.IsNotNull(colors["белоснежный"]);
            Assert.IsNotNull(colors["белый"]);
            Assert.IsNotNull(colors["белый антик"]);
            Assert.IsNotNull(colors["белый навахо"]);
            Assert.IsNotNull(colors["белый-антик"]);
            Assert.IsNotNull(colors["берлинская лазурь"]);
            Assert.IsNotNull(colors["бирюзово голубой крайола"]);
            Assert.IsNotNull(colors["бирюзово зеленый"]);
            Assert.IsNotNull(colors["бирюзово синий"]);
            Assert.IsNotNull(colors["бирюзово-голубой крайола"]);
            Assert.IsNotNull(colors["бирюзово-зеленый"]);
            Assert.IsNotNull(colors["бирюзово-синий"]);
            Assert.IsNotNull(colors["бирюзовый"]);
            Assert.IsNotNull(colors["бисквитный"]);
            Assert.IsNotNull(colors["бистр"]);
            Assert.IsNotNull(colors["бледно васильковый"]);
            Assert.IsNotNull(colors["бледно желтый"]);
            Assert.IsNotNull(colors["бледно зелено серый"]);
            Assert.IsNotNull(colors["бледно зеленый"]);
            Assert.IsNotNull(colors["бледно золотистый"]);
            Assert.IsNotNull(colors["бледно карминный"]);
            Assert.IsNotNull(colors["бледно каштановый"]);
            Assert.IsNotNull(colors["бледно коричневый"]);
            Assert.IsNotNull(colors["бледно песочный"]);
            Assert.IsNotNull(colors["бледно пурпурный"]);
            Assert.IsNotNull(colors["бледно розоватый"]);
            Assert.IsNotNull(colors["бледно розовый"]);
            Assert.IsNotNull(colors["бледно синий"]);
            Assert.IsNotNull(colors["бледно фиолетовый"]);
            Assert.IsNotNull(colors["бледно-васильковый"]);
            Assert.IsNotNull(colors["бледно-желтый"]);
            Assert.IsNotNull(colors["бледно-зелено-серый"]);
            Assert.IsNotNull(colors["бледно-зеленый"]);
            Assert.IsNotNull(colors["бледно-золотистый"]);
            Assert.IsNotNull(colors["бледно-карминный"]);
            Assert.IsNotNull(colors["бледно-каштановый"]);
            Assert.IsNotNull(colors["бледно-коричневый"]);
            Assert.IsNotNull(colors["бледно-песочный"]);
            Assert.IsNotNull(colors["бледно-пурпурный"]);
            Assert.IsNotNull(colors["бледно-розовый"]);
            Assert.IsNotNull(colors["бледно-синий"]);
            Assert.IsNotNull(colors["бледно-фиолетовый"]);
            Assert.IsNotNull(colors["бледный весенний бутон"]);
            Assert.IsNotNull(colors["бледный желтовато зеленый"]);
            Assert.IsNotNull(colors["бледный желтовато розовый"]);
            Assert.IsNotNull(colors["бледный желтовато-зеленый"]);
            Assert.IsNotNull(colors["бледный желтовато-розовый"]);
            Assert.IsNotNull(colors["бледный зеленовато желтый"]);
            Assert.IsNotNull(colors["бледный зеленовато-желтый"]);
            Assert.IsNotNull(colors["бледный зеленый"]);
            Assert.IsNotNull(colors["бледный красновато пурпурный"]);
            Assert.IsNotNull(colors["бледный красновато-пурпурный"]);
            Assert.IsNotNull(colors["бледный оранжево желтый"]);
            Assert.IsNotNull(colors["бледный оранжево-желтый"]);
            Assert.IsNotNull(colors["бледный пурпурно розовый"]);
            Assert.IsNotNull(colors["бледный пурпурно синий"]);
            Assert.IsNotNull(colors["бледный пурпурно-розовый"]);
            Assert.IsNotNull(colors["бледный пурпурно-синий"]);
            Assert.IsNotNull(colors["бледный серо коричневый"]);
            Assert.IsNotNull(colors["бледный серо-коричневый"]);
            Assert.IsNotNull(colors["бледный синий"]);
            Assert.IsNotNull(colors["бледный фиолетово красный"]);
            Assert.IsNotNull(colors["бледный фиолетово-красный"]);
            Assert.IsNotNull(colors["блестящий желто зеленый"]);
            Assert.IsNotNull(colors["блестящий желтовато зеленый"]);
            Assert.IsNotNull(colors["блестящий желтовато-зеленый"]);
            Assert.IsNotNull(colors["блестящий желто-зеленый"]);
            Assert.IsNotNull(colors["блестящий желтый"]);
            Assert.IsNotNull(colors["блестящий зеленовато желтый"]);
            Assert.IsNotNull(colors["блестящий зеленовато синий"]);
            Assert.IsNotNull(colors["блестящий зеленовато-желтый"]);
            Assert.IsNotNull(colors["блестящий зеленовато-синий"]);
            Assert.IsNotNull(colors["блестящий зеленый"]);
            Assert.IsNotNull(colors["блестящий оранжевый"]);
            Assert.IsNotNull(colors["блестящий пурпурно розовый"]);
            Assert.IsNotNull(colors["блестящий пурпурно синий"]);
            Assert.IsNotNull(colors["блестящий пурпурно-розовый"]);
            Assert.IsNotNull(colors["блестящий пурпурно-синий"]);
            Assert.IsNotNull(colors["блестящий пурпурный"]);
            Assert.IsNotNull(colors["блестящий синевато зеленый"]);
            Assert.IsNotNull(colors["блестящий синевато-зеленый"]);
            Assert.IsNotNull(colors["блестящий синий"]);
            Assert.IsNotNull(colors["блестящий фиолетовый"]);
            Assert.IsNotNull(colors["блошиный"]);
            Assert.IsNotNull(colors["бобровый"]);
            Assert.IsNotNull(colors["болгарский розовый"]);
            Assert.IsNotNull(colors["болотный"]);
            Assert.IsNotNull(colors["бордово фиолетовый"]);
            Assert.IsNotNull(colors["бордово-фиолетовый"]);
            Assert.IsNotNull(colors["бордовый"]);
            Assert.IsNotNull(colors["брезентово серый"]);
            Assert.IsNotNull(colors["брезентово-серый"]);
            Assert.IsNotNull(colors["бриллиантово синий"]);
            Assert.IsNotNull(colors["бриллиантово-синий"]);
            Assert.IsNotNull(colors["бриллиантовыый оранжево желтый"]);
            Assert.IsNotNull(colors["бриллиантовыый оранжево-желтый"]);
            Assert.IsNotNull(colors["бронзовый"]);
            Assert.IsNotNull(colors["бургундский"]);
            Assert.IsNotNull(colors["бутылочно зеленый"]);
            Assert.IsNotNull(colors["бутылочно-зеленый"]);
            Assert.IsNotNull(colors["ванильный"]);
            Assert.IsNotNull(colors["васильковый"]);
            Assert.IsNotNull(colors["васильковый крайола"]);
            Assert.IsNotNull(colors["вересково фиолетовый"]);
            Assert.IsNotNull(colors["вересково-фиолетовый"]);
            Assert.IsNotNull(colors["весеннезеленый"]);
            Assert.IsNotNull(colors["весеннезеленый крайола"]);
            Assert.IsNotNull(colors["весенний бутон"]);
            Assert.IsNotNull(colors["византийский"]);
            Assert.IsNotNull(colors["византия"]);
            Assert.IsNotNull(colors["винно красный"]);
            Assert.IsNotNull(colors["винно-красный"]);
            Assert.IsNotNull(colors["винтовочный зеленый"]);
            Assert.IsNotNull(colors["водная синь"]);
            Assert.IsNotNull(colors["воды пляжа бонди"]);
            Assert.IsNotNull(colors["восход солнца"]);
            Assert.IsNotNull(colors["выгоревший оранжевый"]);
            Assert.IsNotNull(colors["галечный серый"]);
            Assert.IsNotNull(colors["гейнсборо"]);
            Assert.IsNotNull(colors["гелиотроп"]);
            Assert.IsNotNull(colors["георгиново желтый"]);
            Assert.IsNotNull(colors["георгиново-желтый"]);
            Assert.IsNotNull(colors["глиняный коричневый"]);
            Assert.IsNotNull(colors["глициния"]);
            Assert.IsNotNull(colors["глициния крайола"]);
            Assert.IsNotNull(colors["глубокая фуксия крайола"]);
            Assert.IsNotNull(colors["глубокий желто зеленый"]);
            Assert.IsNotNull(colors["глубокий желтовато зеленый"]);
            Assert.IsNotNull(colors["глубокий желтовато коричневый"]);
            Assert.IsNotNull(colors["глубокий желтовато розовый"]);
            Assert.IsNotNull(colors["глубокий желтовато-зеленый"]);
            Assert.IsNotNull(colors["глубокий желтовато-коричневый"]);
            Assert.IsNotNull(colors["глубокий желтовато-розовый"]);
            Assert.IsNotNull(colors["глубокий желто-зеленый"]);
            Assert.IsNotNull(colors["глубокий желтый"]);
            Assert.IsNotNull(colors["глубокий зеленовато желтый"]);
            Assert.IsNotNull(colors["глубокий зеленовато-желтый"]);
            Assert.IsNotNull(colors["глубокий зеленый"]);
            Assert.IsNotNull(colors["глубокий карминно розовый"]);
            Assert.IsNotNull(colors["глубокий карминно-розовый"]);
            Assert.IsNotNull(colors["глубокий карминный"]);
            Assert.IsNotNull(colors["глубокий каштановый крайола"]);
            Assert.IsNotNull(colors["глубокий коралловый"]);
            Assert.IsNotNull(colors["глубокий коричневый"]);
            Assert.IsNotNull(colors["глубокий красновато коричневый"]);
            Assert.IsNotNull(colors["глубокий красновато оранжевый"]);
            Assert.IsNotNull(colors["глубокий красновато пурпурный"]);
            Assert.IsNotNull(colors["глубокий красновато-коричневый"]);
            Assert.IsNotNull(colors["глубокий красновато-оранжевый"]);
            Assert.IsNotNull(colors["глубокий красновато-пурпурный"]);
            Assert.IsNotNull(colors["глубокий красный"]);
            Assert.IsNotNull(colors["глубокий оливково зеленый"]);
            Assert.IsNotNull(colors["глубокий оливково-зеленый"]);
            Assert.IsNotNull(colors["глубокий оранжево желтый"]);
            Assert.IsNotNull(colors["глубокий оранжево-желтый"]);
            Assert.IsNotNull(colors["глубокий оранжевый"]);
            Assert.IsNotNull(colors["глубокий пурпурно красный"]);
            Assert.IsNotNull(colors["глубокий пурпурно розовый"]);
            Assert.IsNotNull(colors["глубокий пурпурно синий"]);
            Assert.IsNotNull(colors["глубокий пурпурно-красный"]);
            Assert.IsNotNull(colors["глубокий пурпурно-розовый"]);
            Assert.IsNotNull(colors["глубокий пурпурно-синий"]);
            Assert.IsNotNull(colors["глубокий пурпурный"]);
            Assert.IsNotNull(colors["глубокий розовый"]);
            Assert.IsNotNull(colors["глубокий синевато зеленый"]);
            Assert.IsNotNull(colors["глубокий синевато-зеленый"]);
            Assert.IsNotNull(colors["глубокий синий"]);
            Assert.IsNotNull(colors["глубокий фиолетово черный"]);
            Assert.IsNotNull(colors["глубокий фиолетово-черный"]);
            Assert.IsNotNull(colors["глубокий фиолетовый"]);
            Assert.IsNotNull(colors["голубино синий"]);
            Assert.IsNotNull(colors["голубино-синий"]);
            Assert.IsNotNull(colors["голубой"]);
            Assert.IsNotNull(colors["голубой колокольчик крайола"]);
            Assert.IsNotNull(colors["голубой крайола"]);
            Assert.IsNotNull(colors["горечавково синий"]);
            Assert.IsNotNull(colors["горечавково-синий"]);
            Assert.IsNotNull(colors["горный луг"]);
            Assert.IsNotNull(colors["горчичный"]);
            Assert.IsNotNull(colors["горько сладкий"]);
            Assert.IsNotNull(colors["горько-сладкий"]);
            Assert.IsNotNull(colors["гранатовый"]);
            Assert.IsNotNull(colors["гранитный"]);
            Assert.IsNotNull(colors["гранитовый серый"]);
            Assert.IsNotNull(colors["графитно черный"]);
            Assert.IsNotNull(colors["графитно-черный"]);
            Assert.IsNotNull(colors["графитовый серый"]);
            Assert.IsNotNull(colors["грузинский розовый"]);
            Assert.IsNotNull(colors["грушево зеленый"]);
            Assert.IsNotNull(colors["грушево-зеленый"]);
            Assert.IsNotNull(colors["грушевый"]);
            Assert.IsNotNull(colors["гуммигут"]);
            Assert.IsNotNull(colors["гусеница"]);
            Assert.IsNotNull(colors["дартмутский зеленый"]);
            Assert.IsNotNull(colors["джазовый джем"]);
            Assert.IsNotNull(colors["джинсовый синий"]);
            Assert.IsNotNull(colors["дикая клубника крайола"]);
            Assert.IsNotNull(colors["дикий арбуз крайола"]);
            Assert.IsNotNull(colors["дикий синий крайола"]);
            Assert.IsNotNull(colors["дымчато белый"]);
            Assert.IsNotNull(colors["дымчато-белый"]);
            Assert.IsNotNull(colors["дынно желтый"]);
            Assert.IsNotNull(colors["дынно-желтый"]);
            Assert.IsNotNull(colors["дыня крайола"]);
            Assert.IsNotNull(colors["железно серый"]);
            Assert.IsNotNull(colors["железно-серый"]);
            Assert.IsNotNull(colors["желтая сера"]);
            Assert.IsNotNull(colors["желтая слоновая кость"]);
            Assert.IsNotNull(colors["желто зеленый"]);
            Assert.IsNotNull(colors["желто зеленый крайола"]);
            Assert.IsNotNull(colors["желто золотой"]);
            Assert.IsNotNull(colors["желто оливковый"]);
            Assert.IsNotNull(colors["желто оранжевый"]);
            Assert.IsNotNull(colors["желто оранжевый крайола"]);
            Assert.IsNotNull(colors["желто персиковый"]);
            Assert.IsNotNull(colors["желто серый"]);
            Assert.IsNotNull(colors["желтовато белый"]);
            Assert.IsNotNull(colors["желтовато серый"]);
            Assert.IsNotNull(colors["желтовато-белый"]);
            Assert.IsNotNull(colors["желтовато-серый"]);
            Assert.IsNotNull(colors["желто-зеленый"]);
            Assert.IsNotNull(colors["желто-зеленый крайола"]);
            Assert.IsNotNull(colors["желто-золотой"]);
            Assert.IsNotNull(colors["желто-оливковый"]);
            Assert.IsNotNull(colors["желто-оранжевый"]);
            Assert.IsNotNull(colors["желто-оранжевый крайола"]);
            Assert.IsNotNull(colors["желто-персиковый"]);
            Assert.IsNotNull(colors["желто-серый"]);
            Assert.IsNotNull(colors["желтый"]);
            Assert.IsNotNull(colors["желтый крайола"]);
            Assert.IsNotNull(colors["желтый ракитник"]);
            Assert.IsNotNull(colors["жемчужно белый"]);
            Assert.IsNotNull(colors["жемчужно-белый"]);
            Assert.IsNotNull(colors["жженый апельсин"]);
            Assert.IsNotNull(colors["защитно синий"]);
            Assert.IsNotNull(colors["защитно-синий"]);
            Assert.IsNotNull(colors["защитный"]);
            Assert.IsNotNull(colors["звезды в шоке"]);
            Assert.IsNotNull(colors["зеленая весна"]);
            Assert.IsNotNull(colors["зеленая лужайка"]);
            Assert.IsNotNull(colors["зеленая сосна"]);
            Assert.IsNotNull(colors["зеленая сосна крайола"]);
            Assert.IsNotNull(colors["зелено бежевый"]);
            Assert.IsNotNull(colors["зелено желтый"]);
            Assert.IsNotNull(colors["зелено желтый крайола"]);
            Assert.IsNotNull(colors["зелено коричневый"]);
            Assert.IsNotNull(colors["зелено лаймовый"]);
            Assert.IsNotNull(colors["зелено серый"]);
            Assert.IsNotNull(colors["зелено синий"]);
            Assert.IsNotNull(colors["зелено синий крайола"]);
            Assert.IsNotNull(colors["зелено-бежевый"]);
            Assert.IsNotNull(colors["зеленовато белый"]);
            Assert.IsNotNull(colors["зеленовато серый"]);
            Assert.IsNotNull(colors["зеленовато черный"]);
            Assert.IsNotNull(colors["зеленовато-белый"]);
            Assert.IsNotNull(colors["зеленовато-серый"]);
            Assert.IsNotNull(colors["зеленовато-черный"]);
            Assert.IsNotNull(colors["зеленое море"]);
            Assert.IsNotNull(colors["зелено-желтый"]);
            Assert.IsNotNull(colors["зелено-желтый крайола"]);
            Assert.IsNotNull(colors["зелено-коричневый"]);
            Assert.IsNotNull(colors["зелено-лаймовый"]);
            Assert.IsNotNull(colors["зелено-серый"]);
            Assert.IsNotNull(colors["зелено-синий"]);
            Assert.IsNotNull(colors["зелено-синий крайола"]);
            Assert.IsNotNull(colors["зеленые джунгли крайола"]);
            Assert.IsNotNull(colors["зеленый"]);
            Assert.IsNotNull(colors["зелёный"]);
            Assert.IsNotNull(colors["зеленый крайола"]);
            Assert.IsNotNull(colors["зеленый лишайник"]);
            Assert.IsNotNull(colors["зеленый мох"]);
            Assert.IsNotNull(colors["зеленый орел"]);
            Assert.IsNotNull(colors["зеленый папоротник"]);
            Assert.IsNotNull(colors["зеленый темный"]);
            Assert.IsNotNull(colors["зеленый трилистник"]);
            Assert.IsNotNull(colors["зеленый чай"]);
            Assert.IsNotNull(colors["золотарник крайола"]);
            Assert.IsNotNull(colors["золотисто березовый"]);
            Assert.IsNotNull(colors["золотисто каштановый"]);
            Assert.IsNotNull(colors["золотисто-березовый"]);
            Assert.IsNotNull(colors["золотисто-каштановый"]);
            Assert.IsNotNull(colors["золотой"]);
            Assert.IsNotNull(colors["золотой крайола"]);
            Assert.IsNotNull(colors["ивово коричневый"]);
            Assert.IsNotNull(colors["ивово-коричневый"]);
            Assert.IsNotNull(colors["известковая глина"]);
            Assert.IsNotNull(colors["изумрудно зеленый"]);
            Assert.IsNotNull(colors["изумрудно-зеленый"]);
            Assert.IsNotNull(colors["изумрудный"]);
            Assert.IsNotNull(colors["индиго"]);
            Assert.IsNotNull(colors["индиго крайола"]);
            Assert.IsNotNull(colors["индийский зеленый"]);
            Assert.IsNotNull(colors["индийский красный"]);
            Assert.IsNotNull(colors["ирландский зеленый"]);
            Assert.IsNotNull(colors["июньский бутон"]);
            Assert.IsNotNull(colors["каменно серый"]);
            Assert.IsNotNull(colors["каменно-серый"]);
            Assert.IsNotNull(colors["канареечный"]);
            Assert.IsNotNull(colors["капри синий"]);
            Assert.IsNotNull(colors["каракатица"]);
            Assert.IsNotNull(colors["кардинал"]);
            Assert.IsNotNull(colors["карибский зеленый"]);
            Assert.IsNotNull(colors["кармин"]);
            Assert.IsNotNull(colors["карминно красный"]);
            Assert.IsNotNull(colors["карминно розовый"]);
            Assert.IsNotNull(colors["карминно-красный"]);
            Assert.IsNotNull(colors["карминно-розовый"]);
            Assert.IsNotNull(colors["карри желтый"]);
            Assert.IsNotNull(colors["каштаново коричневый"]);
            Assert.IsNotNull(colors["каштаново-коричневый"]);
            Assert.IsNotNull(colors["каштановый"]);
            Assert.IsNotNull(colors["каштановый крайола"]);
            Assert.IsNotNull(colors["кварцевый"]);
            Assert.IsNotNull(colors["кварцевый серый"]);
            Assert.IsNotNull(colors["киноварь"]);
            Assert.IsNotNull(colors["кирпично красный"]);
            Assert.IsNotNull(colors["кирпично-красный"]);
            Assert.IsNotNull(colors["кирпичный"]);
            Assert.IsNotNull(colors["китайский красный"]);
            Assert.IsNotNull(colors["кленовый зеленый"]);
            Assert.IsNotNull(colors["клубнично красный"]);
            Assert.IsNotNull(colors["клубнично-красный"]);
            Assert.IsNotNull(colors["кобальт синий"]);
            Assert.IsNotNull(colors["кобальтово синий"]);
            Assert.IsNotNull(colors["кобальтово-синий"]);
            Assert.IsNotNull(colors["кожа буйвола"]);
            Assert.IsNotNull(colors["кожура апельсина"]);
            Assert.IsNotNull(colors["кожура-апельсина"]);
            Assert.IsNotNull(colors["кораллово красный"]);
            Assert.IsNotNull(colors["кораллово-красный"]);
            Assert.IsNotNull(colors["коралловый"]);
            Assert.IsNotNull(colors["кордованский"]);
            Assert.IsNotNull(colors["коричневато оранжевый"]);
            Assert.IsNotNull(colors["коричневато розовый"]);
            Assert.IsNotNull(colors["коричневато серый"]);
            Assert.IsNotNull(colors["коричневато черный"]);
            Assert.IsNotNull(colors["коричневато-оранжевый"]);
            Assert.IsNotNull(colors["коричневато-розовый"]);
            Assert.IsNotNull(colors["коричневато-серый"]);
            Assert.IsNotNull(colors["коричневато-черный"]);
            Assert.IsNotNull(colors["коричнево бежевый"]);
            Assert.IsNotNull(colors["коричнево бордовый"]);
            Assert.IsNotNull(colors["коричнево зеленый"]);
            Assert.IsNotNull(colors["коричнево красный"]);
            Assert.IsNotNull(colors["коричнево малиновый"]);
            Assert.IsNotNull(colors["коричнево малиновый крайола"]);
            Assert.IsNotNull(colors["коричнево оливковый"]);
            Assert.IsNotNull(colors["коричнево серый"]);
            Assert.IsNotNull(colors["коричнево-бежевый"]);
            Assert.IsNotNull(colors["коричнево-бордовый"]);
            Assert.IsNotNull(colors["коричнево-зеленый"]);
            Assert.IsNotNull(colors["коричнево-красный"]);
            Assert.IsNotNull(colors["коричнево-малиновый"]);
            Assert.IsNotNull(colors["коричнево-малиновый крайола"]);
            Assert.IsNotNull(colors["коричнево-оливковый"]);
            Assert.IsNotNull(colors["коричнево-серый"]);
            Assert.IsNotNull(colors["коричневый"]);
            Assert.IsNotNull(colors["коричневый крайола"]);
            Assert.IsNotNull(colors["коричневый темный"]);
            Assert.IsNotNull(colors["коричный"]);
            Assert.IsNotNull(colors["королевская фуксия"]);
            Assert.IsNotNull(colors["королевский пурпурный крайола"]);
            Assert.IsNotNull(colors["королевский синий"]);
            Assert.IsNotNull(colors["космические сливки"]);
            Assert.IsNotNull(colors["космос"]);
            Assert.IsNotNull(colors["кофейный"]);
            Assert.IsNotNull(colors["крайоловый абрикос"]);
            Assert.IsNotNull(colors["красно желтовато коричневый"]);
            Assert.IsNotNull(colors["красно коричневый"]);
            Assert.IsNotNull(colors["красно оранжевый"]);
            Assert.IsNotNull(colors["красно оранжевый крайола"]);
            Assert.IsNotNull(colors["красно сиреневый"]);
            Assert.IsNotNull(colors["красно фиолетовый"]);
            Assert.IsNotNull(colors["красно фиолетовый крайола"]);
            Assert.IsNotNull(colors["красновато коричневый"]);
            Assert.IsNotNull(colors["красновато серый"]);
            Assert.IsNotNull(colors["красновато черный"]);
            Assert.IsNotNull(colors["красновато-коричневый"]);
            Assert.IsNotNull(colors["красновато-серый"]);
            Assert.IsNotNull(colors["красновато-черный"]);
            Assert.IsNotNull(colors["красное дерево"]);
            Assert.IsNotNull(colors["красное дерево крайола"]);
            Assert.IsNotNull(colors["красно-желтовато-коричневый"]);
            Assert.IsNotNull(colors["красно-коричневый"]);
            Assert.IsNotNull(colors["красно-оранжевый"]);
            Assert.IsNotNull(colors["красно-оранжевый крайола"]);
            Assert.IsNotNull(colors["красно-сиреневый"]);
            Assert.IsNotNull(colors["красно-фиолетовый"]);
            Assert.IsNotNull(colors["красно-фиолетовый крайола"]);
            Assert.IsNotNull(colors["красный"]);
            Assert.IsNotNull(colors["красный крайола"]);
            Assert.IsNotNull(colors["красный песок"]);
            Assert.IsNotNull(colors["кремово желтый"]);
            Assert.IsNotNull(colors["кремово-желтый"]);
            Assert.IsNotNull(colors["кремовый"]);
            Assert.IsNotNull(colors["кричащий зеленый"]);
            Assert.IsNotNull(colors["крутой розовый крайола"]);
            Assert.IsNotNull(colors["кукурузно желтый"]);
            Assert.IsNotNull(colors["кукурузно-желтый"]);
            Assert.IsNotNull(colors["кукурузный"]);
            Assert.IsNotNull(colors["лаванда"]);
            Assert.IsNotNull(colors["лавандовый крайола"]);
            Assert.IsNotNull(colors["лавандовый розовый"]);
            Assert.IsNotNull(colors["лазерный лимон"]);
            Assert.IsNotNull(colors["лазурно синий"]);
            Assert.IsNotNull(colors["лазурно-синий"]);
            Assert.IsNotNull(colors["лазурный"]);
            Assert.IsNotNull(colors["лазурный крайола"]);
            Assert.IsNotNull(colors["лайм"]);
            Assert.IsNotNull(colors["лаймово зеленый"]);
            Assert.IsNotNull(colors["лаймово-зеленый"]);
            Assert.IsNotNull(colors["ламантин"]);
            Assert.IsNotNull(colors["латунный"]);
            Assert.IsNotNull(colors["лесной волк"]);
            Assert.IsNotNull(colors["лесной зеленый"]);
            Assert.IsNotNull(colors["ливерный"]);
            Assert.IsNotNull(colors["лиловый"]);
            Assert.IsNotNull(colors["лимонно желтый"]);
            Assert.IsNotNull(colors["лимонно желтый крайола"]);
            Assert.IsNotNull(colors["лимонно кремовый"]);
            Assert.IsNotNull(colors["лимонно-желтый"]);
            Assert.IsNotNull(colors["лимонно-желтый крайола"]);
            Assert.IsNotNull(colors["лимонно-кремовый"]);
            Assert.IsNotNull(colors["лимонный"]);
            Assert.IsNotNull(colors["лиственно зеленый"]);
            Assert.IsNotNull(colors["лиственно-зеленый"]);
            Assert.IsNotNull(colors["лиственный зеленый крайола"]);
            Assert.IsNotNull(colors["лососево красный"]);
            Assert.IsNotNull(colors["лососево оранжевый"]);
            Assert.IsNotNull(colors["лососево-красный"]);
            Assert.IsNotNull(colors["лососево-оранжевый"]);
            Assert.IsNotNull(colors["лососевый"]);
            Assert.IsNotNull(colors["лососевый крайола"]);
            Assert.IsNotNull(colors["льняной"]);
            Assert.IsNotNull(colors["люминесцентный красный"]);
            Assert.IsNotNull(colors["люминесцентный ярко оранжевый"]);
            Assert.IsNotNull(colors["люминесцентный ярко-оранжевый"]);
            Assert.IsNotNull(colors["лягушки в обмороке"]);
            Assert.IsNotNull(colors["магическая мята"]);
            Assert.IsNotNull(colors["магнолия"]);
            Assert.IsNotNull(colors["маджента"]);
            Assert.IsNotNull(colors["маджента крайола"]);
            Assert.IsNotNull(colors["майский зеленый"]);
            Assert.IsNotNull(colors["маисовый"]);
            Assert.IsNotNull(colors["макароны и сыр"]);
            Assert.IsNotNull(colors["малахитовый"]);
            Assert.IsNotNull(colors["малиново красный"]);
            Assert.IsNotNull(colors["малиново розовый"]);
            Assert.IsNotNull(colors["малиново-красный"]);
            Assert.IsNotNull(colors["малиново-розовый"]);
            Assert.IsNotNull(colors["малиновый"]);
            Assert.IsNotNull(colors["манго танго"]);
            Assert.IsNotNull(colors["манго-танго"]);
            Assert.IsNotNull(colors["мандариновый"]);
            Assert.IsNotNull(colors["маренго"]);
            Assert.IsNotNull(colors["махагон коричневый"]);
            Assert.IsNotNull(colors["медно коричневый"]);
            Assert.IsNotNull(colors["медно розовый"]);
            Assert.IsNotNull(colors["медно-коричневый"]);
            Assert.IsNotNull(colors["медно-розовый"]);
            Assert.IsNotNull(colors["медный"]);
            Assert.IsNotNull(colors["медный крайола"]);
            Assert.IsNotNull(colors["медово желтый"]);
            Assert.IsNotNull(colors["медово-желтый"]);
            Assert.IsNotNull(colors["медовый"]);
            Assert.IsNotNull(colors["мертвенный индиго"]);
            Assert.IsNotNull(colors["миндаль крайола"]);
            Assert.IsNotNull(colors["миртовый"]);
            Assert.IsNotNull(colors["мовеин"]);
            Assert.IsNotNull(colors["модная фуксия"]);
            Assert.IsNotNull(colors["мокасиновый"]);
            Assert.IsNotNull(colors["мокрый тропический лес"]);
            Assert.IsNotNull(colors["морковный"]);
            Assert.IsNotNull(colors["морской зеленый"]);
            Assert.IsNotNull(colors["морской зеленый крайола"]);
            Assert.IsNotNull(colors["мурена"]);
            Assert.IsNotNull(colors["мусульманский зеленый"]);
            Assert.IsNotNull(colors["мышино серый"]);
            Assert.IsNotNull(colors["мышино-серый"]);
            Assert.IsNotNull(colors["мятно бирюзовый"]);
            Assert.IsNotNull(colors["мятно зеленый"]);
            Assert.IsNotNull(colors["мятно кремовый"]);
            Assert.IsNotNull(colors["мятно-бирюзовый"]);
            Assert.IsNotNull(colors["мятно-зеленый"]);
            Assert.IsNotNull(colors["мятно-кремовый"]);
            Assert.IsNotNull(colors["нарциссово желтый"]);
            Assert.IsNotNull(colors["нарциссово-желтый"]);
            Assert.IsNotNull(colors["насыщенный"]);
            Assert.IsNotNull(colors["насыщенный желто зеленый"]);
            Assert.IsNotNull(colors["насыщенный желтовато зеленый"]);
            Assert.IsNotNull(colors["насыщенный желтовато коричневый"]);
            Assert.IsNotNull(colors["насыщенный желтовато розовый"]);
            Assert.IsNotNull(colors["насыщенный желтовато-зеленый"]);
            Assert.IsNotNull(colors["насыщенный желтовато-коричневый"]);
            Assert.IsNotNull(colors["насыщенный желтовато-розовый"]);
            Assert.IsNotNull(colors["насыщенный желто-зеленый"]);
            Assert.IsNotNull(colors["насыщенный желтый"]);
            Assert.IsNotNull(colors["насыщенный зеленовато желтый"]);
            Assert.IsNotNull(colors["насыщенный зеленовато синий"]);
            Assert.IsNotNull(colors["насыщенный зеленовато-желтый"]);
            Assert.IsNotNull(colors["насыщенный зеленовато-синий"]);
            Assert.IsNotNull(colors["насыщенный зеленый"]);
            Assert.IsNotNull(colors["насыщенный коричневый"]);
            Assert.IsNotNull(colors["насыщенный красновато коричневый"]);
            Assert.IsNotNull(colors["насыщенный красновато оранжевый"]);
            Assert.IsNotNull(colors["насыщенный красновато пурпурный"]);
            Assert.IsNotNull(colors["насыщенный красновато-коричневый"]);
            Assert.IsNotNull(colors["насыщенный красновато-оранжевый"]);
            Assert.IsNotNull(colors["насыщенный красновато-пурпурный"]);
            Assert.IsNotNull(colors["насыщенный красный"]);
            Assert.IsNotNull(colors["насыщенный оливково зеленый"]);
            Assert.IsNotNull(colors["насыщенный оливково-зеленый"]);
            Assert.IsNotNull(colors["насыщенный оранжево желтый"]);
            Assert.IsNotNull(colors["насыщенный оранжево-желтый"]);
            Assert.IsNotNull(colors["насыщенный оранжевый"]);
            Assert.IsNotNull(colors["насыщенный пурпурно красный"]);
            Assert.IsNotNull(colors["насыщенный пурпурно розовый"]);
            Assert.IsNotNull(colors["насыщенный пурпурно синий"]);
            Assert.IsNotNull(colors["насыщенный пурпурно-красный"]);
            Assert.IsNotNull(colors["насыщенный пурпурно-розовый"]);
            Assert.IsNotNull(colors["насыщенный пурпурно-синий"]);
            Assert.IsNotNull(colors["насыщенный розовый"]);
            Assert.IsNotNull(colors["насыщенный синевато зеленый"]);
            Assert.IsNotNull(colors["насыщенный синевато-зеленый"]);
            Assert.IsNotNull(colors["насыщенный синий"]);
            Assert.IsNotNull(colors["насыщенный фиолетовый"]);
            Assert.IsNotNull(colors["натуральная умбра"]);
            Assert.IsNotNull(colors["небесная лазурь"]);
            Assert.IsNotNull(colors["небесно синий"]);
            Assert.IsNotNull(colors["небесно-синий"]);
            Assert.IsNotNull(colors["небесный"]);
            Assert.IsNotNull(colors["нежно оливковый"]);
            Assert.IsNotNull(colors["нежно-оливковый"]);
            Assert.IsNotNull(colors["незрелый желтый"]);
            Assert.IsNotNull(colors["неоново морковный"]);
            Assert.IsNotNull(colors["неоново-морковный"]);
            Assert.IsNotNull(colors["нефритовый"]);
            Assert.IsNotNull(colors["ниагара"]);
            Assert.IsNotNull(colors["ночной синий"]);
            Assert.IsNotNull(colors["обычный весенний бутон"]);
            Assert.IsNotNull(colors["огненная сиенна крайола"]);
            Assert.IsNotNull(colors["огненно красный"]);
            Assert.IsNotNull(colors["огненно-красный"]);
            Assert.IsNotNull(colors["огненный оранжевый"]);
            Assert.IsNotNull(colors["одуванчиковый"]);
            Assert.IsNotNull(colors["океанская синь"]);
            Assert.IsNotNull(colors["оксид красный"]);
            Assert.IsNotNull(colors["олень коричневый"]);
            Assert.IsNotNull(colors["оливково желтый"]);
            Assert.IsNotNull(colors["оливково зеленый"]);
            Assert.IsNotNull(colors["оливково зеленый крайола"]);
            Assert.IsNotNull(colors["оливково коричневый"]);
            Assert.IsNotNull(colors["оливково серый"]);
            Assert.IsNotNull(colors["оливково черный"]);
            Assert.IsNotNull(colors["оливково-желтый"]);
            Assert.IsNotNull(colors["оливково-зеленый"]);
            Assert.IsNotNull(colors["оливково-зеленый крайола"]);
            Assert.IsNotNull(colors["оливково-коричневый"]);
            Assert.IsNotNull(colors["оливково-серый"]);
            Assert.IsNotNull(colors["оливково-черный"]);
            Assert.IsNotNull(colors["оливковый"]);
            Assert.IsNotNull(colors["опаловый зеленый"]);
            Assert.IsNotNull(colors["оперный розовато лиловый"]);
            Assert.IsNotNull(colors["оперный розовато-лиловый"]);
            Assert.IsNotNull(colors["оранжевая заря"]);
            Assert.IsNotNull(colors["оранжево желтый крайола"]);
            Assert.IsNotNull(colors["оранжево коричневый"]);
            Assert.IsNotNull(colors["оранжево красный крайола"]);
            Assert.IsNotNull(colors["оранжево персиковый"]);
            Assert.IsNotNull(colors["оранжево розовый"]);
            Assert.IsNotNull(colors["оранжево-желтый крайола"]);
            Assert.IsNotNull(colors["оранжево-коричневый"]);
            Assert.IsNotNull(colors["оранжево-красный крайола"]);
            Assert.IsNotNull(colors["оранжево-персиковый"]);
            Assert.IsNotNull(colors["оранжево-розовый"]);
            Assert.IsNotNull(colors["оранжевый"]);
            Assert.IsNotNull(colors["оранжевый крайола"]);
            Assert.IsNotNull(colors["орехово коричневый"]);
            Assert.IsNotNull(colors["орехово-коричневый"]);
            Assert.IsNotNull(colors["ориент красный"]);
            Assert.IsNotNull(colors["орхидея"]);
            Assert.IsNotNull(colors["орхидея крайола"]);
            Assert.IsNotNull(colors["отборный желтый"]);
            Assert.IsNotNull(colors["отдаленно синий"]);
            Assert.IsNotNull(colors["отдаленно-синий"]);
            Assert.IsNotNull(colors["охотничий зеленый"]);
            Assert.IsNotNull(colors["охра"]);
            Assert.IsNotNull(colors["охра желтая"]);
            Assert.IsNotNull(colors["охра коричневая"]);
            Assert.IsNotNull(colors["очень бледно пурпурный"]);
            Assert.IsNotNull(colors["очень бледно-пурпурный"]);
            Assert.IsNotNull(colors["очень бледный зеленый"]);
            Assert.IsNotNull(colors["очень бледный пурпурно синий"]);
            Assert.IsNotNull(colors["очень бледный пурпурно-синий"]);
            Assert.IsNotNull(colors["очень бледный синий"]);
            Assert.IsNotNull(colors["очень бледный фиолетовый"]);
            Assert.IsNotNull(colors["очень глубокий желтовато зеленый"]);
            Assert.IsNotNull(colors["очень глубокий желтовато-зеленый"]);
            Assert.IsNotNull(colors["очень глубокий красновато пурпурный"]);
            Assert.IsNotNull(colors["очень глубокий красновато-пурпурный"]);
            Assert.IsNotNull(colors["очень глубокий красный"]);
            Assert.IsNotNull(colors["очень глубокий пурпурно красный"]);
            Assert.IsNotNull(colors["очень глубокий пурпурно-красный"]);
            Assert.IsNotNull(colors["очень глубокий пурпурный"]);
            Assert.IsNotNull(colors["очень пурпурно синий"]);
            Assert.IsNotNull(colors["очень пурпурно-синий"]);
            Assert.IsNotNull(colors["очень светло пурпурный"]);
            Assert.IsNotNull(colors["очень светло-пурпурный"]);
            Assert.IsNotNull(colors["очень светлый желтовато зеленый"]);
            Assert.IsNotNull(colors["очень светлый желтовато-зеленый"]);
            Assert.IsNotNull(colors["очень светлый зеленовато синий"]);
            Assert.IsNotNull(colors["очень светлый зеленовато-синий"]);
            Assert.IsNotNull(colors["очень светлый зеленый"]);
            Assert.IsNotNull(colors["очень светлый пурпурно синий"]);
            Assert.IsNotNull(colors["очень светлый пурпурно-синий"]);
            Assert.IsNotNull(colors["очень светлый синевато зеленый"]);
            Assert.IsNotNull(colors["очень светлый синевато-зеленый"]);
            Assert.IsNotNull(colors["очень светлый синий"]);
            Assert.IsNotNull(colors["очень светлый фиолетовый"]);
            Assert.IsNotNull(colors["очень темно пурпурный"]);
            Assert.IsNotNull(colors["очень темно-пурпурный"]);
            Assert.IsNotNull(colors["очень темный алый"]);
            Assert.IsNotNull(colors["очень темный желтовато зеленый"]);
            Assert.IsNotNull(colors["очень темный желтовато-зеленый"]);
            Assert.IsNotNull(colors["очень темный зеленовато синий"]);
            Assert.IsNotNull(colors["очень темный зеленовато-синий"]);
            Assert.IsNotNull(colors["очень темный зеленый"]);
            Assert.IsNotNull(colors["очень темный красновато пурпурный"]);
            Assert.IsNotNull(colors["очень темный красновато-пурпурный"]);
            Assert.IsNotNull(colors["очень темный красный"]);
            Assert.IsNotNull(colors["очень темный оливковый"]);
            Assert.IsNotNull(colors["очень темный пурпурно красный"]);
            Assert.IsNotNull(colors["очень темный пурпурно-красный"]);
            Assert.IsNotNull(colors["очень темный синевато зеленый"]);
            Assert.IsNotNull(colors["очень темный синевато-зеленый"]);
            Assert.IsNotNull(colors["очищенный миндаль"]);
            Assert.IsNotNull(colors["палевый"]);
            Assert.IsNotNull(colors["панг"]);
            Assert.IsNotNull(colors["папоротник крайола"]);
            Assert.IsNotNull(colors["папоротниково зеленый"]);
            Assert.IsNotNull(colors["папоротниково-зеленый"]);
            Assert.IsNotNull(colors["пастельно бирюзовый"]);
            Assert.IsNotNull(colors["пастельно желтый"]);
            Assert.IsNotNull(colors["пастельно зеленый"]);
            Assert.IsNotNull(colors["пастельно оранжевый"]);
            Assert.IsNotNull(colors["пастельно розовый"]);
            Assert.IsNotNull(colors["пастельно синий"]);
            Assert.IsNotNull(colors["пастельно фиолетовый"]);
            Assert.IsNotNull(colors["пастельно-бирюзовый"]);
            Assert.IsNotNull(colors["пастельно-желтый"]);
            Assert.IsNotNull(colors["пастельно-зеленый"]);
            Assert.IsNotNull(colors["пастельно-оранжевый"]);
            Assert.IsNotNull(colors["пастельно-розовый"]);
            Assert.IsNotNull(colors["пастельно-синий"]);
            Assert.IsNotNull(colors["пастельно-фиолетовый"]);
            Assert.IsNotNull(colors["патиново зеленый"]);
            Assert.IsNotNull(colors["патиново-зеленый"]);
            Assert.IsNotNull(colors["перванш"]);
            Assert.IsNotNull(colors["перекатиполе"]);
            Assert.IsNotNull(colors["перламутрово бежевый"]);
            Assert.IsNotNull(colors["перламутрово ежевичный"]);
            Assert.IsNotNull(colors["перламутрово зеленый"]);
            Assert.IsNotNull(colors["перламутрово золотой"]);
            Assert.IsNotNull(colors["перламутрово оранжевый"]);
            Assert.IsNotNull(colors["перламутрово розовый"]);
            Assert.IsNotNull(colors["перламутрово рубиновый"]);
            Assert.IsNotNull(colors["перламутрово фиолетовый"]);
            Assert.IsNotNull(colors["перламутрово-бежевый"]);
            Assert.IsNotNull(colors["перламутрово-ежевичный"]);
            Assert.IsNotNull(colors["перламутрово-зеленый"]);
            Assert.IsNotNull(colors["перламутрово-золотой"]);
            Assert.IsNotNull(colors["перламутрово-оранжевый"]);
            Assert.IsNotNull(colors["перламутрово-розовый"]);
            Assert.IsNotNull(colors["перламутрово-рубиновый"]);
            Assert.IsNotNull(colors["перламутрово-фиолетовый"]);
            Assert.IsNotNull(colors["перламутровый горечавково синий"]);
            Assert.IsNotNull(colors["перламутровый горечавково-синий"]);
            Assert.IsNotNull(colors["перламутровый медный"]);
            Assert.IsNotNull(colors["перламутровый мышино серый"]);
            Assert.IsNotNull(colors["перламутровый мышино-серый"]);
            Assert.IsNotNull(colors["перламутровый ночной"]);
            Assert.IsNotNull(colors["перламутровый опаловый"]);
            Assert.IsNotNull(colors["перламутровый светло серый"]);
            Assert.IsNotNull(colors["перламутровый светло-серый"]);
            Assert.IsNotNull(colors["перламутровый темно серый"]);
            Assert.IsNotNull(colors["перламутровый темно-серый"]);
            Assert.IsNotNull(colors["персидский зеленый"]);
            Assert.IsNotNull(colors["персидский индиго"]);
            Assert.IsNotNull(colors["персидский красный"]);
            Assert.IsNotNull(colors["персидский розовый"]);
            Assert.IsNotNull(colors["персидский синий"]);
            Assert.IsNotNull(colors["персиковый"]);
            Assert.IsNotNull(colors["персиковый крайола"]);
            Assert.IsNotNull(colors["перу"]);
            Assert.IsNotNull(colors["песок пустыни"]);
            Assert.IsNotNull(colors["песочно желтый"]);
            Assert.IsNotNull(colors["песочно-желтый"]);
            Assert.IsNotNull(colors["песочный"]);
            Assert.IsNotNull(colors["песочный серо коричневый"]);
            Assert.IsNotNull(colors["песочный серо-коричневый"]);
            Assert.IsNotNull(colors["пигментный зеленый"]);
            Assert.IsNotNull(colors["пихтовый зеленый"]);
            Assert.IsNotNull(colors["пламенная маджента крайола"]);
            Assert.IsNotNull(colors["платиново серый"]);
            Assert.IsNotNull(colors["платиново-серый"]);
            Assert.IsNotNull(colors["побег папайи"]);
            Assert.IsNotNull(colors["полумрак крайола"]);
            Assert.IsNotNull(colors["полуночно синий"]);
            Assert.IsNotNull(colors["полуночно-синий"]);
            Assert.IsNotNull(colors["полуночный синий крайола"]);
            Assert.IsNotNull(colors["полуночный черный"]);
            Assert.IsNotNull(colors["почти черный"]);
            Assert.IsNotNull(colors["призрачно белый"]);
            Assert.IsNotNull(colors["призрачно-белый"]);
            Assert.IsNotNull(colors["пурпурная пицца"]);
            Assert.IsNotNull(colors["пурпурно белый"]);
            Assert.IsNotNull(colors["пурпурно красный"]);
            Assert.IsNotNull(colors["пурпурно серый"]);
            Assert.IsNotNull(colors["пурпурно фиолетовый"]);
            Assert.IsNotNull(colors["пурпурно черный"]);
            Assert.IsNotNull(colors["пурпурно-белый"]);
            Assert.IsNotNull(colors["пурпурное горное величие"]);
            Assert.IsNotNull(colors["пурпурное сердце"]);
            Assert.IsNotNull(colors["пурпурно-красный"]);
            Assert.IsNotNull(colors["пурпурно-серый"]);
            Assert.IsNotNull(colors["пурпурно-фиолетовый"]);
            Assert.IsNotNull(colors["пурпурно-черный"]);
            Assert.IsNotNull(colors["пурпурный"]);
            Assert.IsNotNull(colors["пшеничный"]);
            Assert.IsNotNull(colors["пылкий розовый"]);
            Assert.IsNotNull(colors["пыльно серый"]);
            Assert.IsNotNull(colors["пыльно-серый"]);
            Assert.IsNotNull(colors["пыльный голубой"]);
            Assert.IsNotNull(colors["пюсовый"]);
            Assert.IsNotNull(colors["радикальный красный"]);
            Assert.IsNotNull(colors["рапсово желтый"]);
            Assert.IsNotNull(colors["рапсово-желтый"]);
            Assert.IsNotNull(colors["резедово зеленый"]);
            Assert.IsNotNull(colors["резедово-зеленый"]);
            Assert.IsNotNull(colors["ржаво коричневый"]);
            Assert.IsNotNull(colors["ржаво-коричневый"]);
            Assert.IsNotNull(colors["розовато лилово серый"]);
            Assert.IsNotNull(colors["розовато лиловый"]);
            Assert.IsNotNull(colors["розовато лиловый крайола"]);
            Assert.IsNotNull(colors["розовато серый"]);
            Assert.IsNotNull(colors["розовато-лилово-серый"]);
            Assert.IsNotNull(colors["розовато-лиловый"]);
            Assert.IsNotNull(colors["розовато-лиловый крайола"]);
            Assert.IsNotNull(colors["розовато-серый"]);
            Assert.IsNotNull(colors["розовая гвоздика"]);
            Assert.IsNotNull(colors["розовая долина"]);
            Assert.IsNotNull(colors["розовая фуксия"]);
            Assert.IsNotNull(colors["розово золотой"]);
            Assert.IsNotNull(colors["розово коричневый"]);
            Assert.IsNotNull(colors["розово лавандовый"]);
            Assert.IsNotNull(colors["розово серо коричневый"]);
            Assert.IsNotNull(colors["розово фиолетовый"]);
            Assert.IsNotNull(colors["розово эбонитовый"]);
            Assert.IsNotNull(colors["розово-золотой"]);
            Assert.IsNotNull(colors["розово-коричневый"]);
            Assert.IsNotNull(colors["розово-лавандовый"]);
            Assert.IsNotNull(colors["розово-серо-коричневый"]);
            Assert.IsNotNull(colors["розово-фиолетовый"]);
            Assert.IsNotNull(colors["розово-эбонитовый"]);
            Assert.IsNotNull(colors["розовый"]);
            Assert.IsNotNull(colors["розовый антик"]);
            Assert.IsNotNull(colors["розовый кварц"]);
            Assert.IsNotNull(colors["розовый лес"]);
            Assert.IsNotNull(colors["розовый маунтбэттена"]);
            Assert.IsNotNull(colors["розовый поросенок"]);
            Assert.IsNotNull(colors["розовый фламинго"]);
            Assert.IsNotNull(colors["розовый щербет"]);
            Assert.IsNotNull(colors["рубиново красный"]);
            Assert.IsNotNull(colors["рубиново-красный"]);
            Assert.IsNotNull(colors["румянец"]);
            Assert.IsNotNull(colors["салатовый"]);
            Assert.IsNotNull(colors["сангина"]);
            Assert.IsNotNull(colors["сапфирово синий"]);
            Assert.IsNotNull(colors["сапфирово-синий"]);
            Assert.IsNotNull(colors["сапфировый"]);
            Assert.IsNotNull(colors["светлая вишня"]);
            Assert.IsNotNull(colors["светлая слива"]);
            Assert.IsNotNull(colors["светлая слоновая кость"]);
            Assert.IsNotNull(colors["светло бирюзовый"]);
            Assert.IsNotNull(colors["светло вишневый крайола"]);
            Assert.IsNotNull(colors["светло голубой"]);
            Assert.IsNotNull(colors["светло желтый"]);
            Assert.IsNotNull(colors["светло желтый золотистый"]);
            Assert.IsNotNull(colors["светло зеленый"]);
            Assert.IsNotNull(colors["светло золотистый"]);
            Assert.IsNotNull(colors["светло карминово розовый"]);
            Assert.IsNotNull(colors["светло коралловый"]);
            Assert.IsNotNull(colors["светло коричневый"]);
            Assert.IsNotNull(colors["светло морковный"]);
            Assert.IsNotNull(colors["светло оливковый"]);
            Assert.IsNotNull(colors["светло песочный"]);
            Assert.IsNotNull(colors["светло пурпурный"]);
            Assert.IsNotNull(colors["светло розовато лиловый"]);
            Assert.IsNotNull(colors["светло розовая фуксия"]);
            Assert.IsNotNull(colors["светло розово лиловый"]);
            Assert.IsNotNull(colors["светло розовый"]);
            Assert.IsNotNull(colors["светло серебристый"]);
            Assert.IsNotNull(colors["светло серо синий"]);
            Assert.IsNotNull(colors["светло серовато красный"]);
            Assert.IsNotNull(colors["светло серый"]);
            Assert.IsNotNull(colors["светло сине серый"]);
            Assert.IsNotNull(colors["светло синий"]);
            Assert.IsNotNull(colors["светло тициановый"]);
            Assert.IsNotNull(colors["светло фиолетовый"]);
            Assert.IsNotNull(colors["светло-бирюзовый"]);
            Assert.IsNotNull(colors["светло-вишневый крайола"]);
            Assert.IsNotNull(colors["светло-голубой"]);
            Assert.IsNotNull(colors["светлое зеленое море"]);
            Assert.IsNotNull(colors["светло-желтый"]);
            Assert.IsNotNull(colors["светло-желтый золотистый"]);
            Assert.IsNotNull(colors["светло-зеленый"]);
            Assert.IsNotNull(colors["светло-золотистый"]);
            Assert.IsNotNull(colors["светло-карминово-розовый"]);
            Assert.IsNotNull(colors["светло-коралловый"]);
            Assert.IsNotNull(colors["светло-коричневый"]);
            Assert.IsNotNull(colors["светло-морковный"]);
            Assert.IsNotNull(colors["светло-оливковый"]);
            Assert.IsNotNull(colors["светло-песочный"]);
            Assert.IsNotNull(colors["светло-пурпурный"]);
            Assert.IsNotNull(colors["светло-розовато-лиловый"]);
            Assert.IsNotNull(colors["светло-розовая фуксия"]);
            Assert.IsNotNull(colors["светло-розово-лиловый"]);
            Assert.IsNotNull(colors["светло-розовый"]);
            Assert.IsNotNull(colors["светло-серебристый"]);
            Assert.IsNotNull(colors["светло-серовато-красный"]);
            Assert.IsNotNull(colors["светло-серо-синий"]);
            Assert.IsNotNull(colors["светло-серый"]);
            Assert.IsNotNull(colors["светло-сине-серый"]);
            Assert.IsNotNull(colors["светлосиний"]);
            Assert.IsNotNull(colors["светло-синий"]);
            Assert.IsNotNull(colors["светло-тициановый"]);
            Assert.IsNotNull(colors["светло-фиолетовый"]);
            Assert.IsNotNull(colors["светлый аспидно серый"]);
            Assert.IsNotNull(colors["светлый аспидно-серый"]);
            Assert.IsNotNull(colors["светлый глубокий желтый"]);
            Assert.IsNotNull(colors["светлый джинсовый"]);
            Assert.IsNotNull(colors["светлый желто зеленый"]);
            Assert.IsNotNull(colors["светлый желтовато коричневый"]);
            Assert.IsNotNull(colors["светлый желтовато розовый"]);
            Assert.IsNotNull(colors["светлый желтовато-коричневый"]);
            Assert.IsNotNull(colors["светлый желтовато-розовый"]);
            Assert.IsNotNull(colors["светлый желто-зеленый"]);
            Assert.IsNotNull(colors["светлый зеленовато белый"]);
            Assert.IsNotNull(colors["светлый зеленовато желтый"]);
            Assert.IsNotNull(colors["светлый зеленовато синий"]);
            Assert.IsNotNull(colors["светлый зеленовато-белый"]);
            Assert.IsNotNull(colors["светлый зеленовато-желтый"]);
            Assert.IsNotNull(colors["светлый зеленовато-синий"]);
            Assert.IsNotNull(colors["светлый коричневато серый"]);
            Assert.IsNotNull(colors["светлый коричневато-серый"]);
            Assert.IsNotNull(colors["светлый коричневый"]);
            Assert.IsNotNull(colors["светлый красновато коричневый"]);
            Assert.IsNotNull(colors["светлый красновато пурпурный"]);
            Assert.IsNotNull(colors["светлый красновато-коричневый"]);
            Assert.IsNotNull(colors["светлый красновато-пурпурный"]);
            Assert.IsNotNull(colors["светлый малиново красный"]);
            Assert.IsNotNull(colors["светлый малиново-красный"]);
            Assert.IsNotNull(colors["светлый оливково коричневый"]);
            Assert.IsNotNull(colors["светлый оливково серый"]);
            Assert.IsNotNull(colors["светлый оливково-коричневый"]);
            Assert.IsNotNull(colors["светлый оливково-серый"]);
            Assert.IsNotNull(colors["светлый оранжевый"]);
            Assert.IsNotNull(colors["светлый пурпурно розовый"]);
            Assert.IsNotNull(colors["светлый пурпурно серый"]);
            Assert.IsNotNull(colors["светлый пурпурно синий"]);
            Assert.IsNotNull(colors["светлый пурпурно-розовый"]);
            Assert.IsNotNull(colors["светлый пурпурно-серый"]);
            Assert.IsNotNull(colors["светлый пурпурно-синий"]);
            Assert.IsNotNull(colors["светлый серовато желтовато коричневый"]);
            Assert.IsNotNull(colors["светлый серовато коричневый"]);
            Assert.IsNotNull(colors["светлый серовато красновато коричневый"]);
            Assert.IsNotNull(colors["светлый серовато оливковый"]);
            Assert.IsNotNull(colors["светлый серовато пурпурно красный"]);
            Assert.IsNotNull(colors["светлый серовато-желтовато-коричневый"]);
            Assert.IsNotNull(colors["светлый серовато-коричневый"]);
            Assert.IsNotNull(colors["светлый серовато-красновато-коричневый"]);
            Assert.IsNotNull(colors["светлый серовато-оливковый"]);
            Assert.IsNotNull(colors["светлый серовато-пурпурно-красный"]);
            Assert.IsNotNull(colors["светлый серый"]);
            Assert.IsNotNull(colors["светлый синевато зеленый"]);
            Assert.IsNotNull(colors["светлый синевато серый"]);
            Assert.IsNotNull(colors["светлый синевато-зеленый"]);
            Assert.IsNotNull(colors["светлый синевато-серый"]);
            Assert.IsNotNull(colors["светлый стальной синий"]);
            Assert.IsNotNull(colors["светлый телегрей"]);
            Assert.IsNotNull(colors["светлый хаки"]);
            Assert.IsNotNull(colors["светлый циан"]);
            Assert.IsNotNull(colors["сепия"]);
            Assert.IsNotNull(colors["сепия коричневый"]);
            Assert.IsNotNull(colors["сепия крайола"]);
            Assert.IsNotNull(colors["серая белка"]);
            Assert.IsNotNull(colors["серая спаржа"]);
            Assert.IsNotNull(colors["серая умбра"]);
            Assert.IsNotNull(colors["серебристо серый"]);
            Assert.IsNotNull(colors["серебристо-серый"]);
            Assert.IsNotNull(colors["серебряный"]);
            Assert.IsNotNull(colors["серебряный крайола"]);
            Assert.IsNotNull(colors["серо бежевый"]);
            Assert.IsNotNull(colors["серо зеленый"]);
            Assert.IsNotNull(colors["серо коричневый"]);
            Assert.IsNotNull(colors["серо оливковый"]);
            Assert.IsNotNull(colors["серо синий"]);
            Assert.IsNotNull(colors["серо синий крайола"]);
            Assert.IsNotNull(colors["серо-бежевый"]);
            Assert.IsNotNull(colors["серобуромалиновый"]);
            Assert.IsNotNull(colors["серовато желтовато зеленый"]);
            Assert.IsNotNull(colors["серовато желтовато коричневый"]);
            Assert.IsNotNull(colors["серовато желтовато розовый"]);
            Assert.IsNotNull(colors["серовато желтый"]);
            Assert.IsNotNull(colors["серовато зеленовато желтый"]);
            Assert.IsNotNull(colors["серовато зеленый"]);
            Assert.IsNotNull(colors["серовато коричневый"]);
            Assert.IsNotNull(colors["серовато красновато коричневый"]);
            Assert.IsNotNull(colors["серовато красновато оранжевый"]);
            Assert.IsNotNull(colors["серовато красновато пурпурный"]);
            Assert.IsNotNull(colors["серовато красный"]);
            Assert.IsNotNull(colors["серовато оливковый"]);
            Assert.IsNotNull(colors["серовато пурпурно красный"]);
            Assert.IsNotNull(colors["серовато пурпурно розовый"]);
            Assert.IsNotNull(colors["серовато пурпурно синий"]);
            Assert.IsNotNull(colors["серовато пурпурный"]);
            Assert.IsNotNull(colors["серовато розовый"]);
            Assert.IsNotNull(colors["серовато синий"]);
            Assert.IsNotNull(colors["серовато фиолетовый"]);
            Assert.IsNotNull(colors["серовато-желтовато-зеленый"]);
            Assert.IsNotNull(colors["серовато-желтовато-коричневый"]);
            Assert.IsNotNull(colors["серовато-желтовато-розовый"]);
            Assert.IsNotNull(colors["серовато-желтый"]);
            Assert.IsNotNull(colors["серовато-зеленовато-желтый"]);
            Assert.IsNotNull(colors["серовато-зеленый"]);
            Assert.IsNotNull(colors["серовато-коричневый"]);
            Assert.IsNotNull(colors["серовато-красновато-коричневый"]);
            Assert.IsNotNull(colors["серовато-красновато-оранжевый"]);
            Assert.IsNotNull(colors["серовато-красновато-пурпурный"]);
            Assert.IsNotNull(colors["серовато-красный"]);
            Assert.IsNotNull(colors["серовато-оливковый"]);
            Assert.IsNotNull(colors["серовато-пурпурно-красный"]);
            Assert.IsNotNull(colors["серовато-пурпурно-розовый"]);
            Assert.IsNotNull(colors["серовато-пурпурно-синий"]);
            Assert.IsNotNull(colors["серовато-пурпурный"]);
            Assert.IsNotNull(colors["серовато-розовый"]);
            Assert.IsNotNull(colors["серовато-синий"]);
            Assert.IsNotNull(colors["серовато-фиолетовый"]);
            Assert.IsNotNull(colors["сероватый оливково зеленый"]);
            Assert.IsNotNull(colors["сероватый оливково-зеленый"]);
            Assert.IsNotNull(colors["серое окно"]);
            Assert.IsNotNull(colors["серое хаки"]);
            Assert.IsNotNull(colors["серо-зеленый"]);
            Assert.IsNotNull(colors["серо-коричневый"]);
            Assert.IsNotNull(colors["серо-оливковый"]);
            Assert.IsNotNull(colors["серо-синий"]);
            Assert.IsNotNull(colors["серо-синий крайола"]);
            Assert.IsNotNull(colors["серый"]);
            Assert.IsNotNull(colors["серый бетон"]);
            Assert.IsNotNull(colors["серый зеленый чай"]);
            Assert.IsNotNull(colors["серый крайола"]);
            Assert.IsNotNull(colors["серый мох"]);
            Assert.IsNotNull(colors["серый нейтральный"]);
            Assert.IsNotNull(colors["серый шелк"]);
            Assert.IsNotNull(colors["серый шифер"]);
            Assert.IsNotNull(colors["сигнальный желтый"]);
            Assert.IsNotNull(colors["сигнальный зеленый"]);
            Assert.IsNotNull(colors["сигнальный коричневый"]);
            Assert.IsNotNull(colors["сигнальный красный"]);
            Assert.IsNotNull(colors["сигнальный серый"]);
            Assert.IsNotNull(colors["сигнальный синий"]);
            Assert.IsNotNull(colors["сигнальный фиолетовый"]);
            Assert.IsNotNull(colors["сигнальный черный"]);
            Assert.IsNotNull(colors["сиена"]);
            Assert.IsNotNull(colors["сиена жженая"]);
            Assert.IsNotNull(colors["сизый"]);
            Assert.IsNotNull(colors["сине зеленый"]);
            Assert.IsNotNull(colors["сине зеленый крайола"]);
            Assert.IsNotNull(colors["сине лиловый"]);
            Assert.IsNotNull(colors["сине серый"]);
            Assert.IsNotNull(colors["сине серый крайола"]);
            Assert.IsNotNull(colors["сине сиреневый"]);
            Assert.IsNotNull(colors["сине фиолетовый крайола"]);
            Assert.IsNotNull(colors["синевато белый"]);
            Assert.IsNotNull(colors["синевато серый"]);
            Assert.IsNotNull(colors["синевато черный"]);
            Assert.IsNotNull(colors["синевато-белый"]);
            Assert.IsNotNull(colors["синевато-серый"]);
            Assert.IsNotNull(colors["синевато-черный"]);
            Assert.IsNotNull(colors["сине-зеленый"]);
            Assert.IsNotNull(colors["сине-зеленый крайола"]);
            Assert.IsNotNull(colors["сине-лиловый"]);
            Assert.IsNotNull(colors["сине-серый"]);
            Assert.IsNotNull(colors["сине-серый крайола"]);
            Assert.IsNotNull(colors["сине-сиреневый"]);
            Assert.IsNotNull(colors["сине-фиолетовый крайола"]);
            Assert.IsNotNull(colors["синий"]);
            Assert.IsNotNull(colors["синий градуса"]);
            Assert.IsNotNull(colors["синий клейна"]);
            Assert.IsNotNull(colors["синий крайола"]);
            Assert.IsNotNull(colors["синий чирок"]);
            Assert.IsNotNull(colors["синяя пыль"]);
            Assert.IsNotNull(colors["синяя сталь"]);
            Assert.IsNotNull(colors["сиреневый"]);
            Assert.IsNotNull(colors["скандальный оранжевый"]);
            Assert.IsNotNull(colors["скарлет"]);
            Assert.IsNotNull(colors["сладкая вата"]);
            Assert.IsNotNull(colors["сланцево серый"]);
            Assert.IsNotNull(colors["сланцево-серый"]);
            Assert.IsNotNull(colors["сливовый"]);
            Assert.IsNotNull(colors["сливовый крайола"]);
            Assert.IsNotNull(colors["сливочный"]);
            Assert.IsNotNull(colors["слоновая кость"]);
            Assert.IsNotNull(colors["снежно синий"]);
            Assert.IsNotNull(colors["снежно-синий"]);
            Assert.IsNotNull(colors["солнечно желтый"]);
            Assert.IsNotNull(colors["солнечно-желтый"]);
            Assert.IsNotNull(colors["сосновый зеленый"]);
            Assert.IsNotNull(colors["спаржа"]);
            Assert.IsNotNull(colors["спаржа крайола"]);
            Assert.IsNotNull(colors["средний карминный"]);
            Assert.IsNotNull(colors["средний персидский синий"]);
            Assert.IsNotNull(colors["средний пурпурный"]);
            Assert.IsNotNull(colors["средний серый"]);
            Assert.IsNotNull(colors["стально синий"]);
            Assert.IsNotNull(colors["стально-синий"]);
            Assert.IsNotNull(colors["старинный розовый"]);
            Assert.IsNotNull(colors["старое золото"]);
            Assert.IsNotNull(colors["старое кружево"]);
            Assert.IsNotNull(colors["старый лен"]);
            Assert.IsNotNull(colors["сырая охра"]);
            Assert.IsNotNull(colors["сырая умбра"]);
            Assert.IsNotNull(colors["телегрей"]);
            Assert.IsNotNull(colors["телемагента"]);
            Assert.IsNotNull(colors["темная византия"]);
            Assert.IsNotNull(colors["темная орхидея"]);
            Assert.IsNotNull(colors["темно алый"]);
            Assert.IsNotNull(colors["темно бирюзовый"]);
            Assert.IsNotNull(colors["темно голубой"]);
            Assert.IsNotNull(colors["темно желтый"]);
            Assert.IsNotNull(colors["темно зеленый"]);
            Assert.IsNotNull(colors["темно каштановый"]);
            Assert.IsNotNull(colors["темно коралловый"]);
            Assert.IsNotNull(colors["темно коричневый"]);
            Assert.IsNotNull(colors["темно лазурный"]);
            Assert.IsNotNull(colors["темно лососивый"]);
            Assert.IsNotNull(colors["темно мандариновый"]);
            Assert.IsNotNull(colors["темно оливковый"]);
            Assert.IsNotNull(colors["темно оранжевый"]);
            Assert.IsNotNull(colors["темно персиковый"]);
            Assert.IsNotNull(colors["темно пурпурный"]);
            Assert.IsNotNull(colors["темно розовый"]);
            Assert.IsNotNull(colors["темно серо коричневый"]);
            Assert.IsNotNull(colors["темно сероватый оливково зеленый"]);
            Assert.IsNotNull(colors["темно серый"]);
            Assert.IsNotNull(colors["темно синий"]);
            Assert.IsNotNull(colors["темно синий крайола"]);
            Assert.IsNotNull(colors["темно фиолетовый"]);
            Assert.IsNotNull(colors["темно-алый"]);
            Assert.IsNotNull(colors["темно-бирюзовый"]);
            Assert.IsNotNull(colors["темно-голубой"]);
            Assert.IsNotNull(colors["темное зеленое море"]);
            Assert.IsNotNull(colors["темно-желтый"]);
            Assert.IsNotNull(colors["темно-зеленый"]);
            Assert.IsNotNull(colors["темно-каштановый"]);
            Assert.IsNotNull(colors["темно-коралловый"]);
            Assert.IsNotNull(colors["темно-коричневый"]);
            Assert.IsNotNull(colors["темнокрасный"]);
            Assert.IsNotNull(colors["темно-лазурный"]);
            Assert.IsNotNull(colors["темно-лососивый"]);
            Assert.IsNotNull(colors["темно-мандариновый"]);
            Assert.IsNotNull(colors["темно-оливковый"]);
            Assert.IsNotNull(colors["темно-оранжевый"]);
            Assert.IsNotNull(colors["темно-персиковый"]);
            Assert.IsNotNull(colors["темно-пурпурный"]);
            Assert.IsNotNull(colors["темно-розовый"]);
            Assert.IsNotNull(colors["темно-сероватый оливково-зеленый"]);
            Assert.IsNotNull(colors["темно-серо-коричневый"]);
            Assert.IsNotNull(colors["темно-серый"]);
            Assert.IsNotNull(colors["темно-синий"]);
            Assert.IsNotNull(colors["темно-синий крайола"]);
            Assert.IsNotNull(colors["темно-фиолетовый"]);
            Assert.IsNotNull(colors["темный аспидно синий"]);
            Assert.IsNotNull(colors["темный аспидно-синий"]);
            Assert.IsNotNull(colors["темный весенне зеленый"]);
            Assert.IsNotNull(colors["темный весенне-зеленый"]);
            Assert.IsNotNull(colors["темный желто зеленый"]);
            Assert.IsNotNull(colors["темный желто коричневый"]);
            Assert.IsNotNull(colors["темный желтовато зеленый"]);
            Assert.IsNotNull(colors["темный желтовато коричневый"]);
            Assert.IsNotNull(colors["темный желтовато розовый"]);
            Assert.IsNotNull(colors["темный желтовато-зеленый"]);
            Assert.IsNotNull(colors["темный желтовато-коричневый"]);
            Assert.IsNotNull(colors["темный желтовато-розовый"]);
            Assert.IsNotNull(colors["темный желто-зеленый"]);
            Assert.IsNotNull(colors["темный желто-коричневый"]);
            Assert.IsNotNull(colors["темный зеленовато желтовато зеленый"]);
            Assert.IsNotNull(colors["темный зеленовато желтый"]);
            Assert.IsNotNull(colors["темный зеленовато серый"]);
            Assert.IsNotNull(colors["темный зеленовато синий"]);
            Assert.IsNotNull(colors["темный зеленовато-желтовато-зеленый"]);
            Assert.IsNotNull(colors["темный зеленовато-желтый"]);
            Assert.IsNotNull(colors["темный зеленовато-серый"]);
            Assert.IsNotNull(colors["темный зеленовато-синий"]);
            Assert.IsNotNull(colors["темный зеленый чай"]);
            Assert.IsNotNull(colors["темный золотарник"]);
            Assert.IsNotNull(colors["темный индиго"]);
            Assert.IsNotNull(colors["темный красновато коричневый"]);
            Assert.IsNotNull(colors["темный красновато оранжевый"]);
            Assert.IsNotNull(colors["темный красновато пурпурный"]);
            Assert.IsNotNull(colors["темный красновато серый"]);
            Assert.IsNotNull(colors["темный красновато-коричневый"]);
            Assert.IsNotNull(colors["темный красновато-оранжевый"]);
            Assert.IsNotNull(colors["темный красновато-пурпурный"]);
            Assert.IsNotNull(colors["темный красновато-серый"]);
            Assert.IsNotNull(colors["темный красный"]);
            Assert.IsNotNull(colors["темный маджента"]);
            Assert.IsNotNull(colors["темный оливково зеленый"]);
            Assert.IsNotNull(colors["темный оливково коричневый"]);
            Assert.IsNotNull(colors["темный оливково-зеленый"]);
            Assert.IsNotNull(colors["темный оливково-коричневый"]);
            Assert.IsNotNull(colors["темный оранжево желтый"]);
            Assert.IsNotNull(colors["темный оранжево-желтый"]);
            Assert.IsNotNull(colors["темный пастельно зеленый"]);
            Assert.IsNotNull(colors["темный пастельно-зеленый"]);
            Assert.IsNotNull(colors["темный пурпурно красный"]);
            Assert.IsNotNull(colors["темный пурпурно розовый"]);
            Assert.IsNotNull(colors["темный пурпурно серый"]);
            Assert.IsNotNull(colors["темный пурпурно синий"]);
            Assert.IsNotNull(colors["темный пурпурно фиолетовый"]);
            Assert.IsNotNull(colors["темный пурпурно-красный"]);
            Assert.IsNotNull(colors["темный пурпурно-розовый"]);
            Assert.IsNotNull(colors["темный пурпурно-серый"]);
            Assert.IsNotNull(colors["темный пурпурно-синий"]);
            Assert.IsNotNull(colors["темный пурпурно-фиолетовый"]);
            Assert.IsNotNull(colors["темный розовый"]);
            Assert.IsNotNull(colors["темный серо синий"]);
            Assert.IsNotNull(colors["темный серовато желтый"]);
            Assert.IsNotNull(colors["темный серовато коричневый"]);
            Assert.IsNotNull(colors["темный серовато красновато коричневый"]);
            Assert.IsNotNull(colors["темный серовато красный"]);
            Assert.IsNotNull(colors["темный серовато оливковый"]);
            Assert.IsNotNull(colors["темный серовато синий"]);
            Assert.IsNotNull(colors["темный серовато-желтый"]);
            Assert.IsNotNull(colors["темный серовато-коричневый"]);
            Assert.IsNotNull(colors["темный серовато-красновато-коричневый"]);
            Assert.IsNotNull(colors["темный серовато-красный"]);
            Assert.IsNotNull(colors["темный серовато-оливковый"]);
            Assert.IsNotNull(colors["темный серовато-синий"]);
            Assert.IsNotNull(colors["темный серо-синий"]);
            Assert.IsNotNull(colors["темный синевато зеленый"]);
            Assert.IsNotNull(colors["темный синевато черный"]);
            Assert.IsNotNull(colors["темный синевато-зеленый"]);
            Assert.IsNotNull(colors["темный синевато-черный"]);
            Assert.IsNotNull(colors["темный телегрей"]);
            Assert.IsNotNull(colors["темный хаки"]);
            Assert.IsNotNull(colors["темный циан"]);
            Assert.IsNotNull(colors["темный черовато пурпурный"]);
            Assert.IsNotNull(colors["темный черовато-пурпурный"]);
            Assert.IsNotNull(colors["темный янтарь"]);
            Assert.IsNotNull(colors["терракота"]);
            Assert.IsNotNull(colors["терракотовый"]);
            Assert.IsNotNull(colors["тихоокеанский синий"]);
            Assert.IsNotNull(colors["тициановый"]);
            Assert.IsNotNull(colors["томатно красный"]);
            Assert.IsNotNull(colors["томатно-красный"]);
            Assert.IsNotNull(colors["томатный"]);
            Assert.IsNotNull(colors["травяной"]);
            Assert.IsNotNull(colors["травяной зеленый"]);
            Assert.IsNotNull(colors["транспортно желтый"]);
            Assert.IsNotNull(colors["транспортно-желтый"]);
            Assert.IsNotNull(colors["транспортный зеленый"]);
            Assert.IsNotNull(colors["транспортный красный"]);
            Assert.IsNotNull(colors["транспортный оранжевый"]);
            Assert.IsNotNull(colors["транспортный пурпурный"]);
            Assert.IsNotNull(colors["транспортный серый"]);
            Assert.IsNotNull(colors["транспортный синий"]);
            Assert.IsNotNull(colors["транспортный черный"]);
            Assert.IsNotNull(colors["трилистник крайола"]);
            Assert.IsNotNull(colors["тростниково зеленый"]);
            Assert.IsNotNull(colors["тростниково-зеленый"]);
            Assert.IsNotNull(colors["турецкий розовый"]);
            Assert.IsNotNull(colors["тускло розовый"]);
            Assert.IsNotNull(colors["тускло-розовый"]);
            Assert.IsNotNull(colors["тусклый амарантово розовый"]);
            Assert.IsNotNull(colors["тусклый амарантово-розовый"]);
            Assert.IsNotNull(colors["тусклый серый"]);
            Assert.IsNotNull(colors["тыква"]);
            Assert.IsNotNull(colors["ультрамариново синий"]);
            Assert.IsNotNull(colors["ультрамариново-синий"]);
            Assert.IsNotNull(colors["ультрамариновый"]);
            Assert.IsNotNull(colors["умбра жженая"]);
            Assert.IsNotNull(colors["умеренно бирюзовый"]);
            Assert.IsNotNull(colors["умеренно зеленый"]);
            Assert.IsNotNull(colors["умеренно оливковый"]);
            Assert.IsNotNull(colors["умеренно-бирюзовый"]);
            Assert.IsNotNull(colors["умеренно-оливковый"]);
            Assert.IsNotNull(colors["умеренный аквамариновый"]);
            Assert.IsNotNull(colors["умеренный аспидно синий"]);
            Assert.IsNotNull(colors["умеренный аспидно-синий"]);
            Assert.IsNotNull(colors["умеренный весенний зеленый"]);
            Assert.IsNotNull(colors["умеренный желто зеленый"]);
            Assert.IsNotNull(colors["умеренный желтовато зеленый"]);
            Assert.IsNotNull(colors["умеренный желтовато коричневый"]);
            Assert.IsNotNull(colors["умеренный желтовато розовый"]);
            Assert.IsNotNull(colors["умеренный желтовато-зеленый"]);
            Assert.IsNotNull(colors["умеренный желтовато-коричневый"]);
            Assert.IsNotNull(colors["умеренный желтовато-розовый"]);
            Assert.IsNotNull(colors["умеренный желто-зеленый"]);
            Assert.IsNotNull(colors["умеренный желтый"]);
            Assert.IsNotNull(colors["умеренный зеленовато желтый"]);
            Assert.IsNotNull(colors["умеренный зеленовато синий"]);
            Assert.IsNotNull(colors["умеренный зеленовато-желтый"]);
            Assert.IsNotNull(colors["умеренный зеленовато-синий"]);
            Assert.IsNotNull(colors["умеренный зеленый"]);
            Assert.IsNotNull(colors["умеренный коричневый"]);
            Assert.IsNotNull(colors["умеренный красновато оранжевый"]);
            Assert.IsNotNull(colors["умеренный красновато пурпурный"]);
            Assert.IsNotNull(colors["умеренный красновато-оранжевый"]);
            Assert.IsNotNull(colors["умеренный красновато-пурпурный"]);
            Assert.IsNotNull(colors["умеренный красный"]);
            Assert.IsNotNull(colors["умеренный оливково зеленый"]);
            Assert.IsNotNull(colors["умеренный оливково коричневый"]);
            Assert.IsNotNull(colors["умеренный оливково-зеленый"]);
            Assert.IsNotNull(colors["умеренный оливково-коричневый"]);
            Assert.IsNotNull(colors["умеренный оранжево желтый"]);
            Assert.IsNotNull(colors["умеренный оранжево-желтый"]);
            Assert.IsNotNull(colors["умеренный оранжевый"]);
            Assert.IsNotNull(colors["умеренный пурпурно красный"]);
            Assert.IsNotNull(colors["умеренный пурпурно розовый"]);
            Assert.IsNotNull(colors["умеренный пурпурно синий"]);
            Assert.IsNotNull(colors["умеренный пурпурно-красный"]);
            Assert.IsNotNull(colors["умеренный пурпурно-розовый"]);
            Assert.IsNotNull(colors["умеренный пурпурно-синий"]);
            Assert.IsNotNull(colors["умеренный пурпурный"]);
            Assert.IsNotNull(colors["умеренный розовый"]);
            Assert.IsNotNull(colors["умеренный серо коричневый"]);
            Assert.IsNotNull(colors["умеренный серо-коричневый"]);
            Assert.IsNotNull(colors["умеренный синевато зеленый"]);
            Assert.IsNotNull(colors["умеренный синевато-зеленый"]);
            Assert.IsNotNull(colors["умеренный синий"]);
            Assert.IsNotNull(colors["умеренный фиолетово красный"]);
            Assert.IsNotNull(colors["умеренный фиолетово-красный"]);
            Assert.IsNotNull(colors["умеренный фиолетовый"]);
            Assert.IsNotNull(colors["фалунский красный"]);
            Assert.IsNotNull(colors["фанданго"]);
            Assert.IsNotNull(colors["фелдграу"]);
            Assert.IsNotNull(colors["фиалковый"]);
            Assert.IsNotNull(colors["фиолетово баклажанный"]);
            Assert.IsNotNull(colors["фиолетово красный крайола"]);
            Assert.IsNotNull(colors["фиолетово сизый"]);
            Assert.IsNotNull(colors["фиолетово синий"]);
            Assert.IsNotNull(colors["фиолетово синий крайола"]);
            Assert.IsNotNull(colors["фиолетово-баклажанный"]);
            Assert.IsNotNull(colors["фиолетово-красный крайола"]);
            Assert.IsNotNull(colors["фиолетово-сизый"]);
            Assert.IsNotNull(colors["фиолетово-синий"]);
            Assert.IsNotNull(colors["фиолетово-синий крайола"]);
            Assert.IsNotNull(colors["фиолетовый"]);
            Assert.IsNotNull(colors["фисташковый"]);
            Assert.IsNotNull(colors["французский розовый"]);
            Assert.IsNotNull(colors["фталоцианитовый зеленый"]);
            Assert.IsNotNull(colors["фузи вузи"]);
            Assert.IsNotNull(colors["фузи-вузи"]);
            Assert.IsNotNull(colors["фуксия"]);
            Assert.IsNotNull(colors["фуксия крайола"]);
            Assert.IsNotNull(colors["хаки"]);
            Assert.IsNotNull(colors["хромовый зеленый"]);
            Assert.IsNotNull(colors["детской неожиданности"]);
            Assert.IsNotNull(colors["загара"]);
            Assert.IsNotNull(colors["загара крайола"]);
            Assert.IsNotNull(colors["красного моря"]);
            Assert.IsNotNull(colors["маленького мандарина"]);
            Assert.IsNotNull(colors["мокрого асфальта"]);
            Assert.IsNotNull(colors["мокрого асфальта с зеленым"]);
            Assert.IsNotNull(colors["морской раковины"]);
            Assert.IsNotNull(colors["пожарной машины"]);
            Assert.IsNotNull(colors["суеты"]);
            Assert.IsNotNull(colors["черного моря"]);
            Assert.IsNotNull(colors["яйца дрозда"]);
            Assert.IsNotNull(colors["яндекса"]);
            Assert.IsNotNull(colors["цветочный белый"]);
            Assert.IsNotNull(colors["цементно серый"]);
            Assert.IsNotNull(colors["цементно-серый"]);
            Assert.IsNotNull(colors["цементный"]);
            Assert.IsNotNull(colors["циан"]);
            Assert.IsNotNull(colors["цинково желтый"]);
            Assert.IsNotNull(colors["цинково-желтый"]);
            Assert.IsNotNull(colors["циннвальдит"]);
            Assert.IsNotNull(colors["циннвальдитово розовый"]);
            Assert.IsNotNull(colors["циннвальдитово-розовый"]);
            Assert.IsNotNull(colors["черно зеленый"]);
            Assert.IsNotNull(colors["черно коричневый"]);
            Assert.IsNotNull(colors["черно красный"]);
            Assert.IsNotNull(colors["черно оливковый"]);
            Assert.IsNotNull(colors["черно серый"]);
            Assert.IsNotNull(colors["черно синий"]);
            Assert.IsNotNull(colors["черновато зеленый"]);
            Assert.IsNotNull(colors["черновато красный"]);
            Assert.IsNotNull(colors["черновато пурпурный"]);
            Assert.IsNotNull(colors["черновато синий"]);
            Assert.IsNotNull(colors["черновато-зеленый"]);
            Assert.IsNotNull(colors["черновато-красный"]);
            Assert.IsNotNull(colors["черновато-пурпурный"]);
            Assert.IsNotNull(colors["черновато-синий"]);
            Assert.IsNotNull(colors["черно-зеленый"]);
            Assert.IsNotNull(colors["черно-коричневый"]);
            Assert.IsNotNull(colors["черно-красный"]);
            Assert.IsNotNull(colors["черно-оливковый"]);
            Assert.IsNotNull(colors["черно-серый"]);
            Assert.IsNotNull(colors["черно-синий"]);
            Assert.IsNotNull(colors["черный"]);
            Assert.IsNotNull(colors["черный янтарь"]);
            Assert.IsNotNull(colors["чертополох"]);
            Assert.IsNotNull(colors["чертополох крайола"]);
            Assert.IsNotNull(colors["шамуа"]);
            Assert.IsNotNull(colors["шартрез"]);
            Assert.IsNotNull(colors["шафраново желтый"]);
            Assert.IsNotNull(colors["шафраново-желтый"]);
            Assert.IsNotNull(colors["шафрановый"]);
            Assert.IsNotNull(colors["шелковица крайола"]);
            Assert.IsNotNull(colors["шокирующий розовый крайола"]);
            Assert.IsNotNull(colors["шоколадно коричневый"]);
            Assert.IsNotNull(colors["шоколадно-коричневый"]);
            Assert.IsNotNull(colors["шоколадный"]);
            Assert.IsNotNull(colors["экрю"]);
            Assert.IsNotNull(colors["экстравагантный розовый крайола"]);
            Assert.IsNotNull(colors["электрик"]);
            Assert.IsNotNull(colors["электрик лайм"]);
            Assert.IsNotNull(colors["электрик лайм крайола"]);
            Assert.IsNotNull(colors["ядовито зеленый"]);
            Assert.IsNotNull(colors["ядовито-зеленый"]);
            Assert.IsNotNull(colors["янтарный"]);
            Assert.IsNotNull(colors["яркий желтовато зеленый"]);
            Assert.IsNotNull(colors["яркий желтовато розовый"]);
            Assert.IsNotNull(colors["яркий желтовато-зеленый"]);
            Assert.IsNotNull(colors["яркий желтовато-розовый"]);
            Assert.IsNotNull(colors["яркий зеленовато желтый"]);
            Assert.IsNotNull(colors["яркий зеленовато-желтый"]);
            Assert.IsNotNull(colors["яркий зеленый"]);
            Assert.IsNotNull(colors["яркий красновато оранжевый"]);
            Assert.IsNotNull(colors["яркий красновато пурпурный"]);
            Assert.IsNotNull(colors["яркий красновато-оранжевый"]);
            Assert.IsNotNull(colors["яркий красновато-пурпурный"]);
            Assert.IsNotNull(colors["яркий красный"]);
            Assert.IsNotNull(colors["яркий оранжево желтый"]);
            Assert.IsNotNull(colors["яркий оранжево-желтый"]);
            Assert.IsNotNull(colors["яркий оранжевый"]);
            Assert.IsNotNull(colors["яркий пурпурно красный"]);
            Assert.IsNotNull(colors["яркий пурпурно-красный"]);
            Assert.IsNotNull(colors["яркий пурпурный"]);
            Assert.IsNotNull(colors["яркий синевато зеленый"]);
            Assert.IsNotNull(colors["яркий синевато-зеленый"]);
            Assert.IsNotNull(colors["яркий фиолетовый крайола"]);
            Assert.IsNotNull(colors["ярко бирюзовый"]);
            Assert.IsNotNull(colors["ярко желтый"]);
            Assert.IsNotNull(colors["ярко зеленый"]);
            Assert.IsNotNull(colors["ярко красно оранжевый"]);
            Assert.IsNotNull(colors["ярко мандариновый"]);
            Assert.IsNotNull(colors["ярко розовый"]);
            Assert.IsNotNull(colors["ярко синий"]);
            Assert.IsNotNull(colors["ярко сиреневый"]);
            Assert.IsNotNull(colors["ярко фиолетовый"]);
            Assert.IsNotNull(colors["ярко-бирюзовый"]);
            Assert.IsNotNull(colors["ярко-желтый"]);
            Assert.IsNotNull(colors["ярко-зеленый"]);
            Assert.IsNotNull(colors["ярко-красно-оранжевый"]);
            Assert.IsNotNull(colors["ярко-мандариновый"]);
            Assert.IsNotNull(colors["ярко-розовый"]);
            Assert.IsNotNull(colors["ярко-синий"]);
            Assert.IsNotNull(colors["ярко-сиреневый"]);
            Assert.IsNotNull(colors["ярко-фиолетовый"]);

            #endregion
        }

        [TestMethod]
        public void GenerateKnownColorsPalletteColorNames()
        {
            //Generate \"SystemColors\" Palette:
            MnemonicColors colors = MnemonicColors.Global;

            var sb = new StringBuilder();
            var containerSb = new StringBuilder();
            var validationSb = new StringBuilder();

            Array values = Enum.GetValues(typeof (KnownColor));

            foreach (KnownColor value in values)
            {
                var name = Enum.GetName(typeof (KnownColor), value);
                var color = Color.FromKnownColor(value);
                var code = new RGB(color).Code;
                MnemonicColor mnemonicColor = colors.Get(code);

                //Palette
                sb.AppendLine();
                sb.AppendFormat("        public static readonly MnemonicColor {0} = Colors.{1};", name, name);
                sb.AppendLine();
                //PaletteContainer
                containerSb.AppendLine();
                containerSb.AppendFormat("          public readonly MnemonicColor {0} = GlobalPalette.{1};", name, mnemonicColor.MnemonicName);
                containerSb.AppendLine();
                //Validation
                validationSb.AppendLine();
                validationSb.AppendFormat("            Assert.AreEqual(\"{0}\", MnemonicColors.KnownColors.{1}.Code);", code, name);
            }

            string palette = sb.ToString();
            string paletteContainer = containerSb.ToString();
            string paletteValidation = validationSb.ToString();

            Assert.IsNotNull(palette);
            Assert.IsNotNull(paletteContainer);
            Assert.IsNotNull(paletteValidation);
        }

        [TestMethod]
        public void GenerateSystemColorsPalletteColorNames()
        {
            //Generate \"SystemColors\" Palette:
            MnemonicColors colors = MnemonicColors.Global;

            var sb = new StringBuilder();
            var containerSb = new StringBuilder();
            var validationSb = new StringBuilder();
            PropertyInfo[] properties = typeof(SystemColors).GetProperties().Where(property => property.PropertyType == typeof(Color)).ToArray();
            foreach (PropertyInfo propertyInfo in properties)
            {
                var name = propertyInfo.Name;
                var color = (Color) propertyInfo.GetValue(null);
                var code = new RGB(color).Code;
                MnemonicColor mnemonicColor = colors.Get(code);

                //Palette
                sb.AppendLine();
                sb.AppendFormat("        public static readonly MnemonicColor {0} = Colors.{1};", name, name);
                sb.AppendLine();
                //PaletteContainer
                containerSb.AppendLine();
                containerSb.AppendFormat("          public readonly MnemonicColor {0} = GlobalPalette.{1};", name, mnemonicColor.MnemonicName);
                containerSb.AppendLine();
                //Validation
                validationSb.AppendLine();
                validationSb.AppendFormat("            Assert.AreEqual(\"{0}\", MnemonicColors.SystemColors.{1}.Code);", code, name);
            }

            string palette = sb.ToString();
            string paletteContainer = containerSb.ToString();
            string paletteValidation = validationSb.ToString();

            Assert.IsNotNull(palette);
            Assert.IsNotNull(paletteContainer);
            Assert.IsNotNull(paletteValidation);
        }
        
        [TestMethod]
        public void GenerateGlobalPalletteColorNames()
        {
            var globalPaletteContainerSb = new StringBuilder();
            var globalPaletteSb = new StringBuilder();
            var mnemonicColorSb = new StringBuilder();
            MnemonicColors colors = MnemonicColors.Global;

            foreach (MnemonicColor color in colors)
            {
                if (!string.IsNullOrEmpty(color.NameEn))
                {
                    //PaletteContainer
                    globalPaletteContainerSb.AppendLine();
                    globalPaletteContainerSb.AppendFormat("          public readonly MnemonicColor {0};", color.MnemonicName);
                    globalPaletteContainerSb.AppendLine();
                    //Palette
                    globalPaletteSb.AppendLine();
                    globalPaletteSb.AppendFormat("        public static readonly MnemonicColor {0} = Colors.{0};", color.MnemonicName);
                    globalPaletteSb.AppendLine();
                    //Mnemonic color
                    mnemonicColorSb.AppendLine();
                    mnemonicColorSb.AppendFormat("        public static MnemonicColor {0}", color.MnemonicName);
                    mnemonicColorSb.AppendLine();
                    mnemonicColorSb.AppendFormat("        {{");
                    mnemonicColorSb.AppendLine();
                    mnemonicColorSb.AppendFormat("            get {{ return GlobalPalette.{0}; }}", color.MnemonicName);
                    mnemonicColorSb.AppendLine();
                    mnemonicColorSb.AppendFormat("        }}");
                    mnemonicColorSb.AppendLine();
                }
            }

            string globalPaletteContainer = globalPaletteContainerSb.ToString();
            string globalPalette = globalPaletteSb.ToString();
            string mnemonicColor = mnemonicColorSb.ToString();

            Assert.IsNotNull(globalPaletteContainer);
            Assert.IsNotNull(globalPalette);
            Assert.IsNotNull(mnemonicColor);
        }

        [TestMethod]
        public void ColorNameSerializationTest()
        {
            var name = new ColorName("RU", "Тёмно синий", new[] { "Темно синий" });

            XmlDocument dataXml = Serializer.Serialize(name, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(dataXml);

            var restoredName = Serializer.Deserialize<ColorName>(dataXml, Serializer.Type.DataContractSerializer);

            Assert.AreEqual(name.LanguageCode, restoredName.LanguageCode);
            Assert.AreEqual(name.Value, restoredName.Value);
            Assert.AreEqual(name.AlternativeNames.Length, restoredName.AlternativeNames.Length);
            for (int i = 0; i < name.AlternativeNames.Length; i++)
            {
                Assert.AreEqual(name.AlternativeNames[i], restoredName.AlternativeNames[i]);
            }
        }

        [TestMethod]
        public void MnemonicColorSerializationTest()
        {
            MnemonicColor navy = MnemonicColors.Global.Navy;

            XmlDocument dataXml = Serializer.Serialize(navy, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(dataXml);

            var restoredColor = Serializer.Deserialize<MnemonicColor>(dataXml, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(restoredColor);

            Assert.AreEqual(navy.Code, restoredColor.Code);
            Assert.AreEqual(navy.RGB.R, restoredColor.RGB.R);
            Assert.AreEqual(navy.RGB.G, restoredColor.RGB.G);
            Assert.AreEqual(navy.RGB.B, restoredColor.RGB.B);
            Assert.AreEqual(navy.NameEn, restoredColor.NameEn);
            Assert.AreEqual(navy.NameRu, restoredColor.NameRu);
            Assert.AreEqual(navy.Names.Count, restoredColor.Names.Count);
            foreach (ColorName name in navy.Names)
            {
                ColorName restored = restoredColor.Names.FindByLanguageCode(name.LanguageCode);
                Assert.IsNotNull(restored);
                Assert.AreEqual(name.AlternativeNames.Length, restored.AlternativeNames.Length);
                for (int i = 0; i < name.AlternativeNames.Length; i++)
                {
                    Assert.AreEqual(name.AlternativeNames[i], restored.AlternativeNames[i]);
                }
            }
        }

        [TestMethod]
        public void MnemonicColorsSerializationTest()
        {
            var colors = new MnemonicColors(MnemonicColors.Html);

            XmlDocument dataXml = Serializer.Serialize(colors, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(dataXml);

            var restoredColors = Serializer.Deserialize<MnemonicColors>(dataXml, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(restoredColors);
        }

        [TestMethod]
        public void RedColorParseTest()
        {
            MnemonicColor color = MnemonicColor.Parse(" Красный цвет самый красный");

            Assert.IsNotNull(color);
            Assert.AreEqual(MnemonicColor.Red, color);
        }

        [TestMethod]
        public void CamelColorParseTest()
        {
            MnemonicColor color = MnemonicColor.Parse("Коричнево-жёлтый цвета увядших листьев");
            Assert.IsNotNull(color);
            Assert.AreEqual(MnemonicColor.Find("Camel"), color);
        }

        [TestMethod]
        public void ColorsParseTest()
        {
            var testMaterials = new[]
                {
                    new[] { "Цвет яндекса", "Tangerine", true.ToString() },
                    new[] { "красного цвета", "red", true.ToString() },
                    new[] { "экрю с термотрансфером + набивка в нейтральных тонах", "экрю", false.ToString() },
                    new[] { "красный, аппликация Тачки в ассортименте. Просьба уточнять желаемый вами вариант при оформлении заказа или у нашего оператора.", "красный", false.ToString()  },
                    new[] { "Цвет и рисунок в ассортименте. Просьба уточнять желаемый вами вариант при оформлении заказа или у нашего оператора.", null, false.ToString() },
                    new[] { "серый   бетон,", "серый бетон", true.ToString() },
                    new[] { " Красный", "красный", true.ToString() },
                    new[] { " Красный/в полоску", "красный", false.ToString() },
                    new[] { "Коричнево-жёлтый цвета увядших листьев", "camel", true.ToString() }
                };

            foreach (string[] testMaterial in testMaterials)
            {
                string sentence = testMaterial[0];
                MnemonicColor color = (testMaterial[1] != null) ? MnemonicColor.Get(testMaterial[1]) : null;
                bool onlyColor = bool.Parse(testMaterial[2]);

                bool parsedOnlyColor;
                MnemonicColor parsedColor = MnemonicColor.Parse(sentence, out parsedOnlyColor);

                if (color != null)
                {
                    string message = string.Format("Color:\"{0}\" OnlyColor:\"{1}\" Sentence:\"{2}\"", color.MnemonicName, onlyColor, sentence);
                    Assert.IsNotNull(color, message);
                    Assert.AreEqual(color, parsedColor, message);
                    Assert.AreEqual(onlyColor, parsedOnlyColor, message);
                }
                else
                {
                    Assert.IsNull(color, sentence);
                    Assert.IsFalse(parsedOnlyColor);
                }
            }
        }
    }
}