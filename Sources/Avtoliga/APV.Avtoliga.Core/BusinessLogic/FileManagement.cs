using System;
using APV.Avtoliga.Core.DataLayer;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public class FileManagement : BaseManagement<FileEntity, FileCollection, FileDataLayerManager>
    {
        [AdminAccess]
        public override void Save(FileEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                entity.Name = null;
                base.Save(entity);
                return;
            }

            using (var transaction = new TransactionScope())
            {
                FileEntity existing = FindByName(entity.Name);

                if ((existing != null) && (existing.FileId != entity.FileId))
                    throw new ArgumentOutOfRangeException("entity", string.Format("File with name \"{0}\" (\"{1}\") already exists.", existing.Name, existing.FileId));

                base.Save(entity);
                transaction.Commit();
            }

            base.Save(entity);
        }

        public static readonly FileManagement Instance = (FileManagement)EntityFrameworkManager.GetManagement<FileEntity>();
    }
}