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
    public class TrademarkManagement : BaseManagement<TrademarkEntity, TrademarkCollection, TrademarkDataLayerManager>
    {
        [AdminAccess]
        public override void Save(TrademarkEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentOutOfRangeException("entity", string.Format("Trademark name is null or white space."));

            TrademarkEntity existing = DatabaseManager.FindByName(entity.Name);

            if ((existing != null) && (existing.TrademarkId != entity.TrademarkId))
                throw new ArgumentOutOfRangeException("entity", string.Format("Trademark with name \"{0}\" already exists.", entity.Name));

            base.Save(entity);
        }

        [AdminAccess]
        public override void Delete(TrademarkEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.TrademarkId == SystemConstants.UnknownId)
            {
                return;
            }

            using (var transaction = new TransactionScope())
            {
                int reference = DatabaseManager.GetReferenceCount(entity.TrademarkId);
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

        public static readonly TrademarkManagement Instance = (TrademarkManagement)EntityFrameworkManager.GetManagement<TrademarkEntity>();
    }
}