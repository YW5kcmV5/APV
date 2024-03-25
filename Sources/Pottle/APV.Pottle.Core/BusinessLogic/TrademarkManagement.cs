using APV.EntityFramework;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class TrademarkManagement : BaseManagement<TrademarkEntity, TrademarkCollection, TrademarkDataLayerManager>
    {

        public static readonly TrademarkManagement Instance = (TrademarkManagement)EntityFrameworkManager.GetManagement<TrademarkEntity>();
    }
}