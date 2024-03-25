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
    public class ProducerManagement : BaseManagement<ProducerEntity, ProducerCollection, ProducerDataLayerManager>
    {
        [AdminAccess]
        public override void Save(ProducerEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (string.IsNullOrWhiteSpace(entity.Name))
                throw new ArgumentOutOfRangeException("entity", string.Format("Producer name is null or white space."));

            ProducerEntity existing = DatabaseManager.FindByName(entity.Name);

            if ((existing != null) && (existing.ProducerId != entity.ProducerId))
                throw new ArgumentOutOfRangeException("entity", string.Format("Producer with name \"{0}\" already exists.", entity.Name));

            base.Save(entity);
        }

        [AdminAccess]
        public override void Delete(ProducerEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (entity.ProducerId == SystemConstants.UnknownId)
            {
                return;
            }

            using (var transaction = new TransactionScope())
            {
                int reference = DatabaseManager.GetReferenceCount(entity.ProducerId);
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

        public static readonly ProducerManagement Instance = (ProducerManagement)EntityFrameworkManager.GetManagement<ProducerEntity>();
    }
}