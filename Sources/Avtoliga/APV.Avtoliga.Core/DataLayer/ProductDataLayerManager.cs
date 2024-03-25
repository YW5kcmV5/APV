using System;
using System.Collections.Generic;
using System.Text;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;
using APV.Common;

namespace APV.Avtoliga.Core.DataLayer
{
    public class ProductDataLayerManager : BaseDataLayerManager<ProductEntity, ProductCollection>
    {
        public ProductEntity FindByName(long modelId, string name, string period = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            const string whereSqlWithPeriod = @"WHERE ([Product].ModelId = @ModelId) AND ([Product].Name = @Name) AND ([Product].Period = @Period)";
            const string whereSqlWithoutPeriod = @"WHERE ([Product].ModelId = @ModelId) AND ([Product].Name = @Name) AND ([Product].Period IS NULL)";

            bool hasPeriod = (!string.IsNullOrWhiteSpace(period));
            string whereSql;

            var @params = new Dictionary<string, object>
                    {
                        { "@ModelId", modelId },
                        { "@Name", name },
                    };

            if (hasPeriod)
            {
                whereSql = whereSqlWithPeriod;
                @params.Add("@Period", period);
            }
            else
            {
                whereSql = whereSqlWithoutPeriod;
            }

            return Find(whereSql, @params);
        }

        public ProductEntity Find(long producerId, string producerArticle)
        {
            if (producerId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("producerId", "Specified producer id is new (is not stored in database).");
            if (string.IsNullOrEmpty(producerArticle))
                throw new ArgumentNullException("producerArticle");

            const string whereSql = @"WHERE ([Product].ProducerId = @ProducerId) AND ([Product].ProducerArticle = @ProducerArticle)";

            var @params = new Dictionary<string, object>
                    {
                        { "@ProducerId", producerId },
                        { "@ProducerArticle", producerArticle },
                    };

            return Find(whereSql, @params);
        }

        public long[] FindReferences(long productId)
        {
            const string sql = @"SELECT [ReferenceProductId] FROM [ProductReference] WHERE [ProductReference].ProductId = @ProductId";
            var @params = new Dictionary<string, object> { { "@ProductId", productId } };
            return GetKeys(sql, @params);
        }

        public ProductCollection FindReferenceProducts(long productId)
        {
            const string whereSql = @"JOIN [ProductReference] ON [ProductReference].ReferenceProductId = [Product].ProductId WHERE [ProductReference].ProductId = @ProductId";
            var @params = new Dictionary<string, object> { { "@ProductId", productId } };
            return GetList(whereSql, @params);
        }

        public void AddToReferences(long productId, long referenceProductId)
        {
            if (productId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("productId", "Specified product id is new (is not stored in database).");
            if (referenceProductId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("referenceProductId", "Specified reference product id is new (is not stored in database).");

            var @params = new Dictionary<string, object>
                {
                    {"@ProductId", productId},
                    {"@ReferenceProductId", referenceProductId},
                };

            const string sql = @"
IF (NOT(EXISTS (SELECT [ProductReference].ProductId FROM [ProductReference] WHERE ([ProductReference].ProductId = @ProductId) AND ([ProductReference].ReferenceProductId = @ReferenceProductId)))) BEGIN
    INSERT INTO [ProductReference] (ProductId, ReferenceProductId) VALUES (@ProductId, @ReferenceProductId)
END";

            Execute(sql, @params);
        }

        public void DeleteFromReferences(long productId, long referenceProductId)
        {
            if (productId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("productId", "Specified product id is new (is not stored in database).");
            if (referenceProductId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("referenceProductId", "Specified reference product id is new (is not stored in database).");

            var @params = new Dictionary<string, object>
                {
                    {"@ProductId", productId},
                    {"@ReferenceProductId", referenceProductId},
                };

            const string sql = @"DELETE FROM [ProductReference] WHERE ([ProductReference].ProductId = @ProductId) AND ([ProductReference].ReferenceProductId = @ReferenceProductId)";

            Execute(sql, @params);
        }

        public void ClearReferences(long productId)
        {
            if (productId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("productId", "Specified product id is new (is not stored in database).");

            var @params = new Dictionary<string, object> { {"@ProductId", productId} };

            const string sql = @"DELETE FROM [ProductReference] WHERE ([ProductReference].ProductId = @ProductId)";

            Execute(sql, @params);
        }

        public ProductCollection GetList(long modelId, long? producerId, long? productGroupId)
        {
            var whereSqlBuilder = new StringBuilder("WHERE ([Product].ModelId = @ModelId) ");

            var @params = new Dictionary<string, object> { { "@ModelId", modelId } };

            if (producerId != null)
            {
                whereSqlBuilder.Append(" AND ([Product].ProducerId = @ProducerId)");
                @params.Add("@ProducerId", producerId.Value);
            }

            if (productGroupId != null)
            {
                whereSqlBuilder.Append(" AND ([Product].GroupId = @ProductGroupId)");
                @params.Add("@ProductGroupId", productGroupId.Value);
            }

            string whereSql = whereSqlBuilder.ToString();

            return GetList(whereSql, @params);
        }

        public ProductCollection GetList(long producerId)
        {
            const string whereSql = @"WHERE ([Product].ProducerId = @ProducerId)";
            var @params = new Dictionary<string, object> { { "@ProducerId", producerId } };
            return GetList(whereSql, @params);
        }

        public ProductCollection GetPriceList()
        {
            return GetList("ORDER BY ProducerId DESC, ModelId, Name");
        }

        public ProductCollection GetSpecialOffers(int? top = null)
        {
            const string whereSql = @"WHERE ([Product].SpecialOffer = 1) ORDER BY ModifiedAt DESC";
            return (top != null)
                       ? GetList(whereSql, top.Value)
                       : GetList(whereSql);
        }
    }
}