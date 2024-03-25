using APV.EntityFramework;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class SupplierManagement : BaseManagement<SupplierEntity, SupplierCollection, SupplierDataLayerManager>
    {

        public static readonly SupplierManagement Instance = (SupplierManagement)EntityFrameworkManager.GetManagement<SupplierEntity>();
    }
}