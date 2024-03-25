using System;
using System.Collections.Generic;
using System.Linq;
using APV.Common;
using APV.Common.Html;
using APV.Pottle.WebParsers.InfoEntities;

namespace APV.Pottle.WebParsers.Votonia
{
    public class VotoniaCatalogContainerParser : BaseParser<CatalogContainerInfo>
    {
        public const string CatalogLinkPattern = @"div.catalog_razdel_item div.text h2 a";

        protected override CatalogContainerInfo[] Parse(AbsoluteUri url, HtmlDocument doc)
        {
            List<HtmlTag> productLinkTags = doc.Find(CatalogLinkPattern);

            AbsoluteUri[] links = productLinkTags
                .Select(tag => tag.GetAttributeValue("href"))
                .Where(href => !string.IsNullOrEmpty(href))
                .Select(href => new AbsoluteUri(url, href))
                .Distinct()
                .ToArray();

            if (links.Length == 0)
                throw new InvalidOperationException(string.Format("Link can not be parsed for pattern \"{0}\".", CatalogLinkPattern));

            var result = new CatalogContainerInfo(url)
                {
                    Links = links,
                };

            return new[] { result };
        }
    }
}