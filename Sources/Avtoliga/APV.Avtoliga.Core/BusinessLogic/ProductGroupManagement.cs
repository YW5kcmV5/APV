using System;
using APV.Avtoliga.Core.DataLayer;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.EntityFramework.DataLayer;
using APV.Common;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public class ProductGroupManagement : BaseManagement<ProductGroupEntity, ProductGroupCollection, ProductGroupDataLayerManager>
    {
        [AdminAccess]
        public override void Save(ProductGroupEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentOutOfRangeException("entity", string.Format("Producer group name is null or white space."));

            ProductGroupEntity existing = DatabaseManager.FindByName(entity.Name);

            if ((existing != null) && (existing.ProductGroupId != entity.ProductGroupId))
                throw new ArgumentOutOfRangeException("entity", string.Format("Producer group with name \"{0}\" already exists.", entity.Name));

            base.Save(entity);
        }

        [AdminAccess]
        public override void Delete(ProductGroupEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.ProductGroupId == SystemConstants.UnknownId)
            {
                return;
            }

            using (var transaction = new TransactionScope())
            {
                int reference = DatabaseManager.GetReferenceCount(entity.ProductGroupId);
                if (reference == 0)
                {
                    DatabaseManager.Delete(entity);
                }
                else
                {
                    DatabaseManager.MarkAsDeleted(entity);
                }
                transaction.Commit();
            }
        }

        public static readonly ProductGroupManagement Instance = (ProductGroupManagement)EntityFrameworkManager.GetManagement<ProductGroupEntity>();
    }
}