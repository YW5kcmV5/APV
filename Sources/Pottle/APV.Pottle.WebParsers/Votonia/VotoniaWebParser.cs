using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using APV.Common;
using APV.Pottle.WebParsers.InfoEntities;
using APV.Pottle.WebParsers.ResultEntities;

namespace APV.Pottle.WebParsers.Votonia
{
    public class VotoniaWebParser
    {
        private AbsoluteUri[] ListCatalogPages(AbsoluteUri catalogContainerUri)
        {
            var parser = new VotoniaCatalogContainerParser();
            ParseResult<CatalogContainerInfo> result = parser.Parse(catalogContainerUri);

            if (!result.Success)
                throw new InvalidOperationException(string.Format("Catalog container page \"{0}\" can not be parsed. See inner exception for details.", catalogContainerUri.Url), result.Exception);

            AbsoluteUri[] catalogPageLink = result.Data.SelectMany(x => x.Links).Distinct().ToArray();
            return catalogPageLink;
        }

        private AbsoluteUri[] ListProductPages(AbsoluteUri[] catalogPages)
        {
            var links = new List<AbsoluteUri>();
            var parser = new VotoniaCatalogParser();
            foreach (AbsoluteUri catalogPage in catalogPages)
            {
                AbsoluteUri[] products = parser.ListProductReferences(catalogPage);
                links.AddRange(products);
            }
            return links.Distinct().ToArray();
        }

        private VotoniaSupplierProductInfo[] ListProducts(AbsoluteUri[] productPages)
        {
            var products = new List<VotoniaSupplierProductInfo>();
            var errors = new List<ParseResult<VotoniaSupplierProductInfo>>();
            var notFound = new List<ParseResult<VotoniaSupplierProductInfo>>();
            var parser = new VotoniaProductParser();
            foreach (AbsoluteUri productPage in productPages)
            {
                ParseResult<VotoniaSupplierProductInfo> result = parser.Parse(productPage);

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

        private void SaveProducts(AbsoluteUri[] productPages)
        {
            const string productsFolder = @"F:\MyProjects\Pottle\Data\Product database\Supplier\Интернет-магазин Вотоня\Products";
            const string errorsFolder = @"F:\MyProjects\Pottle\Data\Product database\Supplier\Интернет-магазин Вотоня\Products\Errors";

            //Directory.Delete(errorsFolder);
            //Directory.CreateDirectory(errorsFolder);

            var notFound = new List<ParseResult<VotoniaSupplierProductInfo>>();
            var parser = new VotoniaProductParser();
            int errors = 0;
            int index = 0;

            foreach (AbsoluteUri productPage in productPages)
            {
                index++;
                double process = (100.0 * index / productPages.Length);
                Console.WriteLine("{0:00.00} ({1}/{2}/{3}) \"{4}\"", process, index, errors, productPages.Length, productPage.Url);

                ParseResult<VotoniaSupplierProductInfo> result = parser.Parse(productPage);

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

                    foreach (VotoniaSupplierProductInfo product in result.Data)
                    {
                        try
                        {
                            string companyName = FormatForFileName(product.ProducerCompanyName);
                            string article = FormatForFileName(product.Article);
                            string filename = Path.Combine(productsFolder, string.Format("{0}_{1}.xml", companyName, article));
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

                Console.WriteLine();
            }
        }

        public void Export()
        {
            const string productLinksFilename = @"F:\MyProjects\Pottle\Data\Product database\Supplier\Интернет-магазин Вотоня\Product Links.txt";
            //const string url = @"http://www.votonia.ru/catalog/";
            //AbsoluteUri[] catalogPages = ListCatalogPages(new AbsoluteUri(url));
            //AbsoluteUri[] productPages = ListProductPages(catalogPages);

            //string[] productPagesReferences = productPages.Select(x => x.Url).Distinct().ToArray();
            //string text = string.Join("\r\n", productPagesReferences);
            //File.WriteAllText(productLinksFilename, text, Encoding.UTF8);
            string[] productPagesReferences = File.ReadAllLines(productLinksFilename);

            int index = 0;
            for (int i = 0; i < productPagesReferences.Length; i++)
            {
                if (productPagesReferences[i] == "http://www.votonia.ru/poroshok-stiralniiy-ushastiiy-nyan-nk-45-kg/spb/1470")
                {
                    index = i;
                    break;
                }
            }
            productPagesReferences = productPagesReferences.Skip(index).ToArray();

            AbsoluteUri[] productPages = productPagesReferences.Select(url => new AbsoluteUri(url)).ToArray();

            SaveProducts(productPages);
            //ListProducts(productPages);
        }
    }
}