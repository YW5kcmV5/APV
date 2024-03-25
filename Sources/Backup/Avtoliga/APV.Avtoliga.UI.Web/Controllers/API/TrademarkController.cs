using APV.Avtoliga.Core.BusinessLogic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.Avtoliga.UI.Web.Models.Entities;
using APV.Common.Extensions;

namespace APV.Avtoliga.UI.Web.Controllers.API
{
    public static class TrademarkController
    {
        public static TrademarkInfo Find(string trademarkId)
        {
            long trademarkIdValue = trademarkId.ToLong(0);
            TrademarkEntity trademark = TrademarkManagement.Instance.Find(trademarkIdValue);
            if (trademark != null)
            {
                ModelCollection models = ModelManagement.Instance.GetList(trademark);
                return trademark.Transform(true, models);
            }
            return null;
        }

        public static TrademarkInfo[] GetAll()
        {
            TrademarkCollection trademarks = TrademarkManagement.Instance.GetAll();
            return trademarks.Transform(false);
        }
    }
}