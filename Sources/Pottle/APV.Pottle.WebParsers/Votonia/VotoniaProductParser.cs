using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using APV.Common.Extensions;
using APV.Common.Html;
using APV.Common;
using APV.GraphicsLibrary.Extensions;
using APV.GraphicsLibrary.Images;
using APV.Pottle.Common;
using APV.Pottle.WebParsers.InfoEntities;
using APV.Pottle.WebParsers.InfoEntities.Collection;

namespace APV.Pottle.WebParsers.Votonia
{

    public class VotoniaProductParser : BaseParser<VotoniaSupplierProductInfo>
    {
        public const string ProductNamePattern = @"h1#productTitle";
        public const string ProductCostPattern = @"span#currentCost.view_cost[0]";
        public const string ArticlePattern = @"div#code_1c span";
        public const string ProducerCompanyNamePattern = @"div.item_area div.attribute[0] div[0] a";
        public const string ProducerCountryNamePattern = @"div.item_area div.attribute[0] div[1]";
        public const string ColorSelectorPattern = @"td#colors h5";
        public const string ColorPattern = @"td#colors a";
        public const string SizeSelectorPattern = @"td#sizes h5";
        public const string SizePattern = @"td#sizes input";
        public const string ImagesPattern = @"div.photo_mini_area a";
        public const string NavigatorPattern = @"div.navigator span a span";

        private class Selector
        {
            public string Color { get; set; }

            public string Size { get; set; }

            public string Article { get; set; }
        }

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
            imagesTags = imagesTags.Where(x => x.Find("img").Count == 1).ToList();

            var result = new List<ImageInfo>();

            foreach (HtmlTag imageTag in imagesTags)
            {
                string rel = imageTag.GetAttributeValue("rel");
                string js = imageTag.GetAttributeValue("onclick");
                if ((!string.IsNullOrEmpty(rel)) && (!string.IsNullOrEmpty(js)))
                {
                    string[] imagesUrls = ExtractImageUrlFromJs(js);
                    //HtmlTag imgTag = imageTag.Children[0];
                    //string smallImageSrc = imgTag.GetAttributeValue("src");

                    //var smallImageUri = new AbsoluteUri(url, smallImageSrc);
                    //var previewImageUri = new AbsoluteUri(url, imagesUrls[0]);
                    var originalImageUri = new AbsoluteUri(url, imagesUrls[1]);

                    ImageContainer originalImage = originalImageUri.GetImage();
                    var originalImageInfo = new ImageInfo(originalImageUri)
                    {
                        Title = rel,
                        Data = originalImage,
                    };

                    result.Add(originalImageInfo);
                }
            }

            return result.ToArray();
        }

        private string ExtractCountryName(string countryName)
        {
            int index = countryName.LastIndexOf(":");
            if (index != -1)
            {
                countryName = countryName.Substring(index + 1);
            }
            return countryName.Trim().ToPascalCase();
        }

        private string[] ParseNavigator(HtmlDocument doc)
        {
            List<HtmlTag> tags = doc.Find(NavigatorPattern);
            if (tags.Count <= 1)
            {
                return new string[0];
            }
            string[] navigator = tags
                .Skip(1)
                .Where(item => !string.IsNullOrWhiteSpace(item.Text))
                .Select(item => item.Text)
                .ToArray();
            return navigator;
        }

        private Selector ParseColorSelector(HtmlTag colorTag)
        {
            List<HtmlTag> imgTags = colorTag.Find("img");
            if (imgTags.Count == 1)
            {
                string title = colorTag.GetAttributeValue("title");
                string rel = colorTag.GetAttributeValue("rel");
                if ((!string.IsNullOrWhiteSpace(title)) && (!string.IsNullOrWhiteSpace(rel)))
                {
                    string article = rel.ToLowerInvariant().Trim();
                    string color = title.ToLowerInvariant().Trim();

                    var selector = new Selector
                    {
                        Article = article,
                        Color = color,
                    };

                    return selector;
                }
            }
            throw new InvalidOperationException(string.Format("Color selector can not be parsed for patter \"{0}\".", ColorSelectorPattern));
        }

        private Selector[] ParseColorSelector(IEnumerable<HtmlTag> colorTags)
        {
            return colorTags.Select(ParseColorSelector).ToArray();
        }

        private string ExteractInputWithProperties(HtmlDocument doc, string article)
        {
            //par_1-00079811
            string pattern = string.Format("input#par_{0}", article);
            List<HtmlTag> tags = doc.Find(pattern);
            if (tags.Count == 1)
            {
                string value = tags[0].GetAttributeValue("value");
                value = (value ?? string.Empty).Trim();
                return (!string.IsNullOrEmpty(value)) ? value : null;
            }
            return null;
        }

        private double ExtractCostFromProperties(HtmlDocument doc, string article, double defaultCost)
        {
            string properties = ExteractInputWithProperties(doc, article);
            if (properties != null)
            {
                string[] items = properties.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if ((items.Length > 0) && (!string.IsNullOrWhiteSpace(items[0])))
                {
                    //17 595.00|17595.00|85903|527.9|балла|88|Crown Blue|outStock|1|26.05.2015|0||1|0|1|1|17 595.00|17 595.00|0|_off|1|15 568.00|1|26.05.2015|0|0|test|test|0|||0|0|0
                    string stringValue = items[0].Replace(",", ".").Replace(" ", string.Empty);
                    double value;
                    if ((double.TryParse(stringValue, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value)))
                    {
                        if (value > 0)
                        {
                            return value;
                        }
                    }
                }
            }
            return defaultCost;
        }

        private bool ExtractOutOfStockFromProperties(HtmlDocument doc, string article, bool defaultOutOfStock)
        {
            string properties = ExteractInputWithProperties(doc, article);
            if (properties != null)
            {
                //17 595.00|17595.00|85903|527.9|балла|88|Crown Blue|outStock|1|26.05.2015|0||1|0|1|1|17 595.00|17 595.00|0|_off|1|15 568.00|1|26.05.2015|0|0|test|test|0|||0|0|0
                string[] items = properties.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < items.Length - 1; i++)
                {
                    string item = items[i];
                    if (item == "outStock")
                    {
                        item = items[i + 1];
                        if (item == "0")
                        {
                            return true;
                        }
                        if (item == "1")
                        {
                            return false;
                        }
                        break;
                    }
                }
            }
            return defaultOutOfStock;
        }

        private Selector ParseSizeSelector(HtmlTag colorTag)
        {
            string value = colorTag.GetAttributeValue("value");
            string rel = colorTag.GetAttributeValue("rel");

            if ((!string.IsNullOrWhiteSpace(value)) && (!string.IsNullOrWhiteSpace(rel)))
            {
                string article = rel.ToLowerInvariant().Trim();
                string size = value.Trim();

                var selector = new Selector
                {
                    Article = article,
                    Size = size,
                };

                return selector;
            }

            throw new InvalidOperationException(string.Format("Size selector can not be parsed for patter \"{0}\".", SizeSelectorPattern));
        }

        private Selector[] ParseSizeSelector(IEnumerable<HtmlTag> colorTags)
        {
            return colorTags.Select(ParseSizeSelector).ToArray();
        }

        protected override VotoniaSupplierProductInfo[] Parse(AbsoluteUri url, HtmlDocument doc)
        {
            string productName = GetString(doc, ProductNamePattern);
            double productCost = GetDouble(doc, ProductCostPattern);
            string article = GetString(doc, ArticlePattern);
            string producerCountryName = ExtractCountryName(GetString(doc, ProducerCountryNamePattern));
            string[] navigator = ParseNavigator(doc);

            string producerCompanyName = GetString(doc, ProducerCompanyNamePattern, false);

            bool outOfStock = false;

            ImageInfo[] images = ParseImages(url, doc);

            bool hasColorSelector = Exists(doc, ColorSelectorPattern);
            bool hasSizeSelector = Exists(doc, SizeSelectorPattern);

            ProductOptionCollection options = VotoniaProductOptionParser.Parse(doc);

            var productInfo = new VotoniaSupplierProductInfo(url, doc.HtmlHashCode)
            {
                ProductCost = productCost,
                ProductName = productName,
                Article = article,
                ProducerCountryName = producerCountryName,
                ProducerCompanyName = producerCompanyName,
                Images = images,
                Navigator = navigator,
                Options = options,
                OutOfStock = outOfStock,
            };

            if ((!hasColorSelector) && (!hasSizeSelector))
            {
                return new[] { productInfo };
            }

            List<HtmlTag> colorTags = doc.Find(ColorPattern);
            List<HtmlTag> sizeTags = doc.Find(SizePattern);

            bool colorModifier = false;
            bool sizeModifier = false;

            if ((hasColorSelector) && (colorTags.Count == 1))
            {
                hasColorSelector = false;
                if (!options.Characteristics.HasColor)
                {
                    colorModifier = true;
                }
            }
            if ((hasSizeSelector) && (sizeTags.Count == 1))
            {
                hasSizeSelector = false;
                if (!options.Characteristics.HasSize)
                {
                    sizeModifier = true;
                }
            }

            if (colorModifier)
            {
                options.AddModifier(ProductCharacteristicModifier.Color);
            }
            if (sizeModifier)
            {
                options.AddModifier(ProductCharacteristicModifier.Size);
            }

            var result = new List<VotoniaSupplierProductInfo>();

            Selector[] colorSelectors;
            Selector[] sizeSelectors;

            if ((hasColorSelector) && (hasSizeSelector))
            {
                //throw new NotSupportedException("Both selectors (color and size) are presented. The logic is not implemented.");

                colorSelectors = ParseColorSelector(colorTags);
                sizeSelectors = ParseSizeSelector(sizeTags);

                foreach (Selector colorSelector in colorSelectors)
                {
                    foreach (Selector sizeSelector in sizeSelectors)
                    {
                        string[] colorArticles = colorSelector.Article.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        string[] sizeArticles = sizeSelector.Article.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        string[] newArticles = colorArticles.Intersect(sizeArticles).ToArray();

                        if (newArticles.Length > 0)
                        {
                            if ((newArticles.Length != 1) || (string.IsNullOrWhiteSpace(newArticles[0])))
                                throw new InvalidOperationException(string.Format("Product article can not be extracted for color (\"{0}\") and size selectors (\"{1}\").", colorSelector.Article, sizeSelector.Article));

                            string newArticle = newArticles[0];

                            var newOptions = new ProductOptionCollection(options)
                            {
                                Characteristics =
                                {
                                    Color = colorSelector.Color,
                                    Size = sizeSelector.Size.ToString(CultureInfo.InvariantCulture),
                                }
                            };
                            string newProductName = string.Format("{0}, {1}, р.{2}", productName, colorSelector.Color, sizeSelector.Size);

                            var newImages = new List<ImageInfo>();
                            foreach (ImageInfo newImage in images)
                            {
                                string[] articles = newImage.Title.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                if (articles.Any(item => item == newArticle))
                                {
                                    newImages.Add(newImage);
                                }
                            }

                            double newProductCost = ExtractCostFromProperties(doc, newArticle, productInfo.ProductCost);
                            bool colorOutOfStock = ExtractOutOfStockFromProperties(doc, newArticle, outOfStock);

                            var colorIProductInfo = new VotoniaSupplierProductInfo(productInfo)
                            {
                                ProductName = newProductName,
                                Article = newArticle,
                                ProductCost = newProductCost,
                                OutOfStock = colorOutOfStock,
                                Options = newOptions,
                                Images = newImages.ToArray(),
                            };

                            result.Add(colorIProductInfo);
                        }
                    }
                }
            }
            else if (hasColorSelector)
            {
                //has color selector
                colorSelectors = ParseColorSelector(colorTags);

                foreach (Selector colorSelector in colorSelectors)
                {
                    string colorArticle = colorSelector.Article;
                    var colorOptions = new ProductOptionCollection(options) { Characteristics = { Color = colorSelector.Color } };
                    ImageInfo[] colorImages = images.Where(imageInfo => imageInfo.Title == colorArticle).ToArray();
                    string colorProductName = string.Format("{0}, {1}", productName, colorSelector.Color);
                    double colorProductCost = ExtractCostFromProperties(doc, colorArticle, productInfo.ProductCost);
                    bool colorOutOfStock = ExtractOutOfStockFromProperties(doc, colorArticle, outOfStock);

                    var colorProductInfo = new VotoniaSupplierProductInfo(productInfo)
                    {
                        ProductName = colorProductName,
                        Article = colorArticle,
                        ProductCost = colorProductCost,
                        OutOfStock = colorOutOfStock,
                        Images = colorImages,
                        Options = colorOptions,
                    };
                    result.Add(colorProductInfo);
                }
            }
            else
            {
                //hasSizeSelector
                sizeSelectors = ParseSizeSelector(sizeTags);
                foreach (Selector sizeSelector in sizeSelectors)
                {
                    string sizeArticle = sizeSelector.Article;
                    var sizeOptions = new ProductOptionCollection(options) { Characteristics = { Size = sizeSelector.Size.ToString(CultureInfo.InvariantCulture) } };
                    string sizeProductName = string.Format("{0}, р.{1}", productName, sizeSelector.Size);
                    double sizeProductCost = ExtractCostFromProperties(doc, sizeArticle, productInfo.ProductCost);

                    var colorIProductInfo = new VotoniaSupplierProductInfo(productInfo)
                    {
                        ProductName = sizeProductName,
                        Article = sizeArticle,
                        Options = sizeOptions,
                        ProductCost = sizeProductCost,
                    };

                    result.Add(colorIProductInfo);
                }
            }

            if (result.Count == 0)
                throw new InvalidOperationException(string.Format("Page contains the color modifier, but no color found or color data can not be parsed."));

            return result.ToArray();
        }
    }
}