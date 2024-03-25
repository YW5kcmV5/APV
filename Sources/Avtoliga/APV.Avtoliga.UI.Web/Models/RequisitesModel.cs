using APV.Avtoliga.UI.Web.Controllers.API;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Models
{
    public sealed class RequisitesModel : BaseModel
    {
        public SearchMenuModel ToSeachMenu()
        {
            TrademarkInfo[] trademarks = SearchController.GetAllTrademarks();
            ProductGroupInfo[] groups = SearchController.GetAllProductGroups();

            return new SearchMenuModel
                {
                    Trademarks = trademarks,
                    Groups = groups,
                    ProductsInited = true,
                };
        }
    }
}