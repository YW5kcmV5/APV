using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class ImageCollection : BaseEntityCollection<ImageEntity>
    {
        #region Constructors

        public ImageCollection()
        {
        }

        public ImageCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}