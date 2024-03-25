using System.Xml;
using APV.Pottle.Common;
using APV.Pottle.WebParsers.InfoEntities;
using APV.Pottle.WebParsers.ResultEntities;
using APV.Pottle.WebParsers.Votonia;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;

namespace APV.Pottle.UnitTest.WebParsers
{
    [TestClass]
    public class VotoniaWebParserTests
    {
        #region Product Parser

        [TestMethod]
        public void ProductWithOneImageTest()
        {
            const string url = @"http://www.votonia.ru/igrushka-aist-shnurovka-pugovitza-6-otverstiiy-bereza/spb/2437/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Игрушка Аист, Шнуровка пуговица 6 отверстий, береза", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("АИСТ", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00002817", productInfo.Article);
            Assert.AreEqual("Игрушки>Игрушки для всех>Игрушки из дерева", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(111.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Игрушка предназначена для развития мелкой моторики рук. позволяет вырабатывать навыки необходимые в жизни ребёнка.", productInfo.Options.Description);
            Assert.AreEqual("Вдеть шнурок в ушко деревянной иглы (из комплекта). Имитируя шьющие движения, продевать иглу со шнурком через отверстия пуговицы. Стежки можно располагать паралелльно друг другу, крест на крест или в произвольном порядке. Допускается раскрашивание пуговицы фломастерами или водорастворимыми красками (гуашь, акварель, темпера).", productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("с 2 лет", productInfo.Options.Characteristics.Age);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(733, productInfo.Images[0].Data.Width);
            Assert.AreEqual(550, productInfo.Images[0].Data.Height);
        }

        [TestMethod]
        public void ProductWithOneImageSerializationTest()
        {
            const string url = @"http://www.votonia.ru/igrushka-aist-shnurovka-pugovitza-6-otverstiiy-bereza/spb/2437/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            XmlDocument doc = Serializer.Serialize(productInfo, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(doc);

            var restoredProductInfo = Serializer.Deserialize<VotoniaSupplierProductInfo>(doc, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(restoredProductInfo);
            Assert.AreEqual(productInfo.ProductName, restoredProductInfo.ProductName);
            Assert.AreEqual(productInfo.ProductCost, restoredProductInfo.ProductCost);
            Assert.AreEqual(productInfo.Article, restoredProductInfo.Article);

            Assert.AreEqual(productInfo.Options.Count, restoredProductInfo.Options.Count);
            Assert.AreEqual(productInfo.Options.Complectation, restoredProductInfo.Options.Complectation);
            Assert.AreEqual(productInfo.Options.Description, restoredProductInfo.Options.Description);
            Assert.AreEqual(productInfo.Options.Instruction, restoredProductInfo.Options.Instruction);
            Assert.AreEqual(productInfo.Options.Modifiers, restoredProductInfo.Options.Modifiers);
            Assert.AreEqual(productInfo.Options.Warning, restoredProductInfo.Options.Warning);
            Assert.AreEqual(productInfo.Options.Characteristics.Count, restoredProductInfo.Options.Characteristics.Count);
            Assert.AreEqual(productInfo.Options.Characteristics.Age, restoredProductInfo.Options.Characteristics.Age);
            Assert.AreEqual(productInfo.Options.Characteristics.Color, restoredProductInfo.Options.Characteristics.Color);
            Assert.AreEqual(productInfo.Options.Characteristics.Mode, restoredProductInfo.Options.Characteristics.Mode);
            Assert.AreEqual(productInfo.Options.Characteristics.PackingSize, restoredProductInfo.Options.Characteristics.PackingSize);
            Assert.AreEqual(productInfo.Options.Characteristics.Size, restoredProductInfo.Options.Characteristics.Size);
            Assert.AreEqual(productInfo.Options.Characteristics.Structure, restoredProductInfo.Options.Characteristics.Structure);
            Assert.AreEqual(productInfo.Options.Characteristics.Weight, restoredProductInfo.Options.Characteristics.Weight);

            doc = Serializer.Serialize(result, Serializer.Type.DataContractSerializer);

            Assert.IsNotNull(doc);
        }

        [TestMethod]
        public void ProductWithFourImageTest()
        {
            const string url = @"http://www.votonia.ru/figurka-filly-loshadka/spb/8680/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Фигурка Filly, Лошадка", productInfo.ProductName);
            Assert.AreEqual("Китай", productInfo.ProducerCountryName);
            Assert.AreEqual("Simba", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00013312", productInfo.Article);
            Assert.AreEqual("Игрушки>Игрушки для всех>Фигурки, наборы с фигурками", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(100.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Серия лошадок Filly Unicorn покорит сердце вашей принцессы. Коллекция включает в себя 21 лошадку. На каждой Filly одета корона с кристаллом Swarovski.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("Игрушка представлена в ассортименте. Просьба уточнять желаемый вами вариант при оформлении заказа или у нашего оператора.", productInfo.Options.AdditionalInfo);
            Assert.AreEqual("с 3 лет", productInfo.Options.Characteristics.Age);
            Assert.AreEqual("пластик, флок", productInfo.Options.Characteristics.Structure);
            Assert.AreEqual("5 см", productInfo.Options.Characteristics.Size);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(4, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(800, productInfo.Images[0].Data.Width);
            Assert.AreEqual(600, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.Images[1]);
            Assert.IsNotNull(productInfo.Images[1].Data);
            Assert.AreEqual(800, productInfo.Images[1].Data.Width);
            Assert.AreEqual(600, productInfo.Images[1].Data.Height);

            Assert.IsNotNull(productInfo.Images[2]);
            Assert.IsNotNull(productInfo.Images[2].Data);
            Assert.AreEqual(800, productInfo.Images[2].Data.Width);
            Assert.AreEqual(600, productInfo.Images[2].Data.Height);

            Assert.IsNotNull(productInfo.Images[3]);
            Assert.IsNotNull(productInfo.Images[3].Data);
            Assert.AreEqual(800, productInfo.Images[3].Data.Width);
            Assert.AreEqual(600, productInfo.Images[3].Data.Height);
        }

        [TestMethod]
        public void ProductWithColorSelectorTest()
        {
            const string url = @"http://www.votonia.ru/igrushka-dlya-vanni-uf-ribka-plavayushaya-z-fish-podsvetka-led/spb/86952/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(4, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Игрушка для ванны UF, Рыбка плавающая Z-fish, подсветка (led), красный", productInfo.ProductName);
            Assert.AreEqual("Китай", productInfo.ProducerCountryName);
            Assert.AreEqual("UF", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00080836", productInfo.Article);
            Assert.AreEqual("Игрушки>Игрушки для всех>Игрушки для ванны", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(334.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Робо-рыбка с двигающимся хвостом и ртом, с подсветкой, верхний плавник так же может отклоняться влево-вправо, придавая рыбке более естественную манеру поведения.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual("рыбка, 4 батарейки (2 в рыбке + 2 запасные), мини-отвертка, водоросль на присоске.", productInfo.Options.Complectation);
            Assert.AreEqual("красный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("0,040 кг", productInfo.Options.Characteristics.Weight);
            Assert.AreEqual("13*2,5*17 см", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("пластик, электронные компоненты", productInfo.Options.Characteristics.Structure);
            Assert.AreEqual("с 3 лет", productInfo.Options.Characteristics.Age);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(1867, productInfo.Images[0].Data.Width);
            Assert.AreEqual(1400, productInfo.Images[0].Data.Height);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Игрушка для ванны UF, Рыбка плавающая Z-fish, подсветка (led), синий", productInfo.ProductName);
            Assert.AreEqual("Китай", productInfo.ProducerCountryName);
            Assert.AreEqual("UF", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00080837", productInfo.Article);
            Assert.AreEqual("Игрушки>Игрушки для всех>Игрушки для ванны", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(334.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Робо-рыбка с двигающимся хвостом и ртом, с подсветкой, верхний плавник так же может отклоняться влево-вправо, придавая рыбке более естественную манеру поведения.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual("рыбка, 4 батарейки (2 в рыбке + 2 запасные), мини-отвертка, водоросль на присоске.", productInfo.Options.Complectation);
            Assert.AreEqual("синий", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("0,040 кг", productInfo.Options.Characteristics.Weight);
            Assert.AreEqual("13*2,5*17 см", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("пластик, электронные компоненты", productInfo.Options.Characteristics.Structure);
            Assert.AreEqual("с 3 лет", productInfo.Options.Characteristics.Age);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(2073, productInfo.Images[0].Data.Width);
            Assert.AreEqual(1555, productInfo.Images[0].Data.Height);

            productInfo = result.Data[2];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Игрушка для ванны UF, Рыбка плавающая Z-fish, подсветка (led), желтый", productInfo.ProductName);
            Assert.AreEqual("Китай", productInfo.ProducerCountryName);
            Assert.AreEqual("UF", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00080838", productInfo.Article);
            Assert.AreEqual("Игрушки>Игрушки для всех>Игрушки для ванны", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(334.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Робо-рыбка с двигающимся хвостом и ртом, с подсветкой, верхний плавник так же может отклоняться влево-вправо, придавая рыбке более естественную манеру поведения.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual("рыбка, 4 батарейки (2 в рыбке + 2 запасные), мини-отвертка, водоросль на присоске.", productInfo.Options.Complectation);
            Assert.AreEqual("желтый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("0,040 кг", productInfo.Options.Characteristics.Weight);
            Assert.AreEqual("13*2,5*17 см", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("пластик, электронные компоненты", productInfo.Options.Characteristics.Structure);
            Assert.AreEqual("с 3 лет", productInfo.Options.Characteristics.Age);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(2073, productInfo.Images[0].Data.Width);
            Assert.AreEqual(1555, productInfo.Images[0].Data.Height);

            productInfo = result.Data[3];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Игрушка для ванны UF, Рыбка плавающая Z-fish, подсветка (led), зеленый", productInfo.ProductName);
            Assert.AreEqual("Китай", productInfo.ProducerCountryName);
            Assert.AreEqual("UF", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00080839", productInfo.Article);
            Assert.AreEqual("Игрушки>Игрушки для всех>Игрушки для ванны", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(334.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Робо-рыбка с двигающимся хвостом и ртом, с подсветкой, верхний плавник так же может отклоняться влево-вправо, придавая рыбке более естественную манеру поведения.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual("рыбка, 4 батарейки (2 в рыбке + 2 запасные), мини-отвертка, водоросль на присоске.", productInfo.Options.Complectation);
            Assert.AreEqual("зеленый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("0,040 кг", productInfo.Options.Characteristics.Weight);
            Assert.AreEqual("13*2,5*17 см", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("пластик, электронные компоненты", productInfo.Options.Characteristics.Structure);
            Assert.AreEqual("с 3 лет", productInfo.Options.Characteristics.Age);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(2073, productInfo.Images[0].Data.Width);
            Assert.AreEqual(1555, productInfo.Images[0].Data.Height);
        }

        [TestMethod]
        public void ProductWithColorAndSizeSelectorsTest()
        {
            const string url = @"http://www.votonia.ru/tufli-gimnasticheskie-cheshki-malodetskie/spb/37009";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(10, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, черный, р.14,5", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010692", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("черный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("14,5", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, черный, р.15", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010711", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("черный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("15", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);

            productInfo = result.Data[2];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, черный, р.15,5", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010712", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("черный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("15,5", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);

            productInfo = result.Data[3];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, черный, р.16", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010713", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("черный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("16", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);

            productInfo = result.Data[4];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, черный, р.16,5", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010714", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("черный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("16,5", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);

            productInfo = result.Data[5];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, белый, р.14,5", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010691", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("белый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("14,5", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);

            productInfo = result.Data[6];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, белый, р.15", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010705", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("белый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("15", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);

            productInfo = result.Data[7];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, белый, р.15,5", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010706", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("белый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("15,5", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);

            productInfo = result.Data[8];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, белый, р.16", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010707", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("белый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("16", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);

            productInfo = result.Data[9];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Туфли гимнастические чешки малодетские, белый, р.16,5", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Скороход", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00010708", productInfo.Article);
            Assert.AreEqual("Обувь>Чешки, балетки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(203.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("белый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("16,5", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Для занятий гимнастикой и танцами ребенку требуются чешки – специальная мягкая обувь на очень тонкой и гибкой подошве, ведь во время интенсивных занятий детским ножкам нужен максимальный комфорт!  Чешки \"Скороход\" - это качество и надежность по приемлемым ценам.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("натуральная кожа", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
        }

        [TestMethod]
        public void ProductWithColorAndSizeSelectorsDynamicSizeTest()
        {
            const string url = @"http://www.votonia.ru/sabo-vs-dlya-malisheiy/spb/53978/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(14, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062904", productInfo.Article);
            Assert.AreEqual(299.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("розовый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("24", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062905", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("розовый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("25", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[2];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062906", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("розовый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("26", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[3];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062907", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("розовый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("27", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[4];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062908", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("розовый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("28", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[5];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062895", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("синий", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("25", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[6];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062896", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("синий", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("26", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[7];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062897", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("синий", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("27", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[8];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062898", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("синий", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("28", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[9];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062899", productInfo.Article);
            Assert.AreEqual(629.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("зеленый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("24", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[10];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062900", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("зеленый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("25", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[11];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062901", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("зеленый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("26", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[12];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062902", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("зеленый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("27", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[13];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062903", productInfo.Article);
            Assert.AreEqual(621.90, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("зеленый", productInfo.Options.Characteristics.Color);
            Assert.AreEqual("28", productInfo.Options.Characteristics.Size);
        }

        [TestMethod]
        public void ProductWithSizeSelectorAndColorModifierTest()
        {
            const string url = @"http://www.votonia.ru/chepchik-leo/spb/28999/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Чепчик Лео, р.38", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Лео", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00012927", productInfo.Article);
            Assert.AreEqual("Одежда>Одежда для малышей до 1 года>Чепчики, шапочки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(57.60, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Нежный чепчик прикроет ушки вашего малыша и предохранит его головку от переохлаждения.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("Цвета чепчиков представлены в ассортименте. Просьба уточнять имеющийся в наличии вариант у нашего оператора.", productInfo.Options.Warning);
            Assert.AreEqual("Определение размера: Размер головного убора для детей обычно определяют по объему головы ребенка. Для выбора подходящего размера рекомендуем вам воспользоваться", productInfo.Options.AdditionalInfo);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual("38", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("100% хлопок", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(4, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(992, productInfo.Images[0].Data.Width);
            Assert.AreEqual(744, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.Images[1]);
            Assert.IsNotNull(productInfo.Images[1].Data);
            Assert.AreEqual(1016, productInfo.Images[1].Data.Width);
            Assert.AreEqual(762, productInfo.Images[1].Data.Height);

            Assert.IsNotNull(productInfo.Images[2]);
            Assert.IsNotNull(productInfo.Images[2].Data);
            Assert.AreEqual(1016, productInfo.Images[2].Data.Width);
            Assert.AreEqual(762, productInfo.Images[2].Data.Height);

            Assert.IsNotNull(productInfo.Images[3]);
            Assert.IsNotNull(productInfo.Images[3].Data);
            Assert.AreEqual(992, productInfo.Images[3].Data.Width);
            Assert.AreEqual(744, productInfo.Images[3].Data.Height);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Чепчик Лео, р.40", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Лео", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00012928", productInfo.Article);
            Assert.AreEqual("Одежда>Одежда для малышей до 1 года>Чепчики, шапочки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(57.60, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Нежный чепчик прикроет ушки вашего малыша и предохранит его головку от переохлаждения.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual("Цвета чепчиков представлены в ассортименте. Просьба уточнять имеющийся в наличии вариант у нашего оператора.", productInfo.Options.Warning);
            Assert.AreEqual("Определение размера: Размер головного убора для детей обычно определяют по объему головы ребенка. Для выбора подходящего размера рекомендуем вам воспользоваться", productInfo.Options.AdditionalInfo);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual("40", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("100% хлопок", productInfo.Options.Characteristics.Structure);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(4, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(992, productInfo.Images[0].Data.Width);
            Assert.AreEqual(744, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.Images[1]);
            Assert.IsNotNull(productInfo.Images[1].Data);
            Assert.AreEqual(1016, productInfo.Images[1].Data.Width);
            Assert.AreEqual(762, productInfo.Images[1].Data.Height);

            Assert.IsNotNull(productInfo.Images[2]);
            Assert.IsNotNull(productInfo.Images[2].Data);
            Assert.AreEqual(1016, productInfo.Images[2].Data.Width);
            Assert.AreEqual(762, productInfo.Images[2].Data.Height);

            Assert.IsNotNull(productInfo.Images[3]);
            Assert.IsNotNull(productInfo.Images[3].Data);
            Assert.AreEqual(992, productInfo.Images[3].Data.Width);
            Assert.AreEqual(744, productInfo.Images[3].Data.Height);
        }

        [TestMethod]
        public void ProductWithSizeSelectorAndColorModifierAndRotatedSizeRangeTest()
        {
            const string url = @"http://www.votonia.ru/polzunki-ip-zaiytzev-interlok-safari/spb/29641/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(4, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00027887", productInfo.Article);
            Assert.AreEqual(317.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("56-36", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00027888", productInfo.Article);
            Assert.AreEqual(317.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("62-40", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[2];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00027889", productInfo.Article);
            Assert.AreEqual(317.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("68-44", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[3];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00027890", productInfo.Article);
            Assert.AreEqual(317.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("74-48", productInfo.Options.Characteristics.Size);
        }

        [TestMethod]
        public void ProductWithSizeSelectorAndColorModifierAndRotatedSizeAgeRangeTest()
        {
            const string url = @"http://www.votonia.ru/kombinezon-iz-shersti-merinosa-v-seruyu-polosku/spb/34336/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(4, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00013890", productInfo.Article);
            Assert.AreEqual(1611.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("0-3 мес.", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00013891", productInfo.Article);
            Assert.AreEqual(1611.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("3-6 мес.", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[2];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00013892", productInfo.Article);
            Assert.AreEqual(1611.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("6-12 мес.", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[3];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00013893", productInfo.Article);
            Assert.AreEqual(1611.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("12-18 мес", productInfo.Options.Characteristics.Size);
        }

        [TestMethod]
        public void ProductWithSizeSelectorAndColorModifierAndColorInAssortmentTest()
        {
            const string url = @"http://www.votonia.ru/pijama-ip-zaiytzev-trikotaj/spb/46237/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(4, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00006061", productInfo.Article);
            Assert.AreEqual(297.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("74-48", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00006062", productInfo.Article);
            Assert.AreEqual(297.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("80-52", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[2];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00006063", productInfo.Article);
            Assert.AreEqual(297.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("92-56", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[3];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00006064", productInfo.Article);
            Assert.AreEqual(314.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.Color, productInfo.Options.Modifiers);
            Assert.AreEqual("116-60", productInfo.Options.Characteristics.Size);
        }

        [TestMethod]
        public void ProductWithColorSelectorAndOnePredefinedColorTest()
        {
            const string url = @"http://www.votonia.ru/pijama-disney-dlya-malchikov/spb/53918/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(4, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062404", productInfo.Article);
            Assert.AreEqual(416.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("красный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.None, productInfo.Options.Modifiers);
            Assert.AreEqual("98-104", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Стойкие цвета и отделка. Пижаму легко стираеть, она долго сохраняет свой внешний вид при соблюдении режима стирки.\r\nХорошо сидит, имеет удобный крой - отличный вариант для крепкого сна.\r\nЦвет: красный, аппликация Тачки в ассортименте. Просьба уточнять желаемый вами вариант при оформлении заказа или у нашего оператора.", productInfo.Options.AdditionalInfo);
            Assert.AreEqual("Детская одежда Дисней уже давно завоевала доверие родителей. Производитель доказал, что не только яркие и красочные аппликации и принты любимых героев являются его отличительной особенностью. Если вы решили купить одежду Дисней, вы сделали правильный выбор.", productInfo.Options.Description);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062405", productInfo.Article);
            Assert.AreEqual(416.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("красный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.None, productInfo.Options.Modifiers);
            Assert.AreEqual("104-110", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Стойкие цвета и отделка. Пижаму легко стираеть, она долго сохраняет свой внешний вид при соблюдении режима стирки.\r\nХорошо сидит, имеет удобный крой - отличный вариант для крепкого сна.\r\nЦвет: красный, аппликация Тачки в ассортименте. Просьба уточнять желаемый вами вариант при оформлении заказа или у нашего оператора.", productInfo.Options.AdditionalInfo);
            Assert.AreEqual("Детская одежда Дисней уже давно завоевала доверие родителей. Производитель доказал, что не только яркие и красочные аппликации и принты любимых героев являются его отличительной особенностью. Если вы решили купить одежду Дисней, вы сделали правильный выбор.", productInfo.Options.Description);

            productInfo = result.Data[2];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062406", productInfo.Article);
            Assert.AreEqual(416.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("красный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.None, productInfo.Options.Modifiers);
            Assert.AreEqual("110-116", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Стойкие цвета и отделка. Пижаму легко стираеть, она долго сохраняет свой внешний вид при соблюдении режима стирки.\r\nХорошо сидит, имеет удобный крой - отличный вариант для крепкого сна.\r\nЦвет: красный, аппликация Тачки в ассортименте. Просьба уточнять желаемый вами вариант при оформлении заказа или у нашего оператора.", productInfo.Options.AdditionalInfo);
            Assert.AreEqual("Детская одежда Дисней уже давно завоевала доверие родителей. Производитель доказал, что не только яркие и красочные аппликации и принты любимых героев являются его отличительной особенностью. Если вы решили купить одежду Дисней, вы сделали правильный выбор.", productInfo.Options.Description);

            productInfo = result.Data[3];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00062407", productInfo.Article);
            Assert.AreEqual(416.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("красный", productInfo.Options.Characteristics.Color);
            Assert.AreEqual(ProductCharacteristicModifiers.None, productInfo.Options.Modifiers);
            Assert.AreEqual("116-122", productInfo.Options.Characteristics.Size);
            Assert.AreEqual("Стойкие цвета и отделка. Пижаму легко стираеть, она долго сохраняет свой внешний вид при соблюдении режима стирки.\r\nХорошо сидит, имеет удобный крой - отличный вариант для крепкого сна.\r\nЦвет: красный, аппликация Тачки в ассортименте. Просьба уточнять желаемый вами вариант при оформлении заказа или у нашего оператора.", productInfo.Options.AdditionalInfo);
            Assert.AreEqual("Детская одежда Дисней уже давно завоевала доверие родителей. Производитель доказал, что не только яркие и красочные аппликации и принты любимых героев являются его отличительной особенностью. Если вы решили купить одежду Дисней, вы сделали правильный выбор.", productInfo.Options.Description);
        }

        [TestMethod]
        public void ProductNoCharacteristicsTest()
        {
            const string url = @"http://www.votonia.ru/model-welly-mashina-bmw-x5-1:24/spb/11194/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Модель Welly, машина BMW X5, 1:24", productInfo.ProductName);
            Assert.AreEqual("Китай", productInfo.ProducerCountryName);
            Assert.AreEqual("Welly", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00017934", productInfo.Article);
            Assert.AreEqual("Игрушки>Игрушки для мальчиков>Транспорт", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(749.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Коллекционная модель машины масштаба 1:24 BMW X5. Функции: открываются передние двери, капот.", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual(null, productInfo.Options.Warning);
            Assert.AreEqual(null, productInfo.Options.AdditionalInfo);
            Assert.AreEqual(ProductCharacteristicModifiers.None, productInfo.Options.Modifiers);
            Assert.AreEqual(0, productInfo.Options.Characteristics.Count);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(667, productInfo.Images[0].Data.Width);
            Assert.AreEqual(500, productInfo.Images[0].Data.Height);
        }

        [TestMethod]
        public void ProductMultipleAdditionalInfoTest()
        {
            const string url = @"http://www.votonia.ru/igra-educa-minitachpad-ya-uchu-alfavit/spb/15364/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Игра Educa, Минитачпад - Я учу алфавит", productInfo.ProductName);
            Assert.AreEqual("Китай", productInfo.ProducerCountryName);
            Assert.AreEqual("Educa", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00032859", productInfo.Article);
            Assert.AreEqual("Игрушки>Игрушки для всех>Электронные игрушки", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(1199.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("Выучи буквы и научись читать первые слова!", productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual(null, productInfo.Options.Warning);
            Assert.AreEqual("Играя в игру \"Я учу алфавит\" дети научатся узнавать буквы, распознавать их элементы, ассоциировать слово с картинкой, читать первые слова, называть предметы, встречающиеся в повседневной жизни.\r\nВ комплект входит 12 карточек с увлекательными играми и заданиями разной сложности.\r\nМожно играть одному или с друзьями.\r\nПитание от 3-х батареек ААА (в комплект не входят).", productInfo.Options.AdditionalInfo);
            Assert.AreEqual(ProductCharacteristicModifiers.None, productInfo.Options.Modifiers);
            Assert.AreEqual("музыка, обучение, игра в вопросы и ответы.", productInfo.Options.Characteristics.Mode);
            Assert.AreEqual("300 г", productInfo.Options.Characteristics.Weight);
            Assert.AreEqual("пластмасса", productInfo.Options.Characteristics.Structure);
            Assert.AreEqual("от 4-5 лет", productInfo.Options.Characteristics.Age);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(1067, productInfo.Images[0].Data.Width);
            Assert.AreEqual(800, productInfo.Images[0].Data.Height);
        }

        [TestMethod]
        public void ProductWithoutDescriptionTest()
        {
            const string url = @"http://www.votonia.ru/detskoe-uderjivayushee-ustroiystvo-vitalfarm/spb/12166/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Детское удерживающее устройство Виталфарм, красный", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Виталфарм", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00005684", productInfo.Article);
            Assert.AreEqual("Автокресла>Аксессуары для автокресел", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(428.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual(null, productInfo.Options.Warning);
            Assert.AreEqual(ProductCharacteristicModifiers.None, productInfo.Options.Modifiers);
            Assert.AreEqual("3 кг.", productInfo.Options.Characteristics.Weight);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Детское удерживающее устройство Виталфарм, синий", productInfo.ProductName);
            Assert.AreEqual("Россия", productInfo.ProducerCountryName);
            Assert.AreEqual("Виталфарм", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00016200", productInfo.Article);
            Assert.AreEqual("Автокресла>Аксессуары для автокресел", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(428.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual(null, productInfo.Options.Description);
            Assert.AreEqual(null, productInfo.Options.Instruction);
            Assert.AreEqual(null, productInfo.Options.Complectation);
            Assert.AreEqual(null, productInfo.Options.Warning);
            Assert.AreEqual(ProductCharacteristicModifiers.None, productInfo.Options.Modifiers);
            Assert.AreEqual("3 кг.", productInfo.Options.Characteristics.Weight);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
        }

        [TestMethod]
        public void ProductWithDifferentCostPerColorAndOneProductOutOfStockTest()
        {
            const string url = @"http://www.votonia.ru/avtokreslo-romer-king-ii-ls/spb/85907";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(8, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.IsFalse(productInfo.OutOfStock);
            Assert.AreEqual("Автокресло Romer, King II, LS, big giraffe", productInfo.ProductName);
            Assert.AreEqual("Германия", productInfo.ProducerCountryName);
            Assert.AreEqual("Romer", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00079808", productInfo.Article);
            Assert.AreEqual("Автокресла>Автокресла с ременным креплением>Автокресла от 9 до 18 кг", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(19660.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);

            Assert.IsFalse(productInfo.OutOfStock);
            Assert.AreEqual("Автокресло Romer, King II, LS, black thunder", productInfo.ProductName);
            Assert.AreEqual("Германия", productInfo.ProducerCountryName);
            Assert.AreEqual("Romer", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00079809", productInfo.Article);
            Assert.AreEqual("Автокресла>Автокресла с ременным креплением>Автокресла от 9 до 18 кг", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(18560.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            productInfo = result.Data[2];
            Assert.IsNotNull(productInfo);

            Assert.IsFalse(productInfo.OutOfStock);
            Assert.AreEqual("Автокресло Romer, King II, LS, chili pepper", productInfo.ProductName);
            Assert.AreEqual("Германия", productInfo.ProducerCountryName);
            Assert.AreEqual("Romer", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00079810", productInfo.Article);
            Assert.AreEqual("Автокресла>Автокресла с ременным креплением>Автокресла от 9 до 18 кг", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(18560.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            productInfo = result.Data[3];
            Assert.IsNotNull(productInfo);

            Assert.IsFalse(productInfo.OutOfStock);
            Assert.AreEqual("Автокресло Romer, King II, LS, crown blue", productInfo.ProductName);
            Assert.AreEqual("Германия", productInfo.ProducerCountryName);
            Assert.AreEqual("Romer", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00079811", productInfo.Article);
            Assert.AreEqual("Автокресла>Автокресла с ременным креплением>Автокресла от 9 до 18 кг", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(17595.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            productInfo = result.Data[4];
            Assert.IsNotNull(productInfo);

            Assert.IsFalse(productInfo.OutOfStock);
            Assert.AreEqual("Автокресло Romer, King II, LS, dark grape", productInfo.ProductName);
            Assert.AreEqual("Германия", productInfo.ProducerCountryName);
            Assert.AreEqual("Romer", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00079812", productInfo.Article);
            Assert.AreEqual("Автокресла>Автокресла с ременным креплением>Автокресла от 9 до 18 кг", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(18560.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            //OOut Of stock:
            productInfo = result.Data[5];

            Assert.IsTrue(productInfo.OutOfStock);
            Assert.AreEqual("Автокресло Romer, King II, LS, magic dots", productInfo.ProductName);
            Assert.AreEqual("Германия", productInfo.ProducerCountryName);
            Assert.AreEqual("Romer", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00079813", productInfo.Article);
            Assert.AreEqual("Автокресла>Автокресла с ременным креплением>Автокресла от 9 до 18 кг", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(18560.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            productInfo = result.Data[6];

            Assert.IsFalse(productInfo.OutOfStock);
            Assert.AreEqual("Автокресло Romer, King II, LS, smart zebra", productInfo.ProductName);
            Assert.AreEqual("Германия", productInfo.ProducerCountryName);
            Assert.AreEqual("Romer", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00079814", productInfo.Article);
            Assert.AreEqual("Автокресла>Автокресла с ременным креплением>Автокресла от 9 до 18 кг", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(19660.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);

            productInfo = result.Data[7];

            Assert.IsFalse(productInfo.OutOfStock);
            Assert.AreEqual("Автокресло Romer, King II, LS, taupe grey", productInfo.ProductName);
            Assert.AreEqual("Германия", productInfo.ProducerCountryName);
            Assert.AreEqual("Romer", productInfo.ProducerCompanyName);
            Assert.AreEqual("1-00079815", productInfo.Article);
            Assert.AreEqual("Автокресла>Автокресла с ременным креплением>Автокресла от 9 до 18 кг", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(18560.00, productInfo.ProductCost);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
        }

        [TestMethod]
        public void ProductWithZeroSize()
        {
            const string url = @"http://www.votonia.ru/chepchik-ip-zaiytzev-trikotaj/spb/2605/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(3, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00004914", productInfo.Article);
            Assert.AreEqual(34, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("0", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00004760", productInfo.Article);
            Assert.AreEqual(34.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("1", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[2];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00004915", productInfo.Article);
            Assert.AreEqual(34.00, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("2", productInfo.Options.Characteristics.Size);
        }

        [TestMethod]
        public void ProductWithRangeSize()
        {
            const string url = @"http://www.votonia.ru/noski-gamma-h-b-s-tzvetnimi-polosami/spb/37861/";

            var productParser = new VotoniaProductParser();
            ParseResult<VotoniaSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data.Length);

            VotoniaSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00002542", productInfo.Article);
            Assert.AreEqual(43, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("12-14", productInfo.Options.Characteristics.Size);

            productInfo = result.Data[1];
            Assert.IsNotNull(productInfo);
            Assert.AreEqual("1-00002543", productInfo.Article);
            Assert.AreEqual(43, productInfo.ProductCost);
            Assert.IsNotNull(productInfo.Options);
            Assert.AreEqual("16-18", productInfo.Options.Characteristics.Size);
        }

        #endregion

        #region Catalog Parses

        [TestMethod]
        public void CatalogTest()
        {
            const string url = @"http://www.votonia.ru/catalog/igrushki/igrushki-dlya-vseh/?page=75";
            var parser = new VotoniaCatalogParser();
            ParseResult<CatalogPageInfo> result = parser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            CatalogPageInfo pageInfo = result.Data[0];
            Assert.IsNotNull(pageInfo);
            Assert.AreEqual(6, pageInfo.PageNumber);
            Assert.IsNotNull(pageInfo.NextPageLink);
            Assert.AreEqual("http://www.votonia.ru/catalog/igrushki/igrushki-dlya-vseh/?page=90", pageInfo.NextPageLink.Url);
            Assert.IsNotNull(pageInfo.PreviousPageLink);
            Assert.AreEqual("http://www.votonia.ru/catalog/igrushki/igrushki-dlya-vseh/?page=60", pageInfo.PreviousPageLink.Url);
        }

        [TestMethod]
        public void CatalogPage150Test()
        {
            const string url = @"http://www.votonia.ru/catalog/igrushki/igrushki-dlya-vseh/?page=150";
            var parser = new VotoniaCatalogParser();
            ParseResult<CatalogPageInfo> result = parser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            CatalogPageInfo pageInfo = result.Data[0];
            Assert.IsNotNull(pageInfo);
            Assert.AreEqual(11, pageInfo.PageNumber);
            Assert.IsNotNull(pageInfo.NextPageLink);
            Assert.AreEqual("http://www.votonia.ru/catalog/igrushki/igrushki-dlya-vseh/?page=165", pageInfo.NextPageLink.Url);
            Assert.IsNotNull(pageInfo.PreviousPageLink);
            Assert.AreEqual("http://www.votonia.ru/catalog/igrushki/igrushki-dlya-vseh/?page=135", pageInfo.PreviousPageLink.Url);
        }

        [TestMethod]
        public void CatalogPage195Test()
        {
            const string url = @"http://www.votonia.ru/catalog/igrushki/igrushki-dlya-vseh/?page=195";
            var parser = new VotoniaCatalogParser();
            ParseResult<CatalogPageInfo> result = parser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
        }

        [TestMethod]
        public void CatalogContainerParseTest()
        {
            const string url = @"http://www.votonia.ru/catalog/";
            var parser = new VotoniaCatalogContainerParser();

            ParseResult<CatalogContainerInfo> result = parser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            CatalogContainerInfo info = result.Data[0];

            Assert.IsNotNull(info.Links);
            Assert.AreEqual(22, info.Links.Length);
        }

        //[TestMethod]
        //public void CatalogFetchAllPagesTest()
        //{
        //    const string url = @"http://www.votonia.ru/catalog/igrushki/igrushki-dlya-vseh/?page=75";
        //    var parser = new VotoniaCatalogParser();

        //    AbsoluteUri[] productReferences = parser.ListProductReferences(new AbsoluteUri(url));

        //    Assert.IsNotNull(productReferences);
        //    Assert.IsTrue(productReferences.Length > 0);

        //    string[] urls = productReferences.Select(item => item.Url).Distinct().ToArray();

        //    Assert.AreEqual(productReferences.Length, urls.Length);
        //}

        #endregion
    }
}