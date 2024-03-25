using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class ModelCollection : BaseEntityCollection<ModelEntity>
    {
        #region Constructors

        public ModelCollection()
        {
        }

        public ModelCollection(BaseEntity owner)
            : base(owner)
        {
        }

        #endregion
    }
}