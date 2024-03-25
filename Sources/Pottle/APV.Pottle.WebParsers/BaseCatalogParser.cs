using System;
using System.Collections.Generic;
using System.Linq;
using APV.Common;
using APV.Pottle.WebParsers.InfoEntities;
using APV.Pottle.WebParsers.ResultEntities;

namespace APV.Pottle.WebParsers
{
    public abstract class BaseCatalogParser<TCatalogParserInfo> : BaseParser<TCatalogParserInfo>
        where TCatalogParserInfo : CatalogPageInfo
    {
        private AbsoluteUri[] ListProductReferences(AbsoluteUri catalogUri, List<string> parsedCatalogUrls)
        {
            if (parsedCatalogUrls.Contains(catalogUri.Url))
            {
                return new AbsoluteUri[0];
            }

            parsedCatalogUrls.Add(catalogUri.Url);
            ParseResult<TCatalogParserInfo> result = Parse(catalogUri);

            if (!result.Success)
                throw new InvalidOperationException(string.Format("Catalog page \"{0}\" can not be parsed. See inner exception for details.", catalogUri.Url), result.Exception);

            var list = new List<AbsoluteUri>();

            foreach (TCatalogParserInfo catalogInfo in result.Data)
            {
                list.AddRange(catalogInfo.Links);
                if (catalogInfo.NextPageLink != null)
                {
                    AbsoluteUri[] nextPageLinks = ListProductReferences(catalogInfo.NextPageLink, parsedCatalogUrls);
                    list.AddRange(nextPageLinks);
                }
                if (catalogInfo.PreviousPageLink != null)
                {
                    AbsoluteUri[] nextPageLinks = ListProductReferences(catalogInfo.PreviousPageLink, parsedCatalogUrls);
                    list.AddRange(nextPageLinks);
                }
            }

            return list.Distinct().ToArray();
        }

        public virtual AbsoluteUri[] ListProductReferences(AbsoluteUri initialCatalogUri)
        {
            var parsedCatalogUrls = new List<string>();
            return ListProductReferences(initialCatalogUri, parsedCatalogUrls);
        }
    }
}