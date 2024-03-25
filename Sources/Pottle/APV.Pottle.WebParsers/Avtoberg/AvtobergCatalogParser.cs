using System.Collections.Generic;
using System.Linq;
using APV.Common;
using APV.Common.Html;
using APV.Pottle.WebParsers.InfoEntities;

namespace APV.Pottle.WebParsers.Avtoberg
{
    public class AvtobergCatalogParser : BaseCatalogParser<CatalogPageInfo>
    {
        private const string ProductLinkPattern = @"table.catalog td.title a";

        protected override CatalogPageInfo[] Parse(AbsoluteUri url, HtmlDocument doc)
        {
            List<HtmlTag> tags = doc.Find(ProductLinkPattern);

            AbsoluteUri[] links = tags
                .Select(tag => new AbsoluteUri(url, tag.GetAttributeValue("href")))
                .ToArray();

            var catalogInfo = new CatalogPageInfo(url)
            {
                Links = links,
            };

            return new[] { catalogInfo };
        }
    }
}