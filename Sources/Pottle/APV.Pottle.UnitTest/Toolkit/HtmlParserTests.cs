using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using APV.Common;
using APV.Common.Html;

namespace APV.Pottle.UnitTest.Toolkit
{
    [TestClass]
    public class HtmlParserTests
    {
        [TestMethod]
        public void SimpleHtmlParseTest()
        {
            const string html = "<html><head>HEAD</head><body>BODY</body></html>";

            HtmlDocument tag = HtmlParser.Parse(html);

            Assert.IsNotNull(tag);
            Assert.AreEqual("html", tag.Children[0].TagName);
            Assert.AreEqual(2, tag.Children[0].Children.Count);
            Assert.AreEqual("head", tag.Children[0].Children[0].TagName);
            Assert.AreEqual(0, tag.Children[0].Children[0].Children.Count);
            Assert.AreEqual("HEAD", tag.Children[0].Children[0].InnerText);
            Assert.AreEqual("HEAD", tag.Children[0].Children[0].InnerHtml);
            Assert.AreEqual("body", tag.Children[0].Children[1].TagName);
            Assert.AreEqual(0, tag.Children[0].Children[1].Children.Count);
            Assert.AreEqual("BODY", tag.Children[0].Children[1].InnerText);
            Assert.AreEqual("BODY", tag.Children[0].Children[1].InnerHtml);
        }

        [TestMethod]
        public void TagWithTextAndChildrenHtmlParseTest()
        {
            const string html = "<html><body><span>TEXT1<span>INNER SPAN1</span><span>INNER SPAN2</span>TEXT2</span></body></html>";

            HtmlDocument tag = HtmlParser.Parse(html);

            Assert.IsNotNull(tag);
            Assert.AreEqual("html", tag.Children[0].TagName);
            Assert.AreEqual(1, tag.Children[0].Children.Count);
            Assert.AreEqual("body", tag.Children[0].Children[0].TagName);
            Assert.AreEqual(1, tag.Children[0].Children[0].Children.Count);
            Assert.AreEqual("span", tag.Children[0].Children[0].Children[0].TagName);
            Assert.AreEqual("TEXT1TEXT2", tag.Children[0].Children[0].Children[0].InnerText);
            Assert.AreEqual(2, tag.Children[0].Children[0].Children[0].Children.Count);
            Assert.AreEqual("span", tag.Children[0].Children[0].Children[0].Children[0].TagName);
            Assert.AreEqual("INNER SPAN1", tag.Children[0].Children[0].Children[0].Children[0].InnerText);
            Assert.AreEqual("INNER SPAN1", tag.Children[0].Children[0].Children[0].Children[0].InnerHtml);
            Assert.AreEqual(0, tag.Children[0].Children[0].Children[0].Children[0].Children.Count);
            Assert.AreEqual("span", tag.Children[0].Children[0].Children[0].Children[1].TagName);
            Assert.AreEqual("INNER SPAN2", tag.Children[0].Children[0].Children[0].Children[1].InnerText);
            Assert.AreEqual("INNER SPAN2", tag.Children[0].Children[0].Children[0].Children[1].InnerHtml);
            Assert.AreEqual(0, tag.Children[0].Children[0].Children[0].Children[1].Children.Count);
        }

        [TestMethod]
        public void SimpleHtmlWithClosedParseTest()
        {
            const string html = "<html><head>HEAD</head><body><input/><input/><input/><span></span></body></html>";

            HtmlDocument tag = HtmlParser.Parse(html);

            Assert.IsNotNull(tag);
            Assert.AreEqual("html", tag.Children[0].TagName);
            Assert.AreEqual(2, tag.Children[0].Children.Count);
            Assert.AreEqual("head", tag.Children[0].Children[0].TagName);
            Assert.AreEqual(0, tag.Children[0].Children[0].Children.Count);
            Assert.AreEqual("HEAD", tag.Children[0].Children[0].InnerText);
            Assert.AreEqual("HEAD", tag.Children[0].Children[0].InnerHtml);
            Assert.AreEqual("body", tag.Children[0].Children[1].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].InnerText);
            Assert.AreEqual(4, tag.Children[0].Children[1].Children.Count);
            Assert.AreEqual("input", tag.Children[0].Children[1].Children[0].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[0].InnerText);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[0].InnerHtml);
            Assert.AreEqual("input", tag.Children[0].Children[1].Children[1].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[1].InnerText);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[1].InnerHtml);
            Assert.AreEqual("input", tag.Children[0].Children[1].Children[2].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[2].InnerText);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[2].InnerHtml);
            Assert.AreEqual("span", tag.Children[0].Children[1].Children[3].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[3].InnerText);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[3].InnerHtml);
        }

        [TestMethod]
        public void BadFormattedHtmlWithClosedParseTest()
        {
            const string html = "< Html><head  >HEAD</ head><body><inPut /  >< inpuT / ><input  /><span>< /span></body>< /hTml>";

            HtmlDocument tag = HtmlParser.Parse(html);

            Assert.IsNotNull(tag);
            Assert.AreEqual("html", tag.Children[0].TagName);
            Assert.AreEqual(2, tag.Children[0].Children.Count);
            Assert.AreEqual("head", tag.Children[0].Children[0].TagName);
            Assert.AreEqual(0, tag.Children[0].Children[0].Children.Count);
            Assert.AreEqual("HEAD", tag.Children[0].Children[0].InnerText);
            Assert.AreEqual("HEAD", tag.Children[0].Children[0].InnerHtml);
            Assert.AreEqual("body", tag.Children[0].Children[1].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].InnerText);
            Assert.AreEqual(4, tag.Children[0].Children[1].Children.Count);
            Assert.AreEqual("input", tag.Children[0].Children[1].Children[0].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[0].InnerText);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[0].InnerHtml);
            Assert.AreEqual("input", tag.Children[0].Children[1].Children[1].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[1].InnerText);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[1].InnerHtml);
            Assert.AreEqual("input", tag.Children[0].Children[1].Children[2].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[2].InnerText);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[2].InnerHtml);
            Assert.AreEqual("span", tag.Children[0].Children[1].Children[3].TagName);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[3].InnerText);
            Assert.AreEqual("", tag.Children[0].Children[1].Children[3].InnerHtml);
        }

        [TestMethod]
        public void RealHtmlParseAndTagSearchingTest()
        {
            const string url = @"http://www.votonia.ru/igrushka-dlya-vanni-uf-ribka-plavayushaya-z-fish-podsvetka-led/spb/86952/";

            string html = HtmlUtility.GetHtml(url);

            Assert.IsNotNull(html);

            HtmlDocument doc = HtmlParser.Parse(html);

            Assert.IsNotNull(doc);

            List<HtmlTag> tags = doc.Find("div.item_area");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("item_area", tags[0].GetClassName());

            tags = doc.Find("div.item_area div.attribute[0] div");
            Assert.IsNotNull(tags);
            Assert.AreEqual(3, tags.Count);

            tags = doc.Find("div.item_area div.attribute[0] div[0]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);

            tags = doc.Find("div.item_area div.attribute[0] div[0] a");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("UF", tags[0].InnerText.Trim());

            tags = doc.Find("div.item_area div.attribute[0] div[1]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("Страна изготовления: КИТАЙ", tags[0].InnerText.Trim());

            tags = doc.Find("#productTitle");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("Игрушка для ванны UF, Рыбка плавающая Z-fish, подсветка (led)", tags[0].InnerText);

            tags = doc.Find("h1#productTitle");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("Игрушка для ванны UF, Рыбка плавающая Z-fish, подсветка (led)", tags[0].InnerText);

            tags = doc.Find(".view_cost");
            Assert.IsNotNull(tags);
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);
            Assert.AreEqual(" р.", tags[1].InnerText);

            tags = doc.Find("#currentCost");
            Assert.IsNotNull(tags);
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);
            Assert.AreEqual(" р.", tags[1].InnerText);

            tags = doc.Find("#currentCost.view_cost");
            Assert.IsNotNull(tags);
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);
            Assert.AreEqual(" р.", tags[1].InnerText);

            tags = doc.Find("span#currentCost.view_cost");
            Assert.IsNotNull(tags);
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);
            Assert.AreEqual(" р.", tags[1].InnerText);

            tags = doc.Find("span#currentCost");
            Assert.IsNotNull(tags);
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);
            Assert.AreEqual(" р.", tags[1].InnerText);

            tags = doc.Find("span.view_cost");
            Assert.IsNotNull(tags);
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);
            Assert.AreEqual(" р.", tags[1].InnerText);

            tags = doc.Find("body");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);

            tags = doc.Find("body span#currentCost.view_cost");
            Assert.IsNotNull(tags);
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);
            Assert.AreEqual(" р.", tags[1].InnerText);

            tags = doc.Find(".view_cost[0]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);

            tags = doc.Find("#currentCost[0]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);

            tags = doc.Find("#currentCost.view_cost[0]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);

            tags = doc.Find("span#currentCost.view_cost[0]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);

            tags = doc.Find("span#currentCost[0]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);

            tags = doc.Find("span.view_cost[0]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);

            tags = doc.Find("body[0]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);

            tags = doc.Find("body span#currentCost.view_cost[0]");
            Assert.IsNotNull(tags);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual("334.00", tags[0].InnerText);
        }
    }
}