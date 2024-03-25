using System;
using APV.Pottle.WebParsers.Avtoberg;
using APV.Pottle.WebParsers.ResultEntities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;

namespace APV.Pottle.UnitTest.WebParsers
{
    [TestClass]
    public class AvtobergWebParserTests
    {
        #region Product Parser

        [TestMethod]
        public void ProductWithProducer()
        {
            const string url = @"http://avtoberg.ru/catalog/bmw/f01-f02-7-series-09/bmf0109-070-l-2.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Фара противотуманная левая", productInfo.ProductName);
            Assert.AreEqual("Фара противотуманная", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(2154.00, productInfo.ProductCost);
            Assert.AreEqual("BMW", productInfo.TrademarkName);
            Assert.AreEqual("F01 / F02 7 Series", productInfo.ModelName);
            Assert.AreEqual("2009-", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("2009-", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(false, productInfo.OutOfStock);
            Assert.AreEqual("1 день", productInfo.DeliveryPeriod);
            Assert.AreEqual("6317782195", productInfo.OriginalArticle);
            Assert.AreEqual("Depo", productInfo.ProducerCompanyName);
            Assert.AreEqual("444-2028L-UQ", productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(332, productInfo.Images[0].Data.Width);
            Assert.AreEqual(260, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(0, productInfo.BuyTogetherProductReferences.Length);
        }

        [TestMethod]
        public void ProductWithProducerOutOfStock()
        {
            const string url = @"http://avtoberg.ru/catalog/bmw/mini-cooper-countryman-10/mncoo10-000l-l.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("COOPER {COUNTRYMAN} ФАРА ЛЕВ С РЕГ.МОТОР УК.ПОВОР ПРОЗРАЧН", productInfo.ProductName);
            Assert.AreEqual("Фара", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(5584.00, productInfo.ProductCost);
            Assert.AreEqual("BMW", productInfo.TrademarkName);
            Assert.AreEqual("Mini Cooper Countryman", productInfo.ModelName);
            Assert.AreEqual("2010-", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("2010-", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(true, productInfo.OutOfStock);
            Assert.AreEqual(null, productInfo.DeliveryPeriod);
            Assert.AreEqual("63129801027", productInfo.OriginalArticle);
            Assert.AreEqual("Depo", productInfo.ProducerCompanyName);
            Assert.AreEqual("882-1124LMLDEMC", productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(258, productInfo.Images[0].Data.Width);
            Assert.AreEqual(268, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(0, productInfo.BuyTogetherProductReferences.Length);
        }

        [TestMethod]
        public void ProductWitoutProducer()
        {
            const string url = @"http://avtoberg.ru/catalog/audi/a8-94-02-03/vwpas97-813-l-3.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Рычаг передней подвески левый нижний сзади оригинальный", productInfo.ProductName);
            Assert.AreEqual("Прочие", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(1854.00, productInfo.ProductCost);
            Assert.AreEqual("Audi", productInfo.TrademarkName);
            Assert.AreEqual("A8", productInfo.ModelName);
            Assert.AreEqual("1994-2002, 2003-", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("1997-2003", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(false, productInfo.OutOfStock);
            Assert.AreEqual("1 день", productInfo.DeliveryPeriod);
            Assert.AreEqual("4D0407693E/4D0407693N/8E0407693E", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(364, productInfo.Images[0].Data.Width);
            Assert.AreEqual(154, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(0, productInfo.BuyTogetherProductReferences.Length);
        }

        [TestMethod]
        public void ProductWitoutProducerWithBuyTohether()
        {
            const string url = @"http://avtoberg.ru/catalog/volkswagen/golf-vii-12/vwglf12-330.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Капот", productInfo.ProductName);
            Assert.AreEqual("Капот", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(12316.00, productInfo.ProductCost);
            Assert.AreEqual("Volkswagen", productInfo.TrademarkName);
            Assert.AreEqual("Golf Vii", productInfo.ModelName);
            Assert.AreEqual("2012-", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("2012-", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(false, productInfo.OutOfStock);
            Assert.AreEqual("1 день", productInfo.DeliveryPeriod);
            Assert.AreEqual("5G0823031J", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(0, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(2, productInfo.BuyTogetherProductReferences.Length);
            Assert.IsNotNull(productInfo.BuyTogetherProductReferences[0]);
            Assert.AreEqual("http://avtoberg.ru/catalog/volkswagen/golf-vii-12/vwglf12-000b-l.html", productInfo.BuyTogetherProductReferences[0].ToString());
            Assert.IsNotNull(productInfo.BuyTogetherProductReferences[1]);
            Assert.AreEqual("http://avtoberg.ru/catalog/volkswagen/golf-vii-12/vwglf12-000b-r.html", productInfo.BuyTogetherProductReferences[1].ToString());
        }

        [TestMethod]
        public void ProductWitoutProducerWithComplexPeriodWithBuyTohether()
        {
            const string url = @"http://avtoberg.ru/catalog/audi/a8-94-02-03/vwpas00-810-z-1.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Рычаг передней подвески левый нижний спереди", productInfo.ProductName);
            Assert.AreEqual("Прочие", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(2927.00, productInfo.ProductCost);
            Assert.AreEqual("Audi", productInfo.TrademarkName);
            Assert.AreEqual("A8", productInfo.ModelName);
            Assert.AreEqual("1994-2002, 2003-", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("2000-2005", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(false, productInfo.OutOfStock);
            Assert.AreEqual("7 дней", productInfo.DeliveryPeriod);
            Assert.AreEqual("4B3407151C/4B3407151F/4B3407151K", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(368, productInfo.Images[0].Data.Width);
            Assert.AreEqual(212, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(4, productInfo.BuyTogetherProductReferences.Length);
            Assert.IsNotNull(productInfo.BuyTogetherProductReferences[0]);
            Assert.AreEqual("http://avtoberg.ru/catalog/audi/a8-94-02-03/ai0a804-070-r-1.html", productInfo.BuyTogetherProductReferences[0].ToString());
            Assert.IsNotNull(productInfo.BuyTogetherProductReferences[1]);
            Assert.AreEqual("http://avtoberg.ru/catalog/audi/a8-94-02-03/ai0a894-300-r.html", productInfo.BuyTogetherProductReferences[1].ToString());
            Assert.IsNotNull(productInfo.BuyTogetherProductReferences[2]);
            Assert.AreEqual("http://avtoberg.ru/catalog/audi/a8-94-02-03/vwpas97-814-r-3.html", productInfo.BuyTogetherProductReferences[2].ToString());
            Assert.IsNotNull(productInfo.BuyTogetherProductReferences[3]);
            Assert.AreEqual("http://avtoberg.ru/catalog/audi/a8-94-02-03/ai0a803-910.html", productInfo.BuyTogetherProductReferences[3].ToString());
        }

        [TestMethod]
        public void ProductWitoutProducerNoImages()
        {
            const string url = @"http://avtoberg.ru/catalog/audi/a8-94-02-03/ai0a894-300-r.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Подкрылок переднего крыла правый передняя часть", productInfo.ProductName);
            Assert.AreEqual("Локер", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(1005.00, productInfo.ProductCost);
            Assert.AreEqual("Audi", productInfo.TrademarkName);
            Assert.AreEqual("A8", productInfo.ModelName);
            Assert.AreEqual("1994-2002, 2003-", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("1994-2003", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(false, productInfo.OutOfStock);
            Assert.AreEqual("7 дней", productInfo.DeliveryPeriod);
            Assert.AreEqual("4D0821172J", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(0, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(0, productInfo.BuyTogetherProductReferences.Length);
        }

        [TestMethod]
        public void ProductWitoutProducerOutOfStock()
        {
            const string url = @"http://avtoberg.ru/catalog/chrysler/concord-98-04-lhs-99-04/crcon98-100hg.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Решетка радиатора в сборе хромированно-серая", productInfo.ProductName);
            Assert.AreEqual("Решетка радиатора", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(2854.00, productInfo.ProductCost);
            Assert.AreEqual("Chrysler", productInfo.TrademarkName);
            Assert.AreEqual("Concord / Lhs", productInfo.ModelName);
            Assert.AreEqual("1998-2004, 1999-2004", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("1998-2001", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(true, productInfo.OutOfStock);
            Assert.AreEqual(null, productInfo.DeliveryPeriod);
            Assert.AreEqual("4574849", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(409, productInfo.Images[0].Data.Width);
            Assert.AreEqual(122, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(0, productInfo.BuyTogetherProductReferences.Length);
        }

        [TestMethod]
        public void ProductWitoutProducerOutOfStockWithBuyTohether()
        {
            const string url = @"http://avtoberg.ru/catalog/volkswagen/golf-vii-12/vwglf12-450-l.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("GOLF ЗЕРКАЛО ЛЕВ ЭЛЕКТР С ПОДОГРЕВ , ДИОД УК.ПОВОР 6 КОНТ (aspherical)", productInfo.ProductName);
            Assert.AreEqual("Зеркало", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(0.00, productInfo.ProductCost);
            Assert.AreEqual("Volkswagen", productInfo.TrademarkName);
            Assert.AreEqual("Golf Vii", productInfo.ModelName);
            Assert.AreEqual("2012-", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("2012-", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(true, productInfo.OutOfStock);
            Assert.AreEqual(null, productInfo.DeliveryPeriod);
            Assert.AreEqual("5G1857507AF9B9+5G0857537CGRU+5G0857521", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(555, productInfo.Images[0].Data.Width);
            Assert.AreEqual(301, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(1, productInfo.BuyTogetherProductReferences.Length);
            Assert.IsNotNull(productInfo.BuyTogetherProductReferences[0]);
            Assert.AreEqual("http://avtoberg.ru/catalog/volkswagen/golf-vii-12/vwglf12-450-r.html", productInfo.BuyTogetherProductReferences[0].ToString());
        }

        [TestMethod]
        public void ProductWithoutOriginalArticle()
        {
            const string url = @"http://avtoberg.ru/catalog/audi/100-12-91-8-94/ai10091-030tt-l.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Указатель поворота угловой левый тонированый", productInfo.ProductName);
            Assert.AreEqual("Указатель поворота", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(650.00, productInfo.ProductCost);
            Assert.AreEqual("Audi", productInfo.TrademarkName);
            Assert.AreEqual("100", productInfo.ModelName);
            Assert.AreEqual("1991-1994", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("1991-1994", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(false, productInfo.OutOfStock);
            Assert.AreEqual("1 день", productInfo.DeliveryPeriod);
            Assert.AreEqual(null, productInfo.OriginalArticle);
            Assert.AreEqual("Depo", productInfo.ProducerCompanyName);
            Assert.AreEqual("441-1509L-BE-VS", productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(1, productInfo.Images.Length);
            Assert.IsNotNull(productInfo.Images[0]);
            Assert.IsNotNull(productInfo.Images[0].Data);
            Assert.AreEqual(150, productInfo.Images[0].Data.Width);
            Assert.AreEqual(104, productInfo.Images[0].Data.Height);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(0, productInfo.BuyTogetherProductReferences.Length);
        }

        [TestMethod]
        public void ProductWithBrokenImage()
        {
            const string url = @"http://avtoberg.ru/catalog/audi/100-12-91-8-94/ai10091-191-r.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);
        }

        [TestMethod]
        public void ProductWithoutModelPeriod()
        {
            const string url = @"http://avtoberg.ru/catalog/chrysler/cts-03-07-08/crcts08-450-l.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("CTS ЗЕРКАЛО ЛЕВ ЭЛЕКТР С ПОДОГРЕВ ПАМЯТЬЮ", productInfo.ProductName);
            Assert.AreEqual("Зеркало", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(4455.00, productInfo.ProductCost);
            Assert.AreEqual("Chrysler", productInfo.TrademarkName);
            Assert.AreEqual("Cts (03-07/08-)", productInfo.ModelName);
            Assert.AreEqual(null, productInfo.ModelPeriod);
            Assert.AreEqual("2008-", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(false, productInfo.OutOfStock);
            Assert.AreEqual("7 дней", productInfo.DeliveryPeriod);
            Assert.AreEqual("25828084", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(0, productInfo.Images.Length);
        }

        [TestMethod]
        public void ProductWithoutModelPeriodOutOfStock()
        {
            const string url = @"http://avtoberg.ru/catalog/mitsubishi-proton/mitsubishi-proton/mbotl13-740-r.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Фонарь задний внешний правый оригинальный", productInfo.ProductName);
            Assert.AreEqual("Фонарь", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(13359.00, productInfo.ProductCost);
            Assert.AreEqual("Mitsubishi / Proton", productInfo.TrademarkName);
            Assert.AreEqual("Mitsubishi / Proton", productInfo.ModelName);
            Assert.AreEqual(null, productInfo.ModelPeriod);
            Assert.AreEqual("2013-", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(true, productInfo.OutOfStock);
            Assert.AreEqual(null, productInfo.DeliveryPeriod);
            Assert.AreEqual("8330A788", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(0, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(0, productInfo.BuyTogetherProductReferences.Length);
        }

        [TestMethod]
        public void ProducWthoutDeliveryPeriod()
        {
            const string url = @"http://avtoberg.ru/catalog/chevrolet/gmc-savana-express-03-09/gmsav03-330.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("Капот MC SAVANA EXPRESS 03-09", productInfo.ProductName);
            Assert.AreEqual("Капот", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(10500.00, productInfo.ProductCost);
            Assert.AreEqual("Chevrolet", productInfo.TrademarkName);
            Assert.AreEqual("Gmc Savana Express", productInfo.ModelName);
            Assert.AreEqual("2003-2009", productInfo.ModelPeriod.ToString());
            Assert.AreEqual(null, productInfo.ProducedPeriod);
            Assert.AreEqual(false, productInfo.OutOfStock);
            Assert.AreEqual("1 день", productInfo.DeliveryPeriod);
            Assert.AreEqual("88944424", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);

            Assert.IsNotNull(productInfo.Images);
            Assert.AreEqual(0, productInfo.Images.Length);

            Assert.IsNotNull(productInfo.BuyTogetherProductReferences);
            Assert.AreEqual(0, productInfo.BuyTogetherProductReferences.Length);
        }

        [TestMethod]
        public void ProducWithWrongDeliveryPeriod()
        {
            const string url = @"http://avtoberg.ru/catalog/toyota/landcruiser-100-98/tylan02-762rt-n-1.html";

            var productParser = new AvtobergProductParser();
            ParseResult<AvtobergSupplierProductInfo> result = productParser.Parse(new AbsoluteUri(url));

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data.Length);

            AvtobergSupplierProductInfo productInfo = result.Data[0];
            Assert.IsNotNull(productInfo);

            Assert.AreEqual("LANDCRUISER ФОНАРЬ ЗАДН ВНЕШН+ВНУТР Л+П (КОМПЛЕКТ) ТЮНИНГ ПРОЗРАЧ ХРУСТАЛ С ДИОД ГАБАРИТ , СТОП СИГНАЛ (EAGLE EYES) КРАСН-ТОНИР", productInfo.ProductName);
            Assert.AreEqual("Фонарь", string.Join(">", productInfo.Navigator));
            Assert.AreEqual(12167.00, productInfo.ProductCost);
            Assert.AreEqual("Toyota", productInfo.TrademarkName);
            Assert.AreEqual("Landcruiser 100", productInfo.ModelName);
            Assert.AreEqual("1998-", productInfo.ModelPeriod.ToString());
            Assert.AreEqual("2000-2002", productInfo.ProducedPeriod.ToString());
            Assert.AreEqual(true, productInfo.OutOfStock);
            Assert.AreEqual(null, productInfo.DeliveryPeriod);
            Assert.AreEqual("8155060680+8156060600+8159060100+8158060070", productInfo.OriginalArticle);
            Assert.AreEqual(null, productInfo.ProducerCompanyName);
            Assert.AreEqual(null, productInfo.Article);
        }

        [TestMethod]
        public void ComplexImageLoadingTest()
        {
            const string url = @"http://avtoberg.ru/files/parts/AI10091-191-R.jpg";

            byte[] originalData = Resources.ResourceManager.Ai10091191R;
            byte[] remoteData = new Uri(url).GetData();

            Assert.AreEqual(originalData.Length, remoteData.Length);
            for (int i = 0; i < originalData.Length; i++)
            {
                Assert.AreEqual(originalData[i], remoteData[i]);
            }
        }

        #endregion
    }
}