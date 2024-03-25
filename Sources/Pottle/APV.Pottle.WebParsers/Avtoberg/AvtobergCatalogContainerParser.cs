using System;
using System.Collections.Generic;
using System.Linq;
using APV.Common;
using APV.Common.Html;
using APV.Common.Periods;
using APV.GraphicsLibrary.Extensions;
using APV.GraphicsLibrary.Images;
using APV.Pottle.WebParsers.InfoEntities;
using APV.Pottle.WebParsers.ResultEntities;

namespace APV.Pottle.WebParsers.Avtoberg
{
    public class AvtobergCatalogContainerParser : BaseParser<CatalogContainerInfo>
    {
        private const string TrademarkLinkPattern = @"ul.brands li a";
        private const string ModelLinkPattern = @"table.models a";
        private const string TrademarkDescriptionPattern = @"div.b-description p";

        private readonly List<AvtobergTrademarkInfo> _trademarks = new List<AvtobergTrademarkInfo>();

        protected override CatalogContainerInfo[] Parse(AbsoluteUri url, HtmlDocument doc)
        {
            List<HtmlTag> tags = doc.Find(TrademarkLinkPattern);

            var links = new List<AbsoluteUri>();
            if (tags.Count > 0)
            {
                _trademarks.Clear();
                foreach (HtmlTag tag in tags)
                {
                    var href = new AbsoluteUri(url, tag.GetAttributeValue("href"));
                    string name = tag.Children[1].Text;
                    string logoSrc = tag.Children[0].GetAttributeValue("src");
                    var logoUrl = ((!string.IsNullOrEmpty(logoSrc)) && (logoSrc.Length > 1)) ? new AbsoluteUri(url, logoSrc) : null;
                    ImageContainer logo = (logoUrl != null) ? logoUrl.GetImage() : null;
                    var trademark = new AvtobergTrademarkInfo
                        {
                            Name = name,
                            Logo = logo,
                            Url = href,
                        };
                    _trademarks.Add(trademark);
                }

                AbsoluteUri[] trademarkLinks = _trademarks
                    .Select(trademark => trademark.Url)
                    .ToArray();

                foreach (AbsoluteUri trademarkLink in trademarkLinks)
                {
                    ParseResult<CatalogContainerInfo> result = Parse(trademarkLink);

                    if (!result.Success)
                        throw new InvalidOperationException(string.Format("Catalog page \"{0}\" cannot be parsed. See inner exception for details.", trademarkLink), result.Exception);

                    links.AddRange(result.Data.SelectMany(x => x.Links));
                }
            }
            else
            {
                tags = doc.Find(ModelLinkPattern);
                List<HtmlTag> descriptionTags = doc.Find(TrademarkDescriptionPattern);
                string description = string.Join("\r\n", descriptionTags.Select(item => item.Text));

                AvtobergTrademarkInfo trademark = _trademarks.Single(item => item.Url == url);

                var models = new SortedList<string, AvtobergModelInfo>();
                foreach (HtmlTag tag in tags)
                {
                    string name = tag.Text;
                    string modelPeriodValue = tag.Parent.NextSibling.Text;
                    AnnualPeriodCollection modelPeriod = (!string.IsNullOrEmpty(modelPeriodValue))
                                                             ? new AnnualPeriodCollection(modelPeriodValue)
                                                             : null;
                    var modelUrl = new AbsoluteUri(url, tag.GetAttributeValue("href"));
                    
                    string modelKey = (modelPeriod != null)
                                          ? string.Format("{0}:{1}", name, modelPeriod)
                                          : name;

                    var model = new AvtobergModelInfo
                        {
                            Url = modelUrl,
                            Name = name,
                            ModelPeriod = modelPeriod,
                            Key = modelKey,
                        };

                    if (!models.ContainsKey(modelKey))
                    {
                        models.Add(modelKey, model);
                    }
                }

                trademark.Models = models.Values.ToArray();
                trademark.Description = description;

                AbsoluteUri[] modelLinks = models.Values
                    .Select(model => model.Url)
                    .ToArray();

                links.AddRange(modelLinks);
            }

            var catalogInfo = new CatalogContainerInfo(url)
                {
                    Links = links.ToArray(),
                    Trademarks = _trademarks.ToArray(),
                };

            return new[]
                {
                    catalogInfo
                };
        }
    }
}