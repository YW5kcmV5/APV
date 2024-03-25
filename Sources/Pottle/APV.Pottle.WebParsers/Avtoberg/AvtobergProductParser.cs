using System;
using System.Collections.Generic;
using System.Linq;
using APV.Common;
using APV.Common.Html;
using APV.Common.Periods;
using APV.GraphicsLibrary.Extensions;
using APV.GraphicsLibrary.Images;
using APV.Pottle.WebParsers.InfoEntities;
using APV.Pottle.WebParsers.InfoEntities.Collection;

namespace APV.Pottle.WebParsers.Avtoberg
{
    public class AvtobergProductParser : BaseParser<AvtobergSupplierProductInfo>
    {
        public const string ProductNamePattern = @"div.b-card h1";
        public const string ProductCostPattern = @"div.b-info div.store1";
        public const string ImagesPattern = @"div.b-info a.photo";
        public const string GroupNamePattern = @"#f_name option";

        public const string TrademarkNamePattern = @"div.b-details dl[0] dd[0]";
        public const string ModelNamePattern = @"div.b-details dl[0] dd[1]";
        public const string ModelPeriodPattern = "div.b-details dl[0] dd[2]";
        public const string ProducerPeriodPattern = "div.b-details dl[0] dd[3]";
        public const string ProducerPeriodFirstPattern = "div.b-details dl[0] dd[2]";
        public const string OutOfStockPattern = "div.b-details dl[0] dd[4]";
        public const string OutOfStockFirstPattern = "div.b-details dl[0] dd[3]";
        public const string DeliveryPeriodPattern = "div.b-details dl[0] dd[5]";
        public const string DeliveryPeriodFirstPattern = "div.b-details dl[0] dd[4]";
        public const string OriginalArticlePattern = @"div.b-details dl[1] dd[0]";
        public const string ProducerCompanyNamePattern = @"div.b-details dl[1] dd[1] a";
        public const string ProducerCompanyNameFirstPattern = @"div.b-details dl[1] dd[0] a";
        public const string ArticlePattern = @"div.b-details dl[1] dd[2]";
        public const string ArticleFirstPattern = @"div.b-details dl[1] dd[1]";
        public const string BuyTogetherProductReferencesPattern = @"div.b-buy-together ul li";
        //b-buy-together

        private string[] ExtractImageUrlFromJs(string js)
        {
            //toggleMainPhoto(this, '/images/pictures/86952_553ee2b28abf1/igrushka-dlya-vanni-uf-ribka-plavayushaya-z-fish-podsvetka-led.jpg', '/images/pictures/86952_553ee2b28abf1_original/igrushka-dlya-vanni-uf-ribka-plavayushaya-z-fish-podsvetka-led.jpg'); return false;
            string[] items = js.Split(new[] { "\'", "\"" }, StringSplitOptions.RemoveEmptyEntries);
            if ((items.Length != 5) || (string.IsNullOrWhiteSpace(items[1]) || (string.IsNullOrWhiteSpace(items[3]))))
                throw new InvalidOperationException(string.Format("Images can not be parsed. The image urls can not be extracted from JS \"{0}\".", js));

            return new[] { items[1], items[3] };
        }

        private ImageInfo[] ParseImages(AbsoluteUri url, HtmlDocument doc)
        {
            List<HtmlTag> imagesTags = doc.Find(ImagesPattern);

            var result = new List<ImageInfo>();

            foreach (HtmlTag imageTag in imagesTags)
            {
                string href = imageTag.GetAttributeValue("href");
                if (!string.IsNullOrEmpty(href))
                {
                    var originalImageUri = new AbsoluteUri(url, href);
                    ImageContainer originalImage = originalImageUri.GetImage();
                    var originalImageInfo = new ImageInfo(originalImageUri)
                        {
                            Title = originalImageUri.Page,
                            Data = originalImage,
                        };
                    result.Add(originalImageInfo);
                }
            }

            return result.ToArray();
        }

        protected override AvtobergSupplierProductInfo[] Parse(AbsoluteUri url, HtmlDocument doc)
        {
            string productName = GetString(doc, ProductNamePattern);
            double productCost = GetDouble(doc, ProductCostPattern);
            ImageInfo[] images = ParseImages(url, doc);

            //Group name
            List<HtmlTag> tags = doc.Find(GroupNamePattern);
            string groupName = tags.Single(item => item.GetAttributeValue("selected") == "selected").InnerText;

            //Марка:
            string trademarkName = GetString(doc, TrademarkNamePattern);
            //Модель:
            string modelName = GetString(doc, ModelNamePattern);
            //Модель выпуска:
            tags = doc.Find(ModelPeriodPattern);
            if (tags.Count != 1)
                throw new InvalidOperationException(string.Format("Model period cannot be parsed for pattern \"{0}\" for product url \"{1}\".", ModelPeriodPattern, url));

            bool modelPeriodDefined = (tags[0].PreviousSibling != null) && (tags[0].PreviousSibling.Text == "Модель выпуска:");
            var modelPeriod = (modelPeriodDefined)
                                  ? new AnnualPeriodCollection(GetString(doc, ModelPeriodPattern))
                                  : null;
            //Год выпуска:
            string producerPeriodValue = (modelPeriodDefined)
                                     ? GetString(doc, ProducerPeriodPattern)
                                     : GetString(doc, ProducerPeriodFirstPattern);
            var producerPeriod = ((producerPeriodValue != "-") && (producerPeriodValue != "–"))
                                     ? new AnnualPeriodInfo(producerPeriodValue)
                                     : null;
            //В наличии:
            string outOfStockValue = (modelPeriodDefined)
                                         ? GetString(doc, OutOfStockPattern)
                                         : GetString(doc, OutOfStockFirstPattern);
            bool outOfStock = (string.Compare(outOfStockValue, "нет", StringComparison.InvariantCultureIgnoreCase) == 0);
            //Срок доставки:
            string deliveryPeriod = (modelPeriodDefined)
                                        ? GetString(doc, DeliveryPeriodPattern, !outOfStock)
                                        : GetString(doc, DeliveryPeriodFirstPattern, !outOfStock);
            
            //Оригинальный номер:
            string originalArticle = GetString(doc, OriginalArticlePattern, false);
            bool originalArticleDefined = (!string.IsNullOrWhiteSpace(originalArticle));

            //Производитель:
            string producerCompanyName = (originalArticleDefined)
                ? GetString(doc, ProducerCompanyNamePattern, false)
                : GetString(doc, ProducerCompanyNameFirstPattern, false);

            //Номер по каталогу:
            string article = (originalArticleDefined)
                                 ? GetString(doc, ArticlePattern, false)
                                 : GetString(doc, ArticleFirstPattern, false);

            if (string.IsNullOrWhiteSpace(originalArticle))
            {
                originalArticle = null;
            }
            if (string.IsNullOrWhiteSpace(article))
            {
                article = null;
            }

            //By tohether
            tags = doc.Find(BuyTogetherProductReferencesPattern);
            AbsoluteUri[] references = tags.Select(x => new AbsoluteUri(url, x.Children[0].GetAttributeValue("href"))).ToArray();

            var productInfo = new AvtobergSupplierProductInfo(url, doc.HtmlHashCode)
                {
                    TrademarkName = trademarkName,
                    ModelName = modelName,
                    ModelPeriod = modelPeriod,
                    ProducedPeriod = producerPeriod,
                    OutOfStock = outOfStock,
                    DeliveryPeriod = deliveryPeriod,
                    OriginalArticle = originalArticle,
                    ProducerCompanyName = producerCompanyName,
                    Article = article,

                    ProductName = productName,
                    ProductCost = productCost,
                    Images = images,
                    BuyTogetherProductReferences = references,

                    ProducerCountryName = null,
                    Navigator = new[] { groupName },
                    Options = new ProductOptionCollection(),
                };

            return new[] { productInfo };
        }
    }
}