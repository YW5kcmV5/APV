using APV.EntityFramework;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class SupplierProductManagement : BaseManagement<SupplierProductEntity, SupplierProductCollection, SupplierProductDataLayerManager>
    {

        public static readonly SupplierProductManagement Instance = (SupplierProductManagement)EntityFrameworkManager.GetManagement<SupplierProductEntity>();
    }
}