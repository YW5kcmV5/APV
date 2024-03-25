using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class BusinessTypeManagement : BaseManagement<BusinessTypeEntity, BusinessTypeCollection, BusinessTypeDataLayerManager>
    {
        [AnonymousAccess]
        public override BusinessTypeEntity FindByName(string name)
        {
            return base.FindByName(name);
        }

        [AnonymousAccess]
        public override BusinessTypeEntity GetByName(string name)
        {
            return base.GetByName(name);
        }

        [AnonymousAccess]
        public override BusinessTypeCollection GetAll()
        {
            return base.GetAll();
        }

        public static readonly BusinessTypeManagement Instance = (BusinessTypeManagement)EntityFrameworkManager.GetManagement<BusinessTypeEntity>();

        public static readonly BusinessTypeEntity OOO = Instance.GetByName(Constants.BusinessTypeOOO);
    }
}