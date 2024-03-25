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
    public class ModelManagement : BaseManagement<ModelEntity, ModelCollection, ModelDataLayerManager>
    {
        [ClientAccess]
        public virtual ModelEntity Find(TrademarkEntity trademark, string name, AnnualPeriodCollection period = null)
        {
            if (trademark == null)
                throw new ArgumentNullException("trademark");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return DatabaseManager.FindByName(trademark.TrademarkId, name, (period != null) ? period.ToString() : null);
        }

        [AdminAccess]
        public override void Save(ModelEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity.TrademarkId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("entity", string.Format("Trademark is not defined for model."));
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentOutOfRangeException("entity", string.Format("Model name is null or white space."));

            ModelEntity existing = DatabaseManager.FindByName(entity.TrademarkId, entity.Name, entity.Period);

            if ((existing != null) && (existing.ModelId != entity.ModelId))
                throw new ArgumentOutOfRangeException("entity", string.Format("Model with name \"{0} ({1})\" already exists for trademark \"{2}\".", entity.Name, entity.Period, entity.TrademarkId));

            base.Save(entity);
        }

        [AdminAccess]
        public override void Delete(ModelEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.ModelId == SystemConstants.UnknownId)
            {
                return;
            }

            using (var transaction = new TransactionScope())
            {
                int reference = DatabaseManager.GetReferenceCount(entity.ModelId);
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

        [AdminAccess]
        public ModelCollection GetList(TrademarkEntity trademark)
        {
            if (trademark == null)
                throw new ArgumentNullException("trademark");
            if (trademark.TrademarkId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("trademark", "Specified trademark entity is new (is not stored in database).");

            return DatabaseManager.GetList(trademark.TrademarkId);
        }

        public static readonly ModelManagement Instance = (ModelManagement)EntityFrameworkManager.GetManagement<ModelEntity>();
    }
}