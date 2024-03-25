using System.Collections.Generic;
using System.Linq;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.UI.Web.Models.Entities;
using APV.Common;

namespace APV.Avtoliga.UI.Web.Controllers.API
{
    public static class Transformer
    {
        public static string ToHtmlText(this string text)
        {
            text = (!string.IsNullOrWhiteSpace(text))
                       ? string.Format("<p>{0}</p>", text.Trim().Replace("\n", "</p><p>").Replace("\r\n", "</p><p>"))
                       : null;
            return text;
        }

        public static string ToDeliveryString(int deliveryTime)
        {
            return (deliveryTime <= 0) ? null : (deliveryTime == 1) ? "1 день" : string.Format("{0} дней", deliveryTime);
        }

        public static FeedbackEntity Transform(this FeedbackInfo from)
        {
            if (from == null)
            {
                return null;
            }

            return new FeedbackEntity
            {
                Name = from.Name,
                Email = from.Email,
                Phone = from.Phone,
                Text = from.Text,
                Type = from.Type,
            };
        }

        public static FeedbackInfo Transform(this FeedbackEntity from)
        {
            if (from == null)
            {
                return null;
            }

            return new FeedbackInfo
                {
                    FeedbackId = from.FeedbackId,
                    Name = from.Name,
                    Email = from.Email,
                    Phone = from.Phone,
                    Text = from.Text,
                    TextP = from.Text.ToHtmlText(),
                    CreatedAt = from.CreatedAt.ToLongDateString(),
                    Likes = from.Likes ?? 0,
                    Liked = FeedbackController.GetLike(from.FeedbackId),
                };
        }

        public static FeedbackInfo[] Transform(this IEnumerable<FeedbackEntity> from)
        {
            return (from != null)
                       ? from.Where(item => item != null)
                             .Select(Transform)
                             .ToArray()
                       : new FeedbackInfo[0];
        }

        public static ArticleEntity Transform(this ArticleInfo from)
        {
            if (from == null)
            {
                return null;
            }

            return new ArticleEntity
                {
                    Name = from.Name,
                    Html = from.Html,
                };
        }

        public static ArticleInfo Transform(this ArticleEntity from, bool includeGroup)
        {
            if (from == null)
            {
                return null;
            }

            ArticleGroupInfo articleGroup =
                ((includeGroup) && (from.ArticleGroup != null))
                    ? from.ArticleGroup.Transform(false)
                    : null;

            var breadcrumbs = new List<ArticleGroupInfo>();
            ArticleGroupEntity parent = from.ArticleGroup;
            while (parent != null)
            {
                breadcrumbs.Insert(0, parent.Transform(false));
                parent = parent.Parent;
            }

            string url = HtmlHelpers.FormatUrl(string.Format("/articles/{0}/{1}/", from.ArticleGroupId ?? SystemConstants.UnknownId, from.ArticleId));

            return new ArticleInfo
            {
                ArticleId = from.ArticleId,
                ArticleGroupId = from.ArticleGroupId ?? SystemConstants.UnknownId,
                ArticleGroup = articleGroup,
                Breadcrumbs = breadcrumbs.ToArray(),
                Name = from.Name,
                Description = from.Description,
                Html = from.Html,
                CreatedAt = from.CreatedAt,
                Url = url
            };
        }

        public static ArticleInfo[] Transform(this IEnumerable<ArticleEntity> from, bool includeGroup)
        {
            return (from != null)
                       ? from
                             .Where(item => item != null)
                             .Select(item => Transform(item, includeGroup))
                             .ToArray()
                       : new ArticleInfo[0];
        }

        public static ArticleGroupInfo Transform(this ArticleGroupEntity from, bool includeArticles)
        {
            if (from == null)
            {
                return null;
            }

            ArticleInfo[] articles =
                ((includeArticles) && (from.Articles != null))
                    ? from.Articles.Transform(false)
                    : null;

            ArticleGroupInfo[] children = from.Children.Transform(includeArticles);

            string url = HtmlHelpers.FormatUrl(string.Format("/articles/{0}/", from.ArticleGroupId));

            return new ArticleGroupInfo
                {
                    ArticleGroupId = from.ArticleGroupId,
                    Name = from.Name,
                    Articles = articles,
                    Url = url,
                    Children = children
                };
        }

        public static ArticleGroupInfo[] Transform(this IEnumerable<ArticleGroupEntity> from, bool includeArticles)
        {
            return (from != null)
                       ? from
                             .Where(item => item != null)
                             .Select(item => item.Transform(includeArticles))
                             .ToArray()
                       : new ArticleGroupInfo[0];
        }

        public static NewsInfo Transform(this NewsEntity from)
        {
            if (from == null)
            {
                return null;
            }

            string logoUrl = (from.LogoImageId != null)
                                 ? HtmlHelpers.GetImageUrl(from.LogoImageId)
                                 : null;

            return new NewsInfo
                {
                    NewsId = from.NewsId,
                    Caption = from.Caption,
                    Text = from.Text,
                    TextP = from.Text.ToHtmlText(),
                    LogoUrl = logoUrl,
                    CreatedAt = from.CreatedAt.ToLongDateString(),
                    Likes = from.Likes,
                    Liked = NewsController.GetLike(from.NewsId),
                };
        }

        public static NewsInfo[] Transform(this IEnumerable<NewsEntity> from)
        {
            return (from != null)
                       ? from.Where(item => item != null)
                             .Select(Transform)
                             .ToArray()
                       : new NewsInfo[0];
        }

        public static TrademarkInfo Transform(this TrademarkEntity from, bool includeAbout, IEnumerable<ModelEntity> models = null)
        {
            if (from == null)
            {
                return null;
            }

            return new TrademarkInfo
                {
                    TrademarkId = from.TrademarkId,
                    Name = from.Name,
                    About = (includeAbout) ? from.About.ToHtmlText() : null,
                    Url = HtmlHelpers.FormatUrl(string.Format("/catalog/{0}", from.TrademarkId)),
                    LogoUrl = HtmlHelpers.GetImageUrl(from.LogoImageId),
                    Models = models.Transform(false),
                };
        }

        public static TrademarkInfo[] Transform(this IEnumerable<TrademarkEntity> from, bool includeAbout)
        {
            return (from != null)
                       ? from.Where(item => item != null)
                             .Select(item => item.Transform(includeAbout))
                             .ToArray()
                       : new TrademarkInfo[0];
        }

        public static ModelInfo[] Transform(this IEnumerable<ModelEntity> from, bool includeTrademark)
        {
            return (from != null)
                       ? from.Where(item => item != null)
                             .Select(item => item.Transform(includeTrademark))
                             .ToArray()
                       : new ModelInfo[0];
        }

        public static ModelInfo Transform(this ModelEntity from, bool includeTrademark)
        {
            if (from == null)
            {
                return null;
            }

            TrademarkInfo trademark = ((includeTrademark) && (from.Trademark != null))
                                          ? from.Trademark.Transform(false)
                                          : null;

            return new ModelInfo
                {
                    ModelId = from.ModelId,
                    TrademarkId = from.TrademarkId,
                    Name = from.Name,
                    Period = from.Period,
                    Url = HtmlHelpers.FormatUrl(string.Format("/search/{0}/{1}/0/", from.TrademarkId, from.ModelId)),
                    Trademark = trademark,
                };
        }

        public static ProductGroupInfo[] Transform(this IEnumerable<ProductGroupEntity> from)
        {
            return (from != null)
                       ? from.Where(item => item != null)
                             .Select(item => item.Transform())
                             .ToArray()
                       : new ProductGroupInfo[0];
        }

        public static ProductGroupInfo Transform(this ProductGroupEntity from)
        {
            if (from == null)
            {
                return null;
            }

            return new ProductGroupInfo
                {
                    ProductGroupId = from.ProductGroupId,
                    Name = from.Name,
                };
        }

        public static ProducerInfo[] Transform(this IEnumerable<ProducerEntity> from, bool includeAbout)
        {
            return (from != null)
                       ? from.Where(item => item != null)
                             .Select(item => item.Transform(includeAbout))
                             .ToArray()
                       : new ProducerInfo[0];
        }

        public static ProducerInfo Transform(this ProducerEntity from, bool includeAbout)
        {
            if (from == null)
            {
                return null;
            }

            return new ProducerInfo
                {
                    ProducerId = from.ProducerId,
                    Name = from.Name,
                    Title = from.Title,
                    About = (includeAbout) ? from.About.ToHtmlText() : null,
                    ExternalUrl = (from.Url != null) ? from.Url.ToString() : null,
                    LogoUrl = HtmlHelpers.GetImageUrl(from.LogoImageId),
                    Url = HtmlHelpers.FormatUrl(string.Format("/producer/{0}/", from.ProducerId)),
                };
        }

        public static ProductInfo[] Transform(this IEnumerable<ProductEntity> from, bool includeProducer)
        {
            return (from != null)
                       ? from.Where(item => item != null)
                             .Select(item => item.Transform(null, includeProducer))
                             .ToArray()
                       : new ProductInfo[0];
        }

        public static ProductInfo Transform(this ProductEntity from, IEnumerable<ProductEntity> references, bool includeProducer)
        {
            if (from == null)
            {
                return null;
            }

            ModelInfo model = from.Model.Transform(true);
            ProductGroupInfo group = ((includeProducer) && (from.ProductGroup != null))
                                        ? from.ProductGroup.Transform()
                                        : null;
            ProducerInfo producer = ((includeProducer) && (from.Producer != null))
                                        ? from.Producer.Transform(false)
                                        : null;
            ProductInfo[] togetherProducts = references.Transform(false);

            long[] imageIds = ProductManagement.Instance.FindImageSet(from);
            string[] imageUrls = imageIds.Select(imageId => HtmlHelpers.GetImageUrl(imageId)).ToArray();
            string logoUrl = HtmlHelpers.GetImageUrl(imageIds.FirstOrDefault());

            return new ProductInfo
                {
                    ProductId = from.ProductId,
                    Name = from.Name,
                    Article = from.Article,
                    ProducerArticle = from.ProducerArticle,
                    Period = from.Period,
                    OutOfStock = from.OutOfStock,
                    Cost = from.Cost,
                    SpecialOffer = from.SpecialOffer,
                    SpecialOfferDescription = from.SpecialOfferDescription,
                    Url = HtmlHelpers.FormatUrl(string.Format("/catalog/{0}/{1}/{2}/", model.TrademarkId, from.ModelId, from.ProductId)),
                    DeliveryTime = ToDeliveryString(from.DeliveryTime),
                    ImageUrls = imageUrls,
                    LogoUrl = logoUrl,
                    Model = model,
                    Group = group,
                    Producer = producer,
                    TogetherProducts = togetherProducts,
                };
        }
    }
}