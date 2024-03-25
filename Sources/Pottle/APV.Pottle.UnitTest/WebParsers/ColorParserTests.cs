using System.Collections.Generic;
using System.Linq;
using System.Text;
using APV.Pottle.WebParsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common.Html;

namespace APV.Pottle.UnitTest.WebParsers
{
    [TestClass]
    public class ColorParserTests
    {
        [TestMethod]
        public void RedColorTest()
        {
            string color = ColorParser.Parse(" Красный цвет самый красный");

            Assert.IsNotNull(color);
            Assert.AreEqual("красный", color);
        }

        [TestMethod]
        public void ColorsTest()
        {
            var testMaterials = new[]
                {
                    new[] { "экрю с термотрансфером + набивка в нейтральных тонах", "экрю", false.ToString() },
                    new[] { "красный, аппликация Тачки в ассортименте. Просьба уточнять желаемый вами вариант при оформлении заказа или у нашего оператора.", "красный", false.ToString()  },
                    new[] { "Цвет и рисунок в ассортименте. Просьба уточнять желаемый вами вариант при оформлении заказа или у нашего оператора.", null, false.ToString() },
                    new[] { "серый   бетон,", "серый бетон", true.ToString() },
                    new[] { " Красный", "красный", true.ToString() },
                    new[] { " Красный/в полоску", "красный", false.ToString() }
                };

            foreach (string[] testMaterial in testMaterials)
            {
                string sentence = testMaterial[0];
                string color = testMaterial[1];
                bool onlyColor = bool.Parse(testMaterial[2]);

                bool parsedOnlyColor;
                string parsedColor = ColorParser.Parse(sentence, out parsedOnlyColor);

                string message = string.Format("Color:\"{0}\" OnlyColor:\"{1}\" Sentence:\"{2}\"", color, onlyColor, sentence);
                if (color != null)
                {
                    Assert.IsNotNull(color, message);
                    Assert.AreEqual(color, parsedColor, message);
                }
                else
                {
                    Assert.IsNull(color, sentence);
                }

                Assert.AreEqual(onlyColor, parsedOnlyColor, message);
            }
        }

        [TestMethod]
        public void LoadColorsTest()
        {
            var additionlColors = new[]
                    {
                        "Барвинок", "перванш", "Весеннезеленый", "Зеленая весна", "жженый апельсин", "оранжевый", "Выгоревший оранжевый", "гранитный", "серый", "Гранитовый серый",
                        "Зеленые джунгли", "Индийский", "Индийский красный", "каштановый", "Канареечный", "ярко-желтый", "желтый", "Киноварь", "красный", "китайский красный",
                        "Кожа буйвола", "палевый", "Красновато-коричневый", "коричневый", "блошиный", "лазурно-серый", "лазурный", "серый", "зеленовато-синий", "зелёный",
                        "зеленоватый", "синий", "Маджента", "фуксия", "Мовеин", "анилиновый", "пурпур", "анилиновый пурпур", "Розовый", "Сепия", "каракатица", "Серый шифер",
                        "аспидно-серый", "Темно-зеленый", "травяной", "Темно-синий", "Темно-синий", "ультрамариновый", "Фиолетовый", "пурпурный", "Хаки", "защитный", "Цементно-серый",
                        "цементный", "Циан", "цвет морской волны", "Шартрез", "ядовито-зеленый"
                    };

            const string url = @"http://whoyougle.ru/services/colour/list";
            HtmlDocument doc = HtmlDocument.Load(url);
            List<HtmlTag> tags = doc.Find("table.data-table td a");
            var symbodsHashSet = new HashSet<char>("АаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя- ".ToCharArray());
            string[] colors = tags.Select(x => x.Text).ToArray();
            colors = colors.Concat(additionlColors).ToArray();
            colors = colors.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLowerInvariant().Trim().Replace("   ", " ").Replace("  ", " ")).Distinct().ToArray();
            colors = colors.Where(x => x.ToList().All(symbodsHashSet.Contains)).OrderBy(x => x).ToArray();

            var colorDefinitionSb = new StringBuilder();
            var allVariants = new List<string>();
            foreach (var color in colors)
            {
                var variants = new List<string>();
                string baseVariant = color.Replace("-", " ");
                if (!allVariants.Contains(baseVariant))
                {
                    variants.Add(baseVariant);
                    allVariants.Add(baseVariant);
                }

                string variant = baseVariant.Replace("ё", "е");
                if ((variant != color) && (!allVariants.Contains(baseVariant)))
                {
                    variants.Add(variant);
                    allVariants.Add(variant);
                }

                foreach (string innerVariant in variants)
                {
                    colorDefinitionSb.AppendFormat("\t\t\t\t{{ \"{0}\", \"{1}\" }},\r\n", innerVariant, color);
                }
            }
            
            string colorDefinition = colorDefinitionSb.ToString();

            Assert.IsNotNull(colorDefinition);
        }
    }
}