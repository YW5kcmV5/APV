using System;
using System.Collections.Generic;
using APV.EntityFramework.DataLayer;

namespace APV.Avtoliga.Core.Entities.Collection
{
    public class ProductCollection : BaseEntityCollection<ProductEntity>
    {
        #region Constructors

        public ProductCollection()
        {
        }

        public ProductCollection(BaseEntity owner)
            : base(owner)
        {
        }

        public ProductCollection(IEnumerable<ProductEntity> owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            AddRange(owner);
        }

        #endregion
    }
}