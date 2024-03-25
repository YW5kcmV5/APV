using APV.Avtoliga.UI.Web.Controllers.API;
using APV.Avtoliga.UI.Web.Models.Entities;

namespace APV.Avtoliga.UI.Web.Models
{
    public sealed class SearchModel
    {
        public TrademarkInfo Trademark { get; set; }

        public ModelInfo Model { get; set; }

        public ProducerInfo Producer { get; set; }

        public ProductGroupInfo Group { get; set; }

        public ProductInfo[] Products { get; set; }

        public bool ProductsInited { get; set; }

        public SearchMenuModel ToSeachMenu()
        {
            TrademarkInfo[] trademarks = SearchController.GetAllTrademarks();
            ProductGroupInfo[] groups = SearchController.GetAllProductGroups();

            return new SearchMenuModel
                {
                    TrademarkId = (Trademark != null) ? Trademark.TrademarkId : (long?) null,
                    ModelId = (Model != null) ? Model.ModelId : (long?) null,
                    Producer = Producer,
                    ProductGroupId = (Group != null) ? Group.ProductGroupId : ProductGroupInfo.All.ProductGroupId,
                    Trademarks = trademarks,
                    Groups = groups,
                    ProductsInited = ProductsInited,
                };
        }
    }
}