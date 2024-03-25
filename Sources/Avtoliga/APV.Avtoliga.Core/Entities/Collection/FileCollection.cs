using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class FileCollection : BaseEntityCollection<FileEntity>
    {
        #region Constructors

        public FileCollection()
        {
        }

        public FileCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}