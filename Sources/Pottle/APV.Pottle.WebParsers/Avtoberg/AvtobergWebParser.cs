using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using APV.Common;
using APV.Common.Periods;
using APV.Pottle.WebParsers.InfoEntities;
using APV.Pottle.WebParsers.ResultEntities;

namespace APV.Pottle.WebParsers.Avtoberg
{
    public class AvtobergWebParser
    {
        private AbsoluteUri[] ListCatalogPages(AbsoluteUri catalogContainerUri, out AvtobergTrademarkInfo[] trademarks)
        {
            var parser = new AvtobergCatalogContainerParser();
            ParseResult<CatalogContainerInfo> result = parser.Parse(catalogContainerUri);

            if (!result.Success)
                throw new InvalidOperationException(string.Format("Catalog container page \"{0}\" can not be parsed. See inner exception for details.", catalogContainerUri.Url), result.Exception);

            AbsoluteUri[] catalogPageLink = result.Data.SelectMany(x => x.Links).Distinct().ToArray();
            trademarks = result.Data.SelectMany(x => x.Trademarks).ToArray();
            return catalogPageLink;
        }

        private AbsoluteUri[] ListProductPages(AbsoluteUri[] catalogPages)
        {
            var links = new List<AbsoluteUri>();
            var parser = new AvtobergCatalogParser();
            foreach (AbsoluteUri catalogPage in catalogPages)
            {
                AbsoluteUri[] products = parser.ListProductReferences(catalogPage);
                links.AddRange(products);
            }
            return links.Distinct().ToArray();
        }

        private AvtobergSupplierProductInfo[] ListProducts(AbsoluteUri[] productPages)
        {
            var products = new List<AvtobergSupplierProductInfo>();
            var errors = new List<ParseResult<AvtobergSupplierProductInfo>>();
            var notFound = new List<ParseResult<AvtobergSupplierProductInfo>>();
            var parser = new AvtobergProductParser();
            foreach (AbsoluteUri productPage in productPages)
            {
                ParseResult<AvtobergSupplierProductInfo> result = parser.Parse(productPage);
                if (!result.Success)
                {
                    if (result.NotFound)
                    {
                        notFound.Add(result);
                    }
                    else
                    {
                        errors.Add(result);
                    }
                }
                else
                {
                    products.AddRange(result.Data);
                }
            }
            return products.Distinct().ToArray();
        }

        private string FormatForFileName(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                const string chars = @"0123456789AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZzАаБбВвГгДдЕеЁёЖжЗзИиЙйКкЛлМмНнОоПпРрСсТтУуФфХхЦцЧчШшЩщЪъЫыЬьЭэЮюЯя- ";
                var charsHashset = new HashSet<char>(chars.ToCharArray());
                value = new string(value.Where(charsHashset.Contains).ToArray());
            }
            return value;
        }

        private static string[] ListProductPagesWithErrors(IEnumerable<string> productReferences, string folderWithLoadedProducts)
        {
            List<string> all = productReferences.Select(item => item.ToLowerInvariant()).ToList();
            string[] files = Directory.GetFiles(folderWithLoadedProducts);
            foreach (string file in files)
            {
                var product = Serializer.DeserializeFromFile<AvtobergSupplierProductInfo>(file, Serializer.Type.DataContractSerializer);
                string uri = product.Url.ToString().ToLowerInvariant();
                all.Remove(uri);
            }
            return all.ToArray();
        }

        private static void ValidateName(string name, HashSet<char> possibleSymbols, string file)
        {
            if (name != null)
            {
                if (!name.All(possibleSymbols.Contains))
                {
                    int index = -1;
                    for (int i = 0; i < name.Length; i++)
                    {
                        if (!possibleSymbols.Contains(name[i]))
                        {
                            index = i;
                            break;
                        }
                    }
                    throw new InvalidOperationException(string.Format("Unknown symbol \"{0}\" {1}:{2}:#{3} (\"{4}\").", name, index, name[index], (int)name[index], file));
                }
            }
        }

        private static void Validate(string folderWithLoadedProducts, AvtobergTrademarkInfo[] allTrademarks)
        {
            var trademarks = new List<string>();
            var models = new List<string>();
            var groups = new List<string>();

            var possibleOriginalArticleSymbols = new HashSet<char>(@"0123456789AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz #.-+/\()");
            var possibleArticleSymbols = new HashSet<char>(@"0123456789AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz-#");
            var possibleTrademarkSymbols = new HashSet<char>(@"0123456789AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz /");

            string[] files = Directory.GetFiles(folderWithLoadedProducts);
            foreach (string file in files)
            {
                var product = Serializer.DeserializeFromFile<AvtobergSupplierProductInfo>(file, Serializer.Type.DataContractSerializer);
                string trademark = product.TrademarkName;
                string model = product.ModelName;
                string group = product.Navigator[0];
                string article = product.Article;
                string originalArticle = product.OriginalArticle;
                string deliveryPeriod = product.DeliveryPeriod;
                bool outOfStock = product.OutOfStock;
                AnnualPeriodCollection modelPeriod = product.ModelPeriod;

                if (!outOfStock)
                {
                    if ((deliveryPeriod == null) || ((!deliveryPeriod.EndsWith("день")) && (!deliveryPeriod.EndsWith("дней"))))
                    {
                        throw new InvalidOperationException(string.Format("Unknown delivery period (\"{0}\").", deliveryPeriod));
                    }
                }

                //Article
                ValidateName(article, possibleArticleSymbols, file);

                //Original Article
                if (originalArticle != null)
                {
                    if (originalArticle != "ТЮНИНГ")
                    {
                        originalArticle = originalArticle.Replace("С", "C");
                        originalArticle = originalArticle.Replace("Е", "E");
                        originalArticle = originalArticle.Replace("А", "A");
                        ValidateName(originalArticle, possibleOriginalArticleSymbols, file);
                    }
                }

                //Trademark
                ValidateName(trademark, possibleTrademarkSymbols, file);
                if (string.IsNullOrWhiteSpace(trademark))
                {
                    throw new InvalidOperationException(string.Format("Unknown Trademark (\"{0}\").", file));
                }
                if (!trademarks.Contains(trademark))
                {
                    trademarks.Add(trademark);
                }

                //Model
                if (string.IsNullOrWhiteSpace(model))
                {
                    throw new InvalidOperationException(string.Format("Unknown Model (\"{0}\").", file));
                }
                if (!models.Contains(model))
                {
                    models.Add(model);
                }

                //Group
                if (string.IsNullOrWhiteSpace(group))
                {
                    throw new InvalidOperationException(string.Format("Unknown Group (\"{0}\").", file));
                }
                if (!groups.Contains(group))
                {
                    groups.Add(group);
                }

                string modelKey = (modelPeriod != null) ? string.Format("{0}:{1}", model, modelPeriod) : model;

                AvtobergTrademarkInfo[] trademarksInfo = allTrademarks.Where(item => item.Name == trademark).ToArray();
                if (trademarksInfo.Length != 1)
                {
                    throw new InvalidOperationException(string.Format("Unknown trademark name (\"{0}\").", trademark));
                }
                AvtobergTrademarkInfo trademarkInfo = trademarksInfo[0];

                AvtobergModelInfo[] modelsInfo = trademarkInfo.Models.Where(item => item.Key == modelKey).ToArray();
                if (modelsInfo.Length != 1)
                {
                    throw new InvalidOperationException(string.Format("Unknown model name, model period is not defined (\"{0}\", \"{1}\").", trademark, model));
                }
            }
        }

        private void SaveTrademarks(string trademarksFolder, IEnumerable<AvtobergTrademarkInfo> trademarks)
        {
            foreach (AvtobergTrademarkInfo trademark in trademarks)
            {
                string trademarkName = FormatForFileName(trademark.Name);
                string filename = Path.Combine(trademarksFolder, string.Format("{0}_{1}.xml", trademarkName, Guid.NewGuid()));
                if (filename.Length > 259)
                {
                    int dLength = filename.Length - 259;
                    filename = Path.GetFileNameWithoutExtension(filename);
                    filename = filename.Substring(0, filename.Length - dLength);
                    filename = Path.Combine(trademarksFolder, string.Format("{0}.xml", filename));
                }
                Serializer.SerializeToFile(trademark, filename, Serializer.Type.DataContractSerializer);
            }
        }

        private void SaveProducts(AbsoluteUri[] productPages, string productsFolder, string errorsFolder)
        {
            var notFound = new List<ParseResult<AvtobergSupplierProductInfo>>();
            var parser = new AvtobergProductParser();
            int errors = 0;
            int index = 0;

            foreach (AbsoluteUri productPage in productPages)
            {
                index++;
                double process = (100.0 * index / productPages.Length);
                Console.WriteLine("{0:00.00} ({1}/{2}/{3}) \"{4}\"", process, index, errors, productPages.Length, productPage.Url);

                try
                {
                    ParseResult<AvtobergSupplierProductInfo> result = parser.Parse(productPage);

                    if ((!result.Success) && (result.Error.Contains("The operation has timed out")))
                    {
                        Thread.Sleep(60000);
                        result = parser.Parse(productPage);
                    }

                    if (!result.Success)
                    {
                        if (result.NotFound)
                        {
                            Console.WriteLine("Not found");

                            notFound.Add(result);
                        }
                        else
                        {
                            Console.WriteLine("Error - {0}", result.Error);

                            errors++;
                            string filename = Path.Combine(errorsFolder, string.Format("{0}.xml", errors));
                            Serializer.SerializeToFile(result, filename, Serializer.Type.DataContractSerializer);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Success - {0} products", result.Data.Length);

                        foreach (AvtobergSupplierProductInfo product in result.Data)
                        {
                            try
                            {
                                string trademarkName = FormatForFileName(product.TrademarkName);
                                string modelName = FormatForFileName(product.ModelName);
                                string article = FormatForFileName(product.OriginalArticle ?? product.Article ?? product.ProductName);
                                string filename = Path.Combine(productsFolder, string.Format("{0}_{1}_{2}_{3}.xml", trademarkName, modelName, article, Guid.NewGuid()));
                                if (filename.Length > 259)
                                {
                                    int dLength = filename.Length - 259;
                                    filename = Path.GetFileNameWithoutExtension(filename);
                                    filename = filename.Substring(0, filename.Length - dLength);
                                    filename = Path.Combine(productsFolder, string.Format("{0}.xml", filename));
                                }
                                Serializer.SerializeToFile(product, filename, Serializer.Type.DataContractSerializer);
                            }
                            catch (Exception ex)
                            {
                                errors++;
                                string info = string.Format("{0}\r\n{1}\r\n", productPage.Url, ex.ToTraceString());
                                string filename = Path.Combine(errorsFolder, string.Format("{0}_exception.xml", errors));
                                File.WriteAllText(filename, info, Encoding.UTF8);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors++;
                    string info = string.Format("{0}\r\n{1}\r\n", productPage.Url, ex.ToTraceString());
                    string filename = Path.Combine(errorsFolder, string.Format("{0}_exception.xml", errors));
                    File.WriteAllText(filename, info, Encoding.UTF8);
                }

                Console.WriteLine();
            }
        }

        public void Export()
        {
            const string url = @"http://avtoberg.ru/catalog/";
            const string productsFolder = @"F:\MyProjects\Pottle\Data\Product database\Supplier\Интернет-магазин Автоберг\Products";
            const string trademarksFolder = @"F:\MyProjects\Pottle\Data\Product database\Supplier\Интернет-магазин Автоберг\Trademarks";
            const string errorsFolder = @"F:\MyProjects\Pottle\Data\Product database\Supplier\Интернет-магазин Автоберг\Products\Errors";
            const string productLinksFilename = @"F:\MyProjects\Pottle\Data\Product database\Supplier\Интернет-магазин Автоберг\Product Links.txt";

            if (!Directory.Exists(productsFolder))
            {
                Directory.CreateDirectory(productsFolder);
            }
            if (!Directory.Exists(errorsFolder))
            {
                Directory.CreateDirectory(errorsFolder);
            }

            AvtobergTrademarkInfo[] trademarks;
            AbsoluteUri[] catalogPages = ListCatalogPages(new AbsoluteUri(url), out trademarks);

            Validate(productsFolder, trademarks);
            SaveTrademarks(trademarksFolder, trademarks);

            AbsoluteUri[] productPages = ListProductPages(catalogPages);
            string[] productPagesReferences = productPages.Select(x => x.Url).Distinct().ToArray();
            string text = string.Join("\r\n", productPagesReferences);
            File.WriteAllText(productLinksFilename, text, Encoding.UTF8);
            
            productPagesReferences = File.ReadAllLines(productLinksFilename);
            productPagesReferences = ListProductPagesWithErrors(productPagesReferences, productsFolder);

            productPages = productPagesReferences.Select(reference => new AbsoluteUri(reference)).ToArray();

            SaveProducts(productPages, productsFolder, errorsFolder);
        }
    }
}