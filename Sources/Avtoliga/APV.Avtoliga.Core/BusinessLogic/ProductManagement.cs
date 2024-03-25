using System;
using APV.Avtoliga.Core.DataLayer;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.EntityFramework.DataLayer;
using APV.Common;
using APV.Common.Periods;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public class ProductManagement : BaseManagement<ProductEntity, ProductCollection, ProductDataLayerManager>
    {
        [ClientAccess]
        public virtual ProductEntity Find(ModelEntity model, string name, AnnualPeriodInfo period = null)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return DatabaseManager.FindByName(model.ModelId, name, (period != null) ? period.ToString() : null);
        }

        [ClientAccess]
        public virtual ProductEntity Find(ProducerEntity producer, string producerArticle)
        {
            if (producer == null)
                throw new ArgumentNullException("producer");

            return Find(producer.ProducerId, producerArticle);
        }

        [ClientAccess]
        public virtual ProductEntity Find(long producerId, string producerArticle)
        {
            if (string.IsNullOrEmpty(producerArticle))
                throw new ArgumentNullException("producerArticle");

            return DatabaseManager.Find(producerId, producerArticle);
        }

        [AnonymousAccess]
        public virtual ProductCollection GetList(long modelId, long? producerId = null, long? productGroupId = null)
        {
            if (modelId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("modelId", "Specified model id is new (is not stored in database).");
            if ((productGroupId != null) && (productGroupId == SystemConstants.UnknownId))
                throw new ArgumentOutOfRangeException("productGroupId", "Specified product group id is new (is not stored in database).");
            if ((producerId != null) && (producerId == SystemConstants.UnknownId))
                throw new ArgumentOutOfRangeException("producerId", "Specified producer id is new (is not stored in database).");

            return DatabaseManager.GetList(modelId, producerId, productGroupId);
        }

        [AnonymousAccess]
        public virtual ProductCollection GetList(ProducerEntity producer)
        {
            if (producer == null)
                throw new ArgumentNullException("producer");
            if (producer.ProducerId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("producer", "Specified producer entity is new (is not stored in database).");

            return DatabaseManager.GetList(producer.ProducerId);
        }

        [AnonymousAccess]
        public virtual ProductCollection GetLastSpecialOffers()
        {
            return DatabaseManager.GetSpecialOffers(2);
        }

        [AnonymousAccess]
        public virtual ProductCollection GetSpecialOffers()
        {
            return DatabaseManager.GetSpecialOffers();
        }

        [AnonymousAccess]
        public ProductCollection GetTogetherProducts(long productId)
        {
            return DatabaseManager.FindReferenceProducts(productId);
        }

        public long[] FindImageSet(ProductEntity product)
        {
            return ImageManagement.Instance.FindImageSet(product);
        }

        [AnonymousAccess]
        public virtual ProductCollection GetPriceList()
        {
            return DatabaseManager.GetPriceList();
        }

        [AdminAccess]
        public override void Save(ProductEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentOutOfRangeException("entity", string.Format("Producer group name is null or white space."));
            if ((entity.ProducerId != null) && (string.IsNullOrWhiteSpace(entity.ProducerArticle)))
                throw new ArgumentOutOfRangeException("entity", string.Format("Producer article is null or white space."));
            if ((entity.ProducerId == null) && (entity.ProducerArticle != null))
                throw new ArgumentOutOfRangeException("entity", string.Format("Producer article is defined, but producer is not."));

            ProductEntity existing = DatabaseManager.FindByName(entity.ModelId, entity.Name, entity.Period);
            if ((existing != null) && (existing.ProductId != entity.ProductId))
                throw new ArgumentOutOfRangeException("entity", string.Format("Product with name \"{0}\" already exists for model \"{1}\" ({2}).", entity.Name, existing.Model.Name, entity.Period));

            //if (entity.ProducerId != null)
            //{
            //    existing = DatabaseManager.Find(entity.ProducerId.Value, entity.ProducerArticle);
            //    if ((existing != null) && (existing.ProductId != entity.ProductId))
            //        throw new ArgumentOutOfRangeException("entity", string.Format("Product with product with article \"{0}\" already exists for producer \"{1}\".", entity.ProducerArticle, existing.Producer.Name));
            //}

            base.Save(entity);
        }

        [AdminAccess]
        public override void Delete(ProductEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.ProductId == SystemConstants.UnknownId)
            {
                return;
            }

            using (var transaction = new TransactionScope())
            {
                ImageManagement.Instance.ClearImageSet(entity);
                DatabaseManager.ClearReferences(entity.ProductId);
                DatabaseManager.Delete(entity);
                transaction.Commit();
            }
        }

        [AdminAccess]
        public void Delete(long productId)
        {
            ProductEntity entity = Find(productId);
            if (entity != null)
            {
                Delete(productId);
            }
        }

        [AdminAccess]
        public void AddBuyTogetherProduct(ProductEntity product, ProductEntity referenceProduct)
        {
            if (product == null)
                throw new ArgumentNullException("product");
            if (referenceProduct == null)
                throw new ArgumentNullException("referenceProduct");

            AddBuyTogetherProduct(product.ProductId, referenceProduct.ProductId);
        }

        [AdminAccess]
        public void AddBuyTogetherProduct(long productId, long referenceProductId)
        {
            if (productId == referenceProductId)
                throw new ArgumentOutOfRangeException("referenceProductId", "Product can not be added as own \"by together\" reference.");

            DatabaseManager.AddToReferences(productId, referenceProductId);
        }

        [AdminAccess]
        public void DeleteBuyTogetherProduct(long productId, long referenceProductId)
        {
            DatabaseManager.DeleteFromReferences(productId, referenceProductId);
        }

        [AdminAccess]
        public void ClearTogetherProducts(long productId)
        {
            DatabaseManager.ClearReferences(productId);
        }

        public static readonly ProductManagement Instance = (ProductManagement)EntityFrameworkManager.GetManagement<ProductEntity>();
    }
}