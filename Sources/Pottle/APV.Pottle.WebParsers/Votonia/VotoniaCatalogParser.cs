using System;
using System.Collections.Generic;
using System.Linq;
using APV.Common;
using APV.Common.Html;
using APV.Pottle.WebParsers.InfoEntities;

namespace APV.Pottle.WebParsers.Votonia
{
    public class VotoniaCatalogParser : BaseCatalogParser<CatalogPageInfo>
    {
        private const string ProductLinkPattern = @"div.fix_name_a a";
        private const string PageNumberPattern = @"li.activ_zakladka span span";
        private const string PreviousPagePattern = @"li.left_str_a a.prev";
        private const string NextPagePattern = @"li.right_str_a a.next";

        protected override CatalogPageInfo[] Parse(AbsoluteUri url, HtmlDocument doc)
        {
            List<HtmlTag> productLinkTags = doc.Find(ProductLinkPattern);

            int pageNumber = GetInteger(doc, PageNumberPattern);

            HtmlTag previousPageTag = doc.Find(PreviousPagePattern).SingleOrDefault();
            HtmlTag nextPageTag = doc.Find(NextPagePattern).SingleOrDefault();

            AbsoluteUri previousPageLink = (previousPageTag != null) ? new AbsoluteUri(url, previousPageTag.GetAttributeValue("href")) : null;
            AbsoluteUri nextPageLink = (nextPageTag != null) ? new AbsoluteUri(url, nextPageTag.GetAttributeValue("href")) : null;

            if ((previousPageLink == null) && (nextPageLink == null) && (pageNumber != -1))
                throw new InvalidOperationException(string.Format("Next of previous page number can not be found for patterns \"{0}\" or \"{1}\".", PreviousPagePattern, NextPagePattern));

            AbsoluteUri[] links = productLinkTags
                .Select(tag => tag.GetAttributeValue("href"))
                .Where(href => !string.IsNullOrEmpty(href))
                .Select(href => new AbsoluteUri(url, href))
                .Distinct()
                .ToArray();

            if (links.Length == 0)
                throw new InvalidOperationException(string.Format("Link can not be parsed for pattern \"{0}\".", ProductLinkPattern));

            var result = new CatalogPageInfo(url)
                {
                    PageNumber = pageNumber,
                    Links = links,
                    NextPageLink = nextPageLink,
                    PreviousPageLink = previousPageLink,
                };

            return new[] { result };
        }
    }
}