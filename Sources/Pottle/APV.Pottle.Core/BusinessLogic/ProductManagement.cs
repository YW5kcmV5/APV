using APV.EntityFramework;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class ProductManagement : BaseManagement<ProductEntity, ProductCollection, ProductDataLayerManager>
    {
        public static readonly ProductManagement Instance = (ProductManagement)EntityFrameworkManager.GetManagement<ProductEntity>();
    }
}