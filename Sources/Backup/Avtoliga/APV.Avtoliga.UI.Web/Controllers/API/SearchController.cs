using System.Collections.Generic;
using System.Linq;
using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.Avtoliga.UI.Web.Models;
using APV.Avtoliga.UI.Web.Models.Entities;
using APV.Common.Extensions;

namespace APV.Avtoliga.UI.Web.Controllers.API
{
    public static class SearchController
    {
        public static ProducerInfo[] GetAllProducers(byte? limit = null)
        {
            ProducerCollection producers = ProducerManagement.Instance.GetAll();
            ProducerInfo[] items = producers.Transform(false);
            if (limit != null)
            {
                items = items.Take(limit.Value).ToArray();
            }
            return items;
        }

        public static TrademarkInfo[] GetAllTrademarks()
        {
            var result = new List<TrademarkInfo>();
            TrademarkCollection trademarks = TrademarkManagement.Instance.GetAll();
            foreach (TrademarkEntity trademark in trademarks)
            {
                ModelCollection models = ModelManagement.Instance.GetList(trademark);
                TrademarkInfo trademarkInfo = trademark.Transform(false, models);
                result.Add(trademarkInfo);
            }
            return result.ToArray();
        }

        public static ProductGroupInfo[] GetAllProductGroups()
        {
            ProductGroupCollection groups = ProductGroupManagement.Instance.GetAll();
            var allGroups = new List<ProductGroupInfo> { ProductGroupInfo.All };
            allGroups.AddRange(groups.Transform());
            return allGroups.ToArray();
        }

        public static ModelInfo FindModel(long trademarkId, string modelId)
        {
            long modelIdValue = modelId.ToLong(0);
            ModelEntity model = ModelManagement.Instance.Find(modelIdValue);
            if ((model != null) && (model.TrademarkId == trademarkId))
            {
                return model.Transform(true);
            }
            return null;
        }

        public static ProductGroupInfo FindProductGroup(string groupId)
        {
            if (!string.IsNullOrWhiteSpace(groupId))
            {
                long groupIdValue = groupId.ToLong(0);
                ProductGroupEntity productGroup = ProductGroupManagement.Instance.Find(groupIdValue);
                if (productGroup != null)
                {
                    return productGroup.Transform();
                }
            }
            return ProductGroupInfo.All;
        }

        public static ProducerInfo FindProducer(string producerId)
        {
            if (!string.IsNullOrWhiteSpace(producerId))
            {
                long producerIdValue = producerId.ToLong(0);
                ProducerEntity producer = ProducerManagement.Instance.Find(producerIdValue);
                if (producer != null)
                {
                    return producer.Transform(true);
                }
            }
            return null;
        }

        public static ProductInfo FindProduct(string trademarkId, string modelId, string productId)
        {
            long productIdValue = productId.ToLong(0);
            ProductEntity product = ProductManagement.Instance.Find(productIdValue);
            if (product != null)
            {
                ProductCollection references = ProductManagement.Instance.GetTogetherProducts(product.ProductId);
                return product.Transform(references, true);
            }
            return null;
        }

        public static ProductInfo[] FindProducts(long modelId, long? producerId, long? productGroupId)
        {
            ProductCollection products = ProductManagement.Instance.GetList(modelId, producerId, productGroupId);
            return products.Transform(false);
        }

        public static SearchModel Search(string trademarkId, string modelId, string groupId, string producerId)
        {
            TrademarkInfo trademark = TrademarkController.Find(trademarkId);
            ModelInfo model = (trademark != null) ? FindModel(trademark.TrademarkId, modelId) : null;
            ProductGroupInfo productGroup = FindProductGroup(groupId);
            ProducerInfo producer = FindProducer(producerId);
            long? productGroupId = (!productGroup.IsAll) ? productGroup.ProductGroupId : (long?)null;
            long? producerIdValue = (producer != null) ? producer.ProducerId : (long?)null;
            var products = new ProductInfo[0];
            bool productsInited = false;
            if (model != null)
            {
                products = FindProducts(model.ModelId, producerIdValue, productGroupId);
                productsInited = true;
            }

            var result = new SearchModel
                {
                    Trademark = trademark,
                    Model = model,
                    Group = productGroup,
                    Producer = producer,
                    Products = products,
                    ProductsInited = productsInited
                };

            return result;
        }
    }
}