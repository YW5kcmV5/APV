using System;
using System.Collections.Generic;
using System.Drawing;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Common;
using APV.Common.Periods;
using APV.Pottle.WebParsers.Avtoberg;
using APV.Pottle.WebParsers.InfoEntities;

namespace APV.Avtoliga.Export.ConsoleApplication
{
    public sealed class ProductExporter
    {
        public readonly SortedList<string, long> UrlToIdTransfromation = new SortedList<string, long>();

        private static int ParseDeliveryPeriod(string value)
        {
            int index = value.IndexOf(" ");
            value = value.Substring(0, index);
            int deliveryPeriod = int.Parse(value);
            
            if (deliveryPeriod <= 0)
                throw new ArgumentOutOfRangeException("value", string.Format("Invalid delivery period \"{0}\".", value));

            return deliveryPeriod;
        }

        public void Export(AvtobergSupplierProductInfo from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            string name = from.ProductName;
            string trademarkName = from.TrademarkName;
            string modelName = from.ModelName;
            AnnualPeriodCollection modelPeriod = from.ModelPeriod;
            string producerName = from.ProducerCompanyName;
            string groupName = from.Navigator[0];
            AnnualPeriodInfo period = from.ProducedPeriod;
            string article = from.OriginalArticle;
            string producerArticle = from.Article;
            bool outOfStock = from.OutOfStock;
            int delieryPeriod = (!outOfStock) ? ParseDeliveryPeriod(from.DeliveryPeriod) : 0;
            double cost = from.ProductCost;

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentOutOfRangeException("from", "Product name is null or white space.");
            if (string.IsNullOrWhiteSpace(trademarkName))
                throw new ArgumentOutOfRangeException("from", "Trademark name is null or white space.");
            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentOutOfRangeException("from", "Model name is null or white space.");
            if (cost < 0.00)
                throw new ArgumentNullException("from", string.Format("Bad product cost \"{0}\".", cost));

            article = (!string.IsNullOrWhiteSpace(article)) ? article.Trim() : null;

            groupName = groupName.Replace("Решетка радиатора", "Решётка радиатора");

            ProductGroupEntity group = ProductGroupManagement.Instance.FindByName(groupName);
            TrademarkEntity trademark = TrademarkManagement.Instance.FindByName(trademarkName);
            ModelEntity model = ModelManagement.Instance.Find(trademark, modelName, modelPeriod);

            if (group == null)
                throw new ArgumentOutOfRangeException("from", string.Format("Group \"{0}\" can not be found.", groupName));
            if (trademark == null)
                throw new ArgumentOutOfRangeException("from", string.Format("Trademark \"{0}\" can not be found.", trademarkName));
            if (model == null)
                throw new ArgumentOutOfRangeException("from", string.Format("Model \"{0}\" can not be found.", modelName));

            ProducerEntity producer = null;
            if (!string.IsNullOrWhiteSpace(producerName))
            {
                producer = ProducerManagement.Instance.FindByName(producerName);

                if (producer == null)
                    throw new ArgumentOutOfRangeException("from", string.Format("Producer \"{0}\" can not be found.", producerName));
                if (string.IsNullOrWhiteSpace(producerArticle))
                    throw new ArgumentOutOfRangeException("from", "Producer article is null or white space.");

                producerArticle = producerArticle.Trim();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(producerArticle))
                    throw new ArgumentOutOfRangeException("from", "Producer article is defined, but producer name is not.");

                producerArticle = null;
            }

            var product = ProductManagement.Instance.Find(model, name, period);
            product = product ?? new ProductEntity();

            product.Name = name.Trim();
            product.Article = article;
            product.Model = model;
            product.ProductGroup = group;
            product.Producer = producer;
            product.ProducerArticle = producerArticle;
            product.PeriodInfo = period;
            product.DeliveryTime = delieryPeriod;
            product.Cost = cost;
            product.OutOfStock = outOfStock;

            product.Save();

            string key = from.Url.ToString().ToLowerInvariant();
            if (!UrlToIdTransfromation.ContainsKey(key))
            {
                UrlToIdTransfromation.Add(key, product.Id);
            }
            else
            {
                long existingId = UrlToIdTransfromation[key];
                if (existingId != product.ProductId)
                    throw new ArgumentOutOfRangeException("from", "Several product generated from the same url!");
            }

            foreach (ImageInfo imageInfo in from.Images)
            {
                Image image = imageInfo.Data.ToBitmap();
                ImageManagement.Instance.AddImageToSet(product, image);
            }
        }

        public void ExportBuyTogetherProductReferences(AvtobergSupplierProductInfo from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            string key = from.Url.ToString().ToLowerInvariant();
            if ((from.BuyTogetherProductReferences != null) && (UrlToIdTransfromation.ContainsKey(key)))
            {
                long productId = UrlToIdTransfromation[key];
                foreach (AbsoluteUri reference in from.BuyTogetherProductReferences)
                {
                    string referenceKey = reference.ToString().ToLowerInvariant();
                    if (UrlToIdTransfromation.ContainsKey(referenceKey))
                    {
                        long referenceId = UrlToIdTransfromation[referenceKey];
                        if (productId != referenceId)
                        {
                            ProductManagement.Instance.AddBuyTogetherProduct(productId, referenceId);
                        }
                    }
                }
            }
        }
    }
}